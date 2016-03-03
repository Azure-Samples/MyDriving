using System.Collections.Generic;
using System.ComponentModel;

namespace MyTrips.Model
{
	public class Setting : INotifyPropertyChanged
	{
		string _value;

		public string Name { get; set; }
		public string Value 
		{ 
			get { return _value; }
			set { _value = value; NotifyPropertyChanged("Value"); }
		}
		public List<string> PossibleValues { get; set; }

		public bool IsButton { get; set; }
		public bool IsTextField { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
	}
}