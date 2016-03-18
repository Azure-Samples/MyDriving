
using System.Collections.Generic;
using System.Threading.Tasks;

using CoreSpotlight;
using Foundation;
using MobileCoreServices;
using UIKit;

using MyDriving.DataObjects;
using MyDriving.Utils;

namespace MyDriving.iOS
{
	public class SpotlightSearch
	{
		public static Task IndexTrips(IEnumerable<Trip> trips)
		{
			return Task.Run(() =>
			{
				CSSearchableIndex.DefaultSearchableIndex.DeleteAll(deleteError =>
				{
					if (deleteError != null)
					{
						Logger.Instance.Report(new System.Exception("CoreSpotlight Index Deletion Failed"), new Dictionary<string, string>
						{
							{ "Message", deleteError.ToString () }
						});
					}
				});

				var i = 0;
				var dataItems = new List<CSSearchableItem>();
				foreach (var trip in trips)
				{
					var attributeSet = new CSSearchableItemAttributeSet(UTType.Text);
					attributeSet.Title = trip.Name;
					attributeSet.ContentDescription = $"{trip.Name} was on {trip.TimeAgo} and lasted {trip.TotalDistance}.";

					var dataItem = new CSSearchableItem(i.ToString(), "com.microsoft.MyDriving.trip", attributeSet);
					dataItems.Add(dataItem);

					i++;
				}

				CSSearchableIndex.DefaultSearchableIndex.Index (dataItems.ToArray (), insertionError =>
				{
					if (insertionError != null)
					{
						Logger.Instance.Report(new System.Exception("CoreSpotlight Indexing Failed"), new Dictionary<string, string>
						{
							{ "Message", insertionError.ToString () }
						});
					}
				});
			});
		}
	}
}