#load "nuget:?package=Cake.Storm.Fluent"
#load "nuget:?package=Cake.Storm.Fluent.DotNetCore"
#load "nuget:?package=Cake.Storm.Fluent.Android"
#load "nuget:?package=Cake.Storm.Fluent.NuGet"
#load "nuget:?package=Cake.Storm.Fluent.Transformations"

const string MODULE_VERSION = "2.8.2";

MSBuild("../Storm.AndroidPdfViewer.sln", settings => settings
    .SetConfiguration("Release")
    .UseToolVersion(MSBuildToolVersion.VS2019)
    .SetPlatformTarget(PlatformTarget.MSIL));

Configure()
	.UseRootDirectory("..")
	.UseBuildDirectory("build")
	.UseArtifactsDirectory("artifacts")
	.AddConfiguration(configuration => configuration
		.WithSolution("Storm.AndroidPdfViewer.sln")
		.WithBuildParameter("Configuration", "Release")
		.WithBuildParameter("Platform", "Any CPU")
        .WithDotNetCoreOutputType(OutputType.Copy)
	)
	//platforms configuration
	.AddPlatform("dotnet", c => c
        .UseCsprojTransformation(transformations => transformations.UpdatePackageVersion(MODULE_VERSION))
        .UseNugetPack(n => n
            .WithAuthor("Julien Mialon")
            .WithNuspec("misc/Storm.AndroidPdfViewer.nuspec")
            .WithPackageId("Storm.AndroidPdfViewer")
            .WithReleaseNotesFile("misc/Storm.AndroidPdfViewer.md")
            .AddFiles("src/Storm.AndroidPdfViewer/bin/Release", "*.dll", "lib/monoandroid10.0")
            .AddFiles("src/Storm.AndroidPdfViewer/bin/Release", "*.pdb", "lib/monoandroid10.0")
        )
    )
	//targets configuration
	.AddTarget("pack")
    .AddTarget("push", configuration => configuration
        .UseNugetPush(pushConfiguration => pushConfiguration.WithApiKeyFromEnvironment())
    )
    //applications configuration
	.AddApplication("pdfviewer", configuration => configuration
        .WithProject("src/Storm.AndroidPdfium/Storm.AndroidPdfium.csproj")
        .WithVersion(MODULE_VERSION)
    )
	.Build();

RunTarget(Argument("target", "help"));