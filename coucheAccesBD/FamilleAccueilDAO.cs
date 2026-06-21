using Npgsql;
using RefugeAnimaux.classesMetier;

namespace RefugeAnimaux.coucheAccesBD;

public class FamilleAccueilDAO
{
    public int Ajouter(FamilleAccueil f)
    {
        const string sql = "SELECT * FROM ajouterAccueil(@p_dd, @p_df, @p_animal, @p_contact)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterDate(cmd, "p_dd", f.DateDebut);
        ParametresBD.AjouterDateNullable(cmd, "p_df", f.DateFin);
        ParametresBD.AjouterText(cmd, "p_animal", f.IdAnimal);
        cmd.Parameters.AddWithValue("p_contact", f.IdContact);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public bool ADejaUnAccueilEnCours(string idAnimal)
    {
        const string sql = "SELECT * FROM aDejaUnAccueilEnCours(@p_id)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_id", idAnimal);
        return Convert.ToBoolean(cmd.ExecuteScalar());
    }

    public bool AUnAccueilQuiChevauche(string idAnimal, DateTime debut, DateTime? fin)
    {
        const string sql = "SELECT * FROM aUnAccueilQuiChevauche(@p_id, @p_debut, @p_fin)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_id", idAnimal);
        ParametresBD.AjouterDate(cmd, "p_debut", debut);
        ParametresBD.AjouterDateNullable(cmd, "p_fin", fin);
        return Convert.ToBoolean(cmd.ExecuteScalar());
    }

    public bool ModifierDateFin(int idAccueil, DateTime dateFin)
    {
        const string sql = "SELECT * FROM modifierDateFinAccueil(@p_id, @p_dateFin)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("p_id", idAccueil);
        ParametresBD.AjouterDate(cmd, "p_dateFin", dateFin);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public List<string> ListerIdsAnimauxAvecAccueil()
    {
        const string sql = "SELECT * FROM listerIdsAnimauxAvecAccueil()";
        var liste = new List<string>();
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        using var rd = cmd.ExecuteReader();
        while (rd.Read()) liste.Add(rd.GetString(0));
        return liste;
    }

    public List<int> ListerIdsContactsAvecAccueil()
    {
        const string sql = "SELECT * FROM listerIdsContactsAvecAccueil()";
        var liste = new List<int>();
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        using var rd = cmd.ExecuteReader();
        while (rd.Read()) liste.Add(rd.GetInt32(0));
        return liste;
    }

    public List<FamilleAccueil> ListerParAnimal(string idAnimal)
    {
        const string sql = "SELECT * FROM listerAccueilsParAnimal(@p_id)";
        var liste = new List<FamilleAccueil>();
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_id", idAnimal);
        using var rd = cmd.ExecuteReader();
        while (rd.Read()) liste.Add(LireParAnimal(rd));
        return liste;
    }

    public List<FamilleAccueil> ListerParFamille(int idContact)
    {
        const string sql = "SELECT * FROM listerAccueilsParFamille(@p_id)";
        var liste = new List<FamilleAccueil>();
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("p_id", idContact);
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            liste.Add(new FamilleAccueil
            {
                IdAccueil = rd.GetInt32(0),
                DateDebut = rd.GetDateTime(1),
                DateFin = rd.IsDBNull(2) ? null : rd.GetDateTime(2),
                IdAnimal = rd.GetString(3),
                IdContact = rd.GetInt32(4),
                NomAnimal = rd.GetString(5)
            });
        }

        return liste;
    }

    private static FamilleAccueil LireParAnimal(NpgsqlDataReader rd) => new()
    {
        IdAccueil = rd.GetInt32(0),
        DateDebut = rd.GetDateTime(1),
        DateFin = rd.IsDBNull(2) ? null : rd.GetDateTime(2),
        IdAnimal = rd.GetString(3),
        IdContact = rd.GetInt32(4),
        NomContact = rd.GetString(5),
        PrenomContact = rd.GetString(6)
    };
}
