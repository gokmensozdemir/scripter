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
    public class RequestLogin
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class ResponseLogin
    {
        public string accessToken { get; set; }
    }

    public class RequestModel
    {
        public string token { get; set; }
        public string scenario { get; set; }
        public string currentPhoneNumber { get; set; }
        public Dictionary<string, object> variables { get; set; }
    }

    public class ResponseModel
    {
        public string message { get; set; }
    }

    public class ScripterService
    {
        public static string Start(RequestModel model)
        {
            string message = null;

            IRestClient restClient = new RestClient();

            IRestRequest request = new RestRequest("http://10.10.55.51/apiv1/scripterservice/startscripter", Method.POST);

            request.AddHeader("content-type", "application/json");

            request.AddParameter("application/json", JsonConvert.SerializeObject(model), ParameterType.RequestBody);

            IRestResponse response = restClient.Execute(request);

            if (response.ResponseStatus == ResponseStatus.Completed && response.IsSuccessful && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseContent = JsonConvert.DeserializeObject<ResponseModel>(response.Content);

                if (responseContent != null)
                {
                    message = responseContent.message;
                }
            }

            return message;
        }

        public static string Login(RequestLogin model)
        {
            string token = null;

            IRestClient restClient = new RestClient();

            IRestRequest request = new RestRequest("http://10.10.55.51/apiv1/identity/token", Method.POST);

            request.AddHeader("content-type", "application/json");

            request.AddParameter("application/json", JsonConvert.SerializeObject(model), ParameterType.RequestBody);

            IRestResponse response = restClient.Execute(request);

            if (response.ResponseStatus == ResponseStatus.Completed && response.IsSuccessful && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseContent = JsonConvert.DeserializeObject<ResponseLogin>(response.Content);

                return responseContent.accessToken;
            }

            return token;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var requestLogin = new RequestLogin
            {
                username = "sys.test",
                password = "sys.test"
            };

            var requestModel = new RequestModel
            {
                scenario = "ACTIVITY_ERCAN_TEST",
                currentPhoneNumber = "5327004256",
                variables = new Dictionary<string, object>
                    {
                        { "is_attr_CallPexID", "3"},
                        { "agentName", "Gökmen"}
                    }
            };

            try
            {
                requestModel.token = ScripterService.Login(requestLogin);

                string message = ScripterService.Start(requestModel);

                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}
