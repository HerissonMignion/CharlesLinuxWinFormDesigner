using System;
namespace CharlesLinuxWinFormDesigner.GUI
{
    /// <summary>
    /// Cet objet représente une option disponible à l'user pour un dialogue "ListChooserDialog".
    /// </summary>
    public class ListChooserDialogEntry
    {

        public string DisplayText = "notext";
        public object Tag = null; //quand une partie du programme décide d'utiliser un ListChooserDialog, il ajoute des options au dialogue mais il peut attacher un objet avec l'option, et il va récupérer cet objet quand l'user va finir sa sélection.

        public ListChooserDialogEntry()
        {
        }
        public ListChooserDialogEntry(string DisplayText)
        {
            this.DisplayText = DisplayText;
        }
        public ListChooserDialogEntry(string DisplayText, object Tag)
        {
            this.DisplayText = DisplayText;
            this.Tag = Tag;
        }

    }
}
