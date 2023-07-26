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
            SimpleIoc.Default.Register<Main_MatchingViewModel>();
            SimpleIoc.Default.Register<ExcelRecipe_ViewModel>();
            SimpleIoc.Default.Register<ReissueListViewModel>();
            SimpleIoc.Default.Register<TemporaryPrintViewModel>();
        }

        public MainViewModel Main {
            get {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        public Main_MatchingViewModel Main_MatchingViewModel {
            get {
                return ServiceLocator.Current.GetInstance<Main_MatchingViewModel>();
            }
        }
        public ExcelRecipe_ViewModel ExcelRecipe_ViewModel {
            get {
                return ServiceLocator.Current.GetInstance<ExcelRecipe_ViewModel>();
            }
        }
        public ReissueListViewModel ReissueListViewModel {
            get {
                return ServiceLocator.Current.GetInstance<ReissueListViewModel>();
            }
        }
        public TemporaryPrintViewModel TemporaryPrintViewModel {
            get {
                return ServiceLocator.Current.GetInstance<TemporaryPrintViewModel>();
            }
        }
        public static void Cleanup(int position)
        {
            // TODO Clear the ViewModels
        }
    }
}