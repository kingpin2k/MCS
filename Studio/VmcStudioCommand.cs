using Advent.Common.UI;
using Microsoft.Windows.Controls.Ribbon;
using System;
using System.Windows;
using System.Windows.Input;
namespace Advent.VmcStudio
{
    public class VmcStudioCommand : RibbonCommand
    {
        public ICommand Command
        {
            get;
            set;
        }
        public FrameworkElement CommandTarget
        {
            get
            {
                VmcDocument selectedDocument = VmcStudioUtil.Application.SelectedDocument;
                if (selectedDocument != null)
                {
                    FrameworkElement frameworkElement = MainWindow.Instance.ElementFromDocument(selectedDocument);
                    if (frameworkElement != null)
                    {
                        IInputElement focusedElement = Keyboard.FocusedElement;
                        if (focusedElement != null && !((DependencyObject)focusedElement).IsDescendantOf(frameworkElement))
                        {
                            return frameworkElement;
                        }
                    }
                }
                return null;
            }
        }
        public VmcStudioCommand()
        {
            base.CanExecute += new CanExecuteRoutedEventHandler(this.VmcStudioCommandCanExecute);
            base.Executed += new ExecutedRoutedEventHandler(this.VmcStudioCommandExecuted);
        }
        private void VmcStudioCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RoutedCommand routedCommand = this.Command as RoutedCommand;
            if (routedCommand != null)
            {
                routedCommand.Execute(e.Parameter, this.CommandTarget);
                return;
            }
            this.Command.Execute(e.Parameter);
        }
        private void VmcStudioCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (VmcStudioUtil.Application.IsExclusiveOperationInProgress)
            {
                e.CanExecute = false;
                e.Handled = true;
                return;
            }
            RoutedCommand routedCommand = this.Command as RoutedCommand;
            if (routedCommand != null)
            {
                e.CanExecute = routedCommand.CanExecute(e.Parameter, this.CommandTarget);
                return;
            }
            e.CanExecute = (this.Command != null && this.Command.CanExecute(e.Parameter));
        }
    }
}
