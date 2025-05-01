public class BuyCartRequest
{
    public int PlayerId { get; set; }
    public List<CartItem> cartItems { get; set; } = [];
    public List<CartSpell> cartSpells { get; set; } = [];
}
public class BuyResult
{
    public bool Success { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static BuyResult Fail(string error) => new BuyResult { Success = false, ErrorMessage = error };
    public static BuyResult CreateSuccess()
    {
        return new BuyResult { Success = true };
    }
}

public class CartItem
{
    public int ItemId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class CartSpell
{
    public required string SpellId { get; set; }
    public int Quantity { get; set; } = 1;
}
