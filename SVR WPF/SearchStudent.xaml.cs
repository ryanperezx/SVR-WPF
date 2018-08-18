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
        int i = 1;
        string lastName, firstName;
        public SearchStudent()
        {
            InitializeComponent();
            updateSY();
            time.Content = DateTime.Now.ToString("G");
            startTimer();
            DataContext = ListViewSearchStudent.getList();
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
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT COUNT(1) from StudentInfo WHERE (lastName = @LastName) and (givenName = @firstName)", conn))
            {
                cmd.Parameters.AddWithValue("@lastName", txtLastName.Text);
                cmd.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                if (string.IsNullOrEmpty(txtLastName.Text) && string.IsNullOrEmpty(txtFirstName.Text))
                {
                    MessageBox.Show("No user input!");
                }
                else if (string.IsNullOrEmpty(txtLastName.Text))
                {
                    MessageBox.Show("Please fill up the missing fields!");
                    txtLastName.Focus();
                }
                else if (string.IsNullOrEmpty(txtFirstName.Text))
                {
                    MessageBox.Show("Please fill up the missing fields!");
                    txtFirstName.Focus();
                }
                else
                {
                    int studCount;
                    studCount = (int)cmd.ExecuteScalar();
                    if (studCount > 0)
                    {
                        using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT StudentNo from StudentInfo WHERE (lastName = @LastName) and (givenName = @firstName)", conn))
                        {
                            cmd1.Parameters.AddWithValue("@lastName", txtLastName.Text);
                            cmd1.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                            using (SqlCeDataReader reader = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                            {
                                reader.Read();
                                int studNo = Convert.ToInt32(reader.GetValue(0));
                                ReportSpecific rs = new ReportSpecific(studNo);
                                rs.studNo = studNo;
                                rs.ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Student does not exist!");
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

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT LastName, GivenName from StudentInfo WHERE (LastName LIKE @lastName ) or (GivenName LIKE @firstName)", conn))
            {
                cmd.Parameters.AddWithValue("@lastName", txtLastName.Text + "%");
                cmd.Parameters.AddWithValue("@firstName", txtFirstName.Text + "%");
                using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    lvListStudent.Items.Clear();
                    i = 1;
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int lastNameIndex = reader.GetOrdinal("LastName");
                            string lastName = Convert.ToString(reader.GetValue(lastNameIndex));
                            int firstNameIndex = reader.GetOrdinal("GivenName");
                            string firstName = Convert.ToString(reader.GetValue(firstNameIndex));
                            lvListStudent.Items.Add(new ListViewSearchStudent
                            {
                                i = this.i,
                                LastName = lastName,
                                FirstName = firstName
                            });
                            i++;
                        }
                    }
                }
            }
            conn.Close();

        }

        private void lvListStudent_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListViewSearchStudent lv = lvListStudent.SelectedItem as ListViewSearchStudent;
            txtLastName.Text = lv.LastName;
            txtFirstName.Text = lv.FirstName;
        }
    }
}
