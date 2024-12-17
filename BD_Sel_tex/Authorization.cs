using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BD_Sel_tex
{

    public class Authorization
    {
        static public string Role, User;
        static public void Authorizations(string login, string passwd)
        {
            try
            {


                BDConnect.sqlCommand.CommandText = @"SELECT name_role 
                       FROM sp_role 
                       INNER JOIN Users ON Users.id_role = sp_role.id_role 
                       WHERE login_user = @login AND passwd_user = @passwd";

                BDConnect.sqlCommand.Parameters.Clear(); // Очищаем параметры перед добавлением новых.
                BDConnect.sqlCommand.Parameters.AddWithValue("@login", login);
                BDConnect.sqlCommand.Parameters.AddWithValue("@passwd", passwd);

                object result = BDConnect.sqlCommand.ExecuteScalar();

                if (result != null)
                {
                    Role = result.ToString();
                    User = login;
                }
                else
                {
                    Role = null;
                }
            }
            catch
            {
                Role = User = null;
                MessageBox.Show("Ошибка при авторизации!");
            }
        }

        static public string AuthorizationsName(string login)
        {
            try
            {
                BDConnect.sqlCommand.CommandText = @"SELECT login_user FROM Users WHERE login_user = '" + login + "'";
                Object result = BDConnect.sqlCommand.ExecuteScalar();
                login = result.ToString();
                return login;
            }
            catch
            {
                return null;
            }
        }
    }
}
