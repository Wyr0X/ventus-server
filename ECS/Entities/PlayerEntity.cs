public class PlayerEntity : Entity
{
    private Guid _accountId { get; set;}

  public PlayerEntity(int id, Guid accountId) : base(id) // Llamada al constructor base
    {
        _accountId = accountId;
    }


    public Guid GetUserId()
    {
        return _accountId;
    }

}