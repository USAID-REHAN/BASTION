using System;

namespace BASTION.Services.Finance;

public class FinShieldSessionState
{
    public int TotalScansToday { get; private set; } = 0;
    public int ThreatsBlocked { get; private set; } = 0;

    // Protection Score starts at 100% and reduces if threat detected, or matches security status
    public int ProtectionScore => TotalScansToday == 0 ? 100 : Math.Max(0, 100 - (ThreatsBlocked * 15));

    public event Action? OnStateChanged;

    public void RecordScan(bool isThreat)
    {
        TotalScansToday++;
        if (isThreat)
        {
            ThreatsBlocked++;
        }
        NotifyStateChanged();
    }

    public void NotifyStateChanged() => OnStateChanged?.Invoke();
}
