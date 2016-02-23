using System;
using CoreLocation;

namespace MyTrips.iOS
{
	public class EndpointAnnotation : BaseCustomAnnotation
	{
		public EndpointAnnotation(CLLocationCoordinate2D annotationLocation) : base(annotationLocation)
		{ 

		}
	}
}

