using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.Vouchers;
using Ventas.Domain.Entities;
using Ventas.Domain.Enums;

namespace Ventas.Application.Entities.Statistics.TopProducts
{
    public record StatisticsTopProductsOutput(int Id, string Name, decimal Quantity, decimal TotalAmount);

    public record StatisticsTopProductsCommand(DateTime DateFrom, DateTime DateTo) : IRequest<List<StatisticsTopProductsOutput>>;

    public class StatisticsTopProductsHandler : IRequestHandler<StatisticsTopProductsCommand, List<StatisticsTopProductsOutput>>
    {
        private readonly IVoucherRepository _voucherRepository;

        public StatisticsTopProductsHandler(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<List<StatisticsTopProductsOutput>> Handle(StatisticsTopProductsCommand request, CancellationToken cancellationToken)
        {
            // 1. Forzamos que los rangos abarquen todo el día sin usar .Date en la BD
            DateTime dateFrom = request.DateFrom.Date;
            DateTime dateTo = request.DateTo.Date.AddDays(1).AddTicks(-1); // Esto lleva el DateTo a las 23:59:59.999

            Expression<Func<Voucher, bool>> predicate = v =>
                v.Deleted == 0 &&
                v.StateEntityId == (int)StateEntityEnum.VoucherStateEnum.FINALIZADO &&
                v.DateCreation >= dateFrom &&
                v.DateCreation <= dateTo;

            // 2. Filtrar los vouchers en el rango de fechas
            var vouchers = await _voucherRepository.SearchAsync(predicate, ["VoucherDetails.Product"]);

            // 3. Agrupar y procesar en memoria
            var topProducts = vouchers
                .SelectMany(v => v.VoucherDetails) // Abres los detalles de todos los vouchers filtrados
                .GroupBy(d => new { d.ProductId, d.Product!.Name }) // Agrupas por producto
                .Select(g => new StatisticsTopProductsOutput(
                
                    Id: g.Key.ProductId,
                    Name: g.Key.Name,
                    Quantity: g.Sum(d => d.Quantity), // Sumas las unidades vendidas
                    TotalAmount: g.Sum(d => d.AmountFinal) // Sumas el dinero recaudado por ese producto
                ))
                .OrderByDescending(p => p.Quantity) // Ordenas de mayor a menor cantidad
                .Take(5) // Te quedas solo con los primeros 5
                .ToList();

            return topProducts;
        }
    }
}
