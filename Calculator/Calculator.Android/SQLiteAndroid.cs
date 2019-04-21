using Calculator.Controller;
using Calculator.Droid;
using SQLite;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteAndroid))]
namespace Calculator.Droid
{
    public class SQLiteAndroid : ISQLite
    {
        public SQLiteAndroid()
        {

        }

        public SQLiteConnection GetConnection()
        {
            var filename = "calc.db3";
            var documentspath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            var path = Path.Combine(documentspath, filename);
            var con = new SQLiteConnection(path);

            return con;
        }
    }
}