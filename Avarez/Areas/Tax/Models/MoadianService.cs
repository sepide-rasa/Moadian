using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JWT.Algorithms;
using JWT.Builder;
using Jose;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace Avarez.Areas.Tax.Models
{
    public class NonceResponse
    {
        [JsonPropertyName("nonce")]
        public string Nonce { get; set; }

        [JsonPropertyName("expDate")]
        public string ExpDate { get; set; }
    }

    public class ServerInfoResponse
    {
        [JsonPropertyName("serverTime")]
        public long ServerTime { get; set; }

        [JsonPropertyName("publicKeys")]
        public List<PublicKey> PublicKeys { get; set; }
    }

    public class PublicKey
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("algorithm")]
        public string Algorithm { get; set; }

        [JsonPropertyName("purpose")]
        public int Purpose { get; set; }
    }

    public class InvoiceHeader
    {
        [JsonPropertyName("taxid")]
        public string TaxId { get; set; }

        [JsonPropertyName("inno")]
        public string Inno { get; set; }

        [JsonPropertyName("indatim")]
        public long Indatim { get; set; }

        [JsonPropertyName("inty")]
        public int Inty { get; set; }

        [JsonPropertyName("inp")]
        public int Inp { get; set; }

        [JsonPropertyName("ins")]
        public int Ins { get; set; }

        [JsonPropertyName("tins")]
        public string Tins { get; set; }

        [JsonPropertyName("tob")]
        public int Tob { get; set; }

        [JsonPropertyName("bid")]
        public string Bid { get; set; }

        [JsonPropertyName("tinb")]
        public string Tinb { get; set; }

        [JsonPropertyName("tbill")]
        public long Tbill { get; set; }

        [JsonPropertyName("setm")]
        public int Setm { get; set; }
    }

    public class InvoiceBody
    {
        [JsonPropertyName("sstid")]
        public string Sstid { get; set; }

        [JsonPropertyName("sstt")]
        public string Sstt { get; set; }

        [JsonPropertyName("mu")]
        public string Mu { get; set; }

        [JsonPropertyName("am")]
        public decimal Am { get; set; }

        [JsonPropertyName("fee")]
        public long Fee { get; set; }

        [JsonPropertyName("vam")]
        public long Vam { get; set; }

        [JsonPropertyName("tsstam")]
        public long Tsstam { get; set; }
    }

    public class Invoice
    {
        [JsonPropertyName("header")]
        public InvoiceHeader Header { get; set; }

        [JsonPropertyName("body")]
        public List<InvoiceBody> Body { get; set; }

        [JsonPropertyName("payments")]
        public List<object> Payments { get; set; }
    }

    public class InvoicePacket
    {
        [JsonPropertyName("header")]
        public PacketHeader Header { get; set; }

        [JsonPropertyName("payload")]
        public string Payload { get; set; }
    }

    public class PacketHeader
    {
        [JsonPropertyName("requestTraceId")]
        public string RequestTraceId { get; set; }

        [JsonPropertyName("fiscalId")]
        public string FiscalId { get; set; }
    }

    public class InvoiceResponse
    {
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("result")]
        public List<InvoiceResult> Result { get; set; }
    }

    public class InvoiceResult
    {
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("packetType")]
        public string PacketType { get; set; }

        [JsonPropertyName("referenceNumber")]
        public string ReferenceNumber { get; set; }

        [JsonPropertyName("data")]
        public object Data { get; set; }
    }

    public class MoadianService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://tp.tax.gov.ir/requestsmanager";
        private readonly string _certificatePath;
        private readonly string _privateKeyPath;

        public MoadianService(string certificatePath, string privateKeyPath)
        {
            _httpClient = new HttpClient();
            _certificatePath = certificatePath;
            _privateKeyPath = privateKeyPath;
        }

        // دریافت Nonce
        public async Task<string> GetNonceAsync(int timeToLive = 30)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v2/nonce?timeToLive={timeToLive}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var nonceResponse = JsonSerializer.Deserialize<NonceResponse>(content);

            return nonceResponse.Nonce;
        }

        // دریافت اطلاعات سرور
        public async Task<ServerInfoResponse> GetServerInfoAsync(string jwt)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");

            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v2/server-information");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ServerInfoResponse>(content);
        }

        // تولید JWT Token
        public string GenerateJwtToken(string nonce, string clientId)
        {
            // بارگذاری کلید خصوصی
            var privateKeyPemReader = new PemReader(System.IO.File.OpenText(_privateKeyPath));
            var privateKey = DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)privateKeyPemReader.ReadObject());

            // بارگذاری گواهی
           // var certPemReader = new PemReader(new System.IO.StreamReader(_certificatePath));

            //Org.BouncyCastle.X509.X509Certificate bcCert = null;
            //var text = System.IO.File.ReadAllText(_certificatePath);

            //if (text.StartsWith("-----BEGIN CERTIFICATE-----"))
            //{
            //    // حالت PEM
            //    using (var sr = new System.IO.StringReader(text))
            //    {
            //        var pemReader = new PemReader(sr);
            //        bcCert = (Org.BouncyCastle.X509.X509Certificate)pemReader.ReadObject();
            //    }
            //}
            //else
            //{
            //    // حالت DER
            //    var certParser = new Org.BouncyCastle.X509.X509CertificateParser();
            //    using (var fs = System.IO.File.ReadAllBytes(_certificatePath))
            //    {
            //        bcCert = certParser.ReadCertificate(fs);
            //    }
            //}

            //var certificate = DotNetUtilities.ToX509Certificate(bcCert);


            var certPemReader = new PemReader(new System.IO.StreamReader(_certificatePath)); 
            var certificate = DotNetUtilities.ToX509Certificate((Org.BouncyCastle.X509.X509Certificate)certPemReader.ReadObject()); 
            var publicKey = DotNetUtilities.ToRSA((RsaKeyParameters)DotNetUtilities.FromX509Certificate(certificate).GetPublicKey());
            var payload = JsonSerializer.Serialize(new { nonce, clientId });

            // تولید JWT
            var jwt = JwtBuilder.Create()
                .WithAlgorithm(new RS256Algorithm(publicKey, privateKey))
                .AddHeader("x5c", new[] { Convert.ToBase64String(certificate.GetRawCertData()) })
                .AddHeader("sigT", DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture))
                .AddHeader("crit", new[] { "sigT" })
                .Encode(JsonSerializer.Deserialize<System.Text.Json.Nodes.JsonNode>(payload));

            return jwt;
        }

        // امضای فاکتور
        public string SignInvoice(Invoice invoice)
        {
            var privateKeyPemReader = new PemReader(System.IO.File.OpenText(_privateKeyPath));
            var privateKey = DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)privateKeyPemReader.ReadObject());

            var certPemReader = new PemReader(new System.IO.StreamReader(_certificatePath));
            var certificate = DotNetUtilities.ToX509Certificate((Org.BouncyCastle.X509.X509Certificate)certPemReader.ReadObject());
            var publicKey = DotNetUtilities.ToRSA((RsaKeyParameters)DotNetUtilities.FromX509Certificate(certificate).GetPublicKey());

            var invoiceJson = JsonSerializer.Serialize(invoice);

            // امضای فاکتور
            var signedInvoice = JwtBuilder.Create()
                .WithAlgorithm(new RS256Algorithm(publicKey, privateKey))
                .AddHeader("x5c", new[] { Convert.ToBase64String(certificate.GetRawCertData()) })
                .AddHeader("sigT", DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture))
                .AddHeader("crit", new[] { "sigT" })
                .Encode(JsonSerializer.Deserialize<System.Text.Json.Nodes.JsonNode>(invoiceJson));

            return signedInvoice;
        }

        // رمزگذاری فاکتور امضا شده
        public string EncryptSignedInvoice(string signedInvoice, string serverPublicKey, string keyId)
        {
            var decoded = Base64.Decode(serverPublicKey);
            var asymmetricKeyParameter = PublicKeyFactory.CreateKey(decoded);
            var rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)asymmetricKeyParameter);
            var rsa = RSA.Create();
            rsa.ImportParameters(rsaParams);

            var header = new Dictionary<string, object>
            {
                { "kid", keyId }
            };

            var recipient = new JweRecipient(JweAlgorithm.RSA_OAEP_256, rsa, header);
            var encryptedInvoice = JWE.Encrypt(signedInvoice, new[] { recipient }, JweEncryption.A256GCM, mode: SerializationMode.Compact);

            return encryptedInvoice;
        }

        // تولید شماره مالیاتی (نمونه ساده)
        public string GenerateTaxId(string fiscalId, string invoiceNumber, DateTime invoiceDate)
        {
            // این تنها یک نمونه ساده است - برای تولید دقیق به الگوریتم کامل نیاز دارید
            var timestamp = ((DateTimeOffset)invoiceDate).ToUnixTimeMilliseconds();
            var baseId = $"{fiscalId}{timestamp:X}{invoiceNumber}";

            // محاسبه checksum (ساده شده)
            var checksum = baseId.GetHashCode() % 10;
            return $"{baseId}{checksum}";
        }

        // ارسال فاکتور
        public async Task<InvoiceResponse> SendInvoiceAsync(List<InvoicePacket> invoicePackets, string jwt)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");

            var json = JsonSerializer.Serialize(invoicePackets);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v2/invoice", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<InvoiceResponse>(responseContent);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}