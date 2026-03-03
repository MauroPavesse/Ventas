using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.VoucherDetails.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.VoucherDetails.Search
{
    public record VoucherDetailSearchCommand(SearchCommand Search) : IRequest<IEnumerable<VoucherDetailOutput>>;

    public class VoucherDetailSearchHandler : IRequestHandler<VoucherDetailSearchCommand, IEnumerable<VoucherDetailOutput>>
    {
        private readonly IVoucherDetailRepository _voucherDetailRepository;

        public VoucherDetailSearchHandler(IVoucherDetailRepository voucherDetailRepository)
        {
            _voucherDetailRepository = voucherDetailRepository;
        }

        public async Task<IEnumerable<VoucherDetailOutput>> Handle(VoucherDetailSearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<VoucherDetail, bool>> predicate = t => t.Deleted == 0;

            if (search.Id > 0)
            {
                predicate = t => t.Id == search.Id;
            }
            else
            {
                var voucherIdFilter = search.Filters.FirstOrDefault(t => t.Field == "VoucherId");
                if (voucherIdFilter != null)
                {
                    predicate = predicate.And(t => t.VoucherId == Convert.ToInt32(voucherIdFilter.Value));
                }
            }

            var voucherDetails = await _voucherDetailRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return voucherDetails.Adapt<IEnumerable<VoucherDetailOutput>>();
        }
    }
