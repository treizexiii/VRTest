namespace GameServicesAPI.Dto;

public class TodoItemDto
{
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public Guid OwnerKey { get; set; }
}