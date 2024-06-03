using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfPrismDemo.Domain;
using WpfPrismDemo.Views;

namespace WpfPrismDemo.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string title = "prism";

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<MenuItemInfo> MenuItems { get; set; }

        private MenuItemInfo? _selectedItem;

        public MenuItemInfo? SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; RaisePropertyChanged(); }
        }


        public ICommand TestCommand { get; set; }

        public MainWindowViewModel()
        {
            TestCommand = new DelegateCommand(() =>
            {
                Console.WriteLine("hello world");
            });

            MenuItems = new ObservableCollection<MenuItemInfo>
        {
            //不绑定的话 直接在前端Text=&#xe614;  但是要绑定的话要改为下面这样
            new MenuItemInfo("test","",typeof(TestView)),
            new MenuItemInfo("test2","",typeof(Test2View)),
        };

            SelectedItem = MenuItems.FirstOrDefault();
        }
    }
}
