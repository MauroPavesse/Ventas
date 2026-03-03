using Mapster;
using MediatR;
using Ventas.Application.Entities.PointOfSaleVoucherTypes.DTOs;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.PointOfSaleVoucherTypes.Create
{
    public record PointOfSaleVoucherTypeCreateCommand(int PointOfSaleId, int VoucherTypeId, int Numeration) : IRequest<PointOfSaleVoucherTypeOutput>;

    public class PointOfSaleVoucherTypeCreateHandler : IRequestHandler<PointOfSaleVoucherTypeCreateCommand, PointOfSaleVoucherTypeOutput>
    {
        private readonly IPointOfSaleVoucherTypeRepository _pointOfSaleVoucherTypeRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public PointOfSaleVoucherTypeCreateHandler(IPointOfSaleVoucherTypeRepository pointOfSaleVoucherTypeRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _pointOfSaleVoucherTypeRepository = pointOfSaleVoucherTypeRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<PointOfSaleVoucherTypeOutput> Handle(PointOfSaleVoucherTypeCreateCommand request, CancellationToken cancellationToken)
        {
            var pointOfSaleVoucherType = await _pointOfSaleVoucherTypeRepository.CreateAsync(request.Adapt<PointOfSaleVoucherType>());
            await _unitOfWorkRepository.SaveChangesAsync();
            return pointOfSaleVoucherType.Adapt<PointOfSaleVoucherTypeOutput>();
        }
    }
}
