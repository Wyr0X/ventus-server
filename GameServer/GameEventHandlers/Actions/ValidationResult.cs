public struct ValidationResult
{
    public bool IsValid { get; }
    public string Reason  { get; }

    private ValidationResult(bool valid, string reason)
    {
        IsValid  = valid;
        Reason   = reason;
    }

    // Para indicar que todo salió bien
    public static ValidationResult Success()
        => new ValidationResult(true, string.Empty);

    // Para indicar que algo falló, con descripción
    public static ValidationResult Fail(string reason)
        => new ValidationResult(false, reason);
}
