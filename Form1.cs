using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleCalculator
{
    public partial class Form1 : Form
    {
        bool OperateFirstTime = false;
        string Operate = string.Empty;
        string Result = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonMplus_Click(object sender, EventArgs e)
        {
            //to be added
        }

        private void buttonML_Click(object sender, EventArgs e)
        {
            //to be added
        }

        private void buttonMC_Click(object sender, EventArgs e)
        {
            //to be added
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

                Result = string.Empty;
                textBoxDisplay.Text = result.ToString();
            }
        }
    }
}
