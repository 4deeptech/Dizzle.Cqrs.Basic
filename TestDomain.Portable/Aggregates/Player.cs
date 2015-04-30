//----------------------------------------------------------------------- 
// <copyright file="Player.cs" company="4Deep Technologies LLC"> 
// Copyright (c) 4Deep Technologies LLC. All rights reserved. 
// <author>Darren Ford</author> 
// <date>Thursday, April 30, 2015 3:00:44 PM</date> 
// </copyright> 
//-----------------------------------------------------------------------


using Dizzle.Cqrs.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDomain.Aggregates;
using TestDomain.Commands;
using TestDomain.Events;

namespace TestDomain.Aggregates
{
    public class Player : Aggregate, 
        IHandleCommand<CreatePlayer>,
        IHandleCommand<UpdatePlayer>,
        IApplyEvent<PlayerCreated>,
        IApplyEvent<PlayerUpdated>
    {
        public new PlayerId Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Player()
        {
        }



        public IEnumerable<IEvent> Handle(CreatePlayer c)
        {
            yield return new PlayerCreated(c.Id,c.FirstName,c.LastName);
            
        }

        public IEnumerable<IEvent> Handle(UpdatePlayer c)
        {
            yield return new PlayerUpdated(c.Id, c.FirstName, c.LastName);
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
