using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Windows;
using static System.Math;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyDriving.UWP.Controls
{
    public sealed partial class CirclePercentage : UserControl
    {
        public double Percentage;
        public double Radius = 54;
        public double StrokeThickness = 4;
        public Point StartPoint;
        public Point EndPoint;
        public bool IsLargeArc;
        public Size Size;
        public double TotalSize;
    
        public CirclePercentage()
        {
            this.InitializeComponent();
            StartPoint = new Point(Radius + StrokeThickness / 2, StrokeThickness / 2);
            Size = new Size(Radius, Radius);
            TotalSize = 2 * Radius + StrokeThickness;

            Percentage = 87; //todo add binding
            ComputeEndPoint();
        }

        private void ComputeEndPoint()
        {
            if (Percentage == 100)
                Percentage = 99.9;

            double angle = Percentage * 2 * Math.PI / 100;
            IsLargeArc = angle > Math.PI ? true : false;

            double endX = Radius * (1 + Sin(angle)) + StrokeThickness / 2;
            double endY = Radius * (1 - Cos(angle)) + StrokeThickness / 2;
     
            EndPoint = new Point(endX, endY);
        }
    }
}
