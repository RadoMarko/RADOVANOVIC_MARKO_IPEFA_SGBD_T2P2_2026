namespace RefugeAnimaux.classesMetier;

public class Compatibilite
{
    public int IdCompat { get; set; }
    public TypeCompatibilite Type { get; set; }
    public ValeurCompatibilite Valeur { get; set; }
    public string? Description { get; set; }
    public string IdAnimal { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"[{IdCompat}] {Type} : {Valeur}" +
               (string.IsNullOrWhiteSpace(Description) ? "" : $" - {Description}");
    }
}
