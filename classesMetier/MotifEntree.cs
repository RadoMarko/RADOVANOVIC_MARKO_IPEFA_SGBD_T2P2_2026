// ============================================================================
//  MotifEntree.cs  —  COUCHE MÉTIER
//  Raison d'arrivée d'un animal au refuge (abandon, trouvé errant, saisie...).
//  C'est une table de "référence" : une liste de libellés réutilisables.
// ============================================================================

namespace RefugeAnimaux.classesMetier;

public class MotifEntree
{
    public int IdMotifEntree { get; set; }                  // clé primaire
    public string Libelle { get; set; } = string.Empty;     // texte affiché (ex. "Abandon")

    public override string ToString() => $"[{IdMotifEntree}] {Libelle}";
}
