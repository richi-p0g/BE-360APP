using System.Net;

namespace APISALQUOTE.Helpers
{
    public class ApiStatus
    {
        public int StatusCode { get; private set; }

        public string StatusDescription { get; private set; }

        public string Message { get; private set; }

        public ApiStatus(int statusCode)
        {
            HttpStatusCode parsedCode = (HttpStatusCode)statusCode;
            StatusCode = statusCode;
            StatusDescription = parsedCode.ToString();
        }

        public ApiStatus(int statusCode, string statusDescription)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }

        public ApiStatus(int statusCode, string statusDescription, string message)
            : this(statusCode, statusDescription)
        {
            Message = message;
        }
    }


}
