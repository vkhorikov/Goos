using System.Windows;

namespace Goos.UI
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel(((App)Application.Current).XmppConnection);
        }
    }
}
