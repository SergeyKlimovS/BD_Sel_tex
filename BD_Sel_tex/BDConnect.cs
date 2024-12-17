using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BD_Sel_tex
{
    class BDConnect
    {
        static string dbConnection = @"Server=DESKTOP-64A8D5H\SQLEXPRESS;Database=BD_Sel_tex;Integrated Security=True;TrustServerCertificate=True;";
        static public SqlDataAdapter sqlDataAdapter;
        static SqlConnection sqlConnection;
        static public SqlCommand sqlCommand;

        public static bool ConnectionBd()
        {
            try
            {
                sqlConnection = new SqlConnection(dbConnection);
                sqlConnection.Open();
                sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                return true;
            }
            catch
            {
                MessageBox.Show("Error connection!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static void CloseConnection()
        {
            if (sqlConnection != null && sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }

        public static SqlConnection GetSqlConnection() => sqlConnection;

    }
}
