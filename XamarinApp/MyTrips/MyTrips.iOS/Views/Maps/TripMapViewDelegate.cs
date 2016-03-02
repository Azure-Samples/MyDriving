using System;

using UIKit;
using MapKit;

namespace MyTrips.iOS
{
	public class TripMapViewDelegate : MKMapViewDelegate
	{
		const string CAR_ANNOTATION = "CAR_ANNOTATION";
		const string WAYPOINT_ANNOTATION = "WAYPOINT_ANNOTATION";

		UIColor color;
		double alpha;

		public TripMapViewDelegate(UIColor color, double alpha)
		{
			this.color = color;
			this.alpha = alpha;
		}

		public override MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
		{
			// SHould be blue for past trips, red for current trips.
			// Make alpha higher for past trips - like .8
			return new MKPolylineRenderer(overlay as MKPolyline)
			{
				Alpha = (nfloat)alpha,
				LineWidth = (nfloat)4.0,
				FillColor = color,
				StrokeColor = color
			};
		}

		// TODO: Add animation after map finishes rendering.
		//public override async void DidFinishRenderingMap(MKMapView mapView, bool fullyRendered)
		//{
			
		//}

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

				if (((CarAnnotation)annotation).Color == "Blue")
				{
					annotationView.Image = UIImage.FromBundle(Images.CarAnnotationBlue);
				}
				else
				{
					annotationView.Image = UIImage.FromBundle(Images.CarAnnotationRed);
				}

				annotationView.CanShowCallout = false;
			}

			if (annotation is WaypointAnnotation)
			{
				annotationView = mapView.DequeueReusableAnnotation(WAYPOINT_ANNOTATION);

				if (annotationView == null)
					annotationView = new MKAnnotationView(annotation, WAYPOINT_ANNOTATION);

				if (((WaypointAnnotation)annotation).Waypoint == "A")
				{
					annotationView.Image = UIImage.FromBundle(Images.WaypointAnnotationA);
				}
				else
				{
					annotationView.Image = UIImage.FromBundle(Images.WaypointAnnotationB);
				}

				annotationView.CanShowCallout = false;
			}

			return annotationView;
		}

		public override void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
		{
			// TODO: Implement navigation to photo detail page
			if (view.Annotation is PhotoAnnotation)
			{

			}
		}
	}
}