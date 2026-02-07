using Microsoft.AspNetCore.Mvc.Filters;
using MyShopApp.Domain.Common;

namespace MyShopApp.WebApi.Filters
{
    public class TransactionFilter : IAsyncActionFilter
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionFilter> _logger;

        public TransactionFilter(IUnitOfWork unitOfWork, ILogger<TransactionFilter> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                _logger.LogInformation("Начало транзакции.");

                await _unitOfWork.BeginTransactionAsync();
                var resultContext = await next();

                if (resultContext.Exception == null)
                {
                    _logger.LogInformation("Транзакция успешно завершена.");
                    await _unitOfWork.CommitTransactionAsync();
                }
                else
                {
                    _logger.LogWarning(resultContext.Exception, "Ошибка во время выполнения действия. Транзакция будет отменена.");

                    await _unitOfWork.RollbackTransactionAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Критическая ошибка при выполнении транзакции. Транзакция будет отменена.");

                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
