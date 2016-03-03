using System.Collections.Generic;

namespace MyTrips.Model
{
	public class Setting
	{
		public string Name { get; set; }
		public string Value { get; set; }
		public List<string> PossibleValues { get; set; }

		public bool IsButton { get; set; }
		public bool IsTextField { get; set; }
	}
}