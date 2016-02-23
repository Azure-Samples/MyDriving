using System;

using Foundation;
using UIKit;
using MapKit;
using CoreLocation;

namespace MyTrips.iOS
{
	public class CarAnnotation : MKAnnotation
	{
		CLLocationCoordinate2D coordinate;

		public CarAnnotation(CLLocationCoordinate2D annotationLocation)
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

