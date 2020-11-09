using PagedData.WPF.Services;
using PagedData.WPF.ViewModels;
using Unity;

namespace PagedData.WPF.Utilities
{
    public class ViewModelLocator
    {
        private readonly UnityContainer container;

        public ViewModelLocator()
        {
            container = new UnityContainer();
            container.RegisterType<IEmployeesService, EmployeesService>();
        }

        public MainWindowViewModel MainWindowVM => container.Resolve<MainWindowViewModel>();
    }
}
