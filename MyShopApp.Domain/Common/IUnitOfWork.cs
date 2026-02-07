namespace MyShopApp.Domain.Common
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Сохранение изменений в базе данных
        /// </summary>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Количество затронутых записей</returns>
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        /// <summary>
        /// Начало новой транзакции
        /// </summary>
        /// <param name="ct">Токен отмены операции</param>
        Task BeginTransactionAsync(CancellationToken ct = default);

        /// <summary>
        /// Фиксация текущей транзакции
        /// </summary>
        /// <param name="ct">Токен отмены операции</param>
        Task CommitTransactionAsync(CancellationToken ct = default);

        /// <summary>
        /// Откат текущей транзакции
        /// </summary>
        /// <param name="ct">Токен отмены операции</param>
        Task RollbackTransactionAsync(CancellationToken ct = default);
    }
}