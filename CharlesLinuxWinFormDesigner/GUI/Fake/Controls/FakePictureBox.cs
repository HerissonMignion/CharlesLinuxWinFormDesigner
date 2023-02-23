using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakePictureBox : FakeControl
    {
        public FakePictureBox() : base()
        {
            this.ClassName = "PictureBox";
            this.ListProperties.Add(new FakeProperty("BackgroundImageLayout", typeof(System.Windows.Forms.ImageLayout), System.Windows.Forms.ImageLayout.Zoom, this));
        }

        public override void Draw(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on make sure qu'on est visible
            if (this.Visible)
            {
                //on obtient notre position et notre taille
                Rectangle UpLeftSize = this.GetScreenPos();

                //on remplit l'arrière plan
                Brush BackBrush = new SolidBrush((Color)(this.GetProperty("BackColor")));
                g.FillRectangle(BackBrush, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
                BackBrush.Dispose();
            }
        }

    }
}
