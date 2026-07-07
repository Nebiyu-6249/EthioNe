namespace EthioNegarit.WebUI.Application.Payments
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using EthioNegarit.Utilities;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Result of a PayPal create/capture call.
    /// </summary>
    public class PayPalResult
    {
        public bool Success { get; set; }
        public string OrderId { get; set; }
        public string ApproveUrl { get; set; }
        public string Status { get; set; }
        public decimal CapturedAmount { get; set; }
        public string CurrencyCode { get; set; }
        public string CaptureId { get; set; }
        public string Error { get; set; }
    }

    /// <summary>
    /// Minimal PayPal Checkout (Orders v2) client using the REST API directly (no SDK), so it
    /// adds no NuGet dependency. All credentials come from Web.config appSettings (server-only):
    /// PayPal:ClientId, PayPal:Secret, PayPal:Environment (sandbox|live), PayPal:Currency,
    /// PayPal:AmountFactor. The whole feature is inert until ClientId + Secret are set.
    /// </summary>
    public class PayPalService
    {
        private static string ClientId => ConfigUtils.GetAppSetting("PayPal:ClientId", string.Empty);
        private static string Secret => ConfigUtils.GetAppSetting("PayPal:Secret", string.Empty);

        public bool IsConfigured => !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(Secret);

        /// <summary>Currency PayPal is charged in (ad prices are in ETB, which PayPal barely
        /// supports, so this defaults to USD - see PayPal:AmountFactor to convert).</summary>
        public string Currency
        {
            get { var c = ConfigUtils.GetAppSetting("PayPal:Currency", "USD"); return string.IsNullOrWhiteSpace(c) ? "USD" : c.Trim(); }
        }

        /// <summary>Multiplier applied to the (ETB) package price to get the charged amount, e.g.
        /// 0.018 to approximate ETB->USD. Defaults to 1.0 (owner prices packages in the currency).</summary>
        public decimal AmountFactor
        {
            get
            {
                decimal f;
                var raw = ConfigUtils.GetAppSetting("PayPal:AmountFactor", "1");
                return decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out f) && f > 0 ? f : 1m;
            }
        }

        private static string BaseUrl
        {
            get
            {
                var env = ConfigUtils.GetAppSetting("PayPal:Environment", "sandbox");
                return string.Equals(env, "live", StringComparison.OrdinalIgnoreCase)
                    ? "https://api-m.paypal.com"
                    : "https://api-m.sandbox.paypal.com";
            }
        }

        /// <summary>Convert a package price into the PayPal charge amount (2 dp).</summary>
        public decimal ToChargeAmount(double packagePrice)
        {
            return Math.Round((decimal)packagePrice * AmountFactor, 2, MidpointRounding.AwayFromZero);
        }

        public PayPalResult CreateOrder(decimal amount, string reference, string returnUrl, string cancelUrl)
        {
            var result = new PayPalResult();
            try
            {
                var token = GetAccessToken();
                var payload = new JObject
                {
                    ["intent"] = "CAPTURE",
                    ["purchase_units"] = new JArray(new JObject
                    {
                        ["custom_id"] = reference,
                        ["amount"] = new JObject
                        {
                            ["currency_code"] = Currency,
                            ["value"] = amount.ToString("0.00", CultureInfo.InvariantCulture)
                        }
                    }),
                    ["application_context"] = new JObject
                    {
                        ["return_url"] = returnUrl,
                        ["cancel_url"] = cancelUrl,
                        ["user_action"] = "PAY_NOW",
                        ["shipping_preference"] = "NO_SHIPPING"
                    }
                };

                var json = PostJson(BaseUrl + "/v2/checkout/orders", token, payload.ToString());
                result.OrderId = (string)json["id"];
                result.Status = (string)json["status"];
                var links = json["links"] as JArray;
                if (links != null)
                {
                    foreach (var link in links)
                    {
                        if (string.Equals((string)link["rel"], "approve", StringComparison.OrdinalIgnoreCase))
                        {
                            result.ApproveUrl = (string)link["href"];
                            break;
                        }
                    }
                }
                result.Success = !string.IsNullOrEmpty(result.OrderId) && !string.IsNullOrEmpty(result.ApproveUrl);
                if (!result.Success && string.IsNullOrEmpty(result.Error))
                {
                    result.Error = "PayPal did not return an approval link.";
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }
            return result;
        }

        public PayPalResult CaptureOrder(string orderId)
        {
            var result = new PayPalResult { OrderId = orderId };
            try
            {
                var token = GetAccessToken();
                var json = PostJson(BaseUrl + "/v2/checkout/orders/" + Uri.EscapeDataString(orderId) + "/capture", token, "{}");
                result.Status = (string)json["status"];

                var capture = json["purchase_units"]?[0]?["payments"]?["captures"]?[0];
                if (capture != null)
                {
                    result.CaptureId = (string)capture["id"];
                    result.CurrencyCode = (string)capture["amount"]?["currency_code"];
                    decimal amt;
                    if (decimal.TryParse((string)capture["amount"]?["value"], NumberStyles.Any, CultureInfo.InvariantCulture, out amt))
                    {
                        result.CapturedAmount = amt;
                    }
                }
                result.Success = string.Equals(result.Status, "COMPLETED", StringComparison.OrdinalIgnoreCase);
                if (!result.Success && string.IsNullOrEmpty(result.Error))
                {
                    result.Error = "PayPal capture status: " + (result.Status ?? "unknown");
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }
            return result;
        }

        private string GetAccessToken()
        {
            var request = (HttpWebRequest)WebRequest.Create(BaseUrl + "/v1/oauth2/token");
            request.Method = "POST";
            request.Timeout = 15000;
            request.ReadWriteTimeout = 15000;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientId + ":" + Secret));

            var body = Encoding.UTF8.GetBytes("grant_type=client_credentials");
            request.ContentLength = body.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(body, 0, body.Length);
            }
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var json = JObject.Parse(reader.ReadToEnd());
                return (string)json["access_token"];
            }
        }

        private static JObject PostJson(string url, string bearerToken, string jsonBody)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = 20000;
            request.ReadWriteTimeout = 20000;
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + bearerToken;

            var body = Encoding.UTF8.GetBytes(jsonBody ?? "{}");
            request.ContentLength = body.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(body, 0, body.Length);
            }
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return JObject.Parse(reader.ReadToEnd());
            }
        }
    }
}
