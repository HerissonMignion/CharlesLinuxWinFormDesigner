using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using CharlesLinuxWinFormDesigner.GUI.Fake;
namespace CharlesLinuxWinFormDesigner.GUI
{
    /// <summary>
    /// Cette classe sert à permettre à l'user de modifier la valeur d'une fake property, de la même façon que FakeControlContainerEditer permet à l'user de modifier un FakeControlContainer.
    /// </summary>
    public class FakePropertyEditer
    {
        private FakeProperty FakeP = null; //la fake property dont this sert à permet à l'user de modifier sa valeur.
        private FakeControlContainerEditer FakeCCEditer = null; //le control container editer. nous en avons besoin pour rafraichir son image.

        //indique si la propriété (dont this sert à modifier) peut être modifier textuellement, avec un textbox
        private bool IsTextEditable()
        {
            return this.FakeP.Type == typeof(int) || this.FakeP.Type == typeof(string) || this.FakeP.Type == typeof(decimal);
        }

        //si this est une propriété modifiable textuellement, alors cette valeur indique si l'user est actuellement en train de modifier sa valeur.
        private bool IsUserEditingValue = false;

        //examine si la valeur entré par l'user dans this.ValueTextBox est une valeur valide
        private bool IsUserValueOk()
        {
            //si la propriété est une chaine de texte
            if (this.FakeP.Type == typeof(string))
            {
                return true;
            }
            //si c'est un nombre
            else if (this.FakeP.Type == typeof(int))
            {
                return int.TryParse(this.ValueTextBox.Text, out int result);
            }
            //si c'est un decimal
            else if (this.FakeP.Type == typeof(decimal))
            {
                return decimal.TryParse(this.ValueTextBox.Text, out decimal result);
            }
            return false;
        }



        #region Interface graphique


        public Control Parent
        {
            get
            {
                return this.MainPanel.Parent;
            }
            set
            {
                this.MainPanel.Parent = value;
            }
        }
        public int Left
        {
            get
            {
                return this.MainPanel.Left;
            }
            set
            {
                this.MainPanel.Left = value;
            }
        }
        public int Top
        {
            get
            {
                return this.MainPanel.Top;
            }
            set
            {
                this.MainPanel.Top = value;
            }
        }
        public int Width
        {
            get
            {
                return this.MainPanel.Width;
            }
            set
            {
                this.MainPanel.Width = value;
            }
        }
        public int Height
        {
            get
            {
                return this.MainPanel.Height;
            }
            set
            {
                this.MainPanel.Height = value;
            }
        }
        public AnchorStyles Anchor
        {
            get
            {
                return this.MainPanel.Anchor;
            }
            set
            {
                this.MainPanel.Anchor = value;
            }
        }
        public DockStyle Dock
        {
            get
            {
                return this.MainPanel.Dock;
            }
            set
            {
                this.MainPanel.Dock = value;
            }
        }


        private Panel MainPanel = null; //panel pour contenir toute notre interface graphique

        private Button EditButton = null; //le button pour éditer la valeur, ou pour confirmer la nouvelle valeur entré textuellement par l'user.
        private Button CancelButton = null; //bouton pour annuler la nouvelle valeur entré textuellement par l'user.
        private Label NameLabel = null; //le label qui montre le nom de la propiété
        private Label ValueLabel = null; //le label qui montre la valeur actuelle de la propriété
        private TextBox ValueTextBox = null; //le textbox dans lequel le user met la nouvelle valeur



        //refresh les chaines de texte, les couleurs et tout les reste sur notre interface graphique
        private void RefreshUI()
        {

            ////on repositionne les contrôles, juste pour être sûr de ne pas avoir certains bug
            //si notre propriété est une propriété éditable textuellement, alors le edit button fait seulement la moitier de la hauteur, pour laisser de la place au bouton annuler
            this.EditButton.Height = this.IsTextEditable() ? (this.MainPanel.Height / 2) : this.MainPanel.Height;
            this.EditButton.Width = this.MainPanel.Height;
            this.EditButton.Top = 0;
            this.EditButton.Left = this.MainPanel.Width - this.EditButton.Width;

            this.CancelButton.Top = this.EditButton.Top + this.EditButton.Height;
            this.CancelButton.Height = this.MainPanel.Height / 2;
            this.CancelButton.Width = this.MainPanel.Height;
            this.CancelButton.Left = this.EditButton.Left;

            this.NameLabel.Location = new Point(0, 0);
            this.NameLabel.Width = this.EditButton.Left - this.NameLabel.Left;
            this.NameLabel.Height = this.MainPanel.Height / 2;

            this.ValueLabel.Left = 0;
            this.ValueLabel.Top = this.NameLabel.Top + this.NameLabel.Height;
            this.ValueLabel.Width = this.NameLabel.Width;
            this.ValueLabel.Height = this.MainPanel.Height - this.ValueLabel.Top;

            this.ValueTextBox.Location = this.ValueLabel.Location;
            this.ValueTextBox.Size = this.ValueLabel.Size;


            ////on ajuste les chaines de textes aux valeurs actuelles des propriétés
            this.NameLabel.Text = this.FakeP.Name;
            this.ValueLabel.Text = this.FakeP.Value.ToString();


            ////on gère la visibilité des contrôles
            this.EditButton.Visible = true;
            //on check si la propriété peut être édité textuellement
            if (this.IsTextEditable())
            {
                //quand la propriété peut être édité textuellement, alors on affiche le edit button seulement quand le user est en train de modifier le champ
                this.EditButton.Visible = this.IsUserEditingValue;
            }
            //le cancel button est visible seulement si la propriété est éditable textuellement et si l'user est actuellement en train de la modifer
            this.CancelButton.Visible = this.IsTextEditable() && this.IsUserEditingValue;

            //le label de la valeur n'est pas visible quand l'user est en train de modifier la valeur de la propriété
            this.ValueLabel.Visible = !this.IsUserEditingValue;
            this.ValueTextBox.Visible = this.IsUserEditingValue;



        }

        #endregion


        public FakePropertyEditer(FakeProperty fp, FakeControlContainerEditer fcce)
        {
            this.FakeP = fp;
            this.FakeCCEditer = fcce;

            //on crée nos contrôles
            this.MainPanel = new Panel();
            this.MainPanel.BackColor = Color.Black;
            this.Height = 50;

            this.EditButton = new Button();
            this.EditButton.Parent = this.MainPanel;
            this.EditButton.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            this.EditButton.Text = this.IsTextEditable() ? "Ok" : "...";
            this.EditButton.BackColor = Color.DimGray;
            this.EditButton.Click += new EventHandler(this.EditButton_Click);

            this.CancelButton = new Button();
            this.CancelButton.Parent = this.MainPanel;
            this.CancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.CancelButton.Text = "Annuler";
            this.CancelButton.BackColor = Color.DimGray;
            this.CancelButton.Click += new EventHandler(this.CancelButton_Click);

            this.NameLabel = new Label();
            this.NameLabel.Parent = this.MainPanel;
            this.NameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.NameLabel.BackColor = Color.FromArgb(64, 64, 64);
            this.NameLabel.TextAlign = ContentAlignment.MiddleLeft;

            this.ValueLabel = new Label();
            this.ValueLabel.Parent = this.MainPanel;
            this.ValueLabel.Anchor = this.NameLabel.Anchor;
            this.ValueLabel.BackColor = Color.DimGray;
            this.ValueLabel.TextAlign = ContentAlignment.MiddleLeft;
            this.ValueLabel.Click += new EventHandler(this.ValueLabel_Click);
            if (this.IsTextEditable())
            {
                this.ValueLabel.Cursor = Cursors.IBeam;
            }

            this.ValueTextBox = new TextBox();
            this.ValueTextBox.Parent = this.MainPanel;
            this.ValueTextBox.Anchor = this.ValueLabel.Anchor;
            this.ValueTextBox.TextChanged += new EventHandler(this.ValueTextBox_TextChanged);
            this.ValueTextBox.KeyDown += new KeyEventHandler(this.ValueTextBox_KeyDown);



            //on attache l'event pour quand la valeur change
            this.FakeP.ValueChanged += new EventHandler(this.FakeP_ValueChanged);


            this.RefreshUI();
        }


        #region Events

        private void FakeP_ValueChanged(object sender, EventArgs e)
        {
            //quand la valeur de la propriété change, on rafraichi le texte.
            this.RefreshUI();
        }


        private void ValueLabel_Click(object sender, EventArgs e)
        {
            //seulement si this est modifiable textuellement on doit se mettre en mode d'edition textuelle
            if (this.IsTextEditable())
            {
                //le user passe en mode d'édition
                this.IsUserEditingValue = true;
                //on met dans le textbox la valeur actuelle de la propriété
                this.ValueTextBox.Text = this.FakeP.Value.ToString();
                //on fait ajuster le UI
                this.RefreshUI();
            }
        }

        private void ValueTextBox_TextChanged(object sender, EventArgs e)
        {
            //on change la couleur du textbox selon que la valeur soit valide ou non
            if (this.IsUserValueOk())
            {
                this.ValueTextBox.BackColor = Color.White;
                this.ValueTextBox.ForeColor = Color.Black;
            }
            else
            {
                this.ValueTextBox.BackColor = Color.Crimson;
                this.ValueTextBox.ForeColor = Color.White;
            }
        }
        private void ValueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //on check si c'est la touche enter
            if (e.KeyCode == Keys.Return)
            {
                //quand l'user appuit sur enter, on simule un click du edit button
                this.EditButton_Click(this.EditButton, new EventArgs());
            }
            //on check la touche escape
            else if (e.KeyCode == Keys.Escape)
            {
                //quand l'user appuit sur escape, on simule un click du cancel buton
                this.CancelButton_Click(this.CancelButton, new EventArgs());
            }
        }


        private void EditButton_Click(object sender, EventArgs e)
        {
            //on check si nous sommes une propriété editable textuellement
            if (this.IsTextEditable())
            {
                //nous devons vérifier le text entré par l'user et le convertir en le type de la propriété
                if (this.IsUserValueOk())
                {
                    this.IsUserEditingValue = false; //on indique que le user n'est plus en train d'éditer la valeur.

                    //selon le type de la propriété, on doit faire différentes conversion
                    if (this.FakeP.Type == typeof(string))
                    {
                        //on met la nouvelle valeur
                        this.FakeP.Value = this.ValueTextBox.Text;
                    }
                    else if (this.FakeP.Type == typeof(int))
                    {
                        //on fait convertir la valeur
                        bool parsed = int.TryParse(this.ValueTextBox.Text, out int result);
                        if (parsed)
                        {
                            //on met la nouvelle valeur
                            this.FakeP.Value = result;

                            //on fait quelques vérifications sur la valeur entré


                        }
                        //la valeur n'a pas pu être convertie
                        else
                        {
                            MessageBox.Show("Erreur de syncronization entre la validation des données et la faisabilité de convertir les données. Ceci n'est pas votre faute.");
                        }
                    }
                    else if (this.FakeP.Type == typeof(decimal))
                    {
                        //on fait convertir la valeur
                        bool parsed = decimal.TryParse(this.ValueTextBox.Text, out decimal result);
                        if (parsed)
                        {
                            //on met la nouvelle valeur
                            this.FakeP.Value = result;

                            //on fait quelques vérifications sur la valeur entré


                        }
                        //la valeur n'a pas pu être convertie
                        else
                        {
                            MessageBox.Show("Erreur de syncronization entre la validation des données et la faisabilité de convertir les données. Ceci n'est pas votre faute.");
                        }
                    }

                    //nous écoutons l'évènement changed de la propriété, qui va appeler le refresh graphique, alors cet appel est redondant.
                    //this.RefreshUI();
                    //on refresh l'apperçu graphique des contrôles
                    this.FakeCCEditer.RefreshImageBox();
                }
                else
                {
                    MessageBox.Show("La valeur entrée n'est pas valide");
                }
            }
            //la propriété n'est pas modifiable textuellement, nous devons alors afficher à l'user un dialogue qui lui permettera de choisir une valeur valide
            else
            {
                //on check c'est quoi le type de la propriété pour afficher le dialogue approprié
                if (this.FakeP.Type == typeof(Color))
                {
                    ColorDialog cd = new ColorDialog();
                    cd.Color = (Color)(this.FakeP.Value);
                    //on affiche le dialogue et on récupère la réponse
                    DialogResult rep = cd.ShowDialog();
                    if (rep == DialogResult.OK)
                    {
                        //on affecte la nouvelle couleur à la propriété
                        this.FakeP.Value = cd.Color;
                        //on fait refresher l'image
                        this.FakeCCEditer.RefreshImageBox();
                    }
                }
                else if (this.FakeP.Type == typeof(AnchorStyles))
                {
                    AnchorStyleChooserDialog ascd = new AnchorStyleChooserDialog();

                    ascd.Value = (AnchorStyles)(this.FakeP.Value);
                    //on affiche le dialogue
                    ascd.ShowDialog();
                    //on récupère la valeur
                    this.FakeP.Value = ascd.Value;

                }
                else if (this.FakeP.Type == typeof(bool))
                {
                    //on fait juste toggle la valeur booléene
                    this.FakeP.Value = !(bool)(this.FakeP.Value);
                    //on refresh l'image
                    this.FakeCCEditer.RefreshImageBox();
                }
                else if (this.FakeP.Type == typeof(Font))
                {
                    //on crée le dialog et on met la font actuel du controle
                    FontDialog fd = new FontDialog();
                    fd.Font = (Font)(this.FakeP.Value);
                    //on fait choisir une font à l'user et on récupère le dialog result
                    DialogResult rep = fd.ShowDialog();
                    if (rep == DialogResult.OK)
                    {
                        //on donne la font sélectionné à la propriété
                        this.FakeP.Value = fd.Font;
                        //on refresh l'apperçu du contrôle
                        this.FakeCCEditer.RefreshImageBox();
                    }

                }

                //après avoir checké tout les types les plus probables, nous sommes rendû à des trucs plus généraux.

                //on vérifie si c'est une énumération
                else if (this.FakeP.Type.IsEnum)
                {
                    //on prépare notre dialogue
                    ListChooserDialog lcd = new ListChooserDialog();

                    //on utilise la réflection pour récupérer toute les valeurs de cette enum (ce sont les membres à la fois public et statique)
                    string[] members = this.FakeP.Type.GetEnumNames(); //this.FakeP.Type.GetMembers(BindingFlags.Public | BindingFlags.Static);
                    //maintenant on crée une entré dans le dialogue pour chaqune des valeurs possible
                    foreach (string member in members)
                    {
                        lcd.Add(member);
                    }

                    lcd.Message = "Sélectionnez une nouvelle valeur";
                    ListChooserDialogEntry rep = lcd.ShowDialog();
                    //on make sure l'user n'a pas annulé
                    if (rep != null)
                    {
                        //puisqu'on a mis dans le dialogue les noms des valeurs, on fait juste récupérer le nom pour récupérer le membre sélectionné
                        string NewValueSName = rep.DisplayText;


                        //on obtient les valeurs possible de l'énumération
                        var values = this.FakeP.Type.GetEnumValues();
                        object NewValue = null; //on va stocker dans cet objet la nouvelle valeur de l'énumération
                        //on cherche, parmis toutes les valeurs, laquelle correspond au nom sélectionné par l'user
                        foreach (var value in values)
                        {
                            //on check si on est tombé sur la valeur qui correspond au nom
                            if (this.FakeP.Type.GetEnumName(value) == NewValueSName)
                            {
                                //on garde la valeur et on arrête de chercher
                                NewValue = value;
                                break;
                            }
                        }

                        //on met la nouvelle valeur dans la propriété
                        this.FakeP.Value = NewValue;
                        //on refresh l'image
                        this.FakeCCEditer.RefreshImageBox();

                    }

                }



            }
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            //on retire le user du mode d'édition sans appliquer aucun changement
            this.IsUserEditingValue = false;
            //on refresh le ui
            this.RefreshUI();
        }





        #endregion

        public void Dispose()
        {
            this.MainPanel.Dispose();
            this.EditButton.Dispose();
            this.CancelButton.Dispose();
            this.NameLabel.Dispose();
            this.ValueLabel.Dispose();

            //on détruit l'event ValueChanged
            this.FakeP.ValueChanged -= new EventHandler(this.FakeP_ValueChanged);

        }
    }
}
