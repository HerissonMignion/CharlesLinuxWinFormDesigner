using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakePanel : FakeControlContainer
    {
        public FakePanel() : base()
        {
            this.ClassName = "Panel";
            this.ListProperties.Add(new FakeProperty("BorderStyle", typeof(System.Windows.Forms.BorderStyle), System.Windows.Forms.BorderStyle.None, this));
        }

        public override void Draw(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on make sure qu'on est visible
            if (this.Visible)
            {
                //on obtient notre position et notre taille
                Rectangle UpLeftSize = this.GetScreenPos();

                //on remplit la couleur de l'arrière plan
                Brush BackBrush = new SolidBrush((Color)(this.GetProperty("BackColor")));
                g.FillRectangle(BackBrush, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
                BackBrush.Dispose();

                //on dessine la bordure
                g.DrawRectangle(Pens.DimGray, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);

                //on fait dessiner nos enfants
                this.DrawChildren(img, g, fcdc);
            }
        }

    }
}
