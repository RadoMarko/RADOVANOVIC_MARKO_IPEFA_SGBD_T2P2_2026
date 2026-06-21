// ============================================================================
//  AdoptionDAO.cs  —  COUCHE D'ACCÈS AUX DONNÉES (procédures stockées)
// ============================================================================

using Npgsql;
using RefugeAnimaux.classesMetier;

namespace RefugeAnimaux.coucheAccesBD;

public class AdoptionDAO
{
    public int Ajouter(Adoption a)
    {
        const string sql = "SELECT * FROM ajouterAdoption(@p_statut, @p_date, @p_animal, @p_contact)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterVarchar(cmd, "p_statut", a.Statut.ToDb());
        ParametresBD.AjouterDate(cmd, "p_date", a.DateDemande);
        ParametresBD.AjouterText(cmd, "p_animal", a.IdAnimal);
        cmd.Parameters.AddWithValue("p_contact", a.IdContact);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public bool ADejaUneAdoptionAcceptee(string idAnimal)
    {
        const string sql = "SELECT * FROM aDejaUneAdoptionAcceptee(@p_id)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_id", idAnimal);
        return Convert.ToBoolean(cmd.ExecuteScalar());
    }

    public bool ADejaUneDemande(string idAnimal, int idContact)
    {
        const string sql = "SELECT * FROM aDejaUneDemande(@p_animal, @p_contact)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_animal", idAnimal);
        cmd.Parameters.AddWithValue("p_contact", idContact);
        return Convert.ToBoolean(cmd.ExecuteScalar());
    }

    public bool ModifierStatut(int idAdoption, StatutAdoption nouveauStatut)
    {
        const string sql = "SELECT * FROM modifierStatutAdoption(@p_id, @p_statut)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("p_id",     idAdoption);
        ParametresBD.AjouterVarchar(cmd, "p_statut", nouveauStatut.ToDb());
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public List<Adoption> ListerToutes()
    {
        const string sql = "SELECT * FROM listerAdoptions()";
        var liste = new List<Adoption>();
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            liste.Add(new Adoption
            {
                IdAdoption    = rd.GetInt32(0),
                Statut        = EnumsConversion.ToStatutAdoption(rd.GetString(1)),
                DateDemande   = rd.GetDateTime(2),
                IdAnimal      = rd.GetString(3),
                IdContact     = rd.GetInt32(4),
                NomAnimal     = rd.GetString(5),
                NomContact    = rd.GetString(6),
                PrenomContact = rd.GetString(7)
            });
        }
        return liste;
    }
}
