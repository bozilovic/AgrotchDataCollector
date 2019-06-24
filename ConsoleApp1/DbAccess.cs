using System;
using System.Data;
using System.Data.SqlClient;

namespace ArduinoController
{
    class DbAccess
    {
        //"Data Source=172.30.39.99\DEMOROOM;Initial Catalog=eMES_AgroTech;User ID=eng;Password=eng;Application name='eMES_AgroTech';Pooling=true;";
        static private string connectionString = Configuration.GetConfig()["ConnectionString"];

        static public void ComSql(string strSQL)
        {
            SqlCommand cmd;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    cmd = new SqlCommand(strSQL, conn);
                    int records = cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine(sqlEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        static public void ExecSqlProcedure(DataTable dt)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(Configuration.GetConfig()["storedProcedure"], conn); 
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@table", dt);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine(sqlEx.Message);
            }
            catch (Exception ex)
            {
                string err = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message.ToString();
                Console.WriteLine(err);
            }
        }

        /* 
        static public void ExecSqlProcedure(string node, string sensor, object value)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString)) {
                    conn.Open();
                    var cmd = new SqlCommand(Configuration.GetConfig()["storedProcedure"], conn); 
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@node", node);
                    cmd.Parameters.AddWithValue("@sensor", sensor);
                    cmd.Parameters.AddWithValue("@value", value);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine(sqlEx.Message);
            }
            catch (Exception ex)
            {
                string err = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message.ToString();
                Console.WriteLine(err);
            }
        }*/
    }
}
