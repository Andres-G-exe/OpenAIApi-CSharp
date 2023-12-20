using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OpenAIApi
{
    public class OpenAIApi
    {
        private readonly string OpenAIApiKey;
        private readonly HttpClient HttpClient;

        public static string ChatApiUrl => "https://api.openai.com/v1/chat/completions";
        public static string TranscriptionApiUrl => "https://api.openai.com/v1/audio/transcriptions";

        public ChatClient Chat;
        public TranscriptionClient Transcription;
        
        public OpenAIApi(string apikey)
        {
            OpenAIApiKey = apikey;

            HttpClient = CrearHttpClient();

            Chat = new ChatClient(HttpClient);

            Transcription = new TranscriptionClient(HttpClient);
        }
        public HttpClient CrearHttpClient()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {OpenAIApiKey}");

            return client;
        } 
    }
    public class ChatClient
    {
        public readonly List<Message> Messages = new List<Message> { };
        private readonly HttpClient HttpClient;

        public ChatClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
        public async Task<ChatCompletionResponse?> GetChatCompletion(string model = "gpt-3.5-turbo", double temperature = 0.5, int tokens = 250)
        {
            if (Messages.Count == 0) { throw new InvalidMessageException(); }
            using var request = new HttpRequestMessage(HttpMethod.Post, OpenAIApi.ChatApiUrl);
            var jsonBody = new JsonObject
                    {
                        { "model", model },
                        { "messages", JsonNode.Parse(JsonSerializer.Serialize(Messages)) },
                        { "temperature", temperature },
                        { "max_tokens", tokens }
                    };

            var requestBody = jsonBody.ToString();

            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            try
            {
                var response = await HttpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    var classResponse = JsonSerializer.Deserialize<ChatCompletionResponse>(jsonResponse);

                    return classResponse;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ConectionErrorException(ex);
            }
        }
        public void AddMessage(string role, string content)
        {
            Messages.Add(new Message { Role = role, Content = content});
        }

        public void AddUserMessage(string content)
        {
            AddMessage("user", content);
        }

        public void AddAssistantMessage(string content)
        {
            AddMessage("assistant", content);
        }

        public void AddSystemMessage(string content)
        {
            AddMessage("system", content);
        }
        public class InvalidMessageException : Exception
        {
            public InvalidMessageException() : base("Messages list can't be empty. Please use the AddMessage, AddUserMessage, AddAssistantMessage, or AddSystemMessage methods to add messages before making the request.")
            {
            }
        }
    }
    public class TranscriptionClient
    {
        private readonly HttpClient HttpClient;
        public TranscriptionClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
        public async Task<TranscriptionResponse?> GetTranscription(string audiopath, string model = "whisper-1", string language = "en")
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, OpenAIApi.TranscriptionApiUrl);
            using var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(model), "model");
            formData.Add(new StringContent(language), "language");

            var fileContent = new ByteArrayContent(File.ReadAllBytes(audiopath));
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "file",
                FileName = Path.GetFileName(audiopath)
            };

            formData.Add(fileContent);

            request.Content = formData;

            try
            {
                var response = await HttpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    var classResponse = JsonSerializer.Deserialize<TranscriptionResponse>(jsonResponse);

                    return classResponse;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ConectionErrorException(ex);
            }
        }
    }
    public class ConectionErrorException : Exception
    {
        private static readonly string message = "There's a connection problem; the system can't communicate with api.openai.com. Please check your internet connection, DNS, firewall settings, or proxy configuration.";
        public ConectionErrorException() : base(message)
        {
        } 
        public ConectionErrorException(Exception innerException) : base(message, innerException)
        {
        }
    }
}