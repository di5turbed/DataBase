using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DesktopClient
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var apiClient = new ApiClient();
            if (await apiClient.LoginAsync(loginbox.Text, passwordbox.Text))
            {
                var mainForm = new Form1(apiClient);
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Доступ запрещен!");
            }
        }
    }
}
