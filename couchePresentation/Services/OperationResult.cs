namespace RefugeAnimaux.couchePresentation.Services;

public class OperationResult<T>
{
    private OperationResult(bool succes, string message, T? valeur)
    {
        Succes = succes;
        Message = message;
        Valeur = valeur;
    }

    public bool Succes { get; }
    public string Message { get; }
    public T? Valeur { get; }

    public static OperationResult<T> Ok(string message, T valeur)
    {
        return new OperationResult<T>(true, message, valeur);
    }

    public static OperationResult<T> Erreur(string message)
    {
        return new OperationResult<T>(false, message, default);
    }
}
