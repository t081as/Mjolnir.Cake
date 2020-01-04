#addin "nuget:?package=Cake.Git&version=0.21.0"

using System.Text.RegularExpressions;

//////////////////////////////////////////////////////////////////////
// FUNCTIONS
//////////////////////////////////////////////////////////////////////

/// <summary>
/// Generates version information based on the latest matching git tag.
/// </summary>
/// <param name="context">The cake context.</param>
/// <param name="buildNumber">An optional build number provided by the continuous integration system.</param>
/// <remarks>
/// This method expects a git tag of the format <c>v[MAJOR].[MINOR].[REVISION]</c> (e.g. "v1.0.1", "v0.3.10", ...).
/// </remarks>
/// <returns>
/// Returns version information in the following formats:
/// <list type="bullet">
/// <item>version: [MAJOR].[MINOR].[BUILD].[REVISION]</item>
/// <item>short version: [MAJOR].[MINOR].[REVISION]</item>
/// <item>semantic version: [MAJOR].[MINOR].[REVISION]+[DESCRIPTION]</item>
/// </list>
/// </returns>
public static (string version, string versionShort, string versionSematic) GetGitTagVersion(ICakeContext context, int buildNumber = 0)
{
    var tagQuery = new Regex(@"v(?<major>\d+).(?<minor>\d+).(?<revision>\d+)");
    var descriptionQuery = new Regex(@"(?<tag>.*)-(?<commits>\d+)-(?<shasum>.*)");

    var description = GitAliases.GitDescribe(context, "./", true, GitDescribeStrategy.Default);

    var tags = GitAliases.GitTags(context, "./");
    var latestMatchingTag = tags.Where(t => tagQuery.IsMatch(t.FriendlyName))?.LastOrDefault();

    if (latestMatchingTag == null)
    {
        throw new Exception("No annotated version tag detected");
    }

    var tagMatch = tagQuery.Match(latestMatchingTag.FriendlyName);
    var descriptionMatch = descriptionQuery.Match(description);

    var major = ulong.Parse(tagMatch.Groups["major"].Value);
    var minor = ulong.Parse(tagMatch.Groups["minor"].Value);
    var revision = ulong.Parse(tagMatch.Groups["revision"].Value);

    var commits = ulong.Parse(descriptionMatch.Groups["commits"].Value);
    var shasum = descriptionMatch.Groups["shasum"].Value;

    string version = string.Format("{0}.{1}.{2}.{3}", major, minor, buildNumber, revision);
    string shortVersion = string.Format("{0}.{1}.{2}", major, minor, revision);
    
    string label;

    if (commits == 0)
    {
        label = shasum;
    }
    else
    {
        label = string.Format("dev{0}-{1}", commits, shasum);
    }

    string sematicVersion = string.Format("{0}.{1}.{2}+{3}", major, minor, revision, label);

    return (version, shortVersion, sematicVersion);
}