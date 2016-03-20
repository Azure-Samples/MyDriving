// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using UIKit;
using Foundation;
using CoreGraphics;
using System.ComponentModel;

namespace MyTrip.iOS
{
    [Register("TripSlider"), DesignTimeVisible(true)]
    public class TripSlider : UIView
    {
        float percentage;

        public TripSlider()
        {
        }

        public TripSlider(IntPtr p) : base(p)
        {
        }

        [Export("Percentage"), Browsable(true)]
        public float Percentage
        {
            get { return percentage; }
            set
            {
                percentage = value;
                SetNeedsDisplay();
            }
        }

        public override void Draw(CGRect frame)
        {
            // General Declarations
            var context = UIGraphics.GetCurrentContext();

            // Color Declarations
            var minimumTrackColor = UIColor.FromRGBA(0.502f, 0.812f, 0.769f, 1.000f);
            var maximumTrackColor = UIColor.FromRGBA(0.314f, 0.314f, 0.314f, 1.000f);
            var color = UIColor.FromRGBA(0.376f, 0.490f, 0.722f, 1.000f);

            // Variable Declarations
            var expression2 = percentage*2.0f;
            var expression = new CGPoint(25.0f + expression2, 61.0f);
            var expression3 = 200.0f - percentage*2.0f;

            // Minimum Track Drawing
            var minimumTrackPath =
                UIBezierPath.FromRoundedRect(
                    new CGRect(frame.GetMinX(), frame.GetMinY() + 24.0f, frame.Width, frame.Height - 29.0f), 2.0f);
            minimumTrackColor.SetFill();
            minimumTrackPath.Fill();

            // Maximum Track Drawing
            context.SaveState();
            context.TranslateCTM(frame.GetMinX() + 1.00000f*frame.Width, frame.GetMinY() + 0.85714f*frame.Height);
            context.RotateCTM(-180.0f*NMath.PI/180.0f);

            var maximumTrackPath = UIBezierPath.FromRoundedRect(new CGRect(0.0f, 0.0f, expression3, 6.0f),
                UIRectCorner.TopLeft | UIRectCorner.BottomLeft, new CGSize(2.0f, 2.0f));
            maximumTrackPath.ClosePath();
            maximumTrackColor.SetFill();
            maximumTrackPath.Fill();

            context.RestoreState();

            // Bezier Drawing
            context.SaveState();
            context.TranslateCTM(expression.X, expression.Y);

            UIBezierPath bezierPath = new UIBezierPath();
            bezierPath.MoveTo(new CGPoint(0.0f, 2.01f));
            bezierPath.AddCurveToPoint(new CGPoint(2.6f, 0.0f), new CGPoint(0.0f, 0.9f), new CGPoint(1.17f, 0.0f));
            bezierPath.AddLineTo(new CGPoint(10.4f, 0.0f));
            bezierPath.AddCurveToPoint(new CGPoint(13.0f, 2.01f), new CGPoint(11.84f, 0.0f), new CGPoint(13.0f, 0.9f));
            bezierPath.AddLineTo(new CGPoint(13.0f, 13.04f));
            bezierPath.AddCurveToPoint(new CGPoint(11.29f, 16.56f), new CGPoint(13.0f, 14.15f),
                new CGPoint(12.23f, 15.73f));
            bezierPath.AddLineTo(new CGPoint(6.5f, 20.81f));
            bezierPath.AddLineTo(new CGPoint(6.5f, 23.65f));
            bezierPath.AddLineTo(new CGPoint(6.5f, 20.81f));
            bezierPath.AddLineTo(new CGPoint(1.71f, 16.56f));
            bezierPath.AddCurveToPoint(new CGPoint(0.0f, 13.04f), new CGPoint(0.76f, 15.73f), new CGPoint(0.0f, 14.15f));
            bezierPath.AddLineTo(new CGPoint(0.0f, 2.01f));
            bezierPath.ClosePath();
            bezierPath.UsesEvenOddFillRule = true;

            color.SetFill();
            bezierPath.Fill();
            UIColor.White.SetStroke();
            bezierPath.LineWidth = 1.5f;
            bezierPath.Stroke();

            context.RestoreState();
        }
    }
}