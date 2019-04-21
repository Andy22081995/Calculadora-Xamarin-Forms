using Calculator.Models;
using SQLite;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Calculator.Controller
{
    public class DBItemController
    {
        private static object locker = new object();
        private SQLiteConnection database;

        public DBItemController()
        {
            this.database = DependencyService.Get<ISQLite>().GetConnection();
            this.database.CreateTable<History>();
        }

        public IEnumerator<History> GetDBItems()
        {
            lock(locker)
            {
                if (this.database.Table<History>().Count() == 0)
                {
                    return null;
                }
                else
                {
                    return this.database.Table<History>().GetEnumerator();
                }
            }
        }

        public int Save(History history)
        {
            lock(locker)
            {
                return this.database.Insert(history);
            }
        }

        public int SaveOrUpdate(History history)
        {
            lock (locker)
            {
                if (history.Id != 0)
                {
                    this.database.Update(history);
                    return history.Id;
                }
                else
                {
                    return this.database.Insert(history);
                }                
            }
        }

        public int DeleteItem(int id)
        {
            lock (locker)
            {
                return this.database.Delete(id);
            }
        }

        public int DeleteAll()
        {
            lock (locker)
            {
                return this.database.DeleteAll<History>();
            }
        }
    }
}
