using System;
using System.Collections.Generic;
namespace CharlesLinuxWinFormDesigner
{
    public class ListReader
    {

        //la liste de lignes
        public List<string> Lines = new List<string>();

        public int NextIndex = 0; //index de la prochaine ligne à lire

        public string ReadLine()
        {
            //on check si la ligne existe
            if (this.NextIndex < this.Lines.Count)
            {
                this.NextIndex++;
                return this.Lines[this.NextIndex - 1];
            }
            //sinon, on return null
            return null;
        }

        //lit la prochaine ligne sans la consomer
        public string PeakLine()
        {
            //on check si la ligne existe
            if (this.NextIndex < this.Lines.Count)
            {
                return this.Lines[this.NextIndex];
            }
            //si y'a plus de lignes, alors on return null
            return null;
        }


        public ListReader()
        {
        }
        //il va charger les lignes contenu dans un fichier
        public ListReader(string FilePath)
        {
            string[] lines = System.IO.File.ReadAllLines(FilePath);
            foreach (string line in lines)
            {
                this.Lines.Add(line);
            }
        }
    }
}
