using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Process logging
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (exception is ValidationException)
            {
                var result = new JsonResult(new ExceptionErrorMessage(exception.GetBaseException().Message));
                result.StatusCode = StatusCodes.Status400BadRequest;

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(result.Value);
            }
            else if (exception is CustomValidationException)
            {
                var result = new JsonResult(new ExceptionErrorMessage(exception.GetBaseException().Message));
                result.StatusCode = (int)HttpStatusCode.BadRequest;
                result.StatusCode = (int)HttpStatusCode.BadRequest;

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(result.Value);
            }
            else
            {
                var result = new JsonResult(new ExceptionErrorMessage(exception.GetBaseException().Message));
                result.StatusCode = (int)HttpStatusCode.BadRequest;
                result.ContentType = "application/json";

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(result.Value);
            }
        }
    }
}
