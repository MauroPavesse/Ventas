using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.Vouchers;
using Ventas.Domain.Entities;
using Ventas.Domain.Enums;

namespace Ventas.Application.Entities.Statistics.GetSalesAmount
{
    public record StatisticsGetSalesAmountOutput(decimal CurrentSales, decimal PercentageChange, bool IsPositive, int TotalOrders, decimal AverageTicket);

    public record StatisticsGetSalesAmountCommand(DateTime DateFrom, DateTime DateTo) : IRequest<StatisticsGetSalesAmountOutput>;

    public class StatisticsGetSalesAmountHandler : IRequestHandler<StatisticsGetSalesAmountCommand, StatisticsGetSalesAmountOutput>
    {
        private readonly IVoucherRepository _voucherRepository;

        public StatisticsGetSalesAmountHandler(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<StatisticsGetSalesAmountOutput> Handle(StatisticsGetSalesAmountCommand request, CancellationToken cancellationToken)
        {
            // 1. Forzamos que los rangos abarquen todo el día sin usar .Date en la BD
            DateTime dateFrom = request.DateFrom.Date;
            DateTime dateTo = request.DateTo.Date.AddDays(1).AddTicks(-1); // Esto lleva el DateTo a las 23:59:59.999

            // Consulta período actual
            Expression<Func<Voucher, bool>> predicate = v =>
                v.Deleted == 0 &&
                v.StateEntityId == (int)StateEntityEnum.VoucherStateEnum.FINALIZADO &&
                v.DateCreation >= dateFrom &&
                v.DateCreation <= dateTo;

            var vouchers = await _voucherRepository.SearchAsync(predicate, [""]);
            decimal currentSales = vouchers.Sum(v => v.AmountNet + v.AmountVAT);

            // 2. Cálculo del período de comparación
            int numberOfDays = (dateTo.Date - dateFrom.Date).Days;

            DateTime dateFromComparation = dateFrom.AddDays((numberOfDays + 1) * -1);
            DateTime dateToComparation = dateFrom.AddTicks(-1); // El último segundo antes de que empiece el período actual

            // Consulta período anterior
            Expression<Func<Voucher, bool>> predicateComparation = v =>
                v.Deleted == 0 &&
                v.StateEntityId == (int)StateEntityEnum.VoucherStateEnum.FINALIZADO &&
                v.DateCreation >= dateFromComparation &&
                v.DateCreation <= dateToComparation;

            var vouchersComparation = await _voucherRepository.SearchAsync(predicateComparation, [""]);
            decimal comparationSales = vouchersComparation.Sum(v => v.AmountNet + v.AmountVAT);

            // 3. Cálculo del porcentaje a salvo de divisiones por cero
            decimal percentage = 0;
            bool isPositive = true;

            if (comparationSales > 0)
            {
                // Fórmula estándar: ((Actual - Anterior) / Anterior) * 100
                percentage = ((currentSales - comparationSales) / comparationSales) * 100;
                isPositive = percentage >= 0;
            }
            else if (comparationSales == 0 && currentSales > 0)
            {
                // Si antes era 0 y ahora vendiste algo, el crecimiento es técnicamente del 100%
                percentage = 100;
                isPositive = true;
            }
            // Si ambos son 0, el porcentaje se queda en 0 y es positivo (neutral)

            int totalOrders = vouchers.Count();
            decimal averageTicket = totalOrders > 0 ? currentSales / totalOrders : 0;

            return new StatisticsGetSalesAmountOutput(
                CurrentSales: currentSales,
                PercentageChange: Math.Round(Math.Abs(percentage), 2), // Usamos Math.Abs para asegurarnos de enviar el número siempre positivo al front
                IsPositive: isPositive,
                AverageTicket: averageTicket,
                TotalOrders: totalOrders
            );
        }
    }
}