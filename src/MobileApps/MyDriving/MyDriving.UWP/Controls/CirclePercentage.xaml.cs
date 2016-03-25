// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Windows.Foundation;
using static System.Math;

namespace MyDriving.UWP.Controls
{
    public sealed partial class CirclePercentage
    {
        public Point EndPoint;
        public bool IsLargeArc;
        public double Percentage;
        public double Radius = 54;
        public Size Size;
        public Point StartPoint;
        public double StrokeThickness = 4;
        public double TotalSize;

        public CirclePercentage()
        {
            InitializeComponent();
            StartPoint = new Point(Radius + StrokeThickness/2, StrokeThickness/2);
            EndPoint = StartPoint;
            Size = new Size(Radius, Radius);
            TotalSize = 2*Radius + StrokeThickness;
        }

        private void ComputeEndPoint()
        {
            if (Percentage == 100)
                Percentage = 99.9;

            double angle = Percentage*2*PI/100;
            IsLargeArc = angle > PI;

            double endX = Radius*(1 + Sin(angle)) + StrokeThickness/2;
            double endY = Radius*(1 - Cos(angle)) + StrokeThickness/2;

            EndPoint = new Point(endX, endY);
        }

        public void Update()
        {
            ComputeEndPoint();
            Arc.IsLargeArc = IsLargeArc;
            Arc.Point = EndPoint;
        }
    }
}