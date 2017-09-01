using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XamarinCore.StateMachine.Interfaces;

namespace XamarinCore.StateMachine.Base
{
	/// <summary>
	/// A region is defined as an orthogonal part of either a composite state or a state machine. Region contains states and transitions.
	/// </summary>
	/// <seealso cref="IRegion" />
	public sealed class Region : IRegion
	{
		#region Events

		public event EventHandler<string> EventRequest;
		private void OnEventRequest(string args)
		{
			var handler = EventRequest;
			handler?.Invoke(this, args);
		}

		public event EventHandler<TravelledEventArgs> Travelled;
		private void OnTravelled(TravelledEventArgs args)
		{
			var handler = Travelled;
			handler?.Invoke(this, args);
		}

		#endregion Events

		#region Properties

		/// <summary>
		/// Name of this region.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The child vertices of the region.
		/// </summary>
		public IList<IVertex> Vertices { get; set; }

		/// <summary>
		/// The active vertex of the region.
		/// </summary>
		public IVertex ActiveVertex { get; set; }

		/// <summary>
		/// Indicating whether this region has historycal state.
		/// </summary>
		public bool HasHistory { get; set; }

		#endregion Properties

		/// <summary>
		/// Initializes a new instance of the <see cref="Region"/> class.
		/// </summary>
		/// <param name="name">The name of region. This should be unique in the whole state machine.</param>
		public Region(string name)
		{
			Name = name;

			Vertices = new List<IVertex>();
			ActiveVertex = null;
		}

		/// <summary>
		/// Adds vertex to this region.
		/// </summary>
		public void Add(IVertex vertex)
		{
			vertex.MoveRequest += Vertex_OnMoveRequest;

			var composite = vertex as ICompositeState;

			if (composite != null)
			{
				composite.Travelled += Composite_OnTravelled;
			}

			Vertices.Add(vertex);
		}

		/// <summary>
		/// Validates this region.
		/// </summary>
		public bool Validate()
		{
			if (Vertices.Count == 0)
			{
				Debug.WriteLine("Region {0} has no vertex.", Name);
				return false;
			}

			var invalid = Vertices.Where(v => !v.Validate()).ToList();

			if (invalid.Count != 0)
			{
				Debug.WriteLine("Region {0} has invalid vertices: {1}.", Name, string.Join(",", invalid.Select(v => v.GetType().Name)));
				return false;
			}

			return true;
		}

		/// <summary>
		/// Initializes this region.
		/// </summary>
		public void Initialize()
		{
			Debug.WriteLine("Initialize region '{0}'. Has history: '{1}'.", Name, HasHistory);

			ActiveVertex = Vertices[0];
		}

		/// <summary>
		/// Starts running this region.
		/// </summary>
		public void Start(string trigger = null)
		{
			Debug.WriteLine("Start region '{0}' from state '{1}' by trigger '{2}'.", Name, ActiveVertex.GetType().Name, trigger);

			if (ActiveVertex is InitialState)
			{
				var next = ActiveVertex.MoveNext(null);

				if (ActiveVertex != next)
				{
					var currentVertex = ActiveVertex;
					ActiveVertex = next;
					OnTravelled(new TravelledEventArgs(null, currentVertex, next));

					ActiveVertex.OnActive(null);
				}
			}
			else
			{
				// Saved history state
				OnTravelled(new TravelledEventArgs(trigger, Vertices[0], ActiveVertex));

				ActiveVertex.OnEntry();
				ActiveVertex.OnActive(trigger);
			}
		}

		/// <summary>
		/// Stops this region.
		/// </summary>
		public void Stop()
		{
			Debug.WriteLine("Stop region '{0}'.", Name);

			ActiveVertex.OnExit();

			if (!HasHistory)
			{
				Debug.WriteLine("Region '{0}' has no history. Set active vertex to initial state again.", Name);

				ActiveVertex = Vertices[0];
			}
			else
			{
				Debug.WriteLine("Region '{0}' has history. Save vertex '{1}' as history state.", Name, ActiveVertex.GetType().Name);
			}
		}

		/// <summary>
		/// Gets the active vertex of region.
		/// </summary>
		public IVertex GetActiveVertex(string regionName)
		{
			if (string.Equals(regionName, Name))
			{
				return ActiveVertex;
			}

			// Look for children composite state
			foreach (var composite in Vertices.OfType<ICompositeState>())
			{
				var found = composite.GetActiveVertex(regionName);

				if (found != null)
				{
					return found;
				}
			}

			return null;
		}

		/// <summary>
		/// Moves the next vertex in this region by the specified trigger.
		/// </summary>
		public IVertex MoveNext(string trigger)
		{
			var next = ActiveVertex.MoveNext(trigger);

			// No child of this region can handle the event
			if (next == null)
			{
				return null;
			}

			if (ActiveVertex != next)
			{
				var currentVertex = ActiveVertex;
				ActiveVertex = next;
				OnTravelled(new TravelledEventArgs(trigger, currentVertex, next));

				ActiveVertex.OnActive(trigger);
			}

			return ActiveVertex;
		}

		/// <summary>
		/// A vertex of this region requests to move.
		/// Bubble to higher level
		/// </summary>
		private void Vertex_OnMoveRequest(object sender, string trigger)
		{
			OnEventRequest(trigger);
		}

		/// <summary>
		/// A vertex of this region has moved.
		/// Bubble travelled event to higher level.
		/// </summary>
		private void Composite_OnTravelled(object sender, TravelledEventArgs args)
		{
			OnTravelled(args);
		}
	}
}