using System;
using System.Diagnostics;
using System.Linq;
using XamarinCore.StateMachine.Interfaces;

namespace XamarinCore.StateMachine.Base
{
	/// <summary>
	/// Choice with only 2 transitions YES or NO
	/// </summary>
	/// <seealso cref="BaseVertex" />
	/// <seealso cref="IChoice" />
	public abstract class BaseYesNoChoice : BaseVertex, IChoice
	{
		public const string YES = "YES";
		public const string NO = "NO";

		protected abstract Func<bool> Check { get; set; }

		public override void OnActive(string trigger)
		{
			base.OnActive(trigger);

			var choice = Check() ? YES : NO;

			OnMoveRequest(choice);
		}

		/// <summary>
		/// Validates this vertex.
		/// YesNoChoice only accepts trigger YES or NO.
		/// </summary>
		public override bool Validate()
		{
			var invalid = Outgoing.Select(t => t.Trigger).Where(t => t != YES && t != NO).ToList();

			if (invalid.Count != 0)
			{
				Debug.WriteLine("Yes/no choice has invalid transition(s) with trigger: {0}", string.Join(",", invalid));
				return false;
			}

			return base.Validate();
		}
	}
}