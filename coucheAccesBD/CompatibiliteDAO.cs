using Npgsql;
using RefugeAnimaux.classesMetier;

namespace RefugeAnimaux.coucheAccesBD;

public class CompatibiliteDAO
{
    public int Ajouter(Compatibilite c)
    {
        const string sql = "SELECT * FROM ajouterCompatibilite(@p_type, @p_valeur, @p_desc, @p_id)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterVarchar(cmd, "p_type", c.Type.ToDb());
        ParametresBD.AjouterVarchar(cmd, "p_valeur", c.Valeur.ToDb());
        ParametresBD.AjouterText(cmd, "p_desc", c.Description);
        ParametresBD.AjouterText(cmd, "p_id", c.IdAnimal);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public bool Supprimer(int idCompat)
    {
        const string sql = "SELECT * FROM supprimerCompatibilite(@p_id)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("p_id", idCompat);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public List<Compatibilite> ListerParAnimal(string idAnimal)
    {
        const string sql = "SELECT * FROM listerCompatibilitesParAnimal(@p_id)";
        var liste = new List<Compatibilite>();
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_id", idAnimal);
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            liste.Add(new Compatibilite
            {
                IdCompat = rd.GetInt32(0),
                Type = EnumsConversion.ToTypeCompatibilite(rd.GetString(1)),
                Valeur = EnumsConversion.ToValeurCompatibilite(rd.GetString(2)),
                Description = rd.IsDBNull(3) ? null : rd.GetString(3),
                IdAnimal = rd.GetString(4)
            });
        }

        return liste;
    }
}
