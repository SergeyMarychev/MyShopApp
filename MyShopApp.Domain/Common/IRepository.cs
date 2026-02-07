namespace MyShopApp.Domain.Common
{
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; set; }
    }
}
