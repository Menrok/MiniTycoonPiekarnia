namespace MiniTycoonPiekarnia.Models.Campaign;

public class CampaignProgress
{
    public int CurrentStepIndex { get; set; } = 0;
    public List<string> CompletedObjectives { get; set; } = new();
}
