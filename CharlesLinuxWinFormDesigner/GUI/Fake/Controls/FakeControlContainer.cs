using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    /// <summary>
    /// Fake control container. Tout les contrôles sont soit un control, ou un control qui peut contenir d'autres controles.
    /// Ceci est un contrôle qui peut contenir d'autres contrôles.
    /// </summary>
    public class FakeControlContainer : FakeControl
    {

        /// <summary>
        /// ne pas modifier cette liste directement
        /// </summary>
        public List<FakeControl> Children = new List<FakeControl>();

        private bool _AddingChild = false; //indique si nous sommes actuellement en train d'ajouter un enfant
        public void AddChild(FakeControl fc)
        {
            if (!this._AddingChild)
            {
                this._AddingChild = true;

                //on ajoute le child
                fc.Parent = this;
                this.Children.Add(fc);

                this._AddingChild = false;
            }
        }
        private bool _RemovingChild = false; //indique si nous sommes actuellement en train de retirer un enfant
        public void RemoveChild(FakeControl fc)
        {
            if (!this._RemovingChild)
            {
                this._RemovingChild = true;

                //on obtient son index
                int index = this.Children.IndexOf(fc);
                //on agit seulement si c'est l'un de nos enfant
                if (index >= 0)
                {
                    fc.RemoveFromParent();
                    this.Children.Remove(fc);
                }

                this._RemovingChild = false;
            }
        }



        //certains control container ont besoin que la vrai zone des enfants n'occupe pas 100% du contrôle. par exemple: les TabControl. les tab control ont besoin d'un espace pour afficher les onglet, que l'user peut cliquer. et la zone des enfants, les tab pages, n'a pas, comme taille, 100% de la surface du contrôle.
        //cette propriété inique l'espacement réservé en haut à gauche de this, où les enfant ne peuvent pas être.
        protected Point _ChildrenAreaTopLeft = new Point(0, 0);

        //certain contrôles (ToolStripMenuItem par exemple) n'ont pas une valeur constante pour ChildrenAreaTopLeft qui reste inchangé pour tout leur durée de vie.
        //pour cette raison, ChildrenAreaTopLeft est virtual et peut être overridé.

        public virtual Point ChildrenAreaTopLeft
        {
            get
            {
                return this._ChildrenAreaTopLeft;
            }
            set
            {
                this._ChildrenAreaTopLeft = value;
            }
        }




        public FakeControlContainer() : base()
        {
            this.ClassName = "ControlContainer";
        }


        //obtient la coordonnée, relative à la zone enfant de this, d'une coordonnée "graphique" (coordonnée "graphique" = coordonnée d'un enfant du parent imaginaire de FormDesigner.TopFakeCC)
        public virtual Point GetRelativePosFromScreenPos(Point ScreenPos)
        {
            Point rep = ScreenPos;
            //on retire le décalage de la zone cliente
            rep.X -= this.ChildrenAreaTopLeft.X;
            rep.Y -= this.ChildrenAreaTopLeft.Y;

            //on retire notre position relative à notre parent, indiqué par Left et Top.
            //cependant, certain contrôles ne sont pas affectés par les propriétés Top et Left. ces contrôles override cette méthode pour gérer correctement leurs situations et leurs propriétés.

            //puisque nous ne sommes pas un contrôle spéciale, alors on fait la situation normale
            rep.X -= this.Left;
            rep.Y -= this.Top;

            //on check si on a un parent
            if (this.Parent != null)
            {
                //on appele le parent pour que le parent retire son décalage (souvant son .top et son .left).
                //cela est fait récursivement jusqu'à ce qu'il n'y ait plus de parent.
                rep = this.Parent.GetRelativePosFromScreenPos(rep);
            }
            return rep;
        }


        //obtient le controle _enfant_ qui se situe sous la coordonnée "graphique" spécifié.
        //cette fonction n'est pas récursive, c'est par design.
        public virtual FakeControl GetControlUnderScreenPos(Point ScreenPos)
        {
            //certains control container n'ont pas tout leurs enfants visible en même temps. ainsi, ces contrôles ont leur propre code (override).
            //pour les control container "normale", on fait juste vérifier tout les enfants jusqu'à ce qu'on tombe sur un enfant dessous la coordonnée ScreenPos.

            foreach (FakeControl child in this.Children)
            {
                //on obtient sa coordonnée et sa taille graphique
                Rectangle UpLeftSize = child.GetScreenPos();
                //on check si la coordonnée est sur ce child
                if (UpLeftSize.X <= ScreenPos.X && ScreenPos.X < UpLeftSize.X + UpLeftSize.Width)
                {
                    if (UpLeftSize.Y <= ScreenPos.Y && ScreenPos.Y < UpLeftSize.Y + UpLeftSize.Height)
                    {
                        //la coordonnée est sur ce child
                        return child;
                    }
                }
            }

            return null;
        }




        //lorsque l'interface graphique modifie la taille du contrôle this, il faut changer la position/taille horizontale et verticale de nos contrôles enfants.
        //appeler cette méthode avec l'ancienne taille de this après avoir effectué un changement sur la taille de this.
        //cette méthode va parcourir récursivement tout ses control container enfants.
        public void FixChildrenByAnchor(int OldWidth, int OldHeight)
        {
            //on doit ajuster la taille de chacun de nos enfants
            foreach (FakeControl child in this.Children)
            {
                //on conserve l'ancienne taille de cet enfant
                int childLastWidth = child.Width;
                int childLastHeight = child.Height;

                AnchorStyles anchor = (AnchorStyles)(child.GetProperty("Anchor"));
                //on ajuste sa taille selon ses anchor

                //on check s'il est anchré à droite
                if (anchor.HasFlag(AnchorStyles.Right))
                {
                    //si le contrôle est anchré à gauche, alors sa largeur doit changer
                    if (anchor.HasFlag(AnchorStyles.Left))
                    {
                        child.Width += this.Width - OldWidth;
                    }
                    //si le contrôle n'est pas anchré à gauche, alors la position horizontale doit changer
                    else
                    {
                        child.Left += this.Width - OldWidth;
                    }
                }
                //le contrôle n'est pas anchré à droite
                else
                {
                    //si le contrôle n'est pas anchré à gauche, alors il doit floter dans le vide
                    if (!anchor.HasFlag(AnchorStyles.Left))
                    {
                        child.Left += (this.Width - OldWidth) / 2;
                    }
                }

                //on check s'il est anchré en bas
                if (anchor.HasFlag(AnchorStyles.Bottom))
                {
                    //si le contrôle est anchré en haut, alors la hauteur height doit changer
                    if (anchor.HasFlag(AnchorStyles.Top))
                    {
                        child.Height += this.Height - OldHeight;
                    }
                    //si le contrôle n'est pas anchré en haut, alors sa position verticalle doit changer
                    else
                    {
                        child.Top += this.Height - OldHeight;
                    }
                }
                //le contrôle n'est pas anchré en bas
                else
                {
                    //si le contrôle n'est pas anchré en haut, alors il doit floter dans le vide
                    if (!anchor.HasFlag(AnchorStyles.Top))
                    {
                        child.Top += (this.Height - OldHeight) / 2;
                    }
                }



                //si ce child est lui-même un control container, alors il faut lui dire de redimensionner ses enfant, en lui donnant son ancienne taille
                if (child is FakeControlContainer)
                {
                    ((FakeControlContainer)child).FixChildrenByAnchor(childLastWidth, childLastHeight);
                }

            }
        }



        //FakeControlContainer doit overrider cette méthode car un control container doit également faire dessiner ses enfants
        public override void Draw(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on make sure qu'on est visible
            if (this.Visible)
            {
                base.Draw(img, g, fcdc);
                this.DrawChildren(img, g, fcdc);
            }
        }

        //effectue le dessin des enfants.
        public virtual void DrawChildren(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            for (int i = this.Children.Count - 1; i >= 0; i--)
            {
                FakeControl child = this.Children[i];
                //this.DrawFakeControl(img, g, child);
                child.Draw(img, g, fcdc);
            }

        }

        //crée un objet dupliqué de this, mais ne lui donne pas de parent, et ne copie/duplique pas les enfants
        public override FakeControl GetDuplicate()
        {
            FakeControlContainer copy = new FakeControlContainer();

            //on copy les valeurs de nos propriétés
            foreach (FakeProperty fp in this.ListProperties)
            {
                //copy.ListProperties.Add(new FakeProperty(fp.Name, fp.Type, fp.Value, copy));
                copy.SetProperty(fp.Name, fp.Value);
            }

            return FakeControl.GenerateFakeControlFromGeneric(copy);
        }


        //contrairement à un panel ou group box, les coordonnées (0, 0) des tab page ne commencent pas le plus en haut à droite possible en dessous des onglets. cette valeur est l'espacement horizontale et verticale supplémentaire.
        public const int TabControlInnerSpaceH = 4; // 4 s'applique à gauche et à droite
        public const int TabControlInnerSpaceV = 4; // 4 s'applique seulement en bas


    }
}
