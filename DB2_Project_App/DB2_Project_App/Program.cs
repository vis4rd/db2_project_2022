using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using DB2_Project.Test_Scripts;

namespace DB2_Project_App
{
    class Program
    {
        static String sqlconnection = "Data Source=mssqlserver55;Initial Catalog=AdventureWorks2008;Integrated Security=True";

        static void Main(string[] args)
        {
            string openQuery = @"
EXEC CreatePointsTable;
EXEC CreatePolygonsTable;
INSERT INTO Points(Pos) VALUES ('1; 2'), ('5;9'), ('-3; 0'), ('1;2');
INSERT INTO Polygons VALUES ('');
EXEC FillPolygon 1;";
            string closeQuery = @"
EXEC DropPointsTable;
EXEC DropPolygonsTable;
";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(openQuery, conn);
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally { conn.Close(); }

            bool showMenu = true;
            while (showMenu)
            {
                try { showMenu = MainMenu(); }
                catch (Exception e)
                {
                    Console.WriteLine("The program halted due to exception: " + e.Message);
                    showMenu = false;
                    Console.WriteLine("\nPress ENTER to exit...");
                    Console.ReadLine();
                }
            }

            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(closeQuery, conn);
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally { conn.Close(); }
        }

        private static bool MainMenu()
        {
            Console.Clear();
            Console.Write(@"
            Choose an option:
            1) Insert a Point
            2) Insert a Polygon
            3) List Points
            4) List Polygons
            5) Delete Points
            6) Delete Polygons
            7) Calculate a Polygon area
            8) Calculate if a Point lies inside a Polygon
            9) Calculate the distance between two Points
            t) Run Tests
      q, e, 0) Exit
            Select an option: ");

            switch (Console.ReadLine())
            {
                case "1": InsertPointMenuOption(); break;
                case "2": InsertPolygonMenuOption(); break;
                case "3": ListPointsMenuOption(); break;
                case "4": ListPolygonsMenuOption(); break;
                case "5": DeletePointsMenuOption(); break;
                case "6": DeletePolygonsMenuOption(); break;
                case "7": CalculatePolygonAreaMenuOption(); break;
                case "8": CalculatePointInsidePolygonMenuOption(); break;
                case "9": CalculateDistanceMenuOption();  break;
                case "t": new Test().CallAllTests(); break;
                case "q":
                case "e":
                case "0": return false;
            }
            Console.Write("Press ENTER to continue...");
            Console.ReadLine();
            return true;
        }

        private static void InsertPointMenuOption()
        {
            Console.Write("Type in position in format 'x;y': ");
            string input = Console.ReadLine();
            Console.Write(input);

            string query = "INSERT INTO dbo.Points(pos) VALUES (@value);";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@value", SqlDbType.VarChar);
                command.Parameters["@value"].Value = input;

                Int32 affectedRows = command.ExecuteNonQuery();
                Console.WriteLine("RowsAffected: {0}", affectedRows);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally { conn.Close(); }
        }

        private static void InsertPolygonMenuOption()
        {
            Console.Write("Type in position in format '(x1;y1)(x2;y2)...(xn;yn)': ");
            string input = Console.ReadLine();
            Console.Write(input);

            string query = "INSERT INTO dbo.Polygons(polygon) VALUES (@value);";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@value", SqlDbType.VarChar);
                command.Parameters["@value"].Value = input;

                Int32 affectedRows = command.ExecuteNonQuery();
                Console.WriteLine("RowsAffected: {0}", affectedRows);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally { conn.Close(); }
        }

        private static void ListPointsMenuOption()
        {
            string query = "SELECT id, pos.ToString() as pos FROM dbo.Points;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();

                Console.WriteLine("List of Points:");
                while (datareader.Read())
                {
                    Console.WriteLine(datareader["id"] + ": " + datareader["pos"].ToString());
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally { conn.Close(); }
        }

        private static void ListPolygonsMenuOption()
        {
            string query = "SELECT id, polygon.ToString() as poly FROM dbo.Polygons;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();

                Console.WriteLine("List of Polygons:");
                while (datareader.Read())
                {
                    Console.WriteLine(datareader["id"] + ": " + datareader["poly"].ToString());
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally { conn.Close(); }
        }

        private static void DeletePointsMenuOption()
        {
            ListPointsMenuOption();
            Console.WriteLine("Choose a Point by ID: ");
            string input = Console.ReadLine();

            string query = "DELETE FROM dbo.Points WHERE id = @value";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@value", SqlDbType.Int);
                command.Parameters["@value"].Value = Int32.Parse(input);

                command.ExecuteNonQuery();
                Console.WriteLine("The Point of ID = {0} has been deleted.", input);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally { conn.Close(); }
        }

        private static void DeletePolygonsMenuOption()
        {
            ListPolygonsMenuOption();
            Console.WriteLine("Choose a Polygon by ID: ");
            string input = Console.ReadLine();

            string query = "DELETE FROM dbo.Polygons WHERE id = @value";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@value", SqlDbType.Int);
                command.Parameters["@value"].Value = Int32.Parse(input);

                command.ExecuteNonQuery();
                Console.WriteLine("The Polygon of ID = {0} has been deleted.", input);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally { conn.Close(); }
        }

        private static void CalculatePolygonAreaMenuOption()
        {
            ListPolygonsMenuOption();
            Console.Write("Choose a Polygon by ID to get its area: ");
            string input = Console.ReadLine();

            string query = "SELECT id, polygon.CalculateArea() AS area FROM Polygons WHERE id = @value;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@value", SqlDbType.Int);
                command.Parameters["@value"].Value = Int32.Parse(input);

                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Console.WriteLine("Area of Polygon with ID = "
                        + datareader["id"].ToString()
                        + " is "
                        + datareader["area"].ToString());
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally { conn.Close(); }
        }

        private static void CalculatePointInsidePolygonMenuOption()
        {
            ListPolygonsMenuOption();
            Console.Write("Choose a Polygon by ID: ");
            string input = Console.ReadLine();

            Console.Write("Type in a Point position in format 'x;y': ");
            string input2 = Console.ReadLine();

            Console.WriteLine("id = '" + input + "', point = '" + input2 + "'");

            string query = "SELECT id, polygon.IsPointInside(@point) AS result FROM Polygons WHERE id = @polyID;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@point", SqlDbType.VarChar);
                command.Parameters["@point"].Value = input2;
                command.Parameters.Add("@polyID", SqlDbType.Int);
                command.Parameters["@polyID"].Value = Int32.Parse(input);

                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Console.Write("Is the Point ("
                        + input2
                        + ") inside an area of Polygon with ID = "
                        + datareader["id"].ToString()
                        + "?");
                    Console.WriteLine(" " + datareader["result"]);
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally { conn.Close(); }
        }

        private static void CalculateDistanceMenuOption()
        {
            ListPointsMenuOption();
            Console.Write("Choose a Point by ID: ");
            string input = Console.ReadLine();

            Console.Write("Type in a Point position in format 'x;y': ");
            string input2 = Console.ReadLine();

            Console.WriteLine("id = '" + input + "', point = '" + input2 + "'");

            string query = "SELECT id, pos.DistanceTo(@point) AS result FROM Points WHERE id = @ID;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@point", SqlDbType.VarChar);
                command.Parameters["@point"].Value = input2;
                command.Parameters.Add("@ID", SqlDbType.Int);
                command.Parameters["@ID"].Value = Int32.Parse(input);

                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Console.WriteLine("The distance to ("
                        + input2
                        + ") is "
                        + datareader["result"]);
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally { conn.Close(); }
        }
    }
}
