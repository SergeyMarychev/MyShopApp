namespace MyShopApp.Application.Contracts.Authorization
{
    public class AccountSettings
    {
        /// <summary>
        /// Количество дней для восстановления удаленного аккаунта
        /// </summary>
        public int AccountRecoveryDays { get; set; }
    }
}
