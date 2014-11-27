using Dizzle.Cqrs.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDomain.Aggregates;
using TestDomain.Events;
using TestDomain.Views;
using Dizzle.Cqrs.Portable.Storage;

namespace TestDomain.Projections
{
    public sealed class PlayerViewProjection : AbstractBaseProjection,
        IApplyEvent<PlayerCreated>,
        IApplyEvent<PlayerUpdated>
    {
        protected IDocumentWriter<PlayerId, PlayerView> _writer;

        public PlayerViewProjection()
        {
        }

        public PlayerViewProjection(IDocumentWriter<PlayerId,PlayerView> writer)
        {
            _writer = writer;
        }

        public void SetWriter(IDocumentWriter<PlayerId, PlayerView> writer)
        {
            _writer = writer;
        }

        public void Apply(PlayerCreated e)
        {
            _writer.Add(e.Id, new PlayerView
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName
            });
        }

        public void Apply(PlayerUpdated e)
        {
            _writer.UpdateOrThrow(e.Id, pv =>
            {
                pv.Id = e.Id;
                pv.FirstName = e.FirstName;
                pv.LastName = e.LastName;
            });
        }
    }
}
