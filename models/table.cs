//
// Summary:
//   The table of the database
public class Table
{
    public string Name { get; set; }
    public List<Field> Fields { get; set; }

    public Table()
    {
        Name = "";
        Fields = new List<Field>();
    }

    public override string ToString()
    {
        return Name;
    }
}
