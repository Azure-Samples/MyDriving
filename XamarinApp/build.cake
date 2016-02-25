#addin "Cake.Xamarin"

var username = Argument("XamarinLicenseUser", "");
var password = Argument("XamarinLicensePassword", "");

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
