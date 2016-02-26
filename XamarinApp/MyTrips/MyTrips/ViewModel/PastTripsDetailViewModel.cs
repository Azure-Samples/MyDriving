using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyTrips.Helpers;
using MyTrips.Utils;
using MvvmHelpers;
using MyTrips.DataObjects;
using System.Collections.ObjectModel;

namespace MyTrips.ViewModel
{
	public class PastTripsDetailViewModel : ViewModelBase
	{
		public Trip Trip { get; set; }

		public PastTripsDetailViewModel(Trip trip)
		{
			Title = trip.TripId;
			Trip = trip;
		}
	}
}

