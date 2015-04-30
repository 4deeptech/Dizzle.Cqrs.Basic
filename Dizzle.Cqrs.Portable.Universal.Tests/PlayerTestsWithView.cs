//----------------------------------------------------------------------- 
// <copyright file="PlayerTestsWithView.cs" company="4Deep Technologies LLC"> 
// Copyright (c) 4Deep Technologies LLC. All rights reserved. 
// <author>Darren Ford</author> 
// <date>Thursday, April 30, 2015 3:00:44 PM</date> 
// </copyright> 
//-----------------------------------------------------------------------


using Dizzle.Cqrs.Portable;
using Dizzle.Cqrs.Portable.Universal.Tests;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
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
        public void Universal_Can_Create_New_Player()
        {
            Test<PlayerId, PlayerView>(
                Given(new List<IEvent>()),
                When(new CreatePlayer(testId, "FirstName", "LastName")),
                Then(new PlayerCreated(testId, "FirstName", "LastName")),
                testId,
                new PlayerView { Id = testId, FirstName = "FirstName", LastName = "LastName" }
                );
        }

        [TestMethod]
        public void Universal_Can_Update_New_Player()
        {
            Test<PlayerId, PlayerView>(
                Given(new List<IEvent>(){new PlayerCreated(testId, "FirstName", "LastName")}),
                When(new UpdatePlayer(testId, "FirstName2", "LastName2")),
                Then(new PlayerUpdated(testId, "FirstName2", "LastName2")),
                testId,
                new PlayerView { Id = testId, FirstName = "FirstName2", LastName = "LastName2" }
                );
        }
    }
}
