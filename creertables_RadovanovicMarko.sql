-- =============================================================
-- Projet : Gestion d'un refuge d'animaux
-- Auteur : Marko Radovanovic
-- Travail 2 - partie 1 : creation des tables PostgreSQL
-- =============================================================

DROP TABLE IF EXISTS VACCINATION CASCADE;
DROP TABLE IF EXISTS VACCIN CASCADE;
DROP TABLE IF EXISTS ADOPTION CASCADE;
DROP TABLE IF EXISTS FAMILLE_ACCUEIL CASCADE;
DROP TABLE IF EXISTS ANI_SORTIE CASCADE;
DROP TABLE IF EXISTS ANI_ENTREE CASCADE;
DROP TABLE IF EXISTS ANI_COMPATIBILITE CASCADE;
DROP TABLE IF EXISTS COMPATIBILITE CASCADE;
DROP TABLE IF EXISTS ANIMAL_COULEUR CASCADE;
DROP TABLE IF EXISTS COULEUR CASCADE;
DROP TABLE IF EXISTS PERSONNE_ROLE CASCADE;
DROP TABLE IF EXISTS ROLE CASCADE;
DROP TABLE IF EXISTS CONTACT CASCADE;
DROP TABLE IF EXISTS ANIMAL CASCADE;
DROP TABLE IF EXISTS MOTIF_ENTREE CASCADE;
DROP TABLE IF EXISTS MOTIF_SORTIE CASCADE;

-- =============================================================
-- ANIMAL
-- =============================================================
CREATE TABLE ANIMAL (
    idAnimal           CHAR(11)      NOT NULL,
    nom                VARCHAR(50)   NOT NULL,
    type               VARCHAR(10)   NOT NULL,
    sexe               CHAR(1)       NOT NULL,
    particularites     TEXT,
    dateDeces          DATE,
    description        TEXT,
    dateSterilisation  DATE,
    sterilise          BOOLEAN       NOT NULL DEFAULT FALSE,
    dateNaissance      DATE,
    CONSTRAINT pk_animal PRIMARY KEY (idAnimal),
    CONSTRAINT ck_animal_id_format CHECK (idAnimal ~ '^[0-9]{11}$'),
    CONSTRAINT ck_animal_type CHECK (type IN ('chat', 'chien')),
    CONSTRAINT ck_animal_sexe CHECK (sexe IN ('M', 'F')),
    CONSTRAINT ck_animal_sterilise_date CHECK (
        (sterilise = TRUE AND dateSterilisation IS NOT NULL)
        OR (sterilise = FALSE AND dateSterilisation IS NULL)
    ),
    CONSTRAINT ck_animal_date_sterilisation CHECK (
        dateSterilisation IS NULL
        OR dateNaissance IS NULL
        OR dateSterilisation >= dateNaissance
    ),
    CONSTRAINT ck_animal_date_deces CHECK (
        dateDeces IS NULL
        OR dateNaissance IS NULL
        OR dateDeces >= dateNaissance
    ),
    CONSTRAINT ck_animal_date_naissance CHECK (
        dateNaissance IS NULL OR dateNaissance <= CURRENT_DATE
    )
);

-- =============================================================
-- COULEUR / ANIMAL_COULEUR
-- =============================================================
CREATE TABLE COULEUR (
    colIdentifiant  SERIAL       NOT NULL,
    nomCouleur      VARCHAR(30)  NOT NULL,
    CONSTRAINT pk_couleur PRIMARY KEY (colIdentifiant),
    CONSTRAINT uq_couleur_nom UNIQUE (nomCouleur)
);

CREATE TABLE ANIMAL_COULEUR (
    colIdentifiant  INTEGER   NOT NULL,
    idAnimal        CHAR(11)  NOT NULL,
    CONSTRAINT pk_animal_couleur PRIMARY KEY (colIdentifiant, idAnimal),
    CONSTRAINT fk_animal_couleur_couleur FOREIGN KEY (colIdentifiant)
        REFERENCES COULEUR(colIdentifiant),
    CONSTRAINT fk_animal_couleur_animal FOREIGN KEY (idAnimal)
        REFERENCES ANIMAL(idAnimal) ON DELETE CASCADE
);

-- =============================================================
-- CONTACT / ROLE / PERSONNE_ROLE
-- =============================================================
CREATE TABLE CONTACT (
    idContact         SERIAL        NOT NULL,
    nom               VARCHAR(50)   NOT NULL,
    prenom            VARCHAR(50)   NOT NULL,
    rue               VARCHAR(100),
    cp                VARCHAR(10),
    localite          VARCHAR(80),
    registreNational  VARCHAR(20),
    gsm               VARCHAR(20),
    telephone         VARCHAR(20),
    email             VARCHAR(100),
    CONSTRAINT pk_contact PRIMARY KEY (idContact),
    CONSTRAINT uq_contact_registre UNIQUE (registreNational),
    CONSTRAINT ck_contact_nom CHECK (length(trim(nom)) >= 2),
    CONSTRAINT ck_contact_prenom CHECK (length(trim(prenom)) >= 2),
    CONSTRAINT ck_contact_registre_format CHECK (
        registreNational IS NULL OR registreNational ~ '^[0-9]{2}\.[0-9]{2}\.[0-9]{2}-[0-9]{3}\.[0-9]{2}$'
    ),
    CONSTRAINT ck_contact_moyen CHECK (
        gsm IS NOT NULL OR telephone IS NOT NULL OR email IS NOT NULL
    ),
    CONSTRAINT ck_contact_email CHECK (
        email IS NULL OR email ~* '^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$'
    )
);

CREATE TABLE ROLE (
    rolIdentifiant  SERIAL       NOT NULL,
    rolNom          VARCHAR(30)  NOT NULL,
    CONSTRAINT pk_role PRIMARY KEY (rolIdentifiant),
    CONSTRAINT uq_role_nom UNIQUE (rolNom),
    CONSTRAINT ck_role_nom CHECK (rolNom IN ('benevole', 'adoptant', 'candidat', 'famille_accueil'))
);

CREATE TABLE PERSONNE_ROLE (
    idContact       INTEGER  NOT NULL,
    rolIdentifiant  INTEGER  NOT NULL,
    CONSTRAINT pk_personne_role PRIMARY KEY (idContact, rolIdentifiant),
    CONSTRAINT fk_personne_role_contact FOREIGN KEY (idContact)
        REFERENCES CONTACT(idContact) ON DELETE CASCADE,
    CONSTRAINT fk_personne_role_role FOREIGN KEY (rolIdentifiant)
        REFERENCES ROLE(rolIdentifiant)
);

-- =============================================================
-- MOTIFS D'ENTREE / SORTIE
-- =============================================================
CREATE TABLE MOTIF_ENTREE (
    idMotifEntree  SERIAL       NOT NULL,
    libelle        VARCHAR(40)  NOT NULL,
    CONSTRAINT pk_motif_entree PRIMARY KEY (idMotifEntree),
    CONSTRAINT uq_motif_entree UNIQUE (libelle),
    CONSTRAINT ck_motif_entree CHECK (libelle IN
        ('abandon', 'errant', 'deces_proprietaire', 'saisie',
         'retour_adoption', 'retour_famille_accueil'))
);

CREATE TABLE MOTIF_SORTIE (
    idMotifSortie  SERIAL       NOT NULL,
    libelle        VARCHAR(40)  NOT NULL,
    CONSTRAINT pk_motif_sortie PRIMARY KEY (idMotifSortie),
    CONSTRAINT uq_motif_sortie UNIQUE (libelle),
    CONSTRAINT ck_motif_sortie CHECK (libelle IN
        ('adoption', 'retour_proprietaire', 'deces_animal', 'famille_accueil'))
);

-- =============================================================
-- ENTREES / SORTIES
-- =============================================================
CREATE TABLE ANI_ENTREE (
    idEntree       SERIAL    NOT NULL,
    raison         VARCHAR(40) NOT NULL,
    dateEntree     DATE      NOT NULL,
    idAnimal       CHAR(11)  NOT NULL,
    entreeContact  INTEGER,
    CONSTRAINT pk_ani_entree PRIMARY KEY (idEntree),
    CONSTRAINT fk_ani_entree_animal FOREIGN KEY (idAnimal)
        REFERENCES ANIMAL(idAnimal) ON DELETE CASCADE,
    CONSTRAINT fk_ani_entree_contact FOREIGN KEY (entreeContact)
        REFERENCES CONTACT(idContact),
    CONSTRAINT ck_ani_entree_raison CHECK (raison IN
        ('abandon', 'errant', 'deces_proprietaire', 'saisie',
         'retour_adoption', 'retour_famille_accueil'))
);

CREATE TABLE ANI_SORTIE (
    idSortie       SERIAL    NOT NULL,
    raison         VARCHAR(40) NOT NULL,
    dateSortie     DATE      NOT NULL,
    idAnimal       CHAR(11)  NOT NULL,
    sortieContact  INTEGER,
    CONSTRAINT pk_ani_sortie PRIMARY KEY (idSortie),
    CONSTRAINT fk_ani_sortie_animal FOREIGN KEY (idAnimal)
        REFERENCES ANIMAL(idAnimal) ON DELETE CASCADE,
    CONSTRAINT fk_ani_sortie_contact FOREIGN KEY (sortieContact)
        REFERENCES CONTACT(idContact),
    CONSTRAINT ck_ani_sortie_raison CHECK (raison IN
        ('adoption', 'retour_proprietaire', 'deces_animal', 'famille_accueil'))
);

-- =============================================================
-- COMPATIBILITE / ANI_COMPATIBILITE
-- =============================================================
CREATE TABLE COMPATIBILITE (
    idCompatibilite  SERIAL       NOT NULL,
    type             VARCHAR(20)  NOT NULL,
    CONSTRAINT pk_compatibilite PRIMARY KEY (idCompatibilite),
    CONSTRAINT uq_compatibilite_type UNIQUE (type),
    CONSTRAINT ck_compatibilite_type CHECK
        (type IN ('chat', 'chien', 'jeune enfant', 'enfant', 'jardin', 'poney'))
);

CREATE TABLE ANI_COMPATIBILITE (
    idAniCompatibilite  SERIAL    NOT NULL,
    valeur              BOOLEAN   NOT NULL,
    description         TEXT,
    idCompatibilite     INTEGER   NOT NULL,
    idAnimal            CHAR(11)  NOT NULL,
    CONSTRAINT pk_ani_compatibilite PRIMARY KEY (idAniCompatibilite),
    CONSTRAINT fk_ani_compatibilite_type FOREIGN KEY (idCompatibilite)
        REFERENCES COMPATIBILITE(idCompatibilite),
    CONSTRAINT fk_ani_compatibilite_animal FOREIGN KEY (idAnimal)
        REFERENCES ANIMAL(idAnimal) ON DELETE CASCADE,
    CONSTRAINT uq_ani_compatibilite UNIQUE (idAnimal, idCompatibilite)
);

-- =============================================================
-- FAMILLE_ACCUEIL
-- =============================================================
CREATE TABLE FAMILLE_ACCUEIL (
    idAccueil  SERIAL    NOT NULL,
    dateDebut  DATE      NOT NULL,
    dateFin    DATE,
    idAnimal   CHAR(11)  NOT NULL,
    idContact  INTEGER   NOT NULL,
    CONSTRAINT pk_famille_accueil PRIMARY KEY (idAccueil),
    CONSTRAINT fk_famille_accueil_animal FOREIGN KEY (idAnimal)
        REFERENCES ANIMAL(idAnimal) ON DELETE CASCADE,
    CONSTRAINT fk_famille_accueil_contact FOREIGN KEY (idContact)
        REFERENCES CONTACT(idContact),
    CONSTRAINT ck_famille_accueil_dates CHECK (dateFin IS NULL OR dateFin >= dateDebut)
);

CREATE UNIQUE INDEX ux_famille_accueil_un_actif
    ON FAMILLE_ACCUEIL(idAnimal)
    WHERE dateFin IS NULL;

-- =============================================================
-- ADOPTION
-- =============================================================
CREATE TABLE ADOPTION (
    idAdoption  SERIAL       NOT NULL,
    statut      VARCHAR(30)  NOT NULL,
    dateDemande DATE         NOT NULL DEFAULT CURRENT_DATE,
    idAnimal    CHAR(11)     NOT NULL,
    idContact   INTEGER      NOT NULL,
    CONSTRAINT pk_adoption PRIMARY KEY (idAdoption),
    CONSTRAINT fk_adoption_animal FOREIGN KEY (idAnimal)
        REFERENCES ANIMAL(idAnimal) ON DELETE CASCADE,
    CONSTRAINT fk_adoption_contact FOREIGN KEY (idContact)
        REFERENCES CONTACT(idContact),
    CONSTRAINT ck_adoption_statut CHECK
        (statut IN ('demande', 'acceptee', 'rejet_environnement', 'rejet_comportement')),
    CONSTRAINT uq_adoption_demande UNIQUE (idAnimal, idContact)
);

CREATE UNIQUE INDEX ux_adoption_une_acceptee
    ON ADOPTION(idAnimal)
    WHERE statut = 'acceptee';

-- =============================================================
-- VACCIN / VACCINATION
-- =============================================================
CREATE TABLE VACCIN (
    idVaccin   SERIAL       NOT NULL,
    nomVaccin  VARCHAR(80)  NOT NULL,
    CONSTRAINT pk_vaccin PRIMARY KEY (idVaccin),
    CONSTRAINT uq_vaccin_nom UNIQUE (nomVaccin)
);

CREATE TABLE VACCINATION (
    idVaccination  SERIAL    NOT NULL,
    dateVaccin     DATE      NOT NULL,
    fait           BOOLEAN   NOT NULL DEFAULT FALSE,
    idAnimal       CHAR(11)  NOT NULL,
    idVaccin       INTEGER   NOT NULL,
    CONSTRAINT pk_vaccination PRIMARY KEY (idVaccination),
    CONSTRAINT fk_vaccination_animal FOREIGN KEY (idAnimal)
        REFERENCES ANIMAL(idAnimal) ON DELETE CASCADE,
    CONSTRAINT fk_vaccination_vaccin FOREIGN KEY (idVaccin)
        REFERENCES VACCIN(idVaccin),
    CONSTRAINT uq_vaccination_jour UNIQUE (idAnimal, idVaccin, dateVaccin)
);

-- =============================================================
-- DONNEES DE REFERENCE
-- =============================================================
INSERT INTO ROLE (rolNom) VALUES
    ('benevole'),
    ('adoptant'),
    ('candidat'),
    ('famille_accueil');

INSERT INTO MOTIF_ENTREE (libelle) VALUES
    ('abandon'),
    ('errant'),
    ('deces_proprietaire'),
    ('saisie'),
    ('retour_adoption'),
    ('retour_famille_accueil');

INSERT INTO MOTIF_SORTIE (libelle) VALUES
    ('adoption'),
    ('retour_proprietaire'),
    ('deces_animal'),
    ('famille_accueil');

INSERT INTO COMPATIBILITE (type) VALUES
    ('chat'),
    ('chien'),
    ('jeune enfant'),
    ('enfant'),
    ('jardin'),
    ('poney');

-- =============================================================
-- FIN
-- =============================================================
