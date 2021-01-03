using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace exampleservice.AccoutingService.Controller
{
    public class AccountServiceDataBaseRepository : IAccountServiceDataBaseRepository
    {
        private Dictionary<int, int> db;

        public AccountServiceDataBaseRepository()
        {
            db = new Dictionary<int, int>();
        }

        public Task<int> CreateAccount(int CustomerId)
        {
            db.Add(CustomerId, 0);

            // AccountId == CustomerId for simplicity
            return Task.FromResult(CustomerId);
        }

        public Task<bool> ChangeAmount(int AccountId, int Amount)
        {
            int currentAmount = 0;
            if (db.TryGetValue(AccountId, out currentAmount) && currentAmount >= Amount)
            {
                db[AccountId] = currentAmount - Amount;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<int> GetAmount(int AccountId)
        {
            int currentAmount = 0;
            db.TryGetValue(AccountId, out currentAmount);
            return Task.FromResult(currentAmount);
        }
    }
}