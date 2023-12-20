# OpenAIApi C# Library

This C# library provides a convenient way to interact with the OpenAI API, allowing you to easily integrate OpenAI's powerful language models into your C# applications.

## Features

- **Chat Completion:** Interact with OpenAI's chat models using a simplified and intuitive API.
- **Audio Transcription:** Easily transcribe audio files using OpenAI's transcription service.

## Usage

### Chat Completion

```csharp
// Initialize the OpenAIApi with your API key
var openAIApi = new OpenAIApi("YOUR_OPENAI_API_KEY");

// Create a chat client
var chatClient = openAIApi.Chat;

// Add user, assistant, and system messages
chatClient.AddUserMessage("Hello");
chatClient.AddAssistantMessage("Hi there! How can I help you today?");
chatClient.AddUserMessage("Tell me a joke");

// Get chat completion response
var completionResponse = await chatClient.GetChatCompletion;

// Get response content
Console.WriteLine(completionResponse.Choices[0].Message.Content);
```

### Audio Transcription

```csharp
// Initialize the OpenAIApi with your API key
var openAIApi = new OpenAIApi("YOUR_OPENAI_API_KEY");

// Create a transcription client
var transcriptionClient = openAIApi.Transcription;

// Specify the path to the audio file
string audioFilePath = "path/to/your/audio/file.mp3";

// Get transcription response
var transcriptionResponse = await transcriptionClient.GetTranscription(audioFilePath);

// Get transcription content
Console.WriteLine(transcriptionResponse.Text);
