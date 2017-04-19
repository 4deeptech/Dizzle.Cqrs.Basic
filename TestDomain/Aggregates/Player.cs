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
using TestDomain.Cqrs.Commands;
using TestDomain.Cqrs.Events;


namespace TestDomain.Cqrs.Model
{
    public partial class Player 
    {
        public IEnumerable<IEvent> Handle(CreatePlayer c)
        {
            yield return new PlayerCreated(c.Id,c.FirstName,c.LastName, null, c.Street);
            
        }

        public IEnumerable<IEvent> Handle(UpdatePlayer c)
        {
            yield return new PlayerUpdated(c.Id, c.FirstName, c.LastName, null,c.Street);
        }

        public void Apply(PlayerCreated e)
        {
            Id = e.Id;
            FirstName = e.FirstName;
            LastName = e.LastName;
            Street = e.Street;
        }

        public void Apply(PlayerUpdated e)
        {
            Id = e.Id;
            FirstName = e.FirstName;
            LastName = e.LastName;
        }
    }
}
