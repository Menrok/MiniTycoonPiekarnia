namespace MiniTycoonPiekarnia.Models.Campaign;

public class CampaignStep
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public List<CampaignObjective> Objectives { get; set; } = new();
    public CampaignReward? Reward { get; set; }
}