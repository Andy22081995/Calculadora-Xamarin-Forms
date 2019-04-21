using SQLite;

namespace Calculator.Controller
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
