//----------------------------------------------------------------------- 
// <copyright file="SQLiteStoreTests.cs" company="4Deep Technologies LLC"> 
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
using Dizzle.Cqrs.Portable;
using Dizzle.Cqrs.Portable.Universal.Tests;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Storage;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;
using Dizzle.Cqrs.Portable.Storage.SQLite.Events;
using TestDomain.Aggregates;
using TestDomain.Events;

namespace Dizzle.Cqrs.Portable.Universal.Tests
{
    [TestClass]
    public class SQLiteStoreTests
    {
        private SQLiteEventStore store = null;
        private string path = null;
        private PlayerId testId;
        private string testPath = "tscoreeventstest.sqlite";

        [TestInitialize]
        public void Setup()
        {
            testId = new PlayerId(Guid.NewGuid());
            StorageFile storageFile = ApplicationData.Current.LocalFolder.CreateFileAsync(testPath, CreationCollisionOption.OpenIfExists).AsTask().Result;
            path = storageFile.Path;
            store = new SQLiteEventStore(storageFile.Path, new SQLitePlatformWinRT());
        }

        [TestMethod]
        public void CanWriteToEventStore()
        {
            List<IEvent> evts = new List<IEvent>(){new PlayerCreated(testId, "FirstName", "LastName")};
            store.SaveEventsFor<Player>(testId.GetId(), (long)evts.Count, evts);
            IEnumerable<IEvent> evtsLoaded = store.LoadEventsFor<Player>(testId.GetId());
            Assert.IsTrue(evtsLoaded.Count() == 1);
            Assert.AreEqual(((PlayerCreated)evts.First()).Id, ((PlayerCreated)evtsLoaded.First()).Id);
        }

        [TestMethod]
        public void CanReadFromEventStore()
        {
            List<IEvent> evts = new List<IEvent>() { new PlayerCreated(testId, "FirstName", "LastName"), new PlayerUpdated(testId, "FirstName2", "LastName2") };
            store.SaveEventsFor<Player>(testId.GetId(), (long)evts.Count, evts);
            IEnumerable<IEvent> evtsLoaded = store.LoadEventsFor<Player>(testId.GetId());
            Assert.IsTrue(evtsLoaded.Count() == 2);
            Assert.AreEqual(((PlayerUpdated)evts.Last()).LastName, ((PlayerUpdated)evtsLoaded.Last()).LastName);
        }

        [TestCleanup()]
        public void Cleanup()
        {
            store.Clear(); 
            store.Close();
        }
    }
}
