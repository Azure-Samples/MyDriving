using System;
using CoreLocation;
namespace MyTrips.iOS
{
	public class CarAnnotation : BaseCustomAnnotation
	{
		public CarAnnotation(CLLocationCoordinate2D annotationLocation) : base(annotationLocation)
		{ }
	}
}

