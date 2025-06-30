# NdjsonDelta

A .NET library to compute the delta (added, removed, changed objects) between two NDJSON (Newline Delimited JSON) files or strings. Designed for easy integration and NuGet distribution.

## Features
- Compare two NDJSON sources and get added, removed, and changed objects
- Simple API for file or string input
- Well-documented public methods

## Usage
1. Add the NuGet package to your project (instructions will be provided after publishing).
2. Use the `NdjsonDeltaCalculator` class to compute deltas between NDJSON sources.

## Sample Program

Suppose you have two NDJSON files:

**file1.ndjson**
```json
{"id": 1, "name": "A"}
{"id": 2, "name": "B"}
{"id": 3, "name": "C"}
```

**file2.ndjson**
```json
{"id": 1, "name": "A"}
{"id": 2, "name": "B2"}
{"id": 4, "name": "D"}
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
        // Use a key selector for the unique key (e.g., "id")
        NdjsonDeltaResult result = calculator.ComputeDeltaFromFiles(
            "file1.ndjson", "file2.ndjson", obj => obj.GetProperty("id").GetInt32().ToString());

        Console.WriteLine("Added:");
        foreach (var item in result.Added)
            Console.WriteLine(item);

        Console.WriteLine("Removed:");
        foreach (var item in result.Removed)
            Console.WriteLine(item);

        Console.WriteLine("Changed:");
        foreach (var (oldObj, newObj) in result.Changed)
        {
            Console.WriteLine($"Old: {oldObj}");
            Console.WriteLine($"New: {newObj}");
        }
    }
}
```

**Expected Output:**
```
Added:
{"id":4,"name":"D"}
Removed:
{"id":3,"name":"C"}
Changed:
Old: {"id":2,"name":"B"}
New: {"id":2,"name":"B2"}
```

## License
Specify your license here (e.g., MIT, Apache 2.0, etc.).

## Publishing
See instructions below for publishing to NuGet.org after implementation and packaging.
