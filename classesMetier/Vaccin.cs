// ============================================================================
//  Vaccin.cs  —  COUCHE MÉTIER
//  Une entrée du CATALOGUE de vaccins (le nom d'un vaccin existant).
//  À ne pas confondre avec Vaccination (= un vaccin réellement administré).
// ============================================================================

namespace RefugeAnimaux.classesMetier;

public class Vaccin
{
    public int IdVaccin { get; set; }                       // clé primaire
    public string NomVaccin { get; set; } = string.Empty;   // ex. "Rage", "Typhus"

    // "=>" : ToString() tient sur une ligne (membre à corps d'expression).
    public override string ToString() => $"[{IdVaccin}] {NomVaccin}";
}
