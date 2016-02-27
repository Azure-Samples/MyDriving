using System;
using CoreLocation;
namespace MyTrips.iOS
{
	public class CarAnnotation : BaseCustomAnnotation
	{
		public CarAnnotation(CLLocationCoordinate2D annotationLocation, string color) : base(annotationLocation)
		{
			Color = color;
		}

		public string Color { get; set; }
	}
}

