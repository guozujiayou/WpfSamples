using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;

namespace WpfPrismDemo.ViewModels
{
    public class Test2ViewModel : BindableBase
    {
        private int count;

        public int Count
        {
            get { return count; }
            set { count = value; RaisePropertyChanged(); }
        }

        public ICommand AddCommand { get; set; }

        public Test2ViewModel()
        {
            AddCommand = new DelegateCommand(() => Count++);
        }
    }
}
