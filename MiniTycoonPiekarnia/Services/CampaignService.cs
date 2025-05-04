using MiniTycoonPiekarnia.Models.Bakery;
using MiniTycoonPiekarnia.Models.Campaign;

namespace MiniTycoonPiekarnia.Services;

public class CampaignService
{
    private readonly Func<Bakery> _getBakery;

    private readonly List<CampaignStep> CampaignSteps = CampaignStepList.All;

    public CampaignStep? CurrentStep => GetCurrentStep();

    public CampaignService(Func<Bakery> getBakery)
    {
        _getBakery = getBakery;
    }

    private Bakery Bakery => _getBakery();

    private CampaignStep? GetCurrentStep()
    {
        if (Bakery.CampaignProgress.CurrentStepIndex >= CampaignSteps.Count)
            return null;

        var step = CampaignSteps[Bakery.CampaignProgress.CurrentStepIndex];

        foreach (var obj in step.Objectives)
        {
            obj.IsCompleted = Bakery.CampaignProgress.CompletedObjectives.Contains(obj.Id);
        }

        return step;
    }

    public void MarkObjectiveComplete(string id)
    {
        if (!Bakery.CampaignProgress.CompletedObjectives.Contains(id))
        {
            Bakery.CampaignProgress.CompletedObjectives.Add(id);

            var obj = CurrentStep?.Objectives.FirstOrDefault(o => o.Id == id);
            if (obj is not null) obj.IsCompleted = true;

            if (CurrentStep?.Objectives.All(o => o.IsCompleted) == true)
            {
                if (CurrentStep.Reward is not null)
                {
                    if (CurrentStep.Reward.Money > 0)
                        Bakery.Money += CurrentStep.Reward.Money;

                    if (CurrentStep.Reward.Experience > 0)
                        Bakery.AddExperience(CurrentStep.Reward.Experience);
                }

                Bakery.CampaignProgress.CurrentStepIndex++;
                Bakery.CampaignProgress.CompletedObjectives.Clear();
            }
        }
    }

    public bool IsObjectiveCompleted(string id)
    {
        return Bakery.CampaignProgress.CompletedObjectives.Contains(id);
    }
}
