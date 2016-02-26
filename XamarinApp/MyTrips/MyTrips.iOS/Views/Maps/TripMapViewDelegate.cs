using System;

using UIKit;
using MapKit;

namespace MyTrips.iOS
{
	public class TripMapViewDelegate : MKMapViewDelegate
	{
		const string CAR_ANNOTATION = "CAR_ANNOTATION";
		const string WAYPOINT_ANNOTATION = "WAYPOINT_ANNOTATION";

		public override MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
		{
			return new MKPolylineRenderer(overlay as MKPolyline)
			{
				Alpha = (nfloat) 0.4,
				LineWidth = (nfloat) 3.0,
				FillColor = UIColor.Blue,
				StrokeColor = UIColor.Red
			};
		}

		// TODO: Add animation after map finishes rendering.
		public override async void DidFinishRenderingMap(MKMapView mapView, bool fullyRendered)
		{
			
		}

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

			if (annotation is PhotoAnnotation)
			{
				annotationView = mapView.DequeueReusableAnnotation(CAR_ANNOTATION);

				if (annotationView == null)
					annotationView = new MKAnnotationView(annotation, CAR_ANNOTATION);

				// TODO: Update to actual car photo.
				annotationView.Image = UIImage.FromBundle(Images.CarAnnotationImage);
				annotationView.CanShowCallout = false;
			}

			if (annotation is WaypointAnnotation)
			{
				annotationView = mapView.DequeueReusableAnnotation(WAYPOINT_ANNOTATION);

				if (annotationView == null)
					annotationView = new MKAnnotationView (annotation, WAYPOINT_ANNOTATION);

				// TODO: Pass in A or B to show start and end points.
				// annotationView.AddSubview(new WayPointCircle());
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