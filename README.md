# Gaucho Fleches

Ce projet est un logiciel visant à faciliter la création de mots-flechés. Il est développé en C# avec Unity.

## Disclaimer

Ce projet est en cours de développement. Il est actuellement utilisable, mais la plupart des fonctionnalités ne sont pas encore implémentées. De plus, l'interface utilisateur est encore en cours de développement et peut être améliorée. Il n'y a pour le moment pas de génération automatique de mots.

## Installation

- [Build v1.0.2](https://github.com/louisaiva/gaucho-fleches/releases/tag/v1.0.2) disponible -> téléchargement du fichier .zip, extraction et lancement de l'exécutable "gaucho-fleches.exe"
- ou alors clonage de ce repo et ouverture du projet avec Unity 6 (6000.0.28f1)

## Utilisation

Lancez le logiciel. Vous pouvez ouvrir une grille existante ou en créer une nouvelle. Pour créer une nouvelle grille, choisissez les dimensions de la grille et cliquez sur "Créer grille".

### Barre d'outils - Edition de grille
- Bouton quitter
- Bouton sauvegarde
- Bouton export

### Raccourcis souris - Edition de grille
- Clic gauche sur une case/definition : la sélectionner (ensuite clavier pour y entrer une lettre)
- Clic gauche sur une ligne : grossir/retrecir la ligne (pour remplacer un tiret dans un mot)
- Clic droit sur une case : basculer entre case normale et case définition
- molette : zoomer
- bouton de la molette : remettre le zoom à zéro
- LeftAlt + clic gauche : déplacement fluide de la grille

### Raccourcis clavier - Edition de grille
- fleches (bas, haut, gauche, droite) : naviguer dans la grille (après avoir cliqué sur une case)
- tab : basculer entre case normale et case définition
- entrée : déselectionner la case
- retour arrière : effacer le contenu de la case
- lettre : entrer une lettre dans la case / écrire dans la définition
- suppr : effacer le contenu de la case / effacer une définition
- LeftAlt + clic gauche : déplacement fluide de la grille

## Suivi de développement

- [x] Création / Edition de grilles
- [x] Sauvegarde / Chargement de grilles
- [x] Export de la grille au format pdf
- [ ] Gestion de dictionnaires
- [ ] Génération automatique de grilles basées sur des listes de mots.

(voir le fichier [todo](./todo) pour plus de détails)