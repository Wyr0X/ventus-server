public class InvokerModel : BaseModel
{
    public required Guid InvokerId { get; set; }
    public required Guid AccountId { get; set; }
    public required string InvokerName { get; set; }
    public required string ElementType { get; set; }
    public int PowerLevel { get; set; } = 1;
    public DateTime CreatedAt { get; set; }

    public override string ToString()
        => $"[Invoker: {InvokerName} | Element: {ElementType} | Power: {PowerLevel} | AccountId: {AccountId}]";
}
