﻿using Amuse.UI.Commands;
using Amuse.UI.Models;
using Microsoft.Extensions.Logging;
using OnnxStack.StableDiffusion.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Amuse.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for UpdateModelSettingsDialog.xaml
    /// </summary>
    public partial class UpdateModelSettingsDialog : Window, INotifyPropertyChanged
    {
        private readonly ILogger<UpdateModelSettingsDialog> _logger;

        private List<string> _invalidOptions;
        private AmuseSettings _uiSettings;
        private UpdateModelSetViewModel _updateModelSet;
        private StableDiffusionModelSet _modelSetResult;
        private string _validationError;

        public UpdateModelSettingsDialog(AmuseSettings uiSettings, ILogger<UpdateModelSettingsDialog> logger)
        {
            _logger = logger;
            _uiSettings = uiSettings;
            WindowCloseCommand = new AsyncRelayCommand(WindowClose);
            WindowRestoreCommand = new AsyncRelayCommand(WindowRestore);
            WindowMinimizeCommand = new AsyncRelayCommand(WindowMinimize);
            WindowMaximizeCommand = new AsyncRelayCommand(WindowMaximize);
            SaveCommand = new AsyncRelayCommand(Save, CanExecuteSave);
            CancelCommand = new AsyncRelayCommand(Cancel, CanExecuteCancel);
            _invalidOptions = _uiSettings.StableDiffusionModelSets
                .Select(x => x.ModelSet.Name)
                .ToList();
            InitializeComponent();
        }

        public AmuseSettings UISettings => _uiSettings;
        public AsyncRelayCommand WindowMinimizeCommand { get; }
        public AsyncRelayCommand WindowRestoreCommand { get; }
        public AsyncRelayCommand WindowMaximizeCommand { get; }
        public AsyncRelayCommand WindowCloseCommand { get; }
        public AsyncRelayCommand SaveCommand { get; }
        public AsyncRelayCommand CancelCommand { get; }

        public UpdateModelSetViewModel UpdateModelSet
        {
            get { return _updateModelSet; }
            set { _updateModelSet = value; NotifyPropertyChanged(); }
        }

        public StableDiffusionModelSet ModelSetResult
        {
            get { return _modelSetResult; }
        }

        public string ValidationError
        {
            get { return _validationError; }
            set { _validationError = value; NotifyPropertyChanged(); }
        }


        public bool ShowDialog(StableDiffusionModelSet modelSet)
        {
            _invalidOptions.Remove(modelSet.Name);
            UpdateModelSet = UpdateModelSetViewModel.FromModelSet(modelSet);
            return ShowDialog() ?? false;
        }

        private Task Save()
        {
            _modelSetResult = UpdateModelSetViewModel.ToModelSet(_updateModelSet);
            if (_invalidOptions.Contains(_modelSetResult.Name))
            {
                ValidationError = $"Model with name '{_modelSetResult.Name}' already exists";
                return Task.CompletedTask;
            }

            foreach (var modelFile in _modelSetResult.ModelConfigurations)
            {
                modelFile.DeviceId = null;
                modelFile.ExecutionProvider = null;
                modelFile.ExecutionMode = null;
                modelFile.InterOpNumThreads = null;
                modelFile.IntraOpNumThreads = null;
            }

            DialogResult = true;
            return Task.CompletedTask;
        }

        private bool CanExecuteSave()
        {
            return true;
        }

        private Task Cancel()
        {
            _modelSetResult = null;
            UpdateModelSet = null;
            DialogResult = false;
            return Task.CompletedTask;
        }

        private bool CanExecuteCancel()
        {
            return true;
        }

        #region BaseWindow

        private Task WindowClose()
        {
            Close();
            return Task.CompletedTask;
        }

        private Task WindowRestore()
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
            return Task.CompletedTask;
        }

        private Task WindowMinimize()
        {
            WindowState = WindowState.Minimized;
            return Task.CompletedTask;
        }

        private Task WindowMaximize()
        {
            WindowState = WindowState.Maximized;
            return Task.CompletedTask;
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
            InvalidateVisual();
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion
    }

}
