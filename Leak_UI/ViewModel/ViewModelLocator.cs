/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Leak_UI"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using Leak_UI.Utiles;

namespace Leak_UI.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IDispatcher, DispatcherWrapper>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MainProgramViewModel>();
            SimpleIoc.Default.Register<ExcelRecipeViewModel>();
        }

        public MainViewModel Main {
            get {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        public MainProgramViewModel MainProgramViewModel {
            get {
                return ServiceLocator.Current.GetInstance<MainProgramViewModel>();
            }
        }
        public ExcelRecipeViewModel ExcelRecipeViewModel {
            get {
                return ServiceLocator.Current.GetInstance<ExcelRecipeViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}