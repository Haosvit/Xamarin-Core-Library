using System;
using System.Collections.Generic;

namespace XamarinCore.StateMachine.Interfaces
{
	/// <summary>
	/// A region is defined as an orthogonal part of either a composite state or a state machine. Region contains states and transitions.
	/// </summary>
	public interface IRegion
	{
		/// <summary>
		/// Occurs when an event is requested to fire from this region.
		/// </summary>
		event EventHandler<string> EventRequest;

		/// <summary>
		/// Occurs when a vertex in this region has travelled.
		/// </summary>
		event EventHandler<TravelledEventArgs> Travelled;

		/// <summary>
		/// Name of this region.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// The child vertices of the region.
		/// </summary>
		IList<IVertex> Vertices { get; set; }

		/// <summary>
		/// The active vertex of the region.
		/// </summary>
		IVertex ActiveVertex { get; set; }

		/// <summary>
		/// Indicating whether this region has historycal state.
		/// </summary>
		bool HasHistory { get; set; }

		/// <summary>
		/// Adds vertex to this region.
		/// </summary>
		void Add(IVertex vertex);

		/// <summary>
		/// Validates this region.
		/// </summary>
		bool Validate();

		/// <summary>
		/// Initializes this region.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Starts running this region.
		/// </summary>
		void Start(string trigger = null);

		/// <summary>
		/// Stops this region.
		/// </summary>
		void Stop();

		/// <summary>
		/// Gets the active vertex of region.
		/// </summary>
		IVertex GetActiveVertex(string regionName);

		/// <summary>
		/// Moves the next vertex in this region by the specified trigger.
		/// </summary>
		IVertex MoveNext(string trigger);
	}
}