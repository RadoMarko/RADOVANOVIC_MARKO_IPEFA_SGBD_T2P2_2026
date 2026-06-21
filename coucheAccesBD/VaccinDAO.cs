// ============================================================================
//  VaccinDAO.cs  —  COUCHE D'ACCÈS AUX DONNÉES (procédures stockées)
// ============================================================================

using Npgsql;
using RefugeAnimaux.classesMetier;

namespace RefugeAnimaux.coucheAccesBD;

public class VaccinDAO
{
    public int Ajouter(string nomVaccin)
    {
        const string sql = "SELECT * FROM ajouterVaccin(@p_nom)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterVarchar(cmd, "p_nom", nomVaccin);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public bool Supprimer(int idVaccin)
    {
        const string sql = "SELECT * FROM supprimerVaccin(@p_id)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("p_id", idVaccin);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public List<Vaccin> ListerTous()
    {
        const string sql = "SELECT * FROM listerVaccins()";
        var liste = new List<Vaccin>();
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
            liste.Add(new Vaccin { IdVaccin = rd.GetInt32(0), NomVaccin = rd.GetString(1) });
        return liste;
    }
}
