// ============================================================================
//  Enums.cs  —  COUCHE MÉTIER
//  Ce fichier regroupe toutes les énumérations (listes de valeurs fixes) du
//  projet, ainsi qu'une classe utilitaire qui fait le pont entre ces
//  énumérations C# et le texte réellement stocké dans la base PostgreSQL.
// ============================================================================

namespace RefugeAnimaux.classesMetier;

// Une énumération = un type dont les valeurs possibles sont connues d'avance.
// Avantage : le compilateur empêche d'utiliser une valeur invalide (ex. un
// animal ne peut être que Chat ou Chien), ce qui évite les fautes de frappe.

// Espèce de l'animal.
public enum TypeAnimal
{
    Chat,
    Chien
}

// Sexe de l'animal (M = mâle, F = femelle).
public enum Sexe
{
    M,
    F
}

// Rôle d'une personne de contact dans le refuge.
public enum TypeContact
{
    Benevole,
    Adoptant,
    Candidat,
    FamilleAccueil
}

// Ce avec quoi (ou qui) l'animal est testé comme compatible.
public enum TypeCompatibilite
{
    Chat,
    Chien,
    JeuneEnfant,
    Enfant,
    Jardin,
    Poney
}

// Résultat d'un test de compatibilité.
public enum ValeurCompatibilite
{
    Oui,
    Non
}

// État d'avancement d'une demande d'adoption.
public enum StatutAdoption
{
    Demande,
    Acceptee,
    RejetEnvironnement,
    RejetComportement
}

/// <summary>
/// Conversions entre énumérations C# et chaînes stockées en base.
/// Les chaînes correspondent aux contraintes CHECK définies dans le script SQL.
/// </summary>
// Pourquoi cette classe En base, les valeurs sont stockées sous forme de
// texte (ex. "chat", "adoption acceptée"). Côté C#, on manipule des
// énumérations. Cette classe traduit dans les deux sens :
//   - ToDb(...)      : enum  -> texte (avant d'écrire en base)
//   - ToXxx(string)  : texte -> enum  (après lecture depuis la base)
// "static" : on appelle ces méthodes sans créer d'objet (EnumsConversion.ToTypeAnimal(...)).
public static class EnumsConversion
{
    // -------- TypeAnimal --------

    // "this TypeAnimal v" => méthode d'extension : on pourra écrire monType.ToDb().
    // "v switch { ... }" => expression switch : renvoie une valeur selon v.
    public static string ToDb(this TypeAnimal v) => v switch
    {
        TypeAnimal.Chat => "chat",
        TypeAnimal.Chien => "chien",
        _ => throw new ArgumentOutOfRangeException(nameof(v)) // "_" = tout autre cas (sécurité)
    };

    // Sens inverse : on normalise la chaîne (Trim + minuscules) avant de comparer.
    public static TypeAnimal ToTypeAnimal(string s) => s.Trim().ToLower() switch
    {
        "chat" => TypeAnimal.Chat,
        "chien" => TypeAnimal.Chien,
        _ => throw new ArgumentException($"TypeAnimal inconnu : {s}") // texte non reconnu
    };

    // -------- Sexe --------
    public static string ToDb(this Sexe v) => v switch
    {
        Sexe.M => "M",
        Sexe.F => "F",
        _ => throw new ArgumentOutOfRangeException(nameof(v))
    };

    // Ici on met en MAJUSCULES car la base attend "M" ou "F".
    public static Sexe ToSexe(string s) => s.Trim().ToUpper() switch
    {
        "M" => Sexe.M,
        "F" => Sexe.F,
        _ => throw new ArgumentException($"Sexe inconnu : {s}")
    };

    // -------- TypeContact --------
    public static string ToDb(this TypeContact v) => v switch
    {
        TypeContact.Benevole => "benevole",
        TypeContact.Adoptant => "adoptant",
        TypeContact.Candidat => "candidat",
        TypeContact.FamilleAccueil => "famille_accueil",
        _ => throw new ArgumentOutOfRangeException(nameof(v))
    };

    public static TypeContact ToTypeContact(string s) => s.Trim().ToLower() switch
    {
        "bénévole" or "benevole" => TypeContact.Benevole, // on accepte avec/sans accent
        "adoptant" => TypeContact.Adoptant,
        "candidat" => TypeContact.Candidat,
        "famille_accueil" or "famille accueil" => TypeContact.FamilleAccueil,
        _ => throw new ArgumentException($"TypeContact inconnu : {s}")
    };

    // -------- TypeCompatibilite --------
    public static string ToDb(this TypeCompatibilite v) => v switch
    {
        TypeCompatibilite.Chat => "chat",
        TypeCompatibilite.Chien => "chien",
        TypeCompatibilite.JeuneEnfant => "jeune enfant",
        TypeCompatibilite.Enfant => "enfant",
        TypeCompatibilite.Jardin => "jardin",
        TypeCompatibilite.Poney => "poney",
        _ => throw new ArgumentOutOfRangeException(nameof(v))
    };

    public static TypeCompatibilite ToTypeCompatibilite(string s) => s.Trim().ToLower() switch
    {
        "chat" => TypeCompatibilite.Chat,
        "chien" => TypeCompatibilite.Chien,
        "jeune enfant" => TypeCompatibilite.JeuneEnfant,
        "enfant" => TypeCompatibilite.Enfant,
        "jardin" => TypeCompatibilite.Jardin,
        "poney" => TypeCompatibilite.Poney,
        _ => throw new ArgumentException($"TypeCompatibilite inconnu : {s}")
    };

    // -------- ValeurCompatibilite --------
    public static string ToDb(this ValeurCompatibilite v) => v switch
    {
        ValeurCompatibilite.Oui => "oui",
        ValeurCompatibilite.Non => "non",
        _ => throw new ArgumentOutOfRangeException(nameof(v))
    };

    public static ValeurCompatibilite ToValeurCompatibilite(string s) => s.Trim().ToLower() switch
    {
        "oui" => ValeurCompatibilite.Oui,
        "non" => ValeurCompatibilite.Non,
        _ => throw new ArgumentException($"ValeurCompatibilite inconnue : {s}")
    };

    // -------- StatutAdoption --------
    public static string ToDb(this StatutAdoption v) => v switch
    {
        StatutAdoption.Demande => "demande",
        StatutAdoption.Acceptee => "acceptee",
        StatutAdoption.RejetEnvironnement => "rejet_environnement",
        StatutAdoption.RejetComportement => "rejet_comportement",
        _ => throw new ArgumentOutOfRangeException(nameof(v))
    };

    public static StatutAdoption ToStatutAdoption(string s) => s.Trim().ToLower() switch
    {
        "demande" or "demande adoption" => StatutAdoption.Demande,
        "acceptee" or "acceptée" or "adoption acceptée" or "adoption acceptee" => StatutAdoption.Acceptee,
        "rejet_environnement" or "rejet environnement" => StatutAdoption.RejetEnvironnement,
        "rejet_comportement" or "rejet comportement" => StatutAdoption.RejetComportement,
        _ => throw new ArgumentException($"StatutAdoption inconnu : {s}")
    };
}
