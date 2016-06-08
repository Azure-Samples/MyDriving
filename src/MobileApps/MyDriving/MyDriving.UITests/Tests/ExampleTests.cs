// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Xamarin.UITest;
using NUnit.Framework;

namespace MyDriving.UITests
{
	[TestFixture(Platform.Android)]
	public class ExampleTests 
	{
		protected IApp app;
		protected Platform platform;
		public ExampleTests (Platform platform)
		{
			this.platform = platform;
			app = AppInitializer.StartApp(platform);
		}




	}
}