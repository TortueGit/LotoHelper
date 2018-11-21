using Core.Utils.Database;
using Core.Utils.Date;
using Core.Utils.Web;
using LotoHelper.DataTypes.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotoHelper.Models.DataControllers
{
    /// <summary>
    /// Class that allowed to get the euro-millions archive results from the website www.tirage-euromillions
    /// http://www.tirage-euromillions.net/euromillions/annees/annee-[2004 to currentYear]/
    /// The results are show in web page as a table format.
    /// </summary>
    /// <remarks>The euromillions draw are made all the friday since 2004 and all the tuesday and friday since May 2011.</remarks>
    public class LotoResults
    {
        #region Fields
        private const string DBNAME = "LotoHelper";

        private WebAccess _webAccess;
        private Tirage _tirage;

        private int _startingYear = 2004;   // The euromillions results start at the year 2004.
        private int _startingTuesdayYear = 2011;    // The euromillions tuesday draw start at the year 2011.
        private int _endYear = DateTime.Now.Year;

        private string _webSiteUrl = "http://www.tirage-euromillions.net/euromillions/annees/annee-";
        private string _resultTableIdTag = @"""tiragesAnnee""";
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public LotoResults()
        {
            _webAccess = new WebAccess();
        }
        #endregion

        #region Methods - Private
        // TODO: Method that will save the data into the MongoDb.
        private void SaveTirageInMongoDb(List<Tirage> _tirages)
        {
            if (string.IsNullOrEmpty(MongoAccess.Instance.DbName) ||
                !MongoAccess.Instance.DbName.Equals(DBNAME))
                MongoAccess.Instance.DbName = DBNAME;

            MongoAccess.Instance.InsertListInMongoDb<Tirage>(_tirages);
        }

        /// <summary>
        /// Get the source code of the web page given the results of [year]
        /// </summary>
        /// <param name="_year">Results for the [year]</param>
        /// <returns>source code as string</returns>
        private string GetSourceCodeForYear(int _year)
        {
            string sourceCode = string.Empty;

            sourceCode = _webAccess.GetCodeSourceOfPage(string.Concat(_webSiteUrl, _year.ToString()));

            return sourceCode;
        }

        private List<Tirage> GetTiragesFromSourceCode(string _sourceCode, int _year)
        {
            List<Tirage> tirages = new List<Tirage>();
            Tirage tirage;
            List<DateTime> allTuesday = null;
            List<DateTime> allFriday = DateTimeServices.GetAllDatesForDayOfWeekInYear(DayOfWeek.Friday, _year);
            // Since 2011 we need to look for tuesday dates too.
            if (_year >= this._startingTuesdayYear)
            {
                allTuesday = DateTimeServices.GetAllDatesForDayOfWeekInYear(DayOfWeek.Tuesday, _year);
                // Only since May 2011.
                if (_year == this._startingTuesdayYear)
                {
                    allTuesday.RemoveAll(x => x.Month < 5);
                }
            }

            string resultsTableContent = HtmlParsingServices.GetTableContentFromCode(_sourceCode, this._resultTableIdTag);
            
            if (allTuesday != null)
            {
                foreach (var d in allTuesday)
                {
                    tirage = this.GetTirageOfDateInTableContent(resultsTableContent, d);
                    if (tirage != null)
                    {
                        tirages.Add(tirage);
                    }
                }
            }

            foreach (var d in allFriday)
            {
                tirage = this.GetTirageOfDateInTableContent(resultsTableContent, d);
                if (tirage != null)
                {
                    tirages.Add(tirage);
                }
            }

            return tirages;
        }

        /// <summary>
        /// Return the object Tirage for a given date in the given table content
        /// </summary>
        /// <param name="_tableContent">The table content to parse</param>
        /// <param name="_dateTirage">The date of the Tirage to find</param>
        /// <returns>The Tirage of the date or null</returns>
        private Tirage GetTirageOfDateInTableContent(string _tableContent, DateTime _dateTirage)
        {
            Dictionary<int, string> divContent = new Dictionary<int, string>();
            Tirage tirage = null;
            int idDateTirage = _tableContent.IndexOf(_dateTirage.ToString("dd/MM/yyyy"));

            if (idDateTirage >= 0)
            {                
                tirage = new Tirage();
                divContent = this.GetDivContent(_tableContent, idDateTirage);
                tirage.Numero1 = int.Parse(divContent.FirstOrDefault().Value);
                divContent = this.GetDivContent(_tableContent, divContent.FirstOrDefault().Key);
                tirage.Numero2 = int.Parse(divContent.FirstOrDefault().Value);
                divContent = this.GetDivContent(_tableContent, divContent.FirstOrDefault().Key);
                tirage.Numero3 = int.Parse(divContent.FirstOrDefault().Value);
                divContent = this.GetDivContent(_tableContent, divContent.FirstOrDefault().Key);
                tirage.Numero4 = int.Parse(divContent.FirstOrDefault().Value);
                divContent = this.GetDivContent(_tableContent, divContent.FirstOrDefault().Key);
                tirage.Numero5 = int.Parse(divContent.FirstOrDefault().Value);
                divContent = this.GetDivContent(_tableContent, divContent.FirstOrDefault().Key);
                tirage.Etoile1 = int.Parse(divContent.FirstOrDefault().Value);
                divContent = this.GetDivContent(_tableContent, divContent.FirstOrDefault().Key);
                tirage.Etoile2 = int.Parse(divContent.FirstOrDefault().Value);
                tirage.Date = _dateTirage;
            }

            return tirage;
        }

        // TODO: Put this into core if possible.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_tableContent">The table content where find the div</param>
        /// <param name="_startingIndex">The index to start for parsing</param>
        /// <param name="_divClass">The class of the div to find</param>
        /// <returns>The content of the div and the index of the end div tag</returns>
        private Dictionary<int, string> GetDivContent(string _tableContent, int _startingIndex, string _divClass = null)
        {
            Dictionary<int, string> divContent = new Dictionary<int, string>();
            int idEndTag = 0;
            int idStartDivTag = 0;
            int idEndDivTag = 0;

            idStartDivTag = _tableContent.IndexOf("<div", _startingIndex);
            idEndTag = _tableContent.IndexOf('>', idStartDivTag) + 1;
            idEndDivTag = _tableContent.IndexOf("</div>", idEndTag);

            divContent.Add(idEndDivTag, _tableContent.Substring(idEndTag, (idEndDivTag - idEndTag)));

            return divContent;
        }
        #endregion

        #region Methods - Public
        /// <summary>
        /// Get the results from _startingYear (2004) to _endYear (currentYear) from website http://www.tirage-euromillions.net.
        /// </summary>
        /// <returns>return all the results as a List of Tirage</returns>
        public List<Tirage> GetArchiveResults()
        {
            List<Tirage> tirages = new List<Tirage>();

            for (int i = _startingYear; i <= _endYear; i++)
            {
                string sourceCode = this.GetSourceCodeForYear(i);
                tirages.AddRange(this.GetTiragesFromSourceCode(sourceCode, i));
            }

            this.SaveTirageInMongoDb(tirages);

            return tirages;
        }
        #endregion
    }
}
