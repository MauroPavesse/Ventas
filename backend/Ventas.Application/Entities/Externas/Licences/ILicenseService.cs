namespace Ventas.Application.Entities.Externas.Licences
{
    public interface ILicenseService
    {
        Task<bool> IsLicenseValidAsync(string domain);
    }
}
