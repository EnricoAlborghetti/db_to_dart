//
// Summary:
//   Structure of the database
public class Db
{
    public List<Table> Tables { get; set; }

    public Db()
    {
        Tables = new List<Table>();
    }
}
