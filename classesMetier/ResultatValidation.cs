// ============================================================================
//  ResultatValidation.cs - COUCHE METIER
//  Petit objet métier utilisé quand une classe doit valider ses propres règles.
// ============================================================================

namespace RefugeAnimaux.classesMetier;

public class ResultatValidation
{
    private readonly List<string> _erreurs = new();

    public IReadOnlyList<string> Erreurs => _erreurs;
    public bool EstValide => _erreurs.Count == 0;
    public string PremierMessage => EstValide ? string.Empty : _erreurs[0];

    public void AjouterErreur(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
            _erreurs.Add(message);
    }

    public static ResultatValidation Valide()
    {
        return new ResultatValidation();
    }

    public static ResultatValidation Invalide(string message)
    {
        var resultat = new ResultatValidation();
        resultat.AjouterErreur(message);
        return resultat;
    }
}
