using CoreLocation;
using UIKit;

namespace MyTrips.iOS
{
	public class CarAnnotation : BaseCustomAnnotation
	{
		public CarAnnotation(CLLocationCoordinate2D annotationLocation, UIColor color) : base(annotationLocation)
		{
			Color = color;
		}

		public UIColor Color { get; set; }
	}
}

