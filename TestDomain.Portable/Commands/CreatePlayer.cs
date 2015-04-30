//----------------------------------------------------------------------- 
// <copyright file="CreatePlayer.cs" company="4Deep Technologies LLC"> 
// Copyright (c) 4Deep Technologies LLC. All rights reserved. 
// <author>Darren Ford</author> 
// <date>Thursday, April 30, 2015 3:00:44 PM</date> 
// </copyright> 
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TestDomain.Aggregates;

namespace TestDomain.Commands
{
    public class CreatePlayer
    {
        [DataMember(Order = 1)]
        public PlayerId Id { get; set; }

        [DataMember(Order = 2)]
        public string FirstName { get; set; }

        [DataMember(Order = 3)]
        public string LastName { get; set; }


        public CreatePlayer(PlayerId id, string firstName, string lastName)
        {
            Id = id; 
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
