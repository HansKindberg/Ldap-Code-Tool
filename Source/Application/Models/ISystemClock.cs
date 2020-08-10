using System;

namespace Application.Models
{
	public interface ISystemClock
	{
		#region Properties

		DateTime UtcNow { get; }

		#endregion
	}
}