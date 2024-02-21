using System;
using System.IO;

namespace Insig.Common.FileProcessing;

public class Projects
{
    public const string SolutionName = "Insig";

    public static string GetProjectName(ProjectEnum project)
    {
        return project switch
        {
            ProjectEnum.Infrastructure => "Insig.Infrastructure",
            _ => "Insig.Api",
        };
    }

    public static string GetProjectPath(ProjectEnum project)
    {
        var projectName = GetProjectName(project);

        if (Path.GetFileName(Directory.GetCurrentDirectory()).Equals(projectName, StringComparison.OrdinalIgnoreCase))
        {
            return Directory.GetCurrentDirectory();
        }
        else
        {
            var projectPath = Path.Combine(GetSolutionPath(), projectName);

            return Directory.Exists(projectPath) ?
                projectPath :
                throw new DirectoryNotFoundException($"Project directory {projectName} does not exist in current path");
        }
    }

    public static string GetSolutionPath()
    {
        var path = Directory.GetCurrentDirectory();

        while (!Path.GetFileName(path).Equals(SolutionName, StringComparison.OrdinalIgnoreCase))
        {
            var parent = Directory.GetParent(path)?.FullName;
            if (Directory.Exists(parent))
            {
                path = parent;
            }
            else
            {
                throw new DirectoryNotFoundException($"Solution directory {SolutionName} does not exist in current path");
            }
        }

        return path;
    }
}

public enum ProjectEnum
{
    Api,
    Infrastructure
}