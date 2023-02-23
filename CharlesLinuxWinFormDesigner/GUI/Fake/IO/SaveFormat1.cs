using System;
using System.Drawing;
using CharlesLinuxWinFormDesigner.GUI.Fake.Controls;
using CharlesLinuxWinFormDesigner.GUI.Fake;
using System.Collections.Generic;
using System.Windows.Forms;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.IO
{

    //format de sauvegarde #1 du programme.
    public static class SaveFormat1
    {

        //mettre dans un objet SaveFormat1Data les informations à sauvegarder dans un fichier.
        public static void Save(string FilePath, SaveFormat1Data sfd)
        {
            ListWriter lw = new ListWriter();
            //on commence par écrire le numéro de version du format de sauvegarde
            lw.WriteLine("1");

            //on écrit le namespace
            lw.WriteLine("namespace");
            //lw.WriteLine(this.NamespaceTextBox.Text.Trim());
            lw.WriteLine(sfd.Namespace);

            //on fait sauvegarder notre TopFakeCC
            SaveFormat1.Save_Recursive(lw, sfd.TopFakeCC, 0);

            //fin
            lw.WriteLine("eof");
            //on enregistre
            lw.Save(FilePath);
            ////on conserve le fichier actuel dans nos propriétés privés
            //this.FilePath = FilePath;
            //this.FileOpened = true;
            //this.RefreshText();
        }

        //écrit dans la sauvegarde un fake control, et si c'est un control container, il va aussi écrire ses enfants
        private static void Save_Recursive(ListWriter lw, FakeControl fc, int identation = 0)
        {
            //on commence par l'indentation
            string indent = Program.StringMultiply("    ", identation);
            string indent2 = indent + "    ";
            string indent3 = indent2 + "    ";

            //on met d'abors si c'est un "fc" ou un "fcc".
            if (fc is FakeControlContainer)
            {
                FakeControlContainer fcc = (FakeControlContainer)fc;
                lw.WriteLine(indent + "fcc");

                //on écrit des propriétés qui sont interne au fonctionnement du programme.
                //on écrit ChildrenAreaTopLeft
                lw.WriteLine(indent2 + "ChildrenAreaTopLeft");
                lw.WriteLine(indent3 + fcc.ChildrenAreaTopLeft.X.ToString());
                lw.WriteLine(indent3 + fcc.ChildrenAreaTopLeft.Y.ToString());

            }
            else
            {
                lw.WriteLine(indent + "fc");
            }

            //on passe à travert toutes les propriétés
            foreach (FakeProperty fp in fc.ListProperties)
            {
                //on écrit la signature d'une propriété
                lw.WriteLine(indent2 + "property");
                //on écrit le nom de la property
                lw.WriteLine(indent3 + fp.Name);
                //maintenant c'est la section de la «valeur» de la propriété.
                //on écrit le type et la valeur de la propriété
                if (fp.Type == typeof(int))
                {
                    //on écrit le type d'un int
                    lw.WriteLine(indent3 + "int");
                    //on écrit la valeur du int
                    lw.WriteLine(indent3 + ((int)(fp.Value)).ToString());
                }
                else if (fp.Type == typeof(decimal))
                {
                    //on écrit le type d'un decimal
                    lw.WriteLine(indent3 + "decimal");
                    lw.WriteLine(indent3 + ((decimal)(fp.Value)).ToString());
                }
                else if (fp.Type == typeof(string))
                {
                    //on écrit le type d'un string
                    lw.WriteLine(indent3 + "string");
                    //on écrit le nombre de ligne
                    lw.WriteLine(indent3 + "1"); //pour l'instant, on ne supporte pas plus d'une ligne.
                    //on écrit la valeur du string
                    lw.WriteLine((string)(fp.Value));
                }
                else if (fp.Type == typeof(bool))
                {
                    //on écrit le type d'un bool
                    lw.WriteLine(indent3 + "bool");
                    //on écrit la valeur du bool
                    lw.WriteLine(indent3 + fp.Value.ToString().ToLower());
                }
                else if (fp.Type == typeof(Color))
                {
                    //on écrit le type d'une couleur
                    lw.WriteLine(indent3 + "Color");
                    //on écrit les valeurs des composants, dans le "bon" ordre
                    Color c = (Color)(fp.Value);
                    lw.WriteLine(indent3 + c.A.ToString());
                    lw.WriteLine(indent3 + c.R.ToString());
                    lw.WriteLine(indent3 + c.G.ToString());
                    lw.WriteLine(indent3 + c.B.ToString());
                }
                else if (fp.Type == typeof(Font))
                {
                    //on écrit le type d'une font
                    lw.WriteLine(indent3 + "Font");
                    //on écrit les valeurs de la font.
                    Font f = (Font)(fp.Value);
                    lw.WriteLine(indent3 + f.FontFamily.Name);
                    lw.WriteLine(indent3 + Convert.ToInt32(f.Size * 10000).ToString());
                    //on écrit les flags, dans le "bon" ordre
                    lw.WriteLine(indent3 + f.Style.HasFlag(FontStyle.Bold).ToString().ToLower());
                    lw.WriteLine(indent3 + f.Style.HasFlag(FontStyle.Italic).ToString().ToLower());
                    lw.WriteLine(indent3 + f.Style.HasFlag(FontStyle.Underline).ToString().ToLower());
                    lw.WriteLine(indent3 + f.Style.HasFlag(FontStyle.Strikeout).ToString().ToLower());
                }
                else if (fp.Type == typeof(AnchorStyles))
                {
                    //on écrit le type d'un AnchorStyles
                    lw.WriteLine(indent3 + "AnchorStyles");
                    //on écrit les valeurs dans le "bon" ordre
                    AnchorStyles a = (AnchorStyles)(fp.Value);
                    lw.WriteLine(indent3 + a.HasFlag(AnchorStyles.Left).ToString().ToLower());
                    lw.WriteLine(indent3 + a.HasFlag(AnchorStyles.Top).ToString().ToLower());
                    lw.WriteLine(indent3 + a.HasFlag(AnchorStyles.Right).ToString().ToLower());
                    lw.WriteLine(indent3 + a.HasFlag(AnchorStyles.Bottom).ToString().ToLower());
                }

                //maintenant nous sommes rendû à des trucs plus généraux.

                else if (fp.Type.IsEnum)
                {
                    //on écrit le type d'une valeur d'enum
                    lw.WriteLine(indent3 + "EnumValue");
                    //on écrit le type de l'énum
                    lw.WriteLine(indent3 + fp.Type.ToString());
                    //on écrit la valeur
                    lw.WriteLine(indent3 + fp.Type.GetEnumName(fp.Value));
                }
                //la propriété est d'un type non supporté par l'enregistrement.
                else
                {
                    //on met le type object et la valeur null.
                    lw.WriteLine(indent3 + "object"); //le type
                    lw.WriteLine(indent3 + "null"); //la valeur null.
                }

            }


            //si c'est un control container, alors on doit mettre tout ses enfants
            if (fc is FakeControlContainer)
            {
                //on écrit la signature de la liste des children
                lw.WriteLine(indent2 + "Children");

                //on fait écrire les children
                foreach (FakeControl child in ((FakeControlContainer)fc).Children)
                {
                    SaveFormat1.Save_Recursive(lw, child, identation + 2);
                }

                //on écrit la signature de la fin de la liste
                lw.WriteLine(indent2 + "end");
            }


            //on met la fin du block du FakeControl(Container)?
            lw.WriteLine(indent + "end");
        }



        public static SaveFormat1Data Open(string FilePath)
        {
            ListReader lr = new ListReader(FilePath);

            //FakeControlContainer newTopFakeCC = null;
            SaveFormat1Data sfd = new SaveFormat1Data();

            //on lit la première ligne, qui est le numéro de version du format de sauvegarde
            string version = lr.ReadLine();

            //on lit le fichier selon le bon format de version
            if (version == "1")
            {
                //on lit tout les objets qui trainent à la racine
                while (true)
                {
                    //on peak la prochaine ligne
                    string peak = lr.PeakLine().Trim();

                    if (peak == "eof")
                    {
                        break;
                    }
                    //on check si c'est le namespace
                    else if (peak == "namespace")
                    {
                        lr.ReadLine();
                        //on garde le namespace
                        //this.NamespaceTextBox.Text = lr.ReadLine().Trim();
                        sfd.Namespace = lr.ReadLine().Trim();
                    }
                    //si on est tombé sur notre top fake cc
                    else if (peak == "fc" || peak == "fcc")
                    {
                        //on fait lire le top level control
                        sfd.TopFakeCC = (FakeControlContainer)(SaveFormat1.Open_ReadFakeControl(lr));
                    }
                    //on ne supporte pas cette signature
                    else
                    {
                        //si on ne supporte pas cet objet dans la structure, alors on doit consommer la ligne pour passer à la prochaine ligne.
                        //si on ne consomme pas la ligne, alors on va rester dans une boucle infini
                        lr.ReadLine();
                    }
                }
            }
            else
            {
                MessageBox.Show("Ce fichier est d'un format de version non prise en charge");
                return null;
            }
            ////on concerve dans nos propriétés privés les informations importante
            //this.FileOpened = true;
            //this.FilePath = FilePath;
            //this.TopFakeCC = newTopFakeCC;
            ////on fait des refresh
            //this.RefreshText();
            //this.FakeCCEditer.SetTopFakeCC(this.TopFakeCC);
            return sfd;
        }

        //lit un FakeControl(Container)? enregistré dans un fichier.
        //la première lit à être lut doit être le début de la signature, "fc" ou "fcc". sinon, un null sera retourné
        private static FakeControl Open_ReadFakeControl(ListReader lr)
        {
            FakeControl fc = null;
            string signature = lr.ReadLine().Trim();
            if (signature == "fc" || signature == "fcc")
            {
                //on crée le fake control
                if (signature == "fc")
                {
                    fc = new FakeControl();
                }
                else
                {
                    fc = new FakeControlContainer();
                }

                //on lit tout les objets qui trainent dans le block actuel
                while (true)
                {
                    string peak = lr.PeakLine().Trim();

                    //on check c'eat quoi la signature de l'objet actuel
                    if (peak == "end")
                    {
                        //on consomme la ligne du end
                        lr.ReadLine();
                        break;
                    }
                    //la signature de ChildrenAreaTopLeft
                    else if (peak == "ChildrenAreaTopLeft")
                    {
                        //on consome la ligne de la signature
                        lr.ReadLine();
                        //on lit les coordonnées
                        int x = int.Parse(lr.ReadLine().Trim());
                        int y = int.Parse(lr.ReadLine().Trim());
                        ((FakeControlContainer)fc).ChildrenAreaTopLeft = new Point(x, y);
                    }
                    //la signature d'une propriété
                    else if (peak == "property")
                    {
                        //on fait lire la propriété, sa valeur, et fait ajouter ça au fake control
                        SaveFormat1.Open_ReadProperty(lr, fc);
                    }
                    //si on est arrivé sur la liste des children
                    else if (peak == "Children")
                    {
                        //on lit la ligne children
                        lr.ReadLine();

                        //safety check
                        if (fc is FakeControlContainer)
                        {
                            FakeControlContainer fcc = (FakeControlContainer)fc;

                            //on lit tout les objects qui trainent dans notre block
                            while (true)
                            {
                                //on peak la prochaine signature
                                string peak2 = lr.PeakLine().Trim();

                                if (peak2 == "end")
                                {
                                    break;
                                }
                                //s'il s'agit d'un autre élément de la liste
                                else if (peak2 == "fc" || peak2 == "fcc")
                                {
                                    //fcc.AddChild(this.Open_ReadFakeControl(lr));
                                    SaveFormat1.Open_ReadFakeControl(lr).Parent = fcc;
                                }
                            }

                        }

                        //on lit la ligne du end
                        lr.ReadLine();
                    }
                    //on ne supporte pas cette signature
                    else
                    {
                        Program.wdebug("Signature non supporté à la ligne à l'index : " + (lr.NextIndex));
                        //si on ne supporte pas cet objet dans la structure, alors on doit consommer la ligne pour passer à la prochaine ligne
                        lr.ReadLine();
                    }
                }

            }

            //en continuant de programmer de nouvelles fonctionnalités, des propriétés s'ajoutent aux contrôles.
            //cependant, cela ne signifit pas qu'il faut un nouveau format de sauvegarde, ou une nouvelle version.
            //quand on ajoute une propriété à un contrôle, cette propriété manque à tout les fichiers déjà sauvegardés.
            //ici on ajoute des propriétés aux contrôles, au cas où le contrôle contenu dans le fichier ne possède pas les nouvelles propriétés.
            //les nouvelles propriétés dépendent évidement du type du contrôle, pas tout les contrôles sont multiline par exemple.

            if (fc.ClassName == "Button")
            {
                fc.MakeSureHasProperty("TextAlign", ContentAlignment.MiddleCenter);
            }
            else if (fc.ClassName == "Label")
            {
                fc.MakeSureHasProperty("TextAlign", ContentAlignment.TopLeft);
            }
            else if (fc.ClassName == "CheckBox")
            {
                fc.MakeSureHasProperty("Checked", false);
            }
            else if (fc.ClassName == "TextBox")
            {
                fc.MakeSureHasProperty("Multiline", false);
            }

            //return fc;
            return SaveFormat1.GenerateFakeControlFromGeneric(fc);
        }

        //à cause de la spécification du format de sauvegarde, la méthode Open_ReadFakeControl, pendant qu'elle lit le contrôle, génère seulement un FakeControl ou un FakeControlContainer qui est "générique".
        //maintenant, pour le bon fonctionnement du programme, il faut vraiment créer l'objet exacte qui correspond au contrôle. cette information est enregistré
        //dans la propriété ClassName, qui est accessible seulement pendant la lecture du contrôle.
        //cette méthode prend en paramètre un FakeControl ou un FakeControlContainer générique, et retourne une instance du contrôle correspondant dans GUI.Fake.Controls, selon la valeur indiqué par la propriété ClassName.
        private static FakeControl GenerateFakeControlFromGeneric(FakeControl fc)
        {
            return FakeControl.GenerateFakeControlFromGeneric(fc);
        }



        //lit un objet de property. il faut lui donner le fake control à qui ajouter la propriété ensuite.
        //la première ligne qu'il doit lire du ListReader doit être "property", sinon il fait rien.
        private static void Open_ReadProperty(ListReader lr, FakeControl fc)
        {
            string signature = lr.ReadLine().Trim();
            if (signature == "property")
            {
                //on lit le nom de la propriété
                string name = lr.ReadLine().Trim();
                //on lit le type de la propriété
                string type = lr.ReadLine().Trim();
                //on lit la valeur de la propriété, mais la lecture de la valeur dépend du type
                object value = null; //ici on prépare une valeur par défaut
                //si c'est un int
                if (type == "int")
                {
                    value = Convert.ToInt32(lr.ReadLine().Trim());
                }
                //si c'est un decimal
                else if (type == "decimal")
                {
                    try
                    {
                        value = Convert.ToDecimal(lr.ReadLine().Trim());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur pendant la lecture d'un type decimal dans le fichier.");
                        Console.WriteLine(ex.StackTrace);
                        value = 0m;
                    }
                }
                //si c'est une string
                else if (type == "string")
                {
                    //on lit la quantité de lignes de la chaine de texte
                    int count = Convert.ToInt32(lr.ReadLine().Trim());
                    //on lit la chaine de texte
                    string text = lr.ReadLine(); //pour l'instant on ne supporte pas plusieurs lignes
                    //on assigne la chaine de texte
                    value = text;
                }
                //si c'est un bool
                else if (type == "bool")
                {
                    value = lr.ReadLine().Trim() == "true";
                }
                //si c'est un objet spécial
                else if (type == "object")
                {
                    //on check sur la première ligne qui suit c'est quel objet.
                    string ObjectSignature = lr.ReadLine().Trim();
                    //on génère l'objet spécifié
                    if (ObjectSignature == "null")
                    {
                        value = null;
                    }
                    //la signature d'objet indiqué n'est pas supporté, alors on met un null quand même
                    else
                    {
                        value = null;
                    }
                }
                //si c'est une couleur
                else if (type == "Color")
                {
                    //on lit les composantes de la couleur
                    int a = Convert.ToInt32(lr.ReadLine().Trim());
                    int r = Convert.ToInt32(lr.ReadLine().Trim());
                    int g = Convert.ToInt32(lr.ReadLine().Trim());
                    int b = Convert.ToInt32(lr.ReadLine().Trim());
                    //return une valeur comprise de 0 à 255.
                    int SafeColorComponent(int component)
                    {
                        if (component < 0) { return 0; }
                        if (component > 255) { return 255; }
                        return component;
                    }
                    a = SafeColorComponent(a);
                    r = SafeColorComponent(r);
                    g = SafeColorComponent(g);
                    b = SafeColorComponent(b);
                    //on crée la couleur
                    value = Color.FromArgb(a, r, g, b);
                }
                //si c'est une font
                else if (type == "Font")
                {
                    //on lit la famille de la font
                    string FamilyName = lr.ReadLine().Trim();
                    //on lit la taille de la font
                    float FontSize = Convert.ToSingle(lr.ReadLine().Trim()) / 10000f;
                    FontStyle Style = FontStyle.Regular;
                    //on ajoute les flags selon les valeurs true/false indiqué dans le fichier
                    if (lr.ReadLine().Trim() == "true") { Style |= FontStyle.Bold; }
                    if (lr.ReadLine().Trim() == "true") { Style |= FontStyle.Italic; }
                    if (lr.ReadLine().Trim() == "true") { Style |= FontStyle.Underline; }
                    if (lr.ReadLine().Trim() == "true") { Style |= FontStyle.Strikeout; }
                    //on crée la font
                    value = new Font(FamilyName, FontSize, Style);
                }
                //si c'est un AnchorStyles
                else if (type == "AnchorStyles")
                {
                    //on lit les flags selon les valeurs true/false
                    AnchorStyles a = AnchorStyles.None;
                    if (lr.ReadLine().Trim() == "true") { a |= AnchorStyles.Left; }
                    if (lr.ReadLine().Trim() == "true") { a |= AnchorStyles.Top; }
                    if (lr.ReadLine().Trim() == "true") { a |= AnchorStyles.Right; }
                    if (lr.ReadLine().Trim() == "true") { a |= AnchorStyles.Bottom; }
                    //on donne la valeur
                    value = a;
                }
                //si c'est une valeur d'une énumération
                else if (type == "EnumValue")
                {
                    //on lit le type de l'énum
                    string EnumTypeString = lr.ReadLine().Trim();
                    Type EnumType = Program.GetType(EnumTypeString);
                    //on lit le nom de la valeur dans l'énum
                    string ValueString = lr.ReadLine().Trim();

                    //on passe à travers toute les valeurs possible de cette énum pour trouver la valeur dont le nom correspond à ce qui était indiqué
                    object NewValue = null;
                    foreach (var PossibleValue in EnumType.GetEnumValues())
                    {
                        //on check si la valeur actuelle a le nom de la valeur indiqué dans le fichier
                        if (EnumType.GetEnumName(PossibleValue) == ValueString)
                        {
                            //on conserve cette valeur et on arrête de chercher
                            NewValue = PossibleValue;
                            break;
                        }
                    }

                    //on donne la valeur
                    value = NewValue;

                }

                //si la valeur n'est pas null, on l'ajoute au fake control (container)?
                if (value != null)
                {
                    fc.SetProperty(name, value);
                }
            }
        }




    }
}
