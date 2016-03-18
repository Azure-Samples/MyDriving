#addin "Cake.Xamarin"

var username = EnvironmentVariable("XamarinLicenseUser");
var password = EnvironmentVariable("GeneralPassword");

var TARGET = Argument ("target", Argument ("t", "Default"));


Task ("Default").Does (() =>
{
	RestoreComponents ("./MyDriving.sln", new XamarinComponentRestoreSettings
	{
		Email = username,
		Password = password,
		ToolPath = "./tools/xamarin-component.exe"
	});

});


RunTarget (TARGET);
