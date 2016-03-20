// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using CoreAnimation;
using CoreLocation;
using Foundation;
using UIKit;
using MyDriving.DataObjects;
using Plugin.Geolocator.Abstractions;

namespace MyDriving.iOS
{
    public static class ExtensionMethods
    {
        #region Colour

        public static UIColor ToUIColor(this string hexString)
        {
            hexString = hexString.Replace("#", "");

            if (hexString.Length == 3)
                hexString = hexString + hexString;

            if (hexString.Length != 6)
                throw new Exception("Invalid hex string");

            int red = Int32.Parse(hexString.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            int green = Int32.Parse(hexString.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            int blue = Int32.Parse(hexString.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

            return UIColor.FromRGB(red, green, blue);
        }

        #endregion

        #region Animations 

        public static void Pop(this UIView view, double duration, int repeatCount, float force, double delay = 0)
        {
            CAKeyFrameAnimation animation = CAKeyFrameAnimation.FromKeyPath("transform.scale");
            animation.BeginTime = CAAnimation.CurrentMediaTime() + delay;
            animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
            animation.KeyTimes = new[]
            {
                NSNumber.FromFloat(0),
                NSNumber.FromFloat(0.2f),
                NSNumber.FromFloat(0.4f),
                NSNumber.FromFloat(0.6f),
                NSNumber.FromFloat(0.8f),
                NSNumber.FromFloat(1)
            };
            animation.Duration = duration;
            animation.Additive = true;
            animation.RepeatCount = repeatCount;
            animation.Values = new NSObject[]
            {
                NSNumber.FromFloat(0),
                NSNumber.FromFloat(0.2f*force),
                NSNumber.FromFloat(-0.2f*force),
                NSNumber.FromFloat(0.2f*force),
                NSNumber.FromFloat(0)
            };
            if (view.Hidden)
                view.Hidden = false;
            view.Layer.AddAnimation(animation, "pop");
        }

        public static void FadeIn(this UIView view, double duration, float delay)
        {
            UIView.Animate(duration, delay, UIViewAnimationOptions.CurveEaseIn,
                () => { view.Alpha = 1; }, () => { });
        }

        #endregion

        #region Coordinates

        public static CLLocationCoordinate2D ToCoordinate(this Position position)
        {
            return new CLLocationCoordinate2D(position.Latitude, position.Longitude);
        }

        public static CLLocationCoordinate2D ToCoordinate(this TripPoint point)
        {
            return new CLLocationCoordinate2D(point.Latitude, point.Longitude);
        }

        public static CLLocationCoordinate2D ToCoordinate(this POI point)
        {
            return new CLLocationCoordinate2D(point.Latitude, point.Longitude);
        }

        public static CLLocationCoordinate2D[] ToCoordinateArray(this IList<TripPoint> points)
        {
            var count = points.Count;
            var coordinates = new CLLocationCoordinate2D[count];
            for (int i = 0; i < count; i++)
            {
                coordinates[i] = points[i].ToCoordinate();
            }

            return coordinates;
        }

        #endregion
    }
}