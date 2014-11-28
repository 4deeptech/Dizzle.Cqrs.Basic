using Dizzle.Cqrs.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDomain.Cqrs.Commands;
using TestDomain.Cqrs.Events;


namespace TestDomain.Cqrs.Model
{
    public partial class Player 
    {
        public IEnumerable<IEvent> Handle(CreatePlayer c)
        {
            yield return new PlayerCreated(c.Id,c.FirstName,c.LastName, null);
            
        }

        public IEnumerable<IEvent> Handle(UpdatePlayer c)
        {
            yield return new PlayerUpdated(c.Id, c.FirstName, c.LastName, null);
        }

        public void Apply(PlayerCreated e)
        {
            Id = e.Id;
            FirstName = e.FirstName;
            LastName = e.LastName;
        }

        public void Apply(PlayerUpdated e)
        {
            Id = e.Id;
            FirstName = e.FirstName;
            LastName = e.LastName;
        }
    }
}
