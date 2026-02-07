using MyShopApp.Domain.Common;

namespace MyShopApp.Infrastructure.Common
{
    public class EfRepositoryBase : IRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public EfRepositoryBase(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}
