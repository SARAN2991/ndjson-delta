using System.Collections.Generic;
using System.Text.Json;

namespace NdjsonDelta
{
    /// <summary>
    /// Represents the result of a delta comparison between two NDJSON sources.
    /// </summary>
    public class NdjsonDeltaResult
    {
        /// <summary>
        /// Objects that were added in the second NDJSON source.
        /// </summary>
        public List<JsonElement> Added { get; set; } = new List<JsonElement>();

        /// <summary>
        /// Objects that were removed from the first NDJSON source.
        /// </summary>
        public List<JsonElement> Removed { get; set; } = new List<JsonElement>();

        /// <summary>
        /// Objects that were changed between the two NDJSON sources.
        /// </summary>
        public List<(JsonElement Old, JsonElement New)> Changed { get; set; } = new List<(JsonElement, JsonElement)>();
    }
}
