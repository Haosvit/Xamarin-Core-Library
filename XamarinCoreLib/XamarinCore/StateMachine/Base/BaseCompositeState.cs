using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using XamarinCore.StateMachine.Interfaces;

namespace XamarinCore.StateMachine.Base
{
	/// <summary>
	/// Base class for composite state
	/// </summary>
	/// <seealso cref="BaseState" />
	/// <seealso cref="ICompositeState" />
	public abstract class BaseCompositeState : BaseState, ICompositeState
	{
		#region Events

		public event EventHandler<TravelledEventArgs> Travelled;
		private void OnTravelled(TravelledEventArgs args)
		{
			var handler = Travelled;
			handler?.Invoke(this, args);
		}

		#endregion Events

		#region Properties

		/// <summary>
		/// Indicating whether this composite state is initialized before.
		/// </summary>
		public bool IsInitialized { get; set; }

		/// <summary>
		/// The child regions, if any, owned by this state
		/// </summary>
		public IList<IRegion> Regions { get; set; }

		#endregion Properties

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseCompositeState"/> class.
		/// </summary>
		protected BaseCompositeState()
		{
			Regions = new List<IRegion>();
		}

		#region IVertex

		/// <summary>
		/// Called when [active].
		/// Initialize this composite state and start internal state machine
		/// </summary>
		public override void OnActive(string trigger)
		{
			base.OnActive(trigger);

			Initialize();
			Start(trigger);
		}

		/// <summary>
		/// Called when [exit].
		/// Stop the internal state machine.
		/// </summary>
		public override void OnExit()
		{
			Stop();

			base.OnExit();
		}

		/// <summary>
		/// Moves from this vertex by specified trigger.
		/// Check if any child can handle this trigger.
		/// Otherwise, try to handle by this composite state itself.
		/// </summary>
		public override IVertex MoveNext(string trigger)
		{
			var regions = Regions.Select(r => r.MoveNext(trigger)).Where(r => r != null).ToList();

			// no child can handle
			if (regions.Count == 0)
			{
				return base.MoveNext(trigger);
			}
			else
			{
				return this;
			}
		}

		#endregion IVertex

		#region ICompositeState

		/// <summary>
		/// Initializes this composite state by initialize all regions.
		/// </summary>
		/// <exception cref="InvalidOperationException"/>
		public void Initialize()
		{
			Debug.WriteLine("Initialize composite state '{0}.'", $"{StateFactory.Name}.{GetType().Name}");

			if (IsInitialized)
			{
				Debug.WriteLine("Composite state '{0}' is initialized. Skipped.", $"{StateFactory.Name}.{GetType().Name}");
				return;
			}

			if (!Validate())
			{
				Debug.WriteLine("Composite state {0} is invalid.", $"{StateFactory.Name}.{GetType().Name}");

				throw new InvalidOperationException($"Composite state {StateFactory.Name}.{GetType().Name} is invalid.");
			}

			var regions = CreateRegions();

			foreach (var region in regions)
			{
				if (!region.Validate())
				{
					Debug.WriteLine("Region {0} is invalid.", region.Name);

					throw new InvalidOperationException($"Region {region.Name} is invalid.");
				}

				region.EventRequest += Region_EventRequest;
				region.Travelled += Region_Travelled;

				region.Initialize();
			}

			Regions = regions;

			IsInitialized = true;
		}

		/// <summary>
		/// Starts this instance.
		/// </summary>
		public virtual void Start(string trigger = null)
		{
			Debug.WriteLine("Start composite state '{0}' with trigger '{1}'. Regions: '{2}'", $"{StateFactory.Name}.{GetType().Name}", trigger, string.Join(", ", Regions.Select(r => r.Name)));

			var tasks = new List<Task>();

			foreach (var region in Regions)
			{
				var t = Task.Run(() => region.Start(trigger));
				tasks.Add(t);
			}

			Task.WaitAll(tasks.ToArray());
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public virtual void Stop()
		{
			Debug.WriteLine("Stop composite state '{0}'. Region count '{1}': '{2}'", $"{StateFactory.Name}.{GetType().Name}", Regions.Count, string.Join(", ", Regions.Select(r => r.Name)));

			var tasks = new List<Task>();

			foreach (var region in Regions)
			{
				var t = Task.Run(() => region.Stop());
				tasks.Add(t);
			}

			Task.WaitAll(tasks.ToArray());
		}

		/// <summary>
		/// Gets the active vertex of a region.
		/// </summary>
		public IVertex GetActiveVertex(string regionName)
		{
			foreach (var region in Regions)
			{
				var found = region.GetActiveVertex(regionName);

				if (found != null)
				{
					return found;
				}
			}

			return null;
		}

		/// <summary>
		/// Setups this composite state with regions.
		/// </summary>
		protected abstract IList<IRegion> CreateRegions();

		#endregion ICompositeState

		/// <summary>
		/// A vertex of region requests to move.
		/// Bubble to higher level
		/// </summary>
		protected virtual void Region_EventRequest(object sender, string args)
		{
			OnMoveRequest(args);
		}

		/// <summary>
		/// A vertex of region has moved.
		/// Bubble travelled event to higher level.
		/// </summary>
		protected virtual void Region_Travelled(object sender, TravelledEventArgs args)
		{
			OnTravelled(args);
		}
	}
}