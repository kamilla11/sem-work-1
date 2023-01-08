using System.Data;
using System.Text;
using Npgsql;

namespace HttpServer.MyORM;

public class Database
{
    //method to check database correct working
    public static void Check()
    {
        string _connectionStr = GlobalSettings.ConnectionString;
        //var acc = new Database(_connectionStr).Select<Account>(2);

        //new Database(_connectionStr).Delete(new Account { Id = 2, Login = "hihihi", Password = "hahaha" });

        //new Database(_connectionStr).Update(new Account {Id = 2, Email = "kamamama", Password = "kama", Name = "kama", Surname = "kama", Gender = "Небинарная личность"});

        //new Database(_connectionStr).Delete<Account>(6);

        //new Database(_connectionStr).Insert(new Account { Login = "hihihi", Password = "hahaha" });

        //var result = new Database(_connectionStr).Select<Account>();

        // foreach (var r in result)
        // {
        //     Console.WriteLine(r);
        // }
    }

    private readonly IDbConnection _connection;
    private readonly IDbCommand _cmd;

    public Database(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
        _cmd = _connection.CreateCommand();
    }

    public IEnumerable<T> Select<T>()
    {
        var tableName = typeof(T).Name.ToLower();
        return ExecuteQuery<T>($"select * from public.\"{tableName}\"");
    }

    public T Select<T>(int id)
    {
        var tableName = typeof(T).Name.ToLower();
        return ExecuteQuery<T>($"select * from public.\"{tableName}\" where id = {id}").FirstOrDefault();
    }

    public T Select<T>(Guid id)
    {
        var tableName = typeof(T).Name.ToLower();
        return ExecuteQuery<T>($"select * from public.\"{tableName}\" where id::text = '{id}'").FirstOrDefault();
    }

    public int Insert<T>(T obj)
    {
        var t = typeof(T);
        var tableName = t.Name.ToLower();
        var nameList = new List<string>();
        t.GetProperties().ToList().ForEach(p =>
        {
            var name = p.Name.ToLower();
            if (p.PropertyType.Name == "Guid" && name == "id")
            {
                nameList.Add(name);
                AddParameter("@" + name, p.GetValue(obj));
            }

            if (name != "id")
            {
                nameList.Add(name);
                AddParameter("@" + name, p.GetValue(obj));
            }
        });

        var (valuesNames, values) = GenerateValuesString(nameList);

        return this.ExecuteNonQuery(
            $"insert into public.\"{tableName}\" {valuesNames.ToString()} values{values.ToString()}");
    }

    private static (StringBuilder valuesNames, StringBuilder values) GenerateValuesString(List<string> nameList)
    {
        var valuesNames = new StringBuilder();
        var values = new StringBuilder();
        for (var i = 0; i < nameList.Count; i++)
        {
            if (i == 0)
            {
                valuesNames.Append("(");
                values.Append("(");
            }

            valuesNames.Append(nameList[i] + ",");
            values.Append("@" + nameList[i] + ",");

            if (i == nameList.Count - 1)
            {
                valuesNames.Remove(valuesNames.Length - 1, 1);
                valuesNames.Append(')');

                values.Remove(values.Length - 1, 1);
                values.Append(')');
            }
        }

        return (valuesNames, values);
    }

    public int Delete<T>(T obj)
    {
        var t = typeof(T);
        var tableName = t.Name.ToLower();
        var properties = t.GetProperties();
        var values = string.Join(" and ", properties.Select(p =>
        {
            var val = p.GetValue(obj);
            var name = p.Name.ToLower();
            if (name == "id")
            {
                AddParameter("@" + name, val.ToString());
                return name + "::text=@" + name;
            }
            else
            {
                AddParameter("@" + name, val);
                return name + "=@" + name;
            }
        }));
        var query = $"delete from public.\"{tableName}\" where {values}";
        return ExecuteNonQuery(query);
    }

    public int Delete<T>(int id)
    {
        var t = typeof(T);
        var tableName = t.Name.ToLower();
        AddParameter("@id", id);
        return ExecuteNonQuery($"delete from public.\"{tableName}\" where id=@id");
    }

    public int Delete<T>(Guid id)
    {
        var t = typeof(T);
        var tableName = t.Name.ToLower();
        AddParameter("@id", id.ToString());
        return ExecuteNonQuery($"delete from public.\"{tableName}\" where id::text=@id");
    }

    public int Update<T>(T obj)
    {
        var t = typeof(T);
        var tableName = t.Name.ToLower();
        var properties = t.GetProperties();
        var values = string.Join(", ", properties.Where(p => p.Name.ToLower() != "id").Select(p =>
        {
            var val = p.GetValue(obj);
            var name = p.Name.ToLower();
            AddParameter("@" + name, val);
            return name + "=@" + name;
        }));
        AddParameter("@id", properties.First().GetValue(obj).ToString());
        return ExecuteNonQuery($"update public.\"{tableName}\" set {values} where id::text=@id");
    }

    public Database AddParameter<T>(string name, T value)
    {
        NpgsqlParameter param = new NpgsqlParameter();
        param.ParameterName = name;
        param.Value = value;
        _cmd.Parameters.Add(param);
        return this;
    }

    public int ExecuteNonQuery(string query)
    {
        int noOfAffectedRows = 0;
        using (_connection)
        {
            _cmd.CommandText = query;
            _connection.Open();
            noOfAffectedRows = _cmd.ExecuteNonQuery();
        }

        return noOfAffectedRows;
    }

    public IEnumerable<T> ExecuteQuery<T>(string query)
    {
        IList<T> list = new List<T>();
        Type t = typeof(T);
        using (_connection)
        {
            _cmd.CommandText = query;
            _connection.Open();
            var reader = _cmd.ExecuteReader();
            while (reader.Read())
            {
                var obj = (T)Activator.CreateInstance(t);
                t.GetProperties().ToList().ForEach(p =>
                {
                    var val = reader[p.Name.ToLower()];
                    p.SetValue(obj, val);
                });
                list.Add(obj);
            }
        }

        return list;
    }

    public T ExecuteScalar<T>(string query)
    {
        var result = default(T);
        using (_connection)
        {
            _cmd.CommandText = query;
            _connection.Open();
            result = (T)_cmd.ExecuteScalar();
        }

        return result;
    }
}