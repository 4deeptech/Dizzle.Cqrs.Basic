//----------------------------------------------------------------------- 
// <copyright file="TennisEvent.cs" company="4Deep Technologies LLC"> 
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
    public partial class TennisEvent 
    {
        public IEnumerable<IEvent> Handle(CreateTennisEvent c)
        {
            yield return new TennisEventCreated(c.Id,c.USTAEventId,c.StartDate,c.EndDate,c.Name);
        }

        public IEnumerable<IEvent> Handle(UpdateTennisEvent c)
        {
            yield return new TennisEventUpdated(c.Id, c.USTAEventId, c.StartDate, c.EndDate, c.Name);
        }

        public void Apply(TennisEventCreated e)
        {
            Id = e.Id;
            USTAEventId = e.USTAEventId;
            StartDate = e.StartDate;
            EndDate = e.EndDate;
            Name = e.Name;
        }

        public void Apply(TennisEventUpdated e)
        {
            Id = e.Id;
            USTAEventId = e.USTAEventId;
            StartDate = e.StartDate;
            EndDate = e.EndDate;
            Name = e.Name;
        }
    }
}
