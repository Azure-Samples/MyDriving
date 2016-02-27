# Decision Point: Xamarin.Forms vs. Native UI

Xamarin provides two ways to build great native apps: Xamarin Native and Xamarin.Forms.

With Xamarin Native you write separate UI code for each target platform: iOS, Android, and Windows. With this approach you have direct access to platform-specific APIs allowing a customized UI experience. You also have full access to the native designer and controls for each platform to help with building the respective UI.

Xamarin.Forms provides a generalized API that lets you write a single shared UI layer for all platforms using C# and XAML. At runtime, Xamarin.Forms renders native controls, resulting in a native look and feel. 

You don’t need to decide which approach to take up front; apps can be implemented using a combination of both Xamarin Native and Xamarin.Forms. Here's a typical approach:

- Use Xamarin.Forms to build general-purpose screens that provide similar UI and capabilities across platforms, such as logins, contact forms, and search results.
- Use Xamarin Native to build screens that are use unique UI features of each platform, for example, a screen that uses native camera capture and image manipulation.

For most projects, we generally recommend starting with a Xamarin.Forms solution to set up UI code sharing across platforms. Then, on an as-needed basis, you can add platform-specific screens using Xamarin Native. This is especially helpful when your development staff does not have native UI experience on the target platforms; they can learn Xamarin.Forms and apply that knowledge across platforms.   

**Our choice**

With the UX requirements in the mobile app of <TODO project name>, we could have chosen either path but we chose to use native UI from the beginning for the following reasons: 

- TODO <project name> is heavily oriented around geolocation and maps. Implementing native UI meant we could use all the geolocation features of each platform directly in the map-related UI. 
- Although Xamarin.Forms has a map control, it's better suited to simpler uses like displaying pins and points of interest. The app's UX makes extensive use of overlays which are presently somewhat easier to manage with native map controls. 
- The other screens in the app are relatively simple to implement with native UI, and our team already had the necessary expertise. The ease of working with overlays in the native map controls made up for the small extra effort required to use native UI for these screens.    
- Native UI provides more precise control over fine details like animations and transitions, allowing us to polish the UX to a greater extent than Xamarin.Forms presently allows. 
- Using native UI also allowed us, in public demonstrations, to show how Visual Studio integrate with native UI designers for Android and iOS.

As a counter-example, members of the team who also built the app for the Xamarin Evolve 2016 conference chose to use Xamarin.Forms. In this case the app is heavily oriented around presenting data from a cloud backend, and didn't have requirements that couldn't be satisfied with Xamarin.Forms. For a short-lived event app, it was also much more important to keep costs low than it was to polish the experience, so Xamarin.Forms was the best approach.     

