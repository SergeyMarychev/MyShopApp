namespace MyShopApp.Application.Contracts.Email
{
    /// <summary>
    /// Внутренняя модель для хранения данных Email кода в распределенном кэше
    /// </summary>
    [Serializable]
    public class EmailCodeData
    {
        /// <summary>
        /// Сгенерированный код подтверждения
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Срок действия кода
        /// </summary>
        public DateTime Expiry { get; set; }

        /// <summary>
        /// Количество попыток ввода
        /// </summary>
        public int Attempts { get; set; }

        /// <summary>
        /// Время окончания кулдауна (нельзя запросить новый код до этого времени)
        /// </summary>
        public DateTime? CooldownUntil { get; set; }

        /// <summary>
        /// Email адрес (для связки)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// ID пользователя
        /// </summary>
        public long UserId { get; set; }
    }
}
