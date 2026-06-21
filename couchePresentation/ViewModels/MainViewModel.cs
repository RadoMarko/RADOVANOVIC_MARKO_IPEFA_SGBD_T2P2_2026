using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Input;
using RefugeAnimaux.classesMetier;
using RefugeAnimaux.coucheAccesBD;
using RefugeAnimaux.couchePresentation.Services;

namespace RefugeAnimaux.couchePresentation.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly PersonneContactDAO _contactDAO = new();
    private readonly AnimalDAO _animalDAO = new();
    private readonly MotifEntreeDAO _motifEntreeDAO = new();
    private readonly MotifSortieDAO _motifSortieDAO = new();
    private readonly SortieDAO _sortieDAO = new();
    private readonly AdoptionDAO _adoptionDAO = new();
    private readonly CompatibiliteDAO _compatibiliteDAO = new();
    private readonly FamilleAccueilDAO _familleAccueilDAO = new();
    private readonly VaccinDAO _vaccinDAO = new();
    private readonly VaccinationDAO _vaccinationDAO = new();
    private readonly ContactService _contactService;
    private readonly AdoptionService _adoptionService;
    private readonly AccueilService _accueilService;

    private string _statusMessage = "Prêt.";
    private string _errorMessage = string.Empty;
    private string _contactErrorMessage = string.Empty;
    private string _animalErrorMessage = string.Empty;
    private string _animalGestionErrorMessage = string.Empty;
    private string _adoptionErrorMessage = string.Empty;
    private string _accueilErrorMessage = string.Empty;
    private string _vaccinErrorMessage = string.Empty;
    private string _vaccinationErrorMessage = string.Empty;
    private Brush _statusBrush = Brushes.ForestGreen;

    public ObservableCollection<PersonneContact> Contacts { get; } = new();
    public ObservableCollection<Animal> AnimauxPresents { get; } = new();
    public ObservableCollection<Animal> TousLesAnimaux { get; } = new();
    public ObservableCollection<MotifEntree> MotifsEntree { get; } = new();
    public ObservableCollection<MotifSortie> MotifsSortie { get; } = new();
    public ObservableCollection<Adoption> Adoptions { get; } = new();
    public ObservableCollection<string> CouleursAnimal { get; } = new();
    public ObservableCollection<Compatibilite> CompatibilitesAnimal { get; } = new();
    public ObservableCollection<FamilleAccueil> AccueilsParAnimal { get; } = new();
    public ObservableCollection<FamilleAccueil> AccueilsParFamille { get; } = new();
    public ObservableCollection<Vaccin> Vaccins { get; } = new();
    public ObservableCollection<Vaccination> VaccinationsAnimal { get; } = new();

    public IReadOnlyList<TypeAnimal> TypesAnimal { get; } = Enum.GetValues<TypeAnimal>();
    public IReadOnlyList<Sexe> Sexes { get; } = Enum.GetValues<Sexe>();
    public IReadOnlyList<StatutAdoption> StatutsAdoption { get; } = Enum.GetValues<StatutAdoption>();
    public IReadOnlyList<TypeCompatibilite> TypesCompatibilite { get; } = Enum.GetValues<TypeCompatibilite>();
    public IReadOnlyList<ValeurCompatibilite> ValeursCompatibilite { get; } = Enum.GetValues<ValeurCompatibilite>();

    public ICommand RafraichirCommand { get; }
    public ICommand AjouterContactCommand { get; }
    public ICommand ModifierContactCommand { get; }
    public ICommand SupprimerContactCommand { get; }
    public ICommand AjouterAnimalCommand { get; }
    public ICommand ChargerDetailsAnimalCommand { get; }
    public ICommand ModifierInfosAnimalCommand { get; }
    public ICommand EffacerDescriptionCommand { get; }
    public ICommand EffacerParticularitesCommand { get; }
    public ICommand AjouterCouleurAnimalCommand { get; }
    public ICommand SupprimerCouleurAnimalCommand { get; }
    public ICommand AjouterCompatibiliteCommand { get; }
    public ICommand SupprimerCompatibiliteCommand { get; }
    public ICommand EnregistrerSortieCommand { get; }
    public ICommand SupprimerAnimalCommand { get; }
    public ICommand AjouterAdoptionCommand { get; }
    public ICommand ModifierStatutAdoptionCommand { get; }
    public ICommand AjouterAccueilCommand { get; }
    public ICommand ChargerAccueilsCommand { get; }
    public ICommand AjouterVaccinCommand { get; }
    public ICommand SupprimerVaccinCommand { get; }
    public ICommand AjouterVaccinationCommand { get; }
    public ICommand ChargerVaccinationsCommand { get; }

    public MainViewModel()
    {
        _contactService = new ContactService(_contactDAO);
        _adoptionService = new AdoptionService(_adoptionDAO, _animalDAO, _motifSortieDAO, _sortieDAO);
        _accueilService = new AccueilService(_animalDAO, _familleAccueilDAO);

        RafraichirCommand = new RelayCommand(_ => RafraichirTout());
        AjouterContactCommand = new RelayCommand(_ => AjouterContact());
        ModifierContactCommand = new RelayCommand(_ => ModifierContact(), _ => ContactSelectionne != null);
        SupprimerContactCommand = new RelayCommand(_ => SupprimerContact(), _ => ContactSelectionne != null);
        AjouterAnimalCommand = new RelayCommand(_ => AjouterAnimal());
        ChargerDetailsAnimalCommand = new RelayCommand(_ => ChargerDetailsAnimal(), _ => AnimalGestionSelectionne != null);
        ModifierInfosAnimalCommand = new RelayCommand(_ => ModifierInfosAnimal(), _ => AnimalGestionSelectionne != null);
        EffacerDescriptionCommand = new RelayCommand(_ => EffacerDescription(), _ => AnimalGestionSelectionne != null);
        EffacerParticularitesCommand = new RelayCommand(_ => EffacerParticularites(), _ => AnimalGestionSelectionne != null);
        AjouterCouleurAnimalCommand = new RelayCommand(_ => AjouterCouleurAnimal(), _ => AnimalGestionSelectionne != null);
        SupprimerCouleurAnimalCommand = new RelayCommand(_ => SupprimerCouleurAnimal(), _ => AnimalGestionSelectionne != null);
        AjouterCompatibiliteCommand = new RelayCommand(_ => AjouterCompatibilite(), _ => AnimalGestionSelectionne != null);
        SupprimerCompatibiliteCommand = new RelayCommand(_ => SupprimerCompatibilite(), _ => CompatibiliteSelectionnee != null);
        EnregistrerSortieCommand = new RelayCommand(_ => EnregistrerSortie(), _ => AnimalSortieSelectionne != null);
        SupprimerAnimalCommand = new RelayCommand(_ => SupprimerAnimal(), _ => AnimalGestionSelectionne != null);
        AjouterAdoptionCommand = new RelayCommand(_ => AjouterAdoption());
        ModifierStatutAdoptionCommand = new RelayCommand(_ => ModifierStatutAdoption(), _ => AdoptionSelectionnee != null);
        AjouterAccueilCommand = new RelayCommand(_ => AjouterAccueil(), _ => AnimalAccueilSelectionne != null && ContactAccueilSelectionne != null);
        ChargerAccueilsCommand = new RelayCommand(_ => ChargerAccueils());
        AjouterVaccinCommand = new RelayCommand(_ => AjouterVaccin());
        SupprimerVaccinCommand = new RelayCommand(_ => SupprimerVaccin(), _ => VaccinSelectionne != null);
        AjouterVaccinationCommand = new RelayCommand(_ => AjouterVaccination(), _ => AnimalVaccinationSelectionne != null && VaccinVaccinationSelectionne != null);
        ChargerVaccinationsCommand = new RelayCommand(_ => ChargerVaccinationsAnimal(), _ => AnimalVaccinationSelectionne != null);

        DateEntreeAnimal = DateTime.Today;
        DateDemandeAdoption = DateTime.Today;
        DateSortieAnimal = DateTime.Today;
        DateDebutAccueil = DateTime.Today;
        DateVaccination = DateTime.Today;
        TypeAnimalSelectionne = TypeAnimal.Chat;
        SexeSelectionne = Sexe.M;
        TypeCompatibiliteSelectionne = TypeCompatibilite.Chat;
        ValeurCompatibiliteSelectionnee = ValeurCompatibilite.Oui;
        NouveauStatutAdoption = StatutAdoption.Demande;

        RafraichirTout();
    }

    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public string ContactErrorMessage { get => _contactErrorMessage; set => SetProperty(ref _contactErrorMessage, value); }
    public string AnimalErrorMessage { get => _animalErrorMessage; set => SetProperty(ref _animalErrorMessage, value); }
    public string AnimalGestionErrorMessage { get => _animalGestionErrorMessage; set => SetProperty(ref _animalGestionErrorMessage, value); }
    public string AdoptionErrorMessage { get => _adoptionErrorMessage; set => SetProperty(ref _adoptionErrorMessage, value); }
    public string AccueilErrorMessage { get => _accueilErrorMessage; set => SetProperty(ref _accueilErrorMessage, value); }
    public string VaccinErrorMessage { get => _vaccinErrorMessage; set => SetProperty(ref _vaccinErrorMessage, value); }
    public string VaccinationErrorMessage { get => _vaccinationErrorMessage; set => SetProperty(ref _vaccinationErrorMessage, value); }
    public Brush StatusBrush { get => _statusBrush; set => SetProperty(ref _statusBrush, value); }

    private string _contactNom = string.Empty;
    private string _contactPrenom = string.Empty;
    private string _contactRegistreNational = string.Empty;
    private string _contactRue = string.Empty;
    private string _contactCp = string.Empty;
    private string _contactLocalite = string.Empty;
    private string _contactGsm = string.Empty;
    private string _contactTelephone = string.Empty;
    private string _contactEmail = string.Empty;
    private bool _roleBenevole;
    private bool _roleAdoptant;
    private bool _roleCandidat = true;
    private bool _roleFamilleAccueil;
    private PersonneContact? _contactSelectionne;

    public string ContactNom { get => _contactNom; set => SetProperty(ref _contactNom, value); }
    public string ContactPrenom { get => _contactPrenom; set => SetProperty(ref _contactPrenom, value); }
    public string ContactRegistreNational { get => _contactRegistreNational; set => SetProperty(ref _contactRegistreNational, value); }
    public string ContactRue { get => _contactRue; set => SetProperty(ref _contactRue, value); }
    public string ContactCp { get => _contactCp; set => SetProperty(ref _contactCp, value); }
    public string ContactLocalite { get => _contactLocalite; set => SetProperty(ref _contactLocalite, value); }
    public string ContactGsm { get => _contactGsm; set => SetProperty(ref _contactGsm, value); }
    public string ContactTelephone { get => _contactTelephone; set => SetProperty(ref _contactTelephone, value); }
    public string ContactEmail { get => _contactEmail; set => SetProperty(ref _contactEmail, value); }
    public bool RoleBenevole { get => _roleBenevole; set => SetProperty(ref _roleBenevole, value); }
    public bool RoleAdoptant { get => _roleAdoptant; set => SetProperty(ref _roleAdoptant, value); }
    public bool RoleCandidat { get => _roleCandidat; set => SetProperty(ref _roleCandidat, value); }
    public bool RoleFamilleAccueil { get => _roleFamilleAccueil; set => SetProperty(ref _roleFamilleAccueil, value); }

    public PersonneContact? ContactSelectionne
    {
        get => _contactSelectionne;
        set
        {
            if (!SetProperty(ref _contactSelectionne, value) || value == null) return;
            ContactNom = value.Nom;
            ContactPrenom = value.Prenom;
            ContactRegistreNational = value.RegistreNational ?? string.Empty;
            ContactRue = value.Rue ?? string.Empty;
            ContactCp = value.Cp ?? string.Empty;
            ContactLocalite = value.Localite ?? string.Empty;
            ContactGsm = value.GSM ?? string.Empty;
            ContactTelephone = value.Telephone ?? string.Empty;
            ContactEmail = value.Email ?? string.Empty;
            RoleBenevole = value.ALeRole(TypeContact.Benevole);
            RoleAdoptant = value.ALeRole(TypeContact.Adoptant);
            RoleCandidat = value.ALeRole(TypeContact.Candidat);
            RoleFamilleAccueil = value.ALeRole(TypeContact.FamilleAccueil);
        }
    }

    private string _animalNom = string.Empty;
    private DateTime? _dateEntreeAnimal;
    private TypeAnimal _typeAnimalSelectionne;
    private Sexe _sexeSelectionne;
    private DateTime? _dateNaissanceAnimal;
    private bool _animalSterilise;
    private DateTime? _dateSterilisationAnimal;
    private string _animalDescription = string.Empty;
    private string _animalParticularites = string.Empty;
    private string _animalCouleurs = string.Empty;
    private MotifEntree? _motifEntreeSelectionne;
    private PersonneContact? _contactEntreeSelectionne;

    public string AnimalNom { get => _animalNom; set => SetProperty(ref _animalNom, value); }
    public DateTime? DateEntreeAnimal { get => _dateEntreeAnimal; set => SetProperty(ref _dateEntreeAnimal, value); }
    public TypeAnimal TypeAnimalSelectionne { get => _typeAnimalSelectionne; set => SetProperty(ref _typeAnimalSelectionne, value); }
    public Sexe SexeSelectionne { get => _sexeSelectionne; set => SetProperty(ref _sexeSelectionne, value); }
    public DateTime? DateNaissanceAnimal { get => _dateNaissanceAnimal; set => SetProperty(ref _dateNaissanceAnimal, value); }
    public bool AnimalSterilise { get => _animalSterilise; set => SetProperty(ref _animalSterilise, value); }
    public DateTime? DateSterilisationAnimal { get => _dateSterilisationAnimal; set => SetProperty(ref _dateSterilisationAnimal, value); }
    public string AnimalDescription { get => _animalDescription; set => SetProperty(ref _animalDescription, value); }
    public string AnimalParticularites { get => _animalParticularites; set => SetProperty(ref _animalParticularites, value); }
    public string AnimalCouleurs { get => _animalCouleurs; set => SetProperty(ref _animalCouleurs, value); }
    public MotifEntree? MotifEntreeSelectionne { get => _motifEntreeSelectionne; set => SetProperty(ref _motifEntreeSelectionne, value); }
    public PersonneContact? ContactEntreeSelectionne { get => _contactEntreeSelectionne; set => SetProperty(ref _contactEntreeSelectionne, value); }

    private Animal? _animalGestionSelectionne;
    private string _nouvelleDescriptionAnimal = string.Empty;
    private string _nouvellesParticularitesAnimal = string.Empty;
    private string _nouvelleCouleurAnimal = string.Empty;
    private string _couleurAnimalSelectionnee = string.Empty;
    private TypeCompatibilite _typeCompatibiliteSelectionne;
    private ValeurCompatibilite _valeurCompatibiliteSelectionnee;
    private string _descriptionCompatibilite = string.Empty;
    private Compatibilite? _compatibiliteSelectionnee;
    private Animal? _animalSortieSelectionne;
    private DateTime? _dateSortieAnimal;
    private MotifSortie? _motifSortieSelectionne;
    private PersonneContact? _contactSortieSelectionne;

    public Animal? AnimalGestionSelectionne
    {
        get => _animalGestionSelectionne;
        set
        {
            if (!SetProperty(ref _animalGestionSelectionne, value)) return;
            ChargerDetailsAnimal();
        }
    }

    public string NouvelleDescriptionAnimal { get => _nouvelleDescriptionAnimal; set => SetProperty(ref _nouvelleDescriptionAnimal, value); }
    public string NouvellesParticularitesAnimal { get => _nouvellesParticularitesAnimal; set => SetProperty(ref _nouvellesParticularitesAnimal, value); }
    public string NouvelleCouleurAnimal { get => _nouvelleCouleurAnimal; set => SetProperty(ref _nouvelleCouleurAnimal, value); }
    public string CouleurAnimalSelectionnee { get => _couleurAnimalSelectionnee; set => SetProperty(ref _couleurAnimalSelectionnee, value); }
    public TypeCompatibilite TypeCompatibiliteSelectionne { get => _typeCompatibiliteSelectionne; set => SetProperty(ref _typeCompatibiliteSelectionne, value); }
    public ValeurCompatibilite ValeurCompatibiliteSelectionnee { get => _valeurCompatibiliteSelectionnee; set => SetProperty(ref _valeurCompatibiliteSelectionnee, value); }
    public string DescriptionCompatibilite { get => _descriptionCompatibilite; set => SetProperty(ref _descriptionCompatibilite, value); }
    public Compatibilite? CompatibiliteSelectionnee { get => _compatibiliteSelectionnee; set => SetProperty(ref _compatibiliteSelectionnee, value); }
    public Animal? AnimalSortieSelectionne { get => _animalSortieSelectionne; set => SetProperty(ref _animalSortieSelectionne, value); }
    public DateTime? DateSortieAnimal { get => _dateSortieAnimal; set => SetProperty(ref _dateSortieAnimal, value); }
    public MotifSortie? MotifSortieSelectionne { get => _motifSortieSelectionne; set => SetProperty(ref _motifSortieSelectionne, value); }
    public PersonneContact? ContactSortieSelectionne { get => _contactSortieSelectionne; set => SetProperty(ref _contactSortieSelectionne, value); }

    private Animal? _animalAdoptionSelectionne;
    private PersonneContact? _contactAdoptionSelectionne;
    private DateTime? _dateDemandeAdoption;
    private Adoption? _adoptionSelectionnee;
    private StatutAdoption _nouveauStatutAdoption;

    public Animal? AnimalAdoptionSelectionne { get => _animalAdoptionSelectionne; set => SetProperty(ref _animalAdoptionSelectionne, value); }
    public PersonneContact? ContactAdoptionSelectionne { get => _contactAdoptionSelectionne; set => SetProperty(ref _contactAdoptionSelectionne, value); }
    public DateTime? DateDemandeAdoption { get => _dateDemandeAdoption; set => SetProperty(ref _dateDemandeAdoption, value); }
    public Adoption? AdoptionSelectionnee { get => _adoptionSelectionnee; set => SetProperty(ref _adoptionSelectionnee, value); }
    public StatutAdoption NouveauStatutAdoption { get => _nouveauStatutAdoption; set => SetProperty(ref _nouveauStatutAdoption, value); }

    private Animal? _animalAccueilSelectionne;
    private PersonneContact? _contactAccueilSelectionne;
    private DateTime? _dateDebutAccueil;
    private DateTime? _dateFinAccueil;
    private Animal? _animalAccueilListeSelectionne;
    private PersonneContact? _contactAccueilListeSelectionne;

    public Animal? AnimalAccueilSelectionne { get => _animalAccueilSelectionne; set => SetProperty(ref _animalAccueilSelectionne, value); }
    public PersonneContact? ContactAccueilSelectionne { get => _contactAccueilSelectionne; set => SetProperty(ref _contactAccueilSelectionne, value); }
    public DateTime? DateDebutAccueil { get => _dateDebutAccueil; set => SetProperty(ref _dateDebutAccueil, value); }
    public DateTime? DateFinAccueil { get => _dateFinAccueil; set => SetProperty(ref _dateFinAccueil, value); }
    public Animal? AnimalAccueilListeSelectionne { get => _animalAccueilListeSelectionne; set => SetProperty(ref _animalAccueilListeSelectionne, value); }
    public PersonneContact? ContactAccueilListeSelectionne { get => _contactAccueilListeSelectionne; set => SetProperty(ref _contactAccueilListeSelectionne, value); }

    private string _nouveauVaccinNom = string.Empty;
    private Vaccin? _vaccinSelectionne;
    private Animal? _animalVaccinationSelectionne;
    private Vaccin? _vaccinVaccinationSelectionne;
    private DateTime? _dateVaccination;
    private bool _vaccinationFaite = true;

    public string NouveauVaccinNom { get => _nouveauVaccinNom; set => SetProperty(ref _nouveauVaccinNom, value); }
    public Vaccin? VaccinSelectionne { get => _vaccinSelectionne; set => SetProperty(ref _vaccinSelectionne, value); }
    public Animal? AnimalVaccinationSelectionne { get => _animalVaccinationSelectionne; set => SetProperty(ref _animalVaccinationSelectionne, value); }
    public Vaccin? VaccinVaccinationSelectionne { get => _vaccinVaccinationSelectionne; set => SetProperty(ref _vaccinVaccinationSelectionne, value); }
    public DateTime? DateVaccination { get => _dateVaccination; set => SetProperty(ref _dateVaccination, value); }
    public bool VaccinationFaite { get => _vaccinationFaite; set => SetProperty(ref _vaccinationFaite, value); }

    private void RafraichirTout()
    {
        ExecuterAvecGestionErreur(() =>
        {
            Recharger(Contacts, _contactDAO.ListerTous());
            Recharger(AnimauxPresents, _animalDAO.ListerAuRefuge());
            Recharger(TousLesAnimaux, _animalDAO.ListerTous());
            Recharger(MotifsEntree, _motifEntreeDAO.ListerTous());
            Recharger(MotifsSortie, _motifSortieDAO.ListerTous());
            Recharger(Adoptions, _adoptionDAO.ListerToutes());
            Recharger(Vaccins, _vaccinDAO.ListerTous());

            MotifEntreeSelectionne ??= MotifsEntree.FirstOrDefault();
            ContactEntreeSelectionne ??= Contacts.FirstOrDefault();
            AnimalAdoptionSelectionne ??= AnimauxPresents.FirstOrDefault();
            ContactAdoptionSelectionne ??= Contacts.FirstOrDefault();
            AnimalSortieSelectionne ??= AnimauxPresents.FirstOrDefault();
            MotifSortieSelectionne ??= MotifsSortie.FirstOrDefault();
            ContactSortieSelectionne ??= Contacts.FirstOrDefault();
            AnimalAccueilSelectionne ??= AnimauxPresents.FirstOrDefault();
            ContactAccueilSelectionne ??= Contacts.FirstOrDefault(c => c.ALeRole(TypeContact.FamilleAccueil)) ?? Contacts.FirstOrDefault();
            AnimalAccueilListeSelectionne ??= TousLesAnimaux.FirstOrDefault();
            ContactAccueilListeSelectionne ??= Contacts.FirstOrDefault();
            AnimalVaccinationSelectionne ??= TousLesAnimaux.FirstOrDefault();
            VaccinVaccinationSelectionne ??= Vaccins.FirstOrDefault();
            SetInfo("Données chargées.");
        }, ErrorZone.Contact);
    }

    private void AjouterContact()
    {
        EnregistrerContact(false);
    }

    private void ModifierContact()
    {
        EnregistrerContact(true);
    }

    private void EnregistrerContact(bool modification)
    {
        ExecuterAvecGestionErreur(() =>
        {
            var contact = new PersonneContact
            {
                IdContact = modification ? ContactSelectionne?.IdContact ?? 0 : 0,
                Nom = ContactNom.Trim(),
                Prenom = ContactPrenom.Trim(),
                RegistreNational = VideEnNull(ContactRegistreNational),
                Rue = VideEnNull(ContactRue),
                Cp = VideEnNull(ContactCp),
                Localite = VideEnNull(ContactLocalite),
                GSM = VideEnNull(ContactGsm),
                Telephone = VideEnNull(ContactTelephone),
                Email = VideEnNull(ContactEmail),
                Roles = RolesDepuisFormulaire()
            };

            var resultat = _contactService.Enregistrer(contact, modification);
            if (!resultat.Succes)
            {
                SetError(resultat.Message, ErrorZone.Contact);
                return;
            }

            ViderFormulaireContact();
            RafraichirTout();
            SetInfo(resultat.Message);
        }, ErrorZone.Contact);
    }

    private void SupprimerContact()
    {
        ExecuterAvecGestionErreur(() =>
        {
            if (ContactSelectionne == null) return;
            bool ok = _contactDAO.Supprimer(ContactSelectionne.IdContact);
            ViderFormulaireContact();
            RafraichirTout();
            SetInfo(ok ? "Contact supprimé." : "Contact introuvable.");
        }, ErrorZone.Contact);
    }

    private void AjouterAnimal()
    {
        ExecuterAvecGestionErreur(() =>
        {
            if (!DateEntreeAnimal.HasValue)
            {
                SetError("La date d'entrée est obligatoire.", ErrorZone.Animal);
                return;
            }

            if (MotifEntreeSelectionne == null)
            {
                SetError("Le motif d'entrée est obligatoire.", ErrorZone.Animal);
                return;
            }

            var animal = new Animal
            {
                IdAnimal = _animalDAO.GenererIdAnimal(DateEntreeAnimal.Value),
                Nom = AnimalNom.Trim(),
                Type = TypeAnimalSelectionne,
                Sexe = SexeSelectionne,
                DateNaissance = DateNaissanceAnimal,
                Sterilise = AnimalSterilise,
                DateSterilisation = AnimalSterilise ? DateSterilisationAnimal : null,
                Description = VideEnNull(AnimalDescription),
                Particularites = VideEnNull(AnimalParticularites)
            };

            foreach (string couleur in AnimalCouleurs.Split(new[] { ',', ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (!animal.AjouterCouleur(couleur, out string erreurCouleur))
                {
                    SetError(erreurCouleur, ErrorZone.Animal);
                    return;
                }
            }

            var validation = animal.VerifierCreation(DateEntreeAnimal.Value);
            if (!validation.EstValide)
            {
                SetError(validation.PremierMessage, ErrorZone.Animal);
                return;
            }

            _animalDAO.Ajouter(animal, DateEntreeAnimal.Value, MotifEntreeSelectionne.IdMotifEntree, ContactEntreeSelectionne?.IdContact);
            ViderFormulaireAnimal();
            RafraichirTout();
            SetInfo($"Animal créé : {animal.IdAnimal}.");
        }, ErrorZone.Animal);
    }

    private void ChargerDetailsAnimal()
    {
        ExecuterAvecGestionErreur(() =>
        {
            CouleursAnimal.Clear();
            CompatibilitesAnimal.Clear();
            if (AnimalGestionSelectionne == null) return;

            var animal = _animalDAO.Consulter(AnimalGestionSelectionne.IdAnimal) ?? AnimalGestionSelectionne;
            NouvelleDescriptionAnimal = animal.Description ?? string.Empty;
            NouvellesParticularitesAnimal = animal.Particularites ?? string.Empty;
            Recharger(CouleursAnimal, animal.Couleurs);
            Recharger(CompatibilitesAnimal, _compatibiliteDAO.ListerParAnimal(animal.IdAnimal));
            CouleurAnimalSelectionnee = CouleursAnimal.FirstOrDefault() ?? string.Empty;
        }, ErrorZone.AnimalGestion);
    }

    private void ModifierInfosAnimal()
    {
        ExecuterAvecGestionErreur(() =>
        {
            if (AnimalGestionSelectionne == null) return;
            _animalDAO.ModifierDescription(AnimalGestionSelectionne.IdAnimal, VideEnNull(NouvelleDescriptionAnimal), VideEnNull(NouvellesParticularitesAnimal));
            RafraichirTout();
            ChargerDetailsAnimal();
            SetInfo("Description et particularités modifiées.");
        }, ErrorZone.AnimalGestion);
    }

    private void EffacerDescription()
    {
        ExecuterAvecGestionErreur(() =>
        {
            if (AnimalGestionSelectionne == null) return;
            _animalDAO.EffacerDescription(AnimalGestionSelectionne.IdAnimal);
            NouvelleDescriptionAnimal = string.Empty;
            RafraichirTout();
            ChargerDetailsAnimal();
            SetInfo("Description effacée.");
        }, ErrorZone.AnimalGestion);
    }

    private void EffacerParticularites()
    {
        ExecuterAvecGestionErreur(() =>
        {
            if (AnimalGestionSelectionne == null) return;
            _animalDAO.EffacerParticularites(AnimalGestionSelectionne.IdAnimal);
            NouvellesParticularitesAnimal = string.Empty;
            RafraichirTout();
            ChargerDetailsAnimal();
            SetInfo("Particularités effacées.");
        }, ErrorZone.AnimalGestion);
    }

    private void AjouterCouleurAnimal()
    {
        ExecuterAvecGestionErreur(() =>
        {
            if (AnimalGestionSelectionne == null) return;
            var test = new Animal();
            if (!test.AjouterCouleur(NouvelleCouleurAnimal, out string erreur))
            {
                SetError(erreur, ErrorZone.AnimalGestion);
                return;
            }

            _animalDAO.AjouterCouleur(AnimalGestionSelectionne.IdAnimal, test.Couleurs[0]);
            NouvelleCouleurAnimal = string.Empty;
            RafraichirTout();
            ChargerDetailsAnimal();
            SetInfo("Couleur ajoutée.");
        }, ErrorZone.AnimalGestion);
    }

    private void SupprimerCouleurAnimal()
    {
        ExecuterAvecGestionErreur(() =>
        {
            if (AnimalGestionSelectionne == null || string.IsNullOrWhiteSpace(CouleurAnimalSelectionnee)) return;
            _animalDAO.SupprimerCouleur(AnimalGestionSelectionne.IdAnimal, CouleurAnimalSelectionnee);
            RafraichirTout();
            ChargerDetailsAnimal();
            SetInfo("Couleur supprimée.");
        }, ErrorZone.AnimalGestion);
    }

    private void AjouterCompatibilite()
    {
        ExecuterAvecGestionErreur(() =>
        {
            if (AnimalGestionSelectionne == null) return;
            _compatibiliteDAO.Ajouter(new Compatibilite
            {
                IdAnimal = AnimalGestionSelectionne.IdAnimal,
                Type = TypeCompatibiliteSelectionne,
                Valeur = ValeurCompatibiliteSelectionnee,
                Description = VideEnNull(DescriptionCompatibilite)
            });
            DescriptionCompatibilite = string.Empty;
            ChargerDetailsAnimal();
            SetInfo("Compatibilité ajoutée.");
        }, ErrorZone.AnimalGestion);
    }

    private void SupprimerCompatibilite()
    {
        ExecuterAvecGestionErreur(() =>
        {
            if (CompatibiliteSelectionnee == null) return;
            _compatibiliteDAO.Supprimer(CompatibiliteSelectionnee.IdCompat);
            ChargerDetailsAnimal();
            SetInfo("Compatibilité supprimée.");
        }, ErrorZone.AnimalGestion);
    }

    private void EnregistrerSortie()
    {
        ExecuterAvecGestionErreur(() =>
        {
            if (AnimalSortieSelectionne == null || MotifSortieSelectionne == null)
            {
                SetError("Choisissez un animal et un motif de sortie.", ErrorZone.AnimalGestion);
                return;
            }

            DateTime date = DateSortieAnimal ?? DateTime.Today;
            var validation = AnimalSortieSelectionne.VerifierSortie(date);
            if (!validation.EstValide)
            {
                SetError(validation.PremierMessage, ErrorZone.AnimalGestion);
                return;
            }

            int id = _sortieDAO.Ajouter(new Sortie
            {
                IdAnimal = AnimalSortieSelectionne.IdAnimal,
                DateSortie = date,
                IdMotifSortie = MotifSortieSelectionne.IdMotifSortie,
                IdContact = ContactSortieSelectionne?.IdContact
            });
            RafraichirTout();
            SetInfo($"Sortie enregistrée : id {id}.");
        }, ErrorZone.AnimalGestion);
    }

    private void SupprimerAnimal()
    {
        ExecuterAvecGestionErreur(() =>
        {
            if (AnimalGestionSelectionne == null) return;
            bool ok = _animalDAO.Supprimer(AnimalGestionSelectionne.IdAnimal);
            RafraichirTout();
            CouleursAnimal.Clear();
            CompatibilitesAnimal.Clear();
            SetInfo(ok ? "Animal supprimé." : "Animal introuvable.");
        }, ErrorZone.AnimalGestion);
    }

    private void AjouterAdoption()
    {
        ExecuterAvecGestionErreur(() =>
        {
            var resultat = _adoptionService.CreerDemande(
                AnimalAdoptionSelectionne,
                ContactAdoptionSelectionne,
                DateDemandeAdoption);
            if (!resultat.Succes)
            {
                SetError(resultat.Message, ErrorZone.Adoption);
                return;
            }

            RafraichirTout();
            SetInfo(resultat.Message);
        }, ErrorZone.Adoption);
    }

    private void ModifierStatutAdoption()
    {
        ExecuterAvecGestionErreur(() =>
        {
            var resultat = _adoptionService.ModifierStatut(AdoptionSelectionnee, NouveauStatutAdoption);
            if (!resultat.Succes)
            {
                SetError(resultat.Message, ErrorZone.Adoption);
                return;
            }

            RafraichirTout();
            SetInfo(resultat.Message);
        }, ErrorZone.Adoption);
    }

    private void AjouterAccueil()
    {
        ExecuterAvecGestionErreur(() =>
        {
            var resultat = _accueilService.Ajouter(
                AnimalAccueilSelectionne,
                ContactAccueilSelectionne,
                DateDebutAccueil,
                DateFinAccueil);
            if (!resultat.Succes)
            {
                SetError(resultat.Message, ErrorZone.Accueil);
                return;
            }

            ChargerAccueils();
            SetInfo(resultat.Message);
        }, ErrorZone.Accueil);
    }

    private void ChargerAccueils()
    {
        ExecuterAvecGestionErreur(() =>
        {
            Recharger(AccueilsParAnimal, AnimalAccueilListeSelectionne == null
                ? Array.Empty<FamilleAccueil>()
                : _familleAccueilDAO.ListerParAnimal(AnimalAccueilListeSelectionne.IdAnimal));
            Recharger(AccueilsParFamille, ContactAccueilListeSelectionne == null
                ? Array.Empty<FamilleAccueil>()
                : _familleAccueilDAO.ListerParFamille(ContactAccueilListeSelectionne.IdContact));
            SetInfo("Accueils chargés.");
        }, ErrorZone.Accueil);
    }

    private void AjouterVaccin()
    {
        ExecuterAvecGestionErreur(() =>
        {
            string nom = NouveauVaccinNom.Trim();
            if (nom.Length < 2)
            {
                SetError("Le nom du vaccin doit contenir au moins 2 caractères.", ErrorZone.Vaccin);
                return;
            }

            int id = _vaccinDAO.Ajouter(nom);
            NouveauVaccinNom = string.Empty;
            Recharger(Vaccins, _vaccinDAO.ListerTous());
            SetInfo($"Vaccin enregistré : id {id}.");
        }, ErrorZone.Vaccin);
    }

    private void SupprimerVaccin()
    {
        ExecuterAvecGestionErreur(() =>
        {
            if (VaccinSelectionne == null) return;
            bool ok = _vaccinDAO.Supprimer(VaccinSelectionne.IdVaccin);
            Recharger(Vaccins, _vaccinDAO.ListerTous());
            SetInfo(ok ? "Vaccin supprimé du catalogue." : "Vaccin introuvable.");
        }, ErrorZone.Vaccin);
    }

    private void AjouterVaccination()
    {
        ExecuterAvecGestionErreur(() =>
        {
            if (AnimalVaccinationSelectionne == null || VaccinVaccinationSelectionne == null)
            {
                SetError("Choisissez un animal et un vaccin.", ErrorZone.Vaccination);
                return;
            }

            DateTime date = DateVaccination ?? DateTime.Today;
            if (AnimalVaccinationSelectionne.DateNaissance.HasValue &&
                date.Date < AnimalVaccinationSelectionne.DateNaissance.Value.Date)
            {
                SetError("La date du vaccin est antérieure à la date de naissance de l'animal.", ErrorZone.Vaccination);
                return;
            }

            int id = _vaccinationDAO.Ajouter(new Vaccination
            {
                IdAnimal = AnimalVaccinationSelectionne.IdAnimal,
                IdVaccin = VaccinVaccinationSelectionne.IdVaccin,
                DateVaccin = date,
                Fait = VaccinationFaite
            });
            ChargerVaccinationsAnimal();
            SetInfo($"Vaccination enregistrée : id {id}.");
        }, ErrorZone.Vaccination);
    }

    private void ChargerVaccinationsAnimal()
    {
        ExecuterAvecGestionErreur(() =>
        {
            Recharger(VaccinationsAnimal, AnimalVaccinationSelectionne == null
                ? Array.Empty<Vaccination>()
                : _vaccinationDAO.ListerParAnimal(AnimalVaccinationSelectionne.IdAnimal));
            SetInfo("Vaccinations chargées.");
        }, ErrorZone.Vaccination);
    }

    private List<TypeContact> RolesDepuisFormulaire()
    {
        var roles = new List<TypeContact>();
        if (RoleBenevole) roles.Add(TypeContact.Benevole);
        if (RoleAdoptant) roles.Add(TypeContact.Adoptant);
        if (RoleCandidat) roles.Add(TypeContact.Candidat);
        if (RoleFamilleAccueil) roles.Add(TypeContact.FamilleAccueil);
        return roles;
    }

    private void ViderFormulaireContact()
    {
        ContactNom = string.Empty;
        ContactPrenom = string.Empty;
        ContactRegistreNational = string.Empty;
        ContactRue = string.Empty;
        ContactCp = string.Empty;
        ContactLocalite = string.Empty;
        ContactGsm = string.Empty;
        ContactTelephone = string.Empty;
        ContactEmail = string.Empty;
        RoleBenevole = false;
        RoleAdoptant = false;
        RoleCandidat = true;
        RoleFamilleAccueil = false;
        ContactSelectionne = null;
    }

    private void ViderFormulaireAnimal()
    {
        AnimalNom = string.Empty;
        DateEntreeAnimal = DateTime.Today;
        TypeAnimalSelectionne = TypeAnimal.Chat;
        SexeSelectionne = Sexe.M;
        DateNaissanceAnimal = null;
        AnimalSterilise = false;
        DateSterilisationAnimal = null;
        AnimalDescription = string.Empty;
        AnimalParticularites = string.Empty;
        AnimalCouleurs = string.Empty;
    }

    private void SetInfo(string message)
    {
        ViderMessagesErreur();
        StatusMessage = message;
        StatusBrush = Brushes.ForestGreen;
    }

    private void SetError(string message, ErrorZone zone)
    {
        ViderMessagesErreur();
        string texte = NormaliserMessageErreur(message);

        switch (zone)
        {
            case ErrorZone.Contact:
                ContactErrorMessage = texte;
                break;
            case ErrorZone.Animal:
                AnimalErrorMessage = texte;
                break;
            case ErrorZone.AnimalGestion:
                AnimalGestionErrorMessage = texte;
                break;
            case ErrorZone.Adoption:
                AdoptionErrorMessage = texte;
                break;
            case ErrorZone.Accueil:
                AccueilErrorMessage = texte;
                break;
            case ErrorZone.Vaccin:
                VaccinErrorMessage = texte;
                break;
            case ErrorZone.Vaccination:
                VaccinationErrorMessage = texte;
                break;
        }

        StatusMessage = "Erreur de saisie.";
        StatusBrush = Brushes.Firebrick;
    }

    private void ViderMessagesErreur()
    {
        ErrorMessage = string.Empty;
        ContactErrorMessage = string.Empty;
        AnimalErrorMessage = string.Empty;
        AnimalGestionErrorMessage = string.Empty;
        AdoptionErrorMessage = string.Empty;
        AccueilErrorMessage = string.Empty;
        VaccinErrorMessage = string.Empty;
        VaccinationErrorMessage = string.Empty;
    }

    private static string NormaliserMessageErreur(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return string.Empty;

        string texte = message.Trim()
            .Replace("prenom", "prénom", StringComparison.OrdinalIgnoreCase)
            .Replace("caracteres", "caractères", StringComparison.OrdinalIgnoreCase)
            .Replace("telephone", "téléphone", StringComparison.OrdinalIgnoreCase)
            .Replace("role", "rôle", StringComparison.OrdinalIgnoreCase);

        return char.ToUpper(texte[0]) + texte[1..];
    }

    private enum ErrorZone
    {
        Contact,
        Animal,
        AnimalGestion,
        Adoption,
        Accueil,
        Vaccin,
        Vaccination
    }

    private static string? VideEnNull(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static void Recharger<T>(ObservableCollection<T> collection, IEnumerable<T> valeurs)
    {
        collection.Clear();
        foreach (T valeur in valeurs) collection.Add(valeur);
    }

    private void ExecuterAvecGestionErreur(Action action, ErrorZone zone = ErrorZone.Contact)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            SetError(GestionErreurs.Traduire(ex), zone);
        }
    }
}
