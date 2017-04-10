using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransSaga.Model
{
    using Automatonymous;
    using Automatonymous.States;

    public class TheGreatState : SagaStateMachineInstance
    {
        public string CurrentState { get; set; }

        public Guid CorrelationId { get; set; }

        public Guid? ExpirationId { get; set; }

        public string Magic { get; set; }

        public int Count { get; set; }

        public byte[] RowVersion { get; set; } // For Optimistic Concurrency
    }
}
