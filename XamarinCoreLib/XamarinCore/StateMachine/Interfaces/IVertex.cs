using System;
using System.Collections.Generic;

namespace XamarinCore.StateMachine.Interfaces
{
	/// <summary>
	/// Vertex is named element which is an abstraction of a node in a state machine graph.
	/// </summary>
	public interface IVertex
	{
		/// <summary>
		/// Request to move from this vertex by specified trigger.
		/// </summary>
		event EventHandler<string> MoveRequest;

		/// <summary>
		/// The state factory to create state.
		/// </summary>
		IStateFactory StateFactory { get; set; }

		/// <summary>
		/// The set of transitions that this vertex is the source of.
		/// </summary>
		IList<ITransition> Outgoing { get; set; }

		/// <summary>
		/// Adds the specified transition.
		/// </summary>
		void Add(ITransition transition);

		/// <summary>
		/// Validates this vertex.
		/// </summary>
		bool Validate();

		/// <summary>
		/// Called when [entry].
		/// </summary>
		void OnEntry();

		/// <summary>
		/// Called when [active].
		/// </summary>
		void OnActive(string trigger);

		/// <summary>
		/// Called when [exit].
		/// </summary>
		void OnExit();

		/// <summary>
		/// Moves from this vertex by specified trigger.
		/// </summary>
		IVertex MoveNext(string trigger);
	}
}
