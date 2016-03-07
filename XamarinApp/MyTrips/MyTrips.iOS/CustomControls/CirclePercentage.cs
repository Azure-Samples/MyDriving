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
			var context = UIGraphics.GetCurrentContext();
			var expression = 377.0f - percentage;

			// coverView Drawing
			var coverViewPath = UIBezierPath.FromOval(new CGRect(frame.GetMinX() + 5.0f, frame.GetMinY() + 4.0f, frame.Width - 10.0f, frame.Height - 10.0f));
			UIColor.FromRGB(21, 169, 254).SetFill();
			coverViewPath.Fill();

			// completedView Drawing
			context.SaveState();
			context.SaveState();
			context.TranslateCTM(frame.GetMaxX() - 65.0f, frame.GetMinY() + 64.0f);
			context.RotateCTM(-90.0f * NMath.PI / 180.0f);

			var completedViewRect = new CGRect(-60.0f, -60.0f, 120.0f, 120.0f);
			var completedViewPath = new UIBezierPath();
			completedViewPath.AddArc(new CGPoint(completedViewRect.GetMidX(), completedViewRect.GetMidY()), completedViewRect.Width / 2.0f, (nfloat)(-360.0f * NMath.PI/180), (nfloat)(-(expression - 17.0f) * NMath.PI/180.0f), true);
			completedViewPath.AddLineTo(new CGPoint(completedViewRect.GetMidX(), completedViewRect.GetMidY()));
			completedViewPath.ClosePath();

			UIColor.FromRGB(247, 247, 247).SetFill();
			completedViewPath.Fill();
			context.RestoreState();

			// backgroundView Drawing
			var backgroundViewPath = UIBezierPath.FromOval(new CGRect(frame.GetMinX() + 12.0f, frame.GetMinY() + 11.0f, frame.Width - 24.0f, frame.Height - 24.0f));
			UIColor.FromRGB(21, 169, 254).SetFill();
			backgroundViewPath.Fill();
		}

		float percentage; 
		[Export("Value"), Browsable(true)]
		public float Value 
		{
			get { return percentage; }
			set { percentage = value; SetNeedsDisplay(); }
		}	
	}
}