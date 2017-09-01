using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using XamarinCore.StateMachine.Interfaces;

namespace XamarinCore.StateMachine.Base
{
	public abstract class BaseStateMachine : BaseCompositeState, IStateMachine
	{
		#region Events

		public event EventHandler<StateChangedEventArgs> StateChanged;
		private void OnStateChanged(StateChangedEventArgs args)
		{
			var handler = StateChanged;

			if (handler == null)
			{
				return;
			}

			foreach (var receiver in handler.GetInvocationList().Cast<EventHandler<StateChangedEventArgs>>())
			{
				receiver.BeginInvoke(this, args, StateChangedEventInvoke, null);
			}
		}

		private void StateChangedEventInvoke(IAsyncResult iar)
		{
			var asyncDelegate = iar.GetType().GetRuntimeProperty("AsyncDelegate");

			var invoke = asyncDelegate?.GetValue(iar) as EventHandler<StateChangedEventArgs>;

			try
			{
				invoke?.EndInvoke(iar);
			}
			catch (Exception exception)
			{
				Debug.WriteLine(exception);
			}
		}

		#endregion Events

		/// <summary>
		/// The queue of events.
		/// </summary>
		private readonly ConcurrentQueue<string> _triggers = new ConcurrentQueue<string>();

		/// <summary>
		/// Manual event which is set on trigger.
		/// </summary>
		private readonly AutoResetEvent _waitForTrigger = new AutoResetEvent(false);

		/// <summary>
		/// Indicating whether this state machine is running.
		/// </summary>
		private bool _running;

		/// <summary>
		/// Starts this instance.
		/// </summary>
		public override void Start(string trigger = null)
		{
			_running = true;

			Task.Factory.StartNew(WaitForTrigger, TaskCreationOptions.LongRunning);

			base.Start(trigger);
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public override void Stop()
		{
			_running = false;

			_waitForTrigger.Set();

			base.Stop();
		}

		/// <summary>
		/// Fires the specified trigger.
		/// </summary>
		/// <param name="trigger">The trigger.</param>
		public void Fire(string trigger)
		{
			_triggers.Enqueue(trigger);

			_waitForTrigger.Set();
		}

		/// <summary>
		/// Polls for events.
		/// </summary>
		private void WaitForTrigger()
		{
			while (_running)
			{
				if (_triggers.IsEmpty)
				{
					_waitForTrigger.WaitOne();
				}

				while (!_triggers.IsEmpty)
				{
					string trigger;

					if (_triggers.TryDequeue(out trigger))
					{
						foreach (var region in Regions)
						{
							region.MoveNext(trigger);
						}
					}
					else
					{
						Debug.WriteLine("Cannot dequeue a trigger from queue of {0}.", GetType().Name);
					}
				}
			}
		}

		protected override void Region_EventRequest(object sender, string args)
		{
			Fire(args);
		}

		protected override void Region_Travelled(object sender, TravelledEventArgs args)
		{
			OnStateChanged(new StateChangedEventArgs(args.Trigger, args.Source, args.Target));
		}
	}
}
