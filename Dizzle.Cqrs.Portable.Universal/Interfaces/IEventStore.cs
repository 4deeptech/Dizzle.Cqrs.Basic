﻿//----------------------------------------------------------------------- 
// <copyright file="IEventStore.cs" company="4Deep Technologies LLC"> 
// Copyright (c) 4Deep Technologies LLC. All rights reserved. 
// <author>Darren Ford</author> 
// <date>Thursday, April 30, 2015 3:00:44 PM</date> 
// </copyright> 
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Dizzle.Cqrs.Portable
{
    public interface IEventStore
    {
        IEnumerable<IEvent> LoadEventsFor<TAggregate>(string streamName);
        IEnumerable<IEvent> LoadEventsFor<TAggregate>(string streamName, long afterVersion);
        void SaveEventsFor<TAggregate>(string streamName, long eventsLoaded, IEnumerable<IEvent> newEvents);
    }
}
