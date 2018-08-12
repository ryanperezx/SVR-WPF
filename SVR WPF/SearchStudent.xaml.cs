using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace SVR_WPF
{
    /// <summary>
    /// Interaction logic for SearchStudent.xaml
    /// </summary>
    public partial class SearchStudent : Page
    {
        public SearchStudent()
        {
            InitializeComponent();
            updateSY();
            time.Content = DateTime.Now.ToString("G");
            startTimer();
        }

        private void updateSY()
        {
            int currentYear = DateTime.Now.Year;
            cmbSYFrom.Items.Clear();
            cmbSYTo.Items.Clear();
            int b = -2;
            for (int i = 2015; i <= currentYear; i++)
            {
                cmbSYFrom.Items.Add(i);
                cmbSYTo.Items.Add(currentYear + b);
                b++;
            }
        }
        private void updateViolations()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            if (txtViolate.Text == "Departmental")
            {
                using (SqlCeCommand sql = new SqlCeCommand("Select ViolationType, ViolationName from ViolationDetails where ViolationType ='Departmental'", conn))
                {
                    using (DbDataReader reader = sql.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            cmbViolationName.Items.Clear();
                            cmbViolationName.Items.Add("ALL");
                            while (reader.Read())
                            {
                                string ViolationName = reader["ViolationName"].ToString();
                                cmbViolationName.Items.Add(ViolationName);
                            }
                        }
                    }
                }
            }
            else if (txtViolate.Text == "Institutional")
            {
                using (SqlCeCommand sql = new SqlCeCommand("Select ViolationType, ViolationName from ViolationDetails where ViolationType ='Institutional'", conn))
                {
                    using (DbDataReader reader = sql.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            cmbViolationName.Items.Clear();
                            cmbViolationName.Items.Add("ALL");
                            while (reader.Read())
                            {
                                string ViolationName = reader["ViolationName"].ToString();
                                cmbViolationName.Items.Add(ViolationName);
                            }
                        }
                    }
                }
            }
            else if (txtViolate.Text == "Academic")
            {
                using (SqlCeCommand sql = new SqlCeCommand("Select ViolationType, ViolationName from ViolationDetails where ViolationType ='Academic'", conn))
                {
                    using (DbDataReader reader = sql.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            cmbViolationName.Items.Clear();
                            cmbViolationName.Items.Add("ALL");
                            while (reader.Read())
                            {
                                string ViolationName = reader["ViolationName"].ToString();
                                cmbViolationName.Items.Add(ViolationName);
                            }
                        }
                    }
                }
            }
            conn.Close();
        }
        private void startTimer()
        {
            System.Windows.Forms.Timer tmr = null;
            tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 1000;
            tmr.Tick += new EventHandler(tmr_Tick);
            tmr.Enabled = true;
        }
        private void tmr_Tick(object sender, EventArgs e)
        {
            time.Content = DateTime.Now.ToString("G");
        }

        private void cmbViolate_TextChanged(object sender, TextChangedEventArgs e)
        {
            cmbViolationName.Items.Clear();
            updateViolations();
        }


        private void btnGenGenReport_Click(object sender, RoutedEventArgs e)
        {
            if (cmbViolate.Text == "" || cmbResidence.Text == "" || cmbPeriod.Text == "" || cmbSYFrom.Text == "" || cmbSYTo.Text == "")
            {
                MessageBox.Show("Please fill up the missing field(s)!");
            }
            else
            {
                if (txtViolate.Text != "ALL" && txtViolationName.Text == "")
                {
                    MessageBox.Show("Please select from the given Violations!");
                    cmbViolationName.Focus();
                }
                else
                {
                    ReportGeneral rg = new ReportGeneral(cmbPeriod.SelectedValue.ToString(), txtSYFrom.Text, txtSYTo.Text, txtViolationName.Text, txtViolate.Text, cmbResidence.SelectedValue.ToString());
                    rg.ShowDialog();
                }
            }
        }
        private void btnSpeGenReport_Click(object sender, RoutedEventArgs e)
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("Select COUNT(1) from StudentInfo where studentNo = @studentNo;", conn))
            {
                cmd.Parameters.AddWithValue("@studentNo", txtStudNo.Text);
                if (txtStudNo.Text == "")
                {
                    MessageBox.Show("No user input!");
                }
                else
                {
                    int studCount;
                    if (!int.TryParse(txtStudNo.Text, out studCount))
                    {
                        MessageBox.Show("Invalid Input!");
                        return;
                    }
                    else
                    {
                        studCount = (int)cmd.ExecuteScalar();
                        if (studCount > 0)
                        {
                            ReportSpecific rs = new ReportSpecific(int.Parse(txtStudNo.Text));
                            rs.studNo = int.Parse(txtStudNo.Text);
                            rs.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("Student does not exist!");
                        }
                    }
                }
            }
            conn.Close();
        }

        private void txtStudNo_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
    }
}
