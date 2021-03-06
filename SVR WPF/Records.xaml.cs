﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NLog;
using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text.RegularExpressions;


namespace SVR_WPF
{
    /// <summary>
    /// Interaction logic for Records.xaml
    /// </summary>
    public partial class Records : Page
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        int value = 0;
        int i = 1;
        public int userLevel;
        int tempStudNo = 0;
        string[] violations = new string[2];
        string violationName;
        string violationDesc;
        string violationType;
        string user, password;
        int countInsti, countDepart, countAcademic, countProbi;
        List<String> violationsHolder = new List<String>();

        public Records(int userLevel, string user)
        {
            InitializeComponent();
            txtDate.Text = DateTime.Today.ToString("d");
            updateSY();
            disableFields();
            this.userLevel = userLevel;
            this.user = user;
            time.Content = DateTime.Now.ToString("G");
            DataContext = new RecordViewModel();
            checkAccountLevel();
            startTimer();
        }




        private void cmbViolate_TextChanged(object sender, TextChangedEventArgs e)
        {
            cmbViolationName.Items.Clear();
            if (txtViolate.Text == "Departmental")
            {
                lblViolationName.Content = "Departmental: ";

                lblViolationName.Visibility = Visibility.Visible;
                cmbViolationName.Visibility = Visibility.Visible;
                btnViolateAdd.Visibility = Visibility.Visible;
                txtRemarks.IsReadOnly = false;

                updateViolations();

            }
            else if (txtViolate.Text == "Institutional")
            {
                lblViolationName.Content = "Institutional: ";

                lblViolationName.Visibility = Visibility.Visible;
                cmbViolationName.Visibility = Visibility.Visible;
                btnViolateAdd.Visibility = Visibility.Visible;
                txtRemarks.IsReadOnly = false;

                updateViolations();

            }
            else if (txtViolate.Text == "Academic")
            {
                lblViolationName.Content = "Academic: ";
                lblViolationName.Visibility = Visibility.Visible;
                cmbViolationName.Visibility = Visibility.Visible;
                btnViolateAdd.Visibility = Visibility.Visible;
                txtRemarks.IsReadOnly = false;

                updateViolations();
            }
        }
        private void cmbViolationName_TextChanged(object sender, TextChangedEventArgs e)
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
            if (txtViolate.Text == "")
            {
                MessageBox.Show("Please fill up the missing fields!");
                cmbViolate.Focus();

            }
            else if (txtViolationType.Text == "")
            {
                MessageBox.Show("Please fill up the missing fields!");
                cmbViolationName.Focus();
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                if (txtViolate.Text == "Departmental")
                {
                    if (txtViolationType.Text == "Others (Please specify)")
                    {
                        if (txtSpecify.Text == "" || txtViolationDesc.Text == "")
                        {
                            MessageBox.Show("Specify field or violation description field is empty!");
                            conn.Close();
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
                                    conn.Close();
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
                        using (SqlCeCommand cmd = new SqlCeCommand("SELECT violationDesc from ViolationDetails where violationName= @violationName", conn))
                        {
                            cmd.Parameters.AddWithValue("@violationName", violationName);
                            using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    int violationDescIndex = reader.GetOrdinal("violationDesc");
                                    violationDesc = Convert.ToString(reader.GetValue(violationDescIndex));
                                }
                            }
                        }
                    }
                    cmbViolationName.SelectedIndex = -1;
                    countDepart++;
                }
                else if (txtViolate.Text == "Institutional")
                {
                    if (txtViolationType.Text == "Others (Please specify)")
                    {
                        if (txtSpecify.Text == "" || txtViolationDesc.Text == "")
                        {
                            MessageBox.Show("Specify field or violation description field is empty!");
                            conn.Close();
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
                                    conn.Close();
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
                        using (SqlCeCommand cmd = new SqlCeCommand("SELECT violationDesc from ViolationDetails where violationName= @violationName", conn))
                        {
                            cmd.Parameters.AddWithValue("@violationName", violationName);
                            using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    int violationDescIndex = reader.GetOrdinal("violationDesc");
                                    violationDesc = Convert.ToString(reader.GetValue(violationDescIndex));
                                }
                            }
                        }
                    }
                    countInsti++;
                    cmbViolationName.SelectedIndex = -1;
                }
                else if (cmbViolate.Text == "Academic")
                {
                    if (txtViolationType.Text == "Others (Please specify)")
                    {
                        if (txtSpecify.Text == "" || txtViolationDesc.Text == "")
                        {
                            MessageBox.Show("Specify field or violation description field is empty!");
                            conn.Close();
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
                                    countProbi = 0;
                                    conn.Close();
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
                        using (SqlCeCommand cmd = new SqlCeCommand("SELECT violationDesc from ViolationDetails where violationName= @violationName", conn))
                        {
                            cmd.Parameters.AddWithValue("@violationName", violationName);
                            using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    int violationDescIndex = reader.GetOrdinal("violationDesc");
                                    violationDesc = Convert.ToString(reader.GetValue(violationDescIndex));
                                }
                            }
                        }
                    }
                    countProbi++;
                    cmbViolationName.SelectedIndex = -1;
                    countAcademic++;
                }
                violations[0] = i.ToString();
                violationsHolder.Add(violations[1]);
                lvViolations.Items.Add(new ListViewViolations
                {
                    i = this.i,
                    violationName = violations[1],
                    violationDesc = this.violationDesc
                });
                txtSpecify.Text = "";
                txtViolationDesc.Text = "";
                cmbViolate.SelectedIndex = -1;
                i++;
                conn.Close();
            }
        }


        private void txtStudNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
        private void txtStudNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(txtStudNo.Text))
                {
                    MessageBox.Show("Please input student number!");
                    emptyComboBox();
                    emptyTextbox();
                }
                else
                {
                    SqlCeConnection conn = DBUtils.GetDBConnection();
                    conn.Open();
                    using (SqlCeCommand cmd = new SqlCeCommand("Select COUNT(1) from StudentInfo where studentNo = @studNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@studNo", txtStudNo.Text);
                        int studCount;
                        studCount = (int)cmd.ExecuteScalar();
                        if (studCount > 0)
                        {
                            string studentNumber = txtStudNo.Text;
                            using (SqlCeCommand cmd1 = new SqlCeCommand("Select * from StudentInfo where studentNo = @studentNo", conn))
                            {
                                cmd1.Parameters.AddWithValue("@studentNo", studentNumber);
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
                                        int fNameIndex = reader.GetOrdinal("firstName");
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
                    if (lvViolations.Items.Count <= 0)
                    {
                        MessageBox.Show("There is no violation provided!");
                    }
                    else
                    {
                        using (SqlCeCommand cmd = new SqlCeCommand("Select COUNT(1) from StudentInfo where studentNo = @studentNo", conn))
                        {
                            cmd.Parameters.AddWithValue("@studentNo", txtStudNo.Text);
                            int studNo;
                            int studCount = (int)cmd.ExecuteScalar();
                            if (studCount > 0)
                            {
                                using(SqlCeCommand cmd1 = new SqlCeCommand("SELECT CounterDept + CounterAcad + CounterInsti as violationCount from StudentInfo where StudentNo = @studentNo", conn))
                                {
                                    cmd1.Parameters.AddWithValue("@studentNo", txtStudNo.Text);
                                    int violationCount = 0;
                                    using (DbDataReader reader = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                                    {
                                        if (reader.HasRows)
                                        {
                                            reader.Read();
                                            violationCount = Convert.ToInt32(reader.GetValue(0));
                                        }
                                    }
                                    if(violationCount > 4)
                                    {
                                        using (SqlCeCommand cmd2 = new SqlCeCommand("Select Password from Accounts where userID = @userID", conn))
                                        {
                                            cmd2.Parameters.AddWithValue("@userID", user);
                                            using (DbDataReader reader = cmd2.ExecuteResultSet(ResultSetOptions.Scrollable))
                                            {
                                                reader.Read();
                                                int passwordIndex = reader.GetOrdinal("password");
                                                password = Convert.ToString(reader.GetValue(passwordIndex));
                                            }
                                        }
                                        Account_Confirm ac = new Account_Confirm();
                                        if(ac.ShowDialog() == true)
                                        {
                                            string input = ac.Password;
                                            if (input.Equals(password))
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

                                                using (SqlCeCommand command = new SqlCeCommand("UPDATE StudentInfo set CounterInsti = CounterInsti + @counterInsti, CounterDept = CounterDept + @counterDept, CounterAcad = CounterAcad + @counterAcad, CounterProbi = CounterProbi + @counterProbi where studentNo = @studentNo", conn))
                                                {

                                                    command.Parameters.AddWithValue("@counterInsti", countInsti);
                                                    command.Parameters.AddWithValue("@counterDept", countDepart);
                                                    command.Parameters.AddWithValue("@counterAcad", countAcademic);
                                                    command.Parameters.AddWithValue("@studentNo", txtStudNo.Text);
                                                    command.Parameters.AddWithValue("@counterProbi", countProbi);
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
                                                    using (SqlCeCommand cmd2 = new SqlCeCommand("Select ViolationCode from ViolationDetails where violationName = @violationName", conn))
                                                    {
                                                        cmd2.Parameters.AddWithValue("@violationName", violation);
                                                        int violationCode = 0;
                                                        using (DbDataReader reader = cmd2.ExecuteResultSet(ResultSetOptions.Scrollable))
                                                        {
                                                            if (reader.HasRows)
                                                            {
                                                                reader.Read();
                                                                violationCode = Convert.ToInt32(reader.GetValue(0));
                                                            }
                                                        }
                                                        using (SqlCeCommand command = new SqlCeCommand("INSERT INTO RecordDetails (studentNo, ViolationCode, DateCommitted, Period, SYint, SY, Remarks) VALUES (@StudentNo, @ViolationCode, @DateCommitted, @Period, @SYint, @SY, @Remarks)", conn))
                                                        {
                                                            command.Parameters.AddWithValue("@StudentNo", studNo);
                                                            command.Parameters.AddWithValue("@ViolationCode", violationCode);
                                                            command.Parameters.AddWithValue("@DateCommitted", date);
                                                            command.Parameters.AddWithValue("@Period", period);
                                                            command.Parameters.AddWithValue("@SYint", sy);
                                                            command.Parameters.AddWithValue("@SY", schoolYear);
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
                                                MessageBox.Show("Your password is incorrect, please try again.");
                                            }
                                        }
                                    }
                                    else
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

                                        using (SqlCeCommand command = new SqlCeCommand("UPDATE StudentInfo set CounterInsti = CounterInsti + @counterInsti, CounterDept = CounterDept + @counterDept, CounterAcad = CounterAcad + @counterAcad, CounterProbi = CounterProbi + @counterProbi where studentNo = @studentNo", conn))
                                        {

                                            command.Parameters.AddWithValue("@counterInsti", countInsti);
                                            command.Parameters.AddWithValue("@counterDept", countDepart);
                                            command.Parameters.AddWithValue("@counterAcad", countAcademic);
                                            command.Parameters.AddWithValue("@studentNo", txtStudNo.Text);
                                            command.Parameters.AddWithValue("@counterProbi", countProbi);
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
                                            using (SqlCeCommand cmd2 = new SqlCeCommand("Select ViolationCode from ViolationDetails where violationName = @violationName", conn))
                                            {
                                                cmd2.Parameters.AddWithValue("@violationName", violation);
                                                int violationCode = 0;
                                                using (DbDataReader reader = cmd2.ExecuteResultSet(ResultSetOptions.Scrollable))
                                                {
                                                    if (reader.HasRows)
                                                    {
                                                        reader.Read();
                                                        violationCode = Convert.ToInt32(reader.GetValue(0));
                                                    }
                                                }
                                                using (SqlCeCommand command = new SqlCeCommand("INSERT INTO RecordDetails (studentNo, ViolationCode, DateCommitted, Period, SYint, SY, Remarks) VALUES (@StudentNo, @ViolationCode, @DateCommitted, @Period, @SYint, @SY, @Remarks)", conn))
                                                {
                                                    command.Parameters.AddWithValue("@StudentNo", studNo);
                                                    command.Parameters.AddWithValue("@ViolationCode", violationCode);
                                                    command.Parameters.AddWithValue("@DateCommitted", date);
                                                    command.Parameters.AddWithValue("@Period", period);
                                                    command.Parameters.AddWithValue("@SYint", sy);
                                                    command.Parameters.AddWithValue("@SY", schoolYear);
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
                                }
                            }
                            else
                            {
                                if (userLevel == 1)
                                {
                                    try
                                    {
                                        studNo = int.Parse(txtStudNo.Text);
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

                                    using (SqlCeCommand command = new SqlCeCommand("INSERT INTO StudentInfo (studentNo, lastName, firstName, middleName, residenceStatus, counterDept, counterAcad, counterProbi, counterInsti) VALUES (@studentNo, @LastName, @firstName, @MiddleName, @residenceStatus, @counterDept, @counterAcad, @counterProbi, @counterInsti)", conn))
                                    {
                                        command.Parameters.AddWithValue("@studentNo", studNo);
                                        command.Parameters.AddWithValue("@LastName", lastName);
                                        command.Parameters.AddWithValue("@firstName", firstName);
                                        command.Parameters.AddWithValue("@MiddleName", middleName);
                                        command.Parameters.AddWithValue("@residenceStatus", residence);
                                        command.Parameters.AddWithValue("@counterInsti", countInsti);
                                        command.Parameters.AddWithValue("@counterDept", countDepart);
                                        command.Parameters.AddWithValue("@counterAcad", countAcademic);
                                        command.Parameters.AddWithValue("@counterProbi", countProbi);
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
                                        using (SqlCeCommand cmd2 = new SqlCeCommand("Select ViolationCode from ViolationDetails where violationName = @violationName", conn))
                                        {
                                            cmd2.Parameters.AddWithValue("@violationName", violation);
                                            int violationCode = 0;
                                            using (DbDataReader reader = cmd2.ExecuteResultSet(ResultSetOptions.Scrollable))
                                            {
                                                if (reader.HasRows)
                                                {
                                                    reader.Read();
                                                    violationCode = Convert.ToInt32(reader.GetValue(0));
                                                }
                                            }

                                            using (SqlCeCommand command = new SqlCeCommand("INSERT INTO RecordDetails (studentNo, ViolationCode, DateCommitted, Period, SYint, SY, Remarks) VALUES (@StudentNo, @ViolationCode, @DateCommitted, @Period, @SYint, @SY, @Remarks)", conn))
                                            {
                                                command.Parameters.AddWithValue("@StudentNo", studNo);
                                                command.Parameters.AddWithValue("@ViolationCode", violationCode);
                                                command.Parameters.AddWithValue("@DateCommitted", date);
                                                command.Parameters.AddWithValue("@Period", period);
                                                command.Parameters.AddWithValue("@SYint", sy);
                                                command.Parameters.AddWithValue("@SY", schoolYear);
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
                                else
                                {
                                    MessageBox.Show("Account is not authorize to add new students");
                                    emptyTextbox();
                                    emptyComboBox();
                                    emptyValues();
                                }
                            }
                        }
                    }
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
                            using (SqlCeCommand cmd = new SqlCeCommand("UPDATE StudentInfo SET StudentNo = @StudNo, LastName = @LName, firstName = @FName, MiddleName = @MName, ResidenceStatus = @residence where studentNo = @tempStudNo;", conn))
                            {
                                cmd.Parameters.AddWithValue("@tempStudNo", tempStudNo);
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
                            break;

                        case MessageBoxResult.No: break;
                    }
                }
            }
            conn.Close();

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
            if (string.IsNullOrEmpty(txtStudNo.Text))
            {
                MessageBox.Show("Please input student number in the field before editing!");
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("Select COUNT(1) from StudentInfo where studentNo = @studentNo;", conn))
                {
                    cmd.Parameters.AddWithValue("@studentNo", txtStudNo.Text);
                    int studCount = (int)cmd.ExecuteScalar();
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
        private void btnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (txtStudNo.Text == "" || txtLName.Text == "" || txtFName.Text == "" || cmbResidence.Text == "")
            {
                MessageBox.Show("Please fill up the missing fields!");
            }
            else
            {
                if (value == 2)
                {
                    string sMessageBoxText = "Do you want to delete this record?";
                    string sCaption = "Delete Record";
                    MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                    MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                    switch (dr)
                    {
                        case MessageBoxResult.Yes:
                            SqlCeConnection conn = DBUtils.GetDBConnection();
                            conn.Open();
                            using (SqlCeCommand cnt = new SqlCeCommand("Select COUNT(1) from StudentInfoArchive where studentNo = @studentNo;", conn))
                            {
                                cnt.Parameters.AddWithValue("@studentNo", txtStudNo.Text);
                                int studCount;
                                studCount = (int)cnt.ExecuteScalar();
                                if (studCount > 0)
                                {
                                    int studNo = Convert.ToInt32(txtStudNo.Text);
                                    MessageBox.Show("Student " + txtStudNo.Text + " has an record in the archive! (Student has been deleted before)");
                                    using (SqlCeCommand command = new SqlCeCommand("Delete from StudentInfo where studentNo= @studentNo;", conn))
                                    {
                                        command.Parameters.AddWithValue("@studentNo", txtStudNo.Text);
                                        command.ExecuteNonQuery();
                                    }
                                    using (SqlCeCommand cmd = new SqlCeCommand("Insert Into RecordDetailsArchive(studentNo, ViolationCode, dateCommitted, Period, SYint, SY, remarks) select StudentNo, ViolationCode, dateCommitted, Period, SYint, SY, remarks from RecordDetails where studentNo = @studentNo", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@studentNo", studNo);
                                        cmd.ExecuteNonQuery();
                                        using (SqlCeCommand cmd1 = new SqlCeCommand("Delete from RecordDetails where studentNo= @studentNo", conn))
                                        {
                                            cmd1.Parameters.AddWithValue("@studentNo", txtStudNo.Text);
                                            cmd1.ExecuteNonQuery();
                                        }
                                    }

                                    Log = LogManager.GetLogger("studentAlreadyArchived");
                                    Log.Info("Student: " + txtStudNo.Text + " has an existing record on the archive database");

                                    emptyTextbox();
                                    emptyComboBox();
                                    disableFields();
                                }
                                else
                                {
                                    using (SqlCeCommand cmd = new SqlCeCommand("Insert Into StudentInfoArchive(studentNo, LastName, firstName, MiddleName, ResidenceStatus, CounterDept, CounterAcad, CounterProbi, CounterInsti) select studentNo, LastName, firstName, MiddleName, ResidenceStatus, CounterDept, CounterAcad, CounterProbi, CounterInsti from StudentInfo where studentNo = @studentNo;", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@studentNo", txtStudNo.Text);
                                        cmd.ExecuteNonQuery();
                                        using (SqlCeCommand command = new SqlCeCommand("Delete from StudentInfo where studentNo= @studentNo;", conn))
                                        {
                                            command.Parameters.AddWithValue("@studentNo", txtStudNo.Text);
                                            int count = command.ExecuteNonQuery();
                                            if (count == 1)
                                            {
                                                MessageBox.Show("User record has been deleted!");
                                                Log = LogManager.GetLogger("archiveStudent");
                                                Log.Info(": Archived student no:" + txtStudNo.Text);
                                            }
                                            else
                                            {
                                                MessageBox.Show("User does not exist!");
                                                return;
                                            }
                                        }
                                    }
                                    int studNo = Convert.ToInt32(txtStudNo.Text);
                                    using (SqlCeCommand cmd = new SqlCeCommand("Insert Into RecordDetailsArchive(studentNo, ViolationCode, dateCommitted, Period, SYint, SY, remarks) select StudentNo, ViolationCode, dateCommitted, Period, SYint, SY, remarks from RecordDetails where studentNo = @studentNo", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@studentNo", studNo);
                                        cmd.ExecuteNonQuery();
                                        using (SqlCeCommand cmd1 = new SqlCeCommand("Delete from RecordDetails where studentNo= @studentNo", conn))
                                        {
                                            cmd1.Parameters.AddWithValue("@studentNo", txtStudNo.Text);
                                            cmd1.ExecuteNonQuery();
                                        }
                                    }

                                    Log = LogManager.GetLogger("archiveStudent");
                                    Log.Info(": Archiving student no:" + txtStudNo.Text + " records..");

                                    txtStudNo.IsReadOnly = false;
                                    emptyTextbox();
                                    emptyComboBox();
                                    disableFields();
                                    conn.Close();
                                }
                            }
                            conn.Close();
                            break;

                        case MessageBoxResult.No:
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Please click Edit button first to be able to delete!");
                }
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
            txtSpecify.Text = "";
            txtViolationDesc.Text = "";
            txtRemarks.Text = "";
        }
        private void emptyValues()
        {
            countAcademic = 0;
            countDepart = 0;
            countInsti = 0;
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

            btnViolateAdd.IsEnabled = false;
            btnViolateAdd.Visibility = Visibility.Hidden;

            lblViolationName.Visibility = Visibility.Hidden;
            cmbViolationName.Visibility = Visibility.Hidden;
        }
        private void enableFields()
        {
            txtStudNo.IsReadOnly = false;
            txtLName.IsReadOnly = false;
            txtFName.IsReadOnly = false;
            txtMName.IsReadOnly = false;
            txtRemarks.IsReadOnly = false;

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
                            cmbViolationName.Items.Clear();
                            cmbViolationName.Items.Add("Others (Please specify)");
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
                using (SqlCeCommand sql = new SqlCeCommand("Select ViolationType, ViolationName from ViolationDetails where ViolationType ='Institutional'", conn))
                {
                    using (DbDataReader reader = sql.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            cmbViolationName.Items.Clear();
                            cmbViolationName.Items.Add("Others (Please specify)");
                            while (reader.Read())
                            {
                                string ViolationName = reader["ViolationName"].ToString();
                                cmbViolationName.Items.Add(ViolationName);
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
                            cmbViolationName.Items.Add("Others (Please specify)");
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
        private void checkAccountLevel()
        {
            if (userLevel == 1)
            {
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
            else if (userLevel == 2)
            {
                btnEdit.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
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
    }
}
