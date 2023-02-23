using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;

using CharlesLinuxWinFormDesigner.GUI;

namespace CharlesLinuxWinFormDesigner
{
    public class Program
    {
        public static void wdebug(object text)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }

        public static string StringMultiply(string str, int amount)
        {
            string rep = "";
            for (int i = 0; i < amount; i++)
            {
                rep += str;
            }
            return rep;
        }


        private static Graphics _g = Graphics.FromImage(new Bitmap(10, 10));
        public static SizeF MeasureString(string text, Font font)
        {
            return Program._g.MeasureString(text, font);
        }




        //recherche dans TOUT les assembly un type via son nom complet (ex: System.Drawing.Color).
        //return null si on ne réussi pas à avoir le type via son nom complet.
        public static Type GetType(string FullName)
        {
            //on crée d'abord une liste de tout les assembly à vérifier
            List<Assembly> assemblies = new List<Assembly>();
            assemblies.Add(System.Reflection.Assembly.GetExecutingAssembly());
            //on ajoute tout les autres assembly
            foreach (AssemblyName an in System.Reflection.Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                Assembly a = System.Reflection.Assembly.Load(an);
                assemblies.Add(a);
            }

            //maintenant on check dans tout les assembly le type
            foreach (Assembly a in assemblies)
            {
                Type t = a.GetType(FullName);
                //on check si ça a réussi
                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }



        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            //Program.wdebug(SystemInformation.CaptionHeight);
            //Program.wdebug(SystemInformation.FrameBorderSize);
            Application.Run(new FormDesigner());
        }

    }
}
