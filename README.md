Prodige Party 2
==============

**Prodige Party 2** est un projet de jeu vid?o, multijoueur en ligne.
Le principe du jeu est le suivant:
 - **Deux equipes** adverses doivent, dans un donjon, trouver le tr?sor et r?ussir ? sortir avec avant le temps imparti.
 - Il faut sortir avec le tr?sor pour gagner, co?te que co?te, quitte ? le voler ? l'?quipe adverse (c'est l'id?e principale)
 - Tout cela en ?vitant les pi?ges du donjon et en ?vitant de trop se faire tuer par l'?quipe adverse.

> **Note:**
> ###### TODO
> - * testing
> - * playmobile -> stats + skillshots
> - * map "test" (+ trouver un nom)
> - * Cam déplacer cam
> - * Interfaces 2D

#### La branche courante de d?veloppement est "**dev**":
```
// pour aller sur la branche dev
git checkout dev_unity
```

---------

Syst?me de jeu:
----------------

 - Univers m?di?val fantastique
 - No snowball
 - De l'or de l'or de Lor
 - Nombre de joueur par ?quipe: undefined yet

##### Stats:
 - Attack, Def, Health
 - Pas de mana, just cooldown
 - Regene en dormant (with huge cooldown)
 - Crits et crits de d?gats
 - Fumble
 - attack speed
 - Brouillard de guerre
 - (Poids)
 - L?gere diff?rence de vitesse
 - Attributs vision

##### Systeme de vis?e et cam?ra
 - Cam?ra 3prs diagonale
 - Cam?ra qui peut tourner autour
 - Perso d?focus/focus
 - Click gauche pour d?placer le perso
 - Click droit pour tourner la cam?ra
 - Smart Cast always
 - Sorts in?vitable

##### Sorts: Ecoles de sorts (1 parmi 3 pour chaque cat?gorie)
 - Tous les sorts ont le meme prix sauf l'ulti
 - Combien de sorts au total: 5
 - Combien d'?cole en tout : 3 
 - Combien d'?cole possible : 2
 - Combien de sorts par ?cole : 4

##### Item de map
 - Pas d'item ingame achetable.
 - Item en d?but partie
 - 2 sp?cialit?s possibles, par perso, mais une seule ? la fois
