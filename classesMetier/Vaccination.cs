namespace RefugeAnimaux.classesMetier;

public class Vaccination
{
    public int IdVaccination { get; set; }
    public DateTime DateVaccin { get; set; }
    public bool Fait { get; set; }
    public string IdAnimal { get; set; } = string.Empty;
    public int IdVaccin { get; set; }
    public string? NomVaccin { get; set; }

    public string FaitTexte => Fait ? "fait" : "prévu";

    public override string ToString()
    {
        var nom = NomVaccin ?? $"vaccin #{IdVaccin}";
        return $"[{IdVaccination}] {DateVaccin:dd/MM/yyyy} - {nom} - {FaitTexte}";
    }
}
