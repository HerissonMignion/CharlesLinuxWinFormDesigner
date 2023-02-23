using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeListBox : FakeControl
    {
        public FakeListBox() : base()
        {
            this.ClassName = "ListBox";
            this.ListProperties.Add(new FakeProperty("SelectionMode", typeof(SelectionMode), SelectionMode.One, this));
        }

        public override void Draw(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on make sure qu'on est visible
            if (this.Visible)
            {
                //on obtient notre position et taille
                Rectangle UpLeftSize = this.GetScreenPos();

                //on remplit la couleur de l'arrière plan
                Brush BackBrush = new SolidBrush((Color)(this.GetProperty("BackColor")));
                g.FillRectangle(BackBrush, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
                BackBrush.Dispose();

                //on essaye de montrer à l'user un apperçu de la taille finale du ListBox.
                //"essaye" parce que notre formule n'est qu'une aproximation. windows ou mono semble avoir un algo bizarre pour décider la taille finale des ListBox, et il y a toujours de petites différence de +1 ou -1 entre la taille réelle et la taille aproximé que je ne suis pas capable de supprimer.
                //on essaye de prédire la taille des incréments



                //on dessine la bordure
                g.DrawRectangle(Pens.Black, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
            }
        }

    }
}
