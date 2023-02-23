using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeCheckBox : FakeControl
    {
        public FakeCheckBox() : base()
        {
            this.ClassName = "CheckBox";
            this.ListProperties.Add(new FakeProperty("Checked", typeof(bool), false, this));
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

                //rectangle qui représente la position et la taille graphique du carré où le crochet apparaît
                int BoxWidth = 10; //hauteur et largeur du carré du crochet
                Rectangle BoxRect = new Rectangle(UpLeftSize.X, UpLeftSize.Y + (UpLeftSize.Height / 2) - (BoxWidth / 2), BoxWidth, BoxWidth);

                //on dessine le carré du crochet
                g.FillRectangle(Brushes.White, BoxRect);
                g.DrawRectangle(Pens.Black, BoxRect);

                //on dessine le X si le checkbox est "coché"
                if ((bool)(this.GetProperty("Checked")))
                {
                    g.DrawLine(Pens.Black, BoxRect.X, BoxRect.Y, BoxRect.X + BoxWidth, BoxRect.Y + BoxWidth); //top-left vers bottom-right
                    g.DrawLine(Pens.Black, BoxRect.X + BoxWidth, BoxRect.Y, BoxRect.X, BoxRect.Y + BoxWidth); //top-right vers bottom-left
                }

                //on dessine le text
                if (this.Text.Length > 0)
                {
                    SizeF TextSizeF = g.MeasureString(this.Text, (Font)(this.GetProperty("Font")));

                    float TextLeft = (float)(UpLeftSize.X + BoxWidth);
                    float TextTop = (float)(UpLeftSize.Y + (UpLeftSize.Height / 2)) - (TextSizeF.Height / 2f);

                    Brush TextBrush = new SolidBrush((Color)(this.GetProperty("ForeColor")));
                    g.DrawString(this.Text, (Font)(this.GetProperty("Font")), TextBrush, TextLeft, TextTop);
                    TextBrush.Dispose();
                }

            }
        }

    }
}
