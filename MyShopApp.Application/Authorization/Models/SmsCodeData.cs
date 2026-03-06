namespace MyShopApp.Application.Authorization.Models
{
    /// <summary>
    /// Внутренняя модель для хранения данных SMS кода в распределенном кэше
    /// </summary>
    [Serializable]
    public class SmsCodeData
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
        /// Номер телефона (для связки)
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// true - режим восстановления, false - обычный вход
        /// </summary>
        public bool IsRestoreMode { get; set; } 
    }
}