# Refuge d'animaux - Application WPF

Projet C# / PostgreSQL pour le Travail 2 du cours de Projet SGBD.

Le projet contient les classes metier, la couche acces aux donnees et une
interface WPF pour les fonctionnalites de l'application. La presentation WPF
utilise des ViewModels et des services applicatifs qui manipulent les objets
metier avant de passer par la couche acces BD.

## Architecture

- `classesMetier/` : objets metier echanges entre les couches, avec les regles
  et validations propres au domaine
- `coucheAccesBD/` : acces PostgreSQL via Npgsql, uniquement par appels aux
  fonctions/procedures stockees
- `couchePresentation/` : presentation de l'application
- `couchePresentation/Views/` : fenetres WPF de la partie 2
- `couchePresentation/ViewModels/` : ViewModels WPF, commandes et binding
- `couchePresentation/Services/` : orchestration applicative entre la
  presentation, les objets metier et les DAO
- `App.xaml` : configuration globale WPF, ressources et fenetre de demarrage

## Base de donnees

Creer une base PostgreSQL nommee `refuge_animaux`, puis executer les scripts dans cet ordre :

1. `creertables_RadovanovicMarko.sql`
2. `creerprocedures_RadovanovicMarko.sql`

Si la base existait deja avant la partie 2, reexecuter au minimum
`creerprocedures_RadovanovicMarko.sql` afin de mettre a jour les fonctions
appelees par l'application.

Les tables suivent le modele du refuge : `ANIMAL`, `CONTACT`, `ROLE`, `PERSONNE_ROLE`,
`ANI_ENTREE`, `ANI_SORTIE`, `ADOPTION`, `COMPATIBILITE`, `ANI_COMPATIBILITE`,
`FAMILLE_ACCUEIL`, `VACCIN`, `VACCINATION`, etc.

## Connexion

Par defaut, l'application utilise :

```text
Host=localhost;Port=5432;Username=postgres;Password=MOT_DE_PASSE;Database=refuge_animaux
```

Pour utiliser une autre configuration sans modifier le code, definir la variable
d'environnement `REFUGE_ANIMAUX_CONNECTION`.

### Definir la variable sous Windows

Pour la definir uniquement dans la fenetre PowerShell actuelle :

```powershell
$env:REFUGE_ANIMAUX_CONNECTION="Host=localhost;Port=5432;Username=postgres;Password=MOT_DE_PASSE;Database=refuge_animaux"
dotnet run
```

Pour la definir durablement :

```powershell
[Environment]::SetEnvironmentVariable(
  "REFUGE_ANIMAUX_CONNECTION",
  "Host=localhost;Port=5432;Username=postgres;Password=MOT_DE_PASSE;Database=refuge_animaux",
  "User"
)
```

Apres la definition permanente, fermer puis rouvrir PowerShell, Rider ou VS Code
pour que la nouvelle variable soit prise en compte.

## Lancer l'application

```bash
dotnet restore
dotnet run
```

Le lancement ouvre la fenetre WPF principale.

## Fonctionnalites

- Animaux : ajout avec entree, consultation, listes, description, particularites, couleurs, compatibilites, sortie, suppression
- Contacts : ajout, consultation, liste, modification, suppression, gestion de plusieurs roles
- Familles d'accueil : ajout et listes par animal ou par famille
- Adoptions : demandes, liste, modification de statut
- Vaccins / vaccinations : catalogue et vaccinations par animal
