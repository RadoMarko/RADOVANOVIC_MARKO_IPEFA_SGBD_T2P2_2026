// ============================================================================
//  GestionErreurs.cs  —  COUCHE PRÉSENTATION
//  Transforme une exception technique (souvent illisible) en un message clair
//  en français. Les menus appellent GestionErreurs.Traduire(ex) dans leur
//  bloc catch pour afficher quelque chose de compréhensible à l'utilisateur.
// ============================================================================

using Npgsql;

namespace RefugeAnimaux.couchePresentation;

/// <summary>
/// Traduit les exceptions techniques en messages clairs pour l'utilisateur.
/// </summary>
internal static class GestionErreurs
{
    // Prend l'exception attrapée et renvoie une phrase lisible.
    public static string Traduire(Exception ex)
    {
        // CAS 1 : erreur renvoyée PAR PostgreSQL (contrainte violée, doublon...).
        // "ex is PostgresException pg" teste le type ET récupère l'objet dans "pg".
        if (ex is PostgresException pg)
        {
            // Chaque erreur SQL a un code standard (SqlState). On le traduit.
            return pg.SqlState switch
            {
                "23505" => "Cette donnée existe déjà (doublon refusé par la base).",          // unique_violation
                "23503" => "Opération impossible : cette donnée est encore référencée ailleurs " // foreign_key_violation
                         + "(par exemple une personne liée à une entrée, une adoption ou un accueil).",
                "23514" => "Valeur refusée : elle ne respecte pas une contrainte de vérification " // check_violation
                         + "(type, format d'identifiant, statut, ...).",
                "23502" => "Un champ obligatoire est manquant.",                               // not_null_violation
                "22001" => "Une valeur saisie est trop longue pour le champ concerné.",        // string_data_right_truncation
                "28P01" => "Mot de passe PostgreSQL incorrect — vérifiez coucheAccesBD/ConnexionBD.cs.",
                "3D000" => "La base de données 'refuge_animaux' est introuvable.",
                _       => "Erreur base de données : " + pg.MessageText // tout autre code : message brut
            };
        }

        // CAS 2 : problème de connexion (serveur arrêté, mauvais hôte/port...).
        if (ex is NpgsqlException)
        {
            return "Impossible de joindre la base de données. "
                 + "Vérifiez que PostgreSQL est démarré et que la connexion est correcte.";
        }

        // CAS 3 : toute autre erreur applicative -> on renvoie son message tel quel.
        return ex.Message;
    }
}
