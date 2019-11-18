#addin "nuget:?package=Cake.Git&version=0.21.0"

using System.Text.RegularExpressions;

//////////////////////////////////////////////////////////////////////
// FUNCTIONS
//////////////////////////////////////////////////////////////////////

public static (string version, string versionShort, string versionSematic) GetGitTagVersion(ICakeContext context, int buildNumber = 0)
{
    var tagQuery = new Regex(@"v(?<major>\d+).(?<minor>\d+).(?<revision>\d+)");
    var descriptionQuery = new Regex(@"(?<tag>.*)-(?<commits>\d+)-(?<shasum>.*)");

    var description = GitDescribe("./", true, GitDescribeStrategy.Default);

    var tags = Cake.Git.GitTags(context, "/");
    var latestMatchingTag = tags.Where(t => t.IsAnnotated && tagQuery.IsMatch(t.FriendlyName))?.LastOrDefault();

    if (latestMatchingTag == null)
    {
        throw new Exception("No annotated version tag detected");
    }

    var tagMatch = tagQuery.Match(latestMatchingTag);
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
        label = string.Format("dev{0}-shasum", commits)
    }

    string sematicVersion = string.Format("{0}.{1}.{2}+{3}", major, minor, revision, label);

    return (version, shortVersion, sematicVersion);
}