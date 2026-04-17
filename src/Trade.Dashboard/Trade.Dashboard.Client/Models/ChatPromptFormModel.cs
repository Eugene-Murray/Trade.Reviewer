using System.ComponentModel.DataAnnotations;
using Trade.Application.Chat;

namespace Trade.Dashboard.Client.Models;

public sealed class ChatPromptFormModel
{
    [Required]
    [StringLength(500, MinimumLength = 3)]
    public string Question { get; set; } = "How are my open trades performing right now?";

    public AskTradeQuestionRequest ToRequest() => new(Question);
}
