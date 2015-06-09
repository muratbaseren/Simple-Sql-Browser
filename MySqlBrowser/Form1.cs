using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MySqlBrowser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string connectionString = string.Empty; // ""


        private void CreateConnectionString(string dbName)
        {
            if (!string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPassword.Text))
            {
                // Sql Authentication..
                connectionString = string.Format(
                    "Server={0};Database={1};User Id={2};Password={3};",
                    txtServerName.Text, dbName, txtUsername.Text, txtPassword.Text);
            }
            else
            {
                // Windows Authentication..
                connectionString = string.Format(
                    "Server={0};Database={1};Integrated Security=True;",
                    txtServerName.Text, dbName);
            }
        }





        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.CreateConnectionString("Master");

            try
            {
                this.cmbDatabases.Items.Clear();

                SqlHelper helper = new SqlHelper(connectionString);
                helper.Command.CommandText = "SELECT name FROM SYS.DATABASES";

                List<string> databases = helper.GetDataList();

                foreach (string dbName in databases)
                {
                    this.cmbDatabases.Items.Add(dbName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtServerName.Text = @".\MSSQLSERVER2014";
        }

        private void cmbDatabases_SelectedIndexChanged(object sender, EventArgs e)
        {
            string dbName = cmbDatabases.Text;

            this.CreateConnectionString(dbName);

            try
            {
                cmbTables.Items.Clear();

                SqlHelper helper = new SqlHelper(connectionString);
                helper.Command.CommandText = "SELECT name FROM SYS.TABLES";

                List<string> tables = helper.GetDataList();

                foreach (string tableName in tables)
                {
                    cmbTables.Items.Add(tableName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            string dbName = cmbDatabases.Text;

            try
            {
                // Seçilen tablonun kolon listesi checkedlistbox'a eklenir.
                clbColumns.Items.Clear();
                string tableName = cmbTables.Text;

                SqlHelper helper = new SqlHelper(connectionString);
                helper.Command.CommandText = string.Format("SELECT COLUMN_NAME FROM {0}.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName", dbName);

                helper.Command.Parameters.AddWithValue("@TableName", tableName);

                List<string> columns = helper.GetDataList();

                foreach (string columnName in columns)
                {
                    clbColumns.Items.Add(columnName);
                }


                // Seçili tabloya select atılarak verileri elde edilir.
                helper = new SqlHelper(connectionString);
                helper.Command.CommandText = string.Format("SELECT * FROM {0}", tableName);

                txtQuery.Text = helper.Command.CommandText;

                DataTable dt = helper.GetDataTable();

                dgvResults.DataSource = dt;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRunQuery_Click(object sender, EventArgs e)
        {
            string dbName = cmbDatabases.Text;

            try
            {
                SqlHelper helper = new SqlHelper(connectionString);
                helper.Command.CommandText = txtQuery.Text;

                DataTable dt = helper.GetDataTable();

                dgvResults.DataSource = dt;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void clbColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
            string dbName = cmbDatabases.Text;
            string tableName = cmbTables.Text;

            try
            {
                string sorgu = string.Empty;
                string kolonlar = string.Empty;

                if (clbColumns.CheckedItems.Count > 0)
                {
                    foreach (object item in clbColumns.CheckedItems)
                    {
                        kolonlar += string.Format("[{0}],", item.ToString());
                    }

                    kolonlar = kolonlar.TrimEnd(',');
                }
                else
                {
                    kolonlar = "*";
                }

                SqlHelper helper = new SqlHelper(connectionString);
                helper.Command.CommandText = string.Format("SELECT {0} FROM {1}", kolonlar, tableName);

                txtQuery.Text = helper.Command.CommandText;

                DataTable dt = helper.GetDataTable();

                dgvResults.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
