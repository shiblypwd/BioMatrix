using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore
{
    class AlphaSMS
    {
        public AlphaSMS()
        {
        }

        async public void sendSMS(string body, string mobileNo) {
            HttpClient client = new HttpClient();
            var requestInput = new Dictionary<string, string>
            {
                //{ "api_key_1", "u2AE4IjCC87Z7vDfMnSO8r5b147V4fQYD9055hkP" }
                { "api_key", "g694IRVD6e72jw2bBT1NKqgJfn6Af03Y68YtXMIe" }
            };
            Console.WriteLine(body);
            Console.WriteLine(mobileNo);

            requestInput.Add("msg", body);
            requestInput.Add("to", mobileNo);
            
            string url = "https://api.sms.net.bd/sendsms";
            var data = new FormUrlEncodedContent(requestInput);
            var httpResponse = await client.PostAsync(url, data);

            if (httpResponse.Content != null)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                dynamic response = JObject.Parse(responseContent);
                Console.WriteLine("Status: "+response.data.request_status);
                Console.WriteLine("ErrorMessage: " + response.data.error);
                
            }            
        }

       
    }
}
