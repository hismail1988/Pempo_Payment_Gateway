using Newtonsoft.Json;
using PempoPaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;

namespace PempoPaymentGateway.Controllers
{
    public class PempoPaymentController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public bool Post([FromBody]  PaymentRequestDto PRD)
        {
            
            if (PRD == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public IHttpActionResult Credit()
        {
            var token = GetToken();
            var order = CreateOrder(token);
            var paymentToken = GetPaymentToken(order, token);
            return Redirect($"https://portal.weaccept.co/api/acceptance/iframes/{Environment.GetEnvironmentVariable("PAYMOB_IFRAME_ID")}?payment_token={paymentToken}");
        }

        public string GetToken()
        {
            var client = new HttpClient();
            var response = client.PostAsync("https://accept.paymob.com/api/auth/tokens", new StringContent($"{{ \"ZXlKaGJHY2lPaUpJVXpVeE1pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SmpiR0Z6Y3lJNklrMWxjbU5vWVc1MElpd2ljSEp2Wm1sc1pWOXdheUk2T1RReU9EazVMQ0p1WVcxbElqb2lNVGN3TVRFM05EZzFNaTR6TWpFNE9EZ2lmUS5oY3VmU3ZjLWFnVjF0bHlwRHlSZXlhZERXVkdrMWJSYVZia2JlUjV3MFk5Y28tQ3p6LUNHQVlHZ2hlc3NiSmU0X01YbjRuRUo5UWZBeTZCTFZhd0RXdw==\": \"{Environment.GetEnvironmentVariable("PAYMOB_API_KEY")}\" }}", Encoding.UTF8, "application/json")).Result;
            var token = response.Content.ReadAsAsync<dynamic>().Result.token;
            return token;
        }
        public dynamic CreateOrder(string token)
        {
            var items = new List<dynamic>
        {
            new
            {
                name = "ASC1515",
                amount_cents = "500000",
                description = "Smart Watch",
                quantity = "1"
            },
            new
            {
                name = "ERT6565",
                amount_cents = "200000",
                description = "Power Bank",
                quantity = "1"
            }
        };
            var data = new
            {
                auth_token = token,
                delivery_needed = "false",
                amount_cents = "100",
                currency = "EGP",
                items = items
            };
            var client = new HttpClient();
            var response = client.PostAsync("https://accept.paymob.com/api/ecommerce/orders", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")).Result;
            var order = response.Content.ReadAsAsync<dynamic>().Result;
            return order;
        }

        public string GetPaymentToken(dynamic order, string token)
        {
            var billingData = new
            {
                apartment = "803",
                email = "claudette09@exa.com",
                floor = "42",
                first_name = "Clifford",
                street = "Ethan Land",
                building = "8028",
                phone_number = "+86(8)9135210487",
                shipping_method = "PKG",
                postal_code = "01898",
                city = "Jaskolskiburgh",
                country = "CR",
                last_name = "Nicolas",
                state = "Utah"
            };
            var data = new
            {
                auth_token = token,
                amount_cents = "100",
                expiration = 3600,
                order_id = order.id,
                billing_data = billingData,
                currency = "EGP",
                integration_id = Environment.GetEnvironmentVariable("PAYMOB_INTEGRATION_ID")
            };
            var client = new HttpClient();
            var response = client.PostAsync("https://accept.paymob.com/api/acceptance/payment_keys", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")).Result;
            var paymentToken = response.Content.ReadAsAsync<dynamic>().Result.token;
            return paymentToken;
        }

        //public bool IHttpActionResult Callback([FromBody] dynamic request)
        //{
        //    var data = request.ToObject<Dictionary<string, string>>();
        //    var sortedData = data.OrderBy(x => x.Key);
        //    var hmac = data["hmac"];
        //    var array = new List<string>
        //{
        //    "amount_cents",
        //    "created_at",
        //    "currency",
        //    "error_occured",
        //    "has_parent_transaction",
        //    "id",
        //    "integration_id",
        //    "is_3d_secure",
        //    "is_auth",
        //    "is_capture",
        //    "is_refunded",
        //    "is_standalone_payment",
        //    "is_voided",
        //    "order",
        //    "owner",
        //    "pending",
        //    "source_data_pan",
        //    "source_data_sub_type",
        //    "source_data_type",
        //    "success"
        //};
        //    var connectedString = "";
        //    foreach (var item in sortedData)
        //    {
        //        if (array.Contains(item.Key))
        //        {
        //            connectedString += item.Value;
        //        }
        //    }
        //    var secret = Environment.GetEnvironmentVariable("PAYMOB_HMAC");
        //    var hashed = BitConverter.ToString(new HMACSHA512(Encoding.UTF8.GetBytes(secret)).ComputeHash(Encoding.UTF8.GetBytes(connectedString))).Replace("-", "").ToLower();
        //    if (hashed == hmac)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

    }
}
