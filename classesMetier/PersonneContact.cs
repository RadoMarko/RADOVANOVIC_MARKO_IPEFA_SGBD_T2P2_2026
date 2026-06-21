// ============================================================================
//  PersonneContact.cs - COUCHE METIER
//  Toute personne en lien avec le refuge. Une meme personne peut avoir
//  plusieurs roles: benevole, adoptant, candidat, famille_accueil.
// ============================================================================

namespace RefugeAnimaux.classesMetier;

using System.Text.RegularExpressions;

public class PersonneContact
{
    private static readonly Regex RegistreNationalRegex = new(@"^[0-9]{2}\.[0-9]{2}\.[0-9]{2}-[0-9]{3}\.[0-9]{2}$");
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    public int IdContact { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;

    public string? RegistreNational { get; set; }
    public string? Rue { get; set; }
    public string? Cp { get; set; }
    public string? Localite { get; set; }
    public string? GSM { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public List<TypeContact> Roles { get; set; } = new();
    public string NomComplet => $"{Prenom} {Nom}".Trim();
    public string RolesAffichage => RolesTexte();

    public bool AUnMoyenContact()
    {
        return !string.IsNullOrWhiteSpace(GSM)
               || !string.IsNullOrWhiteSpace(Telephone)
               || !string.IsNullOrWhiteSpace(Email);
    }

    public bool ALeRole(TypeContact role)
    {
        return Roles.Contains(role);
    }

    public string RolesTexte()
    {
        return Roles.Count == 0
            ? "(aucun)"
            : string.Join(", ", Roles.Select(r => r.ToDisplay()));
    }

    public string AdresseTexte()
    {
        var adresse = string.Join(", ", new[] { Rue, Cp, Localite }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

        return adresse.Length == 0 ? "(non communiquee)" : adresse;
    }

    public ResultatValidation VerifierReglesMetier()
    {
        var resultat = ResultatValidation.Valide();

        if (Nom.Trim().Length < 2 || Prenom.Trim().Length < 2)
            resultat.AjouterErreur("Le nom et le prénom doivent contenir au moins 2 caractères.");

        if (!string.IsNullOrWhiteSpace(RegistreNational) && !RegistreNationalFormatValide(RegistreNational))
            resultat.AjouterErreur("Le registre national doit respecter le format yy.mm.dd-999.99.");

        if (!string.IsNullOrWhiteSpace(Email) && !EmailFormatValide(Email))
            resultat.AjouterErreur("L'email doit respecter un format valide, par exemple nom@example.com.");

        if (!AUnMoyenContact())
            resultat.AjouterErreur("Au moins un moyen de contact est obligatoire : GSM, téléphone ou email.");

        if (Roles.Count == 0)
            resultat.AjouterErreur("Au moins un rôle est requis.");

        return resultat;
    }

    public static bool RegistreNationalFormatValide(string registreNational)
    {
        return RegistreNationalRegex.IsMatch(registreNational);
    }

    public static bool EmailFormatValide(string email)
    {
        return EmailRegex.IsMatch(email);
    }

    public override string ToString()
    {
        return $"[{IdContact}] {Prenom} {Nom}\n" +
               $"  Roles             : {RolesTexte()}\n" +
               $"  Registre national : {RegistreNational ?? "(non communique)"}\n" +
               $"  Adresse           : {AdresseTexte()}\n" +
               $"  GSM               : {GSM ?? "-"}    Telephone : {Telephone ?? "-"}\n" +
               $"  Email             : {Email ?? "-"}";
    }
}
