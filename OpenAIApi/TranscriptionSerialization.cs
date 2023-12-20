using System.Text.Json.Serialization;

namespace OpenAIApi
{
    public class TranscriptionResponse
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
