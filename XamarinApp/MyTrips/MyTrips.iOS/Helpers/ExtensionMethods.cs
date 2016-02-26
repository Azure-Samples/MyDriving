using System;
using System.Collections.Generic;
using System.Linq;

using CoreAnimation;
using CoreGraphics;
using CoreLocation;
using Foundation;
using UIKit;

using Plugin.Geolocator.Abstractions;
using MyTrips.DataObjects;

namespace MyTrips.iOS
{
	public static class ExtensionMethods
	{
		#region Animations 
		public enum UIViewAnimationFlipDirection
		{
			Top,
			Left,
			Right,
			Bottom
		}

		public enum UIViewAnimationRotationDirection
		{
			Right,
			Left
		}

		public static void ShakeHorizontally(this UIView view, float strength = 12.0f)
		{
			CAKeyFrameAnimation animation = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath ("transform.translation.x");
			animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);
			animation.Duration = 0.5;
			animation.Values = new NSObject[]
			{
				NSNumber.FromFloat(-strength),
				            NSNumber.FromFloat(strength),
				            NSNumber.FromFloat(-strength*0.66f),
				            NSNumber.FromFloat(strength*0.66f),
				            NSNumber.FromFloat(-strength*0.33f),
				            NSNumber.FromFloat(strength*0.33f),
				            NSNumber.FromFloat(0)
			};

			view.Layer.AddAnimation(animation, "shake");
		}

		public static void ShakeVertically(this UIView view, float strength = 12.0f)
		{
			CAKeyFrameAnimation animation = CAKeyFrameAnimation.FromKeyPath("transform.translation.y");
			animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);
			animation.Duration = 0.5;
			animation.Values = new NSObject[]
			{
				NSNumber.FromFloat(-strength),
				            NSNumber.FromFloat(strength),
				            NSNumber.FromFloat(-strength*0.66f),
				            NSNumber.FromFloat(strength*0.66f),
				            NSNumber.FromFloat(-strength*0.33f),
				            NSNumber.FromFloat(strength*0.33f),
				            NSNumber.FromFloat(0)
			};

			view.Layer.AddAnimation(animation, "shake");
		}

		public static void Pop(this UIView view, double duration, int repeatCount, float force)
		{
			CAKeyFrameAnimation animation = CAKeyFrameAnimation.FromKeyPath("transform.scale");
			animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
			animation.KeyTimes = new NSNumber[]
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
				            NSNumber.FromFloat(0.2f * force),
				            NSNumber.FromFloat(-0.2f * force),
				            NSNumber.FromFloat(0.2f * force),
				            NSNumber.FromFloat(0)
			};

			view.Layer.AddAnimation(animation, "pop");
		}

		public static void PulseToSize(this UIView view, float scale, double duration, bool repeat)
		{
			CABasicAnimation pulseAnimation = CABasicAnimation.FromKeyPath("transform.scale");
			pulseAnimation.Duration = duration;
			pulseAnimation.To = NSNumber.FromFloat(scale);
			pulseAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
			pulseAnimation.RepeatCount = repeat == false ? 0 : float.MaxValue;

			view.Layer.AddAnimation(pulseAnimation, "pulse");
		}

		public static void FlipWithDuration(this UIView view, double duration, UIViewAnimationFlipDirection direction, int repeatCount, bool shouldAutoReverse)
		{
			var subType = string.Empty;
			switch (direction)
			{
				case UIViewAnimationFlipDirection.Top:
					subType = "fromTop";
				break;
				case UIViewAnimationFlipDirection.Left:
					subType = "fromLeft";
				break;
				case UIViewAnimationFlipDirection.Bottom:
					subType = "fromBottom";
				break;
				case UIViewAnimationFlipDirection.Right:
					subType = "fromRight";
				break;
				default:
				break;
			}

			CATransition transition = new CATransition();
			transition.StartProgress = 0;
			transition.EndProgress = 1;
			transition.Type = "flip";
			transition.Subtype = subType;
			transition.Duration = duration;
			transition.RepeatCount = repeatCount;
			transition.AutoReverses = shouldAutoReverse;

			view.Layer.AddAnimation(transition, "spin");
		}

		public static void RotateToAngle(this UIView view, float angle, double duration, UIViewAnimationRotationDirection direction, int repeatCount, bool shouldAutoReverse)
		{
			CABasicAnimation rotateAnimation = CABasicAnimation.FromKeyPath("transform.rotation.z");

			rotateAnimation.To = direction == UIViewAnimationRotationDirection.Right ? NSNumber.FromFloat(angle) : NSNumber.FromFloat(-angle);
			rotateAnimation.Duration = duration;
			rotateAnimation.AutoReverses = shouldAutoReverse;
			rotateAnimation.RepeatCount = repeatCount;
			rotateAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);

			view.Layer.AddAnimation(rotateAnimation, "transform.rotation.z");
		}

		public static void ApplyMotionEffects(this UIView view, float horizontalRange = 10.0f, float verticalRange = 10.0f)
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0))
			{
				UIInterpolatingMotionEffect horizontalEffect = new UIInterpolatingMotionEffect("center.x", UIInterpolatingMotionEffectType.TiltAlongHorizontalAxis);
				horizontalEffect.MinimumRelativeValue = NSNumber.FromFloat(-horizontalRange);
				horizontalEffect.MaximumRelativeValue = NSNumber.FromFloat(horizontalRange);

				UIInterpolatingMotionEffect verticalEffect = new UIInterpolatingMotionEffect("center.y", UIInterpolatingMotionEffectType.TiltAlongVerticalAxis);
				verticalEffect.MinimumRelativeValue = NSNumber.FromFloat(-verticalRange);
				verticalEffect.MaximumRelativeValue = NSNumber.FromFloat(verticalRange);

				UIMotionEffectGroup motionEffectGroup = new UIMotionEffectGroup();
				motionEffectGroup.MotionEffects = new UIMotionEffect[] {horizontalEffect, verticalEffect};

				view.AddMotionEffect(motionEffectGroup);
			}
		}

		public static void FadeIn(this UIView view, double duration, float delay)
		{
			UIView.Animate(duration, delay, UIViewAnimationOptions.CurveEaseIn,
			               () =>{
				view.Alpha = 1;
			},() =>{ });
		}

		public static void StopAnimation(this UIView view)
		{
			view.Layer.RemoveAllAnimations();
		}

		public static bool IsBeingAnimated(this UIView view)
		{
			return view.Layer.AnimationKeys.Count() > 0;
		}

		#endregion

		#region Size & Positon

		public static void SetWidth(this UIView view, nfloat width)
		{
			var existingFrame = view.Frame;
			view.Frame = new CGRect(existingFrame.X, existingFrame.Y, width, existingFrame.Height);
		}

		public static void SetHeight(this UIView view, nfloat height)
		{
			var existingFrame = view.Frame;
			view.Frame = new CGRect(existingFrame.X, existingFrame.Y, existingFrame.Width, height);
		}

		public static void SetOriginX(this UIView view, nfloat x)
		{
			var existingFrame = view.Frame;
			view.Frame = new CGRect(x, existingFrame.Y, existingFrame.Width, existingFrame.Height);
		}

		public static void SetOriginY(this UIView view, nfloat y)
		{
			var existingFrame = view.Frame;
			view.Frame = new CGRect(existingFrame.X, y, existingFrame.Width, existingFrame.Height);
		}

		public static CGSize StringSize(this UILabel label)
		{
			return new NSString(label.Text).StringSize(label.Font);
		}


		#endregion

		#region UIImage

		public static void MakeRounded(this UIImageView image, int cornerRadius) {
			if (image == null)
				return;

			var layer = image.Layer;
			layer.CornerRadius = cornerRadius;
			layer.BorderWidth = 0f;
			layer.BorderColor = UIColor.Clear.CGColor;
			layer.MasksToBounds = true;
		}

		public static UIImage Tint(this UIImage img, UIColor tint, CGBlendMode blendMode) {
			UIGraphics.BeginImageContextWithOptions(img.Size, false, 0f);
			tint.SetFill();
			var bounds = new CGRect(0, 0, img.Size.Width, img.Size.Height);
			UIGraphics.RectFill(bounds);

			img.Draw(bounds, blendMode, 1f);

			if (blendMode != CGBlendMode.DestinationIn)
				img.Draw(bounds, CGBlendMode.DestinationIn, 1f);

			var tintedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return tintedImage;
		}

		#endregion

		#region Colour

		public static UIColor ToUIColor(this string hexString)
		{
			hexString = hexString.Replace("#", "");

			if (hexString.Length == 3)
				hexString = hexString + hexString;

			if (hexString.Length != 6)
				throw new Exception("Invalid hex string");

			int red = Int32.Parse(hexString.Substring(0,2), System.Globalization.NumberStyles.AllowHexSpecifier);
			int green = Int32.Parse(hexString.Substring(2,2), System.Globalization.NumberStyles.AllowHexSpecifier);
			int blue = Int32.Parse(hexString.Substring(4,2), System.Globalization.NumberStyles.AllowHexSpecifier);

			return UIColor.FromRGB(red, green, blue);
		}

		public static UIColor Lighten(this UIColor color, int steps)
		{
			int modifier = 16 * steps;

			nfloat rF, gF, bF, aF;
			color.GetRGBA(out rF, out gF, out bF, out aF);

			int r, g, b, a;
			r = (int)Math.Ceiling(rF * 255);
			g = (int)Math.Ceiling(gF * 255);
			b = (int)Math.Ceiling(bF * 255);
			a = (int)Math.Ceiling(aF * 255);

			r += modifier;
			g += modifier;
			b += modifier;
			// leave 'a' alone?

			r = r > 255 ? 255 : r;
			g = g > 255 ? 255 : g;
			b = b > 255 ? 255 : b;

			return UIColor.FromRGBA(r, g, b, a);
		}

		public static UIColor Darken(this UIColor color, int steps)
		{
			int modifier = 16 * steps;

			nfloat rF, gF, bF, aF;

			color.GetRGBA(out rF, out gF, out bF, out aF);

			int r, g, b, a;
			r = (int)Math.Ceiling(rF * 255);
			g = (int)Math.Ceiling(gF * 255);
			b = (int)Math.Ceiling(bF * 255);
			a = (int)Math.Ceiling(aF * 255);

			r -= modifier;
			g -= modifier;
			b -= modifier;
			// leave 'a' alone?

			r = r < 0 ? 0 : r;
			g = g < 0 ? 0 : g;
			b = b < 0 ? 0 : b;

			return UIColor.FromRGBA(r, g, b, a);
		}

		#endregion

		#region Coordinates
		public static CLLocationCoordinate2D ToCoordinate(this Position position)
		{
			return new CLLocationCoordinate2D(position.Latitude, position.Longitude);
		}

		public static CLLocationCoordinate2D ToCoordinate(this Trail trail)
		{
			return new CLLocationCoordinate2D(trail.Latitude, trail.Longitude);
		}

		public static CLLocationCoordinate2D[] ToCoordinateArray(this IList<Trail> trail)
		{
			var count = trail.Count;
			var coordinates = new CLLocationCoordinate2D[count];
			for (int i = 0; i < count; i++)
			{
				coordinates[i] = trail[i].ToCoordinate();
			}

			return coordinates;
		}
		#endregion
	}
}

