namespace XamarinCore.StateMachine.Base
{
	/// <summary>
	/// Base class for state.
	/// </summary>
	/// <seealso cref="BaseVertex" />
	public class BaseState : BaseVertex
	{
		public const string CmdUserOk = "CMD_USER_OK";

		public const string CmdUserBack = "CMD_USER_BACK";

		public const string CmdUserClose = "CMD_USER_CLOSE";

		public const string AppLostInternetConnection = "APP_LOST_INTERNET_CONNECTION";

		public const string AppRestoredInternetConnection = "APP_RESTORED_INTERNET_CONNECTION";
	}
}