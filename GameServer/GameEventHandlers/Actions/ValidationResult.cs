public struct ValidationResult
{
    public bool IsValid { get; private set; }
    public string Reason { get; private set; }

    private ValidationResult(bool valid, string reason)
    {
        IsValid = valid;
        Reason = reason;
    }

    // Crea un resultado exitoso
    public static ValidationResult Success() => new ValidationResult(true, string.Empty);

    // Crea un resultado de fallo
    public static ValidationResult Fail(string reason) => new ValidationResult(false, reason);

    // Método para añadir una razón al resultado actual, sin perder el estado anterior
    public ValidationResult SetValidation(bool isValid, string reason) => new ValidationResult(isValid, Reason + " " + reason);
}
