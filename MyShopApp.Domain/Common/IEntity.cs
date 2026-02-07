namespace MyShopApp.Domain.Common
{
    public interface IEntity<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }
    }

    public interface IEntity : IEntity<long>
    {
    }
}
