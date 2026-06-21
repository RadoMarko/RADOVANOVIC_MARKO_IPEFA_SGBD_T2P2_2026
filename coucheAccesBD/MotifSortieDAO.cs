// ============================================================================
//  MotifSortieDAO.cs  —  COUCHE D'ACCÈS AUX DONNÉES (procédures stockées)
// ============================================================================

using Npgsql;
using RefugeAnimaux.classesMetier;

namespace RefugeAnimaux.coucheAccesBD;

public class MotifSortieDAO
{
    public List<MotifSortie> ListerTous()
    {
        const string sql = "SELECT * FROM listerMotifsSortie()";
        var liste = new List<MotifSortie>();
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
            liste.Add(new MotifSortie { IdMotifSortie = rd.GetInt32(0), Libelle = rd.GetString(1) });
        return liste;
    }
}
