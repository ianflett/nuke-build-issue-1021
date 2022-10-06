using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
    "test",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.Push },
    InvokedTargets = new[] { nameof(Compile) },
    FetchDepth = 1)]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter($"Configuration to build - Default is '{nameof(Configuration.Debug)}' (local) or '{nameof(Configuration.Release)}' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    [PathExecutable] readonly Tool Git;

    Target Init => _ => _
        .Executes(() => Git("config core.hooksPath build/hooks"));

    Target Compile => _ => _
        .Requires(() => Solution)
        .Executes(() =>
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()));
}
