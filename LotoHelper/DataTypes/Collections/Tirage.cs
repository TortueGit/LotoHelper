using Core.Utils.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotoHelper.DataTypes.Collections
{
    public class Tirage
    {
        #region Fields
        private int _numero1;
        private int _numero2;
        private int _numero3;
        private int _numero4;
        private int _numero5;
        private int _etoile1;
        private int _etoile2;
        private DateTime _date;
        #endregion

        #region Properties
        public int Numero1 { get => _numero1; set => _numero1 = value; }
        public int Numero2 { get => _numero2; set => _numero2 = value; }
        public int Numero3 { get => _numero3; set => _numero3 = value; }
        public int Numero4 { get => _numero4; set => _numero4 = value; }
        public int Numero5 { get => _numero5; set => _numero5 = value; }
        public int Etoile1 { get => _etoile1; set => _etoile1 = value; }
        public int Etoile2 { get => _etoile2; set => _etoile2 = value; }
        public DateTime Date { get => _date; set => _date = value; }
        #endregion

        #region Constructors
        public Tirage()
        {
        }

        public Tirage(int[] tirage)
        {
            _numero1 = tirage[0];
            _numero2 = tirage[1];
            _numero3 = tirage[2];
            _numero4 = tirage[3];
            _numero5 = tirage[4];
            _etoile1 = tirage[5];
            _etoile2 = tirage[6];
        }

        public Tirage(int num1, int num2, int num3, int num4, int num5, int eto1, int eto2)
        {
            _numero1 = num1;
            _numero2 = num2;
            _numero3 = num3;
            _numero4 = num4;
            _numero5 = num5;
            _etoile1 = eto1;
            _etoile2 = eto2;
        }
        #endregion

        #region Methods - Private
        #endregion

        #region Methods - Public
        public void InsertInMongoDb()
        {
            MongoAccess.Instance.InsertElementInMongoDb<Tirage>(this);
        }
        #endregion
    }
}
