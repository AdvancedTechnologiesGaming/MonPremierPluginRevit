using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;

namespace MonAddinRevit
{
    public class MaisonForm : System.Windows.Forms.Form
    {
        // Valeurs publiques récupérables après DialogResult.OK
        public double Longueur { get; private set; }
        public double Largeur { get; private set; }
        public double Hauteur { get; private set; }
        public FloorType SelectedFloorType { get; private set; }

        // Controls
        private TextBox longueurBox;
        private TextBox largeurBox;
        private TextBox hauteurBox;
        private ComboBox comboFloor;
        private Button okButton;
        private Button cancelButton;

        public MaisonForm(Document doc)
        {
            // Propriétés de la fenêtre
            this.Text = "Paramètres Maison";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new System.Drawing.Size(420, 220);

            // Labels et TextBoxes
            var lblLong = new Label { Text = "Longueur (m):", Left = 20, Top = 20, AutoSize = true };
            longueurBox = new TextBox { Left = 160, Top = 16, Width = 120, Text = "10" };

            var lblLarg = new Label { Text = "Largeur (m):", Left = 20, Top = 55, AutoSize = true };
            largeurBox = new TextBox { Left = 160, Top = 51, Width = 120, Text = "8" };

            var lblH = new Label { Text = "Hauteur murs (m):", Left = 20, Top = 90, AutoSize = true };
            hauteurBox = new TextBox { Left = 160, Top = 86, Width = 120, Text = "3" };

            // ComboBox pour FloorType
            var lblFloor = new Label { Text = "Type de plancher :", Left = 20, Top = 130, AutoSize = true };
            comboFloor = new ComboBox { Left = 160, Top = 126, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };

            // Récupérer FloorType du document
            var floorTypes = new FilteredElementCollector(doc)
                .OfClass(typeof(FloorType))
                .Cast<FloorType>()
                .ToList();

            if (floorTypes.Count == 0)
            {
                // Si aucun type trouvé, on ajoute un item vide pour éviter crash
                comboFloor.Items.Add("Aucun type de plancher trouvé");
                comboFloor.Enabled = false;
            }
            else
            {
                foreach (var ft in floorTypes) comboFloor.Items.Add(ft);
                comboFloor.DisplayMember = "Name";
                comboFloor.SelectedIndex = 0;
            }

            // Boutons
            okButton = new Button { Text = "OK", Left = 160, Top = 170, Width = 100 };
            cancelButton = new Button { Text = "Annuler", Left = 280, Top = 170, Width = 100 };

            okButton.Click += (s, e) =>
            {
                // Validation et parsing (culture invariant pour le point décimal)
                if (!TryParseDoubleInvariant(longueurBox.Text, out double L))
                {
                    MessageBox.Show("Longueur invalide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!TryParseDoubleInvariant(largeurBox.Text, out double W))
                {
                    MessageBox.Show("Largeur invalide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!TryParseDoubleInvariant(hauteurBox.Text, out double H))
                {
                    MessageBox.Show("Hauteur invalide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Longueur = L;
                Largeur = W;
                Hauteur = H;

                if (comboFloor.Enabled && comboFloor.SelectedItem is FloorType ftSel)
                {
                    SelectedFloorType = ftSel;
                }
                else
                {
                    SelectedFloorType = null;
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            cancelButton.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            // Ajout des controls à la fenêtre
            this.Controls.Add(lblLong);
            this.Controls.Add(longueurBox);
            this.Controls.Add(lblLarg);
            this.Controls.Add(largeurBox);
            this.Controls.Add(lblH);
            this.Controls.Add(hauteurBox);
            this.Controls.Add(lblFloor);
            this.Controls.Add(comboFloor);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);

            // Raccourcis
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private bool TryParseDoubleInvariant(string s, out double value)
        {
            // Accepte "10.5" ou "10,5" selon culture
            s = s.Trim();
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                return true;
            // essai avec current culture
            return double.TryParse(s, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
        }
    }
}
