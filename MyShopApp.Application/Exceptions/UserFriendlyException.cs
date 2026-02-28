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

        #region ProductGroups

        public static void PRODUCT_GROUP_WITH_SPECIFIED_ID_WAS_NOT_FOUND(long id) => 
            throw new UserFriendlyException("PRODUCTGROUPS:00001", $"Группа товаров с ID = {id} не найдена.");

        public static void PRODUCT_GROUP_NAME_CAN_NOT_BE_EMPTY() => 
            throw new UserFriendlyException("PRODUCTGROUPS:00002", "Название группы товаров не может быть пустым.");

        public static void PRODUCT_GROUP_WITH_SPECIFIED_NAME_ALREADY_EXISTS(string name) => 
            throw new UserFriendlyException("PRODUCTGROUPS:00003", $"Группа товаров с названием '{name}' уже существует.");

        public static void PRODUCT_ALREADY_IN_GROUP(long productId, long groupId) => 
            throw new UserFriendlyException("PRODUCTGROUPS:00004", $"Товар ID = {productId} уже добавлен в группу ID = {groupId}.");

        public static void PRODUCT_NOT_IN_GROUP(long productId, long groupId) => 
            throw new UserFriendlyException("PRODUCTGROUPS:00005", $"Товар ID = {productId} не найден в группе ID = {groupId}.");

        public static void PRODUCT_GROUP_MUST_HAVE_AT_LEAST_ONE_PRODUCT() => 
            throw new UserFriendlyException("PRODUCTGROUPS:00006", "Группа товаров должна содержать хотя бы один товар.");

        public static void PRODUCT_GROUP_ONLY_ONE_DISCOUNT_METHOD_ALLOWED() => 
            throw new UserFriendlyException("PRODUCTGROUPS:00007", "Должен быть указан только один параметр: PriceWithDiscount, DiscountPercentage или DiscountedAmount.");

        public static void PRODUCT_GROUP_PRICE_WITH_DISCOUNT_CANNOT_BE_GREATER_THAN_TOTAL() => 
            throw new UserFriendlyException("PRODUCTGROUPS:00008", "Цена со скидкой не может быть больше общей стоимости.");

        public static void PRODUCT_GROUP_DISCOUNT_PERCENTAGE_CANNOT_EXCEED_100() => 
            throw new UserFriendlyException("PRODUCTGROUPS:00009", "Процент скидки не может превышать 100%.");

        public static void PRODUCT_GROUP_DISCOUNTED_AMOUNT_CANNOT_BE_GREATER_THAN_TOTAL() => 
            throw new UserFriendlyException("PRODUCTGROUPS:00010", "Сумма скидки не может быть больше общей стоимости.");

        #endregion

        #region Auth

        public static void PHONE_NUMBER_CAN_NOT_BE_EMPTY() =>
            throw new UserFriendlyException("AUTH:00001", "Номер телефона не может быть пустым.");

        public static void INVALID_OR_EXPIRED_CODE() =>
            throw new UserFriendlyException("AUTH:00002", "Неверный или истекший код подтверждения.");

        #endregion

        #region Users

        public static void USER_WITH_SPECIFIED_ID_WAS_NOT_FOUND(long id) =>
            throw new UserFriendlyException("USERS:00001", $"Пользователь с ID {id} не найден.");

        public static void USER_CREATION_FAILED(string errors) =>
            throw new UserFriendlyException("USERS:00002", $"Ошибка создания пользователя: {errors}");

        public static void USER_UPDATE_FAILED(string errors) =>
            throw new UserFriendlyException("USERS:00003", $"Ошибка обновления пользователя: {errors}");

        public static void USER_DELETION_FAILED(string errors) =>
            throw new UserFriendlyException("USERS:00004", $"Ошибка удаления пользователя: {errors}");

        #endregion
    }
}