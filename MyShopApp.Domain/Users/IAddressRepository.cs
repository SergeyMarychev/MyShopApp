namespace MyShopApp.Domain.Users
{
    public interface IAddressRepository
    {
        /// <summary>
        /// Получает последний добавленный адрес пользователя
        /// </summary>
        /// <returns>Сущность адреса или null, если адреса не найдены</returns>
        Task<Address> GetLastAddressByUserIdAsync(long userId, CancellationToken ct = default);
    }
}
