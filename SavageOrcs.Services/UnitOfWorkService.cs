using SavageOrcs.UnitOfWork;

namespace SavageOrcs.Services
{
    public class UnitOfWorkService
    {
        protected IUnitOfWork UnitOfWork;

        protected UnitOfWorkService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}