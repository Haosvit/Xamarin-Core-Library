namespace XamarinCore.StateMachine.Interfaces
{
	/// <summary>
	/// Factory to create vertex
	/// </summary>
	public interface IStateFactory
	{
		/// <summary>
		/// Factory name
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Create initial state.
		/// </summary>
		IVertex InitialState();

		/// <summary>
		/// Create final state
		/// </summary>
		IVertex FinalState();

		/// <summary>
		/// Creates state of type T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		IVertex Create<T>() where T : IVertex, new();
	}
}