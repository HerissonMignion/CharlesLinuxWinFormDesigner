using System;
using System.Drawing;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    public class FakeTabControl : FakeControlContainer
    {

        //le TabControl doit overrider cette méthode car ce ne sont pas tous ses enfants qui existent (graphiquement) en même temps.
        //alors il ne faut pas utiliser une boucle pour passer à travers tout ses enfants (TabPage), il faut vérifier uniquement le seul
        //TabPage actuellement visible (selui indiqué par SelectedIndex).
        public override FakeControl GetControlUnderScreenPos(Point ScreenPos)
        {
            //on obtient notre SelectedIndex
            int SelectedIndex = (int)(this.GetProperty("SelectedIndex"));
            //on make sure que notre SelectedIndex existe. sinon, il n'y a aucun contrôle sous la souris.
            if (SelectedIndex >= 0 && SelectedIndex < this.Children.Count)
            {
                FakeControl child = this.Children[SelectedIndex];
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

        public FakeTabControl() : base()
        {
            this.ClassName = "TabControl";
            //on lui donne de l'espace en haut pour les onglets
            this.ChildrenAreaTopLeft = new Point(0, 20);
            this.ListProperties.Add(new FakeProperty("SelectedIndex", typeof(int), 0, this));
        }

        public override void Draw(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on make sure qu'on est visible
            if (this.Visible)
            {
                //on obtient notre position et taille graphique
                Rectangle UpLeftSize = this.GetScreenPos();

                //on remplit la couleur de l'arrière plan
                Brush BackBrush = new SolidBrush((Color)(this.GetProperty("BackColor")));
                g.FillRectangle(BackBrush, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
                BackBrush.Dispose();

                //on récupère notre SelectedIndex
                int SelectedIndex = (int)(this.GetProperty("SelectedIndex"));

                //on dessine les onglets
                int index = 0; //index de l'onglet actuel
                float CurrentX = (float)(UpLeftSize.X); //position horizontale de la gauche de l'onglet actuel
                while (index < this.Children.Count)
                {
                    FakeControl child = this.Children[index];
                    //nous savons que nos enfant sont des TabPages, mais juste au cas où, on vérifit
                    if (child is FakeControlContainer)
                    {
                        FakeControlContainer childcc = (FakeControlContainer)child;

                        Font TextFont = this.MainFont;
                        //on obtient la taille du texte
                        SizeF TextSizeF = g.MeasureString(childcc.Text, TextFont);
                        //on calcule la position verticale du texte
                        float TextTop = (float)(UpLeftSize.Y) + ((float)(this.ChildrenAreaTopLeft.Y) / 2f) - (TextSizeF.Height / 2f);

                        //on prépare les couleurs
                        BackBrush = Brushes.White;
                        Brush ForeBrush = Brushes.Black;
                        //si l'onglet actuelle est SelectedIndex, alors on met des couleur différentes pour montrer que c'est l'onglet actuel
                        if (index == SelectedIndex)
                        {
                            BackBrush = Brushes.DimGray;
                            ForeBrush = Brushes.White;
                        }

                        //on dessine l'arrière plan de l'onglet
                        g.FillRectangle(BackBrush, CurrentX, (float)(UpLeftSize.Y), TextSizeF.Width, TextSizeF.Height);
                        //on dessine le texte
                        g.DrawString(childcc.Text, TextFont, ForeBrush, CurrentX, TextTop);

                        //on ajoute à CurrentX la largeur du text/la largeur de l'onglet
                        CurrentX += TextSizeF.Width;

                    }
                    //next
                    index++;
                }

                //on dessine la bordure, même si windows ne dessine pas de bordure pour les tab control. peut-être retirer plus tard
                g.DrawRectangle(Pens.Black, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);

                //on fait dessiner nos enfants
                this.DrawChildren(img, g, fcdc);
            }
        }

        //ce ne sont pas tout les TabPage qui sont visible à l'image. alors un TabControl a besoin de code spécial pour dessiner uniquement la TabPage actuellw
        public override void DrawChildren(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on récupère le selected index
            int SelectedIndex = (int)(this.GetProperty("SelectedIndex"));
            //on make sure que l'index existe
            if (SelectedIndex >= 0 && SelectedIndex < this.Children.Count)
            {
                //on fait dessiner l'enfant, la TabPage actuelle
                //this.DrawFakeControl(img, g, this.Children[SelectedIndex]);
                this.Children[SelectedIndex].Draw(img, g, fcdc);
            }
            //le SelectedIndex n'existe pas
            else
            {
                //on obtient notre position et taille graphique
                Rectangle UpLeftSize = this.GetScreenPos();
                //on dessine un message pour dire à l'user que l'index n'est pas valide
                g.DrawString("SelectedIndex\ncannot be " + SelectedIndex, this.MainFont, Brushes.Black, (float)(UpLeftSize.X), (float)(UpLeftSize.Y + this.ChildrenAreaTopLeft.Y));
            }
        }

        private Font MainFont = new Font("consolas", 10f);

    }
}
