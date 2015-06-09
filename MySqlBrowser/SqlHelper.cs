using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace MySqlBrowser
{
    public class SqlHelper
    {
        public SqlConnection Connection { get; private set; }
        public SqlCommand Command { get; private set; }

        public SqlHelper(string connStr)
        {
            this.Connection = new SqlConnection(connStr);
            this.Command = this.Connection.CreateCommand();
        }

        public List<string> GetDataList()
        {
            List<string> sonuclar = new List<string>();

            this.Connection.Open();

            SqlDataReader reader = this.Command.ExecuteReader();

            while (reader.Read())
            {
                string deger = reader.GetString(0);
                sonuclar.Add(deger);
            }

            reader.Close();
            reader.Dispose();

            this.Connection.Close();

            return sonuclar;
        }

        public DataTable GetDataTable()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(this.Command);
            adapter.Fill(dt);

            return dt;
        }
    }
}
