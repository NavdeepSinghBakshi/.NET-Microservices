using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace IntegrationEventLogEF.Utilities
{
    public class ResilientTransaction
    {
        private readonly DbContext context;
        private ResilientTransaction(DbContext context) =>
            this.context = context ?? throw new ArgumentNullException(nameof(context));

        public static ResilientTransaction New(DbContext context) =>
            new ResilientTransaction(context);

        public async Task ExecuteAsync(Func<Task> action)
        {
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    await action();
                    transaction.Commit();
                }
            });
        }
    }
}