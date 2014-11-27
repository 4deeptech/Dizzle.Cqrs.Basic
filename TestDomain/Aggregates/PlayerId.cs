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
