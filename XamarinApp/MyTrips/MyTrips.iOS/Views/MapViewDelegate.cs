using System;

using UIKit;
using Foundation;
using MapKit;

namespace MyTrips.iOS
{
	public class MapViewDelegate : MKMapViewDelegate
	{
		static string CAR_ANNOTATION = "CAR_ANNOTATION";
		static string WAYPOINT_ANNOTATION = "WAYPOINT_ANNOTATION";

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

			if (annotation is WaypointAnnotation)
			{
				annotationView = mapView.DequeueReusableAnnotation(WAYPOINT_ANNOTATION);

				if (annotationView == null)
					annotationView = new MKAnnotationView (annotation, WAYPOINT_ANNOTATION);

				// A or B
				annotationView.AddSubview(new WayPointCircle());
				annotationView.CanShowCallout = false;
			}

			return annotationView;
		}
	}
}