using SQLite;

namespace Calculator.Models
{
    public class History
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string Expression { get; set; }

        public string Result { get; set; }

    }
}
