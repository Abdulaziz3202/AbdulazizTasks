using MVCRESTAPI.Services.EsriService.Dto;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MVCRESTAPI.Services.EsriService
{
    public class EsriServices : IEsriService
    {

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public EsriServices(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<TokenDto> GetEsriToken()
        {
            var generatedTokenObject = new TokenDto();
            try
            {

                var queryParameters = new Dictionary<string, string>();


                var AddFeatureConfiguration = _configuration.GetSection("Esri");

                var username = AddFeatureConfiguration.GetValue<string>("PortalTokenUserName");//PortalTokenUserName
                var password = AddFeatureConfiguration.GetValue<string>("PortalTokenPassword");
                var tokenUrl = AddFeatureConfiguration.GetValue<string>("TokenUrl");
                var referer = AddFeatureConfiguration.GetValue<string>("RefererUrl");
                var expiration = AddFeatureConfiguration.GetValue<long>("Expiration");

                queryParameters.Add("f", "json");

                queryParameters.Add("username", username);
                queryParameters.Add("password", password);
                queryParameters.Add("client", "referer");

                queryParameters.Add("referer", referer);

                queryParameters.Add("expiration", expiration.ToString());

                var input = new FormUrlEncodedContent(queryParameters);



                var client = _httpClientFactory.CreateClient("HttpClientFactory");
                var response = await client.PostAsync(tokenUrl, input);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    generatedTokenObject = JsonConvert.DeserializeObject<TokenDto>(jsonString);

                }
                else
                {
                    string errMsg = await response.Content.ReadAsStringAsync();
                    throw new Exception(errMsg);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return generatedTokenObject;
        }


        public async Task<string> QueryFeatures(string token)
        {
            try
            {

                var client = _httpClientFactory.CreateClient("HttpClientFactory");

                // Use the obtained token to query features
                var featuresUrl = "https:///arcgis/rest/services/Application_Section/callcenter_new/FeatureServer/38/query"; // Replace with your Esri server URL
                                                                                                                             // Define query options manually
                var queryOptions = new Dictionary<string, string>
    {
        { "where", "1=1" },
        { "outFields", "*" },
        { "returnGeometry", "false" },
         { "f", "json" },
        // Add more query options as needed
    };

                var featureRequest = new HttpRequestMessage(HttpMethod.Get, featuresUrl);
                featureRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // Set the Accept header to indicate JSON response
                featureRequest.Headers.Add("Preferred-ResponseType", "json");


                // Append query options to the URL
                var queryString = string.Join("&", queryOptions.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
                featureRequest.RequestUri = new Uri($"{featuresUrl}?{queryString}");
                // Add query parameters if needed
                // foreach (var queryParam in queryOptions)
                // {
                //     featureRequest.Headers.Add(queryParam.Key, queryParam.Value);
                // }

                var featureResponse = await client.SendAsync(featureRequest);

                if (featureResponse.IsSuccessStatusCode)
                {
                    var featureJsonString = await featureResponse.Content.ReadAsStringAsync();
                    // Process feature data
                    return featureJsonString;

                }
                else
                {
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
