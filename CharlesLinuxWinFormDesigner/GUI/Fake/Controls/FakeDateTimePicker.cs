using System;
using System.Drawing;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeDateTimePicker : FakeControl
    {

        //un DateTimePicker override cette méthode car la propriété height n'est pas contrôlable par le programmeur. (contrôlable par la font)
        public override Rectangle GetScreenPos()
        {
            Rectangle rep = base.GetScreenPos();

            //la hauteur d'un DateTimePicker est décidé par la font
            rep.Height = (int)(7.5f + (1.558f * ((Font)(this.GetProperty("Font"))).Size));

            return rep;
        }

        public FakeDateTimePicker() : base()
        {
            this.ClassName = "DateTimePicker";
            this.Text = ""; //important car "notext" n'est pas une date valide dans aucun langage humain.
            this.Width = 120;
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

                //ici on dessinerait la "valeur" actuellement sélectionné, mais on ne suporte pas de faire les options du DateTimePicker, alors nous dessinons un exemple de date.
                {
                    string Text = "7 décembre 2022";

                    SizeF TextSizeF = g.MeasureString("QWERTYqtypdfghjklb", (Font)(this.GetProperty("Font")));
                    //on prépare la position verticale du texte
                    float TextTop = (float)(UpLeftSize.Y);
                    TextTop = (float)(UpLeftSize.Y + (UpLeftSize.Height / 2)) - (TextSizeF.Height / 2f);

                    Brush TextBrush = new SolidBrush((Color)(this.GetProperty("ForeColor")));
                    g.DrawString(Text, (Font)(this.GetProperty("Font")), TextBrush, (float)(UpLeftSize.X), TextTop);
                    TextBrush.Dispose();
                }



                //on dessine un apperçu de la flèche à droite du DateTimePicker
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
