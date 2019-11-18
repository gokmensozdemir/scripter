using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScripterWebBrowser.Service
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

    public class ScripterService
    {
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
}
