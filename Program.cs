using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp8
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string connectionString = GetConnectionString();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Animals", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string Nazvanie = reader.GetString(1);
                                int Vid = reader.GetInt32(2);
                                Console.WriteLine(id.ToString() + "\t" + Nazvanie + "\t" + Vid);
                            }
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Animals(Nazvanie,Vid) Values (@Nazvanie,@Vid)", conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Nazvanie", "Черепаха"));
                        cmd.Parameters.Add(new SqlParameter("@Vid", "1"));
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine("{0} rows affected by insert", rowsAffected);
                    }
                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM Animals WHERE Nazvanie LIKE N'%Черепаха%'", conn))
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine("{0} rows affect by delete", rowsAffected);
                    }
                    using (SqlCommand cmd = new SqlCommand("GetStoredAnimals", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {

                                /* Console.Writeline ("{0}\t{I}\t(2)", reader.GetInt32(0),reader.GetString(1), reader.GetInt32(2)); */
                                int Id = reader.GetInt32(0);
                                string Nazvanie = reader.GetString(1);
                                int Vid = reader.GetInt32(2);
                                Console.WriteLine(Id.ToString() + "\t" + Nazvanie + "\t" + Vid);
                            }

                        }
                    }
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT INTO ANIMALS(Nazvanie,Vid) VALUES('Test1',99)", conn, transaction))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            using (SqlCommand cmd = new SqlCommand("INSERT INTO ANIMALS(Nazvanie,Vid) VALUES('Test1',99)", conn, transaction))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }
                        catch (SqlException)
                        {
                            Console.WriteLine("Возникла исключительная ситуацияб откат");
                            transaction.Rollback();
                            Console.WriteLine("После попытки совершить вставку");
                            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Animals", conn))
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int Id = reader.GetInt32(0);
                                    string Nazvanie = reader.GetString(1);
                                    int Vid = reader.GetInt32(2);
                                    Console.WriteLine(Id.ToString() + "\t" + Nazvanie + "\t" + Vid);
                                }
                            }
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                Console.Write(ex);
            }
            Console.ReadKey();
        }
        static string GetConnectionString()
        {
            return @"Data Source = (LocalDB)\MSSQLLocalDB;AttachDbFilename = C:\Users\Рустам\Desktop\База\test.mdf; Integrated Security = True; Connect Timeout = 30";
        }
    }
}