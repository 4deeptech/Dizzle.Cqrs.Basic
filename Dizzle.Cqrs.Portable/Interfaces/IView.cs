using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dizzle.Cqrs.Portable
{
    public interface IView 
    {
        AbstractIdentity<Guid> Id { get; set; }
    }
}
