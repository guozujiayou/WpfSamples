using System.Windows;

namespace WpfPrismDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //仅捕获ui线程上的未处理异常 无法捕获多线程异常
            //触发的时候没设置e.Handled = true，会接着执行AppDomain.CurrentDomain.UnhandledException
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            //捕获整个应用程序域中的未处理异常(包括多线程异常，但是不包括Task)
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //捕获Task中的异常 Task中的异常并不是立刻就能捕获到的，而是等到垃圾回收的时候进行捕获
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("所有线程中的异常：" + (e.ExceptionObject as Exception)?.Message);
            Console.WriteLine("程序即将终止...");
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            foreach (Exception ex in e.Exception.Flatten().InnerExceptions)
            {
                MessageBox.Show("task异常：" + ex.Message);
            }
            //MessageBox.Show(e.Exception.Message);
            e.SetObserved();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("ui线程异常：" + e.Exception.Message);
            e.Handled = true;
        }
    }

}
