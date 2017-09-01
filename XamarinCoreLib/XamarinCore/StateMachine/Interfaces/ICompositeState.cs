using System;
using System.Collections.Generic;

namespace XamarinCore.StateMachine.Interfaces
{
	/// <summary>
	/// Composite state is defined as state that has substates (nested states)
	/// </summary>
	/// <seealso cref="IVertex" />
	public interface ICompositeState : IVertex
	{
		/// <summary>
		/// Occurs when a vertex in this region has travelled.
		/// </summary>
		event EventHandler<TravelledEventArgs> Travelled;

		/// <summary>
		/// Indicating whether this composite state is initialized before.
		/// </summary>
		bool IsInitialized { get; set; }

		/// <summary>
		/// The child regions, if any, owned by this state
		/// </summary>
		IList<IRegion> Regions { get; set; }

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Starts this instance by trigger.
		/// </summary>
		void Start(string trigger = null);

		/// <summary>
		/// Gets the active vertex of a region.
		/// </summary>
		IVertex GetActiveVertex(string region);
	}
}