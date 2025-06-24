using Microsoft.AspNetCore.Mvc;

namespace APISALQUOTE.Helpers
{
    public class Requests
    {
        public static IActionResult Response(ControllerBase Controller, ApiStatus statusCode)
        {
            return Response(Controller, statusCode, null, "");
        }

        public static IActionResult Response(ControllerBase Controller, ApiStatus statusCode, object dataValue, string msg)
        {
            var e = new ApiStatus(500);

            var _ = new
            {
                status = e.StatusCode,
                error = true,
                detail = "",
                message = e.StatusDescription,
                data = dataValue
            };

            if (statusCode.StatusCode != 200)
            {
                _ = new
                {
                    status = statusCode.StatusCode,
                    error = true,
                    detail = msg,
                    message = statusCode.StatusDescription,
                    data = dataValue
                };
            }
            else
            {
                _ = new
                {
                    status = statusCode.StatusCode,
                    error = false,
                    detail = msg,
                    message = statusCode.StatusDescription,
                    data = dataValue
                };
            }

            return Controller.StatusCode(statusCode.StatusCode, _);
        }
    }
}
