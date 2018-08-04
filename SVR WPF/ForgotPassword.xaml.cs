using System;
using System.Windows;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Windows.Input;

namespace SVR_WPF
{
    /// <summary>
    /// Interaction logic for ForgotPassword.xaml
    /// </summary>
    public partial class ForgotPassword : Window
    {
        public string userID;
        public string question;
        public string answer;

        public ForgotPassword(string userID)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            txtUsername.Text = userID;
            if (!string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtUsername.IsReadOnly = true;
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("Select * from Accounts where userID = @userID", conn))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);
                    SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable);
                    int ordinal = 0;
                    if (reader.Read())
                    {
                        ordinal = reader.GetOrdinal("securityQuestion");
                        question = reader.GetString(ordinal);
                        ordinal = reader.GetOrdinal("securityAnswer");
                        answer = reader.GetString(ordinal);
                        lblQuestion.Content = question;
                        lblQuestion.Visibility = Visibility.Visible;
                        txtAnswer.Visibility = Visibility.Visible;
                        txtAnswer.Focus();
                    }
                    reader.Close();
                }
            }
        }

        private void ResetPassword_OnClick(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text == "")
            {
                MessageBox.Show("Please input username");
                txtUsername.Focus();
            }
            else if (txtPassword.Password == "" || txtConfirmPassword.Password == "")
            {
                MessageBox.Show("Please fill up the fields");
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                userID = txtUsername.Text;
                using (SqlCeCommand cmd = new SqlCeCommand("Select * from Accounts where userID = @userID", conn))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);
                    SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable);
                    if (reader.Read())
                    {
                        if (txtAnswer.Text.Equals(answer))
                        {
                            if (txtPassword.Password.Equals(txtConfirmPassword.Password))
                            {
                                using (SqlCeCommand cmd1 = new SqlCeCommand("UPDATE Accounts SET Password = @password, loginAttempts = @loginAttempts WHERE userID = @userID", conn))
                                {
                                    cmd1.Parameters.AddWithValue("@userID", txtUsername.Text);
                                    cmd1.Parameters.AddWithValue("@password", txtPassword.Password);
                                    cmd1.Parameters.AddWithValue("@loginAttempts", 0);
                                    cmd1.ExecuteNonQuery();
                                    MessageBox.Show("Password has been changed.");
                                    lockAndClearFields();

                                }
                            }
                            else
                            {
                                MessageBox.Show("New password and confirmation password do not match.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Security answer is wrong");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Account does not exist.");
                        return;
                    }
                }

            }
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                userID = txtUsername.Text;
                using (SqlCeCommand cmd = new SqlCeCommand("Select * from Accounts where userID = @userID", conn))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);
                    SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable);
                    int ordinal = 0;

                    if (reader.Read())
                    {
                        ordinal = reader.GetOrdinal("securityQuestion");
                        question = reader.GetString(ordinal);
                        ordinal = reader.GetOrdinal("securityAnswer");
                        answer = reader.GetString(ordinal);
                        lblQuestion.Content = question;
                        unlockFields();
                        txtAnswer.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Account does not exist.");
                        return;
                    }
                    reader.Close();
                }
            }
        }

        private void unlockFields()
        {
            lblQuestion.Visibility = Visibility.Visible;
            txtAnswer.Visibility = Visibility.Visible;
            txtUsername.IsReadOnly = true;
        }
        private void lockAndClearFields()
        {
            lblQuestion.Visibility = Visibility.Hidden;
            txtAnswer.Visibility = Visibility.Hidden;
            txtUsername.IsReadOnly = false;
            txtUsername.Text = "";
            txtPassword.Password = "";
            txtConfirmPassword.Password = "";
            lblQuestion.Content = "Question";
            txtAnswer.Text = "";
        }

    }
}
