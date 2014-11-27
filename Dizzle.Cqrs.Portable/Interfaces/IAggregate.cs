using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dizzle.Cqrs.Portable
{
    public interface IAggregate
    {
        /// <summary>
        /// The unique ID of the aggregate.
        /// </summary>
        AbstractIdentity<Guid> Id { get; set; }
        void ApplyEvents(IEnumerable<IEvent> events);
    }
}
