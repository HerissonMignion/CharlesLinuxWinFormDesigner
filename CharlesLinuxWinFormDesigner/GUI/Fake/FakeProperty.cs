using System;
using CharlesLinuxWinFormDesigner.GUI.Fake.Controls;
namespace CharlesLinuxWinFormDesigner.GUI.Fake
{
    /// <summary>
    /// Les fake control ont des propriétés dont ce programme sert à permettre à l'user de modifier facilement.
    /// Cet classe sert à représenter une propriété
    /// </summary>
    public class FakeProperty
    {

        public string Name = "noname"; //nom de la propriété
        public Type Type = typeof(int); //type de la propriété


        private object _Value = 0;
        //valeur de la propriété représenté par this
        public object Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
                //on raise l'event
                if (this.ValueChanged != null)
                {
                    this.ValueChanged(this, new EventArgs());
                }
            }
        }

        public event EventHandler ValueChanged;

        public FakeControl Parent = null; //FakeControl "parent" auquelle la propriété this appartient.






        public FakeProperty()
        {
        }
        public FakeProperty(string Name, Type Type, object Value, FakeControl Parent)
        {
            this.Name = Name;
            this.Type = Type;
            this.Value = Value;
            this.Parent = Parent;
        }



    }
}
