using Prism.DryIoc;
using Prism.Ioc;
using System.Windows;
using WpfPrismDemo.Views;

namespace WpfPrismDemo
{
    internal class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
