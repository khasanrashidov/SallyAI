namespace OpenAiSample.WebApi;

public static partial class Constants
{
    public static class OpenAiConstants
    {
        public const string OpenAiSourceApi = "https://api.openai.com/v1/";

        public const string Purpose = "assistants";

        public const string Instructions = "You are a personal math tutor. When asked a question, write and run Python code to answer the question. Use all uploaded files";
    }

    public static class Errors
    {
        public const string Internal = "INTERNAL_SERVER_ERROR";

        public const string NotFound = "NOT_FOUND_ERROR";

        public const string Validation = "VALIDATION_ERROR";
    }

}