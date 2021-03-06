//////////////////////////////////////////////////////////////////////
// FUNCTIONS
//////////////////////////////////////////////////////////////////////

/// <summary>
/// Creates and returns an unique build number for the GitLab continuous integration system by storing the value in a cached directory.
/// </summary>
/// <param name="baseDirectory">The base path of the repository.</param>
/// <returns>An unique build number.</returns>
/// <remarks>
/// <para>
/// The build number will be stored in the file baseDirectory/.cache/build-number
/// </para>
/// <list type="bullet">
/// <item>Add the .cache directory to the .gitignore file</item>
/// <item>Add the .cache directory to the .gitlab-ci.yml file; see https://docs.gitlab.com/ee/ci/caching</item>
/// </list>
/// </remarks>
public static int GetGitLabBuildNumber(string baseDirectory)
{
    int buildNumber;

    string pathName = System.IO.Path.Combine(baseDirectory, ".cache");
    string fileName = System.IO.Path.Combine(pathName, "build-number");

    try
    {
        if (!System.IO.Directory.Exists(pathName))
        {
            System.IO.Directory.CreateDirectory(pathName);
        }

        buildNumber = int.Parse(System.IO.File.ReadAllText(fileName).Trim());
        buildNumber++;
    }
    catch
    {
        buildNumber = 1;
    }

    System.IO.File.WriteAllText(fileName, buildNumber.ToString());

    return buildNumber;
}