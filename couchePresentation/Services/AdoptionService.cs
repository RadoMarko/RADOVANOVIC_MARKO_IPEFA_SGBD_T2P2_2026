using RefugeAnimaux.classesMetier;
using RefugeAnimaux.coucheAccesBD;

namespace RefugeAnimaux.couchePresentation.Services;

public class AdoptionService
{
    private readonly AdoptionDAO _adoptionDAO;
    private readonly AnimalDAO _animalDAO;
    private readonly MotifEntreeDAO _motifEntreeDAO;
    private readonly MotifSortieDAO _motifSortieDAO;
    private readonly SortieDAO _sortieDAO;

    public AdoptionService(
        AdoptionDAO adoptionDAO,
        AnimalDAO animalDAO,
        MotifEntreeDAO motifEntreeDAO,
        MotifSortieDAO motifSortieDAO,
        SortieDAO sortieDAO)
    {
        _adoptionDAO = adoptionDAO;
        _animalDAO = animalDAO;
        _motifEntreeDAO = motifEntreeDAO;
        _motifSortieDAO = motifSortieDAO;
        _sortieDAO = sortieDAO;
    }

    public OperationResult<int> CreerDemande(Animal? animal, PersonneContact? contact, DateTime? dateDemande)
    {
        if (animal == null || contact == null)
            return OperationResult<int>.Erreur("Choisissez un animal et un contact.");

        if (animal.EstDecede)
            return OperationResult<int>.Erreur("Cet animal est décédé : aucune adoption n'est possible.");

        if (_adoptionDAO.ADejaUneAdoptionAcceptee(animal.IdAnimal))
            return OperationResult<int>.Erreur("Cet animal a déjà une adoption acceptée.");

        if (_adoptionDAO.ADejaUneDemande(animal.IdAnimal, contact.IdContact))
            return OperationResult<int>.Erreur("Cette personne a déjà une demande d'adoption pour cet animal.");

        var adoption = Adoption.CreerDemande(animal.IdAnimal, contact.IdContact);
        adoption.DateDemande = dateDemande ?? DateTime.Today;

        int id = _adoptionDAO.Ajouter(adoption);
        return OperationResult<int>.Ok($"Demande d'adoption créée avec l'id {id}.", id);
    }

    public OperationResult<bool> ModifierStatut(
        Adoption? adoption,
        StatutAdoption nouveauStatut,
        DateTime? dateChangementStatut)
    {
        if (adoption == null)
            return OperationResult<bool>.Erreur("Choisissez une demande d'adoption.");

        DateTime dateChangement = dateChangementStatut ?? DateTime.Today;
        StatutAdoption ancienStatut = adoption.Statut;

        bool modifie = _adoptionDAO.ModifierStatut(adoption.IdAdoption, nouveauStatut);
        if (!modifie)
            return OperationResult<bool>.Erreur("Statut non modifié.");

        adoption.Statut = nouveauStatut;

        if (nouveauStatut == StatutAdoption.Acceptee)
        {
            var sortie = CreerSortieSiAnimalEncoreAuRefuge(adoption, dateChangement);
            if (!sortie.Succes)
                return sortie;
        }

        if (ancienStatut == StatutAdoption.Acceptee &&
            (nouveauStatut == StatutAdoption.RejetEnvironnement ||
             nouveauStatut == StatutAdoption.RejetComportement))
        {
            var retour = FaireRevenirAnimal(adoption, dateChangement);
            if (!retour.Succes)
                return retour;
        }

        return OperationResult<bool>.Ok("Statut d'adoption modifié.", true);
    }

    private OperationResult<bool> CreerSortieSiAnimalEncoreAuRefuge(Adoption adoption, DateTime dateSortie)
    {
        bool animalEncoreAuRefuge = _animalDAO.ListerAuRefuge()
            .Any(a => a.IdAnimal.Trim() == adoption.IdAnimal.Trim());

        if (!animalEncoreAuRefuge)
            return OperationResult<bool>.Ok("Aucune sortie à créer.", true);

        var motifAdoption = _motifSortieDAO.ListerTous().FirstOrDefault(m => m.Libelle == "adoption");
        if (motifAdoption == null)
            return OperationResult<bool>.Erreur("Statut modifié, mais le motif de sortie 'adoption' est introuvable.");

        _sortieDAO.Ajouter(adoption.CreerSortieAdoption(motifAdoption, dateSortie));
        return OperationResult<bool>.Ok("Sortie créée.", true);
    }

    private OperationResult<bool> FaireRevenirAnimal(Adoption adoption, DateTime dateEntree)
    {
        var motifRetour = _motifEntreeDAO.ListerTous().FirstOrDefault(m => m.Libelle == "retour_adoption");
        if (motifRetour == null)
            return OperationResult<bool>.Erreur("Statut modifié, mais le motif d'entrée 'retour_adoption' est introuvable.");

        _animalDAO.ClonerPourRetour(adoption.IdAnimal, dateEntree, motifRetour.IdMotifEntree, adoption.IdContact);
        return OperationResult<bool>.Ok("Nouvel animal créé pour le retour au refuge.", true);
    }
}
