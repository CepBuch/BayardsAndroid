using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.IO;
using Bayards_Android.Model;
using System.Linq;

namespace Bayards_Android
{
    public class Database
    {
        private Database() { }
        static Database _manager;

        public static Database Manager
        {
            get
            {
                if (_manager == null)
                    _manager = new Database();
                return _manager;
            }
        }


        private string _dbPath;
        public string DbPath
        {
            get
            {
                if (_dbPath == null)
                {
                    var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                    _dbPath = System.IO.Path.Combine(docsFolder, "Bayards_Db.db3");
                }
                return _dbPath;

            }
        }

        private SqliteConnection _connection;
        public SqliteConnection Connection
        {

            get
            {
                if (_connection == null && DbPath != null)
                {
                    _connection = new SqliteConnection("Data Source=" + DbPath);
                }
                return _connection;
            }
        }


        public bool CreateDatabase()
        {
            var commands = new[] {
                    "CREATE TABLE [Category] ([_id] int,  [ServerId] ntext, [Name] ntext);"
                };

            if (Connection != null)
            {
                try
                {
                    SqliteConnection.CreateFile(DbPath);
                    Connection.Open();

                    foreach (var command in commands)
                    {
                        using (var c = Connection.CreateCommand())
                        {
                            c.CommandText = command;
                            var rowcount = c.ExecuteNonQuery();

                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    Connection.Close();
                }
            }
            return false;
        }


        public bool SaveData(List<Category> categories)
        {
            if (Connection != null)
            {
                try
                {

                    int i = 1;
                    var commands = categories.Select(c => $"INSERT INTO [Category] ([_id],  [ServerId], [Name]) VALUES ('{i++}', '{c.ServerId}', '{c.Name}')");

                    Connection.Open();
                    foreach (var command in commands)
                    {
                        using (var c = Connection.CreateCommand())
                        {
                            c.CommandText = command;
                            var rowcount = c.ExecuteNonQuery();
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    Connection.Close();
                }
            }
            return false;
        }


        public IEnumerable<Category> Categories
        {
            get
            {
                List<Category> categories = new List<Category>();

                if (Connection != null)
                {
                    try
                    {
                        Connection.Open();
                        using (var contents = Connection.CreateCommand())
                        {
                            contents.CommandText = "SELECT [Name], [ServerId] from [Category];";
                            var r = contents.ExecuteReader();
                            while (r.Read())
                            {
                                categories.Add(new Model.Category
                                {
                                    Name = r["Name"].ToString(),
                                    ServerId = r["ServerId"].ToString()
                                });
                            }
                        }
                        return categories.Where(c => !string.IsNullOrWhiteSpace(c.ServerId) && !string.IsNullOrWhiteSpace(c.Name));
                    }
                    catch
                    {
                        return null;
                    }
                    finally
                    {
                        Connection.Close();
                    }
                }
                else return null;
            }
        }

        public Category GetCategory(string category_id)
        {
            if (string.IsNullOrWhiteSpace(category_id))
                throw new NullReferenceException("category_id cannot be null or whitespace");

            if (Connection != null)
            {
                List<Category> categories = new List<Category>();
                try
                {
                    Connection.Open();
                    using (var contents = Connection.CreateCommand())
                    {
                        contents.CommandText = "SELECT [Name], [ServerId] from [Category]" +
                            $"WHERE [ServerId] = '{category_id}'";

                        var r = contents.ExecuteReader();
                        while (r.Read())
                        {
                            categories.Add(new Model.Category
                            {
                                Name = r["Name"].ToString(),
                                ServerId = r["ServerId"].ToString()
                            });
                            
                        }
                    }
                    return categories.Count > 0 ? categories[0] : null;
                }
                catch
                {
                    return null;
                }
                finally
                {
                    Connection.Close();
                }
            }
            else return null;
        }
    }
}
