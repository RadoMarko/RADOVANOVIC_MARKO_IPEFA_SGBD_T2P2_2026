using RefugeAnimaux.classesMetier;
using RefugeAnimaux.coucheAccesBD;

namespace RefugeAnimaux.couchePresentation.Services;

public class AccueilService
{
    private readonly AnimalDAO _animalDAO;
    private readonly FamilleAccueilDAO _familleAccueilDAO;
    private readonly MotifEntreeDAO _motifEntreeDAO;
    private readonly MotifSortieDAO _motifSortieDAO;
    private readonly SortieDAO _sortieDAO;

    public AccueilService(
        AnimalDAO animalDAO,
        FamilleAccueilDAO familleAccueilDAO,
        MotifEntreeDAO motifEntreeDAO,
        MotifSortieDAO motifSortieDAO,
        SortieDAO sortieDAO)
    {
        _animalDAO = animalDAO;
        _familleAccueilDAO = familleAccueilDAO;
        _motifEntreeDAO = motifEntreeDAO;
        _motifSortieDAO = motifSortieDAO;
        _sortieDAO = sortieDAO;
    }

    public OperationResult<int> Ajouter(
        Animal? animalSelectionne,
        PersonneContact? familleSelectionnee,
        DateTime? dateDebut,
        DateTime? dateFin)
    {
        if (animalSelectionne == null || familleSelectionnee == null)
            return OperationResult<int>.Erreur("Choisissez un animal et une famille d'accueil.");

        var animal = _animalDAO.Consulter(animalSelectionne.IdAnimal);
        if (animal == null)
            return OperationResult<int>.Erreur("Animal introuvable.");

        if (animal.EstDecede)
            return OperationResult<int>.Erreur("Cet animal est décédé : impossible de l'envoyer en famille d'accueil.");

        if (!dateDebut.HasValue)
            return OperationResult<int>.Erreur("La date de début est obligatoire.");

        if (dateFin.HasValue && dateFin.Value.Date < dateDebut.Value.Date)
            return OperationResult<int>.Erreur("La date de fin doit être supérieure ou égale à la date de début.");

        if (animal.DateNaissance.HasValue && dateDebut.Value.Date < animal.DateNaissance.Value.Date)
            return OperationResult<int>.Erreur("La date de début est antérieure à la date de naissance de l'animal.");

        if (_familleAccueilDAO.ADejaUnAccueilEnCours(animal.IdAnimal))
            return OperationResult<int>.Erreur("Cet animal a déjà un accueil en cours.");

        if (_familleAccueilDAO.AUnAccueilQuiChevauche(animal.IdAnimal, dateDebut.Value, dateFin))
            return OperationResult<int>.Erreur("Cet animal a déjà un accueil sur une période qui chevauche celle-ci.");

        int id = _familleAccueilDAO.Ajouter(new FamilleAccueil
        {
            IdAnimal = animal.IdAnimal,
            IdContact = familleSelectionnee.IdContact,
            DateDebut = dateDebut.Value,
            DateFin = dateFin
        });

        var motifSortie = _motifSortieDAO.ListerTous().FirstOrDefault(m => m.Libelle == "famille_accueil");
        if (motifSortie == null)
            return OperationResult<int>.Erreur("Accueil créé, mais le motif de sortie 'famille_accueil' est introuvable.");

        _sortieDAO.Ajouter(new Sortie
        {
            IdAnimal = animal.IdAnimal,
            DateSortie = dateDebut.Value,
            IdMotifSortie = motifSortie.IdMotifSortie,
            IdContact = familleSelectionnee.IdContact
        });

        return OperationResult<int>.Ok($"Accueil créé : id {id}.", id);
    }

    public OperationResult<bool> ModifierDateFin(FamilleAccueil? accueil, DateTime? dateFin)
    {
        if (accueil == null)
            return OperationResult<bool>.Erreur("Choisissez un accueil.");

        if (!dateFin.HasValue)
            return OperationResult<bool>.Erreur("La date de fin est obligatoire.");

        if (dateFin.Value.Date < accueil.DateDebut.Date)
            return OperationResult<bool>.Erreur("La date de fin doit être supérieure ou égale à la date de début.");

        bool modifie = _familleAccueilDAO.ModifierDateFin(accueil.IdAccueil, dateFin.Value);
        if (!modifie)
            return OperationResult<bool>.Erreur("Accueil introuvable.");

        var motifEntree = _motifEntreeDAO.ListerTous().FirstOrDefault(m => m.Libelle == "retour_famille_accueil");
        if (motifEntree == null)
            return OperationResult<bool>.Erreur("Date de fin modifiée, mais le motif d'entrée 'retour_famille_accueil' est introuvable.");

        _animalDAO.ClonerPourRetour(accueil.IdAnimal, dateFin.Value, motifEntree.IdMotifEntree, accueil.IdContact);

        return OperationResult<bool>.Ok("Accueil modifié et nouvel animal créé pour le retour au refuge.", true);
    }
}
