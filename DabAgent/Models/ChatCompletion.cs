namespace DabAgent.Models;

public class ChatCompletion
{
    public List<Choice> Choices { get; set; } = [];
    public long Created { get; set; }
    public required string Id { get; set; }
    public required string Model { get; set; }
    public List<PromptFilterResult> PromptFilterResults { get; set; } = [];
    public string SystemFingerprint { get; set; }
    public Usage Usage { get; set; }
}

public class Choice
{
    public ContentFilterResults ContentFilterResults { get; set; }
    public string FinishReason { get; set; }
    public int Index { get; set; }
    public ChatMessage Message { get; set; }
}

public class ContentFilterResults
{
    public FilterResult Hate { get; set; }
    public FilterResult SelfHarm { get; set; }
    public FilterResult Sexual { get; set; }
    public FilterResult Violence { get; set; }
}

public class FilterResult
{
    public bool Filtered { get; set; }
    public string Severity { get; set; }
}

public class PromptFilterResult
{
    public ContentFilterResults ContentFilterResults { get; set; }
    public int PromptIndex { get; set; }
}

public class Usage
{
    public int CompletionTokens { get; set; }
    public CompletionTokensDetails CompletionTokensDetails { get; set; }
    public int PromptTokens { get; set; }
    public PromptTokensDetails PromptTokensDetails { get; set; }
    public int TotalTokens { get; set; }
}

public class CompletionTokensDetails
{
    public int ReasoningTokens { get; set; }
}

public class PromptTokensDetails
{
    public int CachedTokens { get; set; }
}
