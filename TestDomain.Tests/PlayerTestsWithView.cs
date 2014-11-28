using Dizzle.Cqrs.Portable;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestDomain.Cqrs.Commands;
using TestDomain.Cqrs.Events;
using TestDomain.Cqrs.Model;
using TestDomain.Cqrs.Views;

namespace TestDomain.Tests
{
    [TestFixture]
    public class PlayerTestsWithView : BDDTestWithView<Player>
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
            Test<PlayerId, PlayerView>(
                Given(new List<IEvent>()),
                When(new CreatePlayer(testId, "FirstName", "LastName",null)),
                Then(new PlayerCreated(testId, "FirstName", "LastName",null)),
                testId,
                new PlayerView { Id = testId, FirstName = "FirstName", LastName = "LastName" }
                );
        }

        //[Test]
        //public void Can_Update_New_Player()
        //{
        //    Test(
        //        Given(new List<IEvent> { new PlayerCreated(testId, "FirstName", "LastName"), new PlayerUpdated(testId, "FirstName2", "LastName2"), new PlayerUpdated(testId, "FirstName3", "LastName3") }),
        //        When(new UpdatePlayer(testId, "FirstName4", "LastName4")),
        //        Then(new PlayerUpdated(testId, "FirstName4", "LastName4"))
        //        );
        //}
    }
}
