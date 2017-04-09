using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransSaga
{
    using Automatonymous;

    using MassTransit;

    using MassTransSaga.Model;

    using NLog;

    public class TheGreatSaga : MassTransitStateMachine<TheGreatState>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public TheGreatSaga()
        {
            InstanceState(x => x.CurrentState);

            Event(() => this.Received, x => x.CorrelateById(c => c.Message.EventId).SelectId(c => c.Message.EventId));

            this.Initially(
               When(this.Received)
                   .Then(c => Logger.Info($"The Great State Saga - Started - EventId: {c.Data.EventId}"))
                   .Then(SaveState)
                   .TransitionTo(this.Active));

            this.During(
              this.Active,
              When(this.Received)
                  .Then(c => Logger.Info($"The Great State - Updated - EventId: {c.Data.EventId}"))
                  .Then(UpdateState));
        }

        public State Active { get; private set; }

        public Event<IUpdateGreatState> Received { get; private set; }

        private void SaveState(BehaviorContext<TheGreatState, IUpdateGreatState> c)
        {
            var state = c.Instance;
            var data = c.Data;

            state.Magic = data.Magic;
            state.Count++;
        }

        private void UpdateState(BehaviorContext<TheGreatState, IUpdateGreatState> c)
        {
            var state = c.Instance;
            state.Count++;
        }
    }
}
