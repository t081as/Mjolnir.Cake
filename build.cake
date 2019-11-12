#addin "nuget:?package=Cake.Git&version=0.18.0"

using System.IO;
using System.Text.RegularExpressions;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

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

Task("Build")
    .IsDependentOn("Get-Version")
    .Does(() =>
{
    var settings = new NuGetPackSettings
    {
        Version = version,
    };

    NuGetPack("Mjolnir.Cake.nuspec", settings);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

//////////////////////////////////////////////////////////////////////
// FUNCTIONS
//////////////////////////////////////////////////////////////////////

public static int GetPersistentBuildNumber(string baseDirectory)
{
    int buildNumber;
    string persistentPathName = System.IO.Path.Combine(baseDirectory, ".cache");
    string persistentFileName = System.IO.Path.Combine(persistentPathName, "build-number");

    try
    {
        if (!System.IO.Directory.Exists(persistentPathName))
        {
            System.IO.Directory.CreateDirectory(persistentPathName);
        }

        buildNumber = int.Parse(System.IO.File.ReadAllText(persistentFileName).Trim());
        buildNumber++;
    }
    catch
    {
        buildNumber = 1;
    }

    System.IO.File.WriteAllText(persistentFileName, buildNumber.ToString());

    return buildNumber;
}