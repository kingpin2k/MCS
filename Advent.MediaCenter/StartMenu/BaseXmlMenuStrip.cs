


using System.ComponentModel;
using System.Windows;
using System.Xml;

namespace Advent.MediaCenter.StartMenu
{
    public abstract class BaseXmlMenuStrip : StartMenuObject, ISupportInitialize
    {
        public static readonly DependencyProperty PriorityProperty = DependencyProperty.Register("Priority", typeof(int), typeof(XmlMenuStrip), new PropertyMetadata(new PropertyChangedCallback(BaseXmlMenuStrip.PriorityChanged)));
        private readonly XmlElement startMenuElement;

        public XmlElement StartMenuElement
        {
            get
            {
                return this.startMenuElement;
            }
        }

        public int Priority
        {
            get
            {
                return (int)this.GetValue(BaseXmlMenuStrip.PriorityProperty);
            }
            set
            {
                this.SetValue(BaseXmlMenuStrip.PriorityProperty, (object)value);
            }
        }

        protected virtual XmlElement StartMenuTargetElement
        {
            get
            {
                return this.StartMenuElement;
            }
        }

        static BaseXmlMenuStrip()
        {
        }

        internal BaseXmlMenuStrip(StartMenuManager smm, XmlElement startMenuElement)
            : base(smm)
        {
            this.startMenuElement = startMenuElement;
        }

        protected abstract void OnPriorityChanged(int oldValue, int newValue);

        private static void PriorityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((BaseXmlMenuStrip)sender).OnPriorityChanged((int)args.OldValue, (int)args.NewValue);
        }
    }
}
