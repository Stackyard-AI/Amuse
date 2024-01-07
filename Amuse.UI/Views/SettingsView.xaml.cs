﻿using Amuse.UI.Commands;
using Amuse.UI.Models;
using Microsoft.Extensions.Logging;
using OnnxStack.Core.Config;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Amuse.UI.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl, INavigatable, INotifyPropertyChanged
    {
        private readonly ILogger<SettingsView> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsView"/> class.
        /// </summary>
        public SettingsView()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
                _logger = App.GetService<ILogger<SettingsView>>();

            SaveCommand = new AsyncRelayCommand(Save);
            InitializeComponent();
        }

        public AsyncRelayCommand SaveCommand { get; }

        public AmuseSettings UISettings
        {
            get { return (AmuseSettings)GetValue(UISettingsProperty); }
            set { SetValue(UISettingsProperty, value); }
        }
        public static readonly DependencyProperty UISettingsProperty =
            DependencyProperty.Register("UISettings", typeof(AmuseSettings), typeof(SettingsView));



        public Task NavigateAsync(ImageResult imageResult)
        {
            throw new NotImplementedException();
        }

        public Task NavigateAsync(VideoResultModel videoResult)
        {
            throw new NotImplementedException();
        }


        private Task Save()
        {
            try
            {
                ConfigManager.SaveConfiguration(UISettings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving configuration file, {ex.Message}");
            }
            return Task.CompletedTask;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion
    }


}
