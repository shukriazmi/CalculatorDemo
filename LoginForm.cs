using Dapper;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace SimpleCalculator
{
    public partial class LoginForm : Form
    {
        public static string username;
        public LoginForm()
        { 
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString))
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    //Excute sql query, then map data return from sql to User class
                    if(buttonLogin.Text == "LOGIN")
                    {
                        User obj = db.Query<User>($"select * from cal_user where username = '{textBoxUsername.Text}'", commandType: CommandType.Text).SingleOrDefault();
                        if (obj != null)//username exist
                        {
                            using (CalculatorForm frm = new CalculatorForm())//Open calculator form and hide login form
                            {
                                username = textBoxUsername.Text;
                                this.Hide();
                                frm.ShowDialog();
                            }
                        }
                        else
                        {
                            if (MessageBox.Show("Your username don't exist. Please create new username", "Message",
                                MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                labelUsername.Text = "Enter new username";
                                labelUsername.Left = 110;
                                buttonLogin.Text = "REGISTER";
                                buttonLogin.Left = 137;
                            }
                        }

                    }
                    else
                    {
                        var val = textBoxUsername.Text;
                        db.Execute("insert into cal_user(username) values (@val)", new { val });
                        MessageBox.Show(this, "Welcome "+textBoxUsername.Text, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        using (CalculatorForm frm = new CalculatorForm())//Open calculator form and hide register form
                        {
                            username = textBoxUsername.Text;
                            this.Hide();
                            frm.ShowDialog();
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
               MessageBox.Show(this, ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
