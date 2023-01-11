using FxChat.App.Server.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace FxChat.App.Server
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<ServerMainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
