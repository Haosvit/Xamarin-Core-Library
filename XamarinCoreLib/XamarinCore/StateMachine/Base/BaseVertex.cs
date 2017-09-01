using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XamarinCore.StateMachine.Interfaces;

namespace XamarinCore.StateMachine.Base
{
	/// <summary>
	/// Base class for vertex.
	/// </summary>
	/// <seealso cref="IVertex" />
	public abstract class BaseVertex : IVertex
	{
		#region Events

		public event EventHandler<string> MoveRequest;
		protected void OnMoveRequest(string trigger)
		{
			var handler = MoveRequest;
			handler?.Invoke(this, trigger);
		}

		#endregion Events

		/// <summary>
		/// The set of transitions that this vertex is the source of.
		/// </summary>
		public IList<ITransition> Outgoing { get; set; }

		/// <summary>
		/// The state factory to create state.
		/// </summary>
		public IStateFactory StateFactory { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseVertex"/> class.
		/// </summary>
		protected BaseVertex()
		{
			Outgoing = new List<ITransition>();
		}

		/// <summary>
		/// Adds the specified transition.
		/// </summary>
		public virtual void Add(ITransition transition)
		{
			Outgoing.Add(transition);
		}

		/// <summary>
		/// Validates this vertex.
		/// </summary>
		public virtual bool Validate()
		{
			foreach (var transition in Outgoing)
			{
				if (transition.Source == null)
				{
					Debug.WriteLine("Vertex '{0}', transition with trigger '{1}', source is null.", $"{StateFactory.Name}.{GetType().Name}", transition.Trigger);
					return false;
				}

				if (transition.Target == null)
				{
					Debug.WriteLine("Vertex '{0}', transition with trigger '{1}', target is null.", $"{StateFactory.Name}.{GetType().Name}", transition.Trigger);
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Called when [entry].
		/// </summary>
		public virtual void OnEntry()
		{
			Debug.WriteLine("{0} ENTER.", $"{StateFactory.Name}.{GetType().Name}");
		}

		/// <summary>
		/// Called when [active].
		/// </summary>
		public virtual void OnActive(string trigger)
		{
			Debug.WriteLine(string.IsNullOrWhiteSpace(trigger)
				? $"{StateFactory.Name}.{GetType().Name} ACTIVE from 'InitialState'."
				: $"{StateFactory.Name}.{GetType().Name} ACTIVE by '{trigger}'.");
		}

		/// <summary>
		/// Called when [exit].
		/// </summary>
		public virtual void OnExit()
		{
			Debug.WriteLine("{0} EXIT.", $"{StateFactory.Name}.{GetType().Name}");
		}

		/// <summary>
		/// Moves from this vertex by specified trigger.
		/// </summary>
		public virtual IVertex MoveNext(string trigger)
		{
			var transitions = Outgoing.Where(t => t.Trigger == trigger).ToList();

			if (transitions.Count == 0)
			{
				return null;
			}

			foreach (var transition in transitions)
			{
				if (transition.Target == null)
				{
					Debug.WriteLine("Transition found for '{0}' with trigger '{1}' but no target defined. Skip moving.", $"{StateFactory.Name}.{GetType().Name}", trigger);
					continue;
				}

				if (transition.Guard != null && !transition.Guard())
				{
					Debug.WriteLine("Transition found for '{0}' with trigger '{1}' but guard was not fullfil. Skip moving.", $"{StateFactory.Name}.{GetType().Name}", trigger);
					continue;
				}

				try
				{
					transition.Source.OnExit();

					Debug.WriteLine("MOVED from '{0}' to '{1}' with trigger '{2}'.", $"{StateFactory.Name}.{GetType().Name}", $"{StateFactory.Name}.{transition.Target.GetType().Name}", trigger);

					transition.Target.OnEntry();
					transition.Effect?.Invoke();

					return transition.Target;
				}
				catch (Exception)
				{
					Debug.WriteLine("Failed to move from {0} to {1} with {2}.", $"{StateFactory.Name}.{GetType().Name}", $"{StateFactory.Name}.{transition.Target.GetType().Name}", trigger);
				}
			}

			return null;
		}
	}
}
