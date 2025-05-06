namespace MiniTycoonPiekarnia.Models.Campaign;
public static class CampaignStepList
{
    public static List<CampaignStep> All => new()
    {
        new CampaignStep
        {
            Title = "Nowy początek",
            Subtitle = "Wyposażenie piekarni",
            Description = "Zacznij odbudowę piekarni – kup najpotrzebniejsze rzeczy.",
            Objectives = new List<CampaignObjective>
            {
                new() { Id = "buy-oven", Description = "Kup piec" },
                new() { Id = "buy-shelf", Description = "Kup półkę" },
                new() { Id = "buy-cooling", Description = "Kup chłodnię" }
            },
            Reward = new CampaignReward { Money = 100 }
        },
        new CampaignStep
        {
            Title = "Nowy początek",
            Subtitle = "Pierwszy wypiek",
            Description = "Kup składniki w sklepie i upiecz pierwszy chleb.",
            Objectives = new List<CampaignObjective>
            {
                new() { Id = "buy-maka", Description = "Kup mąkę" },
                new() { Id = "buy-drozdze", Description = "Kup drożdże" },
                new() { Id = "bake-bread", Description = "Upiecz chleb" }
            },
            Reward = new CampaignReward { Experience = 20 }
        },
        new CampaignStep
        {
            Title = "Nowy początek",
            Subtitle = "Obsługa klienta",
            Description = "Zrealizuj pierwsze zamówienie, by zarobić swoje pierwsze pieniądze.",
            Objectives = new List<CampaignObjective>
            {
                new() { Id = "complete-order", Description = "Obsłuż klienta" }
            },
            Reward = new CampaignReward { Money = 100 }
        },
        new CampaignStep
{
            Title = "Nowy początek",
            Subtitle = "Zarobki",
            Description = "Realizuj zamówienia i zarób 200 Paksów, by móc dalej inwestować.",
            Objectives = new List<CampaignObjective>
            {
                new() { Id = "earn-200", Description = "Zarób łącznie 200 Paksów" }
            },
            Reward = new CampaignReward { Experience = 30 }
        }
    };
}