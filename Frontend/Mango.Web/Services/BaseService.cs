using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mango.Web.Models;
using Mango.Web.Models.Utilities;
using Mango.Web.Services.IService;
using System.Net;
using Newtonsoft.Json; // For JsonConvert
using System.Text; 

namespace Mango.Web.Services
{
    public class BaseService: IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProviderService _tokenProviderService;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProviderService tokenProviderService)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProviderService = tokenProviderService;

            // Constructor logic here

        }
        public async Task<ResponseDto> SendAsync(RequestDto request, bool withBearer = true)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                if(withBearer)
                {
                    var token = _tokenProviderService.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }
                message.RequestUri = new Uri(request.Url);

                if(request.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage apiResponse = null;

                switch (request.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                //var clients = new HttpClient();
                //var responses = await clients.GetAsync("https://localhost:7000/api/product");
                //var result = await responses.Content.ReadAsStringAsync();

                apiResponse = await client.SendAsync(message);
            
                switch(apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() {Success = false,StatusCode = (int)HttpStatusCode.NotFound, Message = "Resource not found."};
                    case HttpStatusCode.Forbidden:
                        return new() {Success = false,StatusCode = (int)HttpStatusCode.NotFound, Message = "Access to the resource is forbidden."};
                    case HttpStatusCode.Unauthorized:
                        return new() {Success = false,StatusCode = (int)HttpStatusCode.NotFound, Message = "Unauthorized access."};
                    case HttpStatusCode.InternalServerError:
                        return new() {Success = false,StatusCode = (int)HttpStatusCode.NotFound, Message = "Internal server error."};
                    default:
                        var content = await apiResponse.Content.ReadAsStringAsync();
                        var response = JsonConvert.DeserializeObject<ResponseDto>(content);
                        return response;
                }
            }
            catch (Exception ex)
            {
                return new() { Success = false, StatusCode = (int)HttpStatusCode.InternalServerError, Message = ex.Message };
            }
            
        }
    }
}