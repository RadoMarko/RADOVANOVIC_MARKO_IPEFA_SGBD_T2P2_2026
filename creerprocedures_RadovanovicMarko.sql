-- =============================================================
-- Projet : Gestion d'un refuge d'animaux
-- Auteur : Marko Radovanovic
-- Procedures/fonctions appelees par la couche acces BD
-- =============================================================

DROP FUNCTION IF EXISTS genererIdAnimal(date);
DROP FUNCTION IF EXISTS ajouterAnimal(char, varchar, varchar, char, boolean, date, date, date, text, text, text[], date, integer, integer);
DROP FUNCTION IF EXISTS consulterAnimal(char);
DROP FUNCTION IF EXISTS supprimerAnimal(char);
DROP FUNCTION IF EXISTS listerAnimaux();
DROP FUNCTION IF EXISTS listerAnimauxAuRefuge();
DROP FUNCTION IF EXISTS modifierAnimal(text, varchar, varchar, text, boolean, date, date, date, text, text, text[]);
DROP FUNCTION IF EXISTS modifierDescriptionParticularites(char, text, text);
DROP FUNCTION IF EXISTS effacerDescription(char);
DROP FUNCTION IF EXISTS effacerParticularites(char);
DROP FUNCTION IF EXISTS ajouterCouleur(char, varchar);
DROP FUNCTION IF EXISTS supprimerCouleur(char, varchar);
DROP FUNCTION IF EXISTS listerCouleurs(char);
DROP FUNCTION IF EXISTS ajouterCompatibilite(varchar, varchar, text, char);
DROP FUNCTION IF EXISTS supprimerCompatibilite(integer);
DROP FUNCTION IF EXISTS listerCompatibilitesParAnimal(char);
DROP FUNCTION IF EXISTS ajouterContact(varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar);
DROP FUNCTION IF EXISTS ajouterContact(varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, text[]);
DROP FUNCTION IF EXISTS consulterContact(integer);
DROP FUNCTION IF EXISTS listerContacts();
DROP FUNCTION IF EXISTS registreNationalExiste(varchar, integer);
DROP FUNCTION IF EXISTS modifierContact(integer, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar);
DROP FUNCTION IF EXISTS modifierContact(integer, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, text[]);
DROP FUNCTION IF EXISTS supprimerContact(integer);
DROP FUNCTION IF EXISTS listerMotifsEntree();
DROP FUNCTION IF EXISTS listerMotifsSortie();
DROP FUNCTION IF EXISTS ajouterSortie(date, char, integer, integer);
DROP FUNCTION IF EXISTS ajouterAccueil(date, date, char, integer);
DROP FUNCTION IF EXISTS modifierDateFinAccueil(integer, date);
DROP FUNCTION IF EXISTS aDejaUnAccueilEnCours(char);
DROP FUNCTION IF EXISTS aUnAccueilQuiChevauche(char, date, date);
DROP FUNCTION IF EXISTS listerAccueilsParAnimal(char);
DROP FUNCTION IF EXISTS listerAccueilsParFamille(integer);
DROP FUNCTION IF EXISTS listerIdsAnimauxAvecAccueil();
DROP FUNCTION IF EXISTS listerIdsContactsAvecAccueil();
DROP FUNCTION IF EXISTS ajouterAdoption(varchar, char, integer);
DROP FUNCTION IF EXISTS ajouterAdoption(varchar, date, char, integer);
DROP FUNCTION IF EXISTS aDejaUneAdoptionAcceptee(char);
DROP FUNCTION IF EXISTS aDejaUneDemande(char, integer);
DROP FUNCTION IF EXISTS modifierStatutAdoption(integer, varchar);
DROP FUNCTION IF EXISTS listerAdoptions();
DROP FUNCTION IF EXISTS ajouterVaccin(varchar);
DROP FUNCTION IF EXISTS supprimerVaccin(integer);
DROP FUNCTION IF EXISTS listerVaccins();
DROP FUNCTION IF EXISTS ajouterVaccination(date, boolean, char, integer);
DROP FUNCTION IF EXISTS vaccinationExiste(date, char, integer);
DROP FUNCTION IF EXISTS consulterVaccination(date, char, integer);
DROP FUNCTION IF EXISTS modifierEtatVaccination(integer, boolean);
DROP FUNCTION IF EXISTS listerVaccinationsParAnimal(char);
DROP FUNCTION IF EXISTS ajouterAnimal(text, varchar, varchar, text, boolean, date, date, date, text, text, text[], date, integer, integer);
DROP FUNCTION IF EXISTS consulterAnimal(text);
DROP FUNCTION IF EXISTS supprimerAnimal(text);
DROP FUNCTION IF EXISTS modifierAnimal(text, varchar, varchar, text, boolean, date, date, date, text, text, text[]);
DROP FUNCTION IF EXISTS modifierDescriptionParticularites(text, text, text);
DROP FUNCTION IF EXISTS effacerDescription(text);
DROP FUNCTION IF EXISTS effacerParticularites(text);
DROP FUNCTION IF EXISTS ajouterCouleur(text, varchar);
DROP FUNCTION IF EXISTS supprimerCouleur(text, varchar);
DROP FUNCTION IF EXISTS listerCouleurs(text);
DROP FUNCTION IF EXISTS ajouterCompatibilite(varchar, varchar, text, text);
DROP FUNCTION IF EXISTS listerCompatibilitesParAnimal(text);
DROP FUNCTION IF EXISTS ajouterSortie(date, text, integer, integer);
DROP FUNCTION IF EXISTS ajouterAccueil(date, date, text, integer);
DROP FUNCTION IF EXISTS ajouterEntree(date, text, integer, integer);
DROP FUNCTION IF EXISTS clonerAnimalPourRetour(text, date, integer, integer);
DROP FUNCTION IF EXISTS supprimerDerniereSortie(text, varchar);
DROP FUNCTION IF EXISTS modifierDateFinAccueil(integer, date);
DROP FUNCTION IF EXISTS aDejaUnAccueilEnCours(text);
DROP FUNCTION IF EXISTS aUnAccueilQuiChevauche(text, date, date);
DROP FUNCTION IF EXISTS listerAccueilsParAnimal(text);
DROP FUNCTION IF EXISTS listerIdsAnimauxAvecAccueil();
DROP FUNCTION IF EXISTS listerIdsContactsAvecAccueil();
DROP FUNCTION IF EXISTS ajouterAdoption(varchar, date, text, integer);
DROP FUNCTION IF EXISTS aDejaUneAdoptionAcceptee(text);
DROP FUNCTION IF EXISTS aDejaUneDemande(text, integer);
DROP FUNCTION IF EXISTS ajouterVaccination(date, boolean, text, integer);
DROP FUNCTION IF EXISTS vaccinationExiste(date, text, integer);
DROP FUNCTION IF EXISTS consulterVaccination(date, text, integer);
DROP FUNCTION IF EXISTS modifierEtatVaccination(integer, boolean);
DROP FUNCTION IF EXISTS listerVaccinationsParAnimal(text);

-- =============================================================
-- Helpers
-- =============================================================
CREATE OR REPLACE FUNCTION genererIdAnimal(p_date date)
RETURNS char(11) AS $$
DECLARE
    prefixe text;
    suivant integer;
BEGIN
    prefixe := to_char(p_date, 'YYMMDD');
    SELECT COALESCE(MAX(SUBSTRING(idAnimal, 7, 5)::int), 0) + 1
      INTO suivant
      FROM ANIMAL
     WHERE idAnimal LIKE prefixe || '%';

    RETURN prefixe || LPAD(suivant::text, 5, '0');
END;
$$ LANGUAGE plpgsql;

-- =============================================================
-- ANIMAL
-- =============================================================
CREATE OR REPLACE FUNCTION ajouterAnimal(
    p_idAnimal          text,
    p_nom               varchar,
    p_type              varchar,
    p_sexe              text,
    p_sterilise         boolean,
    p_dateSterilisation date,
    p_dateNaissance     date,
    p_dateDeces         date,
    p_description       text,
    p_particularites    text,
    p_couleurs          text[],
    p_dateEntree        date,
    p_idMotifEntree     integer,
    p_idContact         integer
) RETURNS void AS $$
DECLARE
    c text;
    id_couleur integer;
    motif varchar;
BEGIN
    SELECT libelle INTO motif FROM MOTIF_ENTREE WHERE idMotifEntree = p_idMotifEntree;
    IF motif IS NULL THEN
        RAISE EXCEPTION 'Motif d''entree introuvable';
    END IF;

    INSERT INTO ANIMAL (idAnimal, nom, type, sexe, sterilise, dateSterilisation,
                        dateNaissance, dateDeces, description, particularites)
    VALUES (p_idAnimal::char(11), p_nom, p_type, p_sexe::char(1), p_sterilise, p_dateSterilisation,
            p_dateNaissance, p_dateDeces, p_description, p_particularites);

    IF p_couleurs IS NOT NULL THEN
        FOREACH c IN ARRAY p_couleurs LOOP
            IF length(trim(c)) > 0 THEN
                INSERT INTO COULEUR (nomCouleur)
                VALUES (trim(c))
                ON CONFLICT (nomCouleur) DO UPDATE SET nomCouleur = EXCLUDED.nomCouleur
                RETURNING colIdentifiant INTO id_couleur;

                INSERT INTO ANIMAL_COULEUR (colIdentifiant, idAnimal)
                VALUES (id_couleur, p_idAnimal::char(11))
                ON CONFLICT DO NOTHING;
            END IF;
        END LOOP;
    END IF;

    INSERT INTO ANI_ENTREE (raison, dateEntree, idAnimal, entreeContact)
    VALUES (motif, p_dateEntree, p_idAnimal::char(11), p_idContact);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION ajouterEntree(
    p_dateEntree date,
    p_idAnimal text,
    p_idMotifEntree integer,
    p_idContact integer
) RETURNS integer AS $$
DECLARE
    nouvelId integer;
    motif varchar;
BEGIN
    SELECT libelle INTO motif FROM MOTIF_ENTREE WHERE idMotifEntree = p_idMotifEntree;
    IF motif IS NULL THEN
        RAISE EXCEPTION 'Motif d''entree introuvable';
    END IF;

    INSERT INTO ANI_ENTREE (raison, dateEntree, idAnimal, entreeContact)
    VALUES (motif, p_dateEntree, p_idAnimal::char(11), p_idContact)
    RETURNING idEntree INTO nouvelId;

    UPDATE ANIMAL SET dateDeces = NULL WHERE idAnimal = p_idAnimal::char(11);
    RETURN nouvelId;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION clonerAnimalPourRetour(
    p_idAnimalSource text,
    p_dateEntree date,
    p_idMotifEntree integer,
    p_idContact integer
) RETURNS char(11) AS $$
DECLARE
    nouvelId char(11);
    motif varchar;
BEGIN
    SELECT libelle INTO motif FROM MOTIF_ENTREE WHERE idMotifEntree = p_idMotifEntree;
    IF motif IS NULL THEN
        RAISE EXCEPTION 'Motif d''entree introuvable';
    END IF;

    nouvelId := genererIdAnimal(p_dateEntree)::char(11);

    INSERT INTO ANIMAL (idAnimal, nom, type, sexe, sterilise, dateSterilisation,
                        dateNaissance, dateDeces, description, particularites)
    SELECT nouvelId, nom, type, sexe, sterilise, dateSterilisation,
           dateNaissance, NULL, description, particularites
      FROM ANIMAL
     WHERE idAnimal = p_idAnimalSource::char(11);

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Animal source introuvable';
    END IF;

    INSERT INTO ANIMAL_COULEUR (colIdentifiant, idAnimal)
    SELECT colIdentifiant, nouvelId
      FROM ANIMAL_COULEUR
     WHERE idAnimal = p_idAnimalSource::char(11)
    ON CONFLICT DO NOTHING;

    INSERT INTO ANI_COMPATIBILITE (valeur, description, idCompatibilite, idAnimal)
    SELECT DISTINCT ON (sourceCompat.idCompatibilite)
           sourceCompat.valeur,
           sourceCompat.description,
           sourceCompat.idCompatibilite,
           nouvelId
      FROM ANIMAL sourceAnimal
      JOIN ANIMAL animalCompatible
        ON animalCompatible.idAnimal = p_idAnimalSource::char(11)
        OR (
            animalCompatible.idAnimal <> nouvelId
            AND animalCompatible.nom = sourceAnimal.nom
            AND animalCompatible.type = sourceAnimal.type
            AND animalCompatible.sexe = sourceAnimal.sexe
            AND animalCompatible.dateNaissance IS NOT DISTINCT FROM sourceAnimal.dateNaissance
        )
      JOIN ANI_COMPATIBILITE sourceCompat
        ON sourceCompat.idAnimal = animalCompatible.idAnimal
     WHERE sourceAnimal.idAnimal = p_idAnimalSource::char(11)
     ORDER BY sourceCompat.idCompatibilite,
              CASE WHEN animalCompatible.idAnimal = p_idAnimalSource::char(11) THEN 0 ELSE 1 END,
              animalCompatible.idAnimal DESC
    ON CONFLICT DO NOTHING;

    INSERT INTO ANI_ENTREE (raison, dateEntree, idAnimal, entreeContact)
    VALUES (motif, p_dateEntree, nouvelId, p_idContact);

    RETURN nouvelId;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION consulterAnimal(p_idAnimal text)
RETURNS TABLE(
    idAnimal char(11), nom varchar, type varchar, sexe char(1), sterilise boolean,
    dateSterilisation date, dateNaissance date, dateDeces date,
    description text, particularites text,
    dateEntree date, motifEntree varchar, contactEntree text,
    dateSortie date, motifSortie varchar, contactSortie text
) AS $$
BEGIN
    RETURN QUERY
    SELECT a.idAnimal, a.nom, a.type, a.sexe, a.sterilise,
           a.dateSterilisation, a.dateNaissance, a.dateDeces,
           a.description, a.particularites,
           e.dateEntree, e.raison, e.contactEntree,
           s.dateSortie, s.raison, s.contactSortie
      FROM ANIMAL a
      LEFT JOIN LATERAL (
          SELECT ae.dateEntree, ae.raison,
                 NULLIF(trim(COALESCE(c.prenom, '') || ' ' || COALESCE(c.nom, '')), '') AS contactEntree
            FROM ANI_ENTREE ae
            LEFT JOIN CONTACT c ON c.idContact = ae.entreeContact
           WHERE ae.idAnimal = a.idAnimal
           ORDER BY ae.dateEntree DESC, ae.idEntree DESC
           LIMIT 1
      ) e ON true
      LEFT JOIN LATERAL (
          SELECT aso.dateSortie, aso.raison,
                 NULLIF(trim(COALESCE(c.prenom, '') || ' ' || COALESCE(c.nom, '')), '') AS contactSortie
            FROM ANI_SORTIE aso
            LEFT JOIN CONTACT c ON c.idContact = aso.sortieContact
           WHERE aso.idAnimal = a.idAnimal
           ORDER BY aso.dateSortie DESC, aso.idSortie DESC
           LIMIT 1
      ) s ON true
     WHERE a.idAnimal = p_idAnimal::char(11);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION supprimerAnimal(p_idAnimal text)
RETURNS integer AS $$
DECLARE nb integer;
BEGIN
    DELETE FROM ANIMAL WHERE idAnimal = p_idAnimal::char(11);
    GET DIAGNOSTICS nb = ROW_COUNT;
    RETURN nb;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION listerAnimaux()
RETURNS TABLE(
    idAnimal char(11), nom varchar, type varchar, sexe char(1), sterilise boolean,
    dateSterilisation date, dateNaissance date, dateDeces date,
    description text, particularites text,
    dateEntree date, motifEntree varchar, contactEntree text,
    dateSortie date, motifSortie varchar, contactSortie text
) AS $$
BEGIN
    RETURN QUERY
    SELECT a.idAnimal, a.nom, a.type, a.sexe, a.sterilise,
           a.dateSterilisation, a.dateNaissance, a.dateDeces,
           a.description, a.particularites,
           e.dateEntree, e.raison, e.contactEntree,
           s.dateSortie, s.raison, s.contactSortie
      FROM ANIMAL a
      LEFT JOIN LATERAL (
          SELECT ae.dateEntree, ae.raison,
                 NULLIF(trim(COALESCE(c.prenom, '') || ' ' || COALESCE(c.nom, '')), '') AS contactEntree
            FROM ANI_ENTREE ae
            LEFT JOIN CONTACT c ON c.idContact = ae.entreeContact
           WHERE ae.idAnimal = a.idAnimal
           ORDER BY ae.dateEntree DESC, ae.idEntree DESC
           LIMIT 1
      ) e ON true
      LEFT JOIN LATERAL (
          SELECT aso.dateSortie, aso.raison,
                 NULLIF(trim(COALESCE(c.prenom, '') || ' ' || COALESCE(c.nom, '')), '') AS contactSortie
            FROM ANI_SORTIE aso
            LEFT JOIN CONTACT c ON c.idContact = aso.sortieContact
           WHERE aso.idAnimal = a.idAnimal
           ORDER BY aso.dateSortie DESC, aso.idSortie DESC
           LIMIT 1
      ) s ON true
     ORDER BY a.idAnimal;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION listerAnimauxAuRefuge()
RETURNS TABLE(
    idAnimal char(11), nom varchar, type varchar, sexe char(1), sterilise boolean,
    dateSterilisation date, dateNaissance date, dateDeces date,
    description text, particularites text,
    dateEntree date, motifEntree varchar, contactEntree text,
    dateSortie date, motifSortie varchar, contactSortie text
) AS $$
BEGIN
    RETURN QUERY
    SELECT a.idAnimal, a.nom, a.type, a.sexe, a.sterilise,
           a.dateSterilisation, a.dateNaissance, a.dateDeces,
           a.description, a.particularites,
           e.dateEntree, e.raison, e.contactEntree,
           NULL::date, NULL::varchar, NULL::text
      FROM ANIMAL a
      LEFT JOIN LATERAL (
          SELECT ae.dateEntree, ae.raison,
                 NULLIF(trim(COALESCE(c.prenom, '') || ' ' || COALESCE(c.nom, '')), '') AS contactEntree
            FROM ANI_ENTREE ae
            LEFT JOIN CONTACT c ON c.idContact = ae.entreeContact
           WHERE ae.idAnimal = a.idAnimal
           ORDER BY ae.dateEntree DESC, ae.idEntree DESC
           LIMIT 1
      ) e ON true
     WHERE a.dateDeces IS NULL
       AND EXISTS (SELECT 1 FROM ANI_ENTREE e WHERE e.idAnimal = a.idAnimal)
       AND (SELECT COALESCE(MAX(e.dateEntree), DATE '1900-01-01')
              FROM ANI_ENTREE e WHERE e.idAnimal = a.idAnimal)
         > (SELECT COALESCE(MAX(s.dateSortie), DATE '1900-01-01')
              FROM ANI_SORTIE s WHERE s.idAnimal = a.idAnimal)
     ORDER BY a.idAnimal;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION modifierAnimal(
    p_idAnimal          text,
    p_nom               varchar,
    p_type              varchar,
    p_sexe              text,
    p_sterilise         boolean,
    p_dateSterilisation date,
    p_dateNaissance     date,
    p_dateDeces         date,
    p_description       text,
    p_particularites    text,
    p_couleurs          text[]
) RETURNS integer AS $$
DECLARE
    nb integer;
    c text;
    id_couleur integer;
BEGIN
    UPDATE ANIMAL
       SET nom = p_nom,
           type = p_type,
           sexe = p_sexe::char(1),
           sterilise = p_sterilise,
           dateSterilisation = p_dateSterilisation,
           dateNaissance = p_dateNaissance,
           dateDeces = p_dateDeces,
           description = p_description,
           particularites = p_particularites
     WHERE idAnimal = p_idAnimal::char(11);

    GET DIAGNOSTICS nb = ROW_COUNT;
    IF nb = 0 THEN
        RETURN 0;
    END IF;

    DELETE FROM ANIMAL_COULEUR WHERE idAnimal = p_idAnimal::char(11);
    IF p_couleurs IS NOT NULL THEN
        FOREACH c IN ARRAY p_couleurs LOOP
            IF length(trim(c)) > 0 THEN
                INSERT INTO COULEUR (nomCouleur)
                VALUES (trim(c))
                ON CONFLICT (nomCouleur) DO UPDATE SET nomCouleur = EXCLUDED.nomCouleur
                RETURNING colIdentifiant INTO id_couleur;

                INSERT INTO ANIMAL_COULEUR (colIdentifiant, idAnimal)
                VALUES (id_couleur, p_idAnimal::char(11))
                ON CONFLICT DO NOTHING;
            END IF;
        END LOOP;
    END IF;

    RETURN nb;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION modifierDescriptionParticularites(
    p_idAnimal text,
    p_description text,
    p_particularites text
) RETURNS integer AS $$
DECLARE nb integer;
BEGIN
    UPDATE ANIMAL
       SET description = COALESCE(p_description, description),
           particularites = COALESCE(p_particularites, particularites)
     WHERE idAnimal = p_idAnimal::char(11);
    GET DIAGNOSTICS nb = ROW_COUNT;
    RETURN nb;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION effacerDescription(p_idAnimal text)
RETURNS integer AS $$
DECLARE nb integer;
BEGIN
    UPDATE ANIMAL SET description = NULL WHERE idAnimal = p_idAnimal::char(11);
    GET DIAGNOSTICS nb = ROW_COUNT;
    RETURN nb;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION effacerParticularites(p_idAnimal text)
RETURNS integer AS $$
DECLARE nb integer;
BEGIN
    UPDATE ANIMAL SET particularites = NULL WHERE idAnimal = p_idAnimal::char(11);
    GET DIAGNOSTICS nb = ROW_COUNT;
    RETURN nb;
END;
$$ LANGUAGE plpgsql;

-- =============================================================
-- COULEURS
-- =============================================================
CREATE OR REPLACE FUNCTION ajouterCouleur(p_idAnimal text, p_couleur varchar)
RETURNS void AS $$
DECLARE id_couleur integer;
BEGIN
    INSERT INTO COULEUR (nomCouleur)
    VALUES (trim(p_couleur))
    ON CONFLICT (nomCouleur) DO UPDATE SET nomCouleur = EXCLUDED.nomCouleur
    RETURNING colIdentifiant INTO id_couleur;

    INSERT INTO ANIMAL_COULEUR (colIdentifiant, idAnimal)
    VALUES (id_couleur, p_idAnimal::char(11))
    ON CONFLICT DO NOTHING;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION supprimerCouleur(p_idAnimal text, p_couleur varchar)
RETURNS integer AS $$
DECLARE nb integer;
BEGIN
    DELETE FROM ANIMAL_COULEUR ac
     USING COULEUR c
     WHERE ac.colIdentifiant = c.colIdentifiant
       AND ac.idAnimal = p_idAnimal::char(11)
       AND c.nomCouleur = p_couleur;
    GET DIAGNOSTICS nb = ROW_COUNT;
    RETURN nb;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION listerCouleurs(p_idAnimal text)
RETURNS TABLE(couleur varchar) AS $$
BEGIN
    RETURN QUERY
    SELECT c.nomCouleur
      FROM ANIMAL_COULEUR ac
      JOIN COULEUR c ON c.colIdentifiant = ac.colIdentifiant
     WHERE ac.idAnimal = p_idAnimal::char(11)
     ORDER BY c.colIdentifiant;
END;
$$ LANGUAGE plpgsql;

-- =============================================================
-- COMPATIBILITES
-- =============================================================
CREATE OR REPLACE FUNCTION ajouterCompatibilite(
    p_type varchar,
    p_valeur varchar,
    p_description text,
    p_idAnimal text
) RETURNS integer AS $$
DECLARE
    id_type integer;
    id_ligne integer;
    val_bool boolean;
BEGIN
    SELECT idCompatibilite INTO id_type FROM COMPATIBILITE WHERE type = p_type;
    IF id_type IS NULL THEN
        RAISE EXCEPTION 'Type de compatibilite invalide: %', p_type;
    END IF;

    val_bool := CASE lower(trim(p_valeur))
        WHEN 'oui' THEN TRUE
        WHEN 'true' THEN TRUE
        WHEN '1' THEN TRUE
        WHEN 'non' THEN FALSE
        WHEN 'false' THEN FALSE
        WHEN '0' THEN FALSE
    END;

    IF val_bool IS NULL THEN
        RAISE EXCEPTION 'Valeur de compatibilite invalide: %', p_valeur;
    END IF;

    INSERT INTO ANI_COMPATIBILITE (valeur, description, idCompatibilite, idAnimal)
    VALUES (val_bool, p_description, id_type, p_idAnimal::char(11))
    RETURNING idAniCompatibilite INTO id_ligne;

    RETURN id_ligne;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION supprimerCompatibilite(p_idCompat integer)
RETURNS integer AS $$
DECLARE nb integer;
BEGIN
    DELETE FROM ANI_COMPATIBILITE WHERE idAniCompatibilite = p_idCompat;
    GET DIAGNOSTICS nb = ROW_COUNT;
    RETURN nb;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION listerCompatibilitesParAnimal(p_idAnimal text)
RETURNS TABLE(idCompat integer, type varchar, valeur varchar, description text, idAnimal char(11)) AS $$
BEGIN
    RETURN QUERY
    SELECT ac.idAniCompatibilite,
           c.type,
           CASE WHEN ac.valeur THEN 'oui' ELSE 'non' END::varchar,
           ac.description,
           ac.idAnimal
      FROM ANI_COMPATIBILITE ac
      JOIN COMPATIBILITE c ON c.idCompatibilite = ac.idCompatibilite
     WHERE ac.idAnimal = p_idAnimal::char(11)
     ORDER BY ac.idAniCompatibilite;
END;
$$ LANGUAGE plpgsql;

-- =============================================================
-- CONTACTS ET ROLES
-- =============================================================
CREATE OR REPLACE FUNCTION ajouterContact(
    p_nom varchar,
    p_prenom varchar,
    p_registreNational varchar,
    p_rue varchar,
    p_cp varchar,
    p_localite varchar,
    p_gsm varchar,
    p_telephone varchar,
    p_email varchar,
    p_roles text[]
) RETURNS integer AS $$
DECLARE
    nouvelId integer;
    role_nom text;
    role_id integer;
BEGIN
    INSERT INTO CONTACT (nom, prenom, registreNational, rue, cp, localite, gsm, telephone, email)
    VALUES (p_nom, p_prenom, p_registreNational, p_rue, p_cp, p_localite, p_gsm, p_telephone, p_email)
    RETURNING idContact INTO nouvelId;

    IF p_roles IS NOT NULL THEN
        FOREACH role_nom IN ARRAY p_roles LOOP
            SELECT rolIdentifiant INTO role_id FROM ROLE WHERE rolNom = role_nom;
            IF role_id IS NOT NULL THEN
                INSERT INTO PERSONNE_ROLE (idContact, rolIdentifiant)
                VALUES (nouvelId, role_id)
                ON CONFLICT DO NOTHING;
            END IF;
        END LOOP;
    END IF;

    RETURN nouvelId;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION consulterContact(p_idContact integer)
RETURNS TABLE(
    idContact integer, nom varchar, prenom varchar,
    registreNational varchar, rue varchar, cp varchar, localite varchar,
    gsm varchar, telephone varchar, email varchar, roles text[]
) AS $$
BEGIN
    RETURN QUERY
    SELECT c.idContact, c.nom, c.prenom, c.registreNational,
           c.rue, c.cp, c.localite, c.gsm, c.telephone, c.email,
           COALESCE((array_agg(r.rolNom ORDER BY r.rolNom)
                    FILTER (WHERE r.rolNom IS NOT NULL))::text[], ARRAY[]::text[])
      FROM CONTACT c
      LEFT JOIN PERSONNE_ROLE pr ON pr.idContact = c.idContact
      LEFT JOIN ROLE r ON r.rolIdentifiant = pr.rolIdentifiant
     WHERE c.idContact = p_idContact
     GROUP BY c.idContact, c.nom, c.prenom, c.registreNational,
              c.rue, c.cp, c.localite, c.gsm, c.telephone, c.email;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION listerContacts()
RETURNS TABLE(
    idContact integer, nom varchar, prenom varchar,
    registreNational varchar, rue varchar, cp varchar, localite varchar,
    gsm varchar, telephone varchar, email varchar, roles text[]
) AS $$
BEGIN
    RETURN QUERY
    SELECT c.idContact, c.nom, c.prenom, c.registreNational,
           c.rue, c.cp, c.localite, c.gsm, c.telephone, c.email,
           COALESCE((array_agg(r.rolNom ORDER BY r.rolNom)
                    FILTER (WHERE r.rolNom IS NOT NULL))::text[], ARRAY[]::text[])
      FROM CONTACT c
      LEFT JOIN PERSONNE_ROLE pr ON pr.idContact = c.idContact
      LEFT JOIN ROLE r ON r.rolIdentifiant = pr.rolIdentifiant
     GROUP BY c.idContact, c.nom, c.prenom, c.registreNational,
              c.rue, c.cp, c.localite, c.gsm, c.telephone, c.email
     ORDER BY c.idContact;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION registreNationalExiste(
    p_registreNational varchar,
    p_idContactAExclure integer DEFAULT NULL
) RETURNS boolean AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1
          FROM CONTACT
         WHERE registreNational = p_registreNational
           AND (p_idContactAExclure IS NULL OR idContact <> p_idContactAExclure)
    );
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION modifierContact(
    p_idContact integer,
    p_nom varchar,
    p_prenom varchar,
    p_registreNational varchar,
    p_rue varchar,
    p_cp varchar,
    p_localite varchar,
    p_gsm varchar,
    p_telephone varchar,
    p_email varchar,
    p_roles text[]
) RETURNS integer AS $$
DECLARE
    nb integer;
    role_nom text;
    role_id integer;
BEGIN
    UPDATE CONTACT
       SET nom = p_nom,
           prenom = p_prenom,
           registreNational = p_registreNational,
           rue = p_rue,
           cp = p_cp,
           localite = p_localite,
           gsm = p_gsm,
           telephone = p_telephone,
           email = p_email
     WHERE idContact = p_idContact;
    GET DIAGNOSTICS nb = ROW_COUNT;

    IF nb > 0 THEN
        DELETE FROM PERSONNE_ROLE WHERE idContact = p_idContact;
        IF p_roles IS NOT NULL THEN
            FOREACH role_nom IN ARRAY p_roles LOOP
                SELECT rolIdentifiant INTO role_id FROM ROLE WHERE rolNom = role_nom;
                IF role_id IS NOT NULL THEN
                    INSERT INTO PERSONNE_ROLE (idContact, rolIdentifiant)
                    VALUES (p_idContact, role_id)
                    ON CONFLICT DO NOTHING;
                END IF;
            END LOOP;
        END IF;
    END IF;

    RETURN nb;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION supprimerContact(p_idContact integer)
RETURNS integer AS $$
DECLARE nb integer;
BEGIN
    DELETE FROM CONTACT WHERE idContact = p_idContact;
    GET DIAGNOSTICS nb = ROW_COUNT;
    RETURN nb;
END;
$$ LANGUAGE plpgsql;

-- =============================================================
-- MOTIFS
-- =============================================================
CREATE OR REPLACE FUNCTION listerMotifsEntree()
RETURNS SETOF MOTIF_ENTREE AS $$
BEGIN
    RETURN QUERY SELECT * FROM MOTIF_ENTREE ORDER BY idMotifEntree;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION listerMotifsSortie()
RETURNS SETOF MOTIF_SORTIE AS $$
BEGIN
    RETURN QUERY SELECT * FROM MOTIF_SORTIE ORDER BY idMotifSortie;
END;
$$ LANGUAGE plpgsql;

-- =============================================================
-- SORTIES
-- =============================================================
CREATE OR REPLACE FUNCTION ajouterSortie(
    p_dateSortie date,
    p_idAnimal text,
    p_idMotifSortie integer,
    p_idContact integer
) RETURNS integer AS $$
DECLARE
    nouvelId integer;
    motif varchar;
BEGIN
    SELECT libelle INTO motif FROM MOTIF_SORTIE WHERE idMotifSortie = p_idMotifSortie;
    IF motif IS NULL THEN
        RAISE EXCEPTION 'Motif de sortie introuvable';
    END IF;

    INSERT INTO ANI_SORTIE (raison, dateSortie, idAnimal, sortieContact)
    VALUES (motif, p_dateSortie, p_idAnimal::char(11), p_idContact)
    RETURNING idSortie INTO nouvelId;

    IF motif = 'deces_animal' THEN
        UPDATE ANIMAL SET dateDeces = p_dateSortie WHERE idAnimal = p_idAnimal::char(11);
    END IF;

    RETURN nouvelId;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION supprimerDerniereSortie(
    p_idAnimal text,
    p_raison varchar
) RETURNS integer AS $$
DECLARE
    id_a_supprimer integer;
BEGIN
    SELECT idSortie INTO id_a_supprimer
      FROM ANI_SORTIE
     WHERE idAnimal = p_idAnimal::char(11)
       AND raison = p_raison
     ORDER BY dateSortie DESC, idSortie DESC
     LIMIT 1;

    IF id_a_supprimer IS NULL THEN
        RETURN 0;
    END IF;

    DELETE FROM ANI_SORTIE WHERE idSortie = id_a_supprimer;
    RETURN 1;
END;
$$ LANGUAGE plpgsql;

-- =============================================================
-- FAMILLES D'ACCUEIL
-- =============================================================
CREATE OR REPLACE FUNCTION ajouterAccueil(
    p_dateDebut date,
    p_dateFin date,
    p_idAnimal text,
    p_idContact integer
) RETURNS integer AS $$
DECLARE nouvelId integer;
BEGIN
    INSERT INTO FAMILLE_ACCUEIL (dateDebut, dateFin, idAnimal, idContact)
    VALUES (p_dateDebut, p_dateFin, p_idAnimal::char(11), p_idContact)
    RETURNING idAccueil INTO nouvelId;
    RETURN nouvelId;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION modifierDateFinAccueil(
    p_idAccueil integer,
    p_dateFin date
) RETURNS integer AS $$
DECLARE nb integer;
BEGIN
    UPDATE FAMILLE_ACCUEIL
       SET dateFin = p_dateFin
     WHERE idAccueil = p_idAccueil;
    GET DIAGNOSTICS nb = ROW_COUNT;
    RETURN nb;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION aDejaUnAccueilEnCours(p_idAnimal text)
RETURNS boolean AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1 FROM FAMILLE_ACCUEIL
         WHERE idAnimal = p_idAnimal::char(11) AND dateFin IS NULL
    );
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION aUnAccueilQuiChevauche(
    p_idAnimal text,
    p_debut date,
    p_fin date
) RETURNS boolean AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1
          FROM FAMILLE_ACCUEIL
         WHERE idAnimal = p_idAnimal::char(11)
           AND dateDebut <= COALESCE(p_fin, DATE '9999-12-31')
           AND COALESCE(dateFin, DATE '9999-12-31') >= p_debut
    );
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION listerAccueilsParAnimal(p_idAnimal text)
RETURNS TABLE(
    idAccueil integer, dateDebut date, dateFin date, idAnimal char(11),
    idContact integer, nomContact varchar, prenomContact varchar
) AS $$
BEGIN
    RETURN QUERY
    SELECT fa.idAccueil, fa.dateDebut, fa.dateFin, fa.idAnimal, fa.idContact,
           c.nom, c.prenom
      FROM FAMILLE_ACCUEIL fa
      JOIN CONTACT c ON c.idContact = fa.idContact
     WHERE fa.idAnimal = p_idAnimal::char(11)
     ORDER BY fa.idAccueil;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION listerAccueilsParFamille(p_idContact integer)
RETURNS TABLE(
    idAccueil integer, dateDebut date, dateFin date, idAnimal char(11),
    idContact integer, nomAnimal varchar
) AS $$
BEGIN
    RETURN QUERY
    SELECT fa.idAccueil, fa.dateDebut, fa.dateFin, fa.idAnimal, fa.idContact,
           a.nom
      FROM FAMILLE_ACCUEIL fa
      JOIN ANIMAL a ON a.idAnimal = fa.idAnimal
     WHERE fa.idContact = p_idContact
     ORDER BY fa.idAccueil;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION listerIdsAnimauxAvecAccueil()
RETURNS TABLE(idAnimal char(11)) AS $$
BEGIN
    RETURN QUERY
    SELECT DISTINCT fa.idAnimal
      FROM FAMILLE_ACCUEIL fa
     ORDER BY fa.idAnimal;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION listerIdsContactsAvecAccueil()
RETURNS TABLE(idContact integer) AS $$
BEGIN
    RETURN QUERY
    SELECT DISTINCT fa.idContact
      FROM FAMILLE_ACCUEIL fa
     ORDER BY fa.idContact;
END;
$$ LANGUAGE plpgsql;

-- =============================================================
-- ADOPTIONS
-- =============================================================
CREATE OR REPLACE FUNCTION ajouterAdoption(
    p_statut varchar,
    p_dateDemande date,
    p_idAnimal text,
    p_idContact integer
) RETURNS integer AS $$
DECLARE nouvelId integer;
BEGIN
    INSERT INTO ADOPTION (statut, dateDemande, idAnimal, idContact)
    VALUES (p_statut, p_dateDemande, p_idAnimal::char(11), p_idContact)
    RETURNING idAdoption INTO nouvelId;
    RETURN nouvelId;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION aDejaUneAdoptionAcceptee(p_idAnimal text)
RETURNS boolean AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1 FROM ADOPTION
         WHERE idAnimal = p_idAnimal::char(11) AND statut = 'acceptee'
    );
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION aDejaUneDemande(p_idAnimal text, p_idContact integer)
RETURNS boolean AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1 FROM ADOPTION
         WHERE idAnimal = p_idAnimal::char(11) AND idContact = p_idContact
    );
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION modifierStatutAdoption(p_idAdoption integer, p_statut varchar)
RETURNS integer AS $$
DECLARE nb integer;
BEGIN
    UPDATE ADOPTION SET statut = p_statut WHERE idAdoption = p_idAdoption;
    GET DIAGNOSTICS nb = ROW_COUNT;
    RETURN nb;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION listerAdoptions()
RETURNS TABLE(
    idAdoption integer, statut varchar, dateDemande date, idAnimal char(11),
    idContact integer, nomAnimal varchar, nomContact varchar, prenomContact varchar
) AS $$
BEGIN
    RETURN QUERY
    SELECT ad.idAdoption, ad.statut, ad.dateDemande, ad.idAnimal, ad.idContact,
           a.nom, c.nom, c.prenom
      FROM ADOPTION ad
      JOIN ANIMAL a ON a.idAnimal = ad.idAnimal
      JOIN CONTACT c ON c.idContact = ad.idContact
     ORDER BY ad.idAdoption;
END;
$$ LANGUAGE plpgsql;

-- =============================================================
-- VACCINS / VACCINATIONS
-- =============================================================
CREATE OR REPLACE FUNCTION ajouterVaccin(p_nomVaccin varchar)
RETURNS integer AS $$
DECLARE nouvelId integer;
BEGIN
    INSERT INTO VACCIN (nomVaccin)
    VALUES (p_nomVaccin)
    ON CONFLICT (nomVaccin) DO UPDATE SET nomVaccin = EXCLUDED.nomVaccin
    RETURNING idVaccin INTO nouvelId;
    RETURN nouvelId;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION supprimerVaccin(p_idVaccin integer)
RETURNS integer AS $$
DECLARE nb integer;
BEGIN
    DELETE FROM VACCIN WHERE idVaccin = p_idVaccin;
    GET DIAGNOSTICS nb = ROW_COUNT;
    RETURN nb;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION listerVaccins()
RETURNS SETOF VACCIN AS $$
BEGIN
    RETURN QUERY SELECT * FROM VACCIN ORDER BY idVaccin;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION vaccinationExiste(
    p_dateVaccin date,
    p_idAnimal text,
    p_idVaccin integer
) RETURNS boolean AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1
          FROM VACCINATION
         WHERE dateVaccin = p_dateVaccin
           AND idAnimal = p_idAnimal::char(11)
           AND idVaccin = p_idVaccin
    );
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION consulterVaccination(
    p_dateVaccin date,
    p_idAnimal text,
    p_idVaccin integer
) RETURNS TABLE(idVaccination integer, fait boolean) AS $$
BEGIN
    RETURN QUERY
    SELECT v.idVaccination, v.fait
      FROM VACCINATION v
     WHERE v.dateVaccin = p_dateVaccin
       AND v.idAnimal = p_idAnimal::char(11)
       AND v.idVaccin = p_idVaccin
     ORDER BY v.idVaccination
     LIMIT 1;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION modifierEtatVaccination(
    p_idVaccination integer,
    p_fait boolean
) RETURNS integer AS $$
DECLARE nb integer;
BEGIN
    UPDATE VACCINATION
       SET fait = p_fait
     WHERE idVaccination = p_idVaccination;
    GET DIAGNOSTICS nb = ROW_COUNT;
    RETURN nb;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION ajouterVaccination(
    p_dateVaccin date,
    p_fait boolean,
    p_idAnimal text,
    p_idVaccin integer
) RETURNS integer AS $$
DECLARE
    nouvelId integer;
    naissance date;
    vaccinationExistanteId integer;
    vaccinationExistanteFaite boolean;
BEGIN
    SELECT dateNaissance INTO naissance FROM ANIMAL WHERE idAnimal = p_idAnimal::char(11);
    IF naissance IS NOT NULL AND p_dateVaccin < naissance THEN
        RAISE EXCEPTION 'La date du vaccin est anterieure a la date de naissance';
    END IF;

    SELECT v.idVaccination, v.fait
      INTO vaccinationExistanteId, vaccinationExistanteFaite
      FROM VACCINATION v
     WHERE v.dateVaccin = p_dateVaccin
       AND v.idAnimal = p_idAnimal::char(11)
       AND v.idVaccin = p_idVaccin
     ORDER BY v.idVaccination
     LIMIT 1;

    IF vaccinationExistanteId IS NOT NULL THEN
        IF vaccinationExistanteFaite = false AND p_fait = true THEN
            UPDATE VACCINATION
               SET fait = true
             WHERE idVaccination = vaccinationExistanteId;
            RETURN vaccinationExistanteId;
        END IF;

        RAISE EXCEPTION 'Cet animal a deja une vaccination pour ce vaccin a cette date';
    END IF;

    INSERT INTO VACCINATION (dateVaccin, fait, idAnimal, idVaccin)
    VALUES (p_dateVaccin, p_fait, p_idAnimal::char(11), p_idVaccin)
    RETURNING idVaccination INTO nouvelId;
    RETURN nouvelId;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION listerVaccinationsParAnimal(p_idAnimal text)
RETURNS TABLE(
    idVaccination integer, dateVaccin date, fait boolean, idAnimal char(11),
    idVaccin integer, nomVaccin varchar
) AS $$
BEGIN
    RETURN QUERY
    SELECT vn.idVaccination, vn.dateVaccin, vn.fait,
           vn.idAnimal, vn.idVaccin, v.nomVaccin
      FROM VACCINATION vn
      JOIN VACCIN v ON v.idVaccin = vn.idVaccin
     WHERE vn.idAnimal = p_idAnimal::char(11)
     ORDER BY vn.idVaccination;
END;
$$ LANGUAGE plpgsql;

-- =============================================================
-- FIN
-- =============================================================
