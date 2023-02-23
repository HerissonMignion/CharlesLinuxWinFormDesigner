using System;
using System.Collections.Generic;
//using System.Windows.Forms;
using System.Drawing;
namespace CharlesLinuxWinFormDesigner
{
    public class ListWriter
    {

        //la liste de lignes
        public List<string> Lines = new List<string>();

        //écrit une ligne
        public void WriteLine(string line)
        {
            this.Lines.Add(line);
        }

        //sauvegarde dans un fichier
        public void Save(string filepath)
        {
            System.IO.File.WriteAllLines(filepath, this.Lines, System.Text.Encoding.UTF8);
        }




        public ListWriter()
        {
        }
    }
}
