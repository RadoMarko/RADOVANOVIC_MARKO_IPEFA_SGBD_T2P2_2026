// ============================================================================
//  PersonneContactDAO.cs - COUCHE D'ACCES AUX DONNEES
// ============================================================================

using Npgsql;
using NpgsqlTypes;
using RefugeAnimaux.classesMetier;

namespace RefugeAnimaux.coucheAccesBD;

public class PersonneContactDAO
{
    public int Ajouter(PersonneContact p)
    {
        const string sql = @"
            SELECT * FROM ajouterContact(
                @p_nom, @p_prenom, @p_rn, @p_rue, @p_cp, @p_localite,
                @p_gsm, @p_tel, @p_email, @p_roles
            )";

        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        AjouterParametresContact(cmd, p);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public PersonneContact? Consulter(int id)
    {
        const string sql = "SELECT * FROM consulterContact(@p_id)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("p_id", id);
        using var rd = cmd.ExecuteReader();
        return rd.Read() ? Lire(rd) : null;
    }

    public List<PersonneContact> ListerTous()
    {
        const string sql = "SELECT * FROM listerContacts()";
        var liste = new List<PersonneContact>();

        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        using var rd = cmd.ExecuteReader();
        while (rd.Read()) liste.Add(Lire(rd));
        return liste;
    }

    public bool RegistreNationalExiste(string registreNational, int? idContactAExclure = null)
    {
        const string sql = "SELECT * FROM registreNationalExiste(@p_rn, @p_id_exclu)";

        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterVarchar(cmd, "p_rn", registreNational);
        cmd.Parameters.Add(new NpgsqlParameter("p_id_exclu", NpgsqlDbType.Integer)
        {
            Value = (object?)idContactAExclure ?? DBNull.Value
        });
        return Convert.ToBoolean(cmd.ExecuteScalar());
    }

    public bool Modifier(PersonneContact p)
    {
        const string sql = @"
            SELECT * FROM modifierContact(
                @p_id, @p_nom, @p_prenom, @p_rn, @p_rue, @p_cp, @p_localite,
                @p_gsm, @p_tel, @p_email, @p_roles
            )";

        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("p_id", p.IdContact);
        AjouterParametresContact(cmd, p);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public bool Supprimer(int id)
    {
        const string sql = "SELECT * FROM supprimerContact(@p_id)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("p_id", id);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    private static void AjouterParametresContact(NpgsqlCommand cmd, PersonneContact p)
    {
        ParametresBD.AjouterVarchar(cmd, "p_nom", p.Nom);
        ParametresBD.AjouterVarchar(cmd, "p_prenom", p.Prenom);
        ParametresBD.AjouterVarchar(cmd, "p_rn", p.RegistreNational);
        ParametresBD.AjouterVarchar(cmd, "p_rue", p.Rue);
        ParametresBD.AjouterVarchar(cmd, "p_cp", p.Cp);
        ParametresBD.AjouterVarchar(cmd, "p_localite", p.Localite);
        ParametresBD.AjouterVarchar(cmd, "p_gsm", p.GSM);
        ParametresBD.AjouterVarchar(cmd, "p_tel", p.Telephone);
        ParametresBD.AjouterVarchar(cmd, "p_email", p.Email);

        var roles = p.Roles.Select(r => r.ToDb()).Distinct().ToArray();
        cmd.Parameters.Add(new NpgsqlParameter("p_roles", NpgsqlDbType.Array | NpgsqlDbType.Text)
        {
            Value = roles
        });
    }

    private static PersonneContact Lire(NpgsqlDataReader rd)
    {
        var roles = rd.GetFieldValue<string[]>(10)
            .Select(EnumsConversion.ToTypeContact)
            .ToList();

        return new PersonneContact
        {
            IdContact        = rd.GetInt32(0),
            Nom              = rd.GetString(1),
            Prenom           = rd.GetString(2),
            RegistreNational = rd.IsDBNull(3) ? null : rd.GetString(3),
            Rue              = rd.IsDBNull(4) ? null : rd.GetString(4),
            Cp               = rd.IsDBNull(5) ? null : rd.GetString(5),
            Localite         = rd.IsDBNull(6) ? null : rd.GetString(6),
            GSM              = rd.IsDBNull(7) ? null : rd.GetString(7),
            Telephone        = rd.IsDBNull(8) ? null : rd.GetString(8),
            Email            = rd.IsDBNull(9) ? null : rd.GetString(9),
            Roles            = roles
        };
    }
}
