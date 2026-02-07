namespace MyShopApp.Application.Contracts.Common.Dto
{
    public abstract class EntityDto<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }
    }

    public abstract class EntityDto : EntityDto<long>
    {
    }
}
