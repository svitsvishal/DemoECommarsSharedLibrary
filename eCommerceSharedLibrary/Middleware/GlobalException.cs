using eCommerceSharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json;


namespace eCommerceSharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string message = "sorry internal server error";
            int statuscode =(int)HttpStatusCode.InternalServerError;
            string title = "Error";
            try
            {
                await next(context);
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests) 
                {
                    title = "warning";
                    message = "too many requests";
                    statuscode =(int)HttpStatusCode.TooManyRequests;
                    await ModifyHeader (context ,title,message,statuscode);
                }
                // If response is unAuthorized
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Un Athorized";
                    message = "Un Athorized";
                    statuscode = (int)StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context, title, message, statuscode);
                }
                // If response is forbidden
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Status403Forbidden";
                    message = "Status403Forbidden";
                    statuscode = (int)StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message, statuscode);
                }
            }
            catch (Exception ex)
            {

                LogException.LogExecptions(ex);

                //log time out
                if (ex is TimeoutException || ex is TaskCanceledException)
                {
                    title = "out of time";
                    message = "request time out ...... try again";
                    statuscode = StatusCodes.Status408RequestTimeout;
                }
                await ModifyHeader(context, title, message, statuscode);
            }
        }

        public static async Task ModifyHeader(HttpContext context, string title, string message, int statuscode)
        {
            //display message to client
            context.Response.ContentType= "application/json";
            await context.Response.WriteAsync (JsonSerializer.Serialize( new ProblemDetails
            {
                Detail =message,
                Status =statuscode,
                Title = title


            }),CancellationToken.None);
            return;
        }
    }
}
