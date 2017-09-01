using System;
using System.Diagnostics;
using XamarinCore.StateMachine.Interfaces;

namespace XamarinCore.StateMachine.Base
{
	/// <summary>
	/// An initial pseudostate represents a default vertex that is the source for a single transition to the default state.
	/// The outgoing transition from the initial vertex may have a behavior, but not a trigger or guard.
	/// </summary>
	/// <seealso cref="IVertex" />
	public class InitialState : BaseVertex
	{
		/// <summary>
		/// Adds the specified transition to this state.
		/// </summary>
		/// <param name="transition">The transition.</param>
		/// <exception cref="System.InvalidOperationException">
		/// Initial state has no trigger.
		/// or
		/// Initial state has no guard.
		/// </exception>
		public override void Add(ITransition transition)
		{
			if (!string.IsNullOrWhiteSpace(transition.Trigger))
			{
				throw new InvalidOperationException("Initial state has no trigger.");
			}

			base.Add(transition);
		}

		/// <summary>
		/// Moves to next state by the specified trigger.
		/// </summary>
		/// <param name="trigger"></param>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">
		/// Initial state has no trigger.
		/// or
		/// Initial state has no guard.
		/// </exception>
		public override IVertex MoveNext(string trigger)
		{
			if (!string.IsNullOrWhiteSpace(trigger))
			{
				return null;
			}

			foreach(var transition in Outgoing)
			{
				if (transition.Target == null)
				{
					Debug.WriteLine("Transition found for '{0}' with trigger '{1}' but no target defined. Skip moving.", GetType().Name, trigger);
					continue;
				}

				if (transition.Guard != null && !transition.Guard())
				{
					Debug.WriteLine("Transition found for '{0}' with trigger '{1}' but guard was not fullfil. Skip moving.", GetType().Name, trigger);
					continue;
				}

				transition.Source.OnExit();

				Debug.WriteLine("MOVED from '{0}' to '{1}' with trigger '{2}'.", GetType().Name, transition.Target.GetType().Name, trigger);

				transition.Target.OnEntry();
				transition.Effect?.Invoke();

				return transition.Target;
			}

			return null;
		}
	}
}