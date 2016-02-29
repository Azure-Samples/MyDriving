using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MyTrips.iOS.CustomControls
{
	[Register("CirclePercentage"), DesignTimeVisible(true)]
	public class CirclePercentage : UIView
	{
		public CirclePercentage(IntPtr p) : base(p)
		{
			Initialize ();
		}

		public CirclePercentage ()
		{
			Initialize ();
		}

		void Initialize ()
		{
			percentage = 50;
		}

		public override void Draw(CoreGraphics.CGRect frame)
		{
			// General Declarations
			var context = UIGraphics.GetCurrentContext();

			// Variable Declarations
			var expression = 360.0f - percentage;

			// coverView Drawing
			var coverViewPath = UIBezierPath.FromOval(new CGRect(frame.GetMinX() + 5.0f, frame.GetMinY() + 4.0f, 230.0f, 230.0f));
			UIColor.FromRGB(92,92,92).SetFill();
			coverViewPath.Fill();


			// completedView Drawing
			context.SaveState();
			context.TranslateCTM(frame.GetMinX() + 120.0f, frame.GetMinY() + 119.0f);
			context.RotateCTM(-90.0f * NMath.PI / 180.0f);

			var completedViewRect = new CGRect(-115.0f, -115.0f, 230.0f, 230.0f);
			var completedViewPath = new UIBezierPath();
			completedViewPath.AddArc(new CGPoint(completedViewRect.GetMidX(), completedViewRect.GetMidY()), completedViewRect.Width / 2.0f, (nfloat)(-360.0f * NMath.PI/180), (nfloat)(-(expression - 17.0f) * NMath.PI/180.0f), true);
			completedViewPath.AddLineTo(new CGPoint(completedViewRect.GetMidX(), completedViewRect.GetMidY()));
			completedViewPath.ClosePath();

			UIColor.FromRGB(109, 199, 184).SetFill();
			completedViewPath.Fill();

			context.RestoreState();


			// backgroundView Drawing
			var backgroundViewPath = UIBezierPath.FromOval(new CGRect(frame.GetMinX() + 10.0f, frame.GetMinY() + 9.0f, 220.0f, 220.0f));
			UIColor.FromRGB(92,92,92).SetFill();
			backgroundViewPath.Fill();
		}

		float percentage; 
		[Export("Value"), Browsable(true)]
		public float Value 
		{
			get 
			{
				return percentage;
			}
			set
			{
				percentage = value;
				SetNeedsDisplay();
			}
		}	
	}
}

