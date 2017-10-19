using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BonitaTest
{
    class Program
    {

        static void Main(string[] args)
        {
            GetHttp();

            Console.ReadLine();
        }

        public static async void GetHttp()
        {
            var baseAddress = new Uri("http://localhost:8080/bonita/");

            using (var handler = new HttpClientHandler { UseCookies = true })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    var formContent = new Dictionary<string, string>
                        {
                            {"username", "walter.bates"},
                            {"password", "bpm"},
                            {"redirect", "false"}
                        };

                    var result = client.PostAsync("/bonita/loginservice", new FormUrlEncodedContent(formContent)).Result;

                    Console.WriteLine(result.ToString());

                    var setCookie = string.Empty;

                    foreach (var header in result.Headers.Where(header => header.Key == "Set-Cookie"))
                    {
                        foreach (var value in header.Value)
                        {
                            setCookie = value;
                            break;
                        }
                        break;
                    }



                    if (setCookie.Trim().Equals(""))
                    {
                        throw new Exception("No authentication cookie from login call!");
                    }

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    var message = new HttpRequestMessage(HttpMethod.Get, "/bonita/API/bpm/process?p=0&c=10&f=name=Travel%20Request");
                   // var message = new HttpRequestMessage(HttpMethod.Get, "/bonita/API/system/session/1");
                    message.Headers.Add("Cookie", setCookie);
                    


                    var result2 = client.SendAsync(message).Result;

                    result2.EnsureSuccessStatusCode();

                    string text = await result2.Content.ReadAsStringAsync();

                    text = text.Remove(0, 1);
                    text = text.Remove(text.Length - 1, 1);
                    Console.WriteLine(text);
                    /*
                    Test q = new Test();
                    q.displayName = "asdiojhosid";
                    q.id = "10";
                    q.version = "1.10";

                    Console.WriteLine(JsonConvert.SerializeObject(q);
                    */
                    /*
                    Test t = JsonConvert.DeserializeObject<Test>(text);

                    Console.WriteLine(Environment.NewLine + Environment.NewLine);
                    Console.WriteLine(t.displayName);
                    Console.WriteLine(t.id);
                    Console.WriteLine(t.name);
                    */
                }
            }


        }

        class Test
        {
            public string displayDescription { get; set; }
            public string deploymentDate { get; set; }
            public string displayName { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public int deployedBy { get; set; }
            public string id { get; set; }
            public string activationState { get; set; }
            public string version { get; set; }
            public string configurationState { get; set; }
            public string last_update_date { get; set; }
            public int actorinitiatorid { get; set; }
        }


        //[{"displayDescription":"","deploymentDate":"2017-10-12 13:11:12.826","displayName":"Travel Request","name":"Travel Request","description":"","deployedBy":"4","id":"5161451567036450621","activationState":"ENABLED","version":"1.0","configurationState":"RESOLVED","last_update_date":"2017-10-12 13:11:13.140","actorinitiatorid":"2"}]

    }
}
