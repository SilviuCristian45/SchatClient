using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace schat
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        //register
        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;
            string repeatPass = textBox3.Text;

            if(password == repeatPass)
            {
                if (validatePassword(password))
                {
                    string query = "INSERT INTO users VALUES (@username,@password,@salt)";
                    SqlCommand cmd = new SqlCommand(query, Utility.con);

                    String passwordSalt = Utility.GenerateSalt();
                    String passwordHash = Utility.ComputeHash(password, passwordSalt);

                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", passwordHash);
                    cmd.Parameters.AddWithValue("salt",     passwordSalt);

                    try 
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("inregistrare cu succes");
                    }
                    catch(SqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Parola trebuie sa aiba minim 8 caractere, o litera, si o cifra");
                }
            }
            else
            {
                MessageBox.Show("Passwords do not match");
            }
        }
        //validate password
        bool validatePassword(string password)
        {
            Regex regex = new Regex(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{8,32}$");
            return regex.IsMatch(password);
        }

        
    }
}
