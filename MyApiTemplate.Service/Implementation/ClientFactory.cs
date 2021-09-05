using MyApiTemplate.Service.Contract;
using MyApiTemplate.Service.Exceptions;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyApiTemplate.Service.Implementation
{
    public class ClientFactory : IClientFactory
    {
        private readonly HttpClient _httpClient;
        public ClientFactory(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SampleResponse> PostDataAsync<SampleResponse, SampleRequest>(string endPoint, SampleRequest dto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var httpResponse = await _httpClient.PostAsync(endPoint, content);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new ApiException($"[{httpResponse.StatusCode}] An error occured while requesting external api.");
            }

            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<SampleResponse>(jsonString);

            return data;
        }

        public async Task<SampleResponse> GetDataAsync<SampleResponse>(string endPoint)
        {
            var httpResponse = await _httpClient.GetAsync(endPoint);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new ApiException($"[{httpResponse.StatusCode}] An error occured while requesting external api.");
            }

            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<SampleResponse>(jsonString);

            return data;
        }

    }
}
