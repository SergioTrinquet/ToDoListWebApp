# ToDoList Web App

Application web de gestion de tâches développée avec **ASP.NET Core MVC** et **Entity Framework Core**.  
Elle permet à un utilisateur authentifié de créer, modifier, supprimer et suivre l’état de ses tâches personnelles.

L’application intègre :
- un système d’authentification basé sur **ASP.NET Core Identity**
- une gestion des tâches par utilisateur
- un profil de démonstration permettant de visualiser le fonctionnement sans créer de compte

Ce projet a été réalisé dans un objectif de montée en compétences.

---

## 🛠️ Stack technique

- **ASP.NET Core MVC**
- **Entity Framework Core**
- **ASP.NET Core Identity**
- **SQL Server / LocalDB (développement)**
- **Azure App Service & Azure SQL (production)**

---

## 🚀 Lancer le projet en local

### Prérequis
L'appli a été développée avec la configuration suivante :
- .NET SDK 8.0
- Visual Studio 2022
- SQL Server Express / LocalDB

### Étapes

1. Cloner le dépôt :
   ```bash
   git clone https://github.com/SergioTrinquet/ToDoListWebApp.git
2. Changer le nom du fichier
   'appsettings.Development.json.example'
   en
   'appsettings.Development.json'

3. Adapter le nom de la base de données si nécessaire dans 'appsettings.Development.json' :
```
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=___YourDbNameHere___;Trusted_Connection=True;"
}
```

4. Lancer les migrations Entity Framework dans la console du gestionnaire de package :
```
Update-Database
```
5. Créer des données dans la base de données SQL Server locale:
	- Dans la table 'Priorite', créez 3 enregistrements avec dans la colonne 'Libelle' les valeurs 'basse' (Id = 1), 'moyenne' (Id = 2) et 'haute' (Id = 3),
	- Crée un utilisateur Demo en cliquant sur le menu 's'inscrire' sur l'app et créer un compte avec l'email se trouvant dans le fichier 'appsettings.Development.json', à savoir demo@monapp.test.
Quand l'utilisateur est créé, vérifiez l'enregistrement si besoin dans la table 'AspNetUsers'.

6. Lancer l'application depuis Visual Studio

