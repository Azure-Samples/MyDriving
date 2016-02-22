using System;
using UIKit;

namespace MyTrips.iOS
{
	public static class ExtensionMethods
	{
		public static UIColor ToColor (this string hexValue)
		{
			if (hexValue.Contains ("#")) {
				hexValue = hexValue.Replace("#", "");
			}

			if (hexValue.Length != 6) {
				throw new ArgumentOutOfRangeException ("Hexadecimal values must be six characters long.");
			}

			int red = Int32.Parse (hexValue.Substring (0,2), System.Globalization.NumberStyles.AllowHexSpecifier);
			int green = Int32.Parse (hexValue.Substring (2,2), System.Globalization.NumberStyles.AllowHexSpecifier);
			int blue = Int32.Parse (hexValue.Substring (4,2), System.Globalization.NumberStyles.AllowHexSpecifier);

			return UIColor.FromRGB (red, green, blue);
		}
	}
}

