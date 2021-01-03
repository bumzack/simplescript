using System.Threading.Tasks;

namespace exampleservice.AccoutingService.Controller
{
    public interface IAccountServiceDataBaseRepository
    {
        Task<int> CreateAccount(int CustomerId);

        Task<bool> ChangeAmount(int AccountId, int Amount);

        Task<int> GetAmount(int AccountId);
    }
}
