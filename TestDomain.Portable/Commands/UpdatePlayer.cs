using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TestDomain.Aggregates;

namespace TestDomain.Commands
{
    public class UpdatePlayer
    {
        [DataMember(Order = 1)]
        public PlayerId Id { get; set; }

        [DataMember(Order = 2)]
        public string FirstName { get; set; }

        [DataMember(Order = 3)]
        public string LastName { get; set; }


        public UpdatePlayer(PlayerId id, string firstName, string lastName)
        {
            Id = id; 
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
