using System;

namespace XamarinCore.StateMachine.Interfaces
{
	public interface IStateMachine
	{
		/// <summary>
		/// Occurs when [state changed].
		/// </summary>
		event EventHandler<StateChangedEventArgs> StateChanged;

		/// <summary>
		/// Gets a value indicating whether this instance is initialized.
		/// </summary>
		bool IsInitialized { get; }

		/// <summary>
		/// Initialize the state machine.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Starts the state machine.
		/// </summary>
		void Start(string trigger = null);

		/// <summary>
		/// Stops the state machine.
		/// </summary>
		void Stop();

		/// <summary>
		/// Fires the specified trigger.
		/// </summary>
		/// <param name="trigger">The trigger.</param>
		/// <returns>Bool value indicating whether this event is handled by this state machine</returns>
		void Fire(string trigger);

		/// <summary>
		/// Gets the active vertex of a region.
		/// </summary>
		IVertex GetActiveVertex(string region);
	}

	public sealed class StateChangedEventArgs : EventArgs
	{
		public string Trigger { get; }

		public IVertex Source { get; }

		public IVertex Target { get; }

		public StateChangedEventArgs(string trigger, IVertex source, IVertex target)
		{
			Trigger = trigger;
			Source = source;
			Target = target;
		}
	}
}