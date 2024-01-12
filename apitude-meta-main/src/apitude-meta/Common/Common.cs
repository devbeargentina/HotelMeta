using System.Security.Cryptography;
using System.Text;
using apitude_meta.Repositories;

namespace apitude_meta.Common
{
    public class Common
    {
        public static async Task<DXHttpResponse> GetResponse(string request, string serviceName, string httpMethod)
        {
            try
            {
                var headers = new Dictionary<string, string>
                    {
                        {"Content-Type","text/xml"}
                    };

                headers.Add("SOAPAction", "http://www.juniper.es/webservice/2007/" + serviceName);
                //var xSignature = sha256_hash(Environment.GetApiKey() + Environment.GetApiSecreat() + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());
                //headers.Add("X-Signature", xSignature);

                request = request.Replace("##Username##", Environment.GetApiUsername()).Replace("##Password##", Environment.GetAPiPassword());
                
                var response = await HttpRepository.Instance.ExecuteRequest(new DXHttpRequest
                {
                    Url = GetServiceEndPoint(serviceName),
                    Headers = headers,
                    RequestType = httpMethod,
                    Timeout = -1,
                    Body = request
                });
                return response;
            }
            catch (Exception e)
            {
                //Log.LogRequest(e.Message);
            }
            return new DXHttpResponse();
        }
        public static String sha256_hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
        private static string GetServiceEndPoint(string serviceName)
        {
            return Environment.GetSharedServiceUrl();// + serviceName;
        }
    }
}