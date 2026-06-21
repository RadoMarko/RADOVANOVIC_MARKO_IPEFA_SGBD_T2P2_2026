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
