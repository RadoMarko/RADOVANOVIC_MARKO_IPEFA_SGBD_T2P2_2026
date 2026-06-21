using RefugeAnimaux.classesMetier;
using RefugeAnimaux.coucheAccesBD;

namespace RefugeAnimaux.couchePresentation.Services;

public class VaccinationService
{
    private readonly VaccinationDAO _vaccinationDAO;

    public VaccinationService(VaccinationDAO vaccinationDAO)
    {
        _vaccinationDAO = vaccinationDAO;
    }

    public OperationResult<int> AjouterOuMarquerFaite(
        Animal? animal,
        Vaccin? vaccin,
        DateTime? dateVaccination,
        bool vaccinationFaite)
    {
        if (animal == null || vaccin == null)
            return OperationResult<int>.Erreur("Choisissez un animal et un vaccin.");

        DateTime date = dateVaccination ?? DateTime.Today;
        if (animal.DateNaissance.HasValue && date.Date < animal.DateNaissance.Value.Date)
            return OperationResult<int>.Erreur("La date du vaccin est antérieure à la date de naissance de l'animal.");

        var vaccinationExistante = _vaccinationDAO.Consulter(animal.IdAnimal, vaccin.IdVaccin, date);
        if (vaccinationExistante != null)
        {
            if (!vaccinationExistante.Fait && vaccinationFaite)
            {
                _vaccinationDAO.ModifierEtat(vaccinationExistante.IdVaccination, true);
                return OperationResult<int>.Ok("Vaccination prévue passée à l'état effectué.", vaccinationExistante.IdVaccination);
            }

            return OperationResult<int>.Erreur(
                $"{animal.Nom} a déjà une vaccination {vaccin.NomVaccin} le {date:dd/MM/yyyy}. Un animal ne peut pas avoir deux fois le même vaccin le même jour.");
        }

        int id = _vaccinationDAO.Ajouter(new Vaccination
        {
            IdAnimal = animal.IdAnimal,
            IdVaccin = vaccin.IdVaccin,
            DateVaccin = date,
            Fait = vaccinationFaite
        });

        return OperationResult<int>.Ok($"Vaccination enregistrée : id {id}.", id);
    }
}
