using System;
using System.Diagnostics;
using XamarinCore.StateMachine.Interfaces;

namespace XamarinCore.StateMachine.Base
{
	/// <summary>
	/// Final state is a special kind of state signifying that the enclosing region is completed
	/// </summary>
	/// <seealso cref="BaseVertex" />
	public class FinalState : BaseVertex
	{
		/// <summary>
		/// Adds the specified transition.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Final state does not support to add transition.</exception>
		public override void Add(ITransition transition)
		{
			Debug.WriteLine("Adding transition to final state is not allowed.");

			throw new InvalidOperationException("Final state does not support to add transition.");
		}

		/// <summary>
		/// Moves by the specified transition.
		/// </summary>
		public override IVertex MoveNext(string trigger)
		{
			Debug.WriteLine("MOVED to final state with trigger {0}.", trigger);

			return null;
		}

		/// <summary>
		/// Validates this vertex.
		/// </summary>
		public override bool Validate()
		{
			if (Outgoing.Count == 0)
			{
				return true;
			}

			Debug.WriteLine("Final state has {0} outgoing transition(s).", Outgoing.Count);
			return false;
		}
	}
}