// ============================================================================
//  AnimalDAO.cs - COUCHE D'ACCES AUX DONNEES
//  Appelle les fonctions PL/pgSQL definies dans
//  creerprocedures_RadovanovicMarko.sql.
// ============================================================================

using Npgsql;
using NpgsqlTypes;
using RefugeAnimaux.classesMetier;

namespace RefugeAnimaux.coucheAccesBD;

public class AnimalDAO
{
    public string GenererIdAnimal(DateTime dateEntree)
    {
        const string sql = "SELECT * FROM genererIdAnimal(@p_date)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterDate(cmd, "p_date", dateEntree);
        return (string)cmd.ExecuteScalar()!;
    }

    public void Ajouter(Animal animal, DateTime dateEntree, int idMotifEntree, int? idContact)
    {
        const string sql = @"
            SELECT * FROM ajouterAnimal(
                @p_idAnimal, @p_nom, @p_type, @p_sexe,
                @p_sterilise, @p_dateSterilisation,
                @p_dateNaissance, @p_dateDeces,
                @p_description, @p_particularites,
                @p_couleurs,
                @p_dateEntree, @p_idMotifEntree, @p_idContact
            )";

        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_idAnimal", animal.IdAnimal);
        ParametresBD.AjouterVarchar(cmd, "p_nom", animal.Nom);
        ParametresBD.AjouterVarchar(cmd, "p_type", animal.Type.ToDb());
        ParametresBD.AjouterText(cmd, "p_sexe", animal.Sexe.ToDb());
        cmd.Parameters.AddWithValue("p_sterilise", animal.Sterilise);
        ParametresBD.AjouterDateNullable(cmd, "p_dateSterilisation", animal.DateSterilisation);
        ParametresBD.AjouterDateNullable(cmd, "p_dateNaissance", animal.DateNaissance);
        ParametresBD.AjouterDateNullable(cmd, "p_dateDeces", animal.DateDeces);
        ParametresBD.AjouterText(cmd, "p_description", animal.Description);
        ParametresBD.AjouterText(cmd, "p_particularites", animal.Particularites);

        cmd.Parameters.Add(new NpgsqlParameter("p_couleurs", NpgsqlDbType.Array | NpgsqlDbType.Text)
        {
            Value = animal.Couleurs.ToArray()
        });

        ParametresBD.AjouterDate(cmd, "p_dateEntree", dateEntree);
        cmd.Parameters.AddWithValue("p_idMotifEntree", idMotifEntree);
        cmd.Parameters.AddWithValue("p_idContact", (object?)idContact ?? DBNull.Value);

        cmd.ExecuteNonQuery();
    }

    public Animal? Consulter(string idAnimal)
    {
        const string sql = "SELECT * FROM consulterAnimal(@p_idAnimal)";

        using var conn = ConnexionBD.Ouvrir();
        Animal? animal = null;
        using (var cmd = new NpgsqlCommand(sql, conn))
        {
            ParametresBD.AjouterText(cmd, "p_idAnimal", idAnimal);
            using var rd = cmd.ExecuteReader();
            if (rd.Read()) animal = LireAnimal(rd);
        }

        if (animal != null)
            animal.Couleurs = ListerCouleurs(idAnimal, conn);
        return animal;
    }

    public bool Supprimer(string idAnimal)
    {
        const string sql = "SELECT * FROM supprimerAnimal(@p_idAnimal)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_idAnimal", idAnimal);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public List<Animal> ListerTous()
    {
        return ExecuterListe("SELECT * FROM listerAnimaux()");
    }

    public List<Animal> ListerAuRefuge()
    {
        return ExecuterListe("SELECT * FROM listerAnimauxAuRefuge()");
    }

    private List<Animal> ExecuterListe(string sql)
    {
        var liste = new List<Animal>();
        using var conn = ConnexionBD.Ouvrir();
        using (var cmd = new NpgsqlCommand(sql, conn))
        using (var rd = cmd.ExecuteReader())
        {
            while (rd.Read()) liste.Add(LireAnimal(rd));
        }

        foreach (var animal in liste)
            animal.Couleurs = ListerCouleurs(animal.IdAnimal, conn);

        return liste;
    }

    public bool ModifierDescription(string idAnimal, string? description, string? particularites)
    {
        const string sql = "SELECT * FROM modifierDescriptionParticularites(@p_id, @p_desc, @p_part)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_id", idAnimal);
        ParametresBD.AjouterText(cmd, "p_desc", description);
        ParametresBD.AjouterText(cmd, "p_part", particularites);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public bool EffacerDescription(string idAnimal)
    {
        const string sql = "SELECT * FROM effacerDescription(@p_id)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_id", idAnimal);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public bool EffacerParticularites(string idAnimal)
    {
        const string sql = "SELECT * FROM effacerParticularites(@p_id)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_id", idAnimal);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public void AjouterCouleur(string idAnimal, string couleur)
    {
        const string sql = "SELECT * FROM ajouterCouleur(@p_id, @p_couleur)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_id", idAnimal);
        ParametresBD.AjouterVarchar(cmd, "p_couleur", couleur);
        cmd.ExecuteNonQuery();
    }

    public bool SupprimerCouleur(string idAnimal, string couleur)
    {
        const string sql = "SELECT * FROM supprimerCouleur(@p_id, @p_couleur)";
        using var conn = ConnexionBD.Ouvrir();
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_id", idAnimal);
        ParametresBD.AjouterVarchar(cmd, "p_couleur", couleur);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    private static Animal LireAnimal(NpgsqlDataReader rd)
    {
        return new Animal
        {
            IdAnimal          = rd.GetString(0),
            Nom               = rd.GetString(1),
            Type              = EnumsConversion.ToTypeAnimal(rd.GetString(2)),
            Sexe              = EnumsConversion.ToSexe(rd.GetString(3)),
            Sterilise         = rd.GetBoolean(4),
            DateSterilisation = rd.IsDBNull(5) ? null : rd.GetDateTime(5),
            DateNaissance     = rd.IsDBNull(6) ? null : rd.GetDateTime(6),
            DateDeces         = rd.IsDBNull(7) ? null : rd.GetDateTime(7),
            Description       = rd.IsDBNull(8) ? null : rd.GetString(8),
            Particularites    = rd.IsDBNull(9) ? null : rd.GetString(9)
        };
    }

    private static List<string> ListerCouleurs(string idAnimal, NpgsqlConnection conn)
    {
        var couleurs = new List<string>();
        const string sql = "SELECT * FROM listerCouleurs(@p_id)";
        using var cmd = new NpgsqlCommand(sql, conn);
        ParametresBD.AjouterText(cmd, "p_id", idAnimal);
        using var rd = cmd.ExecuteReader();
        while (rd.Read()) couleurs.Add(rd.GetString(0));
        return couleurs;
    }
}
