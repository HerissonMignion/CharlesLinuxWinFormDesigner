using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using CharlesLinuxWinFormDesigner.GUI.Fake;
using CharlesLinuxWinFormDesigner.GUI.Fake.Controls;
using CharlesLinuxWinFormDesigner.GUI.Fake.IO;
using System.Reflection;
namespace CharlesLinuxWinFormDesigner.GUI
{
    /// <summary>
    /// Cet objet est une fenêtre qui permet au programmeur d'éditer des fake control à sa place
    /// </summary>
    public class FormDesigner : Form
    {

        #region Interface Graphique

        private TextBox FilePathTextBox = null;
        private Button OpenButton = null;
        private Button SaveButton = null;
        private Button SaveAsButton = null;
        private Button ExportButton = null;

        private Label NamespaceLabel = null;
        private TextBox NamespaceTextBox = null;


        private FakeControlContainerEditer FakeCCEditer = null; //le contrôle qui se charge de permettre à l'user de modifier sa fake form

        private Button TestButton;


        //refresh les textes et la visibilité et les autres propriétés des contrôles sur notre interface graphique selon l'état interne
        private void RefreshText()
        {
            if (this.FileOpened)
            {
                this.FilePathTextBox.Text = this.FilePath;
                this.SaveButton.Enabled = true;
                this.ExportButton.Enabled = true;
            }
            else
            {
                this.FilePathTextBox.Text = "";
                this.SaveButton.Enabled = false;
                this.ExportButton.Enabled = false;
            }
        }



        #endregion
        #region Fichier actuellement ouvert et interface graphique fake

        private string FilePath = "";
        private bool FileOpened = false; //indique si nous sommes actuellement "syncronisé" à sauvgarder le gui de l'user à l'emplacement de this.FilePath. N'indique PAS si this.FilePath est un fichier qui existe.

        private void Open(string FilePath)
        {
            //on fait ouvrir le fichier 
            SaveFormat1Data sfd = SaveFormat1.Open(FilePath);

            //on concerve dans nos propriétés privés les informations importante
            this.FileOpened = true;
            this.FilePath = FilePath;
            this.NamespaceTextBox.Text = sfd.Namespace;
            this.TopFakeCC = sfd.TopFakeCC;
            //on fait des refresh
            this.RefreshText();
            this.FakeCCEditer.SetTopFakeCC(this.TopFakeCC);
        }



        private void Save(string FilePath)
        {
            //on crée l'objet qui contient les informations de sauvegarde
            SaveFormat1Data sfd = new SaveFormat1Data();
            //on met les informations
            sfd.TopFakeCC = this.TopFakeCC;
            sfd.Namespace = this.NamespaceTextBox.Text.Trim();

            //on lance la sauvegarde
            SaveFormat1.Save(FilePath, sfd);

            //on conserve le fichier actuel dans nos propriétés privés
            this.FilePath = FilePath;
            this.FileOpened = true;
            this.RefreshText();
        }



        //exporte le code qui génère la winform dans le fichier spécifié
        private void Export(string ExportFilePath)
        {
            //on crée l'objet qui contient les informations de sauvegarde
            SaveFormat1Data sfd = new SaveFormat1Data();
            //on met les informations
            sfd.TopFakeCC = this.TopFakeCC;
            sfd.Namespace = this.NamespaceTextBox.Text.Trim();

            //on lance l'exportation
            ExportCSharp1.Export(ExportFilePath, sfd);


            //ListWriter lw = new ListWriter();

            ////on écrit les namespaces qu'on utilise
            //lw.WriteLine("using System;");
            //lw.WriteLine("using System.Windows.Forms;");
            //lw.WriteLine("using System.Drawing;");
            //lw.WriteLine("");

            ////on écrit le namespace
            //lw.WriteLine("namespace " + this.NamespaceTextBox.Text.Trim());
            //lw.WriteLine("{");

            ////on lance l'écriture des déclarations. c'est cette méthode qui rajoute la déclaration de la classe (ex: "public partial class Form1 : Form {").
            //this.Export_WriteDeclarations(lw, this.TopFakeCC, 0);
            //lw.WriteLine("\t\t");

            ////on déplace la méthode InitializeComponent().
            //lw.WriteLine("\t\tprivate void InitializeComponent()");
            //lw.WriteLine("\t\t{");
            ////on lance l'écriture des assignements aux attribut/propriété de la classe
            //this.Export_WriteAssignments(lw, this.TopFakeCC, 0);

            ////on ferme la méthode InitializeComponent().
            //lw.WriteLine("\t\t}");
            ////on ferme la classe
            //lw.WriteLine("\t}");
            ////on ferme le namespace
            //lw.WriteLine("}");

            //lw.Save(ExportFilePath);

        }




        private FakeControlContainer TopFakeCC; // le control container (souvant une fenêtre (form)) que l'user est actuellement en train de fabriquer

        #endregion





        public FormDesigner()
        {
            this.Text = "Charles' WinForm Designer";
            this.Size = new Size(1000, 750);
            this.BackColor = Color.DimGray;

            //on crée le premier contrôle fake, la fenêtre
            this.TopFakeCC = FakeControlBuilder.BuildForm();
            this.TopFakeCC.CodeName = "Form1";
            this.TopFakeCC.Width = 400;
            this.TopFakeCC.Height = 400;



            ////on crée notre interface graphique
            this.FilePathTextBox = new TextBox();
            this.FilePathTextBox.Parent = this;
            this.FilePathTextBox.Location = new Point(2, 2);
            this.FilePathTextBox.Width = this.ClientSize.Width - 200;
            this.FilePathTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            this.FilePathTextBox.ReadOnly = true;
            this.FilePathTextBox.BackColor = Color.Black;
            this.FilePathTextBox.ForeColor = Color.White;

            this.OpenButton = new Button();
            this.OpenButton.Parent = this;
            this.OpenButton.Left = this.FilePathTextBox.Left;
            this.OpenButton.Top = this.FilePathTextBox.Top + this.FilePathTextBox.Height + 2;
            this.OpenButton.Width = 80;
            this.OpenButton.Height = 40;
            this.OpenButton.Text = "Open";
            this.OpenButton.Click += new EventHandler(this.OpenButton_Click);

            this.SaveButton = new Button();
            this.SaveButton.Parent = this;
            this.SaveButton.Top = this.OpenButton.Top;
            this.SaveButton.Left = this.OpenButton.Left + this.OpenButton.Width + 5;
            this.SaveButton.Size = this.OpenButton.Size;
            this.SaveButton.Text = "Save";
            this.SaveButton.Enabled = false;
            this.SaveButton.Click += new EventHandler(this.SaveButton_Click);

            this.SaveAsButton = new Button();
            this.SaveAsButton.Parent = this;
            this.SaveAsButton.Top = this.SaveButton.Top;
            this.SaveAsButton.Left = this.SaveButton.Left + this.SaveButton.Width + 5;
            this.SaveAsButton.Size = this.SaveButton.Size;
            this.SaveAsButton.Text = "Save as";
            this.SaveAsButton.Click += new EventHandler(this.SaveAsButton_Click);

            this.ExportButton = new Button();
            this.ExportButton.Parent = this;
            this.ExportButton.Top = this.SaveAsButton.Top;
            this.ExportButton.Left = this.SaveAsButton.Left + this.SaveAsButton.Width + 15;
            this.ExportButton.Size = this.SaveAsButton.Size;
            this.ExportButton.Text = "Export";
            this.ExportButton.Click += new EventHandler(this.ExportButton_Click);


            //le label et le textbox pour le namespace
            this.NamespaceLabel = new Label();
            this.NamespaceLabel.Parent = this;
            this.NamespaceLabel.AutoSize = true;
            this.NamespaceLabel.Text = "namespace : ";
            this.NamespaceLabel.Left = this.ExportButton.Left + this.ExportButton.Width + 20;
            this.NamespaceLabel.Top = this.ExportButton.Top + (this.ExportButton.Height / 2) - (this.NamespaceLabel.Height / 2);

            this.NamespaceTextBox = new TextBox();
            this.NamespaceTextBox.Parent = this;
            this.NamespaceTextBox.Left = this.NamespaceLabel.Left + this.NamespaceLabel.Width + 3;
            this.NamespaceTextBox.Top = this.NamespaceLabel.Top + (this.NamespaceLabel.Height / 2) - (this.NamespaceTextBox.Height / 2);
            this.NamespaceTextBox.Width = this.ClientSize.Width - 5 - this.NamespaceTextBox.Left;
            this.NamespaceTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;


            //la partie principale du programme

            this.FakeCCEditer = new FakeControlContainerEditer(this.TopFakeCC);
            this.FakeCCEditer.Parent = this;
            //this.FakeCCEditer.Dock = DockStyle.Bottom;
            this.FakeCCEditer.Top = this.OpenButton.Top + this.OpenButton.Height + 5; // 50
            this.FakeCCEditer.Left = 0;
            this.FakeCCEditer.Width = this.ClientSize.Width;
            this.FakeCCEditer.Height = this.ClientSize.Height - this.FakeCCEditer.Top;
            this.FakeCCEditer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;


            this.TestButton = new Button();
            this.TestButton.Parent = this;
            this.TestButton.Top = 2;
            this.TestButton.Left = this.ClientSize.Width - 2 - this.TestButton.Width;
            this.TestButton.Text = "TEST";
            this.TestButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.TestButton.Click += new EventHandler(this.TestButton_Click);


            this.RefreshText();
        }


        #region Events




        private void TestButton_Click(object sender, EventArgs e)
        {
            string typename = "System.Windows.Forms.AnchorStyles";


            Type rep = Program.GetType(typename);
            Program.wdebug(rep);


            FlowLayoutPanel flp = new FlowLayoutPanel();
            Program.wdebug(flp.BorderStyle);




        }




        private void OpenButton_Click(object sener, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            DialogResult rep = ofd.ShowDialog();
            if (rep == DialogResult.OK)
            {
                string filepath = ofd.FileName;

                //on fait loader le contenu
                this.Open(filepath);

            }
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            //on lance l'enregistrement seulement si on a déjà un fichier d'ouvert
            if (this.FileOpened)
            {
                this.Save(this.FilePath);
            }
        }
        private void SaveAsButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            DialogResult rep = sfd.ShowDialog();
            if (rep == DialogResult.OK)
            {
                string filepath = sfd.FileName;

                //on pourrait make sure que le nom du fichier se termine avec l'extension .cwfd


                //on fait enregistrer tout les objets dans le fichier
                this.Save(filepath);


            }
        }
        private void ExportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = this.FilePath + ".designer.cs";
            DialogResult rep = sfd.ShowDialog();
            if (rep == DialogResult.OK)
            {
                string filepath = sfd.FileName;

                //on pourrait make sure que le nom du fichier se termine avec l'extension .designer.cs


                //on fait exporter en c#
                this.Export(filepath);


            }
        }









        #endregion




    }
}
