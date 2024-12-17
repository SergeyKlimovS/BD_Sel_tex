using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BD_Sel_tex
{
    public partial class Login_form : Form
    {
        static public string Loginactive;
        static public string whoIS;
        public Login_form()
        {
            InitializeComponent();
        }
        private void Login_form_Load(object sender, EventArgs e)
        {
            BDConnect.ConnectionBd();
        }
        private void Btn_logIn_Click(object sender, EventArgs e)
        {
            if (Text_login.Text != "" && Text_passwd.Text != "")
            {
                Authorization.Authorizations(Text_login.Text, Text_passwd.Text);

                switch (Authorization.Role)
                {
                    case null:
                        {
                            MessageBox.Show("Такого аккаунта не существует!", "Проверьте данные и попробуйте снова!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        }
                    case "admin":
                        {
                            Loginactive = Text_login.Text;
                            whoIS = "Администратор";
                            Authorization.User = Text_login.Text;
                            string user = Authorization.AuthorizationsName(Text_login.Text);
                            Authorization.User = user;
                            MessageBox.Show(user + ", Добро пожаловать в меню Администратора!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Hide();
                            Admin_Form mainFormsAdmin = new Admin_Form();
                            mainFormsAdmin.Show();
                            break;
                        }
                    case "users":
                        {
                            Loginactive = Text_login.Text;
                            whoIS = "Пользователь";
                            Authorization.User = Text_login.Text;
                            string user = Authorization.AuthorizationsName(Text_login.Text);
                            Authorization.User = user;
                            MessageBox.Show(user + ", Добро пожаловать в меню Пользователя!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Hide();
                            Admin_Form mainFormsUser = new Admin_Form();
                            mainFormsUser.Show();
                            break;
                        }
                }
            }
            else
            {
                MessageBox.Show("Поля пустые..", "Заполните поля", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

      
    }
}
