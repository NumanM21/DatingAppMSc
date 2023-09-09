
using System.Text.Json;
using API.ExternalHelpers;

// Extension so class must be static. 

namespace API.Extensions
{
    public static class ExtensionHTTP
    {
        public static void HeadPaginationAdd(this HttpResponse httpResponse, HeadPagination headPagination)
        {
            // need to serialize this into json so we can send it to client with header (client reads json format and not c# object)
            var jsonOpt = new JsonSerializerOptions
            {
                // default is pascal case (hence we override it to camel case)
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
             httpResponse.Headers.Add("Pagination", JsonSerializer.Serialize(headPagination, jsonOpt));
            
            // CORS (Cross Origin Resource Sharing) -> Allows us to access our API from a different origin (localhost:4200) -> since custom header (otherwise client access)
            // pagination is the header we are exposing
            httpResponse.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}