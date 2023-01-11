using FxChat.App.Client.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System.Windows;

namespace FxChat.App.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<ClientMainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            
        }
    }
}
