 

#region (c) 2015 4Deep Technologies LLC

// Copyright (c) 4Deep Technologies LLC 2015 (http://www.4deeptech.com)
// Created by Darren Ford

//Generated using PDizzle DSL Model and Code Generator
//see http://www.4deeptech.com/blog/post/2012/02/16/Creating-a-DSL-for-DDD-and-Event-Sourcing-Code-Generation.aspx
#endregion
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using Dizzle.Cqrs.Portable;
using TestDomain.Cqrs.Events;
using TestDomain.Cqrs.Commands;
using TestDomain.Cqrs.Model; 

namespace TestDomain.Cqrs.Model
{
	/// <summary>
	/// A tennis event to associate with matches
	/// </summary> 
	public partial class TennisEvent : Aggregate,
	IHandleCommand<CreateTennisEvent>

	,IHandleCommand<UpdateTennisEvent>
,
	IApplyEvent<TennisEventCreated>

	,IApplyEvent<TennisEventUpdated>

	{
		/// <summary>
		/// The id for the event
		/// </summary>
		[DataMember(Order = 1)]
        public TennisEventId Id { get; internal set; }

		/// <summary>
		/// If a USTA event is represented
		/// </summary>
		[DataMember(Order = 2)]
        public String USTAEventId { get; internal set; }

		/// <summary>
		/// The date the event starts
		/// </summary>
		[DataMember(Order = 3)]
        public DateTime? StartDate { get; internal set; }

		/// <summary>
		/// The date the event ends
		/// </summary>
		[DataMember(Order = 4)]
        public DateTime? EndDate { get; internal set; }

		/// <summary>
		/// The name of the event
		/// </summary>
		[DataMember(Order = 5)]
        public String Name { get; internal set; }


		public TennisEvent()
        {

        }

	}//end aggregateroot

	#region Aggregate Id

	[DataContract(Namespace = "TestDomain.Cqrs")]
    public sealed class TennisEventId : AbstractIdentity<Guid>
    {
        public const string TagValue = "player";

        public TennisEventId()
        {
        }

        public TennisEventId(Guid id)
        {
            Id = id;
        }

        public override string GetTag()
        {
            return TagValue;
        }


        [DataMember(Order = 1)]
        public override Guid Id { get; protected set; }


    }

	#endregion

	#region entities
	#endregion
}	

namespace TestDomain.Cqrs.Commands 
{
	/// <summary>
	/// Create a new event to associate with a match or set of matches
	/// </summary>
	[DataContract(Namespace = "TestDomain.Cqrs")]
    public partial class CreateTennisEvent
    {

		/// <summary>
		/// The id for the event
		/// </summary>
		[DataMember(Order = 1)] 
        public TennisEventId Id { get; private set; }

		/// <summary>
		/// If a USTA event is represented
		/// </summary>
		[DataMember(Order = 2)] 
        public String USTAEventId { get; private set; }

		/// <summary>
		/// The date the event starts
		/// </summary>
		[DataMember(Order = 3)] 
        public DateTime? StartDate { get; private set; }

		/// <summary>
		/// The date the event ends
		/// </summary>
		[DataMember(Order = 4)] 
        public DateTime? EndDate { get; private set; }

		/// <summary>
		/// The name of the event
		/// </summary>
		[DataMember(Order = 5)] 
        public String Name { get; private set; }

		public CreateTennisEvent() 
        {
            
        }

        public CreateTennisEvent(TennisEventId id, String uSTAEventId, DateTime? startDate, DateTime? endDate, String name)
        {
            Id = id;
			USTAEventId = uSTAEventId;
			StartDate = startDate;
			EndDate = endDate;
			Name = name;
        }

        public override string ToString()
        {
            return string.Format("Type: {0}, Id {1}, USTAEventId {2}, StartDate {3}, EndDate {4}, Name {5}",GetType().Name, Id, USTAEventId, StartDate, EndDate, Name);
        }
    }//end CreateTennisEvent class


	/// <summary>
	/// Update a tennis event
	/// </summary>
	[DataContract(Namespace = "TestDomain.Cqrs")]
    public partial class UpdateTennisEvent
    {

		/// <summary>
		/// The id for the event
		/// </summary>
		[DataMember(Order = 1)] 
        public TennisEventId Id { get; private set; }

		/// <summary>
		/// If a USTA event is represented
		/// </summary>
		[DataMember(Order = 2)] 
        public String USTAEventId { get; private set; }

		/// <summary>
		/// The date the event starts
		/// </summary>
		[DataMember(Order = 3)] 
        public DateTime? StartDate { get; private set; }

		/// <summary>
		/// The date the event ends
		/// </summary>
		[DataMember(Order = 4)] 
        public DateTime? EndDate { get; private set; }

		/// <summary>
		/// The name of the event
		/// </summary>
		[DataMember(Order = 5)] 
        public String Name { get; private set; }

		public UpdateTennisEvent() 
        {
            
        }

        public UpdateTennisEvent(TennisEventId id, String uSTAEventId, DateTime? startDate, DateTime? endDate, String name)
        {
            Id = id;
			USTAEventId = uSTAEventId;
			StartDate = startDate;
			EndDate = endDate;
			Name = name;
        }

        public override string ToString()
        {
            return string.Format("Type: {0}, Id {1}, USTAEventId {2}, StartDate {3}, EndDate {4}, Name {5}",GetType().Name, Id, USTAEventId, StartDate, EndDate, Name);
        }
    }//end UpdateTennisEvent class


}//end namespace

namespace TestDomain.Cqrs.Events
{
	/// <summary>
	/// A tennis event was created
	/// </summary>
	[DataContract(Namespace = "TestDomain.Cqrs")]
    public partial class TennisEventCreated : IEvent
    {

		/// <summary>
		/// The id for the event
		/// </summary>
		[DataMember(Order = 1)] 
        public TennisEventId Id { get; private set; }

		/// <summary>
		/// If a USTA event is represented
		/// </summary>
		[DataMember(Order = 2)] 
        public String USTAEventId { get; private set; }

		/// <summary>
		/// The date the event starts
		/// </summary>
		[DataMember(Order = 3)] 
        public DateTime? StartDate { get; private set; }

		/// <summary>
		/// The date the event ends
		/// </summary>
		[DataMember(Order = 4)] 
        public DateTime? EndDate { get; private set; }

		/// <summary>
		/// The name of the event
		/// </summary>
		[DataMember(Order = 5)] 
        public String Name { get; private set; }

		public TennisEventCreated() 
        {
            
        }

        public TennisEventCreated(TennisEventId id, String uSTAEventId, DateTime? startDate, DateTime? endDate, String name)
        {
            Id = id;
			USTAEventId = uSTAEventId;
			StartDate = startDate;
			EndDate = endDate;
			Name = name;
        }

        public override string ToString()
        {
            return string.Format("Type: {0}, Id {1}, USTAEventId {2}, StartDate {3}, EndDate {4}, Name {5}",GetType().Name, Id, USTAEventId, StartDate, EndDate, Name);
        }
    }//end TennisEventCreated class


	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "TestDomain.Cqrs")]
    public partial class TennisEventUpdated : IEvent
    {

		/// <summary>
		/// The id for the event
		/// </summary>
		[DataMember(Order = 1)] 
        public TennisEventId Id { get; private set; }

		/// <summary>
		/// If a USTA event is represented
		/// </summary>
		[DataMember(Order = 2)] 
        public String USTAEventId { get; private set; }

		/// <summary>
		/// The date the event starts
		/// </summary>
		[DataMember(Order = 3)] 
        public DateTime? StartDate { get; private set; }

		/// <summary>
		/// The date the event ends
		/// </summary>
		[DataMember(Order = 4)] 
        public DateTime? EndDate { get; private set; }

		/// <summary>
		/// The name of the event
		/// </summary>
		[DataMember(Order = 5)] 
        public String Name { get; private set; }

		public TennisEventUpdated() 
        {
            
        }

        public TennisEventUpdated(TennisEventId id, String uSTAEventId, DateTime? startDate, DateTime? endDate, String name)
        {
            Id = id;
			USTAEventId = uSTAEventId;
			StartDate = startDate;
			EndDate = endDate;
			Name = name;
        }

        public override string ToString()
        {
            return string.Format("Type: {0}, Id {1}, USTAEventId {2}, StartDate {3}, EndDate {4}, Name {5}",GetType().Name, Id, USTAEventId, StartDate, EndDate, Name);
        }
    }//end TennisEventUpdated class


}//end namespace

namespace TestDomain.Cqrs.Views 
{
	/// <summary>
	/// The view for a given event
	/// </summary>
    public partial class TennisEventView
    {

		/// <summary>
		/// The id for the event
		/// </summary>
		[DataMember(Order = 1)] 
        public TennisEventId Id { get; set; }

		/// <summary>
		/// If a USTA event is represented
		/// </summary>
		[DataMember(Order = 2)] 
        public String USTAEventId { get; set; }

		/// <summary>
		/// The date the event starts
		/// </summary>
		[DataMember(Order = 3)] 
        public DateTime? StartDate { get; set; }

		/// <summary>
		/// The date the event ends
		/// </summary>
		[DataMember(Order = 4)] 
        public DateTime? EndDate { get; set; }

		/// <summary>
		/// The name of the event
		/// </summary>
		[DataMember(Order = 5)] 
        public String Name { get; set; }

		public TennisEventView() 
        {
            
        }

        public TennisEventView(TennisEventId id, String uSTAEventId, DateTime? startDate, DateTime? endDate, String name)
        {
            Id = id;
			USTAEventId = uSTAEventId;
			StartDate = startDate;
			EndDate = endDate;
			Name = name;
        }
        
    }//end TennisEventView class

}//end namespace
