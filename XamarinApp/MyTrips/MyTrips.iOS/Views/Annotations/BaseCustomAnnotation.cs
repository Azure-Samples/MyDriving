using CoreLocation;
using MapKit;

namespace MyTrips.iOS
{
	public class BaseCustomAnnotation : MKAnnotation
	{
		CLLocationCoordinate2D coordinate;

		public BaseCustomAnnotation(CLLocationCoordinate2D annotationLocation)
		{
			coordinate = annotationLocation;
		}

		public override CLLocationCoordinate2D Coordinate {
			get {
				return coordinate;
			}
		}
	}
}

