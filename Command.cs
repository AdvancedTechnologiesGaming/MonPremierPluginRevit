using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MonAddinRevit
{
    [Transaction(TransactionMode.Manual)]
    public class Commande : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Ouvrir la boîte de dialogue
            MaisonForm form = new MaisonForm(doc);
            if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK) return Result.Cancelled;

            double longueur = form.Longueur;
            double largeur = form.Largeur;
            double hauteur = form.Hauteur;
            FloorType floorType = form.SelectedFloorType;

            using (Transaction trans = new Transaction(doc, "Créer maison"))
            {
                trans.Start();

                // Créer niveaux
                Level niveau0 = Level.Create(doc, 0.0);
                Level niveau1 = Level.Create(doc, hauteur);

                // Points du rectangle
                XYZ p1 = new XYZ(0, 0, 0);
                XYZ p2 = new XYZ(longueur, 0, 0);
                XYZ p3 = new XYZ(longueur, largeur, 0);
                XYZ p4 = new XYZ(0, largeur, 0);

                // Murs
                Wall.Create(doc, Line.CreateBound(p1, p2), niveau0.Id, false);
                Wall.Create(doc, Line.CreateBound(p2, p3), niveau0.Id, false);
                Wall.Create(doc, Line.CreateBound(p3, p4), niveau0.Id, false);
                Wall.Create(doc, Line.CreateBound(p4, p1), niveau0.Id, false);

                // Toit plat (plancher)
                CurveLoop loop = new CurveLoop();
                loop.Append(Line.CreateBound(p1, p2));
                loop.Append(Line.CreateBound(p2, p3));
                loop.Append(Line.CreateBound(p3, p4));
                loop.Append(Line.CreateBound(p4, p1));

                List<CurveLoop> loops = new List<CurveLoop> { loop };
                Floor floor = Floor.Create(doc, loops, floorType.Id, niveau1.Id);

                trans.Commit();
            }

            TaskDialog.Show("Maison", "Maison générée avec succès !");
            return Result.Succeeded;
        }
    }
}
