// ============================================================================
//  Adoption.cs - COUCHE METIER
//  Represente une demande d'adoption pour un animal et un candidat.
// ============================================================================

namespace RefugeAnimaux.classesMetier;

public class Adoption
{
    public int IdAdoption { get; set; }
    public StatutAdoption Statut { get; set; }
    public DateTime DateDemande { get; set; } = DateTime.Today;
    public string IdAnimal { get; set; } = string.Empty;
    public int IdContact { get; set; }

    public string? NomAnimal { get; set; }
    public string? NomContact { get; set; }
    public string? PrenomContact { get; set; }

    public bool EstAcceptee => Statut == StatutAdoption.Acceptee;
    public string StatutTexte => Statut.ToDb();
    public string ContactTexte => (PrenomContact != null || NomContact != null)
        ? $"{PrenomContact} {NomContact}".Trim()
        : $"contact #{IdContact}";

    public static Adoption CreerDemande(string idAnimal, int idContact)
    {
        return new Adoption
        {
            IdAnimal = idAnimal,
            IdContact = idContact,
            DateDemande = DateTime.Today,
            Statut = StatutAdoption.Demande
        };
    }

    public Sortie CreerSortieAdoption(MotifSortie motifAdoption)
    {
        if (motifAdoption.Libelle != "adoption")
            throw new InvalidOperationException("Le motif fourni n'est pas le motif de sortie 'adoption'.");

        return new Sortie
        {
            DateSortie = DateTime.Today,
            IdAnimal = IdAnimal,
            IdMotifSortie = motifAdoption.IdMotifSortie,
            IdContact = IdContact
        };
    }

    public override string ToString()
    {
        var contact = (PrenomContact != null || NomContact != null)
            ? $"{PrenomContact} {NomContact}".Trim()
            : $"contact #{IdContact}";

        return $"[{IdAdoption}] {IdAnimal}" +
               (NomAnimal != null ? $" ({NomAnimal})" : "") +
               $" | demande : {DateDemande:dd/MM/yyyy}" +
               $" | candidat : {contact} | statut : {Statut.ToDb()}";
    }
}
