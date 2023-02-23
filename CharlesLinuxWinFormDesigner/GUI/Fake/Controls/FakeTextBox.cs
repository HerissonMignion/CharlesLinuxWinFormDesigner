using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeTextBox : FakeControl
    {

        //le TextBox doit overrider cette méthode car, parfois, il ne doit pas réagir à la propriété Height
        public override Rectangle GetScreenPos()
        {
            Rectangle rep = base.GetScreenPos();

            //pour les textbox, c'est seulement si le textbox n'est pas multi ligne que nous n'avons pas le contrôle sur sa hauteur.
            if (!(bool)(this.GetProperty("Multiline")))
            {
                rep.Height = (int)(7.5f + (1.558f * ((Font)(this.GetProperty("Font"))).Size));
            }

            return rep;
        }

        public FakeTextBox() : base()
        {
            this.ClassName = "TextBox";
            this.SetProperty("BackColor", Color.White);
            this.ListProperties.Add(new FakeProperty("Multiline", typeof(bool), false, this));
            this.ListProperties.Add(new FakeProperty("ReadOnly", typeof(bool), false, this));
        }

        public override void Draw(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on make sure qu'on est visible
            if (this.Visible)
            {
                //on obtient notre position et notre taille
                Rectangle UpLeftSize = this.GetScreenPos();

                //on mémorise si c'est un textbox multi lignes
                bool isMultiline = (bool)(this.GetProperty("Multiline"));

                //on prépare la hauteur du textbox dans une variable
                //int TextboxHeight = UpLeftSize.Height;
                //si ce n'est pas un textbox multi ligne, alors la hauteur n'est pas contrôlable
                //if (!isMultiline)
                //{
                //    //puisque nous avons un textbox single line, nous déterminons la hauteur du textbox, avec une petite formule qui est une aproximation
                //    //déplacer ça dans FakeControl.GetScreenPos()
                //    //TextboxHeight = (int)(7.5f + (1.558f * ((Font)(fc.GetProperty("Font"))).Size));
                //}

                //on remplit l'arrière plan
                Brush BackBrush = new SolidBrush((Color)(this.GetProperty("BackColor")));
                g.FillRectangle(BackBrush, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
                BackBrush.Dispose();
                //on dessine la bordure
                g.DrawRectangle(Pens.Black, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);

                //on dessine le texte
                if (this.Text.Length > 0)
                {
                    SizeF TextSizeF = g.MeasureString("QWERTYqtypdfghjklb", (Font)(this.GetProperty("Font")));
                    //on prépare la position verticale du texte
                    float TextTop = (float)(UpLeftSize.Y);
                    //si nous sommes un textbox single line, alors le texte est centré verticalement
                    if (!isMultiline)
                    {
                        TextTop = (float)(UpLeftSize.Y + (UpLeftSize.Height / 2)) - (TextSizeF.Height / 2f);
                    }

                    Brush TextBrush = new SolidBrush((Color)(this.GetProperty("ForeColor")));
                    g.DrawString(this.Text, (Font)(this.GetProperty("Font")), TextBrush, (float)(UpLeftSize.X), TextTop);
                    TextBrush.Dispose();
                }
            }
        }

    }
}
