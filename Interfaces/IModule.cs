namespace BASTION.Interfaces;

/// <summary>
/// The core BASTION module contract.
/// Every module — current and future — implements this single interface.
/// The Homescreen only knows about IModule. It knows nothing else about any module.
/// </summary>
public interface IModule
{
    /// <summary>Module display name (e.g., "CyberShield")</summary>
    string Name { get; }

    /// <summary>Icon identifier or emoji for the module</summary>
    string Icon { get; }

    /// <summary>Route URL path (e.g., "/cybershield")</summary>
    string RouteUrl { get; }

    /// <summary>Short description shown on dashboard card</summary>
    string Description { get; }

    /// <summary>Primary hex color for this module (e.g., "#0891B2")</summary>
    string PrimaryHex { get; }

    /// <summary>CSS class name for this module (e.g., "module-cyber")</summary>
    string CssClass { get; }

    /// <summary>Sidebar display order</summary>
    int Order { get; }

    /// <summary>Returns the module's protection score for the given user</summary>
    Task<int> GetScoreAsync(string userId);
}
