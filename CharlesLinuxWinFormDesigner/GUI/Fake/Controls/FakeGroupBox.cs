using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeGroupBox : FakeControlContainer
    {
        public FakeGroupBox() : base()
        {
            this.ClassName = "GroupBox";
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

                int dist = 10; //distance en pixel avec le bord gauche du contrôle

                //on dessine le texte
                float TextSizeWidth = g.MeasureString(this.Text, (Font)(this.GetProperty("Font"))).Width;
                float TextSizeHeight = g.MeasureString("QWERTYqtypdfghjklb", (Font)(this.GetProperty("Font"))).Height; //on a une chaine de texte différente pour la hauteur parce qu'on veut une hauteur constante peut importe le text entré.
                if (this.Text.Length > 0)
                {
                    Brush TextBrush = new SolidBrush((Color)(this.GetProperty("ForeColor")));
                    g.DrawString(this.Text, (Font)(this.GetProperty("Font")), TextBrush, (float)(UpLeftSize.X + dist), (float)(UpLeftSize.Y));
                    TextBrush.Dispose();
                }

                //on dessine les bordures
                g.DrawLine(Pens.DimGray, UpLeftSize.X, UpLeftSize.Y + (TextSizeHeight / 2), UpLeftSize.X + dist, UpLeftSize.Y + (TextSizeHeight / 2)); //ligne à gauche du texte
                g.DrawLine(Pens.DimGray, UpLeftSize.X + TextSizeWidth, UpLeftSize.Y + (TextSizeHeight / 2), UpLeftSize.X + UpLeftSize.Width, UpLeftSize.Y + (TextSizeHeight / 2)); //ligne à droite du texte.
                g.DrawLine(Pens.DimGray, UpLeftSize.X, UpLeftSize.Y + (TextSizeHeight / 2), UpLeftSize.X, UpLeftSize.Y + UpLeftSize.Height); //ligne à gauche
                g.DrawLine(Pens.DimGray, UpLeftSize.X, UpLeftSize.Y + UpLeftSize.Height, UpLeftSize.X + UpLeftSize.Width, UpLeftSize.Y + UpLeftSize.Height); //ligne d'en bas
                g.DrawLine(Pens.DimGray, UpLeftSize.X + UpLeftSize.Width, UpLeftSize.Y + (TextSizeHeight / 2), UpLeftSize.X + UpLeftSize.Width, UpLeftSize.Y + UpLeftSize.Height); //ligne à droite

                //on fait dessiner nos enfants
                this.DrawChildren(img, g, fcdc);
            }
        }

    }
}
