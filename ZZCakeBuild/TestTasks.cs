using System;
using System.IO;
using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace CakeBuild;

    
/// <summary>
/// Creates a test environment at the specified path.
/// Uses the game startup options --dataPath and --addModPath,
/// which allow specifying a custom data directory and an additional mods directory.
/// </summary>
/// <remarks>
/// </remarks>
[TaskName("SetupTestEnvironment")]
public class SetupTestEnvironment : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var log = context.Log;
        if (context.VintageStoryRequestedTestEnvironmentPath is null)
            throw new InvalidOperationException(
                "Test environment path is not specified. Use --testEnvPath=\"path\" to specify it.");

        log.Information($"Ensuring the test environment directory exists: {context.VintageStoryTestEnvironmentPath}");
        context.EnsureDirectoryExists(context.VintageStoryTestEnvironmentPath);

        if (!context.DirectoryExists(context.VintageStoryTestEnvironmentPath))
            throw new IOException($"Test environment path is unreachable: {context.VintageStoryTestEnvironmentPath}");

        string mainInstancePath = context.VintageStoryMainInstancePath;

        if (!context.UseLazyTestEnvironment)
        {
            if (mainInstancePath is null)
            {
                context.Log.Information(
                    "VintageStoryMainInstancePath is not defined, using the VINTAGE_STORY environment variable.");
                mainInstancePath = context.EnvironmentVariable("VINTAGE_STORY");

                if (mainInstancePath is null)
                {
                    context.Log.Warning(
                        "The VINTAGE_STORY environment variable is not defined, using the default path.");
                    // It's OK to fail because of a null value
                    // ReSharper disable once AssignNullToNotNullAttribute
                    mainInstancePath = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "Vintagestory");
                }
            }

            if (!Directory.Exists(mainInstancePath))
            {
                throw new IOException("Unable to locate the Vintage Story main instance path.");
            }
        }
        else
        {
            log.Information("Skipping game instance cloning.");
        }

        log.Information($"Cleaning the test environment directory: {context.VintageStoryTestEnvironmentPath}");
        context.CleanDirectory(context.VintageStoryTestEnvironmentPath);

        // Create the TestMods directory
        var testModsPath = Path.Combine(
            context.VintageStoryTestEnvironmentPath,
            BuildContext.VintageStoryTestEnvModsDirName);

        log.Information($"Creating a separate mods directory: {testModsPath}");
        context.EnsureDirectoryExists(testModsPath);

        // Copy the game instance to the test environment
        if (!context.UseLazyTestEnvironment)
        {
            var gameInstanceClonePath = Path.Combine(
                context.VintageStoryTestEnvironmentPath,
                BuildContext.VintageStoryTestEnvGameDirName);

            log.Information($"Copying the game instance from {mainInstancePath} to the test environment: {gameInstanceClonePath}");
            context.CopyDirectory(mainInstancePath, gameInstanceClonePath);
        }

        if (context.DefineEnvVar)
        {
            log.Information($"Setting the test environment variable '{BuildContext.TestEnvVariableName}' " +
                            $"to '{context.VintageStoryTestEnvironmentPath}'");

            Environment.SetEnvironmentVariable(
                BuildContext.TestEnvVariableName,
                context.VintageStoryTestEnvironmentPath,
                EnvironmentVariableTarget.User);
        }
    }
}

/// <summary>
/// Cleans the TestMods directory in the test environment and deploys the new build there.
/// </summary>
/// <remarks>
/// If you want to install mods permanently in the test environment, place them in Data/Mods.
/// </remarks>
[TaskName("Deploy")]
[IsDependentOn(typeof(PackageTask))]
public sealed class DeployTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var log = context.Log;
        string deploymentPath = null;
        var testEnvPath = context.VintageStoryTestEnvironmentPath;

        if (testEnvPath is null)
        {
            throw new InvalidOperationException(
                "Test environment path is not defined. " +
                "Use --testEnvPath=\"path\" to specify it " +
                $"or set the {BuildContext.TestEnvVariableName} environment variable.");
        }

        if (!context.DirectoryExists(testEnvPath))
        {
            throw new InvalidOperationException(
                $"Test environment path is unreachable: {testEnvPath}.\n" +
                "Run the SetupTestEnvironment task or create it manually " +
                "(see instructions in README.md).");
        }

        deploymentPath = Path.Combine(testEnvPath, BuildContext.VintageStoryTestEnvModsDirName);

        log.Information($"Cleaning the test environment mods directory: {deploymentPath}");
        context.CleanDirectory(deploymentPath);

        var zipName = $"{context.Name}_{context.Version}.zip";
        var zipPath = $"../Releases/{zipName}";

        log.Information($"Deploying {zipPath} to {deploymentPath}");
        context.CopyFile(zipPath, Path.Combine(deploymentPath, zipName));
    }
}

[TaskName("Test")]
[IsDependentOn(typeof(DeployTask))]
public sealed class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var log = context.Log;

        if (context.VintageStoryTestEnvironmentPath is null)
            throw new IOException("Test environment path is not specified.");

        var executablePath = context.VintageStoryTestInstanceExecutablePath;
        var args =
            $"--dataPath=\"{Path.Combine(context.VintageStoryTestEnvironmentPath, BuildContext.VintageStoryTestEnvDataDirName)}\" " +
            $"--addModPath=\"{Path.Combine(context.VintageStoryTestEnvironmentPath, BuildContext.VintageStoryTestEnvModsDirName)}\"";

        log.Information($"Running {executablePath} {args}");
        context.DotNetExecute(executablePath, args);
    }
}
