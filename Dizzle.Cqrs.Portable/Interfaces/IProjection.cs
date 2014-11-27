using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dizzle.Cqrs.Portable
{
    public interface IProjection
    {
        void ApplyOneEvent<TEvent>(TEvent ev);
    }
}
