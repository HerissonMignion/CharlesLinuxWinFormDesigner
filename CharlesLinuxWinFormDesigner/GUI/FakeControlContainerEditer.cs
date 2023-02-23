using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using CharlesLinuxWinFormDesigner.GUI.Fake;
using CharlesLinuxWinFormDesigner.GUI.Fake.Controls;
namespace CharlesLinuxWinFormDesigner.GUI
{
    /// <summary>
    /// "controle graphique" pour laisser l'user modifier un FakeControlContainer.
    /// ce "controle" est un panel dans lequel figurent d'autres controles. il y a un grand picture box pour montrer un appercu du controle final (souvant une form).
    /// 
    /// </summary>
    public class FakeControlContainerEditer
    {
        private FakeControlContainer TopFakeCC = null; //le control container le plus haut de la hiérarchie. (souvant une form)
        private FakeControlContainer SelectedFakeCC = null; //le control container dont l'user est actuellement en train de jouer avec ses enfants.

        private FakeControl ShownFakeC = null; //contrôle actuellement sélectionné par l'user. C'est le contrôle dont l'user peut actuellement voir et modifier ses propriétés.


        public void SetTopFakeCC(FakeControlContainer NewTopFakeCC)
        {
            this.TopFakeCC = NewTopFakeCC;
            this.SelectedFakeCC = NewTopFakeCC;
            this.ShownFakeC = NewTopFakeCC;
            this.RefreshImageBox();
            this.MakeFakePropertyEditers();
        }



        //méthode récursive utilitaire pour récupérer tout les control contrainer existant, indépendament de leur visibilité
        private List<FakeControlContainer> GetAllControlContainer()
        {
            return this.GetAllControlContainer(this.TopFakeCC);
        }
        //la liste réponse va contenir le fcc donné en paramètre et tout les autres control container contenu à l'intérieur, récursivement
        private List<FakeControlContainer> GetAllControlContainer(FakeControlContainer fcc)
        {
            List<FakeControlContainer> rep = new List<FakeControlContainer>();
            rep.Add(fcc);
            foreach (FakeControl child in fcc.Children)
            {
                //on regarde si ce child est un control container
                if (child is FakeControlContainer)
                {
                    //puisque ce child est un control container, on ajoute lui et tout ses control container récursivement
                    rep.AddRange(this.GetAllControlContainer((FakeControlContainer)child));
                }
            }
            return rep;
        }






        #region Gestion des fake control et des états d'éditions


        private enum EditState
        {
            DoNothing, //l'utilisateur ne fait actuellement rien. dans le ui, cela correspond à "sélectionner" un contrôle
            Drag,
            Resize,
            Adding, //en train d'ajouter un nouveau controle
        }
        private EditState CurrentEditState = EditState.DoNothing; //l'état d'"édition" actuel. cela concerne surtout this.ImageBox.


        //peut importe notre mode d'édition actuel, il va se charger de détruire tout ce dont on n'a plus besoin et nous mettre en EditState.DoNothing.
        private void CancelCurrentEditState()
        {

            //on se met en drag
            this.CurrentEditState = EditState.DoNothing;
            this.RefreshButtonColors();
        }


        private FakeControl AddingStateNewControl = null; //lorsque nous sommes en mode d'édition, ceci contient le nouveau control que nous devrons ajouter.
        private void SetStateAdding(FakeControl fc)
        {
            this.CancelCurrentEditState();
            //on concerve le nouveau controle dans une propriété privé
            this.AddingStateNewControl = fc;
            this.CurrentEditState = EditState.Adding;
            this.RefreshButtonColors();
        }



        //dans le mode draging, l'user déplace le contrôle actuellement "sélectionné" (this.ShownFakeC). Dans le futur, il se pourait que le mode draging permette aussi de changer la sélection actuelle en plus de déplacer, ce qui serait convénient.
        private Point DragingStateMouseDiff = new Point(0, 0); //quand l'user commence à draguer un contrôle, cette variable est la différence de position du contrôle avec la souris afin de maintenant le contrôle à même distance de la souris pendant le draging.
        private bool DragingStateDragStarted = false; //indique si l'user est actuellement en train de draguer le contrôle (this.ShownFakeC). concrètement cette variable indique le mouse left bouton est down.
        private void SetStateDraging()
        {
            this.CancelCurrentEditState();

            this.CurrentEditState = EditState.Drag;
            this.DragingStateDragStarted = false;
            this.RefreshButtonColors();
        }



        //dans le mode de redimentionnement, l'user change la taille du contrôle actuellement "sélectionné" (this.ShownFakeC)
        private bool ResizingStateResizeStarted = false; //indique si l'user est actuellement en train de changer la taille du contrôle (this.ShownFakeC). donc si le bouton gauche de la souris est down.
        //ces variables indiquent quel côtés du contrôle sont actuellement en train d'être déplacé par la souris.
        //c'est mouse move qui défini les valeurs de ces variables, ce n'est pas mouse down.
        private bool ResizingStateGrabTop = false;
        private bool ResizingStateGrabBottom = false;
        private bool ResizingStateGrabLeft = false;
        private bool ResizingStateGrabRight = false;
        //ces valeurs sont la différence entre la position du coin supérieur gauche - position de la souris
        private int ResizingStateDiffLeft = 0;
        private int ResizingStateDiffTop = 0;
        //ces veleurs sont la différence entre la largeur/hauteur et la position correspondante de la souris
        private int ResizingStateDiffWidth = 0;
        private int ResizingStateDiffHeight = 0;
        private void SetStateResizing()
        {
            this.CancelCurrentEditState();

            this.CurrentEditState = EditState.Resize;
            this.ResizingStateResizeStarted = false;
            this.RefreshButtonColors();
        }




        private void RefreshButtonColors()
        {
            this.ActionDoNothingButton.BackColor = Color.Black;
            this.ActionAddButton.BackColor = Color.Black;
            this.ActionDragButton.BackColor = Color.Black;
            this.ActionResizeButton.BackColor = Color.Black;
            if (this.CurrentEditState == EditState.DoNothing)
            {
                this.ActionDoNothingButton.BackColor = Color.DimGray;
            }
            else if (this.CurrentEditState == EditState.Adding)
            {
                this.ActionAddButton.BackColor = Color.DimGray;
            }
            else if (this.CurrentEditState == EditState.Drag)
            {
                this.ActionDragButton.BackColor = Color.DimGray;
            }
            else if (this.CurrentEditState == EditState.Resize)
            {
                this.ActionResizeButton.BackColor = Color.DimGray;
            }
        }


        #endregion



        #region Interface graphique (la vrai)

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
        private PictureBox ImageBox = null; //picture box pour montrer l'apercu de la form/controlcontainer à l'user et le permettre d'interragire

        private GroupBox ActionGroupBox = null; //un group box pour contenir des bouton comme "ajouter", "déplacer", "redimensionner" et etc... on appele ces boutons des "actions". ce group box est situé en haut de this.ImageBox.
        private Button ActionAddButton = null; //button pour ajouter un nouveau contrôle (dans this.SelectedFakeCC).

        private Button ActionDoNothingButton = null; //button pour remettre l'état d'édition à DoNothing.
        private Button ActionDragButton = null;
        private Button ActionResizeButton = null;
        private Button ActionRemoveButton = null;

        private Button ActionDuplicateButton = null; //duplique le contrôle actuellement sélectionné par l'user


        //en haut à droite de la fenêtre, en haut du group box des propriétés, c'est le group box où l'user peut choisir (this.SelectedFakeCC).
        //dans le code, nous allons l'appeler le "group box secondaire" wow
        private GroupBox SecondaryGroupBox = null;
        private Button ChangeSelectedFCCButton = null; //le button pour changer this.SelectedFakeCC


        private GroupBox PropertiesGroupBox = null; //le group box qui contient les propriétés du contrôle actuellement sélectionné (this.ShownFakeC).
        private Panel PropertiesPanel = null; //nous voulons un scrolling verticale quand les propriétés dépasse le bas du groupbox, cependant les group box ne peuvent pas scroller, alors les fakes property editers sont placé à l'intérieur de ce panel, qui peut scroller, et ce panel est dans le group box.
        private List<FakePropertyEditer> ListFakePropertyEditer = new List<FakePropertyEditer>(); //la liste des fake property editer actuellement dans l'interface graphique.

        //cette méthode crée / recrée la liste graphique des fake property editer.
        //doit être appelé quand this.ShownFakeC change.
        private void MakeFakePropertyEditers()
        {
            this.PropertiesGroupBox.Visible = false;
            this.PropertiesPanel.AutoScroll = false; //on désactive et réactive le auto scrolling pour régler un petit bug avec la largeur des contrôles.

            //on détruit tout les editers précédant
            while (this.ListFakePropertyEditer.Count > 0)
            {
                FakePropertyEditer fpe = this.ListFakePropertyEditer[0];
                fpe.Parent = null;
                fpe.Dispose();
                this.ListFakePropertyEditer.RemoveAt(0);
            }

            int currentY = 2; //position verticale où mettre le prochain fake property editer
            //on contruit maintenant les nouveaux fake property editer
            foreach (FakeProperty fp in this.ShownFakeC.ListProperties)
            {
                FakePropertyEditer fpe = new FakePropertyEditer(fp, this);
                fpe.Parent = this.PropertiesPanel;
                fpe.Width = fpe.Parent.Width - 10;
                fpe.Left = 5;
                fpe.Top = currentY;
                fpe.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;


                //on doit concerver le fake property editer pour les détruire plus tard
                this.ListFakePropertyEditer.Add(fpe);
                currentY += fpe.Height + 2;
            }

            this.PropertiesPanel.AutoScroll = true;
            this.PropertiesGroupBox.Visible = true;
        }



        #endregion


        public FakeControlContainerEditer(FakeControlContainer TopFakeCC)
        {
            this.TopFakeCC = TopFakeCC;
            this.SelectedFakeCC = TopFakeCC; //c'est le top parent qui est le premier control container dans lequel on ajoute des contrôles.
            this.ShownFakeC = TopFakeCC; //le premier contrôle sélectionné et le parent.

            ////on crée notre interface graphique
            this.MainPanel = new Panel();
            this.MainPanel.Size = new Size(300, 300);
            this.MainPanel.BackColor = Color.Black;



            this.ImageBox = new PictureBox();
            this.ImageBox.Parent = this.MainPanel;
            this.ImageBox.Location = new Point(0, 50);
            this.ImageBox.Height = this.MainPanel.Height - this.ImageBox.Top;
            this.ImageBox.Width = this.MainPanel.Width - 300;
            this.ImageBox.BackColor = Color.FromArgb(16, 16, 16);
            this.ImageBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            this.ImageBox.SizeChanged += new EventHandler(this.ImageBox_SizeChanged);
            this.ImageBox.MouseDown += new MouseEventHandler(this.ImageBox_MouseDown);
            this.ImageBox.MouseDoubleClick += new MouseEventHandler(this.ImageBox_MouseDoubleClick);
            this.ImageBox.MouseUp += new MouseEventHandler(this.ImageBox_MouseUp);
            this.ImageBox.MouseMove += new MouseEventHandler(this.ImageBox_MouseMove);


            //le group box des "actions"
            this.ActionGroupBox = new GroupBox();
            this.ActionGroupBox.Parent = this.MainPanel;
            this.ActionGroupBox.Top = 0;
            this.ActionGroupBox.Left = this.ImageBox.Left;
            this.ActionGroupBox.Height = this.ImageBox.Top - this.ActionGroupBox.Top;
            this.ActionGroupBox.Width = this.ImageBox.Width;
            this.ActionGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.ActionGroupBox.Text = "Actions";

            ////maintenant on fait les "actions" button
            this.ActionAddButton = new Button();
            this.ActionAddButton.Parent = this.ActionGroupBox;
            this.ActionAddButton.Location = new Point(5, 15); // 5, 15
            this.ActionAddButton.Size = new Size(100, this.ActionGroupBox.Height - this.ActionAddButton.Top - 5);
            this.ActionAddButton.Text = "Ajouter";
            this.ActionAddButton.BackColor = Color.FromArgb(64, 64, 64);
            this.ActionAddButton.Click += new EventHandler(this.ActionAddButton_Click);

            this.ActionDoNothingButton = new Button();
            this.ActionDoNothingButton.Parent = this.ActionGroupBox;
            this.ActionDoNothingButton.Top = this.ActionAddButton.Top;
            this.ActionDoNothingButton.Left = this.ActionAddButton.Left + this.ActionAddButton.Width + 20; //on le met un peu plus à l'écart pour isoler un peu le bouton ajouter.
            this.ActionDoNothingButton.Size = this.ActionAddButton.Size;
            this.ActionDoNothingButton.Text = "Sélectionner";
            this.ActionDoNothingButton.Click += new EventHandler(this.ActionDoNothingButton_Click);

            this.ActionDragButton = new Button();
            this.ActionDragButton.Parent = this.ActionGroupBox;
            this.ActionDragButton.Top = this.ActionDoNothingButton.Top;
            this.ActionDragButton.Left = this.ActionDoNothingButton.Left + this.ActionDoNothingButton.Width + 5;
            this.ActionDragButton.Size = this.ActionDoNothingButton.Size;
            this.ActionDragButton.Text = "Déplacer";
            this.ActionDragButton.Click += new EventHandler(this.ActionDragButton_Click);

            this.ActionResizeButton = new Button();
            this.ActionResizeButton.Parent = this.ActionGroupBox;
            this.ActionResizeButton.Top = this.ActionDragButton.Top;
            this.ActionResizeButton.Left = this.ActionDragButton.Left + this.ActionDragButton.Width + 5;
            this.ActionResizeButton.Size = this.ActionDragButton.Size;
            this.ActionResizeButton.Text = "Redimensionner";
            this.ActionResizeButton.Click += new EventHandler(ActionResizeButton_Click);

            this.ActionRemoveButton = new Button();
            this.ActionRemoveButton.Parent = this.ActionGroupBox;
            this.ActionRemoveButton.Top = this.ActionResizeButton.Top;
            this.ActionRemoveButton.Left = this.ActionResizeButton.Left + this.ActionResizeButton.Width + 20;
            this.ActionRemoveButton.Size = this.ActionResizeButton.Size;
            this.ActionRemoveButton.Text = "Supprimer";
            this.ActionRemoveButton.Click += new EventHandler(this.ActionRemoveButton_Click);

            this.ActionDuplicateButton = new Button();
            this.ActionDuplicateButton.Parent = this.ActionGroupBox;
            this.ActionDuplicateButton.Top = this.ActionRemoveButton.Top;
            this.ActionDuplicateButton.Left = this.ActionRemoveButton.Left + this.ActionRemoveButton.Width + 5;
            this.ActionDuplicateButton.Size = this.ActionRemoveButton.Size;
            this.ActionDuplicateButton.Text = "Dupliquer";
            this.ActionDuplicateButton.Click += new EventHandler(this.ActionDuplicateButton_Click);


            ////le "secondary" group box, où l'user peut changer this.SelectedFakeCC
            this.SecondaryGroupBox = new GroupBox();
            this.SecondaryGroupBox.Parent = this.MainPanel;
            this.SecondaryGroupBox.Top = this.ActionGroupBox.Top; //0;
            this.SecondaryGroupBox.Left = this.ImageBox.Left + this.ImageBox.Width + 2;
            this.SecondaryGroupBox.Width = this.MainPanel.Width - 2 - this.SecondaryGroupBox.Left;
            this.SecondaryGroupBox.Height = 90;
            this.SecondaryGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.SecondaryGroupBox.Text = "Actions secondaires";

            //le button pour sélectionner this.SelectedFakeCC
            this.ChangeSelectedFCCButton = new Button();
            this.ChangeSelectedFCCButton.Parent = this.SecondaryGroupBox;
            this.ChangeSelectedFCCButton.Location = new Point(2, 15);
            this.ChangeSelectedFCCButton.Size = new Size(200, 40);
            this.ChangeSelectedFCCButton.BackColor = Color.DimGray;
            this.ChangeSelectedFCCButton.Text = "Changer le ControlContainer actuel";
            this.ChangeSelectedFCCButton.Click += new EventHandler(this.ChangeSelectedFCCButton_Click);


            //// les propriétés
            this.PropertiesGroupBox = new GroupBox();
            this.PropertiesGroupBox.Parent = this.MainPanel;
            this.PropertiesGroupBox.Top = this.SecondaryGroupBox.Top + this.SecondaryGroupBox.Height + 5;
            this.PropertiesGroupBox.Left = this.ImageBox.Left + this.ImageBox.Width + 2;
            this.PropertiesGroupBox.Width = this.MainPanel.Width - 2 - this.PropertiesGroupBox.Left;
            this.PropertiesGroupBox.Height = this.MainPanel.Height - 2 - this.PropertiesGroupBox.Top;
            this.PropertiesGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            this.PropertiesGroupBox.Text = "Propriétés";

            this.PropertiesPanel = new Panel();
            this.PropertiesPanel.Parent = this.PropertiesGroupBox;
            this.PropertiesPanel.BackColor = Color.Black;
            this.PropertiesPanel.Dock = DockStyle.Fill;
            this.PropertiesPanel.AutoScroll = true;
            //this.PropertiesPanel.VerticalScroll.Visible = true;
            //this.PropertiesPanel.VerticalScroll.Enabled = true;



            //fin
            this.RefreshButtonColors();
            this.MakeFakePropertyEditers();
        }


        #region variables graphique réutilisé

        private Pen WideWhitePen = new Pen(Color.White, 3f); //le wide pen utilisé pour montrer à l'user, quand il déplace la souris, quel est le contrôle actuellement dessous la souris.
        private Pen WideBluePen = new Pen(Color.Blue, 3f); //le wide pen utilisé pour montrer à l'user quel est le contrôle actuellement sélectionné.
        private Pen WideRedPen = new Pen(Color.Red, 3f); //le wide pen utilisé pour montrer à l'user quel est le contrôle container actuellement sélectionné.
        private Font MainFont = new Font("Consolas", 10f);

        private Brush TransparentBlackBackGround = new SolidBrush(Color.FromArgb(128, 0, 0, 0)); //quand l'user effectue une action, on met un arrière plan sombre pour mettre en lumière le changement qu'il effectue.


        #endregion

        #region Events

        private void ImageBox_SizeChanged(object sender, EventArgs e)
        {
            this.RefreshImageBox();
        }
        private void ImageBox_MouseDown(object sender, MouseEventArgs e)
        {
            //on check notre état actuel
            if (this.CurrentEditState == EditState.DoNothing)
            {
                //si l'user fait un click gauche, alors il sélectionne un contrôle
                if (e.Button == MouseButtons.Left)
                {
                    //on check s'il y a un contrôle sous la souris
                    FakeControl fc = this.SelectedFakeCC.GetControlUnderScreenPos(e.Location);
                    //s'il y a un contrôle sous la souris, alors maintenant c'est ce contrôle qui est sélectionné
                    if (fc != null)
                    {
                        this.ShownFakeC = fc;
                        this.MakeFakePropertyEditers();
                    }
                    //sinon c'est le parent (selui dont nous modifions actuellement ses enfants) qui sera le nouveau contrôle sélectionné
                    else
                    {
                        this.ShownFakeC = this.SelectedFakeCC;
                        this.MakeFakePropertyEditers();
                    }
                    this.RefreshImageBox();
                }
                //si l'user fait un click droit, le contrôle qui se trouve sous la souris est envoyé à la fin de la liste des contrôles. cette fonctionnalité permet à l'user de retirer les contrôles qui sont devant d'autres contrôles.
                else if (e.Button == MouseButtons.Right)
                {
                    //on check si y'a un control dessous la souris
                    FakeControl fc = this.SelectedFakeCC.GetControlUnderScreenPos(e.Location);
                    //s'il y a un contrôle sous la souris, on l'envois à la fin de la liste des contrôles
                    if (fc != null)
                    {
                        FakeControlContainer OldParent = fc.Parent;
                        fc.RemoveFromParent();
                        fc.Parent = OldParent;
                        this.RefreshImageBox();
                    }

                }
            }
            //en mode addition, le click gauche dépose le control que l'user est actuellement en train d'en choisir la location
            else if (this.CurrentEditState == EditState.Adding)
            {
                //si l'user click sur le bouton gauche, alors on dépose le control
                if (e.Button == MouseButtons.Left)
                {
                    FakeControl fc = this.AddingStateNewControl;
                    //on obtient la coordonnée de la souris relativement à this.SelectedFakeCC
                    Point mvposRelativeToSFCC = this.SelectedFakeCC.GetRelativePosFromScreenPos(e.Location);
                    //on met le nouveau fake control à cette position
                    fc.Left = mvposRelativeToSFCC.X;
                    fc.Top = mvposRelativeToSFCC.Y;
                    //on l'ajoute à son parent
                    fc.Parent = this.SelectedFakeCC;
                    //on fait en sorte que le nouveau contrôle ajouté est le contrôle sélectionné par l'user
                    this.ShownFakeC = fc;
                    this.MakeFakePropertyEditers();

                    //on refresh l'image
                    this.CancelCurrentEditState();
                    this.RefreshImageBox();
                }
                //si l'user clique sur le bouton droit, alors on annule l'ajout du nouveau contrôle
                else if (e.Button == MouseButtons.Right)
                {
                    this.CancelCurrentEditState();
                }

            }
            //en mode draging, le click gauche commence le déplacement du contrôle
            else if (this.CurrentEditState == EditState.Drag)
            {
                //c'est le bouton gauche qui commence le draging
                if (e.Button == MouseButtons.Left)
                {
                    //on conserve la différence entre la position du contrôle et la position de la souris.
                    //on pourait utiliser la position locale, mais utiliser la position graphique est moins susceptible d'avoir des bugs dans le futur, quand l'user va pouvoir modifier this.SelectedFakeCC.
                    //donc nous utilisons la position graphique pour calculer la différence de position.

                    //nous obtenons la position graphique du contrôle actuellement sélectionné (this.ShownFakeC).
                    Rectangle UpLeftSize = this.ShownFakeC.GetScreenPos();
                    this.DragingStateMouseDiff = new Point(e.X - UpLeftSize.X, e.Y - UpLeftSize.Y);

                    //on indique que l'user drag le contrôle.
                    this.DragingStateDragStarted = true;
                    //maintenant, mouse move va ajuster le controle pour qu'il suive la souris.

                }
            }
            //en mode resizing, le click gauche commence le redimensionnement du contrôle
            else if (this.CurrentEditState == EditState.Resize)
            {
                //c'est le bouton gauche qui commence le redimentionnement
                if (e.Button == MouseButtons.Left)
                {
                    this.ResizingStateResizeStarted = true;

                }
            }
        }
        private void ImageBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.CurrentEditState == EditState.DoNothing)
            {
                //le double click gauche permet de change this.SelectedFakeCC. C'est à dire le contrôle dont nous modifions actuellement les enfants. c'est une fonctionnalité raccourci pour changer le SelectedFakeCC.
                if (e.Button == MouseButtons.Left)
                {

                    //on récupère le contrôle enfant sur lequel l'user a double clické, dessous la souris
                    FakeControl fc = this.SelectedFakeCC.GetControlUnderScreenPos(e.Location);
                    //on check si l'user a double-clické sur un contrôle enfant
                    if (fc != null)
                    {
                        //on check si c'est un control container
                        if (fc is FakeControlContainer)
                        {
                            FakeControlContainer fcc = (FakeControlContainer)fc;

                            //on change le control container actuel (this.SelectedFakeCC) pour le contrôle indiqué par la souris.
                            this.SelectedFakeCC = fcc;
                            //on sélectionne ce control
                            this.ShownFakeC = fcc;

                            //on refresh l'image et les propriétés
                            this.RefreshImageBox();
                            this.MakeFakePropertyEditers();
                        }

                    }
                    //l'user n'a pas double clické sur un contrôle enfant
                    else
                    {
                        //on obtient la position graphique de this.SelectedFakeCC
                        Rectangle UpLeftSize = this.SelectedFakeCC.GetScreenPos();
                        //on check si la souris est à l'intérieur du control container
                        if (UpLeftSize.X <= e.X && e.X < UpLeftSize.X + UpLeftSize.Width && UpLeftSize.Y <= e.Y && e.Y < UpLeftSize.Y + UpLeftSize.Height)
                        {
                            //l'user a double-clické à l'intérieur du control container actuel

                        }
                        //si l'user a fait un double click à l'extérieur de this.SelcetedFakeCC, alors l'user veut monter d'un parent dans la hiérarchie.
                        else
                        {
                            //on s'assure qu'il y a un parent avant d'essayer de monter d'un niveau
                            if (this.SelectedFakeCC.Parent != null)
                            {
                                //on change le control container actuel (this.SelectedFakeCC) pour le parent de notre this.SelectedFakeCC.
                                this.SelectedFakeCC = this.SelectedFakeCC.Parent;
                                //on sélectionne ce control
                                this.ShownFakeC = this.SelectedFakeCC;

                                //on refresh l'image et les propriétés
                                this.RefreshImageBox();
                                this.MakeFakePropertyEditers();

                            }
                        }

                    }



                }

            }
        }
        private void ImageBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.CurrentEditState == EditState.Drag)
            {
                //le user drag les contrôles avec le bouton gauche de la souris.
                if (e.Button == MouseButtons.Left)
                {
                    //this.DragingStateDragStarted = false;
                }
                //on fait ça peut importe le bouton de la souris pour éviter des bugs potentiel.
                this.DragingStateDragStarted = false; //on met la valeur à false pour arrêter d'ajuster la position du contrôle avec la souris.
                this.RefreshImageBox();
            }
            else if (this.CurrentEditState == EditState.Resize)
            {

                //on arrête le redimentionnement
                this.ResizingStateResizeStarted = false;
                this.RefreshImageBox();
            }
        }
        private void ImageBox_MouseMove(object sender, MouseEventArgs e)
        {
            //pour être sûr que le curseur soit toujours correcte, en même temps d'éviter de flickering, on met le curseur sur default systématiquement, seulement si on n'est pas en mode resize
            if (this.CurrentEditState != EditState.Resize)
            {
                this.ImageBox.Cursor = Cursors.Default;
            }

            //on check quoi faire selon l'état actuel d'édition
            if (this.CurrentEditState == EditState.DoNothing)
            {
                this.ImageBox.Refresh();
                //on check si y'a un control dessous la souris
                FakeControl fc = this.SelectedFakeCC.GetControlUnderScreenPos(e.Location);
                //s'il y a un control dessous la souris, on donne un feedback à l'user du control dessous la souris
                if (fc != null)
                {
                    //on obtient la coordonnée du coin supérieur gauche du controle actuellement sous la souris
                    Rectangle UpLeftSize = fc.GetScreenPos();
                    //on dessine un rectangle qui montre quel contrôle est sous la souris
                    Graphics g = this.ImageBox.CreateGraphics();
                    g.DrawRectangle(this.WideWhitePen, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);
                    g.DrawRectangle(Pens.DimGray, UpLeftSize.X, UpLeftSize.Y, UpLeftSize.Width, UpLeftSize.Height);

                    //on affiche le classname et le codename du control en bas complètement de l'écran
                    float currentY = (float)(this.ImageBox.Height);
                    //on dessine class name
                    string Text1 = "Class Name = " + fc.ClassName;
                    currentY -= g.MeasureString(Text1, this.MainFont).Height;
                    g.DrawString(Text1, this.MainFont, Brushes.White, 0f, currentY);
                    //on dessine code name
                    string Text2 = "Code Name = " + fc.CodeName;
                    currentY -= g.MeasureString(Text2, this.MainFont).Height;
                    g.DrawString(Text2, this.MainFont, Brushes.White, 0f, currentY);


                }
            }
            else if (this.CurrentEditState == EditState.Adding)
            {
                this.ImageBox.Refresh();
                Graphics g = this.ImageBox.CreateGraphics();
                FakeControl fc = this.AddingStateNewControl; //shortcut pour le nouveau control qu'on s'apprête à ajouter

                //on dessine un appercu du nouveau contrôle
                g.DrawRectangle(Pens.Blue, e.X, e.Y, fc.Width, fc.Height);

            }
            else if (this.CurrentEditState == EditState.Drag)
            {
                //on make sure que le user a commencé à draguer le contrôle
                if (this.DragingStateDragStarted)
                {
                    //nous utilisons la position graphique du contrôle parce qu'il y a moins de chance que des bugs apparaissent à l'avenir.

                    //en utilisant la position de la souris et this.DragingStateMouseDiff, on détermine la position où le contrôle devrait maintenant se trouver
                    Point DestUpLeft = new Point(e.X - this.DragingStateMouseDiff.X, e.Y - this.DragingStateMouseDiff.Y);
                    //nous obtenons la position graphque du contrôle
                    Rectangle UpLeftSize = this.ShownFakeC.GetScreenPos();
                    //on détermine la différence entre la position actuel du contrôle et sa destination (la position où on veut qu'il soit)
                    Point Diff = new Point(DestUpLeft.X - UpLeftSize.X, DestUpLeft.Y - UpLeftSize.Y);
                    //on déplace le contrôle par cette différence
                    this.ShownFakeC.Left += Diff.X;
                    this.ShownFakeC.Top += Diff.Y;

                    //this.RefreshImageBox();
                    this.ImageBox.Refresh();
                    Graphics g = this.ImageBox.CreateGraphics();
                    //on met un arrière plan sombre pour aider à voir la nouvelle taille du contrôle
                    g.FillRectangle(this.TransparentBlackBackGround, 0, 0, this.ImageBox.Width, this.ImageBox.Height);

                    //on crée un nouveau drawing context
                    FakeControlDrawingContext fcdc = new FakeControlDrawingContext();
                    fcdc.TopFakeCC = this.TopFakeCC;
                    fcdc.SelectedFakeCC = this.SelectedFakeCC;
                    fcdc.ShownFakeC = this.ShownFakeC;
                    //on fait dessiner le contrôle
                    this.ShownFakeC.Draw((Bitmap)(this.ImageBox.Image), g, fcdc);

                }

            }
            //en mode resizing, le click gauche commence le redimensionnement du contrôle
            else if (this.CurrentEditState == EditState.Resize)
            {
                //on check si le redimensionnement a commencé.
                //si le redimensionnement a commencé, alors on doit étirer les côtés du contrôle qui ont été choisi.
                if (this.ResizingStateResizeStarted)
                {
                    //shortcut
                    FakeControl fc = this.ShownFakeC;
                    Rectangle UpLeftSize = fc.GetScreenPos();

                    //on conserve la taille précédante pour les contrôles ancrées à l'intérieur
                    int LastWidth = fc.Width;
                    int LastHeight = fc.Height;

                    //on applique le changement de la taille selon les côtés qui sont grabbé
                    if (this.ResizingStateGrabLeft)
                    {
                        fc.Left += e.X + this.ResizingStateDiffLeft - UpLeftSize.X;
                        fc.Width -= e.X + this.ResizingStateDiffLeft - UpLeftSize.X;
                    }
                    if (this.ResizingStateGrabTop)
                    {
                        fc.Top += e.Y + this.ResizingStateDiffTop - UpLeftSize.Y;
                        fc.Height -= e.Y + this.ResizingStateDiffTop - UpLeftSize.Y;
                    }
                    if (this.ResizingStateGrabRight)
                    {
                        fc.Width = this.ResizingStateDiffWidth + e.X;
                    }
                    if (this.ResizingStateGrabBottom)
                    {
                        fc.Height = this.ResizingStateDiffHeight + e.Y;
                    }

                    //on empêche la taille d'être négative
                    if (fc.Width < 2)
                    {
                        fc.Width = 2;
                    }
                    if (fc.Height < 2)
                    {
                        fc.Height = 2;
                    }

                    //maintenant on examine si c'est un control container pour faire ajuster la postion et la taille de tout les contrôles anchrés
                    if (fc is FakeControlContainer)
                    {
                        //cette méthode va récursivement ajuster la taille et la position des contrôles affectés par le redimensionnement
                        ((FakeControlContainer)fc).FixChildrenByAnchor(LastWidth, LastHeight);
                    }


                    //on donne un aperçu de la nouvelle taille.
                    this.ImageBox.Refresh();
                    Graphics g = this.ImageBox.CreateGraphics();
                    //on met un arrière plan sombre pour aider à voir la nouvelle taille du contrôle
                    g.FillRectangle(this.TransparentBlackBackGround, 0, 0, this.ImageBox.Width, this.ImageBox.Height);

                    //on crée un nouveau drawing context
                    FakeControlDrawingContext fcdc = new FakeControlDrawingContext();
                    fcdc.TopFakeCC = this.TopFakeCC;
                    fcdc.SelectedFakeCC = this.SelectedFakeCC;
                    fcdc.ShownFakeC = this.ShownFakeC;
                    //on fait dessiner le contrôle
                    this.ShownFakeC.Draw((Bitmap)(this.ImageBox.Image), g, fcdc);
                    //this.DrawFakeControl((Bitmap)(this.ImageBox.Image), g, this.ShownFakeC);

                }
                //si le redimensionnement n'a pas commencé, alors on doit juste ajuster l'image de la souris (Cursor) et les variables booléenes qui nous indiquent quel côté étirer.
                else
                {
                    //shortcut
                    FakeControl fc = this.ShownFakeC;

                    //on reset les côtés à grabber
                    this.ResizingStateGrabTop = false;
                    this.ResizingStateGrabBottom = false;
                    this.ResizingStateGrabLeft = false;
                    this.ResizingStateGrabRight = false;

                    Rectangle UpLeftSize = fc.GetScreenPos();
                    //on obtient, en plusieurs étapes, la position graphique du milieu du contrôle
                    Point Middle = new Point(UpLeftSize.X, UpLeftSize.Y);
                    Middle.X += UpLeftSize.Width / 2;
                    Middle.Y += UpLeftSize.Height / 2;

                    //maintenant Middle devient la différence entre la position de la souris et la coordonné du milieu du contrôle
                    Middle.X = e.X - Middle.X;
                    Middle.Y = e.Y - Middle.Y;

                    //on détermine quels sont les côtés que le user grab en examinant le vecteur Middle.
                    //on check si la souris est à l'intérieur ou à l'extérieur, car la logique est différente si la souris est à l'intérieur
                    if (UpLeftSize.X <= e.X && e.X < UpLeftSize.X + UpLeftSize.Width && UpLeftSize.Y <= e.Y && e.Y < UpLeftSize.Y + UpLeftSize.Height)
                    {
                        //on check si la souris est à gauche ou à droite
                        if (Math.Abs(Middle.X) > Math.Abs(Middle.Y))
                        {
                            this.ImageBox.Cursor = Cursors.SizeWE;

                            //on détermine si le user est sur le côté de droite ou de gauche.
                            this.ResizingStateGrabLeft = Middle.X < 0;
                            this.ResizingStateGrabRight = !this.ResizingStateGrabLeft;
                        }
                        //la souris est en haut ou en bas
                        else
                        {
                            this.ImageBox.Cursor = Cursors.SizeNS;

                            //on détermine si le user est sur le côté d'en haut ou d'en bas
                            this.ResizingStateGrabTop = Middle.Y < 0;
                            this.ResizingStateGrabBottom = !this.ResizingStateGrabTop;
                        }
                    }
                    //si la souris est à l'extérieur du contrôle
                    else
                    {
                        //on met les bonnes valeurs aux variables booléenes
                        this.ResizingStateGrabLeft = e.X < UpLeftSize.X;
                        this.ResizingStateGrabRight = UpLeftSize.X + UpLeftSize.Width < e.X;
                        this.ResizingStateGrabTop = e.Y < UpLeftSize.Y;
                        this.ResizingStateGrabBottom = UpLeftSize.Y + UpLeftSize.Height < e.Y;

                        ////maintenant on met le bon curseur pour la souris
                        //en haut à gauche ou en bas à droite
                        if ((this.ResizingStateGrabLeft && this.ResizingStateGrabTop) || (this.ResizingStateGrabBottom && this.ResizingStateGrabRight))
                        {
                            this.ImageBox.Cursor = Cursors.SizeNWSE;
                        }
                        //en haut à droite ou en bas à gauche
                        else if ((this.ResizingStateGrabTop && this.ResizingStateGrabRight) || (this.ResizingStateGrabLeft && this.ResizingStateGrabBottom))
                        {
                            this.ImageBox.Cursor = Cursors.SizeNESW;
                        }
                        //en haut ou en bas
                        else if (this.ResizingStateGrabTop || this.ResizingStateGrabBottom)
                        {
                            this.ImageBox.Cursor = Cursors.SizeNS;
                        }
                        //à gauche ou à droite
                        else
                        {
                            this.ImageBox.Cursor = Cursors.SizeWE;
                        }

                    }

                    //on calcul les valeurs qu'on aura besoin pour ajuster la taille du contrôle à la position de la souris
                    this.ResizingStateDiffLeft = UpLeftSize.X - e.X;
                    this.ResizingStateDiffTop = UpLeftSize.Y - e.Y;
                    this.ResizingStateDiffWidth = UpLeftSize.Width - e.X;
                    this.ResizingStateDiffHeight = UpLeftSize.Height - e.Y;




                }
            }
        }


        //lui donner un codename comme "Panel" ou "Button" et il va trouver un numéro à placer à la fin comme "Button2" ou "Panel1" qui n'est pas encore utilisé.
        private string GetUniqueCodeName(string BaseName)
        {
            int i = 1;
            while (true)
            {
                string name = BaseName + i.ToString();
                //on check si ce nom est utilisé
                bool used = this.IsCodeNameUsed(this.TopFakeCC, name);
                //si on est tombé sur un nom qui n'est pas utilisé, alors on le return
                if (!used)
                {
                    return name;
                }
                //si le nom est utilisé, alors on passe au prochain numéro
                i++;
            }
        }

        //recherche récursivement tout les fake control à l'intérieur de fcc et return si ce codename est déjà utilisé
        private bool IsCodeNameUsed(FakeControlContainer fcc, string CodeName)
        {
            //on check d'abors le fake control qu'on s'est fait donné en paramètres
            if (fcc.CodeName == CodeName)
            {
                return true; //le nom est déjà utilisé
            }

            //on check tout ses enfants
            foreach (FakeControl child in fcc.Children)
            {
                //on check d'abors si ce contrôle possède ce nom
                if (child.CodeName == CodeName)
                {
                    return true; //ce nom est déjà utilisé
                }

                //on check si c'est lui même un control container. dans ce cas, il faut relancer la recherche
                if (child is FakeControlContainer)
                {
                    bool subrep = this.IsCodeNameUsed((FakeControlContainer)child, CodeName);
                    //si une correspondance a été trouvé, on return que le codename est déjà utilisé
                    if (subrep)
                    {
                        return true;
                    }
                }

            }

            //si on est rendçu ici, c'est qu'aucune des vérification précédante n'a trouvé un fake control utilisant déjà ce nom.
            return false;
        }

        private void ActionAddButton_Click(object sender, EventArgs e)
        {
            //on crée un ListChooserDialog pour que l'user sélectionne le nouveau contrôle à ajouter
            ListChooserDialog lcd = new ListChooserDialog();
            //on associe à chaque option du dialogue une fonction qui retourne un FakeControl quand la fonction est appelé. c# est un bon langage mais fuck les delegates...
            //quand l'user fait sa sélection, nous prenons, l'object, on le cast en fonction, et on appelle la fonction, qui nous donne le nouveau fake control.

            //les contrôle que nous pouvons ajouter dépendent du type du parent. les tab control, par exemple, ne peuvent contenir seulement des tab page.
            if (this.SelectedFakeCC.ClassName == "TabControl")
            {
                lcd.Add("TabPage", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildTabPage(); }));
            }
            //les menu strip peuvent seulement contenir certains contrôles
            else if (this.SelectedFakeCC.ClassName == "MenuStrip" || this.SelectedFakeCC.ClassName == "ToolStripMenuItem")
            {
                lcd.Add("ToolStripMenuItem", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildToolStripMenuItem(); }));
            }
            else if (this.SelectedFakeCC.ClassName == "StatusStrip")
            {
                lcd.Add("ToolStripStatusLabel", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildToolStripStatusLabel(); }));
            }
            else
            {
                lcd.Add("Button", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildButton(); }));
                lcd.Add("Label", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildLabel(); }));
                lcd.Add("CheckBox", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildCheckBox(); }));
                lcd.Add("RadioButton", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildRadioButton(); }));
                lcd.Add("TextBox", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildTextBox(); }));
                lcd.Add("PictureBox", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildPictureBox(); }));
                lcd.Add("NumericUpDown", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildNumericUpDown(); }));
                lcd.Add("ComboBox", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildComboBox(); }));
                lcd.Add("DateTimePicker", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildDateTimePicker(); }));
                lcd.Add("Panel", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildPanel(); }));
                lcd.Add("FlowLayoutPanel", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildFlowLayoutPanel(); }));
                lcd.Add("GroupBox", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildGroupBox(); }));
                lcd.Add("ListBox", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildListBox(); }));
                lcd.Add("TabControl", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildTabControl(); }));
                lcd.Add("MenuStrip", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildMenuStrip(); }));
                lcd.Add("StatusStrip", (Func<FakeControl>)(() => { return FakeControlBuilder.BuildStatusStrip(); }));
            }

            //lcd.Add("GroupBox", FakeControlBuilder.BuildTextBox());
            //lcd.Add("PictureBox", FakeControlBuilder.BuildTextBox());
            //lcd.Add("NumericUpDown", FakeControlBuilder.BuildTextBox());
            //lcd.Add("TextBox", FakeControlBuilder.BuildTextBox());
            lcd.Message = "Sélectionnez le type du contrôle à ajouter";
            //on prépare la fenêtre sur laquel se centrer
            Form CenterForm = null;
            if (this.Parent is Form)
            {
                CenterForm = (Form)(this.Parent);
            }
            //on affiche le dialogue
            ListChooserDialogEntry rep = lcd.ShowDialog(CenterForm);

            //lcd.ShowDialog return null si l'user a annulé
            if (rep != null)
            {
                //selon la réponse, on crée le contrôle que l'user a sélectionné
                //FakeControl newfc = (FakeControl)(rep.Tag);
                FakeControl newfc = ((Func<FakeControl>)(rep.Tag))();
                //on obtient un CodeName unique pour ce nouveau contrôle
                newfc.CodeName = this.GetUniqueCodeName(newfc.ClassName);

                this.SetStateAdding(newfc);

            }


        }

        private void ActionDoNothingButton_Click(object sender, EventArgs e)
        {
            this.CancelCurrentEditState();
        }
        private void ActionDragButton_Click(object sender, EventArgs e)
        {
            this.SetStateDraging();
        }
        private void ActionResizeButton_Click(object sender, EventArgs e)
        {
            this.SetStateResizing();
        }

        private void ActionRemoveButton_Click(object sender, EventArgs e)
        {
            //on make sure que ce n'est pas le top level control
            if (this.ShownFakeC != this.TopFakeCC)
            {
                //on demande à l'user de confirmer l'action, parce que je ne vais pas programmer de ctrl+z
                DialogResult rep = MessageBox.Show("Voulez-vous supprimer ce contrôle ?", "Confirmation", MessageBoxButtons.YesNo);
                if (rep == DialogResult.Yes)
                {
                    //nous avons déjà établit que le contrôle actuellement sélectionné n'est pas this.TopFakeCC.

                    //après avoir supprimé le contrôle, la sélection doit être déplacé sur un autre contrôle.
                    //la logique dépend de si le contrôle que nous supprimons est this.SelectedFakeCC.
                    //si le contrôle actuellement sélectionné est this.SelectedFakeCC, alors on le supprime et on met le focus sur un autre enfant de son parent ou sur son parent.
                    //sinon, on le supprime et on met le focus sur un autre enfant de this.SelectedFakeCC, ou sur this.SelectedFakeCC s'il n'y a plus d'enfants.
                    if (this.ShownFakeC == this.SelectedFakeCC)
                    {
                        //puisque nous supprimons this.SelectedFakeCC, le nouveau SelectedFakeCC, après l'opération, sera son parent
                        FakeControlContainer NewSelectedFakeCC = this.SelectedFakeCC.Parent;

                        //on retire le contrôle de son parent
                        this.ShownFakeC.RemoveFromParent();

                        //on met notre nouveau this.SelectedFakeCC
                        this.SelectedFakeCC = NewSelectedFakeCC;

                    }
                    //l'user n'est pas en train de supprimer this.SelectedFakeCC.
                    else
                    {
                        //nous supprimons this.ShownFakeC.
                        this.ShownFakeC.RemoveFromParent();

                    }

                    //nous sélectionnons un autre enfant du this.SelectedFakeCC, ou this.SelectedFakeCC lui-même si il n'a plus d'autres enfants.
                    if (this.SelectedFakeCC.Children.Count > 0)
                    {
                        //nous sélectionnons son premier enfant
                        this.ShownFakeC = this.SelectedFakeCC.Children[0];
                    }
                    else
                    {
                        //on sélectionne this.SelectedFakeCC lui-même puisqu'il n'a plus d'enfants.
                        this.ShownFakeC = this.SelectedFakeCC;
                    }

                    //on refresh l'image et les propriété
                    this.RefreshImageBox();
                    this.MakeFakePropertyEditers();

                }
            }
            else
            {
                MessageBox.Show("Vous ne pouvez pas supprimer le contrôle principale");
            }
        }

        private void ActionDuplicateButton_Click(object sender, EventArgs e)
        {
            //on make sure que l'user n'est pas en train de copier le parent, parce qu'on ne veut pas mettre des form dans la form principale
            if (this.ShownFakeC != this.SelectedFakeCC)
            {
                //l'action du bouton dupliquer est de créer une copie de l'enfant actuellement sélectionné (this.ShownFakeC), et de l'ajouter dans le parent actuel.
                FakeControl copy = this.ShownFakeC.GetDuplicate();
                //on l'ajoute au parent
                copy.Parent = this.SelectedFakeCC;
                //on le téléporte à la coordonné 0, 0 pour que l'user voit que l'objet a été dupliqué
                copy.Left = 0;
                copy.Top = 0;
                //on change son codename parce que sinon ça va faire des erreur en c#
                copy.CodeName = this.GetUniqueCodeName(copy.ClassName);
                //on le sélectionne
                this.ShownFakeC = copy;
                //on refresh tout
                this.RefreshImageBox();
                this.MakeFakePropertyEditers();
            }
            else
            {
                MessageBox.Show("Vous ne pouvez pas dupliquer le parent à l'intérieur de lui-même.");
            }
        }



        private void ChangeSelectedFCCButton_Click(object sender, EventArgs e)
        {
            //ce button permet à l'user de changer this.SelectedFakeCC en lui donnant un dialog ListChooserDialog
            ListChooserDialog lcd = new ListChooserDialog();
            lcd.Message = "Sélectionnez le ControlContainer pour lequel vous modifiez ses enfants.";

            //on obtient tout les control container existant
            List<FakeControlContainer> ListFCC = this.GetAllControlContainer();
            //on les ajoutes comme choix dans le dialogue
            foreach (FakeControlContainer fcc in ListFCC)
            {
                lcd.Add(fcc.ClassName + " " + fcc.CodeName, fcc);
            }

            //on prépare la fenêtre pour centrer le dialogue
            Form CenterForm = null;
            if (this.Parent is Form)
            {
                CenterForm = (Form)this.Parent;
            }
            //on fait afficher le dialogue et on récupère la réponse
            ListChooserDialogEntry rep = lcd.ShowDialog(CenterForm);
            //on make sure que l'user a donné une réponse
            if (rep != null)
            {
                //on change this.SelectedFakeCC et on "sélectionne" ce contrôle (this.ShownFakeC)
                this.SelectedFakeCC = (FakeControlContainer)(rep.Tag);
                this.ShownFakeC = (FakeControl)(rep.Tag);
                this.MakeFakePropertyEditers();
                //on doit refresher l'image
                this.RefreshImageBox();
                //on se met en "mode" "sélection"
                this.CancelCurrentEditState();
            }


        }




        #endregion
        #region Dessin graphique

        //recrée l'image affiché dans this.ImageBox, notre gros picture box.
        public void RefreshImageBox()
        {
            int imgWidth = this.ImageBox.Width;
            int imgHeight = this.ImageBox.Height;
            if (imgWidth < 50) { imgWidth = 50; }
            if (imgHeight < 50) { imgHeight = 50; }
            Bitmap img = new Bitmap(imgWidth, imgHeight);
            Graphics g = Graphics.FromImage(img);
            g.Clear(Color.FromArgb(32, 32, 32));


            //on crée le drawing context
            FakeControlDrawingContext fcdc = new FakeControlDrawingContext();
            fcdc.TopFakeCC = this.TopFakeCC;
            fcdc.SelectedFakeCC = this.SelectedFakeCC;
            fcdc.ShownFakeC = this.ShownFakeC;

            //on lance le dessin des fake contrôles
            //this.DrawFakeControl(img, g, this.TopFakeCC);
            this.TopFakeCC.Draw(img, g, fcdc);

            //on fait dessiner les lignes oranges qui montrent à l'user le control container actuellement sélectionné
            this.DrawHighlightControl(img, g, this.SelectedFakeCC, Pens.Orange, this.WideRedPen);

            //on fait dessiner le ShownFakeC pour make sure qu'il est en face des lignes orange
            this.ShownFakeC.Draw(img, g, fcdc);

            //on fait dessiner les lignes qui montrent à l'user quel est le contrôle actuellement sélectionné
            this.DrawHighlightControl(img, g, this.ShownFakeC, Pens.Cyan, this.WideBluePen);



            //fin
            g.Dispose();
            if (this.ImageBox.Image != null) { this.ImageBox.Image.Dispose(); }
            this.ImageBox.Image = img;
            GC.Collect();
        }



        //dessine les lignes bleues ou orange ou autres couleurs qui montrent à l'user le contrôle actuellement sélectionné.
        //inner et outer pen sont les Pen à utiliser pour la ligne "intérieur" et "extérieur" de la "sélection" affiché à l'écran
        private void DrawHighlightControl(Bitmap img, Graphics g, FakeControl fc, Pen innerPen, Pen outerPen)
        {
            //on obtient la coordonnée du coin supérieur gauche graphiquement
            Rectangle UpLeftSize = fc.GetScreenPos();

            //un petit delta pour s'éloigner un peu du contrôle
            int delta = 5;

            //on dessine les lignes horizontales
            g.DrawLine(outerPen, 0, UpLeftSize.Y, UpLeftSize.X - delta, UpLeftSize.Y);
            g.DrawLine(outerPen, UpLeftSize.X + UpLeftSize.Width + delta, UpLeftSize.Y, img.Width, UpLeftSize.Y);
            g.DrawLine(outerPen, 0, UpLeftSize.Y + UpLeftSize.Height, UpLeftSize.X - delta, UpLeftSize.Y + UpLeftSize.Height);
            g.DrawLine(outerPen, UpLeftSize.X + UpLeftSize.Width + delta, UpLeftSize.Y + UpLeftSize.Height, img.Width, UpLeftSize.Y + UpLeftSize.Height);
            g.DrawLine(innerPen, 0, UpLeftSize.Y, UpLeftSize.X - delta, UpLeftSize.Y);
            g.DrawLine(innerPen, UpLeftSize.X + UpLeftSize.Width + delta, UpLeftSize.Y, img.Width, UpLeftSize.Y);
            g.DrawLine(innerPen, 0, UpLeftSize.Y + UpLeftSize.Height, UpLeftSize.X - delta, UpLeftSize.Y + UpLeftSize.Height);
            g.DrawLine(innerPen, UpLeftSize.X + UpLeftSize.Width + delta, UpLeftSize.Y + UpLeftSize.Height, img.Width, UpLeftSize.Y + UpLeftSize.Height);
            //on dessine les lignes verticales
            g.DrawLine(outerPen, UpLeftSize.X, 0, UpLeftSize.X, UpLeftSize.Y - delta);
            g.DrawLine(outerPen, UpLeftSize.X, UpLeftSize.Y + UpLeftSize.Height + delta, UpLeftSize.X, img.Height);
            g.DrawLine(outerPen, UpLeftSize.X + UpLeftSize.Width, 0, UpLeftSize.X + UpLeftSize.Width, UpLeftSize.Y - delta);
            g.DrawLine(outerPen, UpLeftSize.X + UpLeftSize.Width, UpLeftSize.Y + UpLeftSize.Height + delta, UpLeftSize.X + UpLeftSize.Width, img.Height);
            g.DrawLine(innerPen, UpLeftSize.X, 0, UpLeftSize.X, UpLeftSize.Y - delta);
            g.DrawLine(innerPen, UpLeftSize.X, UpLeftSize.Y + UpLeftSize.Height + delta, UpLeftSize.X, img.Height);
            g.DrawLine(innerPen, UpLeftSize.X + UpLeftSize.Width, 0, UpLeftSize.X + UpLeftSize.Width, UpLeftSize.Y - delta);
            g.DrawLine(innerPen, UpLeftSize.X + UpLeftSize.Width, UpLeftSize.Y + UpLeftSize.Height + delta, UpLeftSize.X + UpLeftSize.Width, img.Height);


        }






        #endregion



    }
}
