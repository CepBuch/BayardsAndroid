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

                @"CREATE TABLE [Category] (
[Id] INTEGER PRIMARY KEY AUTOINCREMENT,
[Category_Id] ntext,
[Category_Name] ntext,
[Language] nvarchar(5),
[Parent_Id] ntext,
FOREIGN KEY ([Parent_Id]) REFERENCES [Category]([Category_Id])
);",

                @"CREATE TABLE [Risk] (
[Id] INTEGER PRIMARY KEY AUTOINCREMENT,
[Risk_Id] ntext,
[Risk_Name] ntext,
[Risk_Content] ntext,
[Language] nvarchar(5),
[Category_Id] ntext,
FOREIGN KEY ([Category_Id]) REFERENCES [Category]([Category_Id])
);",
                @"CREATE TABLE [Media] (
[Id] INTEGER PRIMARY KEY AUTOINCREMENT,
[Media_Path] ntext,
[Media_Type] ntext,
[Language] ntext,
[Risk_Id] ntext,
FOREIGN KEY ([Risk_Id]) REFERENCES [Risk]([Risk_Id])
);" };

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
                catch (Exception ex)
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


        public bool SaveData(Category[] categories)
        {
            if (Connection != null)
            {
                try
                {
                    Connection.Open();

                    foreach (var cat in categories)
                    {
                        //List of commands.
                        var commands = new List<string>
                        {
                            //Inserting category
                            $"INSERT INTO [Category] ([Category_Id], [Category_Name], [Language], [Parent_Id] )" +
                                $"VALUES ('{cat.Id}', '{cat.Name}', '{cat.Language}', NULL);"
                        };

                        //Inserting all risks for category
                        foreach (var risk in cat.Risks)
                        {
                            commands.Add(
                                    $"INSERT INTO [Risk] ([Risk_Id],  [Risk_Name], [Risk_content], [Language], [Category_Id] )" +
                                    $"VALUES ('{risk.Id}', '{risk.Name}', '{risk.Content}', '{risk.Language}', '{cat.Id}');");

                            foreach (var mediaObj in risk.MediaObjects)
                            {
                                commands.Add(
                                    $"INSERT INTO [Media] ([Media_Path], [Media_Type], [Language], [Risk_Id])" +
                                    $"VALUES ('{mediaObj.Link}', '{mediaObj.TypeMedia}', '{risk.Language}', '{risk.Id}');");
                            }
                        }

                        //Inserting all subcategories for this category
                        foreach (var subcat in cat.Subcategories)
                        {
                            commands.Add(
                                $"INSERT INTO [Category] ([Category_Id],  [Category_Name], [Language], [Parent_Id] )" +
                                $"VALUES ('{subcat.Id}', '{subcat.Name}', '{subcat.Language}', '{cat.Id}');");

                            //Inserting all risks for each subcategory
                            foreach (var risk in subcat.Risks)
                            {
                                commands.Add(
                                    $"INSERT INTO [Risk] ([Risk_Id],  [Risk_Name], [Risk_Content], [Language], [Category_Id] )" +
                                    $"VALUES ('{risk.Id}', '{risk.Name}', '{risk.Content}', '{risk.Language}', '{subcat.Id}');");
                                foreach (var mediaObj in risk.MediaObjects)
                                {
                                    commands.Add(
                                       $"INSERT INTO [Media] ([Media_Path], [Media_Type], [Language], [Risk_Id])" +
                                       $"VALUES ('{mediaObj.Link}', '{mediaObj.TypeMedia}', '{risk.Language}', '{risk.Id}');");
                                }
                            }
                        }

                        //Inserting all data for this category in Database
                        foreach (var command in commands)
                        {
                            using (var c = Connection.CreateCommand())
                            {
                                c.CommandText = command;
                                var rowcount = c.ExecuteNonQuery();
                            }
                        }
                    }
                    //Success
                    return true;
                }
                catch (Exception ex)
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


        public IEnumerable<Category> GetCategories(string language)
        {
            List<Category> categories = new List<Category>();


            if (Connection != null)
            {
                try
                {
                    Connection.Open();
                    using (var contents = Connection.CreateCommand())
                    {
                        //Parent_id = NULL is condition to get Categories (not subcategories)
                        contents.CommandText = $"SELECT [Category_Id], [Category_Name], [Language], [Parent_Id] FROM [Category]" +
                            $"WHERE [Language] = '{language}' AND [Parent_id] IS NULL";
                        contents.CommandType = System.Data.CommandType.Text;
                        var r = contents.ExecuteReader();

                        while (r.Read())
                        {
                            categories.Add(new Model.Category
                            {
                                Id = r["Category_Id"].ToString(),
                                Name = r["Category_Name"].ToString(),
                                Language = r["Language"].ToString()
                            });
                        }
                    }
                    return categories.Where(c => c!=null && !string.IsNullOrWhiteSpace(c.Id) && !string.IsNullOrWhiteSpace(c.Name));
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



        public IEnumerable<Category> GetSubcategories(string parent_category_id, string language)
        {
            if (string.IsNullOrWhiteSpace(parent_category_id))
                throw new NullReferenceException("category_id cannot be null or whitespace");

            if (string.IsNullOrWhiteSpace(language))
                throw new NullReferenceException("language cannot be null or whitespace");


            if (Connection != null)
            {
                List<Category> foundSubcategories = new List<Category>();
                try
                {
                    Connection.Open();
                    using (var contents = Connection.CreateCommand())
                    {
                        contents.CommandText = "SELECT  [Category_Id], [Category_Name], [Language] from [Category]" +
                            $"WHERE [Category_Id] = '{parent_category_id}' AND [Language] =  '{language}';";

                        var r = contents.ExecuteReader();
                        while (r.Read())
                        {
                            foundSubcategories.Add(new Model.Category
                            {
                                Id = r["Category_Id"].ToString(),
                                Name = r["Category_Name"].ToString(),
                                Language = r["Language"].ToString()
                            });
                        }
                    }
                    return foundSubcategories.Where(c => c!= null && !string.IsNullOrWhiteSpace(c.Id) && !string.IsNullOrWhiteSpace(c.Name));
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


        public IEnumerable<Risk> GetRisks(string parent_category_id, string language)
        {
            if (string.IsNullOrWhiteSpace(parent_category_id))
                throw new NullReferenceException("category_id cannot be null or whitespace");

            if (string.IsNullOrWhiteSpace(language))
                throw new NullReferenceException("language cannot be null or whitespace");


            if (Connection != null)
            {
                List<Risk> foundRisks = new List<Risk>();
                try
                {
                    Connection.Open();
                    using (var contents = Connection.CreateCommand())
                    {
                        contents.CommandText = "SELECT [Risk_Name], [Risk_Content], [Language], [Category_Id]," +
                            "[Media_path], [Media_type], [Media_description] from [Risk]" +
                            "INNER JOIN [Media] on [Risk].[Risk_Id] = [Media].[Risk_Id]" +
                            $"WHERE [Category_Id] = '{parent_category_id}' AND [Language] = '{language}';";

                        var r = contents.ExecuteReader();
                        while (r.Read())
                        {
                            foundRisks.Add(new Model.Risk
                            {
                                Id = r["Risk_Id"].ToString(),
                                Name = r["Risk_Name"].ToString(),
                                Content = r["Risk_Content"].ToString(),
                                Language = r["Language"].ToString()
                            });
                        }
                    }
                    return foundRisks.Where(r => r!=null && !string.IsNullOrWhiteSpace(r.Id) && !string.IsNullOrWhiteSpace(r.Name));
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
