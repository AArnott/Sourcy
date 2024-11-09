using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Enums;
using ModularPipelines.Extensions;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using File = ModularPipelines.FileSystem.File;

namespace Sourcy.Pipeline.Modules;

public class RunUnitTestsModule : Module<CommandResult>
{
    protected override async Task<CommandResult?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        return await Run(context, Sourcy.DotNet.Projects.Sourcy_Tests, cancellationToken);
    }

    private static async Task<CommandResult> Run(IPipelineContext context, File unitTestProjectFile, CancellationToken cancellationToken)
    {
        var dotNetTestOptions = new DotNetTestOptions
        {
            ProjectSolutionDirectoryDllExe = unitTestProjectFile.Path,
            CommandLogging = CommandLogging.Input | CommandLogging.Error,
        };
        
        return await context.DotNet().Test(dotNetTestOptions, cancellationToken);
    }
}
