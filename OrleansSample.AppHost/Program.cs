var builder = DistributedApplication.CreateBuilder(args);

// Add the resources which you will use for Orleans clustering and
// grain state storage.
var storage = builder.AddAzureStorage("storage").RunAsEmulator();

var clusteringTable = storage.AddTables("clustering");
var reminderTable = storage.AddTables("reminders");
var grainStorage = storage.AddBlobs("grain-state");
var streams = storage.AddQueues("streaming");

// Add the Orleans resource to the Aspire DistributedApplication
// builder, then configure it with Azure Table Storage for clustering
// and Azure Blob Storage for grain storage.
var orleans = builder.AddOrleans("default")
    .WithClustering(clusteringTable)
    .WithReminders(reminderTable)
    .WithStreaming(streams)
    .WithGrainStorage("Default", grainStorage);

// Add our server project and reference your 'orleans' resource from it.
// it can join the Orleans cluster as a service.
// This implicitly add references to the required resources.
// In this case, that is the 'clusteringTable' resource declared earlier.
builder.AddProject<Projects.OrleansSample_Service>("silo")
       .WithReference(orleans)
       .WithReplicas(1);

using var app = builder.Build();

await app.RunAsync();