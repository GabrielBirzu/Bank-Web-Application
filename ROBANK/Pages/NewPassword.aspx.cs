using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;


namespace ROBANK.Pages
{
    public partial class WebForm3 : System.Web.UI.Page
    {

        SqlConnection conn = new SqlConnection("Data Source = localhost; Initial Catalog = ROBANK; Integrated Security = True");

        protected void Page_Load(object sender, EventArgs e)
        {

            lblEroareDateParolaNoua.Visible = false;

        }

        protected void BtnFinalizareSchimbareParola_Click(object sender, EventArgs e)
        {
            
            int codEmail = Convert.ToInt32(Session["codEmail"]);

            if (txtCodEmail.Text == "" || txtParolaNoua.Text == "" || txtVerificareParolaNoua.Text == "")
            {

                lblEroareDateParolaNoua.Visible = true;

                lblEroareDateParolaNoua.Text = "Introduceți toate datele!";

            }
            else if (txtParolaNoua.Text != txtVerificareParolaNoua.Text)
            {

                lblEroareDateParolaNoua.Visible = true;

                lblEroareDateParolaNoua.Text = "Parolele nu corespund!";

            }
            else if (txtCodEmail.Text != codEmail.ToString())
            {

                lblEroareDateParolaNoua.Visible = true;

                lblEroareDateParolaNoua.Text = "Codul nu este corect!";

            }
            else
            {

                string parolaCriptata = ComputeHash(txtParolaNoua.Text);

                string CNP = (string)Session["CNP"];

                conn.Open();

                string updateParola = "UPDATE Clienti " +
                    "SET Parola = '" + parolaCriptata + "' " +
                    "WHERE CNP = '" + CNP + "'";

                SqlCommand cmdUpdateParola = new SqlCommand(updateParola, conn);

                cmdUpdateParola.ExecuteNonQuery();

                conn.Close();

                Response.Redirect("../Pages/Login.aspx");

            }


        }

        private string ComputeHash(string str)
        {
            SHA256 sha256 = SHA256.Create();

            byte[] strHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));

            var strBuilder = new StringBuilder();
            for (int i = 0; i < strHash.Length; i++)
            {
                strBuilder.Append(strHash[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

    }
}