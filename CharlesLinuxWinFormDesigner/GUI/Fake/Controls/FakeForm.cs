using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeForm : FakeControlContainer
    {
        public FakeForm() : base()
        {
            this.ClassName = "Form";
        }

        public override void Draw(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on make sure qu'on est visible
            if (this.Visible)
            {
                //on obtient notre position et taille graphique
                Rectangle UpLeftSize = this.GetScreenPos();
                //on remplit l'arrière plan
                Brush BackBrush = new SolidBrush((Color)(this.GetProperty("BackColor")));
                g.FillRectangle(new SolidBrush((Color)(this.GetProperty("BackColor"))), UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
                BackBrush.Dispose();

                //on fait dessiner nos enfants
                this.DrawChildren(img, g, fcdc);
            }
        }
    }
}
