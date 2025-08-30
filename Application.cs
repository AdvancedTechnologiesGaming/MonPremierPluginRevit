using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System;
using System.Reflection;

namespace MonAddinRevit
{
    public class Application : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            // Créer un onglet personnalisé
            string tabName = "Florian Software";
            try { application.CreateRibbonTab(tabName); } catch { }

            // Créer un panneau
            RibbonPanel panel = application.CreateRibbonPanel(tabName, "Maison");

            // Définir le chemin de l’assembly
            string path = Assembly.GetExecutingAssembly().Location;

            // Créer un bouton qui appelle Commande.cs
            PushButtonData buttonData = new PushButtonData(
                "GenererMaison",
                "Générer\nMaison",
                path,
                "MonAddinRevit.Commande" // ← lien vers ta classe
            );

            panel.AddItem(buttonData);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
