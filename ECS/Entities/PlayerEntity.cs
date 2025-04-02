public class PlayerEntity : Entity
{
    private string _userId { get; set;}

  public PlayerEntity(int id, string userId) : base(id) // Llamada al constructor base
    {
        _userId = userId;
    }


    public string GetUserId()
    {
        return _userId;
    }

}