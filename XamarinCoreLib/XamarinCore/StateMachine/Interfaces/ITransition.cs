using System;

namespace XamarinCore.StateMachine.Interfaces
{
	/// <summary>
	/// A transition is a directed relationship between a source vertex and a target vertex.
	/// </summary>
	public interface ITransition
	{
		/// <summary>
		/// The cause of the transition, which could be a signal, an event, a change in some condition, or the passage of time
		/// </summary>
		string Trigger { get; set; }

		/// <summary>
		/// Condition which must be true in order for the trigger to cause the transition.
		/// </summary>
		Func<bool> Guard { get; set; }

		/// <summary>
		/// Action which will be invoked directly on the object that owns the state machine as a result of the transition.
		/// </summary>
		Action Effect { get; set; }

		/// <summary>
		/// Source state of this transition.
		/// </summary>
		IVertex Source { get; set; }

		/// <summary>
		/// Target state of this transition.
		/// </summary>
		IVertex Target { get; set; }
	}
}