// ============================================================================
//  MotifEntreeDAO.cs  —  COUCHE D'ACCÈS AUX DONNÉES (procédures stockées)
// ============================================================================

using Npgsql;
using RefugeAnimaux.classesMetier;

namespace RefugeAnimaux.coucheAccesBD;

public class MotifEntreeDAO
{
    public List<MotifEntree> ListerTous()
    {
        const string sql = "SELECT * FROM listerMotifsEntree()";
        var liste = new List<MotifEntree>();
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
            liste.Add(new MotifEntree { IdMotifEntree = rd.GetInt32(0), Libelle = rd.GetString(1) });
        return liste;
    }
}
