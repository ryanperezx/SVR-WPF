using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SVR_WPF
{

    public partial class ReportSpecific : Window
    {
        public int studNo;
        int institutionalCount, departmentalCount, academicCount;
        string residence, fullName;
        int row = 1, column = 8;
        string[] records = new string[8];
        public ReportSpecific(int studNo)
        {
            InitializeComponent();
            this.studNo = studNo;        }
    }
}
