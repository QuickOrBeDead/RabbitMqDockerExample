namespace TransferWorker.Infrastructure.Service
{
    using System.Data;

    using Microsoft.EntityFrameworkCore;

    using TransferWorker.Infrastructure.Model;

    public interface IBankService
    {
        void Transfer(TransferModel transfer);
    }

    public sealed class BankService : IBankService
    {
        private readonly TransferDbContext _dbContext;

        public BankService(TransferDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public void Transfer(TransferModel transfer)
        {
            if (transfer == null)
            {
                throw new ArgumentNullException(nameof(transfer));
            }

            Customer GetCustomer(string name)
            {
                var customer = _dbContext.Customers.SingleOrDefault(x => x.Name == name);
                if (customer == null)
                {
                    throw new InvalidOperationException($"'{name}' customer not found");
                }

                return customer;
            }

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                var customerFrom = GetCustomer(transfer.From);
                var customerTo = GetCustomer(transfer.To);

                if (transfer.Amount > customerFrom.Balance)
                {
                    throw new InvalidOperationException("Insufficient balance");
                }

                customerFrom.Balance -= transfer.Amount;
                customerTo.Balance += transfer.Amount;

                _dbContext.Update(customerFrom);
                _dbContext.Update(customerTo);
                _dbContext.SaveChanges();

                transaction.Commit();
            }
        }
    }
}
