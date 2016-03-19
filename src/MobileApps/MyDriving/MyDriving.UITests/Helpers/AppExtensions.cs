// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace MyDriving.UITests
{
    public static class AppExtensions
    {
        // Place IApp extension methods here for custom functionality
        // eg. a cutom swipe or scroll gesture that could be used on any page, element, etc.

        public static void SwipeLeftOnElement(this IApp app, AppResult element)
        {
            var r = element.Rect;
            app.DragCoordinates(r.CenterX, r.CenterY, r.X, r.CenterY);
        }
    }
}