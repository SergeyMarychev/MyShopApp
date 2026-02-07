using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyShopApp.WebApi.Results;

namespace MyShopApp.WebApi.Filters
{
    public sealed class ResultFilter : IResultFilter
    {
        private readonly ILogger<ResultFilter> _logger;

        public ResultFilter(ILogger<ResultFilter> logger)
        {
            _logger = logger;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is not ObjectResult result)
            {
                _logger.LogInformation("Результат отсутствует. Возвращён пустой BaseResponse.");
                context.Result = new ObjectResult(new BaseResponse());
                return;
            }

            var value = result.Value;

            if (value is BaseResponse)
            {
                _logger.LogDebug("Результат уже обёрнут в BaseResponse — повторная обертка не требуется.");
                return;
            }

            _logger.LogInformation("Результат обёрнут в стандартный BaseResponse.");

            result.Value = new BaseResponse<object>
            {
                Result = value,
                Error = null
            };
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
