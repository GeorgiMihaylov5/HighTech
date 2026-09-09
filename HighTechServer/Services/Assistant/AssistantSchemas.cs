namespace HighTech.Services.Assistant
{
    public static class AssistantSchemas
    {
        // Drives Request 1's response. The LLM classifies the user's turn and emits an
        // optional data plan describing what categories of products the backend should
        // fetch before composing the final answer in Request 2.
        public const string DataPlanSchema = @"{
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""required"": [""intent"", ""reasoning"", ""dataPlan""],
  ""properties"": {
    ""intent"": {
      ""type"": ""string"",
      ""enum"": [""tech_question"", ""pc_creation"", ""pc_modification"", ""other""]
    },
    ""reasoning"": { ""type"": ""string"" },
    ""dataPlan"": {
      ""type"": ""object"",
      ""additionalProperties"": false,
      ""required"": [""fetches"", ""needsCurrentSelection"", ""needsCompatibleParts""],
      ""properties"": {
        ""fetches"": {
          ""type"": ""array"",
          ""items"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""required"": [""categoryName"", ""priceMin"", ""priceMax"", ""manufacturer""],
            ""properties"": {
              ""categoryName"": { ""type"": ""string"" },
              ""priceMin"": { ""type"": [""number"", ""null""] },
              ""priceMax"": { ""type"": [""number"", ""null""] },
              ""manufacturer"": { ""type"": [""string"", ""null""] }
            }
          }
        },
        ""needsCurrentSelection"": { ""type"": ""boolean"" },
        ""needsCompatibleParts"": { ""type"": ""boolean"" }
      }
    }
  }
}";

        // Drives Request 2's response. v1 schema permits chat | pc-creation | pc-modification only.
        // The recommendation variant exists in the polymorphic DTO but is unreachable here on purpose.
        // Note: OpenAI strict structured outputs do not support free-form key/value maps,
        // so `selection` is modelled as an array of {categoryName, productId} pairs (same item
        // shape as `changes`, but with productId required non-null).
        public const string AssistantMessageSchema = @"{
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""required"": [""type"", ""rationale"", ""text"", ""selection"", ""changes""],
  ""properties"": {
    ""type"": {
      ""type"": ""string"",
      ""enum"": [""chat"", ""pc-creation"", ""pc-modification""]
    },
    ""rationale"": { ""type"": [""string"", ""null""] },
    ""text"": {
      ""type"": [""string"", ""null""],
      ""description"": ""For type=chat. The plain-text reply shown to the user.""
    },
    ""selection"": {
      ""type"": [""array"", ""null""],
      ""description"": ""For type=pc-creation. Array of {categoryName, productId} entries that together replace the user's current build."",
      ""items"": {
        ""type"": ""object"",
        ""additionalProperties"": false,
        ""required"": [""categoryName"", ""productId""],
        ""properties"": {
          ""categoryName"": { ""type"": ""string"" },
          ""productId"": { ""type"": ""string"" }
        }
      }
    },
    ""changes"": {
      ""type"": [""array"", ""null""],
      ""description"": ""For type=pc-modification. Each entry replaces the part for the given category. productId may be null to remove the part."",
      ""items"": {
        ""type"": ""object"",
        ""additionalProperties"": false,
        ""required"": [""categoryName"", ""productId""],
        ""properties"": {
          ""categoryName"": { ""type"": ""string"" },
          ""productId"": { ""type"": [""string"", ""null""] }
        }
      }
    }
  }
}";
    }
}
