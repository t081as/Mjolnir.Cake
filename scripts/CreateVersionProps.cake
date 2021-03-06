//////////////////////////////////////////////////////////////////////
// FUNCTIONS
//////////////////////////////////////////////////////////////////////

/// <summary>
/// Generates a property file importable by a csproj project defining the assembly version information.
/// </summary>
/// <param name="fileName">The path and filename of the property file that shall be generated.</param>
/// <param name="assemblyVersion">The assembly version.</param>
/// <param name="fileVersion">The file version.</param>
/// <param name="version">The version.</param>
public static void CreateVersionProps(string fileName, string assemblyVersion, string fileVersion, string version)
{
    StringBuilder builder = new StringBuilder();
    builder.AppendLine("<!-->");
    builder.AppendLine(" <auto-generated>");
    builder.AppendLine("     This file was generated by Cake.");
    builder.AppendLine(" </auto-generated>");
    builder.AppendLine("-->");
    builder.AppendLine("<Project>");
    builder.AppendLine("  <PropertyGroup>");
    builder.AppendLine("    <AssemblyVersion>" + assemblyVersion + "</AssemblyVersion>");
    builder.AppendLine("    <FileVersion>" + fileVersion + "</FileVersion>");
    builder.AppendLine("    <Version>" + version + "</Version>");
    builder.AppendLine("  </PropertyGroup>");
    builder.AppendLine("</Project>");

    System.IO.File.WriteAllText(fileName, builder.ToString());
}