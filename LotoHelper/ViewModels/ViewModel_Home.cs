using LotoHelper.Models.DataControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotoHelper.ViewModels
{
    public class ViewModel_Home
    {
        #region Fields
        private RelayCommand _myCommand;
        private LotoResults _lotoResults;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public ViewModel_Home()
        {
            _lotoResults = new LotoResults();
        }
        #endregion

        #region Methods - Private
        #endregion

        #region Methods - Public
        public RelayCommand MyCommand => _myCommand ?? (_myCommand = new RelayCommand(o => { _lotoResults.GetArchiveResults(); }, o => true));
        #endregion
    }
}