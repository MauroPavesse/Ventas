using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.VoucherPayments.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.VoucherPayments.Search
{
    public record VoucherPaymentSearchCommand(SearchCommand Search) : IRequest<IEnumerable<VoucherPaymentOutput>>;

    public class VoucherPaymentSearchHandler : IRequestHandler<VoucherPaymentSearchCommand, IEnumerable<VoucherPaymentOutput>>
    {
        private readonly IVoucherPaymentRepository _voucherPaymentRepository;

        public VoucherPaymentSearchHandler(IVoucherPaymentRepository voucherPaymentRepository)
        {
            _voucherPaymentRepository = voucherPaymentRepository;
        }

        public async Task<IEnumerable<VoucherPaymentOutput>> Handle(VoucherPaymentSearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<VoucherPayment, bool>> predicate = t => t.Deleted == 0;

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

            var voucherPayments = await _voucherPaymentRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return voucherPayments.Adapt<IEnumerable<VoucherPaymentOutput>>();
        }
    }
}
