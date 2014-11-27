using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Dizzle.Cqrs.Portable
{
    public interface IHandleCommand<TCommand>
    {
        IEnumerable<IEvent> Handle(TCommand c);
    }
}
