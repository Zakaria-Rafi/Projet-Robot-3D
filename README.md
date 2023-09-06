# Projet-Robot-3D
Objectif du projet
Le but de ce projet est de développer un logiciel permettant d’obtenir des images 3D des massifs de sols ou objets placés sous le robot embarqué en centrifugeuse. Cela permettra aux chercheurs des réaliser des images 3D « en vol » des massifs de sols embarqués dans la centrifugeuse : c’est pour le moment le seul moyen identifier pour scanner des objets en macro-gravité sous une accélération de 100G.
 Pour cela nous disposons d’un capteur profil laser installé sur la tête du robot qui fournit des profils orientés selon l’axe Y : il nous fournira donc les coordonnées X et Z de points de chaque profil. Pour obtenir les coordonnée Y, nous allons déplacer ce capteur laser avec le robot, a vitesse régulière selon l’axe Y. La position Y du robot est mesurée par un capteur connecté à une chaine d’acquisition QuantumX de chez HBM. Le logiciel devra donc récupérer d’une part les coordonnés Y de l’image en interrogeant la chaine d’acquisition, et d’autre part les coordonnées X et Z en interrogeant le capteur laser profil. Il devra ensuite synchroniser ces données pour créer un nuage de points 3D

Les fonctionnalités attendues de ce logiciel sont les suivantes : 
-	Enregistrement des coordonnées X, Y et Z des images 3D lors du déplacement du robot qui va balayer le massif qui dispose de son propre logiciel de pilotage. Les fichiers produits doivent pouvoir être exploités sous Matlab qui dispose de fonctions graphiques simples pour tracer des images 3D à partir d’une matrice de points,
-	Visualisation en temps réel de l’image 3D pendant le balayage du massif par le robot au fur et a mesure que logiciel récupère les points de mesures.
-	Lecture des fichiers enregistrés pour tracer l’image 3D contenue dans le fichier sur l’interface temps réel du logiciel
-	Si possible mise en place d’outils simples de mesures dimensionnelles sur l’image visualisée.

Étape 1 : Connexion du laser et récupération des données 
	Développer une fonctionnalité dans l'application permettant de se connecter au laser Baumer.
	Implémenter la capacité de l'application à récupérer les données du laser, notamment les profils en X et Z.
Étape 2 : Établissement d'une connexion avec le quantum HBM
	Intégrer la fonctionnalité permettant à l'application de se connecter au quantum HBM.
	Assurer la récupération de la valeur de sortie spécifique requise par l'application depuis le quantum.
Étape 3 : Génération de solutions 3D
	Développer la routine permettant de générer une image 3D à partir des données recueillies.
	Permettre à l'application de tracer en temps réel les données du laser sur le modèle 3D, ou de le faire à partir de données enregistrées après la sauvegarde.





MATLAB : 
![untitled](https://github.com/Zakaria-Rafi/Projet-robot-Labo/assets/124291570/c26bcf3e-cb68-4f61-9752-6493933d5c6a)



logiciel : 
![exemple](https://github.com/Zakaria-Rafi/Projet-robot-Labo/assets/124291570/226d4582-34d6-4527-8c64-7d16291e58d2)


