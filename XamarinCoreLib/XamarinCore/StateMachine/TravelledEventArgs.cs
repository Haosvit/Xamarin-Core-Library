using System;
using XamarinCore.StateMachine.Interfaces;

namespace XamarinCore.StateMachine
{
	public class TravelledEventArgs : EventArgs
	{
		public string Trigger { get; }

		public IVertex Source { get; }

		public IVertex Target { get; }

		public TravelledEventArgs(string trigger, IVertex source, IVertex target)
		{
			Trigger = trigger;
			Source = source;
			Target = target;
		}
	}
}
