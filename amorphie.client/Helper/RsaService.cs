using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.IdentityModel.Tokens;


namespace amorphie.client
{
    public class RsaService
    {

        public (string, string) SaveKeys(RSA rsa)
        {
            var privateKey = rsa.ExportParameters(true); // Export with private keyvar publicKey = rsa.ExportParameters(false); // Export only the public key// Save private key
            var publicKey = rsa.ExportParameters(false);
            // Save public key

            return (SerializeKey(privateKey),SerializeKey(publicKey));
        }

        public RSA LoadPublicKey(string publicKeyRaw)
        {
            var rsa = RSA.Create();
            var publicKey = DeserializeKey(publicKeyRaw);
            rsa.ImportParameters(publicKey);
            return rsa;
        }

        private string SerializeKey(RSAParameters parameters)
        {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, parameters);
            return sw.ToString();
        }

        private RSAParameters DeserializeKey(string keyString)
        {
            var sr = new StringReader(keyString);
            var xs = new XmlSerializer(typeof(RSAParameters));
            return (RSAParameters)xs.Deserialize(sr)!;
        }

        public JwkResponse GetJwks(string publicKey)
        {

            var rsa = LoadPublicKey(publicKey);
            rsa.KeySize = 2048;


            var key = new RsaSecurityKey(rsa)

            {

                KeyId = Guid.NewGuid().ToString()

            };

            var parameters = key.Rsa.ExportParameters(false);

            var jwk = new JsonWebKeyModel

            {

                kty = "RSA",

                use = "sig",

                kid = key.KeyId,

                n = Base64UrlEncoder.Encode(parameters.Modulus),

                e = Base64UrlEncoder.Encode(parameters.Exponent)

            };

            var jwks = new JwkResponse

            {

                keys = new List<JsonWebKeyModel> { jwk }

            };

            return jwks;
        }
    }

    public class JwkResponse
    {
        public List<JsonWebKeyModel> keys { get; set; }
    }

    public class JsonWebKeyModel
    {
        public string kty { get; set; }

        public string use { get; set; }

        public string kid { get; set; }

        public string n { get; set; }

        public string e { get; set; }
    }
}
