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

namespace schat
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        //login
        private void button1_Click(object sender, EventArgs e)
        {
             string username = textBox1.Text;
            string password = textBox2.Text;
            string query = "SELECT * FROM users WHERE username=@username";
            SqlCommand cmd = new SqlCommand(query, Utility.con);
            cmd.Parameters.AddWithValue("username", username);
            SqlDataReader reader = cmd.ExecuteReader();

            bool gasit = false;

            while (reader.Read())
            {
                gasit = true; 
                //luam salt-ul din db si hash-uim parola data in input 
                string hash = Utility.ComputeHash(password, reader.GetString(3));
                if (hash == reader.GetString(2)) //daca parola din input face match cu cea din db 
                {
                    Utility.userid = reader.GetInt32(0);
                    Utility.currentusername = reader.GetString(1);
                    //MessageBox.Show("Logat cu succes");
                    Form1 f = new Form1();
                    f.Show();
                    this.Hide();
                    break;
                }
                else
                    MessageBox.Show("Username sau parola incorecte");
            }

            if(!gasit)
                MessageBox.Show("Username sau parola incorecte");

            reader.Close();

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Utility.con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Faculta\An3\MVP\schat\schat\bin\Debug\database.mdf;Integrated Security=True;Connect Timeout=30");
            Utility.con.Open();
        }
        //register
        private void button2_Click(object sender, EventArgs e)
        {
            Register r = new Register();
            r.Show();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
