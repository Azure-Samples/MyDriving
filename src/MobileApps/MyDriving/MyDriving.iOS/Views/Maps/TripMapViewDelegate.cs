// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using UIKit;
using MapKit;

namespace MyDriving.iOS
{
    public class TripMapViewDelegate : MKMapViewDelegate
    {
        const string CarAnnotation = "CAR_ANNOTATION";
        const string WaypointAnnotation = "WAYPOINT_ANNOTATION";
        const string POI_ANNOTATION = "POI_ANNOTATION";
        readonly double alpha = 0.6;

        readonly UIColor color;

        public TripMapViewDelegate(bool isCurrentTripMap)
        {
            color = isCurrentTripMap ? UIColor.Red : UIColor.Blue;
        }

        public override MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
        {
            return new MKPolylineRenderer(overlay as MKPolyline)
            {
                Alpha = (nfloat) alpha,
                LineWidth = (nfloat) 4.0,
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
                annotationView = mapView.DequeueReusableAnnotation(CarAnnotation) ??
                                 new MKAnnotationView(annotation, CarAnnotation);

                if (((CarAnnotation) annotation).Color == UIColor.Blue)
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
                annotationView = mapView.DequeueReusableAnnotation(POI_ANNOTATION) ??
                                 new MKAnnotationView(annotation, POI_ANNOTATION);

                if (((PoiAnnotation) annotation).Description == "Hard Acceleration")
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
                annotationView = mapView.DequeueReusableAnnotation(WaypointAnnotation) ??
                                 new MKAnnotationView(annotation, WaypointAnnotation);

                if (((WaypointAnnotation) annotation).Waypoint == "A")
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