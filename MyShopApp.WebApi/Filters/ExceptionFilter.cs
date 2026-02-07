using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyShopApp.Application.Exceptions;
using MyShopApp.WebApi.Results;

namespace MyShopApp.WebApi.Filters
{
    public sealed class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            int statusCode;
            string message;

            if (context.Exception is UserFriendlyException ufEx)
            {
                statusCode = 400;
                message = ufEx.Message;

                _logger.LogError("Пользовательская ошибка: {Message}. Код: {Code}.", ufEx.Message, statusCode);
            }
            else
            {
                statusCode = 500;
                message = "Произошла непредвиденная ошибка.";

                _logger.LogError(context.Exception, "Произошла ошибка: {Message}.", context.Exception.Message);
            }

            var error = new ErrorInfo(statusCode.ToString(), message);

            context.Result = new ObjectResult(new BaseResponse
            {
                Error = error
            })
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}
