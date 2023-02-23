using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeComboBox : FakeControl
    {

        //le ComboBox override cette méthode parce qu'ils ne sont pas toujours affectés par la propriété Height
        public override Rectangle GetScreenPos()
        {
            Rectangle rep = base.GetScreenPos();

            //les combo box, on ne peut pas contrôler leur hauteur quand leur style n'est pas "simple"
            if ((ComboBoxStyle)(this.GetProperty("DropDownStyle")) != ComboBoxStyle.Simple)
            {
                rep.Height = (int)(7.5f + (1.558f * ((Font)(this.GetProperty("Font"))).Size));
            }

            return rep;
        }

        public FakeComboBox() : base()
        {
            this.ClassName = "ComboBox";
            this.ListProperties.Add(new FakeProperty("DropDownStyle", typeof(ComboBoxStyle), ComboBoxStyle.DropDownList, this));
            this.ListProperties.Add(new FakeProperty("SelectedIndex", typeof(int), -1, this));
        }

        public override void Draw(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on make sure qu'on est visible
            if (this.Visible)
            {
                //on obtient notre position et notre taille graphique
                Rectangle UpLeftSize = this.GetScreenPos();

                //on remplit l'arrière plan
                Brush BackBrush = new SolidBrush((Color)(this.GetProperty("BackColor")));
                g.FillRectangle(BackBrush, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
                BackBrush.Dispose();

                //ici on dessinerait la "valeur" actuellement sélectionné, mais on ne suporte pas de faire les options du combo box, alors il n'y a rien de sélectionné.

                //on dessine un apperçu de la flèche à droite du combo box
                int arrowAreaWidth = 15;
                //on remplit l'arrière plan
                g.FillRectangle(Brushes.Silver, UpLeftSize.X + UpLeftSize.Width - arrowAreaWidth, UpLeftSize.Y, arrowAreaWidth, UpLeftSize.Height);
                //on dessine la ligne qui sépare la flèche et la zone de la valeur
                g.DrawLine(Pens.Black, UpLeftSize.X + UpLeftSize.Width - arrowAreaWidth, UpLeftSize.Y, UpLeftSize.X + UpLeftSize.Width - arrowAreaWidth, UpLeftSize.Y + UpLeftSize.Height);

                //on dessine la bordure
                g.DrawRectangle(Pens.Black, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
            }
        }

    }
}
