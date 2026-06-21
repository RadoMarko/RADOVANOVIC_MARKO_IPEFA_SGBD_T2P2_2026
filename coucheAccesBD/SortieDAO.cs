using Npgsql;
using RefugeAnimaux.classesMetier;

namespace RefugeAnimaux.coucheAccesBD;

public class SortieDAO
{
    public int Ajouter(Sortie s)
    {
        const string sql = "SELECT * FROM ajouterSortie(@p_date, @p_animal, @p_motif, @p_contact)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterDate(cmd, "p_date", s.DateSortie);
        ParametresBD.AjouterText(cmd, "p_animal", s.IdAnimal);
        cmd.Parameters.AddWithValue("p_motif", s.IdMotifSortie);
        cmd.Parameters.AddWithValue("p_contact", (object?)s.IdContact ?? DBNull.Value);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public bool SupprimerDerniereSortie(string idAnimal, string raison)
    {
        const string sql = "SELECT * FROM supprimerDerniereSortie(@p_animal, @p_raison)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_animal", idAnimal);
        ParametresBD.AjouterVarchar(cmd, "p_raison", raison);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }
}
