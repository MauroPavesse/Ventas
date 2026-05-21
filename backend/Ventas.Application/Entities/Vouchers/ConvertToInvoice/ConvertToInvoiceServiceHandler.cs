using Mapster;
using MediatR;
using Ventas.Application.Entities.Externas.Afip;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.Vouchers.DTOs;
using Ventas.Application.Entities.VoucherTypes;
using Ventas.Domain.Common;

namespace Ventas.Application.Entities.Vouchers.ConvertToInvoice
{
    public record ConvertToInvoiceServiceCommand(int VoucherId) : IRequest<Result<VoucherOutput>>;

    public class ConvertToInvoiceServiceHandler : IRequestHandler<ConvertToInvoiceServiceCommand, Result<VoucherOutput>>
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IAfipService _afipService;
        private readonly IVoucherTypeRepository _voucherTypeRepository;

        public ConvertToInvoiceServiceHandler(IUnitOfWorkRepository unitOfWorkRepository, IVoucherRepository voucherRepository, IAfipService afipService, IVoucherTypeRepository voucherTypeRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _voucherRepository = voucherRepository;
            _afipService = afipService;
            _voucherTypeRepository = voucherTypeRepository;
        }

        public async Task<Result<VoucherOutput>> Handle(ConvertToInvoiceServiceCommand request, CancellationToken cancellationToken)
        {
            // 1. Buscar el comprobante con sus datos relacionados
            var voucher = (await _voucherRepository.SearchAsync(
                predicate: t => t.Id == request.VoucherId,
                includesString: ["Customer", "User.PointOfSale", "VoucherDetails.Product.TaxRate", "VoucherType"]
            )).FirstOrDefault();
            if (voucher == null) return Result<VoucherOutput>.Failure("Comprobante no encontrado");

            // 2. Aplicar lógica de negocio (Dominio)
            voucher.PrepareForAfip();

            var voucherType = (await _voucherTypeRepository.SearchAsync(
                predicate: t => t.Id == voucher.VoucherTypeId
            )).First();
            voucher.VoucherType = voucherType;

            // 3. Comunicar con AFIP (Infraestructura encapsulada)
            var afipResponse = await _afipService.EmitInvoiceAsync(voucher);

            if (!afipResponse.Success)
            {
                string errorMessages = string.Join("; ", afipResponse.Errors.Select(e => $"{e.Code}: {e.Message}"));
                return Result<VoucherOutput>.Failure(errorMessages);
            }

            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo argTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Argentina/Buenos_Aires");
            DateTime fechaArgentina = TimeZoneInfo.ConvertTimeFromUtc(utcNow, argTimeZone);
            // 4. Actualizar datos fiscales en el objeto y persistir
            voucher.SetFiscalData(afipResponse.Cae!, afipResponse.CaeExpiration ?? fechaArgentina, afipResponse.Number);

            await _voucherRepository.UpdateAsync(voucher);
            await _unitOfWorkRepository.SaveChangesAsync();

            return Result<VoucherOutput>.Success(voucher.Adapt<VoucherOutput>());
        }
    }
}
