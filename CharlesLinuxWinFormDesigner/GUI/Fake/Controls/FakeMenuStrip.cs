using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeMenuStrip : FakeControlContainer
    {
        

        //MenuStrip doit overrider cette méthode car un MenuStrip n'est pas affecté par les propriétés Top Left Width Height.
        public override Rectangle GetScreenPos()
        {
            Rectangle rep = new Rectangle(this.Left, this.Top, this.Width, this.Height);

            //les MenuStrip ne sont pas affectés par les propriétés Left Top Width. on ignore ces propriétés dans le code qui suit.
            //les menu strip sont collés en haut de leurs parents
            rep.X = 0;
            rep.Y = 0; //todo : les menu strips sont empilés les uns par dessous les autres. ils ne sont pas tous à la coordonnée y=0
            //rep.Height = 35; //todo : trouver la formule pour calculer la vrai hauteur d'un menu strip

            //aussi longtemps que nous avons un parent, il nous faut nous ajouter sa coordonné graphique. GetScreenPos est une fonction récursive.
            if (this.Parent != null)
            {
                //on s'ajoute le décalage de la zone cliente du parent
                rep.X += this.Parent.ChildrenAreaTopLeft.X;
                rep.Y += this.Parent.ChildrenAreaTopLeft.Y;
                //on s'ajoute la position graphique du parent
                Rectangle parentUpLeftSize = this.Parent.GetScreenPos();
                rep.X += parentUpLeftSize.X;
                rep.Y += parentUpLeftSize.Y;


                //le MenuStrip est un contrôle qui n'est pas affecté par les propriétés Top Left et Width.
                //la largeur d'un menu strip est celle de son parent
                rep.Width = parentUpLeftSize.Width;

            }

            return rep;
        }


        public FakeMenuStrip() : base()
        {
            this.ClassName = "MenuStrip";
            this.Height = 20;
        }

        public override void Draw(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on make sure qu'on est visible
            if (this.Visible)
            {
                //on obtient notre position et notre taille
                Rectangle UpLeftSize = this.GetScreenPos();

                //dessine l'arrière plan
                Brush BackBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
                g.FillRectangle(BackBrush, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
                BackBrush.Dispose();

                //on fait dessiner nos enfants
                this.DrawChildren(img, g, fcdc);
            }
        }

    }
}
