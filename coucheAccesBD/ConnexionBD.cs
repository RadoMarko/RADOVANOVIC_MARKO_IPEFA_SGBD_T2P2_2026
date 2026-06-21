// ============================================================================
//  ConnexionBD.cs - COUCHE D'ACCES AUX DONNEES
// ============================================================================

using Npgsql;

namespace RefugeAnimaux.coucheAccesBD;

public static class ConnexionBD
{
    private static string ChaineConnexion =>
        Environment.GetEnvironmentVariable("REFUGE_ANIMAUX_CONNECTION")
        ?? "Host=localhost;Port=5432;Username=postgres;Password=root;Database=refuge_animaux";

    public static NpgsqlConnection Ouvrir()
    {
        var conn = new NpgsqlConnection(ChaineConnexion);
        conn.Open();
        return conn;
    }
}
