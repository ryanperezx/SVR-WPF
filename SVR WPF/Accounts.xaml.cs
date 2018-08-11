using System;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Windows;
using System.Data.SqlServerCe;
using System.Windows.Input;
using NLog;


namespace SVR_WPF
{
    /// <summary>
    /// Interaction logic for Accounts.xaml
    /// </summary>
    public partial class Accounts : Page
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public Accounts()
        {
            InitializeComponent();
            time.Content = DateTime.Now.ToString("G");
            startTimer();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text == "" || txtFirstName.Text == "" || txtLastName.Text == "")
            {
                MessageBox.Show("Please fill up the missing fields!");
            }
            else
            {
                string sMessageBoxText = "Do you want to delete this account?";
                string sCaption = "Delete Account";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlCeConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        SqlCeCommand command = new SqlCeCommand("Delete from Accounts where userID='" + txtUsername.Text + "'", conn);
                        int count = command.ExecuteNonQuery();
                        if (count == 1)
                        {
                            MessageBox.Show("User has been deleted!");
                        }
                        else
                        {
                            MessageBox.Show("User does not exist!");
                            return;
                        }

                        Log = LogManager.GetLogger("DeleteAccount");
                        Log.Info("Account: " + txtUsername.Text + " has been deleted from the database!");

                        emptyTextbox();
                        emptyComboBox();
                        conn.Close();
                        conn.Dispose();
                        break;

                    case MessageBoxResult.No: break;
                }
            }
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            string gName = txtFirstName.Text;
            string mName = txtMiddleName.Text;
            string lName = txtLastName.Text;
            string un = txtUsername.Text;
            string pw = txtPassword.Password;
            string cp = txtConfirm.Password;
            string sq = cmbQuestion.Text;
            string sa = txtAnswer.Text;
            int userLevel = 0;

            if (cmbUserLevel.Text.Equals("Administrator"))
                userLevel = 1;
            else if (cmbUserLevel.Text.Equals("Student Assistant"))
                userLevel = 2;

            int loginAttempts = 0;

            if (String.IsNullOrEmpty(txtLastName.Text) || String.IsNullOrEmpty(txtFirstName.Text) || String.IsNullOrEmpty(txtMiddleName.Text) || String.IsNullOrEmpty(txtUsername.Text)
                || String.IsNullOrEmpty(txtPassword.Password) || String.IsNullOrEmpty(txtConfirm.Password) || String.IsNullOrEmpty(cmbQuestion.Text) || String.IsNullOrEmpty(txtAnswer.Text) || String.IsNullOrEmpty(cmbUserLevel.Text))
            {
                MessageBox.Show("Please fill up all the missing fields");
                return;
            }

            if (txtPassword.Password.Equals(txtConfirm.Password))
            {
                using (SqlCeCommand cmd = new SqlCeCommand("INSERT INTO Accounts VALUES (@userID, @Password, @LastName, @GivenName, @MiddleName, @securityQuestion, @securityAnswer, @userLevel, @loginAttempts)", conn))
                {
                    cmd.Parameters.AddWithValue("@userID", un);
                    cmd.Parameters.AddWithValue("@Password", pw);
                    cmd.Parameters.AddWithValue("@LastName", lName);
                    cmd.Parameters.AddWithValue("@GivenName", gName);
                    cmd.Parameters.AddWithValue("@MiddleName", mName);
                    cmd.Parameters.AddWithValue("@securityQuestion", sq);
                    cmd.Parameters.AddWithValue("@securityAnswer", sa);
                    cmd.Parameters.AddWithValue("@userLevel", userLevel);
                    cmd.Parameters.AddWithValue("@loginAttempts", loginAttempts);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Registered successfully");

                        Log = LogManager.GetLogger("registerAccount");
                        Log.Info("Account: " + txtUsername.Text + " has been added to database!");
                    }
                    catch (SqlException)
                    {
                        MessageBox.Show("Error: A user with the same User ID already exists.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Your password and confirmation password do not match.");
            }
        }
        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtUsername.Text == "")
                {
                    MessageBox.Show("Please input username!");
                    txtUsername.Text = "";
                    emptyComboBox();
                    emptyTextbox();
                }
                else
                {
                    SqlCeConnection conn = DBUtils.GetDBConnection();
                    conn.Open();
                    using (SqlCeCommand cmd = new SqlCeCommand("Select COUNT(1) from Accounts where userID ='" + txtUsername.Text + "'", conn))
                    {
                        int userCount;
                        userCount = (int)cmd.ExecuteScalar();
                        if (userCount > 0)
                        {
                            string username = txtUsername.Text;
                            using (SqlCeCommand cmd1 = new SqlCeCommand("Select * from Accounts where userID = @username", conn))
                            {
                                cmd1.Parameters.AddWithValue("@username", username);
                                cmd1.Connection = conn;
                                using (SqlCeDataReader reader = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                                {
                                    if (reader.HasRows)
                                    {
                                        reader.Read();
                                        //0
                                        string user = reader.GetValue(1).ToString();
                                        //1
                                        int gNameIndex = reader.GetOrdinal("givenName");
                                        string fName = Convert.ToString(reader.GetValue(gNameIndex));
                                        //2
                                        int mNameIndex = reader.GetOrdinal("middleName");
                                        string mName = Convert.ToString(reader.GetValue(mNameIndex));
                                        //3
                                        int lNameIndex = reader.GetOrdinal("lastName");
                                        string lName = Convert.ToString(reader.GetValue(lNameIndex));
                                        //4
                                        int securityIndex = reader.GetOrdinal("securityQuestion");
                                        string securityQuestion = Convert.ToString(reader.GetValue(securityIndex));
                                        //5
                                        int userIndex = reader.GetOrdinal("userLevel");
                                        int userLevel = Convert.ToInt32(reader.GetValue(userIndex));
                                        string userLvl = "";
                                        switch (userLevel)
                                        {
                                            case 1:
                                                userLvl = "Administrator";
                                                break;
                                            case 2:
                                                userLvl = "Student Assistant";
                                                break;
                                        }
                                        txtUsername.Text = user;
                                        txtFirstName.Text = fName;
                                        txtMiddleName.Text = mName;
                                        txtLastName.Text = lName;
                                        cmbQuestion.Text = securityQuestion;
                                        cmbUserLevel.Text = userLvl;
                                    }

                                    else
                                    {
                                        MessageBox.Show("There is no record of that user!");
                                        emptyComboBox();
                                        emptyTextbox();
                                    }
                                }
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }

        private void emptyComboBox()
        {
            cmbQuestion.SelectedIndex = -1;
            cmbUserLevel.SelectedIndex = -1;
        }
        private void emptyTextbox()
        {
            txtUsername.Text = "";
            txtPassword.Password = "";
            txtConfirm.Password = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtMiddleName.Text = "";
            txtAnswer.Text = "";
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
