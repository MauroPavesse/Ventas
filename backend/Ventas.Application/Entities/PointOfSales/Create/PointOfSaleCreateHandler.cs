using Mapster;
using MediatR;
using Ventas.Application.Entities.PointOfSales.DTOs;
using Ventas.Application.Entities.PointOfSaleVoucherTypes;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Domain.Entities;
using Ventas.Domain.Enums;

namespace Ventas.Application.Entities.PointOfSales.Create
{
    public record PointOfSaleCreateCommand(string Name, string Number, string Address, string City, string Provincie, string PostalCode) : IRequest<PointOfSaleOutput>;

    public class PointOfSaleCreateHandler : IRequestHandler<PointOfSaleCreateCommand, PointOfSaleOutput>
    {
        private readonly IPointOfSaleRepository _pointOfSaleRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IPointOfSaleVoucherTypeRepository _pointOfSaleVoucherTypeRepository;

        public PointOfSaleCreateHandler(IPointOfSaleRepository pointOfSaleRepository, IUnitOfWorkRepository unitOfWorkRepository, IPointOfSaleVoucherTypeRepository pointOfSaleVoucherTypeRepository)
        {
            _pointOfSaleRepository = pointOfSaleRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
            _pointOfSaleVoucherTypeRepository = pointOfSaleVoucherTypeRepository;
        }

        public async Task<PointOfSaleOutput> Handle(PointOfSaleCreateCommand request, CancellationToken cancellationToken)
        {
            var pointOfSale = await _pointOfSaleRepository.CreateAsync(request.Adapt<PointOfSale>());
            await _unitOfWorkRepository.SaveChangesAsync();

            await _pointOfSaleVoucherTypeRepository.CreateAsync(new PointOfSaleVoucherType()
            {
                PointOfSaleId = pointOfSale.Id,
                VoucherTypeId = (int)VoucherTypeEnum.ORDEN_DE_COMPRA,
                Numeration = 0
            });

            await _pointOfSaleVoucherTypeRepository.CreateAsync(new PointOfSaleVoucherType()
            {
                PointOfSaleId = pointOfSale.Id,
                VoucherTypeId = (int)VoucherTypeEnum.FACTURA_A,
                Numeration = 0
            });

            await _pointOfSaleVoucherTypeRepository.CreateAsync(new PointOfSaleVoucherType()
            {
                PointOfSaleId = pointOfSale.Id,
                VoucherTypeId = (int)VoucherTypeEnum.RECIBO_A,
                Numeration = 0
            });

            await _pointOfSaleVoucherTypeRepository.CreateAsync(new PointOfSaleVoucherType()
            {
                PointOfSaleId = pointOfSale.Id,
                VoucherTypeId = (int)VoucherTypeEnum.FACTURA_B,
                Numeration = 0
            });

            await _pointOfSaleVoucherTypeRepository.CreateAsync(new PointOfSaleVoucherType()
            {
                PointOfSaleId = pointOfSale.Id,
                VoucherTypeId = (int)VoucherTypeEnum.RECIBO_B,
                Numeration = 0
            });
            await _unitOfWorkRepository.SaveChangesAsync();

            return pointOfSale.Adapt<PointOfSaleOutput>();
        }
    }
}
