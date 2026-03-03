using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.VoucherTypes.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.VoucherTypes.Search
{
    public record VoucherTypeSearchCommand(SearchCommand Search) : IRequest<IEnumerable<VoucherTypeOutput>>;

    public class VoucherTypeSearchHandler : IRequestHandler<VoucherTypeSearchCommand, IEnumerable<VoucherTypeOutput>>
    {
        private readonly IVoucherTypeRepository _voucherTypeRepository;

        public VoucherTypeSearchHandler(IVoucherTypeRepository voucherTypeRepository)
        {
            _voucherTypeRepository = voucherTypeRepository;
        }

        public async Task<IEnumerable<VoucherTypeOutput>> Handle(VoucherTypeSearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<VoucherType, bool>> predicate = t => t.Deleted == 0;

            if (search.Id > 0)
            {
                predicate = t => t.Id == search.Id;
            }

            var voucherTypes = await _voucherTypeRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return voucherTypes.Adapt<IEnumerable<VoucherTypeOutput>>();
        }
    }
}
