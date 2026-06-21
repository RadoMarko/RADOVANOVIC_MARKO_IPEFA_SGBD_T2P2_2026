// ============================================================================
//  ParametresBD.cs - helpers de parametres PostgreSQL
// ============================================================================

using Npgsql;
using NpgsqlTypes;

namespace RefugeAnimaux.coucheAccesBD;

internal static class ParametresBD
{
    public static void AjouterDate(NpgsqlCommand cmd, string nom, DateTime valeur)
    {
        cmd.Parameters.Add(new NpgsqlParameter(nom, NpgsqlDbType.Date)
        {
            Value = valeur.Date
        });
    }

    public static void AjouterDateNullable(NpgsqlCommand cmd, string nom, DateTime? valeur)
    {
        cmd.Parameters.Add(new NpgsqlParameter(nom, NpgsqlDbType.Date)
        {
            Value = valeur.HasValue ? valeur.Value.Date : DBNull.Value
        });
    }

    public static void AjouterVarchar(NpgsqlCommand cmd, string nom, string? valeur)
    {
        cmd.Parameters.Add(new NpgsqlParameter(nom, NpgsqlDbType.Varchar)
        {
            Value = (object?)valeur ?? DBNull.Value
        });
    }

    public static void AjouterText(NpgsqlCommand cmd, string nom, string? valeur)
    {
        cmd.Parameters.Add(new NpgsqlParameter(nom, NpgsqlDbType.Text)
        {
            Value = (object?)valeur ?? DBNull.Value
        });
    }
}
