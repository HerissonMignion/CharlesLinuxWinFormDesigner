using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI
{
    public class AnchorStyleChooserDialog
    {





        public AnchorStyleChooserDialog()
        {
        }



        /// <summary>
        /// Valeur indiqué par l'user
        /// </summary>
        public AnchorStyles Value = AnchorStyles.None;



        public void ShowDialog()
        {
            this.ShowDialog(null);
        }

        /// <summary>
        /// Affiche le dialogue et retourne le anchor style sélectionné par l'user
        /// </summary>
        /// <returns>The dialog.</returns>
        /// <param name="CenterForm">Form sur laquelle on va se center. mettre null pour ne pas se centrer sur une form.</param>
        public void ShowDialog(Form CenterForm)
        {
            //on crée le gui
            Form FormMain = new Form();
            FormMain = new Form();
            FormMain.Size = new Size(400, 400);
            FormMain.Text = "Choisir un AnchorStyles";
            FormMain.BackColor = Color.FromArgb(32, 32, 32);

            //on centre la fenêtre sur une autre fenêtre
            if (CenterForm != null)
            {
                FormMain.StartPosition = FormStartPosition.Manual;
                FormMain.Top = CenterForm.Top + (CenterForm.Height / 2) - (FormMain.Height / 2);
                FormMain.Left = CenterForm.Left + (CenterForm.Width / 2) - (FormMain.Width / 2);
            }


            //top
            Button TopButton = new Button();
            TopButton.Parent = FormMain;
            TopButton.Size = new Size(100, 75);
            TopButton.Top = 5;
            TopButton.Left = (FormMain.ClientSize.Width / 2) - (TopButton.Width / 2);
            TopButton.Anchor = AnchorStyles.Top;

            //bottom
            Button BottomButton = new Button();
            BottomButton.Parent = FormMain;
            BottomButton.Size = TopButton.Size;
            BottomButton.Top = FormMain.ClientSize.Height - 5 - BottomButton.Height;
            BottomButton.Left = TopButton.Left;
            BottomButton.Anchor = AnchorStyles.Bottom;

            //left
            Button LeftButton = new Button();
            LeftButton.Parent = FormMain;
            LeftButton.Size = TopButton.Size;
            LeftButton.Left = 5;
            LeftButton.Top = (FormMain.ClientSize.Height / 2) - (LeftButton.Height / 2);
            LeftButton.Anchor = AnchorStyles.Left;

            //right
            Button RightButton = new Button();
            RightButton.Parent = FormMain;
            RightButton.Size = TopButton.Size;
            RightButton.Left = FormMain.ClientSize.Width - 5 - RightButton.Width;
            RightButton.Top = LeftButton.Top;
            RightButton.Anchor = AnchorStyles.Right;



            void RefreshText()
            {
                TopButton.BackColor = this.Value.HasFlag(AnchorStyles.Top) ? Color.DimGray : Color.Black;
                TopButton.Text = this.Value.HasFlag(AnchorStyles.Top) ? "Top" : "";

                BottomButton.BackColor = this.Value.HasFlag(AnchorStyles.Bottom) ? Color.DimGray : Color.Black;
                BottomButton.Text = this.Value.HasFlag(AnchorStyles.Bottom) ? "Bottom" : "";

                LeftButton.BackColor = this.Value.HasFlag(AnchorStyles.Left) ? Color.DimGray : Color.Black;
                LeftButton.Text = this.Value.HasFlag(AnchorStyles.Left) ? "Left" : "";

                RightButton.BackColor = this.Value.HasFlag(AnchorStyles.Right) ? Color.DimGray : Color.Black;
                RightButton.Text = this.Value.HasFlag(AnchorStyles.Right) ? "Right" : "";
            }


            TopButton.Click += (sender, e) =>
            {
                this.Value ^= AnchorStyles.Top;
                RefreshText();
            };

            BottomButton.Click += (sender, e) =>
            {
                this.Value ^= AnchorStyles.Bottom;
                RefreshText();
            };

            LeftButton.Click += (sender, e) =>
            {
                this.Value ^= AnchorStyles.Left;
                RefreshText();
            };

            RightButton.Click += (sender, e) =>
            {
                this.Value ^= AnchorStyles.Right;
                RefreshText();
            };



            //on affiche la form. les contrôles vont mettre dans this.Value la valeur de retour correcte.
            RefreshText();
            FormMain.ShowDialog();

        }



    }
}
