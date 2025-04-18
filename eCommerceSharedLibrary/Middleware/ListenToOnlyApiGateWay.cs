using Microsoft.AspNetCore.Http;


namespace eCommerceSharedLibrary.Middleware
{
    public class ListenToOnlyApiGateWay(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //Extraxt specific header from the reqeust
            var singedHeader = context.Request.Headers["Api-Gateway"];
            //Null means, request not comming from api -Gateway
            if (singedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry,service is not available");
                return;
            }
            else { 
               await  next (context);
            }
        }
    }
}
