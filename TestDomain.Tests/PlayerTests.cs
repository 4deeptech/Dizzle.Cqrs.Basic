using Dizzle.Cqrs.Portable;
using Dizzle.Cqrs.Portable.Testing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestDomain.Cqrs.Commands;
using TestDomain.Cqrs.Events;
using TestDomain.Cqrs.Model;

namespace TestDomain.Tests
{
    [TestFixture]
    public class PlayerTests : BDDTest<Player>
    {
        private PlayerId testId;
        [SetUp]
        public void Setup()
        {
            testId = new PlayerId(Guid.NewGuid());
        }

        [Test]
        public void Can_Create_New_Player()
        {
            Test(
                Given(new List<IEvent>()),
                When(new CreatePlayer(testId,"FirstName","LastName",null)),
                Then(new PlayerCreated(testId, "FirstName", "LastName", null))
                );
        }

        [Test]
        public void Can_Update_New_Player()
        {
            Test(
                Given(new List<IEvent> { 

                    new PlayerCreated(testId, "FirstName", "LastName", null),
                    new PlayerUpdated(testId, "FirstName2", "LastName2", null), 
                    new PlayerUpdated(testId, "FirstName3", "LastName3", null) }),

                When(new UpdatePlayer(testId, "FirstName4", "LastName4", null)),

                Then(new PlayerUpdated(testId, "FirstName4", "LastName4", null))

                );
        }
    }
}
