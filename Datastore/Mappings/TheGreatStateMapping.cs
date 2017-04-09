using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransSaga.Datastore.Mappings
{
    using MassTransit.EntityFrameworkIntegration;

    using MassTransSaga.Model;

    public class TheGreatStateMapping : SagaClassMapping<TheGreatState>
    {
        public TheGreatStateMapping()
        {
            Property(x => x.CurrentState).IsUnicode(true).HasMaxLength(64);
            Property(x => x.Count);
            Property(x => x.ExpirationId).IsOptional();
            Property(x => x.Magic).HasColumnType("nvarchar(max)");
            Property(x => x.RowVersion).IsRowVersion(); // For Optimistic Concurrency
        }
    }
}
