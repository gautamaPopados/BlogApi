using System.Net;

namespace WebApplication1
{
    public class APIResponse
    {
        public HttpStatusCode status { get; set; }

        public List<string> errors { get; set; } = new List<string>();

        public object Result { get; set; }
    }
}
