using RefugeAnimaux.classesMetier;
using RefugeAnimaux.coucheAccesBD;

namespace RefugeAnimaux.couchePresentation.Services;

public class ContactService
{
    private readonly PersonneContactDAO _contactDAO;

    public ContactService(PersonneContactDAO contactDAO)
    {
        _contactDAO = contactDAO;
    }

    public OperationResult<int> Enregistrer(PersonneContact contact, bool modification)
    {
        var validation = contact.VerifierReglesMetier();
        if (!validation.EstValide)
            return OperationResult<int>.Erreur(validation.PremierMessage);

        if (!string.IsNullOrWhiteSpace(contact.RegistreNational) &&
            _contactDAO.RegistreNationalExiste(contact.RegistreNational, modification ? contact.IdContact : null))
        {
            return OperationResult<int>.Erreur("Ce registre national est déjà utilisé par un autre contact.");
        }

        if (modification)
        {
            bool modifie = _contactDAO.Modifier(contact);
            return modifie
                ? OperationResult<int>.Ok("Contact modifié.", contact.IdContact)
                : OperationResult<int>.Erreur("Contact introuvable.");
        }

        int id = _contactDAO.Ajouter(contact);
        return OperationResult<int>.Ok($"Contact créé avec l'id {id}.", id);
    }
}
