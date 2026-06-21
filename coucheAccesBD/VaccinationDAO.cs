// ============================================================================
//  VaccinationDAO.cs  —  COUCHE D'ACCÈS AUX DONNÉES (procédures stockées)
// ============================================================================

using Npgsql;
using RefugeAnimaux.classesMetier;

namespace RefugeAnimaux.coucheAccesBD;

public class VaccinationDAO
{
    public int Ajouter(Vaccination v)
    {
        const string sql = "SELECT * FROM ajouterVaccination(@p_date, @p_fait, @p_animal, @p_vaccin)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterDate(cmd, "p_date", v.DateVaccin);
        cmd.Parameters.AddWithValue("p_fait",   v.Fait);
        ParametresBD.AjouterText(cmd, "p_animal", v.IdAnimal);
        cmd.Parameters.AddWithValue("p_vaccin", v.IdVaccin);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public bool Existe(string idAnimal, int idVaccin, DateTime dateVaccin)
    {
        const string sql = "SELECT * FROM vaccinationExiste(@p_date, @p_animal, @p_vaccin)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterDate(cmd, "p_date", dateVaccin);
        ParametresBD.AjouterText(cmd, "p_animal", idAnimal);
        cmd.Parameters.AddWithValue("p_vaccin", idVaccin);
        return Convert.ToBoolean(cmd.ExecuteScalar());
    }

    public Vaccination? Consulter(string idAnimal, int idVaccin, DateTime dateVaccin)
    {
        const string sql = "SELECT * FROM consulterVaccination(@p_date, @p_animal, @p_vaccin)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterDate(cmd, "p_date", dateVaccin);
        ParametresBD.AjouterText(cmd, "p_animal", idAnimal);
        cmd.Parameters.AddWithValue("p_vaccin", idVaccin);

        using var rd = cmd.ExecuteReader();
        if (!rd.Read())
            return null;

        return new Vaccination
        {
            IdVaccination = rd.GetInt32(0),
            Fait = rd.GetBoolean(1),
            DateVaccin = dateVaccin,
            IdAnimal = idAnimal,
            IdVaccin = idVaccin
        };
    }

    public bool ModifierEtat(int idVaccination, bool fait)
    {
        const string sql = "SELECT * FROM modifierEtatVaccination(@p_id, @p_fait)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("p_id", idVaccination);
        cmd.Parameters.AddWithValue("p_fait", fait);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public List<Vaccination> ListerParAnimal(string idAnimal)
    {
        const string sql = "SELECT * FROM listerVaccinationsParAnimal(@p_id)";
        var liste = new List<Vaccination>();
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_id", idAnimal);
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            liste.Add(new Vaccination
            {
                IdVaccination = rd.GetInt32(0),
                DateVaccin    = rd.GetDateTime(1),
                Fait          = rd.GetBoolean(2),
                IdAnimal      = rd.GetString(3),
                IdVaccin      = rd.GetInt32(4),
                NomVaccin     = rd.GetString(5)
            });
        }
        return liste;
    }
}
