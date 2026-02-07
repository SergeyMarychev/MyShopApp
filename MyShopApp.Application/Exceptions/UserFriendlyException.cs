namespace MyShopApp.Application.Exceptions
{
    public class UserFriendlyException : Exception
    {
        public string Code { get; set; }

        public UserFriendlyException(string? message) : base(message)
        {
            Code = "UF:BASE";
        }

        public UserFriendlyException(string code, string message) : base(message)
        {
            Code = code;
        }

        #region Categories

        public static void CATEGORY_NAME_CAN_NOT_BE_EMPTY() =>
            throw new UserFriendlyException("CATEGORIES:00001", "Имя категории не может быть пустым.");

        public static void CATEGORY_WITH_SPECIFIED_ID_WAS_NOT_FOUND(long id) =>
            throw new UserFriendlyException("CATEGORIES:00002", "Категория с указанным ID не найдена.");

        public static void CATEGORY_WITH_NAME_ALREADY_EXISTS(string name) =>
            throw new UserFriendlyException("CATEGORIES:00003", $"Категория с именем '{name}' уже существует.");

        #endregion

        #region Products

        public static void PRODUCT_NAME_CAN_NOT_BE_EMPTY() =>
            throw new UserFriendlyException("PRODUCTS:00001", "Имя продукта не может быть пустым.");

        public static void PRODUCT_WITH_SPECIFIED_ID_WAS_NOT_FOUND(long id) =>
            throw new UserFriendlyException("PRODUCTS:00002", "Продукт с указанным ID не найден.");

        //public static void PRODUCT_WITH_NAME_ALREADY_EXISTS(string name) =>
        //    throw new UserFriendlyException("PRODUCTS:00003", $"Продукт с именем '{name}' уже существует.");

        #endregion
    }
}