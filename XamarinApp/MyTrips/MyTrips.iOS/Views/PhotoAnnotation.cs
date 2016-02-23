using System;
using CoreLocation;

namespace MyTrips.iOS
{
	public class PhotoAnnotation : BaseCustomAnnotation
	{
		// Used to identify photo to load when user taps on annotation.
		public string PhotoId { get; set; }

		public PhotoAnnotation(CLLocationCoordinate2D annotationLocation) : base(annotationLocation)
		{ 

		}
	}
}

