namespace FeatureFlags.Api.Data;

public class FeatureFlag
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool IsEnabled { get; set; }
}