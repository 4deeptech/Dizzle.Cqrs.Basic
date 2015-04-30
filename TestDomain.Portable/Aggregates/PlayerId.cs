//----------------------------------------------------------------------- 
// <copyright file="PlayerId.cs" company="4Deep Technologies LLC"> 
// Copyright (c) 4Deep Technologies LLC. All rights reserved. 
// <author>Darren Ford</author> 
// <date>Thursday, April 30, 2015 3:00:44 PM</date> 
// </copyright> 
//-----------------------------------------------------------------------


using Dizzle.Cqrs.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TestDomain.Aggregates
{
    [DataContract(Namespace = "TScore")]
    public sealed class PlayerId : AbstractIdentity<Guid>
    {
        public const string TagValue = "player";

        public PlayerId()
        {
        }

        public PlayerId(Guid id)
        {
            //Contract.Requires(id > 0);
            Id = id;
        }

        public override string GetTag()
        {
            return TagValue;
        }


        [DataMember(Order = 1)]
        public override Guid Id { get; protected set; }


    }
}
