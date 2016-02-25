#addin "Cake.Xamarin"

var username = EnvironmentVariable("XamarinLicenseUser", "");
var password = EnvironmentVariable("XamarinLicensePassword", "");

var TARGET = Argument ("target", Argument ("t", "Default"));


Task ("Default").Does (() =>
{
	RestoreComponents ("./MyTrips.sln", new XamarinComponentRestoreSettings
	{
		Email = username,
		Password = password
	});

});


RunTarget (TARGET);
