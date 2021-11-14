#tool nuget:?package=NUnit.ConsoleRunner&version=3.6.1
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/DataSlice/bin") + Directory(configuration);
var artifactDir = Directory("./Artifact");



//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
	CleanDirectory(artifactDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./DataSlice.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./DataSlice.sln", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild("./DataSlice.sln", settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("CopyDataModels")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
   var destModelDirectory = Directory("./src/DataSlice/bin") + Directory(configuration) + Directory("Datamodel");
	
	CreateDirectory(destModelDirectory);
	
   var sourceModelDirectory= Directory("./datamodels");
   
   CopyDirectory(sourceModelDirectory, destModelDirectory);
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit3("./src/**/bin/" + configuration + "/*.Tests.dll", new NUnit3Settings {
        NoResults = true
        });
});

Task("CreateArtifact")
.IsDependentOn("CopyDataModels")
    .Does(() =>
{
    CopyDirectory(buildDir, artifactDir);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("CreateArtifact");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
