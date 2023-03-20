namespace Shared.Models;

public class Owner
{
    public Guid Guid { get; set; }
    public List<Guid> TodoKeys { get; set; } = new();
}