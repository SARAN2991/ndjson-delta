using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace NdjsonDelta
{
    /// <summary>
    /// Provides methods to compute the delta (added, removed, changed objects) between two NDJSON sources.
    /// </summary>
    public class NdjsonDeltaCalculator
    {
        /// <summary>
        /// Computes the delta between two NDJSON strings.
        /// </summary>
        /// <param name="ndjson1">The first NDJSON string.</param>
        /// <param name="ndjson2">The second NDJSON string.</param>
        /// <param name="keySelector">A function to select the unique key for each object (e.g., obj => obj.GetProperty("id").GetString()).</param>
        /// <returns>The delta result containing added, removed, and changed objects.</returns>
        public NdjsonDeltaResult ComputeDelta(string ndjson1, string ndjson2, Func<JsonElement, string> keySelector)
        {
            List<JsonElement> list1 = ParseNdjson(ndjson1);
            List<JsonElement> list2 = ParseNdjson(ndjson2);
            return ComputeDelta(list1, list2, keySelector);
        }

        /// <summary>
        /// Computes the delta between two NDJSON files.
        /// </summary>
        /// <param name="filePath1">Path to the first NDJSON file.</param>
        /// <param name="filePath2">Path to the second NDJSON file.</param>
        /// <param name="keySelector">A function to select the unique key for each object.</param>
        /// <returns>The delta result containing added, removed, and changed objects.</returns>
        public NdjsonDeltaResult ComputeDeltaFromFiles(string filePath1, string filePath2, Func<JsonElement, string> keySelector)
        {
            string ndjson1 = File.ReadAllText(filePath1);
            string ndjson2 = File.ReadAllText(filePath2);
            return ComputeDelta(ndjson1, ndjson2, keySelector);
        }

        private List<JsonElement> ParseNdjson(string ndjson)
        {
            List<JsonElement> result = new List<JsonElement>();
            using StringReader reader = new StringReader(ndjson);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    result.Add(JsonDocument.Parse(line).RootElement.Clone());
                }
            }
            return result;
        }

        private NdjsonDeltaResult ComputeDelta(List<JsonElement> list1, List<JsonElement> list2, Func<JsonElement, string> keySelector)
        {
            Dictionary<string, JsonElement> dict1 = list1.ToDictionary(keySelector);
            Dictionary<string, JsonElement> dict2 = list2.ToDictionary(keySelector);

            List<JsonElement> added = dict2.Where(kv => !dict1.ContainsKey(kv.Key)).Select(kv => kv.Value).ToList();
            List<JsonElement> removed = dict1.Where(kv => !dict2.ContainsKey(kv.Key)).Select(kv => kv.Value).ToList();
            List<(JsonElement, JsonElement)> changed = dict1.Where(kv => dict2.ContainsKey(kv.Key) && !JsonElementDeepEquals(kv.Value, dict2[kv.Key]))
                .Select(kv => (kv.Value, dict2[kv.Key])).ToList();

            return new NdjsonDeltaResult
            {
                Added = added,
                Removed = removed,
                Changed = changed
            };
        }

        private bool JsonElementDeepEquals(JsonElement e1, JsonElement e2)
        {
            return JsonSerializer.Serialize(e1) == JsonSerializer.Serialize(e2);
        }
    }
}
