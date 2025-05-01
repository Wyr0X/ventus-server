public class PlayerEntity : Entity
{
    public int PlayerId { get; set;}
    private Guid _accountId { get; set;}

  public PlayerEntity(int id, Guid accountId, int playerId) : base(id) // Llamada al constructor base
    {
        _accountId = accountId;
        PlayerId = playerId;
    }


    public Guid GetAccountId()
    {
        return _accountId;
    }

}