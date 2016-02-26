using System;

using UIKit;
using Foundation;
using MapKit;

namespace MyTrips.iOS
{
	public class TripMapViewDelegate : MKMapViewDelegate
	{
		static string CAR_ANNOTATION = "CAR_ANNOTATION";
		static string WAYPOINT_ANNOTATION = "WAYPOINT_ANNOTATION";

		public override MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
		{
			var polylineRenderer = new MKPolylineRenderer (overlay as MKPolyline);
			polylineRenderer.FillColor = UIColor.Blue;
			polylineRenderer.StrokeColor = UIColor.Red;
			polylineRenderer.LineWidth = 3;
			polylineRenderer.Alpha = 0.4f;

			return polylineRenderer;
		}

		public override async void DidFinishRenderingMap(MKMapView mapView, bool fullyRendered)
		{
			// TODO: DO your thing mike!
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

				// TODO: Make this the car photo
				annotationView.Image = UIImage.FromBundle(Images.CarAnnotationImage);
				annotationView.CanShowCallout = false;
			}

			if (annotation is WaypointAnnotation)
			{
				annotationView = mapView.DequeueReusableAnnotation(WAYPOINT_ANNOTATION);

				if (annotationView == null)
					annotationView = new MKAnnotationView (annotation, WAYPOINT_ANNOTATION);

				// A or B
				// annotationView.AddSubview(new WayPointCircle());
				annotationView.CanShowCallout = false;
			}

			return annotationView;
		}

		public override void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
		{
			base.DidSelectAnnotationView(mapView, view);

			if (view.Annotation is PhotoAnnotation)
			{
				// TODO: Implement navigation to photo detail page.
			}
		}
	}
}