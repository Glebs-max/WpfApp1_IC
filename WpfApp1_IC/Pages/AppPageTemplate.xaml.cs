using System.Windows.Controls;

namespace WpfApp1_IC.Pages
{
    public partial class AppPageTemplate : UserControl
    {
        public AppPageTemplate()
        {
            InitializeComponent();
        }

        public void SetPath(string path)
        {
            PagePath.Text = path;
        }

        public void Log(string message)
        {
            LogTextBox.AppendText(message + "\n");
            LogTextBox.ScrollToEnd();
        }
    }
}
