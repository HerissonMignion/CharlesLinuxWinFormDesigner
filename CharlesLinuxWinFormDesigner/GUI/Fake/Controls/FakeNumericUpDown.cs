using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeNumericUpDown : FakeControl
    {

        //le NumericUpDown override cette méthode car ils ne sont pas affectés par la propriété Height
        public override Rectangle GetScreenPos()
        {
            Rectangle rep = base.GetScreenPos();
            rep.Height = (int)(7.5f + (1.558f * ((Font)(this.GetProperty("Font"))).Size));
            return rep;
        }

        public FakeNumericUpDown() : base()
        {
            this.ClassName = "NumericUpDown";
            this.ListProperties.Add(new FakeProperty("Value", typeof(decimal), 0m, this));
            this.ListProperties.Add(new FakeProperty("Minimum", typeof(decimal), 0m, this));
            this.ListProperties.Add(new FakeProperty("Maximum", typeof(decimal), 10m, this));
            this.ListProperties.Add(new FakeProperty("DecimalPlaces", typeof(int), 0, this));
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

                //on dessine le texte
                string Text = ((decimal)(this.GetProperty("Value"))).ToString();
                SizeF TextSizeF = g.MeasureString(Text, (Font)(this.GetProperty("Font")));
                //on prépare la position verticale du texte
                float TextTop = (float)(UpLeftSize.Y + (UpLeftSize.Height / 2)) - (TextSizeF.Height / 2f);

                Brush TextBrush = new SolidBrush((Color)(this.GetProperty("ForeColor")));
                g.DrawString(Text, (Font)(this.GetProperty("Font")), TextBrush, (float)(UpLeftSize.X), TextTop);
                TextBrush.Dispose();

                //on dessine un apperçu des flèche up et down à droite du numeric up down
                int arrowAreaWidth = 15;
                //on remplit l'arrière plan
                g.FillRectangle(Brushes.Silver, UpLeftSize.X + UpLeftSize.Width - arrowAreaWidth, UpLeftSize.Y, arrowAreaWidth, UpLeftSize.Height);
                //on dessine les lignes qui séparent les flèches et le champ de la valeur
                g.DrawLine(Pens.Black, UpLeftSize.X + UpLeftSize.Width - arrowAreaWidth, UpLeftSize.Y, UpLeftSize.X + UpLeftSize.Width - arrowAreaWidth, UpLeftSize.Y + UpLeftSize.Height);
                g.DrawLine(Pens.Black, UpLeftSize.X + UpLeftSize.Width - arrowAreaWidth, UpLeftSize.Y + (UpLeftSize.Height / 2), UpLeftSize.X + UpLeftSize.Width, UpLeftSize.Y + (UpLeftSize.Height / 2));

                //on dessine la bordure
                g.DrawRectangle(Pens.Black, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
            }
        }

    }
}
