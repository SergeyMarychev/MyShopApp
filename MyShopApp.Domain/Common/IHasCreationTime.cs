namespace MyShopApp.Domain.Common
{
    public interface IHasCreationTime
    {
        DateTime CreatedAt { get; set; }
    }
}
