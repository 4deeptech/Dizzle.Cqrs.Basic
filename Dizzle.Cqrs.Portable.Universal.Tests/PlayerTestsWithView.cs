using Dizzle.Cqrs.Portable;
using Dizzle.Cqrs.Portable.Universal.Tests;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
//using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestDomain.Aggregates;
using TestDomain.Commands;
using TestDomain.Events;
using TestDomain.Views;

namespace TestDomain.Portable.Tests
{
    [TestClass]
    public class PlayerTestsWithView : BDDTestWithView<Player>
    {
        private PlayerId testId;

        [TestInitialize]
        public void Setup()
        {
            testId = new PlayerId(Guid.NewGuid());
        }

        [TestMethod]
        public void Can_Create_New_Player()
        {
            Test<PlayerId, PlayerView>(
                Given(new List<IEvent>()),
                When(new CreatePlayer(testId, "FirstName", "LastName")),
                Then(new PlayerCreated(testId, "FirstName", "LastName")),
                testId,
                new PlayerView { Id = testId, FirstName = "FirstName", LastName = "LastName" }
                );
        }
    }
}
