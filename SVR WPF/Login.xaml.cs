﻿using System;
using System.Windows;
using MaterialDesignThemes;
using MaterialDesignColors;
using NLog;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Media;
namespace SVR_WPF
{


    public partial class MainWindow : Window
    {
        string user, pass;
        bool close;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        //main functions
        private void btnClick(object sender, RoutedEventArgs e)
        {
            user = txtUser.Text;
            pass = txtPassword.Password;
            if (txtUser.Text == "" && txtPassword.Password == "")
            {
                txtUser.Focus();
                MessageBox.Show("No input!");
            }
            else if (txtPassword.Password == "")
            {
                MessageBox.Show("No Password input");
                txtPassword.Focus();
            }
            else if (txtUser.Text == "")
            {
                MessageBox.Show("No Username input!");
                txtUser.Focus();
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                Nullable<int> loginAttempts;
                int userLevel;

                using (SqlCeCommand cmd = new SqlCeCommand("Select loginAttempts FROM Accounts WHERE userID = @userID", conn))
                {
                    cmd.Parameters.AddWithValue("@userID", user);
                    loginAttempts = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (loginAttempts < 5)
                {

                    string un = txtUser.Text;
                    string pw = txtPassword.Password;

                    using (SqlCeCommand cmd = new SqlCeCommand("Select * from Accounts where userID = @userID AND Password = @password", conn))
                    {
                        cmd.Parameters.AddWithValue("@userID", un);
                        cmd.Parameters.AddWithValue("@password", pw);
                        SqlCeDataReader dr = cmd.ExecuteResultSet(ResultSetOptions.Scrollable);

                        if (dr.Read())
                        {
                            string lName, fName, mName;
                            lName = dr.GetString(2);
                            fName = dr.GetString(3);
                            mName = dr.GetString(4);

                            SqlCeCommand cmd2 = new SqlCeCommand("UPDATE Accounts SET loginAttempts = 0", conn);
                            int ordinal = 0;
                            ordinal = dr.GetOrdinal("userLevel");
                            userLevel = dr.GetInt32(ordinal);
                            dr.Close();
                            dr.Dispose();
                            cmd2.ExecuteNonQuery();
                            MessageBox.Show("Login Successful");
                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"Logs\ActivityLogs.txt", true))
                            {
                                string time = DateTime.Now.ToString();
                                file.WriteLine(time + " Account Name: " + lName + ", " + fName + " " + mName + " has logged in.");
                            }
                        }

                        else
                        {
                            using (SqlCeCommand cmd2 = new SqlCeCommand("Select userID from Accounts where userID = @userID", conn))
                            {
                                cmd2.Parameters.AddWithValue("@userID", un);
                                dr.Close();
                                dr.Dispose();
                                dr = cmd2.ExecuteReader();
                                int ordinal = 0;
                                string value = "";

                                if (dr.Read())
                                {
                                    ordinal = dr.GetOrdinal("userID");
                                    value = dr.GetString(ordinal);
                                    if (value.Equals(un))
                                    {
                                        SqlCeCommand cmd3 = new SqlCeCommand("UPDATE Accounts SET loginAttempts = loginAttempts + 1 WHERE userID = @un", conn);
                                        cmd3.Parameters.AddWithValue("@un", un);
                                        dr.Close();
                                        dr.Dispose();
                                        cmd3.ExecuteNonQuery();
                                        cmd3.Dispose();
                                    }
                                }
                            }
                            MessageBox.Show("User ID or Password is invalid");
                            return;
                        }
                    }
                    Hide();
                    new Main(userLevel, un).ShowDialog();
                    txtPassword.Password = "";
                    txtUser.Text = "";
                    ShowDialog();

                }
                else
                {
                    user = txtUser.Text;
                    string sMessageBoxText = "Due to multiple login attempts, your account has been locked. \nPlease unlock it to continue.";
                    string sCaption = "Account Recovery";
                    MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                    MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                    switch (dr)
                    {
                        case MessageBoxResult.Yes:
                            SqlCeConnection cnn = DBUtils.GetDBConnection();
                            cnn.Open();
                            string question = "", answer = "";
                            int ordinal = 0;


                            using (SqlCeCommand cmd = new SqlCeCommand("Select * from Accounts where userID = @userID", cnn))
                            {
                                cmd.Parameters.AddWithValue("@userID", user);
                                SqlCeDataReader reader = cmd.ExecuteReader();

                                if (reader.Read())
                                {
                                    ordinal = reader.GetOrdinal("securityQuestion");
                                    question = reader.GetString(ordinal);
                                    ordinal = reader.GetOrdinal("securityAnswer");
                                    answer = reader.GetString(ordinal);
                                }
                                reader.Close();
                            }
                            Account_Recovery ar = new Account_Recovery(question);
                            if (ar.ShowDialog() == true)
                            {
                                string input = ar.Answer;
                                if (input.Equals(answer))
                                {
                                    SqlCeCommand cmd2 = new SqlCeCommand("UPDATE Accounts SET loginAttempts = 0 WHERE userID = @un", conn);
                                    cmd2.Parameters.AddWithValue("@un", user);
                                    cmd2.ExecuteNonQuery();
                                    cmd2.Dispose();

                                    MessageBoxResult cp = MessageBox.Show("Account has been unlocked. Would you like to change password ?", "Change Password", btnMessageBox, icnMessageBox);
                                    switch (cp)
                                    {
                                        case MessageBoxResult.Yes:
                                            Hide();
                                            new ForgotPassword(user).ShowDialog();
                                            ShowDialog();
                                            break;
                                        case MessageBoxResult.No:
                                            break;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Your answer is incorrect, please try again.");
                                }
                            }
                            break;

                        case MessageBoxResult.No: break;
                    }
                }
            }
        }

        private void txtUser_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtUser.Text == "Username")
            {
                txtUser.Foreground = System.Windows.Media.Brushes.Black;
                txtUser.Text = "";
            }
        }
        private void txtUser_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtUser.Text.Length == 0)
            {
                txtUser.Foreground = System.Windows.Media.Brushes.DimGray;
                txtUser.Text = "Username";
            }
        }
        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password.Length == 0)
            {
                txtPassword.Foreground = System.Windows.Media.Brushes.DimGray;
                txtPassword.Password = "Password";
            }
        }
        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password == "Password")
            {
                txtPassword.Foreground = System.Windows.Media.Brushes.Black;
                txtPassword.Password = "";
            }
        }


        private void lblForgot_OnClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            txtUser.Text = "";
            txtPassword.Password = "";
            user = "";
            Hide();
            new ForgotPassword(user).ShowDialog();
            ShowDialog();
        }




        //event handlers
    }
}