using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransSaga.Datastore.Context
{
    using System.Data.Entity.Infrastructure;
    using System.Threading;

    using MassTransit.EntityFrameworkIntegration;

    using MassTransSaga.Datastore.Mappings;
    using MassTransSaga.Model;

    public class TheGreatSagaDbContext : SagaDbContext<TheGreatState, TheGreatStateMapping>
    {
        public TheGreatSagaDbContext()
            : base("Saga")
        {
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {

                throw;
            }
        }
    }
}
