namespace RefugeAnimaux.classesMetier;

/// <summary>
/// Animal du refuge. L'identifiant est de la forme yymmdd99999.
/// </summary>
public class Animal
{
    public string IdAnimal { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public TypeAnimal Type { get; set; }
    public Sexe Sexe { get; set; }
    public List<string> Couleurs { get; set; } = new();
    public bool Sterilise { get; set; }
    public DateTime? DateSterilisation { get; set; }
    public DateTime? DateNaissance { get; set; }
    public DateTime? DateDeces { get; set; }
    public DateTime? DateEntree { get; set; }
    public string? MotifEntree { get; set; }
    public string? ContactEntree { get; set; }
    public DateTime? DateSortie { get; set; }
    public string? MotifSortie { get; set; }
    public string? ContactSortie { get; set; }
    public string? Description { get; set; }
    public string? Particularites { get; set; }

    public bool EstDecede => DateDeces.HasValue;
    public string NomAvecId => $"{Nom} [{IdAnimal.Trim()}]";
    public string TypeTexte => Type.ToDisplay();
    public string SexeTexte => Sexe.ToDisplay();
    public string CouleursTexte => Couleurs.Count > 0 ? string.Join(", ", Couleurs) : "(aucune)";
    public string SteriliseTexte => Sterilise ? "Oui" : "Non";
    public string MotifEntreeTexte => EnumsConversion.MotifEntreeAffichage(MotifEntree);
    public string MotifSortieTexte => EnumsConversion.MotifSortieAffichage(MotifSortie);

    public DateTime? DateEntreeRefuge()
    {
        if (IdAnimal.Length >= 6 &&
            DateTime.TryParseExact(IdAnimal[..6], "yyMMdd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var d))
        {
            return d;
        }

        return null;
    }

    public bool AjouterCouleur(string couleur, out string erreur)
    {
        couleur = couleur.Trim();
        erreur = string.Empty;

        if (couleur.Length == 0)
        {
            erreur = "La couleur ne peut pas être vide.";
            return false;
        }

        if (couleur.Any(char.IsDigit))
        {
            erreur = "Une couleur ne peut pas contenir de chiffre.";
            return false;
        }

        if (!Couleurs.Contains(couleur, StringComparer.OrdinalIgnoreCase))
            Couleurs.Add(couleur);

        return true;
    }

    public ResultatValidation VerifierCreation(
        DateTime dateEntree,
        bool dateSterilisationMoisAnneeSeulement = false)
    {
        var resultat = ResultatValidation.Valide();
        DateTime today = DateTime.Today;

        if (Nom.Trim().Length < 2)
            resultat.AjouterErreur("Le nom doit contenir au moins 2 caractères.");

        if (dateEntree.Date > today)
            resultat.AjouterErreur("La date d'entrée au refuge ne peut pas être dans le futur.");

        if (DateNaissance.HasValue)
        {
            if (DateNaissance.Value.Date > today)
                resultat.AjouterErreur("La date de naissance ne peut pas être dans le futur.");

            if (dateEntree.Date < DateNaissance.Value.Date)
                resultat.AjouterErreur("La date d'entrée au refuge est antérieure à la date de naissance.");
        }

        if (!Sterilise && DateSterilisation.HasValue)
            resultat.AjouterErreur("Un animal non stérilisé ne peut pas avoir de date de stérilisation.");

        if (Sterilise && !DateSterilisation.HasValue)
            resultat.AjouterErreur("Un animal stérilisé doit avoir une date de stérilisation.");

        if (DateSterilisation.HasValue)
        {
            if (DateSterilisationDansFutur(DateSterilisation.Value, dateSterilisationMoisAnneeSeulement))
                resultat.AjouterErreur("La date de stérilisation ne peut pas être dans le futur.");

            if (DateNaissance.HasValue &&
                DateSterilisationAvantNaissance(
                    DateSterilisation.Value,
                    dateSterilisationMoisAnneeSeulement,
                    DateNaissance.Value))
            {
                resultat.AjouterErreur("La date de stérilisation est antérieure à la date de naissance.");
            }
        }

        return resultat;
    }

    public ResultatValidation VerifierSortie(DateTime dateSortie)
    {
        if (dateSortie.Date > DateTime.Today)
            return ResultatValidation.Invalide("La date de sortie ne peut pas être dans le futur.");

        var dateEntree = DateEntreeRefuge();
        if (dateEntree.HasValue && dateSortie.Date < dateEntree.Value.Date)
        {
            return ResultatValidation.Invalide(
                $"La date de sortie ({dateSortie:dd/MM/yyyy}) est antérieure à la date d'entrée au refuge ({dateEntree:dd/MM/yyyy}).");
        }

        if (DateNaissance.HasValue && dateSortie.Date < DateNaissance.Value.Date)
            return ResultatValidation.Invalide("La date de sortie est antérieure à la date de naissance de l'animal.");

        if (DateDeces.HasValue && dateSortie.Date < DateDeces.Value.Date)
            return ResultatValidation.Invalide("La date de sortie est antérieure à la date de décès enregistrée.");

        return ResultatValidation.Valide();
    }

    private static bool DateSterilisationDansFutur(DateTime dateSterilisation, bool moisAnneeSeulement)
    {
        DateTime today = DateTime.Today;
        return moisAnneeSeulement
            ? dateSterilisation.Year > today.Year ||
              (dateSterilisation.Year == today.Year && dateSterilisation.Month > today.Month)
            : dateSterilisation.Date > today;
    }

    private static bool DateSterilisationAvantNaissance(
        DateTime dateSterilisation,
        bool moisAnneeSeulement,
        DateTime dateNaissance)
    {
        return moisAnneeSeulement
            ? dateSterilisation.Year < dateNaissance.Year ||
              (dateSterilisation.Year == dateNaissance.Year && dateSterilisation.Month < dateNaissance.Month)
            : dateSterilisation.Date < dateNaissance.Date;
    }

    public override string ToString()
    {
        var couleurs = Couleurs.Count > 0 ? string.Join(", ", Couleurs) : "(aucune)";
        return $"[{IdAnimal}] {Nom} - {Type} - {Sexe}\n" +
               $"  Couleurs        : {couleurs}\n" +
               $"  Stérilisé       : {(Sterilise ? "oui" : "non")}" +
               (DateSterilisation.HasValue ? $" (le {DateSterilisation:dd/MM/yyyy})" : "") + "\n" +
               $"  Date naissance  : {(DateNaissance.HasValue ? DateNaissance.Value.ToString("dd/MM/yyyy") : "(inconnue)")}\n" +
               $"  Date décès      : {(DateDeces.HasValue ? DateDeces.Value.ToString("dd/MM/yyyy") : "(en vie)")}\n" +
               $"  Description     : {Description ?? "(aucune)"}\n" +
               $"  Particularités  : {Particularites ?? "(aucune)"}";
    }
}
