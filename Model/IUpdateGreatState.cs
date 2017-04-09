using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransSaga.Model
{
    public interface IUpdateGreatState
    {
        Guid EventId { get; }

        string Magic { get; }
    }
}
