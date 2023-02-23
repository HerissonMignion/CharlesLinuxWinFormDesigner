using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeLabel : FakeControl
    {
        public FakeLabel() : base()
        {
            this.ClassName = "Label";
            this.ListProperties.Add(new FakeProperty("TextAlign", typeof(ContentAlignment), ContentAlignment.TopLeft, this));
        }


        public override void Draw(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on make sure qu'on est visible
            if (this.Visible)
            {
                //on obtient la position graphique absolue de ce fake control
                Rectangle UpLeftSize = this.GetScreenPos();

                //on remplit l'arrière plan
                Brush BackBrush = new SolidBrush((Color)(this.GetProperty("BackColor")));
                g.FillRectangle(BackBrush, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
                BackBrush.Dispose();

                //on dessine le text
                if (this.Text.Length > 0)
                {
                    SizeF TextSizeF = g.MeasureString(this.Text, (Font)(this.GetProperty("Font")));

                    float TextLeft = (float)(UpLeftSize.X);
                    float TextTop = (float)(UpLeftSize.Y);

                    ContentAlignment ca = (ContentAlignment)(this.GetProperty("TextAlign"));
                    //on check s'il faut changer l'alignement horizontale
                    //on commence par le milieu horizontale
                    if (ca == ContentAlignment.TopCenter || ca == ContentAlignment.MiddleCenter || ca == ContentAlignment.BottomCenter)
                    {
                        TextLeft += (float)(UpLeftSize.Width / 2) - (TextSizeF.Width / 2f);
                    }
                    //droite horizontale
                    else if (ca == ContentAlignment.TopRight || ca == ContentAlignment.MiddleRight || ca == ContentAlignment.BottomRight)
                    {
                        TextLeft += (float)(UpLeftSize.Width) - TextSizeF.Width;
                    }

                    //milieu verticale
                    if (ca == ContentAlignment.MiddleLeft || ca == ContentAlignment.MiddleCenter || ca == ContentAlignment.MiddleRight)
                    {
                        TextTop += (float)(UpLeftSize.Height / 2) - (TextSizeF.Height / 2f);
                    }
                    //bas verticale
                    else if (ca == ContentAlignment.BottomLeft || ca == ContentAlignment.BottomCenter || ca == ContentAlignment.BottomRight)
                    {
                        TextTop += (float)(UpLeftSize.Height) - TextSizeF.Height;
                    }

                    Brush TextBrush = new SolidBrush((Color)(this.GetProperty("ForeColor")));
                    g.DrawString(this.Text, (Font)(this.GetProperty("Font")), TextBrush, TextLeft, TextTop);
                    TextBrush.Dispose();
                }
            }
        }

    }
}
