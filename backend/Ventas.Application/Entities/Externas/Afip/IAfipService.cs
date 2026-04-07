using Ventas.Application.Entities.Externas.Afip.DTOs;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Externas.Afip
{
    public interface IAfipService
    {
        Task<int> GetLastVoucherNumberAsync(string token, string sign, string businessCuit, int pointOfSaleNumber, int voucherTypeCode);
        Task<AfipResponse> EmitInvoiceAsync(string token, string sign, string businessCuit, Voucher voucher);
    }
}
