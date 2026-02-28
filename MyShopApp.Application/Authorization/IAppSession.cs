namespace MyShopApp.Application.Authorization
{
    public interface IAppSession
    {
        long? UserId { get; }
    }
}
