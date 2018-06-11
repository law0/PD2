Prodige Party 2
==============

**Prodige Party 2** est un projet de jeu vidéo, multijoueur en ligne.
Le principe du jeu est le suivant:
 - **Deux equipes** adverses doivent, dans un donjon, trouver le trésor et réussir à sortir avec avant le temps imparti.
 - Il faut sortir avec le trésor pour gagner, coéte que coéte, quitte a le voler à l'équipe adverse (c'est l'idée principale)
 - Tout cela en évitant les piéges du donjon et en évitant de trop se faire tuer par l'équipe adverse.

> **Note:**
> ###### TODO
> - * testing
> - * playmobile -> stats + skillshots
> - * map "test" (+ trouver un nom)
> - * Cam déplacer cam
> - * Interfaces 2D

#### La branche courante de développement est "**dev_v1.0.0**":
```
// pour aller sur la branche dev
git checkout dev_v1.0.0
```

---------

Systéme de jeu:
----------------

 - Univers médiéval fantastique
 - No snowball
 - De l'or de l'or de Lor
 - Nombre de joueur par équipe: undefined yet

##### Stats:
 - Attack, Def, Health
 - Pas de mana, just cooldown
 - Regene en dormant (with huge cooldown)
 - Crits et crits de dégats
 - Fumble
 - attack speed
 - Brouillard de guerre
 - (Poids)
 - Légere différence de vitesse
 - Attributs vision

##### Systeme de visée et caméra
 - Caméra 3prs diagonale
 - Caméra qui peut tourner autour
 - Perso défocus/focus
 - Click gauche pour déplacer le perso
 - Click droit pour tourner la caméra
 - Smart Cast always
 - Sorts inévitable

##### Sorts: Ecoles de sorts (1 parmi 3 pour chaque catégorie)
 - Tous les sorts ont le meme prix sauf l'ulti
 - Combien de sorts au total: 5
 - Combien d'école en tout : 3 
 - Combien d'école possible : 2
 - Combien de sorts par école : 4

##### Item de map
 - Pas d'item ingame achetable.
 - Item en début partie
 - 2 spécialités possibles, par perso, mais une seule é la fois
