


using System.Windows;

namespace Advent.MediaCenter.StartMenu.OEM
{
    public class ApplicationRefObject : MediaCenterRegistryObject
    {
        public static readonly DependencyProperty ApplicationProperty = DependencyProperty.Register("Application", typeof(Advent.MediaCenter.StartMenu.OEM.Application), typeof(ApplicationRefObject), new PropertyMetadata(new PropertyChangedCallback(ApplicationRefObject.ApplicationChanged)));
        public static readonly DependencyProperty ApplicationIDProperty = DependencyProperty.Register("ApplicationID", typeof(string), typeof(ApplicationRefObject), new PropertyMetadata(new PropertyChangedCallback(ApplicationRefObject.ApplicationIDChanged)));

        public virtual Advent.MediaCenter.StartMenu.OEM.Application Application
        {
            get
            {
                return (Advent.MediaCenter.StartMenu.OEM.Application)this.GetValue(ApplicationRefObject.ApplicationProperty);
            }
            set
            {
                this.SetValue(ApplicationRefObject.ApplicationProperty, (object)value);
            }
        }

        public virtual string ApplicationID
        {
            get
            {
                return (string)this.GetValue(ApplicationRefObject.ApplicationIDProperty);
            }
            set
            {
                this.SetValue(ApplicationRefObject.ApplicationIDProperty, (object)value);
            }
        }

        static ApplicationRefObject()
        {
        }

        protected virtual void OnApplicationChanged(Advent.MediaCenter.StartMenu.OEM.Application oldApp, Advent.MediaCenter.StartMenu.OEM.Application newApp)
        {
        }

        private static void ApplicationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
                sender.SetValue(ApplicationRefObject.ApplicationIDProperty, (object)((Advent.MediaCenter.StartMenu.OEM.Application)args.NewValue).ID);
            ((ApplicationRefObject)sender).OnApplicationChanged((Advent.MediaCenter.StartMenu.OEM.Application)args.OldValue, (Advent.MediaCenter.StartMenu.OEM.Application)args.NewValue);
        }

        private static void ApplicationIDChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            sender.SetValue(ApplicationRefObject.ApplicationProperty, (object)((MediaCenterRegistryObject)sender).Manager.Applications[(string)args.NewValue]);
        }
    }
}
