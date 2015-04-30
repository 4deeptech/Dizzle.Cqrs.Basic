﻿//----------------------------------------------------------------------- 
// <copyright file="IAggregate.cs" company="4Deep Technologies LLC"> 
// Copyright (c) 4Deep Technologies LLC. All rights reserved. 
// <author>Darren Ford</author> 
// <date>Thursday, April 30, 2015 3:00:44 PM</date> 
// </copyright> 
//-----------------------------------------------------------------------


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
