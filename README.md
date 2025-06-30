# NdjsonDelta

A .NET library to compute the delta (added, removed, changed objects) between two NDJSON (Newline Delimited JSON) files or strings. Designed for easy integration and NuGet distribution.

## Features
- Compare two NDJSON sources and get added, removed, and changed objects
- Simple API for file or string input
- Support for local files and Azure Blob Storage
- Mixed source comparison (local file vs blob, blob vs blob)
- Robust JSON parsing with error handling
- Well-documented public methods

## Usage
1. Add the NuGet package to your project (instructions will be provided after publishing).
2. Use the `NdjsonDeltaCalculator` class to compute deltas between NDJSON sources.

## Sample Program

**Scenario: Compare Azure Digital Twin Topology Versions**

Suppose you have two NDJSON files containing Azure Digital Twin models and instances:

**topology_v1.ndjson** (Previous version)
```json
{"Section": "Header"}
{"fileVersion":"1.0.0","author":"TICO","organization":"TICO"}
{"Section": "Models"}
{"@context":"dtmi:dtdl:context;2","@id":"dtmi:com:toyotaindustries:gaudi:asset;1","@type":"Interface","displayName":"Asset"}
{"Section": "Twins"}
{"$dtId":"MachineA10","$metadata":{"$model":"dtmi:com:toyotaindustries:gaudi:machine;1"}}
{"$dtId":"SensorA14","$metadata":{"$model":"dtmi:com:toyotaindustries:gaudi:sensor;1"}}
{"$dtId":"Plant301","$metadata":{"$model":"dtmi:com:toyotaindustries:gaudi:plant;1"}}
```

**topology_v2.ndjson** (Current version)
```json
{"Section": "Header"}
{"fileVersion":"1.0.1","author":"TICO","organization":"TICO"}
{"Section": "Models"}
{"@context":"dtmi:dtdl:context;2","@id":"dtmi:com:toyotaindustries:gaudi:asset;1","@type":"Interface","displayName":"Asset"}
{"Section": "Twins"}
{"$dtId":"MachineA10","$metadata":{"$model":"dtmi:com:toyotaindustries:gaudi:machine;1"}}
{"$dtId":"SensorA15","$metadata":{"$model":"dtmi:com:toyotaindustries:gaudi:sensor;1"}}
{"$dtId":"Plant301","$metadata":{"$model":"dtmi:com:toyotaindustries:gaudi:plant;1"}}
{"$dtId":"MachineA11","$metadata":{"$model":"dtmi:com:toyotaindustries:gaudi:machine;1"}}
```

**Sample C# code:**
```csharp
using System;
using System.Text.Json;
using NdjsonDelta;

class Program
{
    static void Main()
    {
        var calculator = new NdjsonDeltaCalculator();
        
        // Compare topology versions using $dtId as the unique key
        NdjsonDeltaResult result = calculator.ComputeDeltaFromFiles(
            "topology_v1.ndjson", "topology_v2.ndjson", 
            obj => {
                // Use $dtId for twins, or generate a key for other objects
                if (obj.TryGetProperty("$dtId", out var dtId))
                    return dtId.GetString();
                if (obj.TryGetProperty("@id", out var id))
                    return id.GetString();
                if (obj.TryGetProperty("fileVersion", out var version))
                    return "fileVersion";
                return obj.GetRawText().GetHashCode().ToString();
            });

        Console.WriteLine("=== Digital Twin Topology Changes ===");
        Console.WriteLine($"Added Twins/Models: {result.Added.Count}");
        Console.WriteLine($"Removed Twins/Models: {result.Removed.Count}");
        Console.WriteLine($"Changed Items: {result.Changed.Count}");
        
        foreach (var item in result.Added)
        {
            if (item.TryGetProperty("$dtId", out var dtId))
                Console.WriteLine($"Added Twin: {dtId.GetString()}");
        }
    }
}
```

**Expected Output:**
```
=== Digital Twin Topology Changes ===
Added Twins/Models: 2
Removed Twins/Models: 1
Changed Items: 1
Added Twin: MachineA11
```

## Azure Blob Storage Support

**Compare two files from Azure Blob Storage:**
```csharp
using System.Threading.Tasks;
using NdjsonDelta;

class Program
{
    static async Task Main()
    {
        var calculator = new NdjsonDeltaCalculator();
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=yourstorageaccount;AccountKey=yourkey;EndpointSuffix=core.windows.net";
        
        // Compare two topology files stored in different blob containers
        NdjsonDeltaResult result = await calculator.ComputeDeltaFromBlobsAsync(
            connectionString, 
            "topology-archive", "topology_20240301.ndjson",  // Previous version
            "topology-current", "topology_20240401.ndjson",  // Current version
            obj => {
                if (obj.TryGetProperty("$dtId", out var dtId))
                    return dtId.GetString();
                if (obj.TryGetProperty("@id", out var id))
                    return id.GetString();
                return obj.GetRawText().GetHashCode().ToString();
            });
            
        Console.WriteLine($"Topology changes detected: {result.Added.Count + result.Removed.Count + result.Changed.Count} differences");
    }
}
```

**Compare local file with blob:**
```csharp
// Compare local development topology with production blob
NdjsonDeltaResult result = await calculator.ComputeDeltaFromLocalAndBlobAsync(
    "local-topology.ndjson", 
    connectionString, "production-topology", "current.ndjson",
    obj => obj.TryGetProperty("$dtId", out var dtId) ? dtId.GetString() : obj.GetRawText().GetHashCode().ToString(),
    localFirst: true);
```

## Installation
```bash
dotnet add package NdjsonDelta
```

## License
MIT License

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.
