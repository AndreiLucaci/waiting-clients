using System.Windows;
using CommonServiceLocator;
using Prism.Events;
using Prism.Ioc;
using Prism.Unity;
using Unity.Lifetime;
using WaitingClients.Gui.ViewModels;

namespace WaitingClients.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<MainWindowViewModel, MainWindowViewModel>();
            containerRegistry.Register<IEventAggregator, EventAggregator>();
        }

        protected override Window CreateShell()
        {
            return ServiceLocator.Current.GetInstance<MainWindow>();
        }
    }
}
