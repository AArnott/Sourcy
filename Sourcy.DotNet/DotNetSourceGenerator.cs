using System;
using System.IO;
using Microsoft.CodeAnalysis;

namespace Sourcy.DotNet;

[Generator]
internal class DotNetSourceGenerator : BaseSourcyGenerator
{
    protected override void InitializeInternal(SourceProductionContext context, Compilation compilation)
    {
        var root = GetRootDirectory(compilation);

        foreach (var project in root.EnumerateFiles("**.*sproj", SearchOption.AllDirectories))
        {
            WriteProject(context, project);
        }
        
        foreach (var solution in root.EnumerateFiles("**.sln", SearchOption.AllDirectories))
        {
            WriteSolution(context, solution);
        }
    }

    private static void WriteProject(SourceProductionContext context, FileInfo project)
    {
        var formattedName = Path.GetFileNameWithoutExtension(project.FullName).Replace('.', '_');
        
        context.AddSource($"DotNetProjectExtensions{Guid.NewGuid():N}.g.cs", GetSourceText(
            $$"""
              namespace Sourcy.DotNet;

              internal static partial class Projects
              {
                  public static global::System.IO.FileInfo {{formattedName}} { get; } = new global::System.IO.FileInfo(@"{{project.FullName}}");
              }
              """
        ));
    }
    
    private static void WriteSolution(SourceProductionContext context, FileInfo solution)
    {
        var formattedName = Path.GetFileNameWithoutExtension(solution.FullName).Replace('.', '_');
        
        context.AddSource($"DotNetSolutionExtensions{Guid.NewGuid():N}.g.cs", GetSourceText(
            $$"""
              namespace Sourcy.DotNet;

              internal static partial class Solutions
              {
                  public static global::System.IO.FileInfo {{formattedName}} { get; } = new global::System.IO.FileInfo(@"{{solution.FullName}}");
              }
              """
        ));
    }
}