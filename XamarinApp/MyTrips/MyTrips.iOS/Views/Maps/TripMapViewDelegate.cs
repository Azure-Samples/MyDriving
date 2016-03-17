using System;
using UIKit;
using MapKit;

namespace MyTrips.iOS
{
	public class TripMapViewDelegate : MKMapViewDelegate
	{
		const string CAR_ANNOTATION = "CAR_ANNOTATION";
		const string WAYPOINT_ANNOTATION = "WAYPOINT_ANNOTATION";
		const string POI_ANNOTATION = "POI_ANNOTATION";

		UIColor color;
		double alpha = 0.6;

		public TripMapViewDelegate(bool isCurrentTripMap)
		{
			if (isCurrentTripMap)
				color = UIColor.Red;
			else
				color = UIColor.Blue;
		}

		public override MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
		{
			return new MKPolylineRenderer(overlay as MKPolyline)
			{
				Alpha = (nfloat)alpha,
				LineWidth = (nfloat)4.0,
				FillColor = color,
				StrokeColor = color
			};
		}

		public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
		{
			MKAnnotationView annotationView = null;

			if (annotation is MKUserLocation)
				return null;

			if (annotation is CarAnnotation)
			{
				annotationView = mapView.DequeueReusableAnnotation(CAR_ANNOTATION);

				if (annotationView == null)
					annotationView = new MKAnnotationView(annotation, CAR_ANNOTATION);

				if (((CarAnnotation)annotation).Color == UIColor.Blue)
				{
					annotationView.Image = UIImage.FromBundle(Images.CarAnnotationBlue);
				}
				else
				{
					annotationView.Image = UIImage.FromBundle(Images.CarAnnotationRed);
				}

				annotationView.CanShowCallout = false;
			}

			if (annotation is PoiAnnotation)
			{
				annotationView = mapView.DequeueReusableAnnotation(POI_ANNOTATION);

				if (annotationView == null)
					annotationView = new MKAnnotationView(annotation, POI_ANNOTATION);

				if (((PoiAnnotation)annotation).Description == "Hard Acceleration")
				{
					annotationView.Image = UIImage.FromBundle(Images.TipAnnotation);
				}
				else
				{
					annotationView.Image = UIImage.FromBundle(Images.TipAnnotation);
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
	}
}