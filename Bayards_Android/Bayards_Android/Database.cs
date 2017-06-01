using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.IO;
using Bayards_Android.Model;
using System.Linq;
using Bayards_Android.Enums;

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
                    var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
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
[Category_Language] nvarchar(5),
[Parent_Id] ntext,
[Category_Order] int,
FOREIGN KEY ([Parent_Id]) REFERENCES [Category]([Category_Id])
);",

                @"CREATE TABLE [Risk] (
[Id] INTEGER PRIMARY KEY AUTOINCREMENT,
[Risk_Id] ntext,
[Risk_Name] ntext,
[Risk_Content] ntext,
[Risk_Language] nvarchar(5),
[Category_Id] ntext,
[Risk_Order] int,
FOREIGN KEY ([Category_Id]) REFERENCES [Category]([Category_Id])
);",
                @"CREATE TABLE [Media] (
[Id] INTEGER PRIMARY KEY AUTOINCREMENT,
[Media_Name] ntext,
[Media_Type] ntext,
[Media_Language] nvarchar(5),
[Risk_Id] ntext,
FOREIGN KEY ([Risk_Id]) REFERENCES [Risk]([Risk_Id])
);",

            @"CREATE TABLE [Location] (
[Id] INTEGER PRIMARY KEY AUTOINCREMENT,
[Location_Id] ntext,
[Location_Name] ntext,
[Location_Lat] float,
[Location_Long] float,
[Location_Language] nvarchar(5),
[Location_Order] int);"};

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

        string query;
        public bool SaveData(Category[] categories, Location [] locations)
        {
            if (Connection != null)
            {
                try
                {
                    Connection.Open();

                    ReplaceQuotes(categories);

                    foreach (var cat in categories)
                    {
                        //List of commands.
                        var commands = new List<string>
                        {
                            //Inserting category
                            $"INSERT INTO [Category] ([Category_Id], [Category_Name], [Category_Language], [Parent_Id], [Category_Order])" +
                                $"VALUES ('{cat.Id}', '{cat.Name}', '{cat.Language}', NULL, '{cat.Order}');"
                        };

                        //Inserting all risks for category
                        foreach (var risk in cat.Risks)
                        {
                            commands.Add(
                                    $"INSERT INTO [Risk] ([Risk_Id],  [Risk_Name], [Risk_content], [Risk_Language], [Category_Id], [Risk_Order])" +
                                    $"VALUES ('{risk.Id}', '{risk.Name}', '{risk.Content}', '{risk.Language}', '{cat.Id}', '{risk.Order}');");

                            foreach (var mediaObj in risk.MediaObjects)
                            {
                                commands.Add(
                                    $"INSERT INTO [Media] ([Media_Name], [Media_Type], [Media_Language], [Risk_Id])" +
                                    $"VALUES ('{mediaObj.Name}', '{mediaObj.TypeMedia.ToString().ToLower()}', '{risk.Language}', '{risk.Id}');");
                            }
                        }

                        //Inserting all subcategories for this category
                        foreach (var subcat in cat.Subcategories)
                        {
                            commands.Add(
                                $"INSERT INTO [Category] ([Category_Id],  [Category_Name], [Category_Language], [Parent_Id], [Category_Order])" +
                                $"VALUES ('{subcat.Id}', '{subcat.Name}', '{subcat.Language}', '{cat.Id}','{cat.Order}');");

                            //Inserting all risks for each subcategory
                            foreach (var risk in subcat.Risks)
                            {
                                commands.Add(
                                    $"INSERT INTO [Risk] ([Risk_Id],  [Risk_Name], [Risk_Content], [Risk_Language], [Category_Id], [Risk_Order])" +
                                    $"VALUES ('{risk.Id}', '{risk.Name}', '{risk.Content}', '{risk.Language}', '{subcat.Id}', '{risk.Order}');");
                                foreach (var mediaObj in risk.MediaObjects)
                                {
                                    commands.Add(
                                       $"INSERT INTO [Media] ([Media_Name], [Media_Type], [Media_Language], [Risk_Id])" +
                                       $"VALUES ('{mediaObj.Name}', '{mediaObj.TypeMedia.ToString().ToLower()}',  '{risk.Language}', '{risk.Id}');");
                                }
                            }
                        }
                        //Inserting all data for this category in Database
                        foreach (var command in commands)
                        {
                            using (var c = Connection.CreateCommand())
                            {
                                c.CommandText = command;
                                query = command;
                                var rowcount = c.ExecuteNonQuery();
                            }
                        }
                    }
                    //Inserting all locations
                    foreach (var loc in locations)
                    {
                        using (var c = Connection.CreateCommand())
                        {
                            string command = $"INSERT INTO [Location] ([Location_Id],  [Location_Name]," +
                                $" [Location_Lat], [Location_Long], [Location_Language], [Location_Order]) " +
                                $"VALUES ('{loc.Id}', '{loc.Name}', '{loc.Latitude}', '{loc.Longtitude}','{loc.Language}', '{loc.Order}');";
                            c.CommandText = command;
                            query = command;
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



        /// <summary>
        /// 
        /// </summary>
        /// <param name="language">Application current language</param>
        /// <returns></returns>
        public int? CountCategories(string language)
        {
            if (Connection != null)
            {
                try
                {
                    Connection.Open();
                    using (var contents = Connection.CreateCommand())
                    {
                        contents.CommandText = $"SELECT COUNT(*) FROM [Category]" +
                            $"WHERE [Category_Language] = '{language}' AND [Parent_id] IS NULL";
                        var r = contents.ExecuteScalar();
                        return int.Parse(r.ToString());
                    }
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

        public int? CountSubcategories(string parent_category_id, string language)
        {
            if (Connection != null)
            {
                try
                {
                    Connection.Open();
                    using (var contents = Connection.CreateCommand())
                    {
                        contents.CommandText = "SELECT  COUNT(*) FROM [Category]" +
                            $"WHERE [Parent_Id] = '{parent_category_id}' AND [Category_Language] =  '{language}';";
                        var r = contents.ExecuteScalar();
                        return int.Parse(r.ToString());
                    }
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


        public int? CountRisks(string parent_category_id, string language)
        {
            if (Connection != null)
            {
                try
                {
                    Connection.Open();
                    using (var contents = Connection.CreateCommand())
                    {
                        contents.CommandText ="SELECT COUNT(*) FROM [Risk]" +
                            $" WHERE [Category_Id] = '{parent_category_id}' AND [Risk_Language] = '{language}';";
                        var r = contents.ExecuteScalar();
                        return int.Parse(r.ToString());
                    }
                }
                catch (Exception ex)
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


        public List<Category> GetCategories(string language)
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
                        contents.CommandText = $"SELECT [Category_Id], [Category_Name], [Category_Language] FROM [Category]" +
                            $"WHERE [Category_Language] = '{language}' AND [Parent_id] IS NULL" +
                            $" ORDER BY [Category_Order] ASC;";
                        contents.CommandType = System.Data.CommandType.Text;
                        var r = contents.ExecuteReader();

                        while (r.Read())
                        {
                            categories.Add(new Model.Category
                            {
                                Id = r["Category_Id"].ToString(),
                                Name = r["Category_Name"].ToString(),
                                Language = r["Category_Language"].ToString()
                            });
                        }
                    }
                    return categories.Where(c => c != null && !string.IsNullOrWhiteSpace(c.Id) && !string.IsNullOrWhiteSpace(c.Name)).ToList();
                }
                catch (Exception ex)
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



        public List<Category> GetSubcategories(string parent_category_id, string language)
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
                        contents.CommandText =
                            "SELECT  [Category_Id], [Category_Name], [Category_Language] from [Category]" +
                            $"WHERE [Parent_Id] = '{parent_category_id}' AND [Category_Language] =  '{language}'" +
                            $"ORDER BY [Category_Order] ASC;";

                        var r = contents.ExecuteReader();
                        while (r.Read())
                        {
                            foundSubcategories.Add(new Model.Category
                            {
                                Id = r["Category_Id"].ToString(),
                                Name = r["Category_Name"].ToString(),
                                Language = r["Category_Language"].ToString()
                            });
                        }
                    }
                    return foundSubcategories.Where(c => c != null && !string.IsNullOrWhiteSpace(c.Id) && !string.IsNullOrWhiteSpace(c.Name)).ToList();
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


        public List<Risk> GetRisks(string parent_category_id, string language)
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
                        contents.CommandType = System.Data.CommandType.Text;

                        contents.CommandText =
                            "SELECT [Risk_Id], [Risk_Name], [Risk_Content], [Risk_Language] FROM [Risk]" +
                            $" WHERE [Category_Id] = '{parent_category_id}' AND [Risk_Language] = '{language}'" +
                            $"ORDER BY [Risk_Order] ASC;";

                        var r = contents.ExecuteReader();

                        while (r.Read())
                        {

                            foundRisks.Add(new Model.Risk
                            {
                                Id = r["Risk_Id"].ToString(),
                                Name = r["Risk_Name"].ToString(),
                                Content = r["Risk_Content"].ToString(),
                                Language = r["Risk_Language"].ToString(),
                            });
                        }
                    }
                    return foundRisks.Where(risk => risk != null && !string.IsNullOrWhiteSpace(risk.Id) && !string.IsNullOrWhiteSpace(risk.Name)).ToList();
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

        public List<Location> GetLocations(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                throw new NullReferenceException("language cannot be null or whitespace");


            if (Connection != null)
            {
                List<Location> locations = new List<Location>();
                try
                {
                    Connection.Open();
                    using (var contents = Connection.CreateCommand())
                    {
                        contents.CommandType = System.Data.CommandType.Text;

                        contents.CommandText =
                            "SELECT  [Location_Name], [Location_Lat], [Location_Long], [Location_Language] FROM [Location]" +
                            $"WHERE [Location_Language] = '{language}'" +
                            $"ORDER BY [Location_Order] ASC;";

                        var r = contents.ExecuteReader();

                        while (r.Read())
                        {

                            locations.Add(new Model.Location
                            {
                                Name = r["Location_Name"].ToString(),
                                Latitude = double.Parse(r["Location_Lat"].ToString()),
                                Longtitude = double.Parse(r["Location_Long"].ToString()),
                                Language = r["Location_Language"].ToString()
                            });
                        }
                    }
                    return locations.Where(loc => loc != null && !string.IsNullOrWhiteSpace(loc.Name)).ToList();
                }
                catch (Exception e)
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


        public List<MediaObject> GetMedia(string parent_risk_id, string language)
        {
            if (string.IsNullOrWhiteSpace(parent_risk_id))
                throw new NullReferenceException("risk_id cannot be null or whitespace");

            if (string.IsNullOrWhiteSpace(language))
                throw new NullReferenceException("language cannot be null or whitespace");


            if (Connection != null)
            {
                List<MediaObject> foundMedia = new List<MediaObject>();
                try
                {
                    Connection.Open();
                    using (var contents = Connection.CreateCommand())
                    {
                        contents.CommandType = System.Data.CommandType.Text;

                        contents.CommandText =
                            "SELECT [Media_Name], [Media_Type] FROM [Media]" +
                            $"WHERE [Risk_Id] = '{parent_risk_id}' AND [Media_Language] = '{language}';";

                        var r = contents.ExecuteReader();
                        while (r.Read())
                        {
                            var mo = new MediaObject();
                            mo.Name = r["Media_Name"].ToString();
                            mo.TypeMedia = ToTypeMedia(r["Media_Type"].ToString());
                            foundMedia.Add(mo);
                        }
                    }
                    return foundMedia.Where(m => m != null && !string.IsNullOrWhiteSpace(m.Name) && m.TypeMedia != TypeMedia.Undefined).ToList();
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

        private TypeMedia ToTypeMedia(string str)
        {
            switch (str.ToLower())
            {
                case "image":
                    return TypeMedia.Image;
                case "video":
                    return TypeMedia.Video;
                default:
                    return TypeMedia.Undefined;
            }
        }


        private void ReplaceQuotes(Category[] categories)
        {
            foreach (var category in categories.Where(c => c != null))
            {
                category.Name = category.Name != null ? category.Name.Replace("'", "''").Replace("\"", "\\\"") : category.Name;

                if (category.Risks != null)
                {
                    foreach (var risk in category.Risks.Where(r => r != null))
                    {
                        risk.Name = risk.Name != null ? risk.Name.Replace("'", "''").Replace("\"", "\\\"") : risk.Name;
                        risk.Content = risk.Content != null ? risk.Content.Replace("'", "''").Replace("\"", "\\\"") : risk.Content;
                    }
                }

                if (category.Subcategories != null)
                {
                    foreach (var subcat in category.Subcategories.Where(sc => sc != null))
                    {
                        subcat.Name = subcat.Name != null ? subcat.Name.Replace("'", "''").Replace("\"", "\\\"") : subcat.Name;

                        if (subcat.Risks != null)
                        {
                            foreach (var risk in category.Risks.Where(r => r != null))
                            {
                                risk.Name = risk.Name != null ? risk.Name.Replace("'", "''").Replace("\"", "\\\"") : risk.Name;
                                risk.Content = risk.Content != null ? risk.Content.Replace("'", "''").Replace("\"", "\\\"") : risk.Content;
                            }
                        }
                    }


                }
            }
        }



    }
}
