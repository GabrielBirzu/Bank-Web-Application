using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;

namespace ROBANK.Pages
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        SqlConnection conn = new SqlConnection("Data Source = localhost; Initial Catalog = ROBANK; Integrated Security = True");

        protected void Page_Load(object sender, EventArgs e)
        {

            imgCodSecuritate.ImageUrl = (string)Session["path"];

            lblEroareDate.Visible = false;

        }

        protected void BtnGenerareCod_Click(object sender, EventArgs e)
        {

            int nr = (int)Session["nrCod"];


            if (nr == 6)
            {

                nr = 1;

            }

            imgCodSecuritate.ImageUrl = "~/Media/Coduri_Securitate/cod_" + nr.ToString() + ".png";

            Session["path"] = imgCodSecuritate.ImageUrl;

            nr++;

            Session["nrCod"] = nr;

        }

        protected void BtnContinuare_Click(object sender, EventArgs e)
        {

            if (txtNume.Text == "" || txtPrenume.Text == "" || txtCNP.Text == "" || txtCodSecuritate.Text == "")
            {

                lblEroareDate.Text = "Introduceți toate datele!";

                lblEroareDate.Visible = true;

                return;

            }
            else if (txtCodSecuritate.Text != "")
            {

                if (imgCodSecuritate.ImageUrl.ToString() == "~/Media/Coduri_Securitate/cod_1.png")
                {

                    if (txtCodSecuritate.Text != "mcym8")
                    {

                        txtCodSecuritate.Text = "";

                        lblEroareDate.Text = "Codul introdus eronat!";

                        lblEroareDate.Visible = true;

                        return;

                    }
                    else
                    {

                        Send_Email();

                        Refresh_TextBox();

                    }

                }
                else if (imgCodSecuritate.ImageUrl.ToString() == "~/Media/Coduri_Securitate/cod_2.png")
                {

                    if (txtCodSecuritate.Text != "w434m")
                    {

                        txtCodSecuritate.Text = "";

                        lblEroareDate.Text = "Codul introdus eronat!";

                        lblEroareDate.Visible = true;

                        return;

                    }
                    else
                    {

                        Send_Email();

                        Refresh_TextBox();

                    }

                }
                else if (imgCodSecuritate.ImageUrl.ToString() == "~/Media/Coduri_Securitate/cod_3.png")
                {

                    if (txtCodSecuritate.Text != "6mmxy")
                    {

                        txtCodSecuritate.Text = "";

                        lblEroareDate.Text = "Codul introdus eronat!";

                        lblEroareDate.Visible = true;

                        return;

                    }
                    else
                    {

                        Send_Email();

                        Refresh_TextBox();

                    }

                }
                else if (imgCodSecuritate.ImageUrl.ToString() == "~/Media/Coduri_Securitate/cod_4.png")
                {

                    if (txtCodSecuritate.Text != "rxp4r")
                    {

                        txtCodSecuritate.Text = "";

                        lblEroareDate.Text = "Codul introdus eronat!";

                        lblEroareDate.Visible = true;

                        return;

                    }
                    else
                    {

                        Send_Email();

                        Refresh_TextBox();

                    }

                }
                else if (imgCodSecuritate.ImageUrl.ToString() == "~/Media/Coduri_Securitate/cod_5.png")
                {

                    if (txtCodSecuritate.Text != "een7n")
                    {

                        txtCodSecuritate.Text = "";

                        lblEroareDate.Text = "Codul introdus eronat!";

                        lblEroareDate.Visible = true;

                        return;

                    }
                    else
                    {

                        Send_Email();

                        Refresh_TextBox();

                    }

                }


            }
        }

        protected void Send_Email()
        {

            Session["CNP"] = txtCNP.Text;

            conn.Open();

            string selectDateClient = "SELECT Email " +
                "FROM Clienti " +
                "WHERE Nume = '" + txtNume.Text + "' AND Prenume = '" + txtPrenume.Text + "' AND CNP = '" + txtCNP.Text + "'";

            SqlCommand cmdDateClienti = new SqlCommand(selectDateClient, conn);

            SqlDataReader readerDateClienti = cmdDateClienti.ExecuteReader();

            readerDateClienti.Read();

            string email = readerDateClienti.GetString(0);

            readerDateClienti.Close();

            conn.Close();

            Random random = new Random();

            string numar = random.Next().ToString();

            Session["codEmail"] = numar;

            var fromAddress = new MailAddress("robank.noreply@yahoo.com", "From ROBANK");
            var toAddress = new MailAddress(email, "To " + txtPrenume.Text + " " + txtNume.Text);
            string fromPassword = "icuxpmlsbkgidskh";
            string subject = "Schimbare parolă";
            string body = "Bună, \n \n Codul pentru schimbarea parolei este: " + numar + ". \n \n Mulțumim, \n ROBANK" + " \n \n \n Telefon: 0234222092 \n Adresă: Strada ROBANK, București";

            var smtp = new SmtpClient
            {
                Host = "smtp.mail.yahoo.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }

            Response.Redirect("../Pages/NewPassword.aspx");

        }

        protected void Refresh_TextBox()
        {

            txtCNP.Text = "";

            txtCodSecuritate.Text = "";

            txtNume.Text = "";

            txtPrenume.Text = "";

        }

        protected void BtnRevenire_Click(object sender, EventArgs e)
        {

            Response.Redirect("../Pages/Login.aspx");

        }
    }
}