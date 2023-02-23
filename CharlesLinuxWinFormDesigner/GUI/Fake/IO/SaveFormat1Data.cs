using System;
using System.Drawing;
using CharlesLinuxWinFormDesigner.GUI.Fake.Controls;
using CharlesLinuxWinFormDesigner.GUI.Fake;
namespace CharlesLinuxWinFormDesigner.GUI.Fake.IO
{

    //information de sauvegarde. essentiellement ce qui est sauvegardé et restauré dans/d'un fichier.
    public class SaveFormat1Data
    {

        public FakeControlContainer TopFakeCC = null;
        public string Namespace = "";

        public SaveFormat1Data()
        {
        }

    }
}
