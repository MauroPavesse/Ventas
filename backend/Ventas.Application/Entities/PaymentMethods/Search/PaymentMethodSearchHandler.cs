using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.PaymentMethods.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.PaymentMethods.Search
{
    public record PaymentMethodSearchCommand(SearchCommand Search) : IRequest<IEnumerable<PaymentMethodOutput>>;

    public class PaymentMethodSearchHandler : IRequestHandler<PaymentMethodSearchCommand, IEnumerable<PaymentMethodOutput>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public PaymentMethodSearchHandler(IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<IEnumerable<PaymentMethodOutput>> Handle(PaymentMethodSearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<PaymentMethod, bool>> predicate = t => t.Deleted == 0;

            if (search.Id > 0)
            {
                predicate = t => t.Id == search.Id;
            }

            var paymentMethods = await _paymentMethodRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return paymentMethods.Adapt<IEnumerable<PaymentMethodOutput>>();
        }
    }
}
