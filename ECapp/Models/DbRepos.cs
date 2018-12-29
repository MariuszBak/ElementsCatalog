using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using ECapp.Models.Entities;

namespace ECapp.Models
{
    public class DbRepos
    {
        private SQLiteConnectionStringBuilder _connStringBuilder;

        public DbRepos(string filePath = "./main.db")
        {
            _connStringBuilder = new SQLiteConnectionStringBuilder
            {
                DataSource = filePath,
                DefaultTimeout = 2
            };

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Database file not exist.");
                DbCreate();
            }
            else
            {
                Console.WriteLine("Database file exist.");
                DbCheck();
            }


            // testy
            /*
            InsertElement(new Element { Name = "BC107", Package = "TO20", Category = "Tranzystor", Quantity = 100, Status = "nowe", Desc = "Tranzystor NPN", Container = "B11" });
            DeleteElement(4);
            int i = DateTime.Now.Second;
            UpdateElement(new Element { Id = 2, Name = "BC107" + i, Package = "TO20" + i, Category = "Tranzystor" + i, Quantity = 100 + i, Status = "nowe" + i, Desc = "Tranzystor NPN" + i, Container = "B11" + i });
            Element elem = GetElement(2);
            Console.WriteLine(elem.Id);
            Console.WriteLine(elem.Name);
            Console.WriteLine(elem.Package);
            Console.WriteLine(elem.Category);
            Console.WriteLine(elem.Quantity);
            Console.WriteLine(elem.Status);
            Console.WriteLine(elem.Desc);
            Console.WriteLine(elem.Container);
            List<Element> myElements = GetElements("nowe3");
            Console.WriteLine("Found " + myElements.Count + " element(s).");
            */

        }

        private void DbCheck()
        {
            Console.WriteLine("Checking database file: " + _connStringBuilder.DataSource);
            if (DbTableExist("elements")) Console.WriteLine("Database is OK.");
        }

        private bool DbTableExist(string tableName)
        {
            Console.WriteLine("Check if table " + tableName + " exist.");

            bool result = false;
            using (var conn = new SQLiteConnection(_connStringBuilder.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='" + tableName + "';";
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (tableName.Equals(reader[0])) result = true;
                    }
                }
            }

            if (result) Console.WriteLine("Table exists."); else Console.WriteLine("Table does not exist.");

            return result;
        }

        private void DbCreate()
        {
            Console.WriteLine("Create database file: " + _connStringBuilder.DataSource);
            using (var conn = new SQLiteConnection(_connStringBuilder.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE elements(Id INTEGER PRIMARY KEY UNIQUE NOT NULL, Name TEXT, Category TEXT, Container TEXT, Package TEXT, Desc TEXT, Status TEXT, Quantity INTEGER)";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public long InsertElement(Element element)
        {
            Console.Write("Insert element: " + element.Name);
            long id = 0;
            using (var conn = new SQLiteConnection(_connStringBuilder.ConnectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO elements (Name, Category, Container, Package, Desc, Status, Quantity) VALUES(@Name, @Category, @Container, @Package, @Desc, @Status, @Quantity)";
                    cmd.Parameters.Add(new SQLiteParameter("@Name", element.Name));
                    cmd.Parameters.Add(new SQLiteParameter("@Category", element.Category));
                    cmd.Parameters.Add(new SQLiteParameter("@Container", element.Container));
                    cmd.Parameters.Add(new SQLiteParameter("@Package", element.Package));
                    cmd.Parameters.Add(new SQLiteParameter("@Desc", element.Desc));
                    cmd.Parameters.Add(new SQLiteParameter("@Status", element.Status));
                    cmd.Parameters.Add(new SQLiteParameter("@Quantity", element.Quantity));
                    cmd.ExecuteNonQuery();
                    id = conn.LastInsertRowId;
                }
            }
            Console.WriteLine(" at Id: " + id);
            return id;
        }

        public Element GetElement(long id)
        {
            Console.WriteLine("Get element: " + id);
            Element element = new Element();
            using (var conn = new SQLiteConnection(_connStringBuilder.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name, Category, Container, Package, Desc, Status, Quantity FROM elements WHERE Id = @Id";
                    cmd.Parameters.Add(new SQLiteParameter("@Id", id));
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            element.Id = (long)reader["Id"];
                            element.Name = reader["Name"] as string;
                            element.Category = reader["Category"] as string;
                            element.Container = reader["Container"] as string;
                            element.Package = reader["Package"] as string;
                            element.Desc = reader["Desc"] as string;
                            element.Status = reader["Status"] as string;
                            element.Quantity = (long)reader["Quantity"];
                        }
                    }
                }
            }
            return element;
        }

        public List<ElementShort> GetElementsShort(string searchPhrase)
        {
            Console.WriteLine("Get elements with phrase: " + searchPhrase);
            List<ElementShort> elements = new List<ElementShort>();
            using (var conn = new SQLiteConnection(_connStringBuilder.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name, Category, Container, Package, Desc, Status, Quantity FROM elements WHERE Name LIKE @ph OR Category LIKE @ph OR Package LIKE @ph OR Desc LIKE @ph OR Status LIKE @ph";
                    cmd.Parameters.Add(new SQLiteParameter("@ph", "%" + searchPhrase + "%"));
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        ElementShort element;
                        while (reader.Read())
                        {
                            element = new ElementShort
                            {
                                Id = (long)reader["Id"],
                                Name = reader["Name"] as string,
                                Container = reader["Container"] as string,
                                Package = reader["Package"] as string,
                                Quantity = (long)reader["Quantity"]
                            };
                            elements.Add(element);
                        }
                    }
                }
            }
            return elements;
        }

        public List<ElementShort> GetAllElementsShort()
        {
            return GetElementsShort("");
        }

        public List<Element> GetElements(string searchPhrase)
        {
            Console.WriteLine("Get elements with phrase: " + searchPhrase);
            List<Element> elements = new List<Element>();
            using (var conn = new SQLiteConnection(_connStringBuilder.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name, Category, Container, Package, Desc, Status, Quantity FROM elements WHERE Name LIKE @ph OR Category LIKE @ph OR Package LIKE @ph OR Desc LIKE @ph OR Status LIKE @ph";
                    cmd.Parameters.Add(new SQLiteParameter("@ph", "%" + searchPhrase + "%"));
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        Element element;
                        while (reader.Read())
                        {
                            element = new Element
                            {
                                Id = (long)reader["Id"],
                                Name = (string)reader["Name"],
                                Category = (string)reader["Category"],
                                Container = (string)reader["Container"],
                                Package = (string)reader["Package"],
                                Desc = (string)reader["Desc"],
                                Status = (string)reader["Status"],
                                Quantity = (long)reader["Quantity"]
                            };
                            elements.Add(element);
                        }
                    }
                }
            }
            return elements;
        }

        public List<Element> GetAllElements()
        {
            return GetElements("");
        }

        public void UpdateElement(Element element)
        {
            Console.WriteLine("Update element: " + element.Name);
            using (var conn = new SQLiteConnection(_connStringBuilder.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE elements SET Name = @Name, Category = @Category, Container = @Container, Package = @Package, Desc = @Desc, Status = @Status, Quantity = @Quantity WHERE Id = @Id";
                    cmd.Parameters.Add(new SQLiteParameter("@Id", element.Id));
                    cmd.Parameters.Add(new SQLiteParameter("@Name", element.Name));
                    cmd.Parameters.Add(new SQLiteParameter("@Category", element.Category));
                    cmd.Parameters.Add(new SQLiteParameter("@Container", element.Container));
                    cmd.Parameters.Add(new SQLiteParameter("@Package", element.Package));
                    cmd.Parameters.Add(new SQLiteParameter("@Desc", element.Desc));
                    cmd.Parameters.Add(new SQLiteParameter("@Status", element.Status));
                    cmd.Parameters.Add(new SQLiteParameter("@Quantity", element.Quantity));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteElement(long id)
        {
            Console.WriteLine("Delete element: " + id);
            using (var conn = new SQLiteConnection(_connStringBuilder.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM elements WHERE Id = @Id";
                    cmd.Parameters.Add(new SQLiteParameter("@Id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}
