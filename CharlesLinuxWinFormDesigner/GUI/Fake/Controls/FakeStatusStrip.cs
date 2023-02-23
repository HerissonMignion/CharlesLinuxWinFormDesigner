using System;
using System.Drawing;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeStatusStrip : FakeControlContainer
    {


        //status strip doit overrider cette méthode car un StatusStrip n'est pas affecté par les propriétés Top Left Width et Height.
        public override Rectangle GetScreenPos()
        {
            Rectangle rep = new Rectangle(this.Left, this.Top, this.Width, this.Height);

            //les StatusStrip ne sont pas affectés par les propriétés Top Left Width et Height. on ignore ces propriétés dans le code qui suit.
            rep.X = 0;
            rep.Y = 0; //valeur temporaire. cette valeur dépend de la taille du parent, donc la valeur finale est plus bas dans le code.

            if (this.Parent != null)
            {
                //on s'ajoute le décalage de la zone cliente du parent
                rep.X += this.Parent.ChildrenAreaTopLeft.X;
                rep.Y += this.Parent.ChildrenAreaTopLeft.Y;

                //on obtient la taille du parent
                Rectangle parentUpLeftSize = this.Parent.GetScreenPos();
                //on s'ajoute la position graphique du parent
                rep.X += parentUpLeftSize.X;
                rep.Y += parentUpLeftSize.Y;
                //la position d'un status strip est en bas du parent.
                rep.Y += parentUpLeftSize.Height - this.Height;

                //le StatusStrip est un contrôle qui n'est pas affecté par les propriétés Top Left Width et Height.
                //la largeur du status strip est la largeur de son parent
                rep.Width = parentUpLeftSize.Width;

            }

            return rep;
        }


        public FakeStatusStrip() : base()
        {
            this.ClassName = "StatusStrip";
            this.Height = 20;
        }

        public override void Draw(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on make sure qu'on est visible
            if (this.Visible)
            {
                //on obtient notre position et notre taille
                Rectangle UpLeftSize = this.GetScreenPos();

                //on remplit l'arrière plan
                g.FillRectangle(Brushes.Gainsboro, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
                //on dessine une ligne en haut pour le différencier du restant du body de conteneur parent de this
                g.DrawLine(Pens.DimGray, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.X + UpLeftSize.Width, UpLeftSize.Y);

                //on fait dessiner nos enfant
                this.DrawChildren(img, g, fcdc);
            }
        }


    }
}
