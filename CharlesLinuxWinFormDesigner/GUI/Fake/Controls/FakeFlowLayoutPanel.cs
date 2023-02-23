using System;
using System.Drawing;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeFlowLayoutPanel : FakeControlContainer
    {



        public FakeFlowLayoutPanel() : base()
        {
            this.ClassName = "FlowLayoutPanel";
            this.ListProperties.Add(new FakeProperty("BorderStyle", typeof(System.Windows.Forms.BorderStyle), System.Windows.Forms.BorderStyle.None, this));
            this.ListProperties.Add(new FakeProperty("FlowDirection", typeof(System.Windows.Forms.FlowDirection), System.Windows.Forms.FlowDirection.LeftToRight, this));

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
