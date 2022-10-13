using System.Data.SqlClient;

public static partial class Connector
{
    //
    // Summary:
    //   Connect to Microsoft SQL Server and extract all the data
    //
    // Parameters:
    //   datasource:
    //     the destination (ip, domain,...)
    //   username:
    //     username to login
    //   password:
    //     password to login
    //   catalog:
    //     the database name
    // Returns:
    //     A DB object containing all tables, fields and relation
    public static Db MsSQL(string datasource, string username, string password, string catalog)
    {
        var builder = new SqlConnectionStringBuilder();

        builder.DataSource = datasource;
        builder.UserID = username;
        builder.Password = password;
        builder.InitialCatalog = catalog;

        return MsSQL(builder.ConnectionString);
    }

    //
    // Summary:
    //   Connect to Microsoft SQL Server and extract all the data
    //
    // Parameters:
    //   connectionString:
    //     The connection used to open the SQL Server database.
    // Returns:
    //     A DB object containing all tables, fields and relation
    public static Db MsSQL(string connectionString)
    {
        var db = new Db();
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var sql = "SELECT name FROM sys.tables";
            using (var command = new SqlCommand(sql, connection))
            {
                Console.WriteLine("\nQuery tables:");
                Console.WriteLine("=========================================");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var table = new Table();
                        table.Name = reader.GetString(0);
                        db.Tables.Add(table);
                        Console.WriteLine($"Found table {table.Name}");
                    }
                }
            }
            foreach (var table in db.Tables)
            {
                Console.WriteLine($"\nQuery field of {table.Name}:");
                Console.WriteLine("=========================================");
                sql =
                    $"SELECT column_name, data_Type, is_nullable FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{table.Name}'";
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var field = new Field(table, reader.GetString(0));
                            field.Type = MapType(reader.GetString(1));
                            field.Nullable = reader.GetString(2) == "YES";
                            Console.WriteLine(
                                $"Found field {field.Name} in {table.Name} of type {reader.GetString(1)}/{field.Type} {(field.Nullable ? "nullable" : "")}"
                            );
                            table.Fields.Add(field);
                        }
                    }
                }

                Console.WriteLine($"\nQuery PK of {table.Name}:");
                Console.WriteLine("=========================================");
                sql =
                    $@"SELECT  c.name
                            FROM sys.objects o
                       LEFT JOIN sys.indexes i
                            ON i.object_id = o.object_id
                       INNER JOIN sys.index_columns ic
                            ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                       INNER JOIN sys.columns c
                            ON c.object_id = ic.object_id AND c.column_id = ic.column_id
                       WHERE i.is_primary_key = 1
                            AND o.name = N'{table.Name}'";
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var fieldName = reader.GetString(0);
                            table.Fields.Where(t => t.Name == fieldName).All(t => t.PrimaryKey = true);
                        }
                    }
                }
            }

            sql =
                @"SELECT
                        tp.name 'Parent table',
                        cp.name,
                        tr.name 'Refrenced table',
                        cr.name
                    FROM 
                        sys.foreign_keys fk
                    INNER JOIN 
                        sys.tables tp ON fk.parent_object_id = tp.object_id
                    INNER JOIN 
                        sys.tables tr ON fk.referenced_object_id = tr.object_id
                    INNER JOIN 
                        sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
                    INNER JOIN 
                        sys.columns cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id
                    INNER JOIN 
                        sys.columns cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id
                    ORDER BY
                        tp.name, cp.column_id";

            using (var command = new SqlCommand(sql, connection))
            {
                Console.WriteLine("\nQuery relations:");
                Console.WriteLine("=========================================");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var childTable = reader.GetString(0);
                        var childField = reader.GetString(1);
                        var parentTable = reader.GetString(2);
                        var parentField = reader.GetString(3);

                        var relationTable = db.Tables.FirstOrDefault(t => t.Name == childTable);

                        if (relationTable != null)
                        {
                            var relationField = relationTable.Fields.FirstOrDefault(
                                t => t.Name == childField
                            );

                            if (relationField != null)
                            {
                                var pField = db.Tables
                                    .First(t => t.Name == parentTable)
                                    .Fields.First(t => t.Name == parentField);
                                pField.ChildrendField = relationField;

                                relationField.FatherField = pField;
                            }
                            else
                            {
                                Console.WriteLine(
                                    $"MISSING FIELD {childField} OF TABLE {childTable}"
                                );
                            }
                        }
                        else
                        {
                            Console.WriteLine($"MISSING TABLE {childTable}");
                        }
                        Console.WriteLine(
                            $"Found {childTable} with {childField} on {parentTable} with {parentField}"
                        );
                    }
                }
            }
        }
        return db;
    }

    private static DartType.DartFieldType MapType(string type)
    {
        switch ((type + "").ToLower())
        {
            case "bit":
                return DartType.DartFieldType.BOOL;
            case "tinyint":
            case "smallint":
            case "int":
            case "bigint":
                return DartType.DartFieldType.INT;
            case "smallmoney":
            case "money":
            case "numeric":
            case "decimal":
            case "real":
            case "float":
                return DartType.DartFieldType.DOUBLE;
            case "smalldatetime":
            case "datetime":
                return DartType.DartFieldType.DATETIME;
            case "varbinary":
            case "binary":
            case "image":
            case "varchar":
            case "char":
            case "nvarchar":
            case "nchar":
            case "text":
            case "ntext":
            case "rowversion":
            case "table":
            case "cursor":
            case "timestamp":
            case "xml":
            default:
                return DartType.DartFieldType.STRING;
        }
    }
}
