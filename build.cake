#load "scripts/GetGitTagVersion.cake"

using System.Linq;
using System.Text.RegularExpressions;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var key = Argument("key", "-TEST-");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

var version = "0.0.0";

Task("Clean")
    .Does(() =>
{
    DeleteFiles(GetFiles("*.nupkg"));
});

Task("Get-Version")
    .IsDependentOn("Clean")
    .WithCriteria(DirectoryExists(".git"))
    .Does((context) =>
{
    (string version, string shortVersion, string semanticVersion) versionInformation = GetGitTagVersion(context, 0);

    Information("Version: " + versionInformation.version);
    Information("Version (short): " + versionInformation.shortVersion);
    Information("Version (semantic): " + versionInformation.semanticVersion);

    version = versionInformation.semanticVersion;
});

Task("Pack")
    .IsDependentOn("Get-Version")
    .Does(() =>
{
    var settings = new NuGetPackSettings
    {
        Version = version,
    };

    NuGetPack("Mjolnir.Cake.nuspec", settings);
});

Task("Push")
    .IsDependentOn("Pack")
    .Does(() =>
{
    var settings = new NuGetPushSettings
    {
        Source = "https://api.nuget.org/v3/index.json",
        ApiKey = key
    };

    var packageFile = GetFiles("./Mjolnir.Cake.*.nupkg").FirstOrDefault();
    Information("Generated nuget package: " + packageFile);
    
    NuGetPush(packageFile, settings);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);