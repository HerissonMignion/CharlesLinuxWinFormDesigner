using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeTabPage : FakeControlContainer
    {

        //TabPage doit overrider cette méthode car TabPage n'est pas affecté pas les propriétés Top Left Width Height.
        public override Rectangle GetScreenPos()
        {
            Rectangle rep = new Rectangle(this.Left, this.Top, this.Width, this.Height);

            //les TabPage ne sont pas affectés par les propriétés Left et Top. on fait juste ignorer Top et Left dans la suite du code.
            //les tab pages ne sont pas positionné le plus en haut à gauche possible de leur parent. il y a un petit espace horizontale à gauche et à droite.
            rep.X = FakeControlContainer.TabControlInnerSpaceH; //l'espacement qui est à gauche n'est pas un espacement géré par la propriété ChildrenAreaTopLeft.
            rep.Y = 0;

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


                //les TabPage sont un contrôle qui n'est pas affecté par les propriétés Top Left Width et Height.
                //notre taille dépend de notre parent. deplus, il ne faut pas oublier de retirer l'espacement supplémentaire, unique aux TabPage, à gauche, à droite et en bas.
                rep.Width = this.Parent.Width - (FakeControlContainer.TabControlInnerSpaceH * 2);
                rep.Height = this.Parent.Height - this.Parent.ChildrenAreaTopLeft.Y - (FakeControlContainer.TabControlInnerSpaceV);

            }

            return rep;
        }

        //parce que les TabPage sont situés dans des TabControl, ils ne sont pas affectés par les propriétés Top et Left.
        //il faut overrider cette méthode afin d'ignorer les propriétés Top et Left.
        public override Point GetRelativePosFromScreenPos(Point ScreenPos)
        {
            Point rep = ScreenPos;
            //on retire le décalage de la zone cliente
            rep.X -= this.ChildrenAreaTopLeft.X;
            rep.Y -= this.ChildrenAreaTopLeft.Y;

            //les contrôles normales retire leur position relative à leur parent, indiqué par Left et Top.
            //cependant, les TabPage ne sont pas affectés par les propriétés Top et Left. au lieu de restreindre, partout dans le code, la possibilité de mettre autre chose que 0 à Top et Left, j'ai choisi d'ignorer Top et Left pour les TabPage.

            //on retire l'espacement entre la bordure gauche du TabControl et la bordure gauche du TabPage
            rep.X -= FakeControlContainer.TabControlInnerSpaceH;

            //on check si on a un parent
            if (this.Parent != null)
            {
                //on appele le parent pour que le parent retire son décalage (souvant son .top et son .left).
                //cela est fait récursivement jusqu'à ce qu'il n'y ait plus de parent.
                rep = this.Parent.GetRelativePosFromScreenPos(rep);
            }
            return rep;
        }


        public FakeTabPage() : base()
        {
            this.ClassName = "TabPage";
            this.SetProperty("Anchor", AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);
        }

        public override void Draw(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on make sure qu'on est visible
            if (this.Visible)
            {
                //on obtient notre position et notre taille graphique
                Rectangle UpLeftSize = this.GetScreenPos();

                //on remplit la couleur de l'arrière plan
                Brush BackBrush = new SolidBrush((Color)(this.GetProperty("BackColor")));
                g.FillRectangle(BackBrush, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
                BackBrush.Dispose();

                //on dessine la bordure
                g.DrawRectangle(Pens.Black, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);

                //on fait dessiner nos enfants
                this.DrawChildren(img, g, fcdc);
            }
        }

    }
}
