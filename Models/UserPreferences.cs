namespace LiveNewsAggregator.Models;

public class UserPreferences
{
    public string UserId { get; set; } = "default";
    public List<string> Interests { get; set; } = new() { "سياسة", "تكنولوجيا", "رياضة" };
}