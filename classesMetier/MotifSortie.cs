// ============================================================================
//  MotifSortie.cs  —  COUCHE MÉTIER
//  Raison de départ d'un animal (adoption, décès, transfert...).
//  Table de référence, miroir de MotifEntree.
// ============================================================================

namespace RefugeAnimaux.classesMetier;

public class MotifSortie
{
    public int IdMotifSortie { get; set; }                  // clé primaire
    public string Libelle { get; set; } = string.Empty;     // texte affiché (ex. "Adoption")
    public string LibelleAffichage => EnumsConversion.MotifSortieAffichage(Libelle);

    public override string ToString() => $"[{IdMotifSortie}] {LibelleAffichage}";
}
