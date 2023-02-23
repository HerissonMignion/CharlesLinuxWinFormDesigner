using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeToolStripMenuItem : FakeControlContainer
    {
        //les ToolStripMenuItem sont placés dans des control container qui stackent leurs enfants horizontalement ou verticalement de façon automatique.


        //indique si les enfants dans notre parent sont alignés horizontalement ou verticalement.
        //return true pour horizontale. false pour verticale.
        //todo: peut être faire une interface qui contient cette méthode. aussi, faire en sorte que le parent lui-même nous indique dans quel direction vont ses enfants.
        private bool IsHorizontalParent()
        {
            //on check si on a un parent
            if (this.Parent != null)
            {
                //si notre parent est un MenuStrip, alors on assume que ses enfants sont stackés horizontalement. autrement, verticalement
                if (this.Parent is FakeMenuStrip)
                {
                    return true;
                }

                return false;
            }
            return true;
        }


        //ToolStripMenuItem a besoin d'overrider cette propriété car cette propriété change selon le type de parent et peut changer (à l'avenir) selon certaines propriétés de this.
        public override Point ChildrenAreaTopLeft
        {
            get
            {
                //si notre parent aligne les enfants horizontalement, alors notre zone enfant doit être placé en dessous de nous
                if (this.IsHorizontalParent())
                {
                    this._ChildrenAreaTopLeft.X = 0;
                    this._ChildrenAreaTopLeft.Y = this.GetScreenPos().Height + 5;
                }
                //sinon on doit placer nos enfants à notre droite
                else
                {
                    this._ChildrenAreaTopLeft.X = this.GetScreenPos().Width + 5;
                    this._ChildrenAreaTopLeft.Y = 0;
                }

                return this._ChildrenAreaTopLeft;
            }
            set
            {

            }
        }

        //un ToolStripMenuItem doit overrider cette méthode car sa position et sa taille n'est pas contrôlable de façon flexible par le programmeur.
        public override Rectangle GetScreenPos()
        {
            Rectangle rep = new Rectangle(this.Left, this.Top, this.Width, this.Height);

            //on mémorise si notre parent est verticale ou horizontal
            bool isHorizontal = this.IsHorizontalParent();

            //la position et la taille n'est pas contrôlable par l'user/programmeur. on ignore nos propriétés Top Left Width et Height.
            rep.X = 0;
            rep.Y = 0;
            rep.Height = 50; // 20 todo: trouver la vrai formule pour calculer le height d'un ToolStripMenuItem.
            //rep.Width = (int)(20f + (3.2f * Program.MeasureString(this.Text, (Font)(this.GetProperty("Font"))).Width));
            rep.Width = (int)(20f + (1f * Program.MeasureString(this.Text, (Font)(this.GetProperty("Font"))).Width));

            //on check si on a un parent
            if (this.Parent != null)
            {
                //on prend le height du parent
                rep.Height = this.Parent.Height; //cela ne doit pas être récursif. MenuStrip semble pouvoir avoir une height différente de tout les sous menu contenu à l'intérieur.

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


        public FakeToolStripMenuItem() : base()
        {
            this.ClassName = "ToolStripMenuItem";
            this.SetProperty("Font", new Font("Consolas", 10f));
            this.Height = 20; // 20
        }

        public override void Draw(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on make sure qu'on est visible
            if (this.Visible)
            {
                //on obtient notre position et notre taille
                Rectangle UpLeftSize = this.GetScreenPos();

                g.FillRectangle(Brushes.White, UpLeftSize);
                g.DrawRectangle(Pens.Blue, UpLeftSize);



                //on dessine notre text au milieu
                Font TextFont = (Font)(this.GetProperty("Font"));
                SizeF TextSizeF = g.MeasureString(this.Text, TextFont);
                Brush ForeBrush = new SolidBrush((Color)(this.GetProperty("ForeColor")));
                g.DrawString(this.Text, TextFont, ForeBrush, UpLeftSize.X + (UpLeftSize.Width / 2) - (TextSizeF.Width / 2f), UpLeftSize.Y + (UpLeftSize.Height / 2) - (TextSizeF.Height / 2f));
                ForeBrush.Dispose();

                //on affiche nos enfants seulement si le ShownFakeC est nous ou un de nos enfants (récursivement)
                if (fcdc.ShownFakeC == this || fcdc.ShownFakeC.IsEventualAncestor(this))
                {
                    this.DrawChildren(img, g, fcdc);
                }
            }
        }
    }
}
