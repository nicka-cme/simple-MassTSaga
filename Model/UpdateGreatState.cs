using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransSaga.Model
{
    public class UpdateGreatState : IUpdateGreatState
    {
        public UpdateGreatState(Guid eventId, string magic)
        {
            this.EventId = eventId;
            this.Magic = magic;
        }

        public Guid EventId { get; }

        public string Magic { get; }
    }
}
