namespace BASTION.Services.Gamification;

public class XpService
{
    public int CurrentXp { get; private set; } = 0;
    public string CurrentRank => GetRank(CurrentXp);
    public List<string> Badges { get; private set; } = new();

    public event Action? OnXpChanged;

    public void AddXp(int xp)
    {
        CurrentXp += xp;
        OnXpChanged?.Invoke();
    }

    public void SetXp(int xp)
    {
        CurrentXp = xp;
        OnXpChanged?.Invoke();
    }

    public void UnlockBadge(string badgeName)
    {
        if (!Badges.Contains(badgeName))
        {
            Badges.Add(badgeName);
            OnXpChanged?.Invoke();
        }
    }

    public string GetRank(int xp)
    {
        if (xp >= 1000) return "Cyber Guardian";
        if (xp >= 600) return "Ethical Hacker";
        if (xp >= 300) return "White Hat Analyst";
        if (xp >= 100) return "Script Kiddie";
        return "Civilian";
    }
}
