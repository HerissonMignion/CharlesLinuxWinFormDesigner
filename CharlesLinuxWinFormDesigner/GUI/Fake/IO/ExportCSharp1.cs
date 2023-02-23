using System;
using System.Drawing;
using CharlesLinuxWinFormDesigner.GUI.Fake.Controls;
using CharlesLinuxWinFormDesigner.GUI.Fake;
using System.Collections.Generic;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.IO
{
    public static class ExportCSharp1
    {

        //exporte le code qui génère la winform dans le fichier spécifié.
        //mettre dans un objet SaveFormat1Data les informations à sauvegarder dans un fichier.
        public static void Export(string ExportFilePath, SaveFormat1Data sfd)
        {
            ListWriter lw = new ListWriter();

            //on écrit les namespaces qu'on utilise
            lw.WriteLine("using System;");
            lw.WriteLine("using System.Windows.Forms;");
            lw.WriteLine("using System.Drawing;");
            lw.WriteLine("");

            //on écrit le namespace
            lw.WriteLine("namespace " + sfd.Namespace);
            lw.WriteLine("{");

            //on lance l'écriture des déclarations. c'est cette méthode qui rajoute la déclaration de la classe (ex: "public partial class Form1 : Form {").
            ExportCSharp1.Export_WriteDeclarations(sfd, lw, sfd.TopFakeCC, 0);
            lw.WriteLine("\t\t");

            //on déplace la méthode InitializeComponent().
            lw.WriteLine("\t\tprivate void InitializeComponent()");
            lw.WriteLine("\t\t{");
            //on lance l'écriture des assignements aux attribut/propriété de la classe
            ExportCSharp1.Export_WriteAssignments(sfd, lw, sfd.TopFakeCC, 0);

            //on ferme la méthode InitializeComponent().
            lw.WriteLine("\t\t}");
            //on ferme la classe
            lw.WriteLine("\t}");
            //on ferme le namespace
            lw.WriteLine("}");

            lw.Save(ExportFilePath);

        }

        //écrit une déclaration. la depth indique notre profondeur dans les contrôles. le seul contrôle à avoir une depth de 0 est this.TopFakeCC.
        //si depth = 0 (this.TopFakeCC) alors c'est la déclaration de la classe. autrement, c'est un attribut/propriété de la classe.
        //cette méthode est récursive
        private static void Export_WriteDeclarations(SaveFormat1Data sfd, ListWriter lw, FakeControl fc, int depth)
        {
            //on check si on doit déclarer la classe (donc si c'est this.TopFakeCC).
            if (depth == 0)
            {
                //on ajoute la déclaration de la classe
                lw.WriteLine("\t" + (string)(fc.GetProperty("Modifier")) + " partial class " + fc.CodeName + " : " + fc.ClassName);
                lw.WriteLine("\t{");

            }
            //ce n'est pas le top level control
            else
            {
                //on ajoute la déclaration du contrôle dans la classe
                lw.WriteLine("\t\t" + (string)(fc.GetProperty("Modifier")) + " " + fc.ClassName + " " + fc.CodeName + ";");

            }

            //si c'est un control container, alors on doit faire ajouter les déclaration de ses enfants
            if (fc is FakeControlContainer)
            {
                FakeControlContainer fcc = (FakeControlContainer)fc;

                //on fait rajouter la déclaration de chaques enfants
                foreach (FakeControl child in fcc.Children)
                {
                    ExportCSharp1.Export_WriteDeclarations(sfd, lw, child, depth + 1);
                }

            }
        }

        //écrit les "this.moncontrol = new Control();" et le reste dans la méthode InitializeComponent().
        //cette méthode est récursive.
        private static void Export_WriteAssignments(SaveFormat1Data sfd, ListWriter lw, FakeControl fc, int depth)
        {

            //on ajoute notre déclaration
            lw.WriteLine("\t\t\t//");
            lw.WriteLine("\t\t\t// " + fc.CodeName);
            lw.WriteLine("\t\t\t//");

            //on prépare « la chaine de texte » qui nous permet d'accéder à l'attribut/propriété depuis l'intérieur de la méthode InitializeComponent()
            string ObjectPath = "this";
            //si nous ne sommes pas this.TopFakeCC
            if (depth > 0)
            {
                ObjectPath += "." + fc.CodeName;

                //on doit créer une instance du contrôle si nous ne somme pas this.TopFakeCC.
                lw.WriteLine("\t\t\t" + ObjectPath + " = new " + fc.ClassName + "();");

                //il faut aussi lui assigner son parent.
                //nous avons du code générale pour assigner le parent pour la plupart des contrôles. cependant certains contrôles n'ont pas la propriété Parent.
                //pour certain contrôles, nous devons les ajouter à leur parent d'une façon différente.
                bool WriteGenericParentAssignment = true; //devient false s'il en faut pas utiliser le code générale d'assignation du parent

                //avant de vérifier le parent, on make sure qu'il y a un parent
                if (fc.Parent != null) //cette vérification est redondante
                {
                    //on check si c'est un parent particulier dont on ne peut pas ajouter un enfant avec la propriété .Parent du child
                    if (fc.Parent.ClassName == "MenuStrip")
                    {
                        //nous ne devons pas utiliser le code générique plus bas pour définir (en c#) la parent d'un enfant d'un MenuStrip
                        WriteGenericParentAssignment = false;

                        //nous inscrivons nous-même le code l'ajoutera à son parent
                        lw.WriteLine("\t\t\tthis." + fc.Parent.CodeName + ".Items.Add(" + ObjectPath + ");");

                    }
                    else if (fc.Parent.ClassName == "StatusStrip")
                    {
                        //nous ne devons pas utiliser le code générique plus bas pour définir (en c#) le parent d'un enfant d'un StatusStrip
                        WriteGenericParentAssignment = false;

                        //nous inscrivons nous-même le code l'ajoutera à son parent
                        lw.WriteLine("\t\t\tthis." + fc.Parent.CodeName + ".Items.Add(" + ObjectPath + ");");

                    }
                    else if (fc.Parent.ClassName == "ToolStripMenuItem")
                    {
                        //nous ne devons pas utiliser le code générique plus bas pour définir (en c#) la parent d'un ToolStripMenuItem
                        WriteGenericParentAssignment = false;

                        //nous inscrivons nous-même le code l'ajoutera à son parent
                        lw.WriteLine("\t\t\tthis." + fc.Parent.CodeName + ".DropDownItems.Add(" + ObjectPath + ");");

                    }

                }

                //on écrit le code générale d'assignation de parent seulement si le code/la section précédante n'a pas écrit elle-même ce code à cause que, par exemple, la propriété Parent n'existe pas, ou d'autre situations spéciales.
                if (WriteGenericParentAssignment)
                {
                    string ParentString = "this";
                    if (fc.Parent != sfd.TopFakeCC)
                    {
                        ParentString += "." + fc.Parent.CodeName;
                    }
                    lw.WriteLine("\t\t\t" + ObjectPath + ".Parent = " + ParentString + ";");
                }
            }

            //on écrit toute les propriétés
            foreach (FakeProperty fp in fc.ListProperties)
            {
                //on va d'abord déterminer si c'est une propriété qu'il faut écrire dans le fichier

                //indique s'il faut utiliser le code générique pour écrire la propriété dans le fichier.
                bool WriteProperty = true; //devient false s'il ne faut pas écrire la propriété dans le fichier (avec le code générique plus tard)

                //on s'assure que ce n'est pas une fausse propriété ajouté aux fake controls pour l'user du programme
                if (fp.Name == "Modifier" || fp.Name == "ClassName" || fp.Name == "CodeName")
                {
                    WriteProperty = false;
                }

                //certains contrôles n'ont pas les propriété Top et Left ou d'autres propriétés communes à la plupart des contrôles.
                //on check si notre contrôle actuel ne supporte (en réalité) pas la propriété qu'on est sur le point d'écrire
                //on check d'abord c'est quoi le type du contrôle
                if (fc.ClassName == "MenuStrip")
                {

                }
                else if (fc.ClassName == "ToolStripMenuItem" || fc.ClassName == "ToolStripStatusLabel")
                {
                    //on check si c'est une propriété inexistante pour ce contrôle
                    if (fp.Name == "Top" || fp.Name == "Left")
                    {
                        //il ne faut pas écrire la propriété
                        WriteProperty = false;
                    }
                }

                //on check si nous sommes actuellement en train d'écrire le this.TopFakeCC
                if (depth <= 0)
                {
                    //on ne met pas automatiquement à true la propriété visible pour le this.TopFakeCC.
                    //le programme sert souvant à désigner des Form. mono a un bug avec la méthode ShowDialog si on met à true la propriété visible
                    if (fp.Name == "Visible")
                    {
                        WriteProperty = false;
                    }

                    //des vérifications qui s'appliquent seulement si le TopFakeCC est une Form
                    if (fc.ClassName == "Form")
                    {
                        //si le this.TopFakeCC est une Form, alors en mettant le Width et le Height, il faut prendre en considération l'épaisseur de la bordure de la fenêtre.
                        //dans ce cas, nous écrivons ici et maintenant la valeur de la propriété Width ou Height.
                        if (fp.Name == "Width" || fp.Name == "Height")
                        {
                            //on écrit nous même ici c'est quoi la valeur de la propriété Width ou Height.
                            WriteProperty = false;

                            //on prépare le début de la ligne de code à écrire
                            string line = "\t\t\t" + ObjectPath + "." + fp.Name + " = " + fp.Value.ToString(); //il n'y a pas le ; parce qu'on va ajouter quelque chose à la fin
                            //on utilise des constantes pour la taille des bordures des fenêtres
                            if (fp.Name == "Width")
                            {
                                line += " + 2 * SystemInformation.FrameBorderSize.Width";
                            }
                            else if (fp.Name == "Height")
                            {
                                line += " + SystemInformation.CaptionHeight + SystemInformation.FrameBorderSize.Height";
                            }

                            //on écrit la ligne de code dans le fichier
                            lw.WriteLine(line + ";"); //on met le ; ici
                        }
                    }

                }

                //on écrit dans le fichier la propriété seulement si on a déterminé précédement que c'était une propriété qu'il fallait écrire dans le fichier (avec ce code générique).
                if (WriteProperty)
                {
                    //les littéraux sont différent selon le type de la propriété
                    if (fp.Type == typeof(int))
                    {
                        lw.WriteLine("\t\t\t" + ObjectPath + "." + fp.Name + " = " + fp.Value.ToString() + ";");
                    }
                    else if (fp.Type == typeof(decimal))
                    {
                        lw.WriteLine("\t\t\t" + ObjectPath + "." + fp.Name + " = " + fp.Value.ToString().Replace(',', '.') + "m;");
                    }
                    else if (fp.Type == typeof(string))
                    {
                        lw.WriteLine("\t\t\t" + ObjectPath + "." + fp.Name + " = \"" + fp.Value.ToString() + "\";");
                    }
                    else if (fp.Type == typeof(bool))
                    {
                        lw.WriteLine("\t\t\t" + ObjectPath + "." + fp.Name + " = " + ((bool)(fp.Value)).ToString().ToLower() + ";");
                    }
                    else if (fp.Type == typeof(Color))
                    {
                        Color c = (Color)(fp.Value);
                        //on l'écrit sous la forme " ... = Color.FromArgb(asdf, asdf, asdf, asdf);"
                        lw.WriteLine("\t\t\t" + ObjectPath + "." + fp.Name + " = Color.FromArgb(" + c.A.ToString() + ", " + c.R.ToString() + ", " + c.G.ToString() + ", " + c.B.ToString() + ");");
                    }
                    else if (fp.Type == typeof(Font))
                    {
                        Font f = (Font)(fp.Value);
                        //TODO : supporter les font style
                        lw.WriteLine("\t\t\t" + ObjectPath + "." + fp.Name + " = new Font(\"" + f.FontFamily.Name + "\", " + f.Size.ToString().Replace(',', '.') + "f);");
                    }
                    else if (fp.Type == typeof(AnchorStyles))
                    {
                        AnchorStyles a = (AnchorStyles)(fp.Value);
                        AnchorStyles[] Styles = new AnchorStyles[] { AnchorStyles.Left, AnchorStyles.Top, AnchorStyles.Right, AnchorStyles.Bottom };
                        string finalLiteral = ""; //on accumule les styles dans cette chaine de texte
                        bool first = true; //si on ajoute un anchor style, indique si c'est le premier qu'on ajoute.
                        foreach (AnchorStyles style in Styles)
                        {
                            //on check si le contrôle possède ce style
                            if (a.HasFlag(style))
                            {
                                //on rajoute le | entre les différents littérales
                                if (!first)
                                {
                                    finalLiteral += " | ";
                                }
                                //on rajoute le style actuel.
                                finalLiteral += "AnchorStyles." + style.ToString();

                                //désormais, il y a au moins 1 style
                                first = false;
                            }
                        }
                        //si le contrôle ne possède aucun anchor style, alors on met none
                        if (first)
                        {
                            finalLiteral = "AnchorStyles.None";
                        }
                        //on écrit la ligne
                        lw.WriteLine("\t\t\t" + ObjectPath + "." + fp.Name + " = " + finalLiteral + ";");
                    }
                    //maintenant on fait des propriété d'un type plus générale
                    else if (fp.Type.IsEnum)
                    {
                        lw.WriteLine("\t\t\t" + ObjectPath + "." + fp.Name + " = " + fp.Type.ToString() + "." + fp.Type.GetEnumName(fp.Value) + ";");
                    }
                }
            }

            ////si nous sommes le top level control et que nous sommes une form, nous ajuste notre hauteur verticale pour correspondre à ce que l'user voyait dans l'interface graphique du programme
            //if (depth == 0 && fc.ClassName == "Form")
            //{
            //    //on ajoute un calcul mathématique qui ajuste la hauteur de la form, en lui ajoutant le height de sa title bar
            //    lw.WriteLine("\t\t\t" + ObjectPath + ".Height += this.RectangleToScreen(this.ClientRectangle).Top - this.Top;");
            //    //on ajoute un calcul mathématique pour ajuster la largeur de la form
            //    lw.WriteLine("\t\t\t" + ObjectPath + ".Width += (this.RectangleToScreen(this.ClientRectangle).Left - this.Left) * 2;");
            //}


            //on check si nous avons un control container. si c'est le cas, alors on doit relancer pour ses enfants récursivement
            if (fc is FakeControlContainer)
            {
                //on passe par chacun de ses enfants
                foreach (FakeControl child in ((FakeControlContainer)fc).Children)
                {
                    ExportCSharp1.Export_WriteAssignments(sfd, lw, child, depth + 1);
                }
            }
        }


    }
}
