using Prism.Commands;
using System.Windows.Input;

namespace WpfPrismDemo.ViewModels
{
    public class TestViewModel
    {
        public ICommand TestCommand { get; set; }

        public TestViewModel()
        {
            TestCommand = new DelegateCommand(Test);
        }

        private void Test()
        {
            Console.WriteLine();
        }
    }
}
