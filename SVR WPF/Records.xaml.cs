﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NLog;
using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.Data.Common;


namespace SVR_WPF
{
    /// <summary>
    /// Interaction logic for Records.xaml
    /// </summary>
    public partial class Records : Page
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        int value = 0;
        int tempStudNo = 0;
        int i = 1;
        public Boolean permission;
        string[] violations = new string[2];
        string violationName;
        string violationDesc;
        string violationType;
        int countInsti, countDepart, countAcademic, countProbi, countLastChance;
        List<String> violationsHolder = new List<String>();

        public Records()
        {
            InitializeComponent();
            txtDate.Text = DateTime.Today.ToString("d");
            updateSY();
            disableFields();
        }


        private void chkNoLC_Checked(object sender, RoutedEventArgs e)
        {
            if (chkNoLC.IsChecked ?? true)
            {
                chkYesLC.IsChecked = false;
            }
        }
        private void chkYesLC_Checked(object sender, RoutedEventArgs e)
        {
            if (chkYesLC.IsChecked ?? true)
            {
                chkNoLC.IsChecked = false;
            }

        }
        private void chkYesProb_Checked(object sender, RoutedEventArgs e)
        {
            if (chkYesProb.IsChecked ?? true)
            {
                chkNoProb.IsChecked = false;
            }
        }
        private void chkNoProb_Checked(object sender, RoutedEventArgs e)
        {
            if (chkNoProb.IsChecked ?? true)
            {
                chkYesProb.IsChecked = false;
            }
        }


        private void cmbViolate_TextChanged(object sender, TextChangedEventArgs e)
        {
            cmbViolationType.Items.Clear();
            if (txtViolate.Text == "Departmental")
            {
                lblViolationType.Content = "Departmental: ";

                lblViolationType.Visibility = Visibility.Visible;
                cmbViolationType.Visibility = Visibility.Visible;
                btnViolateAdd.Visibility = Visibility.Visible;
                chkYesLC.IsEnabled = true;
                chkNoLC.IsEnabled = true;

                chkYesProb.IsEnabled = false;
                chkNoProb.IsEnabled = false;

                updateViolations();

            }
            else if (txtViolate.Text == "Institutional")
            {
                lblViolationType.Content = "Institutional: ";

                lblViolationType.Visibility = Visibility.Visible;
                cmbViolationType.Visibility = Visibility.Visible;
                btnViolateAdd.Visibility = Visibility.Visible;

                chkYesProb.IsEnabled = false;
                chkNoProb.IsEnabled = false;
                chkYesLC.IsEnabled = false;
                chkNoLC.IsEnabled = false;

                updateViolations();

            }
            else if (txtViolate.Text == "Academic")
            {
                lblViolationType.Content = "Academic: ";
                lblViolationType.Visibility = Visibility.Visible;
                cmbViolationType.Visibility = Visibility.Visible;
                btnViolateAdd.Visibility = Visibility.Visible;


                chkYesLC.IsEnabled = true;
                chkNoLC.IsEnabled = true;
                chkYesProb.IsEnabled = true;
                chkNoProb.IsEnabled = true;

                updateViolations();
            }
            else
            {
                lblViolationType.Visibility = Visibility.Hidden;
                txtRemarks.Visibility = Visibility.Hidden;
                cmbViolationType.Visibility = Visibility.Hidden;
                lblRemarks.Visibility = Visibility.Hidden;
                lblSpecify.Visibility = Visibility.Hidden;
                txtSpecify.Visibility = Visibility.Hidden;
                lblViolationDesc.Visibility = Visibility.Hidden;
                txtViolationDesc.Visibility = Visibility.Hidden;
                btnViolateAdd.Visibility = Visibility.Hidden;
            }
        }
        private void cmbViolationType_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtViolationType.Text == "Others (Please specify)")
            {
                btnViolateAdd.Margin = new Thickness(481, 128, 176, 0);
                txtSpecify.Visibility = Visibility.Visible;
                lblSpecify.Visibility = Visibility.Visible;
                lblViolationDesc.Visibility = Visibility.Visible;
                txtViolationDesc.Visibility = Visibility.Visible;
            }
            else if (txtViolationType.Text != "Others (Please specify)" && txtViolationType.Text != "")
            {
                btnViolateAdd.Margin = new Thickness(148, 128, 509, 0);
                txtSpecify.Visibility = Visibility.Hidden;
                lblSpecify.Visibility = Visibility.Hidden;
                lblViolationDesc.Visibility = Visibility.Hidden;
                txtViolationDesc.Visibility = Visibility.Hidden;
                btnViolateAdd.Visibility = Visibility.Visible;
                txtSpecify.Text = "";
                txtViolationDesc.Text = "";
            }
        }
        private void btnViolateAdd_OnClick(object sender, RoutedEventArgs e)
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            if (txtViolate.Text == "")
            {
                MessageBox.Show("Please fill up the missing fields!");
                cmbViolate.Focus();

            }
            else if (txtViolationType.Text == "")
            {
                MessageBox.Show("Please fill up the missing fields!");
                cmbViolationType.Focus();
            }
            else
            {
                if (txtViolate.Text == "Departmental")
                {
                    if (txtViolationType.Text == "Others (Please specify)")
                    {
                        if (txtSpecify.Text == "" || txtViolationDesc.Text == "")
                        {
                            MessageBox.Show("Specify field or violation description field is empty!");
                            return;
                        }
                        else
                        {
                            violationType = cmbViolate.Text;
                            violationName = txtSpecify.Text;
                            violationDesc = txtViolationDesc.Text;
                            using (SqlCeCommand command = new SqlCeCommand("Insert into ViolationDetails (violationType, violationName, violationDesc) Values (@violationType, @violationName, @violationDesc) ", conn))
                            {
                                command.Parameters.AddWithValue("@violationType", violationType);
                                command.Parameters.AddWithValue("@violationName", violationName);
                                command.Parameters.AddWithValue("@violationDesc", violationDesc);
                                try
                                {
                                    command.ExecuteNonQuery();
                                    Log = LogManager.GetLogger("violationAdded");
                                    Log.Info("Violation Name: " + violationName + " added to database!");
                                    updateViolations();
                                }
                                catch (SqlException ex)
                                {
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex, "Query Error");
                                }
                            }
                            violations[1] = txtSpecify.Text;
                        }
                    }
                    else
                    {
                        violationName = txtViolationType.Text;
                        violations[1] = txtViolationType.Text;
                    }
                    cmbViolationType.SelectedIndex = -1;
                    countDepart++;
                }
                else if (txtViolate.Text == "Institutional")
                {
                    if (txtViolationType.Text == "Others (Please specify)")
                    {
                        if (txtSpecify.Text == "" || txtViolationDesc.Text == "")
                        {
                            MessageBox.Show("Specify field or violation description field is empty!");
                            return;
                        }
                        else
                        {
                            violationType = txtViolate.Text;
                            violationName = txtSpecify.Text;
                            violationDesc = txtViolationDesc.Text;
                            using (SqlCeCommand command = new SqlCeCommand("Insert into ViolationDetails (violationType, violationName, violationDesc) Values (@violationType, @violationName, @violationDesc) ", conn))
                            {
                                command.Parameters.AddWithValue("@violationType", violationType);
                                command.Parameters.AddWithValue("@violationName", violationName);
                                command.Parameters.AddWithValue("@violationDesc", violationDesc);
                                try
                                {
                                    command.ExecuteNonQuery();
                                    Log = LogManager.GetLogger("violationAdded");
                                    Log.Info("Violation Name: " + violationName + " added to database!");
                                    updateViolations();
                                }
                                catch (SqlException ex)
                                {
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex, "Query Error");
                                }
                            }
                            violations[1] = txtSpecify.Text;
                        }
                    }
                    else
                    {
                        violationName = txtViolationType.Text;
                        violations[1] = txtViolationType.Text;
                    }
                    countInsti++;
                    cmbViolationType.SelectedIndex = -1;
                }
                else if (cmbViolate.Text == "Academic")
                {
                    if (txtViolationType.Text == "Others (Please specify)")
                    {
                        if (txtSpecify.Text == "" || txtViolationDesc.Text == "")
                        {
                            MessageBox.Show("Specify field or violation description field is empty!");
                            return;
                        }
                        else
                        {
                            violationType = cmbViolate.Text;
                            violationName = txtSpecify.Text;
                            violationDesc = txtViolationDesc.Text;
                            using (SqlCeCommand command = new SqlCeCommand("Insert into ViolationDetails (violationType, violationName, violationDesc) Values (@violationType, @violationName, @violationDesc) ", conn))
                            {
                                command.Parameters.AddWithValue("@violationType", violationType);
                                command.Parameters.AddWithValue("@violationName", violationName);
                                command.Parameters.AddWithValue("@violationDesc", violationDesc);
                                try
                                {
                                    command.ExecuteNonQuery();
                                    Log = LogManager.GetLogger("violationAdded");
                                    Log.Info("Violation Name: " + violationName + " added to database!");
                                    updateViolations();
                                }
                                catch (SqlException ex)
                                {
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex, "Query Error");
                                    return;
                                }
                            }
                            violations[1] = txtSpecify.Text;
                        }
                    }
                    else
                    {
                        violationName = txtViolationType.Text;
                        violations[1] = txtViolationType.Text;
                    }
                    cmbViolationType.SelectedIndex = -1;
                    countAcademic++;
                }
                violations[0] = i.ToString();
                violationsHolder.Add(violations[1]);
                lvViolations.Items.Add(new ListViewViolations {
                    i = this.i,
                    violationName = violations[1],
                    violationDesc = txtViolationDesc.Text
                });
                txtSpecify.Text = "";
                txtViolationDesc.Text = "";
                cmbViolate.SelectedIndex = -1;
                i++;
            }
        }

        private void txtStudNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtStudNo.Text == "")
                {
                    MessageBox.Show("Please input student number!");
                    txtStudNo.Text = "";
                    emptyComboBox();
                    emptyTextbox();
                }
                else
                {
                    SqlCeConnection conn = DBUtils.GetDBConnection();
                    conn.Open();
                    using (SqlCeCommand cmd = new SqlCeCommand("Select COUNT(1) from StudentInfo where studentNo =" + txtStudNo.Text, conn))
                    {
                        int studCount;
                        int check;
                        if (!int.TryParse(txtStudNo.Text, out check))
                        {
                            MessageBox.Show("Invalid Input!");
                            emptyComboBox();
                            emptyTextbox();
                            return;
                        }
                        else
                        {
                            studCount = (int)cmd.ExecuteScalar();
                        }
                        if (studCount > 0)
                        {
                            string studentNumber = txtStudNo.Text;
                            using (SqlCeCommand cmd1 = new SqlCeCommand("Select * from StudentInfo where studentNo = @studentNo", conn))
                            {
                                cmd1.Parameters.AddWithValue("@studentNo", studentNumber);
                                cmd1.Connection = conn;
                                using (DbDataReader reader = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                                {
                                    if (reader.HasRows)
                                    {
                                        reader.Read();
                                        //0
                                        int studNo = Convert.ToInt32(reader.GetValue(1));
                                        tempStudNo = studNo;
                                        //2
                                        int lNameIndex = reader.GetOrdinal("lastName");
                                        string lName = Convert.ToString(reader.GetValue(lNameIndex));
                                        //3
                                        int fNameIndex = reader.GetOrdinal("givenName");
                                        string fName = Convert.ToString(reader.GetValue(fNameIndex));
                                        //4
                                        int mNameIndex = reader.GetOrdinal("middleName");
                                        string mName = Convert.ToString(reader.GetValue(mNameIndex));
                                        //5
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residenceStatus = Convert.ToString(reader.GetValue(residenceIndex));

                                        txtStudNo.Text = studNo.ToString();
                                        txtLName.Text = lName;
                                        txtFName.Text = fName;
                                        txtMName.Text = mName;
                                        cmbResidence.Text = residenceStatus;
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("There is no record of that user!");
                            emptyComboBox();
                            emptyTextbox();
                        }
                    }
                    conn.Close();
                }

            }
        }
        private void btnSave_OnClick(object sender, RoutedEventArgs e)
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            if (txtStudNo.Text == "" || txtLName.Text == "" || txtFName.Text == "" || cmbResidence.Text == "")
            {
                MessageBox.Show("Please fill up the missing fields!");
                return;
            }
            if (value == 1)
            {
                if (txtStudNo.Text == "" || txtLName.Text == "" || txtFName.Text == "" || cmbResidence.Text == "" || cmbPeriod.Text == "" || cmbSY.Text == "")
                {
                    MessageBox.Show("Please fill up the missing fields!");
                }
                else
                {
                    using (SqlCeCommand cmd = new SqlCeCommand("Select COUNT(1) from StudentInfo where studentNo =" + txtStudNo.Text, conn))
                    {
                        int studNo;
                        int check;
                        int studCount;
                        if (!int.TryParse(txtStudNo.Text, out check))
                        {
                            MessageBox.Show("Invalid Input!");
                            return;
                        }
                        else
                        {
                            studCount = (int)cmd.ExecuteScalar();
                        }
                        if (studCount > 0)
                        {
                            studNo = int.Parse(txtStudNo.Text);
                            string lastName = txtLName.Text;
                            string firstName = txtFName.Text;
                            string middleName = txtMName.Text;
                            string residence = cmbResidence.Text;
                            string period = cmbPeriod.Text;
                            string schoolYear = cmbSY.Text;
                            int sy = int.Parse(schoolYear.Split('-')[0]);
                            violationType = cmbViolate.Text;

                            string remarks;
                            if (txtRemarks.Text == "")
                            {
                                remarks = "None";
                            }
                            else
                            {
                                remarks = txtRemarks.Text;
                            }
                            string date = txtDate.Text;

                            using (SqlCeCommand command = new SqlCeCommand("Update StudentInfo set CounterInsti = CounterInsti + @counterInsti, CounterDept = CounterDept + @counterDept, CounterAcad = CounterAcad + @counterAcad, CounterLastChance = CounterLastChance + @CounterLastChance, CounterProbi = CounterProbi + @CounterProbi where studentNo=" + txtStudNo.Text, conn))
                            {

                                command.Parameters.AddWithValue("@counterInsti", countInsti);
                                command.Parameters.AddWithValue("@counterDept", countDepart);
                                command.Parameters.AddWithValue("@counterAcad", countAcademic);
                                if (chkYesLC.IsChecked ?? true)
                                {
                                    countLastChance = 1;
                                }
                                else if (chkNoLC.IsChecked ?? true)
                                {
                                    countLastChance = 0;
                                }
                                command.Parameters.AddWithValue("@CounterLastChance", countLastChance);
                                if (chkYesProb.IsChecked ?? true)
                                {
                                    countProbi = 1;
                                }
                                else if (chkNoProb.IsChecked ?? true)
                                {
                                    countProbi = 0;
                                }
                                command.Parameters.AddWithValue("@CounterProbi", countProbi);

                                try
                                {
                                    command.ExecuteNonQuery();
                                    MessageBox.Show("Added Successfully");
                                    Log = LogManager.GetLogger("addStudent");
                                    Log.Info("Student no: " + studNo + " added to database!");
                                }
                                catch (SqlException ex)
                                {
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex, "Query Error");
                                }
                            }
                            foreach (var violation in violationsHolder)
                            {
                                using (SqlCeCommand cmd2 = new SqlCeCommand("Select ViolationCode from ViolationDetails where violationName = '" + violation + "'", conn))
                                {
                                    int violationCode = 0;
                                    using (DbDataReader reader = cmd2.ExecuteResultSet(ResultSetOptions.Scrollable))
                                    {
                                        if (reader.HasRows)
                                        {
                                            reader.Read();
                                            violationCode = Convert.ToInt32(reader.GetValue(0));
                                        }
                                    }
                                    using (SqlCeCommand command = new SqlCeCommand("INSERT INTO RecordDetails (studentNo, ViolationCode, DateCommitted, Period, SY, Remarks) VALUES (@StudentNo, @ViolationCode, @DateCommitted, @Period, @SY, @Remarks)", conn))
                                    {
                                        command.Parameters.AddWithValue("@StudentNo", studNo);
                                        command.Parameters.AddWithValue("@ViolationCode", violationCode);
                                        command.Parameters.AddWithValue("@DateCommitted", date);
                                        command.Parameters.AddWithValue("@Period", period);
                                        command.Parameters.AddWithValue("@SY", sy);
                                        command.Parameters.AddWithValue("@Remarks", remarks);

                                        try
                                        {
                                            command.ExecuteNonQuery();
                                            Log = LogManager.GetLogger("studentRecord");
                                            Log.Info("Student no:" + studNo + " records added to database!");
                                            emptyTextbox();
                                            emptyComboBox();
                                        }
                                        catch (SqlException ex)
                                        {
                                            Log = LogManager.GetLogger("*");
                                            Log.Error(ex, "Error has been encountered! Log has been updated with the error");
                                            emptyTextbox();
                                            emptyComboBox();
                                        }
                                    }
                                }
                            }
                            disableFields();
                            updateViolations();
                            emptyValues();
                            violationsHolder.Clear();
                            lvViolations.Items.Clear();
                            i = 1;
                        }
                        else
                        {
                            try
                            {
                                if (!int.TryParse(txtStudNo.Text, out check))
                                {
                                    MessageBox.Show("Invalid Input!");
                                    return;
                                }
                                else
                                {
                                    studNo = int.Parse(txtStudNo.Text);
                                }
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("Query error! Log has been updated with the error");
                                Log = LogManager.GetLogger("*");
                                Log.Error(ex);
                                return;
                            }
                            string lastName = txtLName.Text;
                            string firstName = txtFName.Text;
                            string middleName = txtMName.Text;
                            string residence = cmbResidence.Text;
                            string period = cmbPeriod.Text;
                            string schoolYear = cmbSY.Text;
                            int sy = int.Parse(schoolYear.Split('-')[0]);
                            string remarks;
                            if (txtRemarks.Text == "")
                            {
                                remarks = "None";
                            }
                            else
                            {
                                remarks = txtRemarks.Text;
                            }
                            string date = txtDate.Text;

                            using (SqlCeCommand command = new SqlCeCommand("INSERT INTO StudentInfo (studentNo, lastName, givenName, middleName, residenceStatus, counterLastChance, counterDept, counterAcad, counterProbi, counterInsti) VALUES (@studentNo, @LastName, @GivenName, @MiddleName, @residenceStatus, @counterLastChance, @counterDept, @counterAcad , @counterProbi, @counterInsti)", conn))
                            {
                                command.Parameters.AddWithValue("@studentNo", studNo);
                                command.Parameters.AddWithValue("@LastName", lastName);
                                command.Parameters.AddWithValue("@GivenName", firstName);
                                command.Parameters.AddWithValue("@MiddleName", middleName);
                                command.Parameters.AddWithValue("@residenceStatus", residence);
                                command.Parameters.AddWithValue("@counterInsti", countInsti);
                                command.Parameters.AddWithValue("@CounterDept", countDepart);
                                command.Parameters.AddWithValue("@CounterAcad", countAcademic);
                                command.Parameters.AddWithValue("@CounterProbi", countProbi);
                                if (chkYesLC.IsChecked ?? true)
                                {
                                    countLastChance = 1;
                                }
                                else if (chkNoLC.IsChecked ?? true)
                                {
                                    countLastChance = 0;
                                }
                                command.Parameters.AddWithValue("@CounterLastChance", countLastChance);
                                try
                                {
                                    command.ExecuteNonQuery();
                                    MessageBox.Show("Added Successfully");
                                    Log = LogManager.GetLogger("addStudent");
                                    Log.Info("Student no: " + studNo + " added to database!");

                                }
                                catch (SqlException ex)
                                {
                                    Log = LogManager.GetLogger("*");
                                    Log.Error("Query Error: " + ex);

                                }
                            }
                            foreach (var violation in violationsHolder)
                            {
                                using (SqlCeCommand cmd2 = new SqlCeCommand("Select ViolationCode from ViolationDetails where violationName = '" + violationName + "'", conn))
                                {
                                    int violationCode = 0;
                                    using (DbDataReader reader = cmd2.ExecuteResultSet(ResultSetOptions.Scrollable))
                                    {
                                        if (reader.HasRows)
                                        {
                                            reader.Read();
                                            violationCode = Convert.ToInt32(reader.GetValue(0));
                                        }
                                    }

                                    using (SqlCeCommand command = new SqlCeCommand("INSERT INTO RecordDetails (studentNo, ViolationCode, DateCommitted, Period, SY, Remarks) VALUES (@studentNo, @ViolationCode, @dateCommitted, @Period, @SY, @Remarks)", conn))
                                    {
                                        command.Parameters.AddWithValue("@studentNo", studNo);
                                        command.Parameters.AddWithValue("@ViolationCode", violationCode);
                                        command.Parameters.AddWithValue("@dateCommitted", date);
                                        command.Parameters.AddWithValue("@Period", period);
                                        command.Parameters.AddWithValue("@SY", sy);
                                        command.Parameters.AddWithValue("@Remarks", remarks);

                                        try
                                        {
                                            command.ExecuteNonQuery();
                                            Log = LogManager.GetLogger("addStudent");
                                            Log.Info("Student no: " + studNo + " records has been added to database!");
                                            emptyTextbox();
                                            emptyComboBox();
                                        }
                                        catch (SqlException ex)
                                        {
                                            Log = LogManager.GetLogger("*");
                                            Log.Error("Query Error: " + ex);
                                            emptyTextbox();
                                            emptyComboBox();
                                        }
                                    }
                                }
                            }
                            disableFields();
                            updateViolations();
                            emptyValues();
                            violationsHolder.Clear();
                            lvViolations.Items.Clear();
                            i = 1;
                        }
                    }
                    conn.Close();
                }
            }
            else if (value == 2)
            {
                if (txtLName.Text == "" || txtFName.Text == "" || cmbResidence.Text == "" || txtStudNo.Text == "")
                {
                    MessageBox.Show("Please fill up the missing fields!");
                }
                else
                {
                    string sMessageBoxText = "Do you want to update the record?";
                    string sCaption = "Edit Record";
                    MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                    MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                    switch (dr)
                    {
                        case MessageBoxResult.Yes:
                            string studNo = txtStudNo.Text;
                            string lastName = txtLName.Text;
                            string firstName = txtFName.Text;
                            string middleName = txtMName.Text;
                            string residence = cmbResidence.Text;
                            using (SqlCeCommand cmd = new SqlCeCommand("UPDATE StudentInfo SET StudentNo = @StudNo, LastName = @LName, GivenName = @FName, MiddleName = @MName, ResidenceStatus = @residence where studentNo =" + tempStudNo, conn))
                            {
                                cmd.Parameters.AddWithValue("@StudNo", studNo);
                                cmd.Parameters.AddWithValue("@LName", lastName);
                                cmd.Parameters.AddWithValue("@FName", firstName);
                                cmd.Parameters.AddWithValue("@MName", middleName);
                                cmd.Parameters.AddWithValue("@residence", residence);

                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    MessageBox.Show("Updated Successfully");
                                    Log = LogManager.GetLogger("addStudent");
                                    Log.Info("Updated Information for student no:" + txtStudNo.Text);
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("Error: " + ex);
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex + "Query Error");
                                }
                            }
                            txtStudNo.IsReadOnly = false;
                            emptyTextbox();
                            emptyComboBox();
                            disableFields();
                            conn.Close();
                            break;

                        case MessageBoxResult.No: break;
                    }
                }
            }
        }
        private void btnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            value = 1;
            enableFields();
            emptyComboBox();
            emptyTextbox();
        }
        private void btnEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (txtStudNo.Text == "")
            {
                MessageBox.Show("Please input student number in the field before editing!");
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("Select COUNT(1) from StudentInfo where studentNo =" + txtStudNo.Text, conn))
                {
                    int check;
                    int studCount;
                    if (!int.TryParse(txtStudNo.Text, out check))
                    {
                        MessageBox.Show("Invalid Input!");
                        return;
                    }
                    else
                    {
                        studCount = (int)cmd.ExecuteScalar();
                    }
                    if (studCount > 0)
                    {
                        value = 2;
                        enableFields();
                        cmbPeriod.IsEnabled = false;
                        cmbSY.IsEnabled = false;
                        cmbViolate.IsEnabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Student does not exist!");
                        emptyTextbox();
                        emptyComboBox();
                    }
                }
                conn.Close();
            }
        }

        private void emptyComboBox()
        {
            cmbResidence.SelectedIndex = -1;
            cmbPeriod.SelectedIndex = -1;
            cmbSY.SelectedIndex = -1;
            cmbViolate.SelectedIndex = -1;
        }
        private void emptyTextbox()
        {
            txtStudNo.Text = "";
            txtFName.Text = "";
            txtMName.Text = "";
            txtLName.Text = "";
            /*
            txtSpecify.Text = "";
            txtViolateDesc.Text = "";
            */
            txtRemarks.Text = "";
        }
        private void emptyValues()
        {
            countAcademic = 0;
            countDepart = 0;
            countInsti = 0;
            countLastChance = 0;
            countProbi = 0;
        }


        private void disableFields()
        {
            txtLName.IsReadOnly = true;
            txtFName.IsReadOnly = true;
            txtMName.IsReadOnly = true;
            txtRemarks.IsReadOnly = true;

            cmbResidence.IsEnabled = false;
            cmbPeriod.IsEnabled = false;
            cmbSY.IsEnabled = false;
            cmbViolate.IsEnabled = false;

            chkYesLC.IsEnabled = false;
            chkNoLC.IsEnabled = false;
            chkYesProb.IsEnabled = false;
            chkNoProb.IsEnabled = false;

            btnViolateAdd.IsEnabled = false;
            btnViolateAdd.Visibility = Visibility.Hidden;

            lblViolationType.Visibility = Visibility.Hidden;
            cmbViolationType.Visibility = Visibility.Hidden;
        }
        private void enableFields()
        {
            txtStudNo.IsReadOnly = false;
            txtLName.IsReadOnly = false;
            txtFName.IsReadOnly = false;
            txtMName.IsReadOnly = false;
            txtRemarks.IsEnabled = false;

            cmbResidence.IsEnabled = true;
            cmbPeriod.IsEnabled = true;
            cmbSY.IsEnabled = true;
            cmbViolate.IsEnabled = true;

            btnViolateAdd.IsEnabled = true;
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
                            cmbViolationType.Items.Clear();
                            cmbViolationType.Items.Add("Others (Please specify)");
                            while (reader.Read())
                            {
                                string ViolationName = reader["ViolationName"].ToString();
                                cmbViolationType.Items.Add(ViolationName);
                            }
                        }
                    }
                }
            }
            else if (txtViolate.Text == "Institutional")
                using (SqlCeCommand sql = new SqlCeCommand("Select ViolationType, ViolationName from ViolationDetails where ViolationType ='Institutional'", conn))
                {
                    using (DbDataReader reader = sql.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            cmbViolationType.Items.Clear();
                            cmbViolationType.Items.Add("Others (Please specify)");
                            while (reader.Read())
                            {
                                string ViolationName = reader["ViolationName"].ToString();
                                cmbViolationType.Items.Add(ViolationName);
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
                            cmbViolationType.Items.Clear();
                            cmbViolationType.Items.Add("Others (Please specify)");
                            while (reader.Read())
                            {
                                string ViolationName = reader["ViolationName"].ToString();
                                cmbViolationType.Items.Add(ViolationName);
                            }
                        }
                    }
                }
            }
            conn.Close();
        }
        private void updateSY()
        {
            int currentYear = DateTime.Now.Year;
            cmbSY.Items.Clear();
            int b = -2;
            for (int i = 2015; i <= currentYear; i++)
            {
                cmbSY.Items.Add(i + "-" + (currentYear + b));
                b++;
            }
        }
        /*
        private void checkAccountLevel()
        {
            ((Control)this.tabSearch).Enabled = permission;
            ((Control)this.tabUsers).Enabled = permission;
            btnEditProb.Enabled = permission;
            btnDeleteRecord.Enabled = permission;
            btnGenGenReport.Enabled = permission;
            btnSpeGenReport.Enabled = permission;
        }
        */
    }
}