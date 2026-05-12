namespace BASTION.Services.Storage;

/// <summary>
/// Generic EF Core repository — all modules use via IRepository&lt;T&gt; interface.
/// Modules never touch DbContext directly.
/// </summary>
public class EFRepository<T> where T : class
{
    // TODO: Implement generic CRUD operations via EF Core
    // TODO: Expose via IRepository<T> interface
}
