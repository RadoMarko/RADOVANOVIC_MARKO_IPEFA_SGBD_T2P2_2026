namespace RefugeAnimaux.classesMetier;

public class FamilleAccueil
{
    public int IdAccueil { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string IdAnimal { get; set; } = string.Empty;
    public int IdContact { get; set; }

    public string? NomAnimal { get; set; }
    public string? NomContact { get; set; }
    public string? PrenomContact { get; set; }

    public string ContactTexte =>
        (PrenomContact != null || NomContact != null)
            ? $"{PrenomContact} {NomContact}".Trim()
            : $"contact #{IdContact}";

    public string PeriodeTexte => $"{DateDebut:dd/MM/yyyy} -> {(DateFin.HasValue ? DateFin.Value.ToString("dd/MM/yyyy") : "en cours")}";

    public override string ToString()
    {
        return $"[{IdAccueil}] {PeriodeTexte} | animal {IdAnimal}" +
               (NomAnimal != null ? $" ({NomAnimal})" : "") +
               $" | famille : {ContactTexte}";
    }
}
