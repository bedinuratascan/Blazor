using BookStore_UI.Contracts;
using BookStore_UI.Models;
using BookStore_UI.Static;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_UI.Services
{
    public class AuthenticationRespository : IAuthenticationRepository
    {
        private readonly IHttpClientFactory _client;
        public AuthenticationRespository(IHttpClientFactory client)
        {
            _client = client;
        }
        public async Task<bool> Register(RegistrationModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.RegisterEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(user),Encoding.UTF8,"application/json");
            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}
