using MyShopApp.Domain.Categories;
using MyShopApp.Domain.Common;
using MyShopApp.Domain.ProductGroups;

namespace MyShopApp.Domain.Products
{
    public sealed class Product : Entity, IHasCreationTime
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public long CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string Image { get; set; } = string.Empty;
        public ICollection<ProductGroupProduct> ProductGroupProducts { get; set; } = new List<ProductGroupProduct>();
        public int Value { get; set; } //Вес, литраж, штуки
        public string Ingredients { get; set; } = string.Empty; //Состав
        public string StorageLife { get; set; } = string.Empty; //Срок хранения
        public string StorageConditions { get; set; } = string.Empty; //Условия хранения
        public string Manufacturer { get; set; } = string.Empty; //Производитель
        public string ProductType { get; set; } = string.Empty; //Тип товара
        public string Brand { get; set; } = string.Empty; //Бренд
        public bool IsVegan { get; set; } //Веган да/нет
        public bool IsSugarFree { get; set; } //Без сахара да/нет
        public bool IsGlutenFree { get; set; } //Без глютена да/нет
        public bool IsHot { get; set; } //Приедет горячим да/нет
        public string Note { get; set; } = string.Empty; //Примечание
        public string Kilocalories { get; set; } = string.Empty; //Килокалории
        public string Proteins { get; set; } = string.Empty; //Белки
        public string Fats { get; set; } = string.Empty; //Жиры
        public string Carbohydrates { get; set; } = string.Empty; //Углеводы
    }
}
