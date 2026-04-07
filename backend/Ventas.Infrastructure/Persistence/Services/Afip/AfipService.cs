using Afip.WSFE;
using System.Globalization;
using System.ServiceModel;
using Ventas.Application.Entities.Externas.Afip;
using Ventas.Application.Entities.Externas.Afip.DTOs;
using Ventas.Domain.Entities;
using Ventas.Domain.Enums;

namespace Ventas.Infrastructure.Persistence.Services.Afip
{
    public class AfipService : IAfipService
    {
        public async Task<AfipResponse> EmitInvoiceAsync(string token, string sign, string businessCuit, Voucher voucher)
        {
            var auth = new FEAuthRequest
            {
                Token = token,
                Sign = sign,
                Cuit = long.Parse(businessCuit)
            };

            var voucherDetailsGroup = voucher.VoucherDetails.GroupBy(t => t.Product!.TaxRateId).ToList();

            List<AlicIva> ivas = [];
            foreach (var voucherDetail in voucherDetailsGroup)
            {
                ivas.Add(new AlicIva()
                {
                    Id = Convert.ToInt32(voucherDetail.First().Product!.TaxRate!.Code),
                    BaseImp = Convert.ToDouble(voucherDetail.Sum(t => t.AmountNet)),
                    Importe = Convert.ToDouble(voucherDetail.Sum(t => t.AmountFinal - t.AmountNet + t.Discount))
                });
            }

            var customer = voucher.Customer;

            var req = new FECAERequest
            {
                FeCabReq = new FECAECabRequest
                {
                    CantReg = 1,
                    PtoVta = Convert.ToInt32(voucher.User!.PointOfSale!.Number),
                    CbteTipo = Convert.ToInt32(voucher.VoucherType!.Code)
                },
                FeDetReq =
                [
                    new FECAEDetRequest
                    {
                        Concepto = 1,
                        DocTipo = customer == null || customer.TaxConditionId == (int)TaxConditionEnum.CONSUMIDOR_FINAL ? 99 : customer.TaxConditionId == (int)TaxConditionEnum.RESPONSABLE_INSCRIPTO ? 80 : 96,
                        DocNro = customer == null || customer.TaxConditionId == (int)TaxConditionEnum.CONSUMIDOR_FINAL ? 0 : customer.Document,
                        CbteDesde = voucher.Number,
                        CbteHasta = voucher.Number,
                        CbteFch = DateTime.Now.ToString("yyyyMMdd"),
                        ImpTotal = Convert.ToDouble(voucher.AmountNet + voucher.AmountVAT),
                        ImpNeto = Convert.ToDouble(voucher.AmountNet),
                        ImpIVA = Convert.ToDouble(voucher.AmountVAT),
                        MonId = "PES",
                        MonCotiz = 1,
                        Iva = voucher.AmountVAT > 0 ? [.. ivas] : [],
                        CondicionIVAReceptorId = customer != null && customer.TaxConditionId == (int)TaxConditionEnum.RESPONSABLE_INSCRIPTO ? 1 : 4
                    }
                ]
            };

            var wsfe = CreateClientWsfe();
            var result = new AfipResultOutput<FECAEResponse>();

            try
            {
                var response = (await wsfe.FECAESolicitarAsync(auth, req)).Body.FECAESolicitarResult;

                if (response.Errors != null)
                {
                    foreach (var err in response.Errors)
                    {
                        result.Errors.Add(new AfipErrorOutput
                        {
                            Code = err.Code.ToString(),
                            Message = err.Msg,
                            Source = "WSFE"
                        });
                    }
                }

                var det = response.FeDetResp?.FirstOrDefault();

                if (det != null)
                {
                    if (det.Resultado == "R")
                    {
                        foreach (var obs in det.Observaciones)
                        {
                            result.Errors.Add(new AfipErrorOutput
                            {
                                Code = obs.Code.ToString(),
                                Message = obs.Msg,
                                Source = "WSFE"
                            });
                        }
                    }
                }

                if (result.Errors.Count == 0)
                    result.Data = response;
            }
            catch (Exception ex)
            {
                result.Errors.Add(new AfipErrorOutput
                {
                    Code = "WSFE",
                    Message = ex.Message,
                    Source = "WSFE"
                });
            }

            if (!result.Success)
            {
                return AfipResponse.Fail(result.Errors);
            }

            var resultado = result.Data!.FeDetResp![0];

            if (resultado.Resultado == "A")
            {
                string cae = resultado.CAE;
                string vencimiento = resultado.CAEFchVto;

                voucher.CAE = cae;
                voucher.CAEExpiration = DateTime.ParseExact(vencimiento, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                return AfipResponse.Ok(
                    resultado.CAE,
                    DateTime.ParseExact(
                        resultado.CAEFchVto,
                        "yyyyMMdd",
                        CultureInfo.InvariantCulture
                    )
                );
            }
            else
            {
                var errors = new List<AfipErrorOutput>();

                foreach (var obs in resultado.Observaciones)
                {
                    errors.Add(new AfipErrorOutput
                    {
                        Code = obs.Code.ToString(),
                        Message = obs.Msg,
                        Source = "WSFE"
                    });
                }

                return AfipResponse.Fail(errors);
            }
        }

        private ServiceSoapClient CreateClientWsfe()
        {
            var client = new ServiceSoapClient(
                ServiceSoapClient.EndpointConfiguration.ServiceSoap);

            bool isProduction = false;
            string wsfeUrlHomo = "https://wswhomo.afip.gov.ar/wsfev1/service.asmx";
            string wsfeUrlProd = "https://wswh.afip.gov.ar/wsfev1/service.asmx";


            client.Endpoint.Address = new EndpointAddress(
                isProduction
                    ? wsfeUrlProd
                    : wsfeUrlHomo
            );

            return client;
        }

        public async Task<int> GetLastVoucherNumberAsync(string token, string sign, string businessCuit, int pointOfSaleNumber, int voucherTypeCode)
        {
            var auth = new FEAuthRequest
            {
                Token = token,
                Sign = sign,
                Cuit = long.Parse(businessCuit)
            };

            var wsfe = CreateClientWsfe();

            var resp = await wsfe.FECompUltimoAutorizadoAsync(auth, pointOfSaleNumber, voucherTypeCode);
            return resp.Body.FECompUltimoAutorizadoResult.CbteNro;
        }
    }
}
