using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serialization.Json;
using RestSharpExample.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

namespace RestSharpExample
{

    public class UnitTest1
    {
        [Fact]
        public void GetOperation()
        {
            var client = new RestClient("http://localhost:3030");

            var request = new RestRequest("posts/{postid}", Method.GET);
            request.AddUrlSegment("postid", 1);

            var response = client.Execute(request);

            //Deserializar response______________________________________

            //Option 1:
            //var deserialize = new JsonDeserializer();
            //var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            //Assert.Equal(output["author"].ToString(), "Karthik KKK");

            //Option 2:
            JObject obj = JObject.Parse(response.Content);
            Assert.Equal(obj["author"].ToString(), "Karthik KKK");
        }

        [Fact]
        public void PostWithDeserialize()
        {
            var client = new RestClient("http://localhost:3030");

            var request = new RestRequest("posts", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new Post() { id = 1, author = "Natalia", title = "test" });

            var response = client.Execute(request);

            //Deserializar response______________________________________

            //Option 1:
            var deserialize = new JsonDeserializer();
            var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            Assert.Equal(output["author"], "Natalia");

            //Option 2:
            //JObject obj = JObject.Parse(response.Content);
            //Assert.Equal(obj["author"].ToString(), "Karthik KKK");       
        }

        [Fact]
        public async void PostWithoutDeserialize()
        {
            var client = new RestClient("http://localhost:3030");

            var request = new RestRequest("posts", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new Post() { id = 1, author = "Natalia", title = "test" });

            var response = client.Execute<Post>(request);

            Assert.Equal(response.Data.author, "Natalia");
        }

        [Fact]
        public async void PostWithAsync()
        {
            var client = new RestClient("http://localhost:3030");

            var request = new RestRequest("posts", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new Post() { id = 1, author = "Natalia", title = "test" });

            var response = await ExecuteAsyncRequest<Post>(client, request);

            //Ou sem o metodo ser async: 
            //var response = ExecuteAsyncRequest<Post>(client, request).GetAwaiter().GetResult();

            Assert.Equal(response.Data.author, "Natalia");
        }

        private async Task<IRestResponse<T>> ExecuteAsyncRequest<T>(RestClient client, IRestRequest request) where T : class, new()
        {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse<T>>();

            client.ExecuteAsync<T>(request, restResponse =>
            {
                if (restResponse.ErrorException != null)
                {
                    const string message = "Error retrieving response.";
                    throw new ApplicationException(message, restResponse.ErrorException);
                }
                taskCompletionSource.SetResult(restResponse);
            });
            return await taskCompletionSource.Task;
        }
    }
}
