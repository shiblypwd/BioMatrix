﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore
{
    public class AlphaSMS
    {
        public AlphaSMS()
        {
        }

        async public void sendSMS(string body, string mobileNo)
        {
            HttpClient client = new HttpClient();
            var requestInput = new Dictionary<string, string>
            {
                //{ "api_key_1", "u2AE4IjCC87Z7vDfMnSO8r5b147V4fQYD9055hkP" }
                //{ "api_key", "g694IRVD6e72jw2bBT1NKqgJfn6Af03Y68YtXMIe" }
            };
            //Console.WriteLine(body);
            //Console.WriteLine(mobileNo);

            requestInput.Add("msg", body);
            requestInput.Add("to", mobileNo);

            string url = "https://api.sms.net.bd/sendsms";
            var data = new FormUrlEncodedContent(requestInput);
            var httpResponse = await client.PostAsync(url, data);

            if (httpResponse.Content != null)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                dynamic response = JObject.Parse(responseContent);
                Console.WriteLine("Status: " + response.data.request_status);
                Console.WriteLine("ErrorMessage: " + response.data.error);

            }
        }

        async public void insertEntry()
        {
            try
            {

                HttpClient client = new HttpClient();
                var requestInput = new Dictionary<string, string>
            {
                {"id", "10"},
                {"attendanceCardId", "0"},
                {"entryTime", "2022-09-26T06:35:17.149Z"},
                {"machineName", "InGate-22"},
                {"verifyMode", "Card"},
                {"inOutStatus", "1"}
            };


                //requestInput.Add("msg", body);
                //requestInput.Add("to", mobileNo);

                string url = "http://localhost:44399/Home/Create";
                var data = new FormUrlEncodedContent(requestInput);
                var httpResponse = client.PostAsync(url, data);
            }
            catch (Exception ex)
            {

                throw;
            }
            //var httpResponse = await client.PostAsync(url, data);

            //if (httpResponse.Content != null)
            //{
            //    var responseContent = await httpResponse.Content.ReadAsStringAsync();
            //    dynamic response = JObject.Parse(responseContent);
            //    Console.WriteLine("Status: " + response.data.request_status);
            //    Console.WriteLine("ErrorMessage: " + response.data.error);

            //}
        }

        async public void postRequest(Post postData)
        {
            HttpClient client = new HttpClient();

            //Post post = new Post();



            //post.id = id;
            //post.attendanceCardId = attendanceCardId;
            //post.entryTime = entryTime;
            //post.machineName = machineName;
            //post.verifyMode = verifyMode;
            //post.inOutStatus = inOutStatus;

            var stringPost = JsonConvert.SerializeObject(postData);

            var httpContent = new StringContent(stringPost, Encoding.UTF8, "application/json");


            string url = "https://localhost:44399/api/app/attendance";
            //var data = new FormUrlEncodedContent(post);
            var httpResponse = await client.PostAsync(url, httpContent);

            while (httpResponse.Content != null)
            {
                Console.WriteLine(httpResponse);
                Console.WriteLine();
            }

        }



    }
}
