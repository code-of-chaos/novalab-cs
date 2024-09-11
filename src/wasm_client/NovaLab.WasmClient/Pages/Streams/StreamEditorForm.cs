// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace NovaLab.WasmClient.Pages.Streams;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class StreamEditorForm { 
    [Required, StringLength(124, ErrorMessage = "Title Length exceeded maximum length of 124 characters")]
    public string TwitchTitle { get; set; } = string.Empty;
    public string? TwitchBroadcastLanguage { get; set; } = null;
    public string? TwitchTags { get; set; } = null;
        
    [Required]
    public string TwitchGameTitleName { get; set; }  = string.Empty;
}
