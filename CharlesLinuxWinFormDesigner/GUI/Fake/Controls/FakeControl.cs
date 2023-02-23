using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.Controls
{
    /// <summary>
    /// Fake control représente un control dont l'interface graphique (ce programme) sert à designer à la place du programmeur
    /// </summary>
    public class FakeControl
    {
        //Form, Button, Label, PictureBox, TextBox, ...
        public string ClassName
        {
            get
            {
                return (string)(this.GetProperty("ClassName"));
            }
            set
            {
                this.SetProperty("ClassName", value);
            }
        }

        //nom dans le code c#
        public string CodeName
        {
            get
            {
                return (string)(this.GetProperty("CodeName"));
            }
            set
            {
                this.SetProperty("CodeName", value);
            }
        }


        private FakeControlContainer _Parent = null; //notre parent actuel
        private bool _AddingToParent = false; //indique si nous sommes actuellement en train de s'ajouter dans un parent
        public FakeControlContainer Parent
        {
            get
            {
                return this._Parent;
            }
            set
            {
                if (!this._AddingToParent)
                {
                    this._AddingToParent = true;

                    //on commence par se retirer de notre parent, si nous en avons un
                    this.RemoveFromParent();

                    //on se met dans notre nouveau parent
                    this._Parent = value;
                    value.AddChild(this);

                    this._AddingToParent = false;
                }
            }
        }

        private bool _RemovingFromParent = false; //indique si nous sommes actuellement en train de se retirer de notre parent
        public void RemoveFromParent()
        {
            //on make sure qu'on a un parent et qu'on n'a pas déjà commencé à se retirer de lui
            if (this._Parent != null && !this._RemovingFromParent)
            {
                this._RemovingFromParent = true;

                //on dit à notre parent de nous retirer
                this._Parent.RemoveChild(this);
                this._Parent = null; //on oublie le parent

                this._RemovingFromParent = false;
            }
        }

        //return true si le fcc donné est le parent ou un autre ancêtre plus haut de this
        public bool IsEventualAncestor(FakeControlContainer fcc)
        {
            FakeControl current = this;
            while (current.Parent != null)
            {
                //on passe au parent
                current = current.Parent;
                //on check si c'est l'objet reçu en paramètre
                if (current == fcc)
                {
                    return true;
                }
            }
            return false;
        }




        public int Left
        {
            get
            {
                return (int)(this.GetProperty("Left"));
            }
            set
            {
                this.SetProperty("Left", value);
            }
        }
        public int Top
        {
            get
            {
                return (int)(this.GetProperty("Top"));
            }
            set
            {
                this.SetProperty("Top", value);
            }
        }
        public int Width
        {
            get
            {
                return (int)(this.GetProperty("Width"));
            }
            set
            {
                this.SetProperty("Width", value);
            }
        }
        public int Height
        {
            get
            {
                return (int)(this.GetProperty("Height"));
            }
            set
            {
                this.SetProperty("Height", value);
            }
        }

        public string Text
        {
            get
            {
                return (string)(this.GetProperty("Text"));
            }
            set
            {
                this.SetProperty("Text", value);
            }
        }

        public bool Visible
        {
            get
            {
                return (bool)(this.GetProperty("Visible"));
            }
            set
            {
                this.SetProperty("Visible", value);
            }
        }


        public List<FakeProperty> ListProperties = new List<FakeProperty>(); //la liste des propriétés du fake control. ce sont ces propriétés que l'user a besoin de modifier.
        //obtient la valeur d'une fake propriété.
        //return null si la propriété n'existe pas.
        public object GetProperty(string Name)
        {
            foreach (FakeProperty fp in this.ListProperties)
            {
                //on check si le nom correspond
                if (fp.Name == Name)
                {
                    return fp.Value;
                }
            }
            return null;
        }
        //défini la valeur d'une fake propriété
        public void SetProperty(string Name, object Value)
        {
            bool found = false;
            foreach (FakeProperty fp in this.ListProperties)
            {
                //on check si c'est le nom qu'on cherche
                if (fp.Name == Name)
                {
                    fp.Value = Value;
                    found = true;
                    break;
                }
            }
            //si la propriété n'a pas été trouvé, on la crée. on doit créer la propriété à cause du fonctionnement du processus de sauvegarde et de restauration
            if (!found)
            {
                this.ListProperties.Add(new FakeProperty(Name, Value.GetType(), Value, this));
            }
        }

        //s'assure que nous avons la propriété donné. si nous n'avons pas la propriété, alors nous nous ajoutons la propriété avec la valeur donné en paramètre.
        //si la propriété existe, nous NE remplaçons PAS la valeur déjà existante.
        public void MakeSureHasProperty(string Name, object Value)
        {
            bool found = false;
            foreach (FakeProperty fp in this.ListProperties)
            {
                //on check si c'est le nom qu'on cherche
                if (fp.Name == Name)
                {
                    found = true;
                    break;
                }
            }
            //si la valeur n'existe pas, nous créons la propriété
            if (!found)
            {
                this.SetProperty(Name, Value);
            }
        }


        public FakeControl()
        {
            //on ajoute nos propriétés
            this.ListProperties.Add(new FakeProperty("Modifier", typeof(string), "private", this));
            this.ListProperties.Add(new FakeProperty("ClassName", typeof(string), "Control", this));
            this.ListProperties.Add(new FakeProperty("CodeName", typeof(string), "codename", this));

            this.ListProperties.Add(new FakeProperty("Left", typeof(int), 0, this));
            this.ListProperties.Add(new FakeProperty("Top", typeof(int), 0, this));
            this.ListProperties.Add(new FakeProperty("Width", typeof(int), 100, this));
            this.ListProperties.Add(new FakeProperty("Height", typeof(int), 100, this));
            this.ListProperties.Add(new FakeProperty("Text", typeof(string), "notext", this));
            this.ListProperties.Add(new FakeProperty("Font", typeof(Font), new Font("DejaVu Sans", 8.25f), this));

            this.ListProperties.Add(new FakeProperty("BackColor", typeof(Color), Color.Gainsboro, this));
            this.ListProperties.Add(new FakeProperty("ForeColor", typeof(Color), Color.Black, this));

            this.ListProperties.Add(new FakeProperty("Anchor", typeof(AnchorStyles), AnchorStyles.Top | AnchorStyles.Left, this));


            this.ListProperties.Add(new FakeProperty("Visible", typeof(bool), true, this));
            this.ListProperties.Add(new FakeProperty("Enabled", typeof(bool), true, this));

        }


        //nous donne sa coordonné graphique (coin supérieur gauche) ainsi que sa taille, en remontant tout ses parents.
        //certaint contrôles ne sont pas affectés par les propriétés Top Left Width ou Height, et à la place de restreindre, partout dans le code, la possibilité de changer les valeurs de ces propriétés, j'ai décidé d'ignorer les valeurs de ces propriétés pour les contrôles concernés.
        //ainsi, cette méthode va ignorer Top Left Width et Height pour les contrôles spéciaux comme les TabPage.
        public virtual Rectangle GetScreenPos()
        {
            Rectangle rep = new Rectangle(this.Left, this.Top, this.Width, this.Height);

            //certains contrôles ne sont pas affectés par les propriétés Left et Top.
            //ces contrôles overrident la méthode GetScreenPos.

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

            }

            return rep;
        }


        //crée un objet dupliqué de this, mais ne lui donne pas de parent
        public virtual FakeControl GetDuplicate()
        {
            FakeControl copy = new FakeControl();

            //on copy les valeurs de nos propriétés
            foreach (FakeProperty fp in this.ListProperties)
            {
                //copy.ListProperties.Add(new FakeProperty(fp.Name, fp.Type, fp.Value, copy));
                copy.SetProperty(fp.Name, fp.Value);
            }

            return FakeControl.GenerateFakeControlFromGeneric(copy);
        }

        //méthode qui dessine le contrôle this sur l'image donné en paramètre en utilisatant l'objet Graphics donné.
        public virtual void Draw(Bitmap img, Graphics g, FakeControlDrawingContext fcdc)
        {
            //on make sure qu'on est visible
            if (this.Visible)
            {
                //on obtient notre position et taille graphique
                Rectangle UpLeftSize = this.GetScreenPos();

                //on se dessine un petit rectangle blanc pour représenté qu'il y a quelque chose ici
                g.DrawRectangle(Pens.White, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);

            }
        }










        //cette méthode prend en paramètre un FakeControl ou un FakeControlContainer générique, et retourne une instance du contrôle correspondant dans GUI.Fake.Controls, selon la valeur indiqué par la propriété ClassName.
        public static FakeControl GenerateFakeControlFromGeneric(FakeControl fc)
        {
            FakeControl rep = null;

            //on commence par créer une instance qui correspond au ClassName indiqué
            switch (fc.ClassName)
            {
                case "Form":
                    rep = new FakeForm();
                    break;
                case "Button":
                    rep = new FakeButton();
                    break;
                case "Label":
                    rep = new FakeLabel();
                    break;
                case "CheckBox":
                    rep = new FakeCheckBox();
                    break;
                case "RadioButton":
                    rep = new FakeRadioButton();
                    break;
                case "TextBox":
                    rep = new FakeTextBox();
                    break;
                case "PictureBox":
                    rep = new FakePictureBox();
                    break;
                case "NumericUpDown":
                    rep = new FakeNumericUpDown();
                    break;
                case "ComboBox":
                    rep = new FakeComboBox();
                    break;
                case "Panel":
                    rep = new FakePanel();
                    break;
                case "GroupBox":
                    rep = new FakeGroupBox();
                    break;
                case "ListBox":
                    rep = new FakeListBox();
                    break;
                case "TabControl":
                    rep = new FakeTabControl();
                    break;
                case "TabPage":
                    rep = new FakeTabPage();
                    break;
                case "MenuStrip":
                    rep = new FakeMenuStrip();
                    break;
                case "ToolStripMenuItem":
                    rep = new FakeToolStripMenuItem();
                    break;
                case "StatusStrip":
                    rep = new FakeStatusStrip();
                    break;
                case "ToolStripStatusLabel":
                    rep = new FakeToolStripStatusLabel();
                    break;
                case "DateTimePicker":
                    rep = new FakeDateTimePicker();
                    break;
                case "FlowLayoutPanel":
                    rep = new FakeFlowLayoutPanel();
                    break;
                default:
                    //todo: utiliser la réflection pour gérer les cas restant/tout les cas.
                    break;
            }

            //si on n'a pas créé le nouveau objet (parce qu'on n'a oublié d'ajouter un nouveau case après l'ajout d'un nouveau contrôle au programme), alors on return l'objet d'origine
            if (rep == null)
            {
                return fc;
            }

            //on copie les propriétés de fc dans rep
            foreach (FakeProperty fp in fc.ListProperties)
            {
                rep.SetProperty(fp.Name, fp.Value);
            }

            //si c'est un control container, on déplace les enfants dans rep
            if (rep is FakeControlContainer)
            {
                FakeControlContainer repcc = (FakeControlContainer)rep;
                FakeControlContainer fcc = (FakeControlContainer)fc;
                //en changeant les contrôles de parent, les liste des enfants seront affectés.
                //pour éviter des problèmes potentiels, on crée une copie de la liste des enfants de fc.
                List<FakeControl> CopyChildren = new List<FakeControl>();
                CopyChildren.AddRange(fcc.Children);
                foreach (FakeControl child in CopyChildren)
                {
                    //on assigne le nouveau parent
                    child.Parent = repcc;
                }
            }

            return rep;
        }


    }
}
