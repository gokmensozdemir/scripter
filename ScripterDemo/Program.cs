using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScripterDemo
{
    public class RequestModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string scenario { get; set; }
        public string direction { get; set; }
        public string dialId { get; set; }
        public string guid { get; set; }
        public string campaignId { get; set; }
        public string listId { get; set; }
        public string callId { get; set; }
        public string agentId { get; set; }
        public string agentName { get; set; }
        public string currentPhoneNumber { get; set; }
        public Dictionary<string, object> variables { get; set; }
    }

    public class ResponseModel
    {
        public string message { get; set; }
        public ResponseData data { get; set; }
    }

    public class ResponseData
    {
        public string token { get; set; }
    }

    public class ScripterService
    {
        public static string Start(RequestModel model)
        {
            string token = null;

            IRestClient restClient = new RestClient();

            IRestRequest request = new RestRequest("https://andromeda.viases.cloud/apiv1/scripterservice/startscripter", Method.POST);

            request.AddHeader("content-type", "application/json");

            request.AddParameter("application/json", JsonConvert.SerializeObject(model), ParameterType.RequestBody);

            IRestResponse response = restClient.Execute(request);

            if (response.ResponseStatus == ResponseStatus.Completed && response.IsSuccessful && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseContent = JsonConvert.DeserializeObject<ResponseModel>(response.Content);

                if (responseContent.data != null)
                {
                    token = responseContent.data.token;
                }
            }

            return token;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var requestModel = new RequestModel
            {
                username = "gokmen",
                password = "gokmen",
                scenario = "WebHelpIninTest",
                direction = "INBOUND",
                dialId = "1",
                guid = "2",
                campaignId = "3",
                listId = "4",
                callId = "5",
                agentId = "6",
                agentName = "7",
                currentPhoneNumber = "8",
                variables = new Dictionary<string, object>
                    {
                        { "IninCallPexId", "9"},
                        { "IninCallId", "10"},
                        { "IninCallIdKey", "11"},
                        { "IninDnis", "12"}
                    }
            };

            try
            {
                string token = ScripterService.Start(requestModel);

                Console.WriteLine(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}
