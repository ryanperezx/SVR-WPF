using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlServerCe;
using System.Windows.Media;
namespace SVR_WPF
{
    public partial class Account_Recovery : Window
    {

        public Account_Recovery(string question)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            lblQuestion.Content = question;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }

        public string Answer
        {
            get { return txtAnswer.Text; }
        }

        private void txtAnswer_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtAnswer.Text == "Answer")
            {
                txtAnswer.Foreground = Brushes.Black;
                txtAnswer.Text = "";
            }
        }

        private void txtAnswer_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtAnswer.Text.Length == 0)
            {
                txtAnswer.Foreground = Brushes.DimGray;
                txtAnswer.Text = "Answer";
            }
        }
    }


}
