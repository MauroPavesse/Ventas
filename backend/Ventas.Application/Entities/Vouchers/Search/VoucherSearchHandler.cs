using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.Vouchers.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;
using Ventas.Domain.Enums;

namespace Ventas.Application.Entities.Vouchers.Search
{
    public record VoucherSearchCommand(SearchCommand Search) : IRequest<IEnumerable<VoucherOutput>>;

    public class VoucherSearchHandler : IRequestHandler<VoucherSearchCommand, IEnumerable<VoucherOutput>>
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherSearchHandler(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<IEnumerable<VoucherOutput>> Handle(VoucherSearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<Voucher, bool>> predicate = t => t.Deleted == 0;

            if (search.Id > 0)
            {
                predicate = t => t.Id == search.Id;
            }
            else
            {
                var sinCajaDiaria = search.Filters.FirstOrDefault(t => t.Field == "SinCajaDiaria");
                if (sinCajaDiaria != null)
                {
                    predicate = predicate.And(t => t.DailyBoxId == null && t.StateEntityId == (int)StateEntityEnum.VoucherStateEnum.FINALIZADO);
                }
            }

            var vouchers = await _voucherRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return vouchers.Adapt<IEnumerable<VoucherOutput>>();
        }
    }
}
