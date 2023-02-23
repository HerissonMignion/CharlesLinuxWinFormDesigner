using System;
using System.Drawing;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeToolStripStatusLabel : FakeControl
    {


        //ToolStripStatusLabel doit overrider cette méthode car un ToolStripStatusLabel est un contrôle dont on ne peut pas contrôler ses propriétés Top Left Width et Height.
        public override Rectangle GetScreenPos()
        {
            Rectangle rep = new Rectangle(this.Left, this.Top, this.Width, this.Height);

            //on mémorise si notre parent est verticale ou horizontal
            bool isHorizontal = true;

            //la position et la taille n'est pas contrôlable par l'user/programmeur. on ignore nos propriétés Top Left Width et Height.
            rep.X = 0;
            rep.Y = 0;
            rep.Height = 50; // 20 todo: trouver la vrai formule pour calculer le height d'un ToolStripStatusLabel.
            //rep.Width = (int)(20f + (3.2f * Program.MeasureString(this.Text, (Font)(this.GetProperty("Font"))).Width));
            rep.Width = (int)(20f + (1f * Program.MeasureString(this.Text, (Font)(this.GetProperty("Font"))).Width));

            //on check si on a un parent
            if (this.Parent != null)
            {
                //on prend le height du parent
                rep.Height = this.Parent.Height;

                //on s'ajoute le décalage de la zone cliente du parent
                rep.X += this.Parent.ChildrenAreaTopLeft.X;
                rep.Y += this.Parent.ChildrenAreaTopLeft.Y;
                //on s'ajoute la position graphique du parent
                Rectangle parentUpLeftSize = this.Parent.GetScreenPos();
                rep.X += parentUpLeftSize.X;
                rep.Y += parentUpLeftSize.Y;

                //nous devons mainenant calculer notre décalage depuis de début du control container parent.

                //on passe à travers tout les enfants de notre parent
                foreach (FakeControl fc in this.Parent.Children)
                {
                    //si nous sommes arrivés à nous, on s'arrête
                    if (fc == this)
                    {
                        break;
                    }

                    //on obtient la taille de ce child
                    Rectangle ChildUpLeftSize = fc.GetScreenPos();

                    //on se déplace de la taille de ce child (nous précédant)
                    //on check si notre parent est verticale ou horizontal
                    if (isHorizontal)
                    {
                        rep.X += ChildUpLeftSize.Width;
                    }
                    //notre parent est verticale
                    else
                    {
                        rep.Y += ChildUpLeftSize.Height;
                    }

                }


            }


            return rep;
        }

        public FakeToolStripStatusLabel() : base()
        {
            this.ClassName = "ToolStripStatusLabel";
            this.SetProperty("Font", new Font("Consolas", 10f));
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
                g.FillRectangle(BackBrush, UpLeftSize);
                BackBrush.Dispose();
                g.DrawRectangle(Pens.Blue, UpLeftSize);

                //on dessine notre text au milieu
                Font TextFont = (Font)(this.GetProperty("Font"));
                SizeF TextSizeF = g.MeasureString(this.Text, TextFont);
                Brush ForeBrush = new SolidBrush((Color)(this.GetProperty("ForeColor")));
                g.DrawString(this.Text, TextFont, ForeBrush, UpLeftSize.X + (UpLeftSize.Width / 2) - (TextSizeF.Width / 2f), UpLeftSize.Y + (UpLeftSize.Height / 2) - (TextSizeF.Height / 2f));
                ForeBrush.Dispose();


            }
        }

    }
}
