using Dapper;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace SimpleCalculator
{
    public partial class CalculatorForm : Form
    {
        bool OperateFirstTime = false;
        string Operate = string.Empty;
        string Result = string.Empty;
        int previousResult = 0;
        public CalculatorForm()
        {
            InitializeComponent();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            if(textBoxDisplay.TextLength > 0)
            {
                textBoxDisplay.Text = textBoxDisplay.Text.Remove(textBoxDisplay.TextLength - 1, 1);
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
             
            if (btn.Text.Equals("/") || btn.Text.Equals("*") || btn.Text.Equals("-") || btn.Text.Equals("+"))
            {
                if (!string.IsNullOrEmpty(Result) && !string.IsNullOrEmpty(Operate) && !OperateFirstTime)
                {
                    Compute();
                }

                if (!OperateFirstTime)
                {
                    Result = textBoxDisplay.Text;
                }

                Operate = btn.Text;
                OperateFirstTime = true;
            }

            else if (btn.Text.Equals("="))
            {
                Compute();
            }
            else if (btn.Text.Equals("M+"))
            //button will be able to save the calculated result.
            {
                if (previousResult != 0)
                {
                    try
                    {
                        using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString))
                        {
                            if (db.State == ConnectionState.Closed)
                                db.Open();
                            
                                var val = previousResult;
                                var username = LoginForm.username;
                                db.Execute("update cal_user set lastResult=@val where username=@username", new { val, username });
                                MessageBox.Show(this, "Result " + textBoxDisplay.Text + " is saved.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(this,"Result is 0. Do some calculation.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
            else if (btn.Text.Equals("ML"))
            //button will be able to retrieve a list of his/her previously saved result,
            //and user can select it to be part of the current calculation.
            {
                textBoxDisplay.Clear();
                textBoxDisplay.Text = previousResult.ToString();
            }
            else if (btn.Text.Equals("MC"))
            //button will clear ALL his/her stored result.
            {
                textBoxDisplay.Clear();
                previousResult = 0;
                try
                {
                    using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString))
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();

                        var val = previousResult;
                        var username = LoginForm.username;
                        db.Execute("update cal_user set lastResult=@val where username=@username", new { val, username });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (btn.Text.Equals("Fn"))
            //button show a function for M+, ML, MC.
            {
                MessageBox.Show(this, " M+ button for save your current result.\n ML button for retrive your previously saved result.\n MC button for clear your stored result", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (OperateFirstTime)
                {
                    textBoxDisplay.Clear();
                }

                string oldResult = textBoxDisplay.Text;
                textBoxDisplay.Text = oldResult + btn.Text;
                OperateFirstTime = false;
            }
        }

        private void Compute()
        {
            if (!string.IsNullOrEmpty(Result) && !string.IsNullOrEmpty(Operate))
            {
                int result = 0;
                int textInt = int.Parse(textBoxDisplay.Text);
                try //to catch calculation error e.g number divided by zero.
                {
                    switch (Operate)
                    {
                        case "/":
                            result = int.Parse(Result) / textInt;
                            break;
                        case "*":
                            result = int.Parse(Result) * textInt;
                            break;
                        case "-":
                            result = int.Parse(Result) - textInt;
                            break;
                        case "+":
                            result = int.Parse(Result) + textInt;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Result = string.Empty;
                previousResult = result;
                textBoxDisplay.Text = result.ToString();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString))
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    User obj = db.Query<User>($"select lastResult from cal_user where username = '{LoginForm.username}'", commandType: CommandType.Text).SingleOrDefault();
                    if (obj.lastResult != 0)
                    {
                        textBoxDisplay.Text = obj.lastResult.ToString();//load lastResult
                    }
                    else
                    {
                        textBoxDisplay.Clear();
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
