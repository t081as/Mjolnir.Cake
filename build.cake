#addin "nuget:?package=Cake.Git&version=0.21.0"

using System.IO;
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
    .Does(() =>
{
    string major = "0";
    string minor = "0";
    string revision = "0";
    string shasum = "X";

    var gitDescription = GitDescribe("./", true, GitDescribeStrategy.Default);

    if (string.IsNullOrEmpty(gitDescription))
    {
        throw new Exception("Unable to read version tag");
    }

    Information("Repository description: " + gitDescription);

    Regex query = new Regex(@"v(?<major>\d+).(?<minor>\d+).(?<revision>\d+)-(?<commits>\d+)-(?<shasum>.*)");
    MatchCollection matches = query.Matches(gitDescription);

    foreach (Match match in matches)
    {
        major = match.Groups["major"].Value;
        minor = match.Groups["minor"].Value;
        revision = match.Groups["revision"].Value;
        shasum = match.Groups["shasum"].Value;
    }

    version = string.Format("{0}.{1}.{2}+{3}", major, minor, revision, shasum);
    Information("Version: " + version);
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

    NuGetPush("./Mjolnir.Cake.*.nupkg", settings);
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