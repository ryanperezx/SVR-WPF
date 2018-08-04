﻿using System;
using System.Windows;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Input;
using NLog;

namespace SVR_WPF
{

    public partial class Main : Window
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        public int userLevel;
        public bool close = false;
        public string user;
        public Main(int userLevel, string user)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.userLevel = userLevel;
            lblUser.Content = user;
            checkAccountLevel();
        }
        private void tabAccounts_OnClick(object sender, MouseButtonEventArgs e)
        {
            Frame.Content = new Accounts();
        }

        private void tabSearch_OnClick(object sender, MouseButtonEventArgs e)
        {
            Frame.Content = new SearchStudent();
        }

        private void tabRecords_OnClick(object sender, MouseButtonEventArgs e)
        {
            Frame.Content = new Records();
        }

        private void ButtonPopUpLogout_Click(object sender, RoutedEventArgs e)
        {
            string sMessageBoxText = "Do you want to log out";
            string sCaption = "Logout";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (dr)
            {
                case MessageBoxResult.Yes:
                    this.DialogResult = false;
                    Close();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void checkAccountLevel()
        {
            if (userLevel == 1)
            {
                tabSearchAccount.Visibility = Visibility.Visible;
                tabAccount.Visibility = Visibility.Visible;

            }
            else if(userLevel == 2)
            {
                tabSearchAccount.Visibility = Visibility.Collapsed;
                tabAccount.Visibility = Visibility.Collapsed;
            }
        }

        private void ButtonPopUpExit_Click(object sender, RoutedEventArgs e)
        {
            string sMessageBoxText = "Do you want to Exit";
            string sCaption = "Exit";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (dr)
            {
                case MessageBoxResult.Yes:
                    this.DialogResult = true;
                    Application.Current.Shutdown();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

    }
}