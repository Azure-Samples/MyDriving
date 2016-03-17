using System;
using CoreLocation;
using UIKit;
using Foundation;

using MyTrips.DataObjects;

namespace MyTrips.iOS
{
	public class PoiAnnotation : BaseCustomAnnotation
	{
		public POI PointOfInterest { get; set; }
		public string Description
		{
			get
			{
				switch (PointOfInterest.POIType)
				{
					case POIType.HardAcceleration:
						return "Hard Acceleration";
						break;
					case POIType.HardBrake:
						return "Hard Brake";
						break;
					default:
						return string.Empty;
				}
			}
		}

		public PoiAnnotation(POI pointOfInterest, CLLocationCoordinate2D coordinate) : base(coordinate)
		{
			PointOfInterest = pointOfInterest;
		}
	}
}


