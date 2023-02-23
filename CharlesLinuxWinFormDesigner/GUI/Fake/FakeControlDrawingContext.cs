using System;
using CharlesLinuxWinFormDesigner.GUI.Fake.Controls;
namespace CharlesLinuxWinFormDesigner.GUI.Fake
{
    public class FakeControlDrawingContext
    {
        public FakeControlContainer TopFakeCC = null; //le control container le plus haut de la hiérarchie. (souvant une form)
        public FakeControlContainer SelectedFakeCC = null; //le control container dont l'user est actuellement en train de jouer avec ses enfants.

        public FakeControl ShownFakeC = null; //contrôle actuellement sélectionné par l'user. C'est le contrôle dont l'user peut actuellement voir et modifier ses propriétés.

        public FakeControlDrawingContext()
        {
        }
    }
}
