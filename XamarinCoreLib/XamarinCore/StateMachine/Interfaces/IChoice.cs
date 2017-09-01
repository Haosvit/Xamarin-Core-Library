namespace XamarinCore.StateMachine.Interfaces
{
	/// <summary>
	/// Choice pseudostate realizes a dynamic conditional branch.
	/// It evaluates the guards of the triggers of its outgoing transitions to select only one outgoing transition
	/// </summary>
	/// <seealso cref="IVertex" />
	public interface IChoice : IVertex
	{

	}
}