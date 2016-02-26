using System;
using UIKit;
using Foundation;
using CoreGraphics;
using System.ComponentModel;

namespace MyTrips.iOS.CustomControls
{
	[Register("WayPointCircle"), DesignTimeVisible(true)]
	public class WayPointCircle : UIView
	{
		public WayPointCircle()
		{
			Initialize();
		}

		public WayPointCircle(IntPtr p): base(p)
		{
			Initialize();
		}

		void Initialize()
		{
			Text = "A";
			BackgroundColor = UIColor.FromRGBA(0.882f, 0.133f, 0.180f, 1.000f);
			BorderColor = UIColor.White;
			borderWidth = 2;
			SetNeedsDisplay();
		}

		private UIColor color;
		private UIColor borderColor;
		private string text;
		private nfloat borderWidth;

		[Export("Color"), Browsable(true)]
		public UIColor Color	 
		{
			get 
			{ 
				return color; 
			}
			set 
			{ 
				color = value;
				SetNeedsDisplay ();
			}
		}

		[Export("BorderColor"), Browsable(true)]
		public UIColor BorderColor	 
		{
			get 
			{ 
				return borderColor; 
			}
			set 
			{ 
				borderColor = value;
				SetNeedsDisplay ();
			}
		}

		[Export("Text"), Browsable(true)]
		public string Text	 
		{
			get 
			{ 
				return text; 
			}
			set 
			{ 
				text = value;
				SetNeedsDisplay ();
			}
		}

		[Export("BorderWidth"), Browsable(true)]
		public nfloat BorderWidth	 
		{
			get 
			{ 
				return borderWidth; 
			}
			set 
			{ 
				borderWidth = value;
				SetNeedsDisplay ();
			}
		}

		public override void Draw(CGRect frame)
		{
			base.Draw(frame);

			//// General Declarations
			var context = UIGraphics.GetCurrentContext();

			//// Circle Drawing
			CGRect circleRect = new CGRect(frame.GetMinX() + 2.0f, frame.GetMinY() + 2.0f, frame.Width - 5.0f, frame.Height - 5.0f);
			var circlePath = UIBezierPath.FromOval(circleRect);
			BackgroundColor.SetFill();
			circlePath.Fill();
			BorderColor.SetStroke();
			circlePath.LineWidth = BorderWidth;
			circlePath.Stroke();
			UIColor.White.SetFill();
			var circleStyle = new NSMutableParagraphStyle ();
			circleStyle.Alignment = UITextAlignment.Center;

			var circleFontAttributes = new UIStringAttributes () {Font = UIFont.FromName("HelveticaNeue", 18.0f), ForegroundColor = UIColor.White, ParagraphStyle = circleStyle};
			var circleTextHeight = new NSString(Text).GetBoundingRect(new CGSize(circleRect.Width, nfloat.MaxValue), NSStringDrawingOptions.UsesLineFragmentOrigin, circleFontAttributes, null).Height;
			context.SaveState();
			context.ClipToRect(circleRect);
			new NSString(Text).DrawString(new CGRect(circleRect.GetMinX(), circleRect.GetMinY() + (circleRect.Height - circleTextHeight) / 2.0f, circleRect.Width, circleTextHeight), UIFont.FromName("HelveticaNeue", 18.0f), UILineBreakMode.WordWrap, UITextAlignment.Center);
			context.RestoreState();
		}
	}
}

