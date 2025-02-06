using System.Text.Json;

namespace DabAgent
{
    public static class HttpResponseExtensions
    {
        public static async Task WriteAckStreamEvent(this HttpResponse response)
        {
            await response.WriteTextStreamEvent(string.Empty);
        }

        public static async Task WriteTextStreamEvent(this HttpResponse response, string text)
        {
            var textObject = new
            {
                choices = new[]
                {
                    new { index = 0, delta = new { content = text, role = "assistant" } }
                }
            };
            response.ContentType = "text/event-stream";
            await response.WriteAsync($"data: {JsonSerializer.Serialize(textObject)}\n\n");
        }

        public static async Task WriteConfirmationStreamEvent(this HttpResponse response, string id, string title, string message)
        {
            var confirmationObject = new
            {
                type = "action",
                title,
                message,
                confirmation = new
                {
                    id
                }
            };
            response.ContentType = "text/event-stream";
            await response.WriteAsync("event: copilot_confirmation\n");
            await response.WriteAsync($"data: {JsonSerializer.Serialize(confirmationObject)}\n\n");
        }

        public static async Task WriteEndStreamEvent(this HttpResponse response)
        {
            var endObject = new
            {
                choices = new[]
                {
                    new { index = 0, finish_reason = "stop", delta = new { content = (string)null } }
                }
            };
            await response.WriteAsync($"data: {JsonSerializer.Serialize(endObject)}\n\ndata: [DONE]\n\n");
        }
    }
}
