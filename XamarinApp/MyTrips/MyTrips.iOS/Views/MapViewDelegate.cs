using System;

using UIKit;
using Foundation;
using MapKit;

namespace MyTrips.iOS
{
	public class MapViewDelegate : MKMapViewDelegate
	{
		static string CAR_ANNOTATION = "CAR_ANNOTATION";

		public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
		{
			MKAnnotationView annotationView = null;

			if (annotation is MKUserLocation)
				return null; 

			if (annotation is CarAnnotation)
			{
				annotationView = mapView.DequeueReusableAnnotation (CAR_ANNOTATION);

				if (annotationView == null)
					annotationView = new MKAnnotationView (annotation, CAR_ANNOTATION);

				annotationView.Image = UIImage.FromBundle(Images.CarAnnotationImage);
				annotationView.CanShowCallout = false;
			}

			return annotationView;
		}
	}
}