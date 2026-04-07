using Mapster;
using MediatR;
using Ventas.Application.Entities.AfipTokens;
using Ventas.Application.Entities.AfipTokens.DTOs;
using Ventas.Application.Entities.Configurations;
using Ventas.Application.Entities.Externas.Afip;
using Ventas.Application.Entities.PointOfSaleVoucherTypes;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.Vouchers.DTOs;
using Ventas.Application.Entities.VoucherTypes;
using Ventas.Domain.Entities;
using Ventas.Domain.Enums;

namespace Ventas.Application.Entities.Vouchers.ConvertToInvoice
{
    public record ConvertToInvoiceServiceCommand(int VoucherId) : IRequest<VoucherOutput>;

    public class ConvertToInvoiceServiceHandler : IRequestHandler<ConvertToInvoiceServiceCommand, VoucherOutput>
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IAfipTokenRepository _afipTokenRepository;
        private readonly IAfipAuthService _afipAuthService;
        private readonly IPointOfSaleVoucherTypeRepository _pointOfSaleVoucherTypeRepository;
        private readonly IAfipService _afipService;
        private readonly IVoucherTypeRepository _voucherTypeRepository;

        public ConvertToInvoiceServiceHandler(IUnitOfWorkRepository unitOfWorkRepository, IVoucherRepository voucherRepository, IConfigurationRepository configurationRepository, IAfipTokenRepository afipTokenRepository, IAfipAuthService afipAuthService, IPointOfSaleVoucherTypeRepository pointOfSaleVoucherTypeRepository, IAfipService afipService, IVoucherTypeRepository voucherTypeRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _voucherRepository = voucherRepository;
            _configurationRepository = configurationRepository;
            _afipTokenRepository = afipTokenRepository;
            _afipAuthService = afipAuthService;
            _pointOfSaleVoucherTypeRepository = pointOfSaleVoucherTypeRepository;
            _afipService = afipService;
            _voucherTypeRepository = voucherTypeRepository;
        }

        public async Task<VoucherOutput> Handle(ConvertToInvoiceServiceCommand request, CancellationToken cancellationToken)
        {
            var voucher = (await _voucherRepository.SearchAsync(
                predicate: t => t.Id == request.VoucherId,
                includesString: ["Customer", "User.PointOfSale", "VoucherDetails.Product.TaxRate", "VoucherType"]
            )).First();

            var configurations = await _configurationRepository.GetAllAsync(t => t.Variable == "arcaAlias" || t.Variable == "arcaCertificado" || t.Variable == "arcaClave" || t.Variable == "cuit");

            var pathAlias = configurations.First(t => t.Variable == "arcaAlias").StringValue;
            var pathCertificate = configurations.First(t => t.Variable == "arcaCertificado").StringValue;
            var pathPassword = configurations.First(t => t.Variable == "arcaClave").StringValue;
            var businessCuit = configurations.First(t => t.Variable == "cuit").StringValue;

            var voucherTypeEnum = (voucher.Customer != null && voucher.Customer.TaxConditionId == (int)TaxConditionEnum.RESPONSABLE_INSCRIPTO)
                      ? VoucherTypeEnum.FACTURA_A
                      : VoucherTypeEnum.FACTURA_B;

            var pointOfSale = voucher.User!.PointOfSale!;
            var afipToken = (await _afipTokenRepository.GetLatest(pointOfSale.Id)).Adapt<AfipTokenOutput>();

            if(afipToken == null || afipToken.Expiration <= DateTime.UtcNow.AddMinutes(-5))
            {
                var responseToken = await _afipAuthService.GetToken(pathCertificate, pathPassword);
                if (!responseToken.Success)
                {
                    throw new Exception("Error al obtener el token de AFIP: " + responseToken.Errors.First().Message);
                }
                afipToken = responseToken.Data!;

                afipToken = (await _afipTokenRepository.CreateAsync(new AfipToken()
                {
                    Token = afipToken.Token,
                    Sign = afipToken.Sign,
                    Expiration = afipToken.Expiration,
                    PointOfSaleId = pointOfSale.Id
                })).Adapt<AfipTokenOutput>();
            }

            var voucherType = await _voucherTypeRepository.GetByIdAsync((int)voucherTypeEnum);
            voucher.VoucherType = voucherType;

            int lastVoucherNumber = await _afipService.GetLastVoucherNumberAsync(afipToken.Token, afipToken.Sign, businessCuit, pointOfSale.Id, Convert.ToInt32(voucherType!.Code));
            voucher.Number = lastVoucherNumber + 1;

            var pointOfSaleVoucherType = (await _pointOfSaleVoucherTypeRepository.SearchAsync(t => t.PointOfSaleId == pointOfSale.Id && t.VoucherTypeId == voucherType.Id)).FirstOrDefault();
            if (pointOfSaleVoucherType == null)
            {
                pointOfSaleVoucherType = await _pointOfSaleVoucherTypeRepository.CreateAsync(new PointOfSaleVoucherType()
                {
                    PointOfSaleId = pointOfSale.Id,
                    VoucherTypeId = (int)voucherTypeEnum,
                    Numeration = lastVoucherNumber + 1
                });
            }

            var afipResponse = await _afipService.EmitInvoiceAsync(afipToken.Token, afipToken.Sign, businessCuit, voucher);
            if (afipResponse.Success)
            {
                voucher.CAE = afipResponse.Cae!;
                voucher.CAEExpiration = afipResponse.CaeExpiration;
                await _voucherRepository.UpdateAsync(voucher);

                pointOfSaleVoucherType.Numeration = voucher.Number;
                await _pointOfSaleVoucherTypeRepository.UpdateAsync(pointOfSaleVoucherType);

                await _unitOfWorkRepository.SaveChangesAsync();
            }

            return voucher.Adapt<VoucherOutput>();
        }
    }
}
