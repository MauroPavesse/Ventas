using Afip.WSFE;
using Mapster;
using System.Globalization;
using System.ServiceModel;
using Ventas.Application.Entities.AfipTokens;
using Ventas.Application.Entities.AfipTokens.DTOs;
using Ventas.Application.Entities.Configurations;
using Ventas.Application.Entities.Externas.Afip;
using Ventas.Application.Entities.Externas.Afip.DTOs;
using Ventas.Application.Entities.Externas.Encryption;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Domain.Entities;
using Ventas.Domain.Enums;

namespace Ventas.Infrastructure.Persistence.Services.Afip
{
    public class AfipService : IAfipService
    {
        private readonly IAfipAuthService _afipAuthService;
        private readonly IAfipTokenRepository _tokenRepository;
        private readonly IConfigurationRepository _configRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IEncryptionService encryptionService;

        public AfipService(IAfipAuthService afipAuthService, IAfipTokenRepository tokenRepository, IConfigurationRepository configRepository, IUnitOfWorkRepository unitOfWorkRepository, IEncryptionService encryptionService)
        {
            _afipAuthService = afipAuthService;
            _tokenRepository = tokenRepository;
            _configRepository = configRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
            this.encryptionService = encryptionService;
        }

        public async Task<AfipResponse> EmitInvoiceAsync(Voucher voucher)
        {
            // 1. Obtener Token (Lógica interna, el Handler ni se entera)
            var token = await GetValidTokenAsync(voucher.User!.PointOfSaleId ?? 0);

            // 2. Obtener CUIT de configuración
            var cuit = (await _configRepository.GetAllAsync(t => t.Variable == "cuit")).First().StringValue;

            // 3. Consultar último número y emitir (Tu lógica de WSFE que ya tenías)
            // ... (aquí va la llamada a AFIP usando el token obtenido)
            int lastVoucherNumber = await GetLastVoucherNumberAsync(token.Token, token.Sign, cuit, Convert.ToInt32(voucher.User!.PointOfSale!.Number), Convert.ToInt32(voucher.VoucherType!.Code));
            voucher.Number = lastVoucherNumber + 1;

            var auth = new FEAuthRequest
            {
                Token = token.Token,
                Sign = token.Sign,
                Cuit = long.Parse(cuit)
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
                        DocNro = customer == null || customer.TaxConditionId == (int)TaxConditionEnum.CONSUMIDOR_FINAL ? 0 : customer.TaxConditionId == (int)TaxConditionEnum.RESPONSABLE_INSCRIPTO ? Convert.ToInt64(customer.Cuit) : customer.Document,
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
                    if (det.Resultado == "R" && det.Observaciones != null)
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
                return AfipResponse.Ok(
                    resultado.CAE,
                    DateTime.ParseExact(
                        resultado.CAEFchVto,
                        "yyyyMMdd",
                        CultureInfo.InvariantCulture
                    ),
                    voucher.Number
                );
            }
            else
            {
                var errors = new List<AfipErrorOutput>();

                if(resultado.Observaciones != null)
                {
                    foreach (var obs in resultado.Observaciones)
                    {
                        errors.Add(new AfipErrorOutput
                        {
                            Code = obs.Code.ToString(),
                            Message = obs.Msg,
                            Source = "WSFE"
                        });
                    }
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

        private async Task<AfipTokenOutput> GetValidTokenAsync(int pointOfSaleId)
        {
            var token = await _tokenRepository.GetLatest(pointOfSaleId);

            if (token == null || token.Expiration <= DateTime.UtcNow.AddMinutes(-5))
            {
                // Lógica de lectura de certificados y pedido de nuevo token
                var configurations = await _configRepository.GetAllAsync(t => t.Variable == "arcaAlias" || t.Variable == "arcaCertificado" || t.Variable == "arcaClave");
                var pathAlias = configurations.First(t => t.Variable == "arcaAlias").StringValue;
                var pathCertificate = configurations.First(t => t.Variable == "arcaCertificado").StringValue;
                var pathPassword = configurations.First(t => t.Variable == "arcaClave").StringValue;

                pathPassword = encryptionService.Decrypt(pathPassword);

                var response = await _afipAuthService.GetToken(pathCertificate, pathPassword);

                if (response.Success)
                {
                    // Guardar en BD para la próxima vez
                    token = await _tokenRepository.CreateAsync(new AfipToken
                    {
                        Token = response.Data!.Token,
                        Sign = response.Data!.Sign,
                        Expiration = response.Data!.Expiration,
                        PointOfSaleId = pointOfSaleId
                    });
                    await _unitOfWorkRepository.SaveChangesAsync();
                }
                else
                {
                    throw new Exception(response.Errors.First().Message);
                }
            }

            return token.Adapt<AfipTokenOutput>();
        }
    }
}
