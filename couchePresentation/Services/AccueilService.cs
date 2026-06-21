using RefugeAnimaux.classesMetier;
using RefugeAnimaux.coucheAccesBD;

namespace RefugeAnimaux.couchePresentation.Services;

public class AccueilService
{
    private readonly AnimalDAO _animalDAO;
    private readonly FamilleAccueilDAO _familleAccueilDAO;

    public AccueilService(AnimalDAO animalDAO, FamilleAccueilDAO familleAccueilDAO)
    {
        _animalDAO = animalDAO;
        _familleAccueilDAO = familleAccueilDAO;
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

        return OperationResult<int>.Ok($"Accueil créé : id {id}.", id);
    }
}
