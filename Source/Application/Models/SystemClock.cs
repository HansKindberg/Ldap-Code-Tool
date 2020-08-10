using System;

namespace Application.Models
{
	public class SystemClock : ISystemClock
	{
		#region Properties

		public virtual DateTime UtcNow => DateTime.UtcNow;

		#endregion
	}
}