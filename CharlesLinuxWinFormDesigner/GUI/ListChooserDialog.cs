using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI
{
    /// <summary>
    /// Dialog générale pour demander à l'user de sélectionner une option parmis d'autres.
    /// </summary>
    public class ListChooserDialog
    {




        public string Message = "no message";



        #region Gestion de la liste des choix

        private List<ListChooserDialogEntry> ChoiceList = new List<ListChooserDialogEntry>();

        //utiliser ces méthodes pour ajouter des options au dialogue
        public ListChooserDialogEntry Add(ListChooserDialogEntry lcde)
        {
            this.ChoiceList.Add(lcde);
            return lcde;
        }
        public ListChooserDialogEntry Add(string DisplayText)
        {
            ListChooserDialogEntry lcde = new ListChooserDialogEntry(DisplayText);
            this.ChoiceList.Add(lcde);
            return lcde;
        }
        public ListChooserDialogEntry Add(string DisplayText, object Tag)
        {
            ListChooserDialogEntry lcde = new ListChooserDialogEntry(DisplayText, Tag);
            this.ChoiceList.Add(lcde);
            return lcde;
        }


        #endregion



        public ListChooserDialog()
        {


        }

        #region Events





        #endregion

        private ListChooserDialogEntry ReturnValue = null; //valeur de retour de notre dialogue.



        public ListChooserDialogEntry ShowDialog()
        {
            return this.ShowDialog(null);
        }

        /// <summary>
        /// Affiche le dialogue et retourne le ListChooserDialogEntry que l'user a sélectionné, ou null si l'user annule.
        /// </summary>
        /// <returns>The dialog.</returns>
        /// <param name="CenterForm">Form sur laquelle on va se center. mettre null pour ne pas se centrer sur une form.</param>
        public ListChooserDialogEntry ShowDialog(Form CenterForm)
        {
            //on crée le gui
            Form formMain = new Form();
            formMain = new Form();
            formMain.Size = new Size(400, 400);

            //on centre la fenêtre sur une autre fenêtre
            if (CenterForm != null)
            {
                formMain.StartPosition = FormStartPosition.Manual;
                formMain.Top = CenterForm.Top + (CenterForm.Height / 2) - (formMain.Height / 2);
                formMain.Left = CenterForm.Left + (CenterForm.Width / 2) - (formMain.Width / 2);
            }


            Label lblMessage = new Label();
            lblMessage.Parent = formMain;
            lblMessage.Dock = DockStyle.Top;
            lblMessage.Height = 75;
            lblMessage.Text = this.Message;



            ListBox lbChoices = new ListBox(); //on crée la variable de notre liste avant les bouton car btnValidate a besoin de récupérer l'index de l'élément actuellement sélectionné

            //la procédure qui valide le choix actuel et ferme la fenêtre, ShowDialog va retourner la valeur sélectionné
            void Validate(object sender, EventArgs e)
            {
                //on récupère l'index actuellement sélectionné
                int selectedIndex = lbChoices.SelectedIndex;
                //on make sure qu'il y a un index actuellemen sélectionné
                if (selectedIndex >= 0)
                {
                    //ou fait en sorte que ShowDialog return l'élément sélectionné.
                    this.ReturnValue = this.ChoiceList[selectedIndex];
                    formMain.Close();
                }
                //il n'y a aucun index sélectionné
                else
                {
                    MessageBox.Show("Vous devez sélectionner un choix.");
                }
            }


            //les boutons
            Button btnValidate = new Button();
            btnValidate.Parent = formMain;
            btnValidate.Size = new Size(100, 35);
            btnValidate.Text = "Valider";
            btnValidate.Top = formMain.ClientSize.Height - 5 - btnValidate.Height;
            btnValidate.Left = formMain.ClientSize.Width - 5 - btnValidate.Width;
            btnValidate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnValidate.Click += Validate;

            Button btnCancel = new Button();
            btnCancel.Parent = formMain;
            btnCancel.Size = btnValidate.Size;
            btnCancel.Text = "Annuler";
            btnCancel.Top = btnValidate.Top;
            btnCancel.Left = btnValidate.Left - 5 - btnCancel.Width;
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Click += (sender, e) =>
            {
                this.ReturnValue = null;
                formMain.Close();
            };



            //on crée la liste
            lbChoices.Parent = formMain;
            lbChoices.Left = 0;
            lbChoices.Top = lblMessage.Top + lblMessage.Height;
            lbChoices.Width = formMain.ClientSize.Width;
            lbChoices.Height = btnCancel.Top - lbChoices.Top;
            lbChoices.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            lbChoices.SelectionMode = SelectionMode.One;
            lbChoices.DoubleClick += Validate;
            lbChoices.ItemHeight = 20;


            //on ajoute les choix
            foreach (ListChooserDialogEntry lcde in this.ChoiceList)
            {
                lbChoices.Items.Add(lcde.DisplayText);
            }


            //on affiche la form. les contrôles vont mettre dans this.ReturnValue la valeur de retour correcte.
            formMain.ShowDialog();

            return this.ReturnValue;
        }




    }
}
