#addin nuget:https://nuget.org/api/v2/?package=Cake.FileHelpers&version=1.0.3.2
#addin nuget:https://nuget.org/api/v2/?package=Cake.Xamarin&version=1.2.3

var username = EnvironmentVariable("XamarinLicenseUser");
var password = EnvironmentVariable("XamarinLicensePassword");

var TARGET = Argument ("target", Argument ("t", "Default"));


Task ("Default").Does (() =>
{
	RestoreComponents ("./MyTrips.sln", new XamarinComponentRestoreSettings
	{
		Email = username,
		Password = password,
		ToolPath = "./tools/xamarin-component.exe"
	});

});


RunTarget (TARGET);
