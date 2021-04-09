using Caliburn.Micro;
using FlightGearProject.ViewModels;
using System.Windows;

namespace FlightGearProject
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }


    }
}
