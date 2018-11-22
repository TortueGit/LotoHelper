using Core.ViewModels;
using LotoHelper.DataTypes.Collections;
using LotoHelper.Models.DataControllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotoHelper.ViewModels
{
    public class ViewModel_Home : ViewModelBase
    {
        #region Fields
        private int _currentProgress;
        private RelayCommand _getResultsCommand;

        private readonly BackgroundWorker _workerGetResults;

        private LotoResults _lotoResults;
        #endregion

        #region Properties
        public int CurrentProgress
        {
            get => this._currentProgress;
            private set
            {
                this._currentProgress = value;
                this.OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public ViewModel_Home()
        {
            this._lotoResults = new LotoResults();

            this._workerGetResults = new BackgroundWorker();
            this._workerGetResults.WorkerReportsProgress = true;
            this._workerGetResults.DoWork += _workerGetResults_DoWork;
            this._workerGetResults.ProgressChanged += _workerGetResults_ProgressChanged;
        }
        #endregion

        #region Methods - Private
        private void _workerGetResults_DoWork(object sender, DoWorkEventArgs e)
        {
            int i = 0;
            int percent = 0;
            // TODO: Implement a method to update the progress in 5s from 0 to 20%
            this._workerGetResults.ReportProgress(0);
            List<Tirage> tirages = this._lotoResults.GetArchiveResults();
            this._workerGetResults.ReportProgress(20);

            foreach (var t in tirages)
            {
                i++;
                this._lotoResults.SaveTirageInMongoDb(t);
                percent = (int)(i / tirages.Count * 100);
                this._workerGetResults.ReportProgress(percent / (20 + percent) * 100);
            }
        }

        private void _workerGetResults_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.CurrentProgress = e.ProgressPercentage;
        }
        #endregion

        #region Methods - Public
        public RelayCommand GetResultsCommand => this._getResultsCommand ?? (this._getResultsCommand = new RelayCommand(o => { this._workerGetResults.RunWorkerAsync(); }, o => !this._workerGetResults.IsBusy));
        #endregion
    }
}