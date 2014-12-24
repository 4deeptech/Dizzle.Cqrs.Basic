using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
//using NUnit.Framework;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Dizzle.Cqrs.Portable.Storage;
using System.Collections.Concurrent;
using Dizzle.Cqrs.Universal.Storage;
using Dizzle.Cqrs.Portable.Storage.SQLite;
using SQLite.Net.Platform.WinRT;

namespace Dizzle.Cqrs.Portable.Universal.Tests
{
    /// <summary>
    /// Provides infrastructure for a set of tests on a given aggregate.
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public class BDDTestWithView<TAggregate>
        where TAggregate : Aggregate, new()
    {
        private TAggregate sut;
        
        private IDocumentStore _docStore = null;
        private Dictionary<Type, IProjection> _cache = new Dictionary<Type, IProjection>();

        [Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitialize]
        public void BDDTestSetup()
        {
            sut = new TAggregate();
            //ConcurrentDictionary<string, ConcurrentDictionary<string, byte[]>> _store = new ConcurrentDictionary<string,ConcurrentDictionary<string,byte[]>>();
            //_docStore = new MemoryDocumentStore(_store, new ViewStrategy());
            string path = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            string testPath = "tscoreviews.sqlite";
            //_docStore = new IsolatedStorageDocumentStore(testPath, new ViewStrategy());
            StorageFile storageFile = ApplicationData.Current.LocalFolder.CreateFileAsync(testPath, CreationCollisionOption.OpenIfExists).AsTask().Result;
            _docStore = new SQLiteDocumentStore(storageFile.Path, new SQLitePlatformWinRT(), new ViewStrategy());
            //scan for ViewProjection handlers
            //_docStore.ResetAll();
            AssemblyName name = new AssemblyName("TestDomain.Portable, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            System.Reflection.Assembly asm = System.Reflection.Assembly.Load(name);
            ScanForProjections(asm);
        }

        public void ScanForProjections(Assembly ass)
        {
            // Scan for and register projections
            var projections =
                from t in ass.DefinedTypes
                where t.BaseType.Name.Equals("AbstractBaseProjection")
                select t;
            foreach (var typ in projections)
            {
                var instance = CreateInstanceOf(typ.AsType());
                //we have our instance of the projection
                //now we need to call the GetWriter method on the doc store to get the defined writer for the type defined SetWriter call on the projection
                var m = _docStore.GetType().GetTypeInfo().DeclaredMethods.Single(t=>t.Name.Equals("GetWriter"));
                var x = m.GetGenericArguments();
                var m2 = instance.GetType().GetTypeInfo().DeclaredMethods.Single(t => t.Name.Equals("SetWriter"));
                var x2 = m2.GetParameters()[0].ParameterType.GenericTypeArguments;
                MethodInfo method = m.MakeGenericMethod(new Type[] { x2[0], x2[1] });

                var writer = method.Invoke(_docStore, new object[] { });
                m2.Invoke(instance, new object[] { writer });
                _cache.Add(typ.AsType(), (IProjection)instance);
            }
        }

        /// <summary>
        /// Creates an instance of the specified type. If you are using some kind
        /// of DI container, and want to use it to create instances of the handler
        /// or subscriber, you can plug it in here.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private object CreateInstanceOf(Type t)
        {
            return Activator.CreateInstance(t);
        }

        public void ExecuteProjectionEventHandlers(IEvent evt)
        {
            foreach(var handler in _cache.Values)
            {
                var subscriber = from m in handler.GetType().GetTypeInfo().DeclaredMethods
                                 where m.Name.Equals("Apply")
                                 where m.GetParameters()[0].ParameterType.Name.Equals(evt.GetType().Name)
                                 select m;
                subscriber.First().Invoke(handler, new object[] { evt });
            }
        }

        protected void Test<TKey, TEntity>(IEnumerable<IEvent> given, Func<TAggregate, object> when, Action<object> then,TKey id, TEntity view)
        {
            then(when(ApplyEvents(sut, given)));
            TEntity _view = default(TEntity);
            var docreader = _docStore.GetReader<TKey, TEntity>();
            if (docreader.TryGet(id, out _view))
            {
                Assert.AreEqual(Serialize(view), Serialize(_view));
            }
            else
            {
                Assert.Fail("Unable to get the projected view");
            }
            
        }

        protected IEnumerable<IEvent> Given(List<IEvent> events)
        {
            //run init events to the view projections
            foreach (IEvent e in events)
            {
                ExecuteProjectionEventHandlers(e);
            }
            return events as IEnumerable<IEvent>;
        }

        protected Func<TAggregate, object> When<TCommand>(TCommand command)
        {
            return agg =>
            {
                try
                {
                    return DispatchCommand(command).Cast<object>().ToArray();
                }
                catch (Exception e)
                {
                    return e;
                }
            };
        }

        protected Action<object> Then(params object[] expectedEvents)
        {
            return got =>
            {
                var gotEvents = got as object[];
                if (gotEvents != null)
                {
                    if (gotEvents.Length == expectedEvents.Length)
                        for (var i = 0; i < gotEvents.Length; i++)
                            if (gotEvents[i].GetType() == expectedEvents[i].GetType())
                                Assert.AreEqual(Serialize(expectedEvents[i]), Serialize(gotEvents[i]));
                            else
                                Assert.Fail(string.Format(
                                    "Incorrect event in results; expected a {0} but got a {1}",
                                    expectedEvents[i].GetType().Name, gotEvents[i].GetType().Name));
                    else if (gotEvents.Length < expectedEvents.Length)
                        Assert.Fail(string.Format("Expected event(s) missing: {0}",
                            string.Join(", ", EventDiff(expectedEvents, gotEvents))));
                    else
                        Assert.Fail(string.Format("Unexpected event(s) emitted: {0}",
                            string.Join(", ", EventDiff(gotEvents, expectedEvents))));
                }
                else if (got is CommandHandlerNotDefinedException)
                    Assert.Fail((got as Exception).Message);
                else
                    Assert.Fail("Expected events, but got exception {0}",
                        got.GetType().Name);
            };
        }

        private string[] EventDiff(object[] a, object[] b)
        {
            var diff = a.Select(e => e.GetType().Name).ToList();
            foreach (var remove in b.Select(e => e.GetType().Name))
                diff.Remove(remove);
            return diff.ToArray();
        }

        protected Action<object> ThenFailWith<TException>()
        {
            return got =>
            {
                if (got is TException)
                {
                    //Assert.Pass("Got correct exception type");
                }
                else if (got is CommandHandlerNotDefinedException)
                    Assert.Fail((got as Exception).Message);
                else if (got is Exception)
                    Assert.Fail(string.Format(
                        "Expected exception {0}, but got exception {1}",
                        typeof(TException).Name, got.GetType().Name));
                else
                    Assert.Fail(string.Format(
                        "Expected exception {0}, but got event result",
                        typeof(TException).Name));
            };
        }

        private IEnumerable<IEvent> DispatchCommand<TCommand>(TCommand c)
        {
            var handler = sut as IHandleCommand<TCommand>;
            if (handler == null)
                throw new CommandHandlerNotDefinedException(string.Format(
                    "Aggregate {0} does not yet handle command {1}",
                    sut.GetType().Name, c.GetType().Name));
            var events = handler.Handle(c);
            //dispatch events to the view projections
            foreach (IEvent e in events)
            {
                ExecuteProjectionEventHandlers(e);
            }
            return events;
        }

        private TAggregate ApplyEvents(TAggregate agg, IEnumerable<IEvent> events)
        {
            agg.ApplyEvents(events);
            return agg;
        }

        private string Serialize(object obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.SerializeObject(obj, settings);
        }

        private class CommandHandlerNotDefinedException : Exception
        {
            public CommandHandlerNotDefinedException(string msg) : base(msg) { }
        }
    }
}
