using Afip.WSAA;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using Ventas.Application.Entities.AfipTokens.DTOs;
using Ventas.Application.Entities.Externas.Afip;
using Ventas.Application.Entities.Externas.Afip.DTOs;

namespace Ventas.Infrastructure.Persistence.Services.Afip
{
    public class AfipAuthService : IAfipAuthService
    {
        public static string CreateTRA(string servicio)
        {
            var now = DateTimeOffset.Now;
            var genTime = now.AddMinutes(-10).ToString("yyyy-MM-ddTHH:mm:sszzz");
            var expTime = now.AddHours(12).ToString("yyyy-MM-ddTHH:mm:sszzz");
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
           "<loginTicketRequest version=\"1.0\">" +
           "<header>" +
           $"<uniqueId>{(int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()}</uniqueId>" +
           $"<generationTime>{genTime}</generationTime>" +
           $"<expirationTime>{expTime}</expirationTime>" +
           "</header>" +
           $"<service>{servicio}</service>" +
           "</loginTicketRequest>";
        }

        public static byte[] SignTRA(string traXml, string pathCert, string password)
        {
            var cert = X509CertificateLoader.LoadPkcs12FromFile(
                pathCert,
                password,
                X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.Exportable
            );
            var encoding = new UTF8Encoding(false); // False evita el BOM
            var traBytes = encoding.GetBytes(traXml);

            var contentInfo = new ContentInfo(
                new Oid("1.2.840.113549.1.7.1"),
                traBytes
            );

            var cms = new SignedCms(
                contentInfo,
                detached: false
            );

            var signer = new CmsSigner(
                SubjectIdentifierType.IssuerAndSerialNumber,
                cert
            )
            {
                //IncludeOption = X509IncludeOption.WholeChain,
                IncludeOption = X509IncludeOption.EndCertOnly,
                DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1")
            };

            cms.ComputeSignature(signer);
            var cmsBytes = cms.Encode();
            return cmsBytes;
        }

        public async Task<AfipResultOutput<AfipTokenOutput>> GetToken(string pathCert, string password)
        {
            var result = new AfipResultOutput<AfipTokenOutput>();

            try
            {
                var tra = CreateTRA("wsfe");
                if (tra.StartsWith("<?xml") == true)
                {
                    var caca = $"First char: {(int)tra[0]}";
                }
                Console.WriteLine(tra);
                var cms = SignTRA(tra, pathCert, password);

                var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport)
                {
                    Security = { Transport = { ClientCredentialType = HttpClientCredentialType.None } },
                    SendTimeout = TimeSpan.FromMinutes(2),
                    MaxReceivedMessageSize = 2000000
                };

                bool isProduction = false;
                string wsaaUrlProd = "https://wsaa.afip.gov.ar/ws/services/LoginCms";
                string wsaaUrlHomo = "https://wsaahomo.afip.gov.ar/ws/services/LoginCms";

                var wsaa = new LoginCMSClient(binding, 
                    new EndpointAddress(isProduction ? wsaaUrlProd : wsaaUrlHomo));

                var response = await wsaa.loginCmsAsync(Convert.ToBase64String(cms));

                var xml = XDocument.Parse(response.loginCmsReturn);
                //var xml = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n<loginTicketResponse version=\"1.0\">\r\n    <header>\r\n        <source>CN=wsaahomo, O=AFIP, C=AR, SERIALNUMBER=CUIT 33693450239</source>\r\n        <destination>SERIALNUMBER=CUIT 20430417356, CN=homologacion</destination>\r\n        <uniqueId>1984293740</uniqueId>\r\n        <generationTime>2025-12-18T13:49:42.319-03:00</generationTime>\r\n        <expirationTime>2025-12-19T01:49:42.319-03:00</expirationTime>\r\n    </header>\r\n    <credentials>\r\n        <token>PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8c3NvIHZlcnNpb249IjIuMCI+CiAgICA8aWQgc3JjPSJDTj13c2FhaG9tbywgTz1BRklQLCBDPUFSLCBTRVJJQUxOVU1CRVI9Q1VJVCAzMzY5MzQ1MDIzOSIgZHN0PSJDTj13c2ZlLCBPPUFGSVAsIEM9QVIiIHVuaXF1ZV9pZD0iNTU5NjM1NyIgZ2VuX3RpbWU9IjE3NjYwNzY1MjIiIGV4cF90aW1lPSIxNzY2MTE5NzgyIi8+CiAgICA8b3BlcmF0aW9uIHR5cGU9ImxvZ2luIiB2YWx1ZT0iZ3JhbnRlZCI+CiAgICAgICAgPGxvZ2luIGVudGl0eT0iMzM2OTM0NTAyMzkiIHNlcnZpY2U9IndzZmUiIHVpZD0iU0VSSUFMTlVNQkVSPUNVSVQgMjA0MzA0MTczNTYsIENOPWhvbW9sb2dhY2lvbiIgYXV0aG1ldGhvZD0iY21zIiByZWdtZXRob2Q9IjIyIj4KICAgICAgICAgICAgPHJlbGF0aW9ucz4KICAgICAgICAgICAgICAgIDxyZWxhdGlvbiBrZXk9IjIwNDMwNDE3MzU2IiByZWx0eXBlPSI0Ii8+CiAgICAgICAgICAgIDwvcmVsYXRpb25zPgogICAgICAgIDwvbG9naW4+CiAgICA8L29wZXJhdGlvbj4KPC9zc28+Cg==</token>\r\n        <sign>Muwtw4KR1bEUrdRvasaBPnJEERvXihcYfO7Y/QNNgzJu2V/TrlN8/fWKXqkcwF0zdWkzz+o46l7Kfjivf+c0Kvu7PVoW5UH3MhdRwu5zggL+PxcXf4RJWaBAPUun8sl7LO/faCMlLuXXT7GFOwGJA1xHQ6XvTzTzLzQ5MhdQJS0=</sign>\r\n    </credentials>\r\n</loginTicketResponse>");

                var expirationText = xml.Descendants("expirationTime").FirstOrDefault()?.Value;
                var token = xml.Descendants("token").FirstOrDefault()?.Value;
                var sign = xml.Descendants("sign").FirstOrDefault()?.Value;
                if (token == null || sign == null || expirationText == null)
                    throw new Exception("XML de AFIP inválido");

                result.Data = new AfipTokenOutput
                {
                    Token = token,
                    Sign = sign,
                    Expiration = DateTimeOffset.Parse(expirationText)
                };
            }
            catch (Exception ex)
            {
                result.Errors.Add(new AfipErrorOutput
                {
                    Code = "WSAA",
                    Message = ex.Message,
                    Source = "WSAA"
                });
            }

            return result;
        }
    }
}
