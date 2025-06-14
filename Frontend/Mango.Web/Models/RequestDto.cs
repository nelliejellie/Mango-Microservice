using Mango.Web.Models.Utilities;
using static Mango.Web.Models.Utilities.ContentTypes;

namespace Mango.Web.Models
{
    public class RequestDto
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; } = string.Empty;
        public object? Data { get; set; }
        public string AccessToken { get; set; }

        public ContentTypeEnum ContentType { get; set; } = ContentTypeEnum.Json;
    }
}