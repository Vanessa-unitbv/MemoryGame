﻿using MemoryGame.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace MemoryGame.Views
{
    public partial class AboutView : Window
    {
        private AboutViewModel _viewModel;

        public AboutView()
        {
            InitializeComponent();

            _viewModel = new AboutViewModel();
            _viewModel.RequestClose += () => Close();
            DataContext = _viewModel;
        }
        private void EmailLink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "mailto:vanessa.palatka@student.unitbv.ro",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nu s-a putut deschide clientul de email: {ex.Message}",
                               "Eroare",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }
    }
}