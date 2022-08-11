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

namespace ROBANK
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        SqlConnection conn = new SqlConnection("Data Source = localhost; Initial Catalog = ROBANK; Integrated Security = True");

        protected void Page_Load(object sender, EventArgs e)
        {

            Session["nrCod"] = 2;

            Session["path"] = "~/Media/Coduri_Securitate/cod_1.png";

        }

        public void butonLogare_Click(object sender, EventArgs e)
        {
            conn.Open();

            try
            {
                wrongLbl.Visible = false;

                string sql = "SELECT Cod_Client, LEFT(Nume,1) AS initialaNume, LEFT(Prenume,1) AS initialaPrenume, Parola FROM Clienti WHERE Cod_Client = LEFT('" + idLogare.Text + "',7)";
                 
                SqlCommand cmd = new SqlCommand(sql, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    reader.Read();

                    string codClient = reader[0].ToString();

                    string numeClient = reader[1].ToString();

                    string prenumeClient = reader[2].ToString();

                    string parola = reader[3].ToString();

                    if (idLogare.Text == codClient + numeClient + prenumeClient && VerifyHash(parolaLogare.Text, parola))
                    {

                        Session["codClient"] = codClient;

                        Session["nrInternet"] = 0;

                        Session["nrNumerar"] = 0;

                        Session["nrTranzactii"] = 0;

                        Session["nrPOS"] = 0;


                        Session["nrInternetCredit"] = 0;

                        Session["nrNumerarCredit"] = 0;

                        Session["nrTranzactiiCredit"] = 0;

                        Session["nrPOSCredit"] = 0;


                        Response.Redirect("../Pages/Menu.aspx");

                    }

                    else
                    {

                        wrongLbl.Visible = true;

                        wrongLbl.Text = "Date incorecte!";

                        idLogare.Text = "";
                        parolaLogare.Text = "";

                    }

                    reader.Close();

                }
                
            }

            catch
            {

                idLogare.Text = "";
                parolaLogare.Text = "";

                wrongLbl.Visible = true;

                wrongLbl.Text = "Date incorecte!";

            }

            conn.Close();

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

        private bool VerifyHash(string str, string referenceStrHash)
        {
            return ComputeHash(str) == referenceStrHash;
        }

    }
}