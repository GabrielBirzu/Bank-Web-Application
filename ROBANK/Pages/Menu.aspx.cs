using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Xml;

namespace ROBANK.Pages
{

    public partial class WebForm2 : System.Web.UI.Page
    {

        SqlConnection conn = new SqlConnection("Data Source = localhost; Initial Catalog = ROBANK; Integrated Security = True; MultipleActiveResultSets = True");

        protected void Page_Load(object sender, EventArgs e)
        {

            LoadProfileImage();

            NameLabel();

            if (!IsPostBack)
            {

                Accounts();

            }

            Calcul_Depozit();

            DropDownList_Change();

            Populate_DgvConturi();

            Populate_DgvConturiEconomii();

            Populate_DgvDepozite();

            Populate_DgvDepoziteInactive();

            Populate_DgvCarduriDebit();

            Populate_DgvCarduriCredit();

            Populate_DgvCredite();

            Verificare_Limite();

            Plati_Programate();

            Actualizare_Credite();

            Hide_Div();

        }

        protected void Hide_Div()
        {

            dataContainer.Visible = false;

            detaliiDepozite.Visible = false;

            detaliiEconomii.Visible = false;

            dateDepozite.Visible = false;

            stergereDepozit.Visible = false;

            desfiintareEconomii.Visible = false;

            deschidereDepozit.Visible = false;

            deschideCont.Visible = false;

            cardurileMele.Visible = false;

            vizualizareCardDebit.Visible = false;

            limiteDebit.Visible = false;

            istoricTranzactiiDebit.Visible = false;

            istoricTranzactiiCont.Visible = false;

            linieCredit.Visible = false;

            limiteCredit.Visible = false;

            vizualizareCardCredit.Visible = false;

            platiIntreConturi.Visible = false;

            platiCont.Visible = false;

            conversieBani.Visible = false;

            platiProgramate.Visible = false;

            adaugarePlatiProgramate.Visible = false;

            beneficiariPredefiniti.Visible = false;

            adaugareBeneficiarPredefinit.Visible = false;

            contractareImprumut.Visible = false;

            vizualizareImprumuturi.Visible = false;

            detaliiImprumut.Visible = false;

            statisticiDate.Visible = false;

            txtAnStatistica.Text = DateTime.Now.Year.ToString();

        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            string serverPath = Server.MapPath("./");
            string profileImageFileName = file.PostedFile.FileName;
            profileImageFileName = Path.GetFileName(profileImageFileName);
            string profileImageFilePath = serverPath + profileImageFileName;

            System.Diagnostics.Debug.Assert(Directory.Exists(serverPath), serverPath + "  folder does not exist.");

            if (!File.Exists(profileImageFilePath))
            {
                file.PostedFile.SaveAs(profileImageFilePath);
            }

            SaveProfileImageInDB(profileImageFilePath);

            File.Delete(profileImageFilePath);
        }

        private void SaveProfileImageInDB(string profileImageFilePath)
        {

            conn.Open();

            string sql = "UPDATE Clienti SET Poza = BulkColumn FROM Openrowset(Bulk '" + profileImageFilePath + "', Single_Blob) as image";

            SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.ExecuteNonQuery();

            conn.Close();

            LoadProfileImage();

        }

        private void LoadProfileImage()
        {

            conn.Open();

            string sql = "SELECT Poza FROM Clienti";

            SqlCommand cmd = new SqlCommand(sql, conn);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    profileImg.Visible = true;

                    byte[] bytePhoto = (byte[])reader[0];

                    profileImg.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(bytePhoto);
                }

            }

            conn.Close();

        }


        private void NameLabel()
        {

            conn.Open();

            string codClient = (string)Session["codClient"];

            string sql = "SELECT Nume, Prenume FROM Clienti WHERE Cod_Client = '" + codClient + "'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataReader reader = cmd.ExecuteReader();

            reader.Read();

            nameLbl.Text = reader[0].ToString() + " " + reader[1].ToString();

            conn.Close();

        }

        private void Accounts()
        {

            conn.Open();

            string codClient = (string)Session["codClient"];

            string sql = "SELECT * FROM Conturi WHERE Cod_Client = '" + codClient + "'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            accountDl.DataSource = dt;

            accountDl.DataTextField = "IBAN_Cont";

            accountDl.DataBind();

            conn.Close();

        }

        private void DropDownList_Change()
        {

            conn.Open();

            string sql = "SELECT Sold, Valuta FROM Conturi WHERE IBAN_Cont LIKE '" + accountDl.SelectedItem.Text + "'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {

                    txtSold.Text = String.Format("{0:n}", reader[0]);

                    txtValuta.Text = reader[1].ToString();

                }

            }

            gvConturi.SelectedIndex = -1;

            conn.Close();

        }

        protected void FirstBtn1_Click(object sender, EventArgs e)
        {

            dataContainer.Visible = true;

            Session["index"] = -1;

            btnCopiere.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnCopiere.ForeColor = System.Drawing.Color.White;


            btnCopiereEconomii.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnCopiereEconomii.ForeColor = System.Drawing.Color.White;


            gvConturi.SelectedIndex = -1;

            gvConturiEconomii.SelectedIndex = -1;

        }

        private void Populate_DgvConturi()
        {

            conn.Open();

            string codClient = (string)Session["codClient"];

            string sql = "SELECT IBAN_Cont AS IBAN, Valuta AS Valută, FORMAT(Sold,'N') AS Sold FROM Conturi WHERE Cod_Client = '" + codClient + "' " +
                "AND Tip_Cont = 'Cont curent'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            gvConturi.DataSource = dt;

            gvConturi.DataBind();

            conn.Close();

        }


        private void Populate_DgvDepozite()
        {

            conn.Open();

            string codClient = (string)Session["codClient"];

            string sql = "SELECT IBAN_Depozit AS [ID Depozit], FORMAT(Depozite.Data_Creare, 'dd/MM/yyyy') AS [Dată activare], Valuta AS Valută, FORMAT(Valoare_Depozit,'N') AS [Valoare depozit]" +
                "FROM Conturi INNER JOIN Depozite ON Conturi.IBAN_Cont=Depozite.IBAN_Cont " +
                "WHERE Cod_Client = '" + codClient + "' AND Depozit_Activ = 'True' AND Depozite.Data_Creare <= GETDATE() ";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            gvDepozite.DataSource = dt;

            gvDepozite.DataBind();

            conn.Close();

        }

        private void Populate_DgvDepoziteInactive()
        {

            conn.Open();

            string codClient = (string)Session["codClient"];

            string sql = "SELECT IBAN_Depozit AS [ID Depozit], FORMAT(Depozite.Data_Creare, 'dd/MM/yyyy') AS [Dată activare], Valuta AS Valută, FORMAT(Valoare_Depozit,'N') AS [Valoare depozit]" +
                "FROM Conturi INNER JOIN Depozite ON Conturi.IBAN_Cont=Depozite.IBAN_Cont " +
                "WHERE Cod_Client = '" + codClient + "' AND Depozit_Activ = 'True' AND Depozite.Data_Creare > GETDATE() ";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            gvDepoziteInactive.DataSource = dt;

            gvDepoziteInactive.DataBind();

            conn.Close();

        }

        private void Populate_DgvConturiEconomii()
        {

            conn.Open();

            string codClient = (string)Session["codClient"];

            string sql = "SELECT Tip_Cont AS [Tip Cont], IBAN_Cont AS IBAN, Valuta AS Valută, FORMAT(Sold,'N') AS Sold FROM Conturi WHERE Cod_Client = '" + codClient + "' " +
                "AND Tip_Cont = 'Cont economii'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            gvConturiEconomii.DataSource = dt;

            gvConturiEconomii.DataBind();

            conn.Close();

        }

        protected void GvConturi_SelectedIndexChanged(object sender, EventArgs e)
        {

            dataContainer.Visible = true;

            int index = gvConturi.SelectedRow.RowIndex;

            Session["index"] = index;


            btnCopiere.Text = "Copiază IBAN";

            btnCopiere.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnCopiere.ForeColor = System.Drawing.Color.White;


            btnCopiereEconomii.Text = "Copiază IBAN";

            btnCopiereEconomii.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnCopiereEconomii.ForeColor = System.Drawing.Color.White;


            gvConturiEconomii.SelectedIndex = -1;

        }

        [Obsolete]
        protected void GvConturi_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.DataItemIndex == -1)
                return;
            e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';");

            e.Row.Attributes.Add("onclick", this.GetPostBackClientEvent(gvConturi, "Select$" + e.Row.RowIndex.ToString()));

        }

        [Obsolete]
        protected void BtnCopiere_Click(object sender, EventArgs e)
        {

            if (Convert.ToInt32(Session["index"]) == -1)
            {

                dataContainer.Visible = true;

                btnCopiere.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

                btnCopiere.ForeColor = System.Drawing.Color.White;

                btnCopiere.Text = "Selectează cont!";

                return;

            }

            Thread th;

            th = new Thread(new System.Threading.ThreadStart(ClipboardIBAN))
            {
                ApartmentState = ApartmentState.STA
            };

            th.Start();

            int index = (int)Session["index"];

            dataContainer.Visible = true;

            btnCopiere.BackColor = System.Drawing.Color.White;

            btnCopiere.ForeColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnCopiere.Text = "IBAN Copiat";

            gvConturi.SelectedIndex = index;

        }

        private void ClipboardIBAN()
        {

            int index = (int)Session["index"];

            if (index == -1)
            {

                return;

            }

            Clipboard.SetText(gvConturi.Rows[index].Cells[0].Text);

        }

        private void ClipboardEconomii()
        {

            int index = (int)Session["index"];

            if (index == -1)
            {

                return;

            }

            Clipboard.SetText(gvConturiEconomii.Rows[index].Cells[1].Text);

        }

        protected void BtnDetaliiDepozit_Click(object sender, EventArgs e)
        {

            if (Convert.ToInt32(Session["index"]) == -1)
            {

                dateDepozite.Visible = true;

                btnDetaliiDepozit.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

                btnDetaliiDepozit.ForeColor = System.Drawing.Color.White;

                btnDetaliiDepozit.Text = "Selectați depozit!";

                return;

            }

            detaliiDepozite.Visible = true;

            int index = gvDepozite.SelectedRow.RowIndex;

            conn.Open();

            string sql = "SELECT Perioada_Dobanda, (Valoare_Depozit + Suma_Dobanda) AS Sold, Valuta, Incasare_Scadenta, Suma_Dobanda, FORMAT(Data_Scadenta,'dd/MM/yyyy'), FORMAT(Data_Prelungire,'dd/MM/yyyy') " +
                "FROM Conturi INNER JOIN (Depozite INNER JOIN Dobanda_Depozit ON Depozite.ID_Dobanda=Dobanda_Depozit.ID_Dobanda) ON Conturi.IBAN_Cont=Depozite.IBAN_Cont " +
                "WHERE IBAN_Depozit = '" + gvDepozite.Rows[index].Cells[0].Text + "'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {

                    txtPerioadaDepozit.Text = reader[0].ToString() + " LUNI";

                    txtSoldDepozit.Text = String.Format("{0:n}", reader[1]);

                    txtValutaDepozit.Text = reader[2].ToString();

                    txtRataDobanda.Text = reader[3].ToString() + " %";

                    txtSumaDobanda.Text = reader[4].ToString();

                    txtDataScadentaDepozit.Text = reader[5].ToString();

                    txtDataPrelungireDepozit.Text = reader[6].ToString();

                }

            }

            btnCopiere.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnCopiere.ForeColor = System.Drawing.Color.White;

            conn.Close();

        }

        protected void BtnStergereDepozit_Click(object sender, EventArgs e)
        {

            if (Convert.ToInt32(Session["index"]) == -1)
            {

                dateDepozite.Visible = true;

                btnStergereDepozit.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

                btnStergereDepozit.ForeColor = System.Drawing.Color.White;

                btnStergereDepozit.Text = "Selectați depozit!";

                return;

            }

            stergereDepozit.Visible = true;

            int index = gvDepozite.SelectedRow.RowIndex;

            conn.Open();

            string sql = "SELECT Valoare_Depozit, Suma_Dobanda, FORMAT(Data_Scadenta,'dd/MM/yyyy'), Depozite.IBAN_Cont, Valuta " +
                "FROM Conturi INNER JOIN (Depozite INNER JOIN Dobanda_Depozit ON Depozite.ID_Dobanda=Dobanda_Depozit.ID_Dobanda) ON Conturi.IBAN_Cont=Depozite.IBAN_Cont " +
                "WHERE IBAN_Depozit = '" + gvDepozite.Rows[index].Cells[0].Text + "'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {

                    lblMesaj.Text = "Urmează să vă desființați depozitul " + "<b>" + gvDepozite.Rows[index].Cells[0].Text + "</b>" +
                        ", având scadența la data de " + "<b>" + reader[2].ToString() + "</b>" + ". Dacă continuați, se va transfera " +
                        " în contul curent " + "<b>" + reader[3].ToString() + "</b>" + " suma de " + "<b>" + String.Format("{0:n}", reader[0]) + "</b>" + " " + "<b>" + reader[4].ToString() + "</b>" +
                        ", fără dobânda acumulată în valoare de " + "<b>" + String.Format("{0:n}", reader[1]) + "</b>" + " " + "<b>" + reader[4].ToString() + "</b>" + ". Sunteți sigur că vreți" +
                        " să continuați?";

                }

            }

            conn.Close();

        }

        protected void GvDepozite_SelectedIndexChanged(object sender, EventArgs e)
        {

            dateDepozite.Visible = true;

            int index = gvDepozite.SelectedRow.RowIndex;

            Session["index"] = index;


            btnDetaliiDepozit.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnDetaliiDepozit.ForeColor = System.Drawing.Color.White;

            btnDetaliiDepozit.Text = "Detalii depozit";


            btnStergereDepozit.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnStergereDepozit.ForeColor = System.Drawing.Color.White;

            btnStergereDepozit.Text = "Desființare";

        }


        [Obsolete]
        protected void GvDepozite_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.DataItemIndex == -1)
                return;
            e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';");

            e.Row.Attributes.Add("onclick", this.GetPostBackClientEvent(gvDepozite, "Select$" + e.Row.RowIndex.ToString()));

        }

        protected void BtnDepozitRevenire_Click(object sender, EventArgs e)
        {

            detaliiDepozite.Visible = false;

            dateDepozite.Visible = true;


        }

        protected void BtnAnulareDesfiintare_Click(object sender, EventArgs e)
        {

            stergereDepozit.Visible = false;

            dateDepozite.Visible = true;

            cbAcordDesfiintare.Checked = false;

        }

        protected void BtnContinuareDesfiintare_Click(object sender, EventArgs e)
        {

            if (cbAcordDesfiintare.Checked == false)
            {

                lblMesajEroare.Text = "Bifați căsuta de mai sus dacă doriți să continuați!";

                stergereDepozit.Visible = true;

            }
            else
            {

                int index = gvDepozite.SelectedRow.RowIndex;

                conn.Open();

                string updateDepozit = "UPDATE Depozite " +
                    "SET Depozit_Activ = 'False' " +
                    "WHERE IBAN_Depozit = '" + gvDepozite.Rows[index].Cells[0].Text + "'";

                SqlCommand cmdUpdateDepozit = new SqlCommand(updateDepozit, conn);

                string updateConturi = "UPDATE Conturi " +
                    "SET Sold = Sold + Valoare_Depozit " +
                    "FROM Conturi INNER JOIN(Depozite INNER JOIN Dobanda_Depozit ON Depozite.ID_Dobanda = Dobanda_Depozit.ID_Dobanda) ON Conturi.IBAN_Cont = Depozite.IBAN_Cont " +
                    "WHERE IBAN_Depozit = '" + gvDepozite.Rows[index].Cells[0].Text + "'";

                SqlCommand cmdUpdateConturi = new SqlCommand(updateConturi, conn);

                cmdUpdateConturi.ExecuteNonQuery();

                cmdUpdateDepozit.ExecuteNonQuery();

                stergereDepozit.Visible = false;

                cbAcordDesfiintare.Checked = false;

                dataContainer.Visible = true;

                gvDepozite.SelectedIndex = -1;


                string sql = "SELECT Sold, Valuta FROM Conturi WHERE IBAN_Cont LIKE '" + accountDl.SelectedItem.Text + "'";

                SqlCommand cmd = new SqlCommand(sql, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        txtSold.Text = String.Format("{0:n}", reader[0]);

                        txtValuta.Text = reader[1].ToString();

                    }

                }

                gvConturi.SelectedIndex = -1;

                conn.Close();

                Populate_DgvDepozite();

            }

        }

        protected void Calcul_Depozit()
        {

            conn.Open();

            string codClient = (string)Session["codClient"];

            string cmdImpozit = "SELECT Suma_Dobanda * (Valoare_Comision/100) " +
                "FROM Conturi INNER JOIN (Depozite INNER JOIN (Comisioane_Depozit INNER JOIN Comisioane ON Comisioane_Depozit.ID_Comision=Comisioane.ID_Comision) ON Depozite.Nr_Depozit=Comisioane_Depozit.Nr_Depozit) ON Conturi.IBAN_Cont=Depozite.IBAN_Cont " +
                "WHERE Conturi.Cod_Client = '" + codClient + "'";

            string cmdVerificare = "SELECT IBAN_Depozit, FORMAT(Data_Scadenta,'dd/MM/yyyy'), Depozit_Activ, Capitalizare, Prelungire, Perioada_Dobanda, FORMAT(Data_Prelungire,'dd/MM/yyyy') " +
                "FROM Conturi INNER JOIN(Depozite INNER JOIN Dobanda_Depozit ON Depozite.ID_Dobanda = Dobanda_Depozit.ID_Dobanda) ON Conturi.IBAN_Cont = Depozite.IBAN_Cont " +
                "WHERE Conturi.Cod_Client = '" + codClient + "'";

            SqlCommand cmdV = new SqlCommand(cmdVerificare, conn);

            SqlCommand cmdI = new SqlCommand(cmdImpozit, conn);

            SqlDataAdapter adapterV = new SqlDataAdapter(cmdV);

            SqlDataAdapter adapterI = new SqlDataAdapter(cmdI);

            DataTable dtV = new DataTable();

            DataTable dtI = new DataTable();

            adapterI.Fill(dtI);

            adapterV.Fill(dtV);

            int i = 0;

            int rowsNumber = dtV.Rows.Count;

            while (i < rowsNumber)
            {

                string columnIdDepozit = dtV.Rows[i][0].ToString();

                string columnDataScadenta = dtV.Rows[i][1].ToString();

                string columnDepozitActiv = dtV.Rows[i][2].ToString();

                string columnCapitalizare = dtV.Rows[i][3].ToString();

                string columnPrelungire = dtV.Rows[i][4].ToString();

                string columnPerioadaDobanda = dtV.Rows[i][5].ToString();

                string columnDataPrelungire = dtV.Rows[i][6].ToString();

                string columnDobandaImpozitata = dtI.Rows[i][0].ToString();

                if (columnDepozitActiv == "True")
                {

                    if (DateTime.Now < DateTime.Parse(columnDataScadenta))
                    {

                        if (columnDataPrelungire == "")
                        {

                            string updateDepozit = "UPDATE Depozite " +
                                "SET Suma_Dobanda = ((Valoare_Depozit * (Incasare_Scadenta / 100)) / 360) * DATEDIFF(day, Depozite.Data_Creare, GETDATE()) " +
                                "FROM Conturi INNER JOIN(Depozite INNER JOIN Dobanda_Depozit ON Depozite.ID_Dobanda = Dobanda_Depozit.ID_Dobanda) ON Conturi.IBAN_Cont = Depozite.IBAN_Cont " +
                                "WHERE IBAN_Depozit = '" + columnIdDepozit + "'";

                            SqlCommand cmdUpdate = new SqlCommand(updateDepozit, conn);

                            cmdUpdate.ExecuteNonQuery();

                        }
                        else
                        {

                            string updateDepozit = "UPDATE Depozite " +
                              "SET Suma_Dobanda = ((Valoare_Depozit * (Incasare_Scadenta / 100)) / 360) * DATEDIFF(day, Data_Prelungire, GETDATE()) " +
                              "FROM Conturi INNER JOIN(Depozite INNER JOIN Dobanda_Depozit ON Depozite.ID_Dobanda = Dobanda_Depozit.ID_Dobanda) ON Conturi.IBAN_Cont = Depozite.IBAN_Cont " +
                              "WHERE IBAN_Depozit = '" + columnIdDepozit + "'";

                            SqlCommand cmdUpdate = new SqlCommand(updateDepozit, conn);

                            cmdUpdate.ExecuteNonQuery();

                        }

                    }
                    else if (DateTime.Today == DateTime.Parse(columnDataScadenta) && columnPrelungire == "True" && columnCapitalizare == "True")
                    {

                        string updateDepozit = "UPDATE Depozite " +
                            "SET Valoare_Depozit = Valoare_Depozit + Suma_Dobanda - (Suma_Dobanda * (Valoare_Comision/100)), Suma_Dobanda = 0, Data_Scadenta = DATEADD(month, CAST('" + columnPerioadaDobanda + "' AS int), Data_Scadenta), Data_Prelungire = '" + DateTime.Now.ToString("yyyy'-'MM'-'dd") + "'" +
                            "FROM Dobanda_Depozit INNER JOIN (Depozite INNER JOIN (Comisioane_Depozit INNER JOIN Comisioane ON Comisioane_Depozit.ID_Comision=Comisioane.ID_Comision) ON Depozite.Nr_Depozit=Comisioane_Depozit.Nr_Depozit) ON Dobanda_Depozit.ID_Dobanda=Depozite.ID_Dobanda " +
                            "WHERE IBAN_Depozit = '" + columnIdDepozit + "'";

                        SqlCommand cmdUpdate = new SqlCommand(updateDepozit, conn);

                        cmdUpdate.ExecuteNonQuery();

                    }
                    else if (DateTime.Today == DateTime.Parse(columnDataScadenta) && columnPrelungire == "True" && columnCapitalizare == "False")
                    {

                        string updateCont = "UPDATE Conturi " +
                            "SET Sold = Sold + Suma_Dobanda - '" + columnDobandaImpozitata + "' " +
                            "FROM Conturi INNER JOIN(Depozite INNER JOIN Dobanda_Depozit ON Depozite.ID_Dobanda = Dobanda_Depozit.ID_Dobanda) ON Conturi.IBAN_Cont = Depozite.IBAN_Cont " +
                            "WHERE IBAN_Depozit = '" + columnIdDepozit + "'";

                        string updateDepozit = "UPDATE Depozite " +
                            "SET Suma_Dobanda = 0, Data_Scadenta = DATEADD(month, CAST('" + columnPerioadaDobanda + "' AS int), Data_Scadenta), Data_Prelungire = '" + DateTime.Now.ToString("yyyy'-'MM'-'dd") + "'" +
                            "FROM Conturi INNER JOIN(Depozite INNER JOIN Dobanda_Depozit ON Depozite.ID_Dobanda = Dobanda_Depozit.ID_Dobanda) ON Conturi.IBAN_Cont = Depozite.IBAN_Cont " +
                            "WHERE IBAN_Depozit = '" + columnIdDepozit + "'";

                        SqlCommand cmdUpdateCont = new SqlCommand(updateCont, conn);

                        SqlCommand cmdUpdateDepozit = new SqlCommand(updateDepozit, conn);

                        cmdUpdateCont.ExecuteNonQuery();

                        cmdUpdateDepozit.ExecuteNonQuery();

                    }
                    else if (DateTime.Today == DateTime.Parse(columnDataScadenta) && columnPrelungire == "False" && columnCapitalizare == "False")
                    {

                        string updateCont = "UPDATE Conturi " +
                            "SET Sold = Sold + Valoare_Depozit + Suma_Dobanda - '" + columnDobandaImpozitata + "' " +
                            "FROM Conturi INNER JOIN(Depozite INNER JOIN Dobanda_Depozit ON Depozite.ID_Dobanda = Dobanda_Depozit.ID_Dobanda) ON Conturi.IBAN_Cont = Depozite.IBAN_Cont " +
                            "WHERE IBAN_Depozit = '" + columnIdDepozit + "'";

                        string updateDepozit = "UPDATE Depozite " +
                            "SET Suma_Dobanda = 0, Depozit_Activ = 'False' " +
                            "FROM Conturi INNER JOIN(Depozite INNER JOIN Dobanda_Depozit ON Depozite.ID_Dobanda = Dobanda_Depozit.ID_Dobanda) ON Conturi.IBAN_Cont = Depozite.IBAN_Cont " +
                            "WHERE IBAN_Depozit = '" + columnIdDepozit + "'";

                        SqlCommand cmdUpdateCont = new SqlCommand(updateCont, conn);

                        SqlCommand cmdUpdateDepozit = new SqlCommand(updateDepozit, conn);

                        cmdUpdateCont.ExecuteNonQuery();

                        cmdUpdateDepozit.ExecuteNonQuery();

                    }

                }

                i++;

            }

            conn.Close();

        }

        protected void SecondBtn1_Click(object sender, EventArgs e)
        {

            dateDepozite.Visible = true;

            Session["index"] = -1;

            gvDepozite.SelectedIndex = -1;

            btnDetaliiDepozit.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnDetaliiDepozit.ForeColor = System.Drawing.Color.White;

            btnDetaliiDepozit.Text = "Detalii depozit";


            btnStergereDepozit.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnStergereDepozit.ForeColor = System.Drawing.Color.White;

            btnStergereDepozit.Text = "Desființare";

        }

        protected void BtnDetaliiEconomii_Click(object sender, EventArgs e)
        {

            detaliiEconomii.Visible = true;

            int index = gvConturiEconomii.SelectedRow.RowIndex;

            conn.Open();

            string sql = "SELECT FORMAT(Data_Creare,'dd/MM/yyyy'), Sold, Valuta, Incasare_Lunara, CAST(((Sold * (Incasare_Lunara/100))/360) * DATEDIFF(day,Data_Creare, GETDATE()) AS DECIMAL(18,2)) " +
                "FROM Conturi INNER JOIN Dobanda_Conturi ON Conturi.ID_Dobanda=Dobanda_Conturi.ID_Dobanda " +
                "WHERE IBAN_Cont = '" + gvConturiEconomii.Rows[index].Cells[1].Text + "'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {

                    txtDataCreare.Text = reader[0].ToString();

                    txtSoldEconomii.Text = String.Format("{0:n}", Convert.ToDouble(reader[1]) + Convert.ToDouble(reader[4]));

                    txtValutaEconomii.Text = reader[2].ToString();

                    txtDobandaEconomii.Text = reader[3].ToString() + " %";

                    txtSumaDobandaEconomii.Text = reader[4].ToString();

                }

            }

            btnCopiereEconomii.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnCopiereEconomii.ForeColor = System.Drawing.Color.White;

            conn.Close();

        }

        [Obsolete]
        protected void BtnCopiereEconomii_Click(object sender, EventArgs e)
        {

            if (Convert.ToInt32(Session["index"]) == -1)
            {

                dataContainer.Visible = true;

                btnCopiereEconomii.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

                btnCopiereEconomii.ForeColor = System.Drawing.Color.White;

                btnCopiereEconomii.Text = "Selectează cont!";

                return;

            }

            Thread th;

            th = new Thread(new System.Threading.ThreadStart(ClipboardEconomii))
            {
                ApartmentState = ApartmentState.STA
            };

            th.Start();

            int index = (int)Session["index"];

            dataContainer.Visible = true;

            btnCopiereEconomii.BackColor = System.Drawing.Color.White;

            btnCopiereEconomii.ForeColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnCopiereEconomii.Text = "IBAN Copiat";

            gvConturiEconomii.SelectedIndex = -1;

            gvConturiEconomii.SelectedIndex = index;

        }

        protected void GvConturiEconomii_SelectedIndexChanged(object sender, EventArgs e)
        {

            dataContainer.Visible = true;

            int index = gvConturiEconomii.SelectedRow.RowIndex;

            Session["index"] = index;

            btnCopiereEconomii.Text = "Copiază IBAN";

            btnCopiereEconomii.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnCopiereEconomii.ForeColor = System.Drawing.Color.White;


            btnCopiere.Text = "Copiază IBAN";

            btnCopiere.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnCopiere.ForeColor = System.Drawing.Color.White;

        }

        [Obsolete]
        protected void GvConturiEconomii_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.DataItemIndex == -1)
                return;
            e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';");

            e.Row.Attributes.Add("onclick", this.GetPostBackClientEvent(gvConturiEconomii, "Select$" + e.Row.RowIndex.ToString()));

        }

        protected void BtnEconomiiRevenire_Click(object sender, EventArgs e)
        {

            detaliiEconomii.Visible = false;

            dataContainer.Visible = true;

            gvConturiEconomii.SelectedIndex = -1;

        }

        protected void BtnDesfiintareEconomii_Click(object sender, EventArgs e)
        {

            desfiintareEconomii.Visible = true;

            int index = gvConturiEconomii.SelectedRow.RowIndex;

            conn.Open();

            string sql = "SELECT DATEDIFF(month, Data_Creare, GETDATE()), Sold, Valuta, Sold * (Valoare_Comision/100), Valuta_Comision " +
                "FROM Conturi INNER JOIN (Comisioane_Conturi INNER JOIN Comisioane ON Comisioane_Conturi.ID_Comision=Comisioane.ID_Comision) " +
                "ON Conturi.IBAN_Cont=Comisioane_Conturi.IBAN_Cont " +
                "WHERE Conturi.IBAN_Cont = '" + gvConturiEconomii.Rows[index].Cells[1].Text + "' AND Tip_Comision = 'Desfiintare cont economii'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {

                    string account = accountDl.Text;

                    if (account.Substring(8, 3) == reader[2].ToString().Substring(0, 3))
                    {

                        btnAnulareEconomii.Visible = true;

                        btnContinuareEconomii.Visible = true;

                        cbAcordDesfiintareEconomii.Visible = true;

                        lblAcordEconomii.Visible = true;

                        lblMesajEconomii.Visible = true;

                        if (Convert.ToInt64(reader[0].ToString()) > 12)
                        {

                            lblMesajEconomii.Text = "Urmează să vă desființați contul de economii " + "<b>" + gvConturiEconomii.Rows[index].Cells[1].Text + "</b>" +
                            ". Dacă continuați, se va transfera în contul curent " + "<b>" + accountDl.Text + "</b>" + " suma de " + "<b>" + String.Format("{0:n}", reader[1]) + "</b>" + " " + "<b>" + reader[2].ToString() + "</b>" +
                            ". Sunteți sigur că vreți să continuați?";

                        }
                        else
                        {

                            lblMesajEconomii.Text = "Urmează să vă desființați contul de economii " + "<b>" + gvConturiEconomii.Rows[index].Cells[1].Text + "</b>" +
                            ". Dacă continuați, se va transfera în contul curent " + "<b>" + accountDl.Text + "</b>" + " suma de " + "<b>" + String.Format("{0:n}", reader[1]) + "</b>" + " " + "<b>" + reader[2].ToString() + "</b>" +
                            ". Deoarece acest cont este desființat în primele 12 luni de la deschidere, se va reține un comision de " + "<b>" + String.Format("{0:n}", reader[3]) + "</b>" + " " + "<b>" + reader[4].ToString() + "</b>" + ". Sunteți sigur că vreți să continuați?";

                        }

                    }
                    else
                    {
                        lblMesajEconomii.Text = "Selectați un cont curent cu valuta în " + reader[2].ToString() + " pentru a continua!";

                        btnAnulareEconomii.Visible = false;

                        btnContinuareEconomii.Visible = false;

                        cbAcordDesfiintareEconomii.Visible = false;

                        lblAcordEconomii.Visible = false;

                    }

                }

            }

            conn.Close();

        }

        protected void BtnAnulareEconomii_Click(object sender, EventArgs e)
        {

            desfiintareEconomii.Visible = false;

            dataContainer.Visible = true;

            gvConturiEconomii.SelectedIndex = -1;

            cbAcordDesfiintareEconomii.Checked = false;

        }

        protected void BtnContinuareEconomii_Click(object sender, EventArgs e)
        {

            if (cbAcordDesfiintareEconomii.Checked == false)
            {

                lblMesajEroareEconomii.Text = "Bifați căsuta de mai sus dacă doriți să continuați!";

                desfiintareEconomii.Visible = true;

            }
            else
            {

                int index = gvConturiEconomii.SelectedRow.RowIndex;

                conn.Open();

                string selectSold = "SELECT Sold + (Sold*(Incasare_Lunara/100))/360 * DATEDIFF(day, Data_Creare, GETDATE()), DATEDIFF(month, Data_Creare, GETDATE()) " +
                    "FROM Conturi INNER JOIN Dobanda_Conturi ON Conturi.ID_Dobanda=Dobanda_Conturi.ID_Dobanda " +
                    "WHERE IBAN_Cont = '" + gvConturiEconomii.Rows[index].Cells[1].Text + "'";

                string selectComision = "SELECT Sold * (Valoare_Comision/100) " +
                    "FROM Conturi INNER JOIN (Comisioane_Conturi INNER JOIN Comisioane ON Comisioane_Conturi.ID_Comision=Comisioane.ID_Comision) " +
                    "ON Conturi.IBAN_Cont=Comisioane_Conturi.IBAN_Cont " +
                    "WHERE Conturi.IBAN_Cont = '" + gvConturiEconomii.Rows[index].Cells[1].Text + "' AND Tip_Comision = 'Desfiintare cont economii'";

                SqlCommand cmdComision = new SqlCommand(selectComision, conn);

                SqlCommand cmdSold = new SqlCommand(selectSold, conn);

                SqlDataReader readerSold = cmdSold.ExecuteReader();

                readerSold.Read();

                double sold = Convert.ToDouble(readerSold[0].ToString());

                double luni = Convert.ToInt64(readerSold[1].ToString());

                readerSold.Close();

                if (luni > 12)
                {

                    string updateConturi = "UPDATE Conturi " +
                    "SET Sold = Sold + '" + sold + "' " +
                    "WHERE IBAN_Cont = '" + accountDl.Text + "'";

                    SqlCommand cmdupdateConturi = new SqlCommand(updateConturi, conn);

                    cmdupdateConturi.ExecuteNonQuery();


                    string selectNrOperatiuneP = "SELECT MAX(Nr_Operatiune) " +
                    "FROM Operatiuni";

                    SqlCommand cmdNrP = new SqlCommand(selectNrOperatiuneP, conn);

                    SqlDataReader readerNrP = cmdNrP.ExecuteReader();

                    readerNrP.Read();

                    int nrP;

                    if (readerNrP.IsDBNull(0))
                    {

                        nrP = 1;

                    }
                    else
                    {

                        nrP = Convert.ToInt32(readerNrP[0].ToString()) + 1;

                    }

                    readerNrP.Close();


                    string insertOperatiune = "INSERT INTO Operatiuni(Nr_Operatiune, Suma_Operatiune, Data_Operatiune, Detalii_Operatiune, Tip_Operatiune, IBAN_Cont) " +
                        "VALUES(@nr_operatiune, @suma_operatiune, @data_operatiune, @detalii_operatiune, @tip_operatiune, @iban_cont)";

                    SqlCommand cmdIOP = new SqlCommand(insertOperatiune, conn);

                    cmdIOP.Parameters.AddWithValue("@nr_operatiune", nrP);

                    cmdIOP.Parameters.AddWithValue("@suma_operatiune", sold);

                    cmdIOP.Parameters.AddWithValue("@data_operatiune", DateTime.Now.Date);

                    cmdIOP.Parameters.AddWithValue("@detalii_operatiune", "DESFIINTARE CONT ECONOMII");

                    cmdIOP.Parameters.AddWithValue("@tip_operatiune", "TRANSFER");

                    cmdIOP.Parameters.AddWithValue("@iban_cont", accountDl.Text);

                    cmdIOP.ExecuteNonQuery();


                }
                else
                {

                    SqlDataReader readerComision = cmdComision.ExecuteReader();

                    readerComision.Read();

                    double comision = Convert.ToDouble(readerComision[0].ToString());

                    string updateConturi = "UPDATE Conturi " +
                    "SET Sold = Sold + " + sold + " - " + comision + " " +
                    "WHERE Conturi.IBAN_Cont = '" + accountDl.Text + "'";

                    SqlCommand cmdupdateConturi = new SqlCommand(updateConturi, conn);

                    readerComision.Close();

                    cmdupdateConturi.ExecuteNonQuery();


                    string selectNrOperatiuneP = "SELECT MAX(Nr_Operatiune) " +
                    "FROM Operatiuni";

                    SqlCommand cmdNrP = new SqlCommand(selectNrOperatiuneP, conn);

                    SqlDataReader readerNrP = cmdNrP.ExecuteReader();

                    readerNrP.Read();

                    int nrP;

                    if (readerNrP.IsDBNull(0))
                    {

                        nrP = 1;

                    }
                    else
                    {

                        nrP = Convert.ToInt32(readerNrP[0].ToString()) + 1;

                    }

                    readerNrP.Close();


                    string insertOperatiune = "INSERT INTO Operatiuni(Nr_Operatiune, Suma_Operatiune, Data_Operatiune, Detalii_Operatiune, Tip_Operatiune, IBAN_Cont) " +
                        "VALUES(@nr_operatiune, @suma_operatiune, @data_operatiune, @detalii_operatiune, @tip_operatiune, @iban_cont)";

                    SqlCommand cmdIOP = new SqlCommand(insertOperatiune, conn);

                    cmdIOP.Parameters.AddWithValue("@nr_operatiune", nrP);

                    cmdIOP.Parameters.AddWithValue("@suma_operatiune", sold - comision);

                    cmdIOP.Parameters.AddWithValue("@data_operatiune", DateTime.Now.Date);

                    cmdIOP.Parameters.AddWithValue("@detalii_operatiune", "DESFIINTARE CONT ECONOMII");

                    cmdIOP.Parameters.AddWithValue("@tip_operatiune", "TRANSFER");

                    cmdIOP.Parameters.AddWithValue("@iban_cont", accountDl.Text);

                    cmdIOP.ExecuteNonQuery();

                }

                string deleteConturiEconomii = "DELETE FROM Conturi " +
                    "WHERE IBAN_Cont = '" + gvConturiEconomii.Rows[index].Cells[1].Text + "'";

                SqlCommand cmdDeleteCont = new SqlCommand(deleteConturiEconomii, conn);

                string deleteComisioane = "DELETE FROM Comisioane_Conturi " +
                    "WHERE IBAN_Cont = '" + gvConturiEconomii.Rows[index].Cells[1].Text + "'";

                SqlCommand cmdDeleteComisioane = new SqlCommand(deleteComisioane, conn);

                string deleteOperatiuni = "DELETE FROM Operatiuni " +
                    "WHERE IBAN_Cont = '" + gvConturiEconomii.Rows[index].Cells[1].Text + "'";

                SqlCommand cmdDeleteOperatiuni = new SqlCommand(deleteOperatiuni, conn);


                cmdDeleteComisioane.ExecuteNonQuery();

                cmdDeleteOperatiuni.ExecuteNonQuery();

                cmdDeleteCont.ExecuteNonQuery();

                desfiintareEconomii.Visible = false;

                cbAcordDesfiintareEconomii.Checked = false;

                dataContainer.Visible = true;

                gvConturiEconomii.SelectedIndex = -1;

                conn.Close();

                Populate_DgvConturiEconomii();

            }

            DropDownList_Change();

        }

        protected void IbCalendar_Click(object sender, ImageClickEventArgs e)
        {

            deschidereDepozit.Visible = true;

            cldDepozitNou.Visible = true;

        }

        protected void SecondBtn2_Click(object sender, EventArgs e)
        {

            deschidereDepozit.Visible = true;

            cldDepozitNou.Visible = false;

            string account = accountDl.Text;

            if (account.Substring(11, 3) == "ECN")
            {

                txtContTransferNou.Text = "SELECTAȚI UN CONT CURENT";

            }
            else
            {

                txtContTransferNou.Text = accountDl.Text;

            }

            txtValutaDepozitNou.Text = txtValuta.Text;

            lblEroareCalendar.Visible = false;

            txtPerioadaDepozitNou.Text = "1";

            txtValoareDepozitNou.Text = "";

            txtDataInceput.Text = "";

            cbPrelungire.Checked = false;

            cbCapitalizare.Checked = false;

            cbCapitalizare.Enabled = false;

            txtRataDobanda.Text = "";

            if (dlTipDepozitNou.Items.Count == 0)
            {

                dlTipDepozitNou.Items.Add("DEPOZIT CLASIC 1 LUNI");
                dlTipDepozitNou.Items.Add("DEPOZIT CLASIC 3 LUNI");
                dlTipDepozitNou.Items.Add("DEPOZIT CLASIC 6 LUNI");
                dlTipDepozitNou.Items.Add("DEPOZIT CLASIC 12 LUNI");

            }

            dlTipDepozitNou.SelectedIndex = 0;

            conn.Open();

            string cmdRataDobanda = "SELECT Incasare_Scadenta " +
                "FROM Dobanda_Depozit " +
                "WHERE Perioada_Dobanda = " + txtPerioadaDepozitNou.Text + "";

            SqlCommand cmdRD = new SqlCommand(cmdRataDobanda, conn);

            SqlDataReader reader = cmdRD.ExecuteReader();

            reader.Read();

            txtRataDobandaDepozitNou.Text = reader[0].ToString();

            reader.Close();

            conn.Close();

        }

        protected void CldDepozitNou_SelectionChanged(object sender, EventArgs e)
        {

            if (cldDepozitNou.SelectedDate < DateTime.Now)
            {

                txtDataInceput.Text = "";

                lblEroareCalendar.Visible = true;

            }
            else
            {

                txtDataInceput.Text = cldDepozitNou.SelectedDate.ToString("dd'/'MM'/'yyyy");

                lblEroareCalendar.Visible = false;

            }


            deschidereDepozit.Visible = true;

            cldDepozitNou.Visible = false;


        }

        protected void DlTipDepozitNou_SelectedIndexChanged(object sender, EventArgs e)
        {

            deschidereDepozit.Visible = true;

            if (dlTipDepozitNou.Text == "DEPOZIT CLASIC 1 LUNI")
            {
                txtPerioadaDepozitNou.Text = "1";
            }
            else if (dlTipDepozitNou.Text == "DEPOZIT CLASIC 3 LUNI")
            {
                txtPerioadaDepozitNou.Text = "3";
            }
            else if (dlTipDepozitNou.Text == "DEPOZIT CLASIC 6 LUNI")
            {
                txtPerioadaDepozitNou.Text = "6";
            }
            else if (dlTipDepozitNou.Text == "DEPOZIT CLASIC 12 LUNI")
            {
                txtPerioadaDepozitNou.Text = "12";
            }

            conn.Open();

            string cmdRataDobanda = "SELECT Incasare_Scadenta " +
                "FROM Dobanda_Depozit " +
                "WHERE Perioada_Dobanda = " + txtPerioadaDepozitNou.Text + "";

            SqlCommand cmdRD = new SqlCommand(cmdRataDobanda, conn);

            SqlDataReader reader = cmdRD.ExecuteReader();

            reader.Read();

            txtRataDobandaDepozitNou.Text = reader[0].ToString();

            reader.Close();

            conn.Close();

        }

        protected void CldDepozitNou_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            deschidereDepozit.Visible = true;

            cldDepozitNou.Visible = true;

        }

        protected void CbPrelungire_CheckedChanged(object sender, EventArgs e)
        {

            deschidereDepozit.Visible = true;

            cldDepozitNou.Visible = false;

            if (cbPrelungire.Checked == true)
            {

                cbCapitalizare.Enabled = true;

            }
            else if (cbPrelungire.Checked == false)
            {

                cbCapitalizare.Checked = false;

                cbCapitalizare.Enabled = false;

            }

        }


        protected void BtnFinalizareDepozitNou_Click(object sender, EventArgs e)
        {

            conn.Open();

            cldDepozitNou.Visible = false;

            if (txtContTransferNou.Text == "SELECTAȚI UN CONT CURENT")
            {

                lblDateIncomplete.Text = "Introduceți toate datele!";

                deschidereDepozit.Visible = true;

                conn.Close();

                return;

            }

            if (txtValoareDepozitNou.Text == "")
            {

                lblDateIncomplete.Text = "Introduceți toate datele!";

                deschidereDepozit.Visible = true;

                conn.Close();

                return;

            }

            if (txtDataInceput.Text == "")
            {

                lblDateIncomplete.Text = "Introduceți toate datele!";

                deschidereDepozit.Visible = true;

                conn.Close();

                return;

            }

            if (cbAcordDepozitNou.Checked == false)
            {

                lblDateIncomplete.Text = "Bifați căsuța de mai sus pentru a continua!";

                deschidereDepozit.Visible = true;

                conn.Close();

                return;

            }

            string nrIbanDepozit = "SELECT MAX(RIGHT(IBAN_Depozit,2)) " +
                "FROM Depozite";

            SqlCommand cmdNr = new SqlCommand(nrIbanDepozit, conn);

            SqlDataReader readerNr = cmdNr.ExecuteReader();
            
            readerNr.Read();

            int nrIban;

            if (readerNr.IsDBNull(0))
            {

                nrIban = 1;

            }
            else
            {

                nrIban = Convert.ToInt32(readerNr[0].ToString()) + 1;

            }

            readerNr.Close();

            string selectedIBAN = accountDl.Text;

            string codClient = (string)Session["codClient"];

            string depozitIBAN;

            if (txtPerioadaDepozitNou.Text != "12")
            {

                if (nrIban > 9)
                {

                    depozitIBAN = selectedIBAN.Substring(0, 11) + "P0" + txtPerioadaDepozitNou.Text + "L" + codClient + Convert.ToString(nrIban);

                }
                else
                {

                    depozitIBAN = selectedIBAN.Substring(0, 11) + "P0" + txtPerioadaDepozitNou.Text + "L" + codClient + "0" + Convert.ToString(nrIban);

                }


            }
            else
            {

                if (nrIban > 9)
                {

                    depozitIBAN = selectedIBAN.Substring(0, 11) + "P" + txtPerioadaDepozitNou.Text + "L" + codClient + Convert.ToString(nrIban);

                }
                else
                {

                    depozitIBAN = selectedIBAN.Substring(0, 11) + "P" + txtPerioadaDepozitNou.Text + "L" + codClient + "0" + Convert.ToString(nrIban);

                }


            }

            string maxDepozit = "SELECT MAX(Nr_Depozit) " +
                "FROM Depozite";

            SqlCommand maxD = new SqlCommand(maxDepozit, conn);

            SqlDataReader readerMaxDepozit = maxD.ExecuteReader();

            readerMaxDepozit.Read();

            int nrDepozit;

            if (readerMaxDepozit.IsDBNull(0))
            {

                nrDepozit = 1;

            }
            else
            {

                nrDepozit = Convert.ToInt32(readerMaxDepozit[0].ToString()) + 1;

            }
           
            readerMaxDepozit.Close();


            string idDobanda = "SELECT ID_Dobanda " +
                "FROM Dobanda_Depozit " +
                "WHERE Perioada_Dobanda = " + txtPerioadaDepozitNou.Text + "";

            SqlCommand idD = new SqlCommand(idDobanda, conn);

            SqlDataReader readerId = idD.ExecuteReader();

            readerId.Read();

            int idDob = Convert.ToInt32(readerId[0].ToString());

            readerId.Close();

            string cmdInsertDepozit = "INSERT INTO Depozite (Nr_Depozit, IBAN_Depozit, Valoare_Depozit, Suma_Dobanda, Data_Creare, Data_Scadenta, Depozit_Activ, ID_Dobanda, IBAN_Cont, Capitalizare, Prelungire) " +
                "VALUES (@nrdepozit, @ibandepozit, @valoaredepozit, @sumadobanda, @datacreare, @datascadenta, @depozitactiv, @iddobanda, @ibancont, @capitalizare, @prelungire) ";

            SqlCommand cmdID = new SqlCommand(cmdInsertDepozit, conn);

            cmdID.Parameters.AddWithValue("@nrdepozit", nrDepozit);

            cmdID.Parameters.AddWithValue("@ibandepozit", depozitIBAN);

            cmdID.Parameters.AddWithValue("@valoaredepozit", txtValoareDepozitNou.Text);

            cmdID.Parameters.AddWithValue("@sumadobanda", "0");

            cmdID.Parameters.AddWithValue("@datacreare", DateTime.Parse(txtDataInceput.Text));

            cmdID.Parameters.AddWithValue("@datascadenta", DateTime.Parse(txtDataInceput.Text).AddMonths(Convert.ToInt32(txtPerioadaDepozitNou.Text)));

            cmdID.Parameters.AddWithValue("@depozitactiv", "True");

            cmdID.Parameters.AddWithValue("@iddobanda", idDob);

            cmdID.Parameters.AddWithValue("@ibancont", accountDl.Text);

            cmdID.Parameters.AddWithValue("@capitalizare", cbCapitalizare.Checked);

            cmdID.Parameters.AddWithValue("@prelungire", cbPrelungire.Checked);

            cmdID.ExecuteNonQuery();



            string impozitVenit = "SELECT ID_Comision " +
                "FROM Comisioane " +
                "WHERE Tip_Comision = 'Impozit pe venit'";

            SqlCommand impozitV = new SqlCommand(impozitVenit, conn);

            SqlDataReader readerImp = impozitV.ExecuteReader();

            readerImp.Read();

            int impozitID = Convert.ToInt32(readerImp[0].ToString());

            readerImp.Close();

            string insertComision = "INSERT INTO Comisioane_Depozit(Nr_Depozit, ID_Comision) " +
                "VALUES (@nrdepozit,@idcomision) ";

            SqlCommand insertC = new SqlCommand(insertComision, conn);

            insertC.Parameters.AddWithValue("@nrdepozit", nrDepozit);

            insertC.Parameters.AddWithValue("@idcomision", impozitID);

            insertC.ExecuteNonQuery();


            string updateCont = "UPDATE Conturi " +
                   "SET Sold = Sold - " + Convert.ToDecimal(txtValoareDepozitNou.Text) + " " +
                   "WHERE IBAN_Cont = '" + accountDl.Text + "'";

            SqlCommand cmdUpdateCont = new SqlCommand(updateCont, conn);

            cmdUpdateCont.ExecuteNonQuery();



            string selectNrOperatiuneP = "SELECT MAX(Nr_Operatiune) " +
                   "FROM Operatiuni";

            SqlCommand cmdNrP = new SqlCommand(selectNrOperatiuneP, conn);

            SqlDataReader readerNrP = cmdNrP.ExecuteReader();

            readerNrP.Read();

            int nrP;

            if (readerNrP.IsDBNull(0))
            {

                nrP = 1;

            }
            else
            {

                nrP = Convert.ToInt32(readerNrP[0].ToString()) + 1;

            }

            readerNrP.Close();



            string insertOperatiunePrincipal = "INSERT INTO Operatiuni(Nr_Operatiune, Suma_Operatiune, Data_Operatiune, Detalii_Operatiune, Tip_Operatiune, IBAN_Cont) " +
                "VALUES(@nr_operatiune, @suma_operatiune, @data_operatiune, @detalii_operatiune, @tip_operatiune, @iban_cont)";

            SqlCommand cmdIOP = new SqlCommand(insertOperatiunePrincipal, conn);

            cmdIOP.Parameters.AddWithValue("@nr_operatiune", nrP);

            cmdIOP.Parameters.AddWithValue("@suma_operatiune", "-" + Convert.ToDecimal(txtValoareDepozitNou.Text));

            cmdIOP.Parameters.AddWithValue("@data_operatiune", DateTime.Now.Date);

            cmdIOP.Parameters.AddWithValue("@detalii_operatiune", "CONSTITUIRE DEPOZIT");

            cmdIOP.Parameters.AddWithValue("@tip_operatiune", "TRANSFER");

            cmdIOP.Parameters.AddWithValue("@iban_cont", accountDl.Text);

            cmdIOP.ExecuteNonQuery();


            conn.Close();

            DropDownList_Change();

        }

        protected void BtnAnulareDepozitNou_Click(object sender, EventArgs e)
        {

            cldDepozitNou.Visible = false;

            dateDepozite.Visible = true;

        }

        protected void FirstBtn2_Click(object sender, EventArgs e)
        {

            deschideCont.Visible = true;

            if (dlTipContNou.Items.Count == 0)
            {

                dlTipContNou.Items.Add("CONT CURENT");

                dlTipContNou.Items.Add("CONT ECONOMII");

            }

            if (dlValutaContNou.Items.Count == 0)
            {

                dlValutaContNou.Items.Add("RON");

                dlValutaContNou.Items.Add("EURO");

            }

            dlValutaContNou.SelectedIndex = 0;

            dlTipContNou.SelectedIndex = 0;

            lblEroareContNou.Text = "";

            cbAcordContNou.Checked = false;

        }


        protected void BtnFinalizareContNou_Click(object sender, EventArgs e)
        {

            conn.Open();

            if (cbAcordContNou.Checked == false)
            {

                lblEroareContNou.Text = "Bifați căsuța de mai sus pentru a continua!";

                deschideCont.Visible = true;

            }
            else
            {

                string account = accountDl.Text;

                string codClient = (string)Session["codClient"];

                string nrCont = "SELECT MAX(RIGHT(IBAN_Cont,2)) " +
                    "FROM Conturi";

                SqlCommand cmdNr = new SqlCommand(nrCont, conn);

                SqlDataReader readerNr = cmdNr.ExecuteReader();

                readerNr.Read();

                int nrIbanCont = Convert.ToInt32(readerNr[0].ToString()) + 1;

                readerNr.Close();

                string tipCont;

                if (dlTipContNou.Text == "CONT ECONOMII")
                {

                    tipCont = "Cont economii";

                }
                else
                {

                    tipCont = "Cont curent";

                }

                string ibanCont;

                if (dlTipContNou.Text == "CONT ECONOMII")
                {

                    if (nrIbanCont < 9)
                    {

                        ibanCont = account.Substring(0, 8) + dlValutaContNou.Text + "ECN0" + codClient + "0" + nrIbanCont;

                    }
                    else
                    {

                        ibanCont = account.Substring(0, 8) + dlValutaContNou.Text + "ECN0" + codClient + nrIbanCont;

                    }

                }
                else
                {

                    if (nrIbanCont < 9)
                    {

                        ibanCont = account.Substring(0, 8) + dlValutaContNou.Text + "CRT0" + codClient + "0" + nrIbanCont;

                    }
                    else
                    {

                        ibanCont = account.Substring(0, 8) + dlValutaContNou.Text + "CRT0" + codClient + nrIbanCont;

                    }

                }



                string insertCont = "INSERT INTO Conturi(IBAN_Cont, Valuta, Sold, Cod_Client, Tip_Cont, Data_Creare, ID_Dobanda) " +
                    "VALUES (@ibancont, @valuta, @sold, @codclient, @tipcont, @datacreare, @iddobanda)";

                SqlCommand insertC = new SqlCommand(insertCont, conn);

                insertC.Parameters.AddWithValue("@ibancont", ibanCont);

                insertC.Parameters.AddWithValue("@valuta", dlValutaContNou.Text);

                insertC.Parameters.AddWithValue("@sold", "0");

                insertC.Parameters.AddWithValue("@codclient", codClient);

                insertC.Parameters.AddWithValue("@tipcont", tipCont);

                insertC.Parameters.AddWithValue("@datacreare", DateTime.Now.ToString("yyyy'-'MM'-'dd"));

                insertC.Parameters.AddWithValue("@iddobanda", 1);

                insertC.ExecuteNonQuery();



                string selectComision = "SELECT ID_Comision " +
                    "FROM Comisioane " +
                    "WHERE Tip_Comision = 'Desfiintare cont economii'";

                SqlCommand selectCom = new SqlCommand(selectComision, conn);

                SqlDataReader readerCom = selectCom.ExecuteReader();

                readerCom.Read();

                string idComision = readerCom[0].ToString();

                readerCom.Close();

                if (dlTipContNou.Text == "CONT ECONOMII")
                {

                    string insertComision = "INSERT INTO Comisioane_Conturi(IBAN_Cont, ID_Comision) " +
                        "VALUES (@ibancontcomision, @idcomision)";

                    SqlCommand insertCom = new SqlCommand(insertComision, conn);

                    insertCom.Parameters.AddWithValue("@ibancontcomision", ibanCont);

                    insertCom.Parameters.AddWithValue("@idcomision", idComision);

                    insertCom.ExecuteNonQuery();

                }

            }

            conn.Close();

            DropDownList_Change();

        }

        protected void BtnAnulareContNou_Click(object sender, EventArgs e)
        {

            dataContainer.Visible = true;

        }

        private void Populate_DgvCarduriDebit()
        {

            conn.Open();

            string codClient = (string)Session["codClient"];

            string sql = "SELECT Nr_Card AS [Număr card], FORMAT(Data_Expirare, 'dd/MM/yyyy') AS [Dată expirare], CVV, Carduri_Debit.IBAN_Cont AS [IBAN Cont atașat], Card_Activ AS [Card activ] " +
                "FROM Carduri_Debit INNER JOIN Conturi ON Carduri_Debit.IBAN_Cont=Conturi.IBAN_Cont " +
                "WHERE Conturi.Cod_Client = '" + codClient + "' AND Carduri_Debit.IBAN_Cont = '" + accountDl.Text + "' AND Card_Activ = 'True'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            gvCarduriDebit.DataSource = dt;

            gvCarduriDebit.DataBind();

            conn.Close();

        }

        private void Populate_DgvCarduriCredit()
        {

            conn.Open();

            string codClient = (string)Session["codClient"];

            string sql = "SELECT Nr_Card_Credit AS [Număr card], FORMAT(Data_Expirare, 'dd/MM/yyyy') AS [Dată expirare], CVV, Carduri_Credit.IBAN_Cont AS [IBAN Cont atașat], Card_Activ AS [Card activ] " +
                "FROM Carduri_Credit INNER JOIN Conturi ON Carduri_Credit.IBAN_Cont=Conturi.IBAN_Cont " +
                "WHERE Conturi.Cod_Client = '" + codClient + "' AND Carduri_Credit.IBAN_Cont = '" + accountDl.Text + "'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            gvCarduriCredit.DataSource = dt;

            gvCarduriCredit.DataBind();

            conn.Close();

        }

        [Obsolete]
        protected void GvCarduriDebit_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.DataItemIndex == -1)
                return;
            e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';");

            e.Row.Attributes.Add("onclick", this.GetPostBackClientEvent(gvCarduriDebit, "Select$" + e.Row.RowIndex.ToString()));

        }

        protected void GvCarduriDebit_SelectedIndexChanged(object sender, EventArgs e)
        {

            gvCarduriCredit.SelectedIndex = -1;

            cardurileMele.Visible = true;

        }

        [Obsolete]
        protected void GvCarduriCredit_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.DataItemIndex == -1)
                return;
            e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';");

            e.Row.Attributes.Add("onclick", this.GetPostBackClientEvent(gvCarduriCredit, "Select$" + e.Row.RowIndex.ToString()));

        }

        protected void GvCarduriCredit_SelectedIndexChanged(object sender, EventArgs e)
        {

            gvCarduriDebit.SelectedIndex = -1;

            cardurileMele.Visible = true;

        }

        protected void FourthBtn1_Click(object sender, EventArgs e)
        {

            cardurileMele.Visible = true;

            gvCarduriDebit.SelectedIndex = -1;

            gvCarduriCredit.SelectedIndex = -1;

            cldInceputInternet.SelectedDate = DateTime.Now;

            cldInceputNumerar.SelectedDate = DateTime.Now;

            cldInceputPOS.SelectedDate = DateTime.Now;

            cldInceputTranzactii.SelectedDate = DateTime.Now;

            cldSfarsitInternet.SelectedDate = DateTime.Now;

            cldSfarsitNumerar.SelectedDate = DateTime.Now;

            cldSfarsitTranzactii.SelectedDate = DateTime.Now;

            cldSfarsitPOS.SelectedDate = DateTime.Now;

        }

        protected void BtnDetaliiDebit_Click(object sender, EventArgs e)
        {

            cardurileMele.Visible = false;

            vizualizareCardDebit.Visible = true;

            txtDenumireProdusDebit.Text = "VISA Debit";

            int index = gvCarduriDebit.SelectedRow.RowIndex;

            conn.Open();

            string selectCardDebit = "SELECT Carduri_Debit.Nr_Card, FORMAT(Data_Expirare,'MM'),FORMAT(Data_Expirare,'yy'), CVV, Card_Activ, IBAN_Cont, Stare_Card " +
                "FROM Carduri_Debit " +
                "WHERE Carduri_Debit.Nr_Card = '" + gvCarduriDebit.Rows[index].Cells[0].Text + "'";

            SqlCommand selectCD = new SqlCommand(selectCardDebit, conn);

            SqlDataReader reader = selectCD.ExecuteReader();

            reader.Read();

            string nrCard = reader[0].ToString();

            txtNumarCardDebit.Text = nrCard.Substring(0, 4) + " " + nrCard.Substring(3, 4) + " " + nrCard.Substring(7, 4) + " " + nrCard.Substring(11, 4);

            txtCVVDebit.Text = reader[3].ToString();

            txtIbanAtasatDebit.Text = reader[5].ToString();

            txtNumeTitularDebit.Text = nameLbl.Text;

            txtDataExpirareDebit.Text = reader[1].ToString() + "/" + reader[2].ToString();

            txtLinieCreditDebit.Text = "0";

            txtStareCardDebit.Text = reader[6].ToString();

            lblNumarCardFizicDebit.Text = nrCard.Substring(0, 4) + " " + nrCard.Substring(3, 4) + " " + nrCard.Substring(7, 4) + " " + nrCard.Substring(11, 4);

            lblNumeTitularFizicDebit.Text = nameLbl.Text;

            lblCVVFizicDebit.Text = "CVV: " + reader[3].ToString();

            lblDataExpirareFizicDebit.Text = "EXP: " + reader[1].ToString() + "/" + reader[2].ToString();

            lblTipCardFizicDebit.Text = "Debit";

            reader.Close();

            conn.Close();

        }


        protected void BtnLimiteDebit_Click(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;

            cldSfarsitTranzactii.Visible = false;

            ibInceputCalendarInternet.Visible = false;

            ibInceputCalendarNumerar.Visible = false;

            ibInceputCalendarPOS.Visible = false;

            ibInceputCalendarTranzactii.Visible = false;

            ibSfarsitCalendarInternet.Visible = false;

            ibSfarsitCalendarNumerar.Visible = false;

            ibSfarsitCalendarPOS.Visible = false;

            ibSfarsitCalendarTranzactii.Visible = false;

            btnAnulareDebitInternet.Visible = false;

            btnAnulareDebitNumerar.Visible = false;

            btnAnulareDebitPOS.Visible = false;

            btnAnulareDebitTranzactii.Visible = false;


            txtLimitaNouaDebitInternet.Enabled = false;

            txtLimitaNouaDebitNumerar.Enabled = false;

            txtLimitaNouaDebitPOS.Enabled = false;

            txtLimitaNouaDebitTranzactii.Enabled = false;

            conn.Open();

            string selectDateLimite = "SELECT Tip_Limita, Limita_Standard " +
                "FROM Limite ";

            SqlCommand selectDL = new SqlCommand(selectDateLimite, conn);

            SqlDataAdapter adapterDL = new SqlDataAdapter(selectDL);

            DataTable dtDL = new DataTable();

            adapterDL.Fill(dtDL);

            txtTipLimitaDebitInternet.Text = dtDL.Rows[0][0].ToString();

            txtTipLimitaDebitNumerar.Text = dtDL.Rows[1][0].ToString();

            txtTipLimitaDebitTranzactii.Text = dtDL.Rows[2][0].ToString();

            txtTipLimitaDebitPOS.Text = dtDL.Rows[3][0].ToString();

            txtLimitaStandardDebitInternet.Text = dtDL.Rows[0][1].ToString();

            txtLimitaStandardDebitNumerar.Text = dtDL.Rows[1][1].ToString();

            txtLimitaStandardDebitTranzactii.Text = dtDL.Rows[2][1].ToString();

            txtLimitaStandardDebitPOS.Text = dtDL.Rows[3][1].ToString();




            int index = gvCarduriDebit.SelectedRow.RowIndex;

            string selectDateNoi = "SELECT FORMAT(Data_Inceput_Limita, 'dd/MM/yyyy'), FORMAT(Data_Sfarsit_Limita, 'dd/MM/yyyy'), Limita_Noua " +
                "FROM Limite_Card_Debit " +
                "WHERE Nr_Card = '" + gvCarduriDebit.Rows[index].Cells[0].Text + "'";

            SqlCommand cmdDateNoi = new SqlCommand(selectDateNoi, conn);

            SqlDataAdapter adapterDateNoi = new SqlDataAdapter(cmdDateNoi);

            DataTable dtDateNoi = new DataTable();

            adapterDateNoi.Fill(dtDateNoi);


            txtDataInceputLimitaDebitInternet.Text = dtDateNoi.Rows[0][0].ToString();

            txtDataSfarsitLimitaDebitInternet.Text = dtDateNoi.Rows[0][1].ToString();

            txtLimitaNouaDebitInternet.Text = dtDateNoi.Rows[0][2].ToString();



            txtDataInceputLimitaDebitNumerar.Text = dtDateNoi.Rows[1][0].ToString();

            txtDataSfarsitLimitaDebitNumerar.Text = dtDateNoi.Rows[1][1].ToString();

            txtLimitaNouaDebitNumerar.Text = dtDateNoi.Rows[1][2].ToString();




            txtDataInceputLimitaDebitTranzactii.Text = dtDateNoi.Rows[2][0].ToString();

            txtDataSfarsitLimitaDebitTranzactii.Text = dtDateNoi.Rows[2][1].ToString();

            txtLimitaNouaDebitTranzactii.Text = dtDateNoi.Rows[2][2].ToString();




            txtDataInceputLimitaDebitPOS.Text = dtDateNoi.Rows[3][0].ToString();

            txtDataSfarsitLimitaDebitPOS.Text = dtDateNoi.Rows[3][1].ToString();

            txtLimitaNouaDebitPOS.Text = dtDateNoi.Rows[3][2].ToString();

            conn.Close();

        }


        protected void IbInceputCalendarInternet_Click(object sender, ImageClickEventArgs e)
        {

            limiteDebit.Visible = true;

            cldSfarsitTranzactii.Visible = false;

            cldInceputInternet.Visible = true;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;

        }

        protected void IbSfarsitCalendarTranzactii_Click(object sender, ImageClickEventArgs e)
        {

            limiteDebit.Visible = true;

            cldSfarsitTranzactii.Visible = true;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;

        }

        protected void CldInceputInternet_SelectionChanged(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;

            cldSfarsitTranzactii.Visible = false;


            if (txtDataSfarsitLimitaDebitInternet.Text == "" && cldInceputInternet.SelectedDate >= DateTime.Now.Date)
            {

                txtDataInceputLimitaDebitInternet.Text = cldInceputInternet.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataSfarsitLimitaDebitInternet.Text == "" && cldInceputInternet.SelectedDate < DateTime.Now.Date)
            {

                txtDataInceputLimitaDebitInternet.Text = "Data incorectă";

            }
            else if (cldInceputInternet.SelectedDate < DateTime.Now.Date || cldInceputInternet.SelectedDate > cldSfarsitInternet.SelectedDate)
            {

                txtDataInceputLimitaDebitInternet.Text = "Data incorectă";

            }
            else
            {

                txtDataInceputLimitaDebitInternet.Text = cldInceputInternet.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void CldInceputInternet_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputInternet.Visible = true;

        }

        protected void IbSfarsitCalendarInternet_Click(object sender, ImageClickEventArgs e)
        {

            limiteDebit.Visible = true;

            cldSfarsitTranzactii.Visible = false;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = true;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;


        }

        protected void CldSfarsitInternet_SelectionChanged(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;

            cldSfarsitTranzactii.Visible = false;


            if (txtDataInceputLimitaDebitInternet.Text == "" && cldSfarsitInternet.SelectedDate >= DateTime.Now.Date)
            {

                txtDataSfarsitLimitaDebitInternet.Text = cldSfarsitInternet.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataInceputLimitaDebitInternet.Text == "" && cldSfarsitInternet.SelectedDate < DateTime.Now.Date)
            {

                txtDataSfarsitLimitaDebitInternet.Text = "Data incorectă";

            }
            else if (cldSfarsitInternet.SelectedDate < DateTime.Now.Date || cldSfarsitInternet.SelectedDate < cldInceputInternet.SelectedDate)
            {

                txtDataSfarsitLimitaDebitInternet.Text = "Data incorectă";

            }
            else
            {

                txtDataSfarsitLimitaDebitInternet.Text = cldSfarsitInternet.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void CldSfarsitInternet_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteDebit.Visible = true;

            cldSfarsitInternet.Visible = true;

        }

        protected void CldSfarsitTranzactii_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteDebit.Visible = true;

            cldSfarsitTranzactii.Visible = true;

        }

        protected void CldSfarsitTranzactii_SelectionChanged(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;

            cldSfarsitTranzactii.Visible = false;


            if (txtDataInceputLimitaDebitTranzactii.Text == "" && cldSfarsitTranzactii.SelectedDate >= DateTime.Now.Date)
            {

                txtDataSfarsitLimitaDebitTranzactii.Text = cldSfarsitTranzactii.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataInceputLimitaDebitTranzactii.Text == "" && cldSfarsitTranzactii.SelectedDate < DateTime.Now.Date)
            {

                txtDataSfarsitLimitaDebitTranzactii.Text = "Data incorectă";

            }
            else if (cldSfarsitTranzactii.SelectedDate < DateTime.Now.Date || cldSfarsitTranzactii.SelectedDate < cldInceputTranzactii.SelectedDate)
            {

                txtDataSfarsitLimitaDebitTranzactii.Text = "Data incorectă";

            }
            else
            {

                txtDataSfarsitLimitaDebitTranzactii.Text = cldSfarsitTranzactii.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void IbInceputCalendarNumerar_Click(object sender, ImageClickEventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputNumerar.Visible = true;

            cldInceputInternet.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;

            cldSfarsitTranzactii.Visible = false;


        }

        protected void CldInceputNumerar_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputNumerar.Visible = true;

        }

        protected void CldInceputNumerar_SelectionChanged(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;

            cldSfarsitTranzactii.Visible = false;


            if (txtDataSfarsitLimitaDebitNumerar.Text == "" && cldInceputNumerar.SelectedDate >= DateTime.Now.Date)
            {

                txtDataInceputLimitaDebitNumerar.Text = cldInceputNumerar.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataSfarsitLimitaDebitNumerar.Text == "" && cldInceputNumerar.SelectedDate < DateTime.Now.Date)
            {

                txtDataInceputLimitaDebitNumerar.Text = "Data incorectă";

            }
            else if (cldInceputNumerar.SelectedDate < DateTime.Now.Date || cldInceputNumerar.SelectedDate > cldSfarsitNumerar.SelectedDate)
            {

                txtDataInceputLimitaDebitNumerar.Text = "Data incorectă";

            }
            else
            {

                txtDataInceputLimitaDebitNumerar.Text = cldInceputNumerar.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void IbSfarsitCalendarNumerar_Click(object sender, ImageClickEventArgs e)
        {

            limiteDebit.Visible = true;

            cldSfarsitNumerar.Visible = true;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitPOS.Visible = false;

            cldSfarsitTranzactii.Visible = false;


        }

        protected void CldSfarsitNumerar_SelectionChanged(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;

            cldSfarsitTranzactii.Visible = false;


            if (txtDataInceputLimitaDebitNumerar.Text == "" && cldSfarsitNumerar.SelectedDate >= DateTime.Now.Date)
            {

                txtDataSfarsitLimitaDebitNumerar.Text = cldSfarsitNumerar.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataInceputLimitaDebitNumerar.Text == "" && cldSfarsitNumerar.SelectedDate < DateTime.Now.Date)
            {

                txtDataSfarsitLimitaDebitNumerar.Text = "Data incorectă";

            }
            else if (cldSfarsitNumerar.SelectedDate < DateTime.Now.Date || cldSfarsitNumerar.SelectedDate < cldInceputNumerar.SelectedDate)
            {

                txtDataSfarsitLimitaDebitNumerar.Text = "Data incorectă";

            }
            else
            {

                txtDataSfarsitLimitaDebitNumerar.Text = cldSfarsitNumerar.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void CldSfarsitNumerar_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteDebit.Visible = true;

            cldSfarsitNumerar.Visible = true;

        }

        protected void IbInceputCalendarTranzactii_Click(object sender, ImageClickEventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputTranzactii.Visible = true;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;

            cldSfarsitTranzactii.Visible = false;


        }

        protected void CldInceputTranzactii_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputTranzactii.Visible = true;

        }

        protected void CldInceputTranzactii_SelectionChanged(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;

            cldSfarsitTranzactii.Visible = false;


            if (txtDataSfarsitLimitaDebitTranzactii.Text == "" && cldInceputTranzactii.SelectedDate >= DateTime.Now.Date)
            {

                txtDataInceputLimitaDebitTranzactii.Text = cldInceputTranzactii.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataSfarsitLimitaDebitTranzactii.Text == "" && cldInceputTranzactii.SelectedDate < DateTime.Now.Date)
            {

                txtDataInceputLimitaDebitTranzactii.Text = "Data incorectă";

            }
            else if (cldInceputTranzactii.SelectedDate < DateTime.Now.Date || cldInceputTranzactii.SelectedDate > cldSfarsitTranzactii.SelectedDate)
            {

                txtDataInceputLimitaDebitTranzactii.Text = "Data incorectă";

            }
            else
            {

                txtDataInceputLimitaDebitTranzactii.Text = cldInceputTranzactii.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void IbInceputCalendarPOS_Click(object sender, ImageClickEventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputPOS.Visible = true;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;

            cldSfarsitTranzactii.Visible = false;


        }

        protected void CldInceputPOS_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputPOS.Visible = true;

        }

        protected void CldInceputPOS_SelectionChanged(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;

            cldSfarsitTranzactii.Visible = false;


            if (txtDataSfarsitLimitaDebitPOS.Text == "" && cldInceputPOS.SelectedDate >= DateTime.Now.Date)
            {

                txtDataInceputLimitaDebitPOS.Text = cldInceputPOS.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataSfarsitLimitaDebitPOS.Text == "" && cldInceputPOS.SelectedDate < DateTime.Now.Date)
            {

                txtDataInceputLimitaDebitPOS.Text = "Data incorectă";

            }
            else if (cldInceputPOS.SelectedDate < DateTime.Now.Date || cldInceputPOS.SelectedDate > cldSfarsitPOS.SelectedDate)
            {

                txtDataInceputLimitaDebitPOS.Text = "Data incorectă";

            }
            else
            {

                txtDataInceputLimitaDebitPOS.Text = cldInceputPOS.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void IbSfarsitCalendarPOS_Click(object sender, ImageClickEventArgs e)
        {

            limiteDebit.Visible = true;

            cldSfarsitInternet.Visible = false;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitPOS.Visible = true;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitTranzactii.Visible = false;


        }

        protected void CldSfarsitPOS_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteDebit.Visible = true;

            cldSfarsitPOS.Visible = true;

        }

        protected void CldSfarsitPOS_SelectionChanged(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            cldInceputInternet.Visible = false;

            cldInceputNumerar.Visible = false;

            cldInceputPOS.Visible = false;

            cldInceputTranzactii.Visible = false;

            cldSfarsitInternet.Visible = false;

            cldSfarsitNumerar.Visible = false;

            cldSfarsitPOS.Visible = false;

            cldSfarsitTranzactii.Visible = false;


            if (txtDataInceputLimitaDebitPOS.Text == "" && cldSfarsitPOS.SelectedDate >= DateTime.Now.Date)
            {

                txtDataSfarsitLimitaDebitPOS.Text = cldSfarsitPOS.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataInceputLimitaDebitPOS.Text == "" && cldSfarsitPOS.SelectedDate < DateTime.Now.Date)
            {

                txtDataSfarsitLimitaDebitPOS.Text = "Data incorectă";

            }
            else if (cldSfarsitPOS.SelectedDate < DateTime.Now.Date || cldSfarsitPOS.SelectedDate < cldInceputPOS.SelectedDate)
            {

                txtDataSfarsitLimitaDebitPOS.Text = "Data incorectă";

            }
            else
            {

                txtDataSfarsitLimitaDebitPOS.Text = cldSfarsitPOS.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }



        protected void BtnModificareDebitInternet_Click(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            int i = 0;

            i += (int)Session["nrInternet"];

            if (i % 2 == 0)
            {

                btnAnulareDebitInternet.Visible = true;

                btnModificareDebitInternet.Text = "Salvează";

                ibInceputCalendarInternet.Visible = true;

                ibSfarsitCalendarInternet.Visible = true;

                txtLimitaNouaDebitInternet.Enabled = true;

                txtLimitaNouaDebitInternet.BackColor = System.Drawing.Color.White;

                txtDataInceputLimitaDebitInternet.BackColor = System.Drawing.Color.White;

                txtDataSfarsitLimitaDebitInternet.BackColor = System.Drawing.Color.White;

            }
            else
            {

                btnAnulareDebitInternet.Visible = false;

                btnModificareDebitInternet.Text = "Modifică";

                ibInceputCalendarInternet.Visible = false;

                ibSfarsitCalendarInternet.Visible = false;

                txtLimitaNouaDebitInternet.Enabled = false;

                txtLimitaNouaDebitInternet.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataInceputLimitaDebitInternet.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataSfarsitLimitaDebitInternet.BackColor = System.Drawing.Color.WhiteSmoke;

                int index = gvCarduriDebit.SelectedRow.RowIndex;

                if (txtDataInceputLimitaDebitInternet.Text == "")
                {

                    txtDataSfarsitLimitaDebitInternet.Text = "";

                    txtLimitaNouaDebitInternet.Text = "";

                }

                if (txtDataSfarsitLimitaDebitInternet.Text == "")
                {

                    txtDataInceputLimitaDebitInternet.Text = "";

                    txtLimitaNouaDebitInternet.Text = "";

                }

                if (txtLimitaNouaDebitInternet.Text == "")
                {

                    txtDataInceputLimitaDebitInternet.Text = "";

                    txtDataSfarsitLimitaDebitInternet.Text = "";

                }

                conn.Open();

                if (txtDataInceputLimitaDebitInternet.Text == "" && txtDataSfarsitLimitaDebitInternet.Text == "" && txtLimitaNouaDebitInternet.Text == "")
                {

                    string updateLimita = "UPDATE Limite_Card_Debit " +
                        "SET Data_Inceput_Limita = NULL, Data_Sfarsit_Limita = NULL, Limita_Noua = NULL " +
                        "FROM Limite_Card_Debit INNER JOIN Limite ON Limite_Card_Debit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card = '" + gvCarduriDebit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Tranzactii internet'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }
                else
                {

                    DateTime dataInceput = DateTime.Parse(txtDataInceputLimitaDebitInternet.Text);

                    DateTime dataSfarsit = DateTime.Parse(txtDataSfarsitLimitaDebitInternet.Text);

                    string updateLimita = "UPDATE Limite_Card_Debit " +
                        "SET Data_Inceput_Limita = '" + dataInceput.ToString("yyyy'-'MM'-'dd") + "', Data_Sfarsit_Limita = '" + dataSfarsit.ToString("yyyy'-'MM'-'dd") + "', Limita_Noua = '" + txtLimitaNouaDebitInternet.Text + "' " +
                        "FROM Limite_Card_Debit INNER JOIN Limite ON Limite_Card_Debit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card = '" + gvCarduriDebit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Tranzactii internet'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }

                string selectDateNoi = "SELECT FORMAT(Data_Inceput_Limita, 'dd/MM/yyyy'), FORMAT(Data_Sfarsit_Limita, 'dd/MM/yyyy'), Limita_Noua " +
                "FROM Limite_Card_Debit " +
                "WHERE Nr_Card = '" + gvCarduriDebit.Rows[index].Cells[0].Text + "'";

                SqlCommand cmdDateNoi = new SqlCommand(selectDateNoi, conn);

                SqlDataAdapter adapterDateNoi = new SqlDataAdapter(cmdDateNoi);

                DataTable dtDateNoi = new DataTable();

                adapterDateNoi.Fill(dtDateNoi);


                txtDataInceputLimitaDebitInternet.Text = dtDateNoi.Rows[0][0].ToString();

                txtDataSfarsitLimitaDebitInternet.Text = dtDateNoi.Rows[0][1].ToString();

                txtLimitaNouaDebitInternet.Text = dtDateNoi.Rows[0][2].ToString();



                txtDataInceputLimitaDebitNumerar.Text = dtDateNoi.Rows[1][0].ToString();

                txtDataSfarsitLimitaDebitNumerar.Text = dtDateNoi.Rows[1][1].ToString();

                txtLimitaNouaDebitNumerar.Text = dtDateNoi.Rows[1][2].ToString();




                txtDataInceputLimitaDebitTranzactii.Text = dtDateNoi.Rows[2][0].ToString();

                txtDataSfarsitLimitaDebitTranzactii.Text = dtDateNoi.Rows[2][1].ToString();

                txtLimitaNouaDebitTranzactii.Text = dtDateNoi.Rows[2][2].ToString();




                txtDataInceputLimitaDebitPOS.Text = dtDateNoi.Rows[3][0].ToString();

                txtDataSfarsitLimitaDebitPOS.Text = dtDateNoi.Rows[3][1].ToString();

                txtLimitaNouaDebitPOS.Text = dtDateNoi.Rows[3][2].ToString();

                conn.Close();

            }

            i++;

            Session["nrInternet"] = i;

        }

        protected void BtnAnulareDebitInternet_Click(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            txtDataSfarsitLimitaDebitInternet.Text = "";

            txtDataInceputLimitaDebitInternet.Text = "";

            txtLimitaNouaDebitInternet.Text = "";

        }

        protected void BtnModificareDebitNumerar_Click(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            int i = 0;

            i += (int)Session["nrNumerar"];

            if (i % 2 == 0)
            {

                btnAnulareDebitNumerar.Visible = true;

                btnModificareDebitNumerar.Text = "Salvează";

                ibInceputCalendarNumerar.Visible = true;

                ibSfarsitCalendarNumerar.Visible = true;

                txtLimitaNouaDebitNumerar.Enabled = true;

                txtLimitaNouaDebitNumerar.BackColor = System.Drawing.Color.White;

                txtDataInceputLimitaDebitNumerar.BackColor = System.Drawing.Color.White;

                txtDataSfarsitLimitaDebitNumerar.BackColor = System.Drawing.Color.White;

            }
            else
            {

                btnAnulareDebitNumerar.Visible = false;

                btnModificareDebitNumerar.Text = "Modifică";

                ibInceputCalendarNumerar.Visible = false;

                ibSfarsitCalendarNumerar.Visible = false;

                txtLimitaNouaDebitNumerar.Enabled = false;

                txtLimitaNouaDebitNumerar.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataInceputLimitaDebitNumerar.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataSfarsitLimitaDebitNumerar.BackColor = System.Drawing.Color.WhiteSmoke;

                int index = gvCarduriDebit.SelectedRow.RowIndex;

                if (txtDataInceputLimitaDebitNumerar.Text == "")
                {

                    txtDataSfarsitLimitaDebitNumerar.Text = "";

                    txtLimitaNouaDebitNumerar.Text = "";

                }

                if (txtDataSfarsitLimitaDebitNumerar.Text == "")
                {

                    txtDataInceputLimitaDebitNumerar.Text = "";

                    txtLimitaNouaDebitNumerar.Text = "";

                }

                if (txtLimitaNouaDebitNumerar.Text == "")
                {

                    txtDataInceputLimitaDebitNumerar.Text = "";

                    txtDataSfarsitLimitaDebitNumerar.Text = "";

                }

                conn.Open();

                if (txtDataInceputLimitaDebitNumerar.Text == "" && txtDataSfarsitLimitaDebitNumerar.Text == "" && txtLimitaNouaDebitNumerar.Text == "")
                {

                    string updateLimita = "UPDATE Limite_Card_Debit " +
                        "SET Data_Inceput_Limita = NULL, Data_Sfarsit_Limita = NULL, Limita_Noua = NULL " +
                        "FROM Limite_Card_Debit INNER JOIN Limite ON Limite_Card_Debit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card = '" + gvCarduriDebit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Ridicare numerar'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }
                else
                {

                    DateTime dataInceput = DateTime.Parse(txtDataInceputLimitaDebitNumerar.Text);

                    DateTime dataSfarsit = DateTime.Parse(txtDataSfarsitLimitaDebitNumerar.Text);

                    string updateLimita = "UPDATE Limite_Card_Debit " +
                        "SET Data_Inceput_Limita = '" + dataInceput.ToString("yyyy'-'MM'-'dd") + "', Data_Sfarsit_Limita = '" + dataSfarsit.ToString("yyyy'-'MM'-'dd") + "', Limita_Noua = '" + txtLimitaNouaDebitNumerar.Text + "' " +
                        "FROM Limite_Card_Debit INNER JOIN Limite ON Limite_Card_Debit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card = '" + gvCarduriDebit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Ridicare numerar'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }


                string selectDateNoi = "SELECT FORMAT(Data_Inceput_Limita, 'dd/MM/yyyy'), FORMAT(Data_Sfarsit_Limita, 'dd/MM/yyyy'), Limita_Noua " +
                "FROM Limite_Card_Debit " +
                "WHERE Nr_Card = '" + gvCarduriDebit.Rows[index].Cells[0].Text + "'";

                SqlCommand cmdDateNoi = new SqlCommand(selectDateNoi, conn);

                SqlDataAdapter adapterDateNoi = new SqlDataAdapter(cmdDateNoi);

                DataTable dtDateNoi = new DataTable();

                adapterDateNoi.Fill(dtDateNoi);


                txtDataInceputLimitaDebitInternet.Text = dtDateNoi.Rows[0][0].ToString();

                txtDataSfarsitLimitaDebitInternet.Text = dtDateNoi.Rows[0][1].ToString();

                txtLimitaNouaDebitInternet.Text = dtDateNoi.Rows[0][2].ToString();



                txtDataInceputLimitaDebitNumerar.Text = dtDateNoi.Rows[1][0].ToString();

                txtDataSfarsitLimitaDebitNumerar.Text = dtDateNoi.Rows[1][1].ToString();

                txtLimitaNouaDebitNumerar.Text = dtDateNoi.Rows[1][2].ToString();




                txtDataInceputLimitaDebitTranzactii.Text = dtDateNoi.Rows[2][0].ToString();

                txtDataSfarsitLimitaDebitTranzactii.Text = dtDateNoi.Rows[2][1].ToString();

                txtLimitaNouaDebitTranzactii.Text = dtDateNoi.Rows[2][2].ToString();




                txtDataInceputLimitaDebitPOS.Text = dtDateNoi.Rows[3][0].ToString();

                txtDataSfarsitLimitaDebitPOS.Text = dtDateNoi.Rows[3][1].ToString();

                txtLimitaNouaDebitPOS.Text = dtDateNoi.Rows[3][2].ToString();

                conn.Close();

            }

            i++;

            Session["nrNumerar"] = i;

        }

        protected void BtnAnulareDebitNumerar_Click(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            txtDataSfarsitLimitaDebitNumerar.Text = "";

            txtDataInceputLimitaDebitNumerar.Text = "";

            txtLimitaNouaDebitNumerar.Text = "";

        }

        protected void BtnModificareDebitTranzactii_Click(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            int i = 0;

            i += (int)Session["nrTranzactii"];

            if (i % 2 == 0)
            {

                btnAnulareDebitTranzactii.Visible = true;

                btnModificareDebitTranzactii.Text = "Salvează";

                ibInceputCalendarTranzactii.Visible = true;

                ibSfarsitCalendarTranzactii.Visible = true;

                txtLimitaNouaDebitTranzactii.Enabled = true;

                txtLimitaNouaDebitTranzactii.BackColor = System.Drawing.Color.White;

                txtDataInceputLimitaDebitTranzactii.BackColor = System.Drawing.Color.White;

                txtDataSfarsitLimitaDebitTranzactii.BackColor = System.Drawing.Color.White;

            }
            else
            {

                btnAnulareDebitTranzactii.Visible = false;

                btnModificareDebitTranzactii.Text = "Modifică";

                ibInceputCalendarTranzactii.Visible = false;

                ibSfarsitCalendarTranzactii.Visible = false;

                txtLimitaNouaDebitTranzactii.Enabled = false;

                txtLimitaNouaDebitTranzactii.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataInceputLimitaDebitTranzactii.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataSfarsitLimitaDebitTranzactii.BackColor = System.Drawing.Color.WhiteSmoke;

                int index = gvCarduriDebit.SelectedRow.RowIndex;

                if (txtDataInceputLimitaDebitTranzactii.Text == "")
                {

                    txtDataSfarsitLimitaDebitTranzactii.Text = "";

                    txtLimitaNouaDebitTranzactii.Text = "";

                }

                if (txtDataSfarsitLimitaDebitTranzactii.Text == "")
                {

                    txtDataInceputLimitaDebitTranzactii.Text = "";

                    txtLimitaNouaDebitTranzactii.Text = "";

                }

                if (txtLimitaNouaDebitTranzactii.Text == "")
                {

                    txtDataInceputLimitaDebitTranzactii.Text = "";

                    txtDataSfarsitLimitaDebitTranzactii.Text = "";

                }

                conn.Open();

                if (txtDataInceputLimitaDebitTranzactii.Text == "" && txtDataSfarsitLimitaDebitTranzactii.Text == "" && txtLimitaNouaDebitTranzactii.Text == "")
                {

                    string updateLimita = "UPDATE Limite_Card_Debit " +
                        "SET Data_Inceput_Limita = NULL, Data_Sfarsit_Limita = NULL, Limita_Noua = NULL " +
                        "FROM Limite_Card_Debit INNER JOIN Limite ON Limite_Card_Debit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card = '" + gvCarduriDebit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Tranzactii zilnice'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }
                else
                {

                    DateTime dataInceput = DateTime.Parse(txtDataInceputLimitaDebitTranzactii.Text);

                    DateTime dataSfarsit = DateTime.Parse(txtDataSfarsitLimitaDebitTranzactii.Text);

                    string updateLimita = "UPDATE Limite_Card_Debit " +
                        "SET Data_Inceput_Limita = '" + dataInceput.ToString("yyyy'-'MM'-'dd") + "', Data_Sfarsit_Limita = '" + dataSfarsit.ToString("yyyy'-'MM'-'dd") + "', Limita_Noua = '" + txtLimitaNouaDebitTranzactii.Text + "' " +
                        "FROM Limite_Card_Debit INNER JOIN Limite ON Limite_Card_Debit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card = '" + gvCarduriDebit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Tranzactii zilnice'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }


                string selectDateNoi = "SELECT FORMAT(Data_Inceput_Limita, 'dd/MM/yyyy'), FORMAT(Data_Sfarsit_Limita, 'dd/MM/yyyy'), Limita_Noua " +
                "FROM Limite_Card_Debit " +
                "WHERE Nr_Card = '" + gvCarduriDebit.Rows[index].Cells[0].Text + "'";

                SqlCommand cmdDateNoi = new SqlCommand(selectDateNoi, conn);

                SqlDataAdapter adapterDateNoi = new SqlDataAdapter(cmdDateNoi);

                DataTable dtDateNoi = new DataTable();

                adapterDateNoi.Fill(dtDateNoi);


                txtDataInceputLimitaDebitInternet.Text = dtDateNoi.Rows[0][0].ToString();

                txtDataSfarsitLimitaDebitInternet.Text = dtDateNoi.Rows[0][1].ToString();

                txtLimitaNouaDebitInternet.Text = dtDateNoi.Rows[0][2].ToString();



                txtDataInceputLimitaDebitNumerar.Text = dtDateNoi.Rows[1][0].ToString();

                txtDataSfarsitLimitaDebitNumerar.Text = dtDateNoi.Rows[1][1].ToString();

                txtLimitaNouaDebitNumerar.Text = dtDateNoi.Rows[1][2].ToString();




                txtDataInceputLimitaDebitTranzactii.Text = dtDateNoi.Rows[2][0].ToString();

                txtDataSfarsitLimitaDebitTranzactii.Text = dtDateNoi.Rows[2][1].ToString();

                txtLimitaNouaDebitTranzactii.Text = dtDateNoi.Rows[2][2].ToString();




                txtDataInceputLimitaDebitPOS.Text = dtDateNoi.Rows[3][0].ToString();

                txtDataSfarsitLimitaDebitPOS.Text = dtDateNoi.Rows[3][1].ToString();

                txtLimitaNouaDebitPOS.Text = dtDateNoi.Rows[3][2].ToString();

                conn.Close();

            }

            i++;

            Session["nrTranzactii"] = i;

        }

        protected void BtnAnulareDebitTranzactii_Click(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            txtDataSfarsitLimitaDebitTranzactii.Text = "";

            txtDataInceputLimitaDebitTranzactii.Text = "";

            txtLimitaNouaDebitTranzactii.Text = "";

        }

        protected void BtnModificareDebitPOS_Click(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            int i = 0;

            i += (int)Session["nrPOS"];

            if (i % 2 == 0)
            {

                btnAnulareDebitPOS.Visible = true;

                btnModificareDebitPOS.Text = "Salvează";

                ibInceputCalendarPOS.Visible = true;

                ibSfarsitCalendarPOS.Visible = true;

                txtLimitaNouaDebitPOS.Enabled = true;

                txtLimitaNouaDebitPOS.BackColor = System.Drawing.Color.White;

                txtDataInceputLimitaDebitPOS.BackColor = System.Drawing.Color.White;

                txtDataSfarsitLimitaDebitPOS.BackColor = System.Drawing.Color.White;

            }
            else
            {

                btnAnulareDebitPOS.Visible = false;

                btnModificareDebitPOS.Text = "Modifică";

                ibInceputCalendarPOS.Visible = false;

                ibSfarsitCalendarPOS.Visible = false;

                txtLimitaNouaDebitPOS.Enabled = false;

                txtLimitaNouaDebitPOS.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataInceputLimitaDebitPOS.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataSfarsitLimitaDebitPOS.BackColor = System.Drawing.Color.WhiteSmoke;

                int index = gvCarduriDebit.SelectedRow.RowIndex;

                if (txtDataInceputLimitaDebitPOS.Text == "")
                {

                    txtDataSfarsitLimitaDebitPOS.Text = "";

                    txtLimitaNouaDebitPOS.Text = "";

                }

                if (txtDataSfarsitLimitaDebitPOS.Text == "")
                {

                    txtDataInceputLimitaDebitPOS.Text = "";

                    txtLimitaNouaDebitPOS.Text = "";

                }

                if (txtLimitaNouaDebitPOS.Text == "")
                {

                    txtDataInceputLimitaDebitPOS.Text = "";

                    txtDataSfarsitLimitaDebitPOS.Text = "";

                }

                conn.Open();

                if (txtDataInceputLimitaDebitPOS.Text == "" && txtDataSfarsitLimitaDebitPOS.Text == "" && txtLimitaNouaDebitPOS.Text == "")
                {

                    string updateLimita = "UPDATE Limite_Card_Debit " +
                        "SET Data_Inceput_Limita = NULL, Data_Sfarsit_Limita = NULL, Limita_Noua = NULL " +
                        "FROM Limite_Card_Debit INNER JOIN Limite ON Limite_Card_Debit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card = '" + gvCarduriDebit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Tranzactii POS'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }
                else
                {

                    DateTime dataInceput = DateTime.Parse(txtDataInceputLimitaDebitPOS.Text);

                    DateTime dataSfarsit = DateTime.Parse(txtDataSfarsitLimitaDebitPOS.Text);

                    string updateLimita = "UPDATE Limite_Card_Debit " +
                        "SET Data_Inceput_Limita = '" + dataInceput.ToString("yyyy'-'MM'-'dd") + "', Data_Sfarsit_Limita = '" + dataSfarsit.ToString("yyyy'-'MM'-'dd") + "', Limita_Noua = '" + txtLimitaNouaDebitPOS.Text + "' " +
                        "FROM Limite_Card_Debit INNER JOIN Limite ON Limite_Card_Debit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card = '" + gvCarduriDebit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Tranzactii POS'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }


                string selectDateNoi = "SELECT FORMAT(Data_Inceput_Limita, 'dd/MM/yyyy'), FORMAT(Data_Sfarsit_Limita, 'dd/MM/yyyy'), Limita_Noua " +
                "FROM Limite_Card_Debit " +
                "WHERE Nr_Card = '" + gvCarduriDebit.Rows[index].Cells[0].Text + "'";

                SqlCommand cmdDateNoi = new SqlCommand(selectDateNoi, conn);

                SqlDataAdapter adapterDateNoi = new SqlDataAdapter(cmdDateNoi);

                DataTable dtDateNoi = new DataTable();

                adapterDateNoi.Fill(dtDateNoi);


                txtDataInceputLimitaDebitInternet.Text = dtDateNoi.Rows[0][0].ToString();

                txtDataSfarsitLimitaDebitInternet.Text = dtDateNoi.Rows[0][1].ToString();

                txtLimitaNouaDebitInternet.Text = dtDateNoi.Rows[0][2].ToString();



                txtDataInceputLimitaDebitNumerar.Text = dtDateNoi.Rows[1][0].ToString();

                txtDataSfarsitLimitaDebitNumerar.Text = dtDateNoi.Rows[1][1].ToString();

                txtLimitaNouaDebitNumerar.Text = dtDateNoi.Rows[1][2].ToString();




                txtDataInceputLimitaDebitTranzactii.Text = dtDateNoi.Rows[2][0].ToString();

                txtDataSfarsitLimitaDebitTranzactii.Text = dtDateNoi.Rows[2][1].ToString();

                txtLimitaNouaDebitTranzactii.Text = dtDateNoi.Rows[2][2].ToString();




                txtDataInceputLimitaDebitPOS.Text = dtDateNoi.Rows[3][0].ToString();

                txtDataSfarsitLimitaDebitPOS.Text = dtDateNoi.Rows[3][1].ToString();

                txtLimitaNouaDebitPOS.Text = dtDateNoi.Rows[3][2].ToString();

                conn.Close();

            }

            i++;

            Session["nrPOS"] = i;

        }

        protected void BtnAnulareDebitPOS_Click(object sender, EventArgs e)
        {

            limiteDebit.Visible = true;

            txtDataSfarsitLimitaDebitPOS.Text = "";

            txtDataInceputLimitaDebitPOS.Text = "";

            txtLimitaNouaDebitPOS.Text = "";

        }

        protected void Verificare_Limite()
        {

            conn.Open();

            string updateLimite = "UPDATE Limite_Card_Debit " +
                "SET Data_Inceput_Limita = NULL, Data_Sfarsit_Limita = NULL, Limita_Noua = NULL " +
                "WHERE Data_Sfarsit_Limita < '" + DateTime.Now.ToString("yyyy'-'MM'-'dd") + "'";

            SqlCommand cmdUL = new SqlCommand(updateLimite, conn);

            cmdUL.ExecuteNonQuery();

            conn.Close();

        }

        protected void BtnLimiteInapoi_Click(object sender, EventArgs e)
        {

            limiteDebit.Visible = false;

            cardurileMele.Visible = true;

        }


        protected void IbDataInceputIstoricDebit_Click(object sender, ImageClickEventArgs e)
        {

            cldDataInceputIstoricDebit.Visible = true;

            istoricTranzactiiDebit.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaDebit.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

        }

        protected void IbDataSfarsitIstoricDebit_Click(object sender, ImageClickEventArgs e)
        {

            cldDataSfarsitIstoricDebit.Visible = true;

            istoricTranzactiiDebit.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaDebit.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

        }

        protected void CldDataInceputIstoricDebit_SelectionChanged(object sender, EventArgs e)
        {

            istoricTranzactiiDebit.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaDebit.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

            cldDataInceputIstoricDebit.Visible = false;

            if (txtDataSfarsitIstoricDebit.Text == "")
            {

                txtDataInceputIstoricDebit.Text = cldDataInceputIstoricDebit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataSfarsitIstoricDebit.Text != "" && cldDataInceputIstoricDebit.SelectedDate > cldDataSfarsitIstoricDebit.SelectedDate)
            {

                txtDataInceputIstoricDebit.Text = "Data incorectă";

            }
            else
            {

                txtDataInceputIstoricDebit.Text = cldDataInceputIstoricDebit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void CldDataSfarsitIstoricDebit_SelectionChanged(object sender, EventArgs e)
        {

            istoricTranzactiiDebit.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaDebit.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

            cldDataSfarsitIstoricDebit.Visible = false;

            if (txtDataInceputIstoricDebit.Text == "")
            {

                txtDataSfarsitIstoricDebit.Text = cldDataSfarsitIstoricDebit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataInceputIstoricDebit.Text != "" && cldDataSfarsitIstoricDebit.SelectedDate < cldDataInceputIstoricDebit.SelectedDate)
            {

                txtDataSfarsitIstoricDebit.Text = "Data incorectă";

            }
            else
            {

                txtDataSfarsitIstoricDebit.Text = cldDataSfarsitIstoricDebit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void CldDataInceputIstoricDebit_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            istoricTranzactiiDebit.Visible = true;

            cldDataInceputIstoricDebit.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaDebit.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

        }

        protected void CldDataSfarsitIstoricDebit_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            istoricTranzactiiDebit.Visible = true;

            cldDataSfarsitIstoricDebit.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaDebit.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

        }

        protected void BtnVizualizareTranzactiiIstoricDebit_Click(object sender, EventArgs e)
        {

            istoricTranzactiiDebit.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaDebit.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

            if (rblPerioadaDebit.SelectedIndex == 0)
            {

                if (dlCardDebit.Text == "")
                {

                    lblEroareIstoric.Visible = true;

                    return;

                }
                else
                {

                    lblEroareIstoric.Visible = false;

                }

            }
            else if (rblPerioadaDebit.SelectedIndex == 1)
            {

                if (txtDataInceputIstoricDebit.Text == "" || txtDataSfarsitIstoricDebit.Text == "")
                {

                    lblEroareIstoric.Visible = true;

                    return;

                }
                else
                {

                    lblEroareIstoric.Visible = false;

                }

            }

            DataTable dt = new DataTable();

            if (rblPerioadaDebit.SelectedIndex == 0)
            {

                if (dlTipTranzactieIstoricDebit.SelectedIndex == 0)
                {

                    conn.Open();

                    string sql = "SELECT Nr_Card AS [Număr card], Tip_Operatiune AS [Tip tranzacție], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Dată], Suma_Operatiune AS [Sumă] " +
                        "FROM Operatiuni " +
                        "WHERE Nr_Card = '" + dlCardDebit.Text + "'";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(dt);

                    gvListaTranzactiiIstoricDebit.DataSource = dt;

                    gvListaTranzactiiIstoricDebit.DataBind();

                    conn.Close();

                }
                else
                {

                    conn.Open();

                    string sql = "SELECT Nr_Card AS [Număr card], Tip_Operatiune AS [Tip tranzacție], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Dată], Suma_Operatiune AS [Sumă] " +
                        "FROM Operatiuni " +
                        "WHERE Nr_Card = '" + dlCardDebit.Text + "' AND Tip_Operatiune = '" + dlTipTranzactieIstoricDebit.Text + "'";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(dt);

                    gvListaTranzactiiIstoricDebit.DataSource = dt;

                    gvListaTranzactiiIstoricDebit.DataBind();

                    conn.Close();

                }


            }
            else if (rblPerioadaDebit.SelectedIndex == 1)
            {

                if (dlTipTranzactieIstoricDebit.SelectedIndex == 0)
                {

                    conn.Open();

                    string sql = "SELECT Nr_Card AS [Număr card], Tip_Operatiune AS [Tip tranzacție], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Dată], Suma_Operatiune AS [Sumă] " +
                        "FROM Operatiuni " +
                        "WHERE Nr_Card = '" + dlCardDebit.Text + "' AND Data_Operatiune BETWEEN '" + DateTime.Parse(txtDataInceputIstoricDebit.Text).ToString("yyyy'-'MM'-'dd") + "' AND '" + DateTime.Parse(txtDataSfarsitIstoricDebit.Text).ToString("yyyy'-'MM'-'dd") + "'";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(dt);

                    gvListaTranzactiiIstoricDebit.DataSource = dt;

                    gvListaTranzactiiIstoricDebit.DataBind();

                    conn.Close();

                }
                else
                {

                    conn.Open();

                    string sql = "SELECT Nr_Card AS [Număr card], Tip_Operatiune AS [Tip tranzacție], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Dată], Suma_Operatiune AS [Sumă] " +
                        "FROM Operatiuni " +
                        "WHERE Nr_Card = '" + dlCardDebit.Text + "' AND Tip_Operatiune = '" + dlTipTranzactieIstoricDebit.Text + "' AND Data_Operatiune BETWEEN '" + DateTime.Parse(txtDataInceputIstoricDebit.Text).ToString("yyyy'-'MM'-'dd") + "' AND '" + DateTime.Parse(txtDataSfarsitIstoricDebit.Text).ToString("yyyy'-'MM'-'dd") + "'";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(dt);

                    gvListaTranzactiiIstoricDebit.DataSource = dt;

                    gvListaTranzactiiIstoricDebit.DataBind();

                    conn.Close();

                }

            }

            if (dt.Rows.Count < 1)
            {

                btnExportPdfIstoric.Visible = false;

            }
            else
            {

                btnExportPdfIstoric.Visible = true;

            }

        }

        protected void FourthBtn2_Click(object sender, EventArgs e)
        {

            istoricTranzactiiDebit.Visible = true;

            cldDataInceputIstoricDebit.Visible = false;

            cldDataSfarsitIstoricDebit.Visible = false;

            txtDataInceputIstoricDebit.Text = "";

            txtDataSfarsitIstoricDebit.Text = "";

            cldDataInceputIstoricDebit.SelectedDate = DateTime.MinValue;

            cldDataSfarsitIstoricDebit.SelectedDate = DateTime.MinValue;

            lblEroareIstoric.Visible = false;

            ibDataInceputIstoricDebit.Visible = false;

            ibDataSfarsitIstoricDebit.Visible = false;

            txtDataInceputIstoricDebit.BackColor = System.Drawing.Color.WhiteSmoke;

            txtDataSfarsitIstoricDebit.BackColor = System.Drawing.Color.WhiteSmoke;


            string sql = "SELECT * FROM Carduri_Debit WHERE IBAN_Cont = '" + accountDl.Text + "'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            dlCardDebit.DataSource = dt;

            dlCardDebit.DataTextField = "Nr_Card";

            dlCardDebit.DataBind();



            if (rblPerioadaDebit.Items.Count == 0)
            {

                rblPerioadaDebit.Items.Add("TOATE TRANZACȚIILE");

                rblPerioadaDebit.Items.Add("SELECTARE PERIOADĂ");

                rblPerioadaDebit.RepeatDirection = RepeatDirection.Horizontal;

            }

            rblPerioadaDebit.SelectedIndex = 0;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaDebit.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }



            if (dlTipTranzactieIstoricDebit.Items.Count == 0)
            {

                dlTipTranzactieIstoricDebit.Items.Add("");

                dlTipTranzactieIstoricDebit.Items.Add("ATM");

                dlTipTranzactieIstoricDebit.Items.Add("POS");

            }

            dlTipTranzactieIstoricDebit.SelectedIndex = 0;



            string sqlLista = "SELECT Nr_Card AS [Număr card], Tip_Operatiune AS [Tip tranzacție], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Dată], Suma_Operatiune AS [Sumă] " +
                "FROM Operatiuni " +
                "WHERE Nr_Card = '" + dlCardDebit.Text + "'";

            SqlCommand cmdLista = new SqlCommand(sqlLista, conn);

            SqlDataAdapter adapterLista = new SqlDataAdapter(cmdLista);

            DataTable dtLista = new DataTable();

            adapterLista.Fill(dtLista);

            gvListaTranzactiiIstoricDebit.DataSource = dtLista;

            gvListaTranzactiiIstoricDebit.DataBind();

            Session["nrColoaneIstoric"] = dtLista.Columns.Count;

            conn.Close();

            if (dtLista.Rows.Count == 0)
            {

                btnExportPdfIstoric.Visible = false;

            }
            else
            {

                btnExportPdfIstoric.Visible = true;

            }

        }

        protected void RblPerioadaDebit_SelectedIndexChanged(object sender, EventArgs e)
        {

            istoricTranzactiiDebit.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaDebit.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

            if (rblPerioadaDebit.SelectedIndex == 0)
            {

                ibDataInceputIstoricDebit.Visible = false;

                ibDataSfarsitIstoricDebit.Visible = false;

                txtDataInceputIstoricDebit.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataSfarsitIstoricDebit.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataInceputIstoricDebit.Text = "";

                txtDataSfarsitIstoricDebit.Text = "";

            }
            else if (rblPerioadaDebit.SelectedIndex == 1)
            {

                cldDataInceputIstoricDebit.SelectedDate = DateTime.MinValue.Date;

                cldDataSfarsitIstoricDebit.SelectedDate = DateTime.MinValue.Date;

                ibDataInceputIstoricDebit.Visible = true;

                ibDataSfarsitIstoricDebit.Visible = true;

                txtDataInceputIstoricDebit.BackColor = System.Drawing.Color.White;

                txtDataSfarsitIstoricDebit.BackColor = System.Drawing.Color.White;

            }

        }

        protected void BtnLinieCredit_Click(object sender, EventArgs e)
        {

            linieCredit.Visible = true;

            cardurileMele.Visible = false;

            int index = gvCarduriDebit.SelectedIndex;

            string nrCard = gvCarduriDebit.Rows[index].Cells[0].Text;

            txtNrCardLinie.Text = nrCard.Substring(0, 4) + " " + nrCard.Substring(3, 4) + " " + nrCard.Substring(7, 4) + " " + nrCard.Substring(11, 4);


            if (dlPlafonLinie.Items.Count == 0)
            {

                dlPlafonLinie.Items.Add("1");

                dlPlafonLinie.Items.Add("2");

                dlPlafonLinie.Items.Add("3");

            }


            string codClient = (string)Session["codClient"];

            conn.Open();

            string selectVenit = "SELECT Venit " +
                "FROM Clienti " +
                "WHERE Cod_Client = '" + codClient + "'";

            SqlCommand cmdVenit = new SqlCommand(selectVenit, conn);

            SqlDataReader drVenit = cmdVenit.ExecuteReader();

            drVenit.Read();

            decimal venit = Convert.ToDecimal(drVenit[0]);

            drVenit.Close();

            conn.Close();

            int plafon = Convert.ToInt32(dlPlafonLinie.Text);

            txtSumaLinie.Text = Convert.ToString(plafon * venit);

        }

        protected void DlPlafonLinie_SelectedIndexChanged(object sender, EventArgs e)
        {

            linieCredit.Visible = true;

            string codClient = (string)Session["codClient"];

            conn.Open();

            string selectVenit = "SELECT Venit " +
                "FROM Clienti " +
                "WHERE Cod_Client = '" + codClient + "'";

            SqlCommand cmdVenit = new SqlCommand(selectVenit, conn);

            SqlDataReader drVenit = cmdVenit.ExecuteReader();

            drVenit.Read();

            decimal venit = Convert.ToDecimal(drVenit[0]);

            drVenit.Close();

            conn.Close();

            int plafon = Convert.ToInt32(dlPlafonLinie.Text);

            txtSumaLinie.Text = Convert.ToString(plafon * venit);

        }

        protected void BtnFinalizareLinie_Click(object sender, EventArgs e)
        {

            int index = gvCarduriDebit.SelectedIndex;

            string nrCard = gvCarduriDebit.Rows[index].Cells[0].Text;

            conn.Open();

            string updateLinie = "UPDATE Linii_Credit " +
                "SET Plafon_Linie_Credit = '" + dlPlafonLinie.Text + "', Suma_Linie_Credit = '" + txtSumaLinie.Text + "' " +
                "WHERE Nr_Card = '" + nrCard + "'";

            SqlCommand cmdUL = new SqlCommand(updateLinie, conn);

            cmdUL.ExecuteNonQuery();

            conn.Close();

            linieCredit.Visible = false;

            cardurileMele.Visible = true;

        }

        protected void BtnAnulareLinie_Click(object sender, EventArgs e)
        {

            linieCredit.Visible = false;

            cardurileMele.Visible = true;

        }

        protected void BtnModificareCreditInternet_Click(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            int i = 0;

            i += (int)Session["nrInternetCredit"];

            if (i % 2 == 0)
            {

                btnAnulareCreditInternet.Visible = true;

                btnModificareCreditInternet.Text = "Salvează";

                ibInceputCalendarInternetCredit.Visible = true;

                ibSfarsitCalendarInternetCredit.Visible = true;

                txtLimitaNouaCreditInternet.Enabled = true;

                txtLimitaNouaCreditInternet.BackColor = System.Drawing.Color.White;

                txtDataInceputLimitaCreditInternet.BackColor = System.Drawing.Color.White;

                txtDataSfarsitLimitaCreditInternet.BackColor = System.Drawing.Color.White;

            }
            else
            {

                btnAnulareCreditInternet.Visible = false;

                btnModificareCreditInternet.Text = "Modifică";

                ibInceputCalendarInternetCredit.Visible = false;

                ibSfarsitCalendarInternetCredit.Visible = false;

                txtLimitaNouaCreditInternet.Enabled = false;

                txtLimitaNouaCreditInternet.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataInceputLimitaCreditInternet.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataSfarsitLimitaCreditInternet.BackColor = System.Drawing.Color.WhiteSmoke;

                int index = gvCarduriCredit.SelectedRow.RowIndex;

                if (txtDataInceputLimitaCreditInternet.Text == "")
                {

                    txtDataSfarsitLimitaCreditInternet.Text = "";

                    txtLimitaNouaCreditInternet.Text = "";

                }

                if (txtDataSfarsitLimitaCreditInternet.Text == "")
                {

                    txtDataInceputLimitaCreditInternet.Text = "";

                    txtLimitaNouaCreditInternet.Text = "";

                }

                if (txtLimitaNouaCreditInternet.Text == "")
                {

                    txtDataInceputLimitaCreditInternet.Text = "";

                    txtDataSfarsitLimitaCreditInternet.Text = "";

                }

                conn.Open();

                if (txtDataInceputLimitaCreditInternet.Text == "" && txtDataSfarsitLimitaCreditInternet.Text == "" && txtLimitaNouaCreditInternet.Text == "")
                {

                    string updateLimita = "UPDATE Limite_Card_Credit " +
                        "SET Data_Inceput_Limita = NULL, Data_Sfarsit_Limita = NULL, Limita_Noua = NULL " +
                        "FROM Limite_Card_Credit INNER JOIN Limite ON Limite_Card_Credit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card_Credit = '" + gvCarduriCredit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Tranzactii internet'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }
                else
                {

                    DateTime dataInceput = DateTime.Parse(txtDataInceputLimitaCreditInternet.Text);

                    DateTime dataSfarsit = DateTime.Parse(txtDataSfarsitLimitaCreditInternet.Text);

                    string updateLimita = "UPDATE Limite_Card_Credit " +
                        "SET Data_Inceput_Limita = '" + dataInceput.ToString("yyyy'-'MM'-'dd") + "', Data_Sfarsit_Limita = '" + dataSfarsit.ToString("yyyy'-'MM'-'dd") + "', Limita_Noua = '" + txtLimitaNouaCreditInternet.Text + "' " +
                        "FROM Limite_Card_Credit INNER JOIN Limite ON Limite_Card_Credit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card_Credit = '" + gvCarduriCredit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Tranzactii internet'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }

                string selectDateNoi = "SELECT FORMAT(Data_Inceput_Limita, 'dd/MM/yyyy'), FORMAT(Data_Sfarsit_Limita, 'dd/MM/yyyy'), Limita_Noua " +
                "FROM Limite_Card_Credit " +
                "WHERE Nr_Card_Credit = '" + gvCarduriCredit.Rows[index].Cells[0].Text + "'";

                SqlCommand cmdDateNoi = new SqlCommand(selectDateNoi, conn);

                SqlDataAdapter adapterDateNoi = new SqlDataAdapter(cmdDateNoi);

                DataTable dtDateNoi = new DataTable();

                adapterDateNoi.Fill(dtDateNoi);


                txtDataInceputLimitaCreditInternet.Text = dtDateNoi.Rows[0][0].ToString();

                txtDataSfarsitLimitaCreditInternet.Text = dtDateNoi.Rows[0][1].ToString();

                txtLimitaNouaCreditInternet.Text = dtDateNoi.Rows[0][2].ToString();



                txtDataInceputLimitaCreditNumerar.Text = dtDateNoi.Rows[1][0].ToString();

                txtDataSfarsitLimitaCreditNumerar.Text = dtDateNoi.Rows[1][1].ToString();

                txtLimitaNouaCreditNumerar.Text = dtDateNoi.Rows[1][2].ToString();




                txtDataInceputLimitaCreditTranzactii.Text = dtDateNoi.Rows[2][0].ToString();

                txtDataSfarsitLimitaCreditTranzactii.Text = dtDateNoi.Rows[2][1].ToString();

                txtLimitaNouaCreditTranzactii.Text = dtDateNoi.Rows[2][2].ToString();




                txtDataInceputLimitaCreditPOS.Text = dtDateNoi.Rows[3][0].ToString();

                txtDataSfarsitLimitaCreditPOS.Text = dtDateNoi.Rows[3][1].ToString();

                txtLimitaNouaCreditPOS.Text = dtDateNoi.Rows[3][2].ToString();

                conn.Close();

            }

            i++;

            Session["nrInternetCredit"] = i;

        }

        protected void BtnAnulareCreditInternet_Click(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            txtDataSfarsitLimitaCreditInternet.Text = "";

            txtDataInceputLimitaCreditInternet.Text = "";

            txtLimitaNouaCreditInternet.Text = "";

        }

        protected void IbInceputCalendarInternetCredit_Click(object sender, ImageClickEventArgs e)
        {

            limiteCredit.Visible = true;

            cldSfarsitTranzactiiCredit.Visible = false;

            cldInceputInternetCredit.Visible = true;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

        }

        protected void IbSfarsitCalendarInternetCredit_Click(object sender, ImageClickEventArgs e)
        {

            limiteCredit.Visible = true;

            cldSfarsitTranzactiiCredit.Visible = false;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = true;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

        }

        protected void CldInceputInternetCredit_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputInternetCredit.Visible = true;

        }

        protected void CldInceputInternetCredit_SelectionChanged(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

            cldSfarsitTranzactiiCredit.Visible = false;


            if (txtDataSfarsitLimitaCreditInternet.Text == "" && cldInceputInternetCredit.SelectedDate >= DateTime.Now.Date)
            {

                txtDataInceputLimitaCreditInternet.Text = cldInceputInternetCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataSfarsitLimitaCreditInternet.Text == "" && cldInceputInternetCredit.SelectedDate < DateTime.Now.Date)
            {

                txtDataInceputLimitaCreditInternet.Text = "Data incorectă";

            }
            else if (cldInceputInternetCredit.SelectedDate < DateTime.Now.Date || cldInceputInternetCredit.SelectedDate > cldSfarsitInternetCredit.SelectedDate)
            {

                txtDataInceputLimitaCreditInternet.Text = "Data incorectă";

            }
            else
            {

                txtDataInceputLimitaCreditInternet.Text = cldInceputInternetCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void CldSfarsitInternetCredit_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteCredit.Visible = true;

            cldSfarsitInternetCredit.Visible = true;

        }

        protected void CldSfarsitInternetCredit_SelectionChanged(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

            cldSfarsitTranzactiiCredit.Visible = false;


            if (txtDataInceputLimitaCreditInternet.Text == "" && cldSfarsitInternetCredit.SelectedDate >= DateTime.Now.Date)
            {

                txtDataSfarsitLimitaCreditInternet.Text = cldSfarsitInternetCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataInceputLimitaCreditInternet.Text == "" && cldSfarsitInternetCredit.SelectedDate < DateTime.Now.Date)
            {

                txtDataSfarsitLimitaCreditInternet.Text = "Data incorectă";

            }
            else if (cldSfarsitInternetCredit.SelectedDate < DateTime.Now.Date || cldSfarsitInternetCredit.SelectedDate < cldInceputInternetCredit.SelectedDate)
            {

                txtDataSfarsitLimitaCreditInternet.Text = "Data incorectă";

            }
            else
            {

                txtDataSfarsitLimitaCreditInternet.Text = cldSfarsitInternetCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void BtnModificareCreditNumerar_Click(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            int i = 0;

            i += (int)Session["nrNumerarCredit"];

            if (i % 2 == 0)
            {

                btnAnulareCreditNumerar.Visible = true;

                btnModificareCreditNumerar.Text = "Salvează";

                ibInceputCalendarNumerarCredit.Visible = true;

                ibSfarsitCalendarNumerarCredit.Visible = true;

                txtLimitaNouaCreditNumerar.Enabled = true;

                txtLimitaNouaCreditNumerar.BackColor = System.Drawing.Color.White;

                txtDataInceputLimitaCreditNumerar.BackColor = System.Drawing.Color.White;

                txtDataSfarsitLimitaCreditNumerar.BackColor = System.Drawing.Color.White;

            }
            else
            {

                btnAnulareCreditNumerar.Visible = false;

                btnModificareCreditNumerar.Text = "Modifică";

                ibInceputCalendarNumerarCredit.Visible = false;

                ibSfarsitCalendarNumerarCredit.Visible = false;

                txtLimitaNouaCreditNumerar.Enabled = false;

                txtLimitaNouaCreditNumerar.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataInceputLimitaCreditNumerar.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataSfarsitLimitaCreditNumerar.BackColor = System.Drawing.Color.WhiteSmoke;

                int index = gvCarduriCredit.SelectedRow.RowIndex;

                if (txtDataInceputLimitaCreditNumerar.Text == "")
                {

                    txtDataSfarsitLimitaCreditNumerar.Text = "";

                    txtLimitaNouaCreditNumerar.Text = "";

                }

                if (txtDataSfarsitLimitaCreditNumerar.Text == "")
                {

                    txtDataInceputLimitaCreditNumerar.Text = "";

                    txtLimitaNouaCreditNumerar.Text = "";

                }

                if (txtLimitaNouaCreditNumerar.Text == "")
                {

                    txtDataInceputLimitaCreditNumerar.Text = "";

                    txtDataSfarsitLimitaCreditNumerar.Text = "";

                }

                conn.Open();

                if (txtDataInceputLimitaCreditNumerar.Text == "" && txtDataSfarsitLimitaCreditNumerar.Text == "" && txtLimitaNouaCreditNumerar.Text == "")
                {

                    string updateLimita = "UPDATE Limite_Card_Credit " +
                        "SET Data_Inceput_Limita = NULL, Data_Sfarsit_Limita = NULL, Limita_Noua = NULL " +
                        "FROM Limite_Card_Credit INNER JOIN Limite ON Limite_Card_Credit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card_Credit = '" + gvCarduriCredit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Ridicare numerar'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }
                else
                {

                    DateTime dataInceput = DateTime.Parse(txtDataInceputLimitaCreditNumerar.Text);

                    DateTime dataSfarsit = DateTime.Parse(txtDataSfarsitLimitaCreditNumerar.Text);

                    string updateLimita = "UPDATE Limite_Card_Credit " +
                        "SET Data_Inceput_Limita = '" + dataInceput.ToString("yyyy'-'MM'-'dd") + "', Data_Sfarsit_Limita = '" + dataSfarsit.ToString("yyyy'-'MM'-'dd") + "', Limita_Noua = '" + txtLimitaNouaCreditNumerar.Text + "' " +
                        "FROM Limite_Card_Credit INNER JOIN Limite ON Limite_Card_Credit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card_Credit = '" + gvCarduriCredit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Ridicare numerar'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }


                string selectDateNoi = "SELECT FORMAT(Data_Inceput_Limita, 'dd/MM/yyyy'), FORMAT(Data_Sfarsit_Limita, 'dd/MM/yyyy'), Limita_Noua " +
                "FROM Limite_Card_Credit " +
                "WHERE Nr_Card_Credit = '" + gvCarduriCredit.Rows[index].Cells[0].Text + "'";

                SqlCommand cmdDateNoi = new SqlCommand(selectDateNoi, conn);

                SqlDataAdapter adapterDateNoi = new SqlDataAdapter(cmdDateNoi);

                DataTable dtDateNoi = new DataTable();

                adapterDateNoi.Fill(dtDateNoi);


                txtDataInceputLimitaCreditInternet.Text = dtDateNoi.Rows[0][0].ToString();

                txtDataSfarsitLimitaCreditInternet.Text = dtDateNoi.Rows[0][1].ToString();

                txtLimitaNouaCreditInternet.Text = dtDateNoi.Rows[0][2].ToString();



                txtDataInceputLimitaCreditNumerar.Text = dtDateNoi.Rows[1][0].ToString();

                txtDataSfarsitLimitaCreditNumerar.Text = dtDateNoi.Rows[1][1].ToString();

                txtLimitaNouaCreditNumerar.Text = dtDateNoi.Rows[1][2].ToString();




                txtDataInceputLimitaCreditTranzactii.Text = dtDateNoi.Rows[2][0].ToString();

                txtDataSfarsitLimitaCreditTranzactii.Text = dtDateNoi.Rows[2][1].ToString();

                txtLimitaNouaCreditTranzactii.Text = dtDateNoi.Rows[2][2].ToString();




                txtDataInceputLimitaCreditPOS.Text = dtDateNoi.Rows[3][0].ToString();

                txtDataSfarsitLimitaCreditPOS.Text = dtDateNoi.Rows[3][1].ToString();

                txtLimitaNouaCreditPOS.Text = dtDateNoi.Rows[3][2].ToString();

                conn.Close();

            }

            i++;

            Session["nrNumerarCredit"] = i;

        }

        protected void BtnAnulareCreditNumerar_Click(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            txtDataSfarsitLimitaCreditNumerar.Text = "";

            txtDataInceputLimitaCreditNumerar.Text = "";

            txtLimitaNouaCreditNumerar.Text = "";

        }

        protected void IbInceputCalendarNumerarCredit_Click(object sender, ImageClickEventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputNumerarCredit.Visible = true;

            cldInceputInternetCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

            cldSfarsitTranzactiiCredit.Visible = false;

        }

        protected void IbSfarsitCalendarNumerarCredit_Click(object sender, ImageClickEventArgs e)
        {

            limiteCredit.Visible = true;

            cldSfarsitNumerarCredit.Visible = true;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

            cldSfarsitTranzactiiCredit.Visible = false;

        }

        protected void CldInceputNumerarCredit_SelectionChanged(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

            cldSfarsitTranzactiiCredit.Visible = false;


            if (txtDataSfarsitLimitaCreditNumerar.Text == "" && cldInceputNumerarCredit.SelectedDate >= DateTime.Now.Date)
            {

                txtDataInceputLimitaCreditNumerar.Text = cldInceputNumerarCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataSfarsitLimitaCreditNumerar.Text == "" && cldInceputNumerarCredit.SelectedDate < DateTime.Now.Date)
            {

                txtDataInceputLimitaCreditNumerar.Text = "Data incorectă";

            }
            else if (cldInceputNumerarCredit.SelectedDate < DateTime.Now.Date || cldInceputNumerarCredit.SelectedDate > cldSfarsitNumerarCredit.SelectedDate)
            {

                txtDataInceputLimitaCreditNumerar.Text = "Data incorectă";

            }
            else
            {

                txtDataInceputLimitaCreditNumerar.Text = cldInceputNumerarCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void CldInceputNumerarCredit_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputNumerarCredit.Visible = true;

        }

        protected void CldSfarsitNumerarCredit_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteCredit.Visible = true;

            cldSfarsitNumerarCredit.Visible = true;

        }

        protected void CldSfarsitNumerarCredit_SelectionChanged(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

            cldSfarsitTranzactiiCredit.Visible = false;


            if (txtDataInceputLimitaCreditNumerar.Text == "" && cldSfarsitNumerarCredit.SelectedDate >= DateTime.Now.Date)
            {

                txtDataSfarsitLimitaCreditNumerar.Text = cldSfarsitNumerarCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataInceputLimitaCreditNumerar.Text == "" && cldSfarsitNumerarCredit.SelectedDate < DateTime.Now.Date)
            {

                txtDataSfarsitLimitaCreditNumerar.Text = "Data incorectă";

            }
            else if (cldSfarsitNumerarCredit.SelectedDate < DateTime.Now.Date || cldSfarsitNumerarCredit.SelectedDate < cldInceputNumerarCredit.SelectedDate)
            {

                txtDataSfarsitLimitaCreditNumerar.Text = "Data incorectă";

            }
            else
            {

                txtDataSfarsitLimitaCreditNumerar.Text = cldSfarsitNumerarCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void BtnModificareCreditTranzactii_Click(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            int i = 0;

            i += (int)Session["nrTranzactiiCredit"];

            if (i % 2 == 0)
            {

                btnAnulareCreditTranzactii.Visible = true;

                btnModificareCreditTranzactii.Text = "Salvează";

                ibInceputCalendarTranzactiiCredit.Visible = true;

                ibSfarsitCalendarTranzactiiCredit.Visible = true;

                txtLimitaNouaCreditTranzactii.Enabled = true;

                txtLimitaNouaCreditTranzactii.BackColor = System.Drawing.Color.White;

                txtDataInceputLimitaCreditTranzactii.BackColor = System.Drawing.Color.White;

                txtDataSfarsitLimitaCreditTranzactii.BackColor = System.Drawing.Color.White;

            }
            else
            {

                btnAnulareCreditTranzactii.Visible = false;

                btnModificareCreditTranzactii.Text = "Modifică";

                ibInceputCalendarTranzactiiCredit.Visible = false;

                ibSfarsitCalendarTranzactiiCredit.Visible = false;

                txtLimitaNouaCreditTranzactii.Enabled = false;

                txtLimitaNouaCreditTranzactii.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataInceputLimitaCreditTranzactii.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataSfarsitLimitaCreditTranzactii.BackColor = System.Drawing.Color.WhiteSmoke;

                int index = gvCarduriCredit.SelectedRow.RowIndex;

                if (txtDataInceputLimitaCreditTranzactii.Text == "")
                {

                    txtDataSfarsitLimitaCreditTranzactii.Text = "";

                    txtLimitaNouaCreditTranzactii.Text = "";

                }

                if (txtDataSfarsitLimitaCreditTranzactii.Text == "")
                {

                    txtDataInceputLimitaCreditTranzactii.Text = "";

                    txtLimitaNouaCreditTranzactii.Text = "";

                }

                if (txtLimitaNouaCreditTranzactii.Text == "")
                {

                    txtDataInceputLimitaCreditTranzactii.Text = "";

                    txtDataSfarsitLimitaCreditTranzactii.Text = "";

                }

                conn.Open();

                if (txtDataInceputLimitaCreditTranzactii.Text == "" && txtDataSfarsitLimitaCreditTranzactii.Text == "" && txtLimitaNouaCreditTranzactii.Text == "")
                {

                    string updateLimita = "UPDATE Limite_Card_Credit " +
                        "SET Data_Inceput_Limita = NULL, Data_Sfarsit_Limita = NULL, Limita_Noua = NULL " +
                        "FROM Limite_Card_Credit INNER JOIN Limite ON Limite_Card_Credit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card_Credit = '" + gvCarduriCredit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Tranzactii zilnice'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }
                else
                {

                    DateTime dataInceput = DateTime.Parse(txtDataInceputLimitaCreditTranzactii.Text);

                    DateTime dataSfarsit = DateTime.Parse(txtDataSfarsitLimitaCreditTranzactii.Text);

                    string updateLimita = "UPDATE Limite_Card_Credit " +
                        "SET Data_Inceput_Limita = '" + dataInceput.ToString("yyyy'-'MM'-'dd") + "', Data_Sfarsit_Limita = '" + dataSfarsit.ToString("yyyy'-'MM'-'dd") + "', Limita_Noua = '" + txtLimitaNouaCreditTranzactii.Text + "' " +
                        "FROM Limite_Card_Credit INNER JOIN Limite ON Limite_Card_Credit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card_Credit = '" + gvCarduriCredit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Tranzactii zilnice'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }


                string selectDateNoi = "SELECT FORMAT(Data_Inceput_Limita, 'dd/MM/yyyy'), FORMAT(Data_Sfarsit_Limita, 'dd/MM/yyyy'), Limita_Noua " +
                "FROM Limite_Card_Credit " +
                "WHERE Nr_Card_Credit = '" + gvCarduriCredit.Rows[index].Cells[0].Text + "'";

                SqlCommand cmdDateNoi = new SqlCommand(selectDateNoi, conn);

                SqlDataAdapter adapterDateNoi = new SqlDataAdapter(cmdDateNoi);

                DataTable dtDateNoi = new DataTable();

                adapterDateNoi.Fill(dtDateNoi);


                txtDataInceputLimitaCreditInternet.Text = dtDateNoi.Rows[0][0].ToString();

                txtDataSfarsitLimitaCreditInternet.Text = dtDateNoi.Rows[0][1].ToString();

                txtLimitaNouaCreditInternet.Text = dtDateNoi.Rows[0][2].ToString();



                txtDataInceputLimitaCreditNumerar.Text = dtDateNoi.Rows[1][0].ToString();

                txtDataSfarsitLimitaCreditNumerar.Text = dtDateNoi.Rows[1][1].ToString();

                txtLimitaNouaCreditNumerar.Text = dtDateNoi.Rows[1][2].ToString();




                txtDataInceputLimitaCreditTranzactii.Text = dtDateNoi.Rows[2][0].ToString();

                txtDataSfarsitLimitaCreditTranzactii.Text = dtDateNoi.Rows[2][1].ToString();

                txtLimitaNouaCreditTranzactii.Text = dtDateNoi.Rows[2][2].ToString();




                txtDataInceputLimitaCreditPOS.Text = dtDateNoi.Rows[3][0].ToString();

                txtDataSfarsitLimitaCreditPOS.Text = dtDateNoi.Rows[3][1].ToString();

                txtLimitaNouaCreditPOS.Text = dtDateNoi.Rows[3][2].ToString();

                conn.Close();

            }

            i++;

            Session["nrTranzactiiCredit"] = i;

        }

        protected void BtnAnulareCreditTranzactii_Click(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            txtDataSfarsitLimitaCreditTranzactii.Text = "";

            txtDataInceputLimitaCreditTranzactii.Text = "";

            txtLimitaNouaCreditTranzactii.Text = "";

        }

        protected void IbInceputCalendarTranzactiiCredit_Click(object sender, ImageClickEventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputTranzactiiCredit.Visible = true;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

            cldSfarsitTranzactiiCredit.Visible = false;

        }

        protected void IbSfarsitCalendarTranzactiiCredit_Click(object sender, ImageClickEventArgs e)
        {

            limiteCredit.Visible = true;

            cldSfarsitTranzactiiCredit.Visible = true;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

        }

        protected void CldInceputTranzactiiCredit_SelectionChanged(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

            cldSfarsitTranzactiiCredit.Visible = false;


            if (txtDataSfarsitLimitaCreditTranzactii.Text == "" && cldInceputTranzactiiCredit.SelectedDate >= DateTime.Now.Date)
            {

                txtDataInceputLimitaCreditTranzactii.Text = cldInceputTranzactiiCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataSfarsitLimitaCreditTranzactii.Text == "" && cldInceputTranzactiiCredit.SelectedDate < DateTime.Now.Date)
            {

                txtDataInceputLimitaCreditTranzactii.Text = "Data incorectă";

            }
            else if (cldInceputTranzactiiCredit.SelectedDate < DateTime.Now.Date || cldInceputTranzactiiCredit.SelectedDate > cldSfarsitTranzactiiCredit.SelectedDate)
            {

                txtDataInceputLimitaCreditTranzactii.Text = "Data incorectă";

            }
            else
            {

                txtDataInceputLimitaCreditTranzactii.Text = cldInceputTranzactiiCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void CldSfarsitTranzactiiCredit_SelectionChanged(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

            cldSfarsitTranzactiiCredit.Visible = false;


            if (txtDataInceputLimitaCreditTranzactii.Text == "" && cldSfarsitTranzactiiCredit.SelectedDate >= DateTime.Now.Date)
            {

                txtDataSfarsitLimitaCreditTranzactii.Text = cldSfarsitTranzactiiCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataInceputLimitaCreditTranzactii.Text == "" && cldSfarsitTranzactiiCredit.SelectedDate < DateTime.Now.Date)
            {

                txtDataSfarsitLimitaCreditTranzactii.Text = "Data incorectă";

            }
            else if (cldSfarsitTranzactiiCredit.SelectedDate < DateTime.Now.Date || cldSfarsitTranzactiiCredit.SelectedDate < cldInceputTranzactiiCredit.SelectedDate)
            {

                txtDataSfarsitLimitaCreditTranzactii.Text = "Data incorectă";

            }
            else
            {

                txtDataSfarsitLimitaCreditTranzactii.Text = cldSfarsitTranzactiiCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void CldSfarsitTranzactiiCredit_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteCredit.Visible = true;

            cldSfarsitTranzactiiCredit.Visible = true;

        }

        protected void CldInceputTranzactiiCredit_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputTranzactiiCredit.Visible = true;

        }

        protected void BtnModificareCreditPOS_Click(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            int i = 0;

            i += (int)Session["nrPOSCredit"];

            if (i % 2 == 0)
            {

                btnAnulareCreditPOS.Visible = true;

                btnModificareCreditPOS.Text = "Salvează";

                ibInceputCalendarPOSCredit.Visible = true;

                ibSfarsitCalendarPOSCredit.Visible = true;

                txtLimitaNouaCreditPOS.Enabled = true;

                txtLimitaNouaCreditPOS.BackColor = System.Drawing.Color.White;

                txtDataInceputLimitaCreditPOS.BackColor = System.Drawing.Color.White;

                txtDataSfarsitLimitaCreditPOS.BackColor = System.Drawing.Color.White;

            }
            else
            {

                btnAnulareCreditPOS.Visible = false;

                btnModificareCreditPOS.Text = "Modifică";

                ibInceputCalendarPOSCredit.Visible = false;

                ibSfarsitCalendarPOSCredit.Visible = false;

                txtLimitaNouaCreditPOS.Enabled = false;

                txtLimitaNouaCreditPOS.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataInceputLimitaCreditPOS.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataSfarsitLimitaCreditPOS.BackColor = System.Drawing.Color.WhiteSmoke;

                int index = gvCarduriCredit.SelectedRow.RowIndex;

                if (txtDataInceputLimitaCreditPOS.Text == "")
                {

                    txtDataSfarsitLimitaCreditPOS.Text = "";

                    txtLimitaNouaCreditPOS.Text = "";

                }

                if (txtDataSfarsitLimitaCreditPOS.Text == "")
                {

                    txtDataInceputLimitaCreditPOS.Text = "";

                    txtLimitaNouaCreditPOS.Text = "";

                }

                if (txtLimitaNouaCreditPOS.Text == "")
                {

                    txtDataInceputLimitaCreditPOS.Text = "";

                    txtDataSfarsitLimitaCreditPOS.Text = "";

                }

                conn.Open();

                if (txtDataInceputLimitaCreditPOS.Text == "" && txtDataSfarsitLimitaCreditPOS.Text == "" && txtLimitaNouaCreditPOS.Text == "")
                {

                    string updateLimita = "UPDATE Limite_Card_Credit " +
                        "SET Data_Inceput_Limita = NULL, Data_Sfarsit_Limita = NULL, Limita_Noua = NULL " +
                        "FROM Limite_Card_Credit INNER JOIN Limite ON Limite_Card_Credit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card_Credit = '" + gvCarduriCredit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Tranzactii POS'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }
                else
                {

                    DateTime dataInceput = DateTime.Parse(txtDataInceputLimitaCreditPOS.Text);

                    DateTime dataSfarsit = DateTime.Parse(txtDataSfarsitLimitaCreditPOS.Text);

                    string updateLimita = "UPDATE Limite_Card_Credit " +
                        "SET Data_Inceput_Limita = '" + dataInceput.ToString("yyyy'-'MM'-'dd") + "', Data_Sfarsit_Limita = '" + dataSfarsit.ToString("yyyy'-'MM'-'dd") + "', Limita_Noua = '" + txtLimitaNouaCreditPOS.Text + "' " +
                        "FROM Limite_Card_Credit INNER JOIN Limite ON Limite_Card_Credit.ID_Limita = Limite.ID_Limita " +
                        "WHERE Nr_Card_Credit = '" + gvCarduriCredit.Rows[index].Cells[0].Text + "' AND Tip_Limita = 'Tranzactii POS'";

                    SqlCommand cmdLimita = new SqlCommand(updateLimita, conn);

                    cmdLimita.ExecuteNonQuery();

                }


                string selectDateNoi = "SELECT FORMAT(Data_Inceput_Limita, 'dd/MM/yyyy'), FORMAT(Data_Sfarsit_Limita, 'dd/MM/yyyy'), Limita_Noua " +
                "FROM Limite_Card_Credit " +
                "WHERE Nr_Card_Credit = '" + gvCarduriCredit.Rows[index].Cells[0].Text + "'";

                SqlCommand cmdDateNoi = new SqlCommand(selectDateNoi, conn);

                SqlDataAdapter adapterDateNoi = new SqlDataAdapter(cmdDateNoi);

                DataTable dtDateNoi = new DataTable();

                adapterDateNoi.Fill(dtDateNoi);


                txtDataInceputLimitaCreditInternet.Text = dtDateNoi.Rows[0][0].ToString();

                txtDataSfarsitLimitaCreditInternet.Text = dtDateNoi.Rows[0][1].ToString();

                txtLimitaNouaCreditInternet.Text = dtDateNoi.Rows[0][2].ToString();



                txtDataInceputLimitaCreditNumerar.Text = dtDateNoi.Rows[1][0].ToString();

                txtDataSfarsitLimitaCreditNumerar.Text = dtDateNoi.Rows[1][1].ToString();

                txtLimitaNouaCreditNumerar.Text = dtDateNoi.Rows[1][2].ToString();




                txtDataInceputLimitaCreditTranzactii.Text = dtDateNoi.Rows[2][0].ToString();

                txtDataSfarsitLimitaCreditTranzactii.Text = dtDateNoi.Rows[2][1].ToString();

                txtLimitaNouaCreditTranzactii.Text = dtDateNoi.Rows[2][2].ToString();




                txtDataInceputLimitaCreditPOS.Text = dtDateNoi.Rows[3][0].ToString();

                txtDataSfarsitLimitaCreditPOS.Text = dtDateNoi.Rows[3][1].ToString();

                txtLimitaNouaCreditPOS.Text = dtDateNoi.Rows[3][2].ToString();

                conn.Close();

            }

            i++;

            Session["nrPOSCredit"] = i;

        }

        protected void BtnAnulareCreditPOS_Click(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            txtDataSfarsitLimitaCreditPOS.Text = "";

            txtDataInceputLimitaCreditPOS.Text = "";

            txtLimitaNouaCreditPOS.Text = "";

        }

        protected void IbInceputCalendarPOSCredit_Click(object sender, ImageClickEventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputPOSCredit.Visible = true;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

            cldSfarsitTranzactiiCredit.Visible = false;

        }

        protected void IbSfarsitCalendarPOSCredit_Click(object sender, ImageClickEventArgs e)
        {

            limiteCredit.Visible = true;

            cldSfarsitInternetCredit.Visible = false;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = true;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitTranzactiiCredit.Visible = false;

        }

        protected void CldInceputPOSCredit_SelectionChanged(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

            cldSfarsitTranzactiiCredit.Visible = false;


            if (txtDataSfarsitLimitaCreditPOS.Text == "" && cldInceputPOSCredit.SelectedDate >= DateTime.Now.Date)
            {

                txtDataInceputLimitaCreditPOS.Text = cldInceputPOSCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataSfarsitLimitaCreditPOS.Text == "" && cldInceputPOSCredit.SelectedDate < DateTime.Now.Date)
            {

                txtDataInceputLimitaCreditPOS.Text = "Data incorectă";

            }
            else if (cldInceputPOSCredit.SelectedDate < DateTime.Now.Date || cldInceputPOSCredit.SelectedDate > cldSfarsitPOSCredit.SelectedDate)
            {

                txtDataInceputLimitaCreditPOS.Text = "Data incorectă";

            }
            else
            {

                txtDataInceputLimitaCreditPOS.Text = cldInceputPOSCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void CldSfarsitPOSCredit_SelectionChanged(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

            cldSfarsitTranzactiiCredit.Visible = false;


            if (txtDataInceputLimitaCreditPOS.Text == "" && cldSfarsitPOSCredit.SelectedDate >= DateTime.Now.Date)
            {

                txtDataSfarsitLimitaCreditPOS.Text = cldSfarsitPOSCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataInceputLimitaCreditPOS.Text == "" && cldSfarsitPOSCredit.SelectedDate < DateTime.Now.Date)
            {

                txtDataSfarsitLimitaCreditPOS.Text = "Data incorectă";

            }
            else if (cldSfarsitPOSCredit.SelectedDate < DateTime.Now.Date || cldSfarsitPOSCredit.SelectedDate < cldInceputPOSCredit.SelectedDate)
            {

                txtDataSfarsitLimitaCreditPOS.Text = "Data incorectă";

            }
            else
            {

                txtDataSfarsitLimitaCreditPOS.Text = cldSfarsitPOSCredit.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void CldInceputPOSCredit_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputPOSCredit.Visible = true;

        }

        protected void CldSfarsitPOSCredit_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            limiteCredit.Visible = true;

            cldSfarsitPOSCredit.Visible = true;

        }

        protected void BtnLimiteInapoiCredit_Click(object sender, EventArgs e)
        {

            limiteCredit.Visible = false;

            cardurileMele.Visible = true;

        }

        protected void BtnLimiteCredit_Click(object sender, EventArgs e)
        {

            limiteCredit.Visible = true;

            cldInceputInternetCredit.Visible = false;

            cldInceputNumerarCredit.Visible = false;

            cldInceputPOSCredit.Visible = false;

            cldInceputTranzactiiCredit.Visible = false;

            cldSfarsitInternetCredit.Visible = false;

            cldSfarsitNumerarCredit.Visible = false;

            cldSfarsitPOSCredit.Visible = false;

            cldSfarsitTranzactiiCredit.Visible = false;

            ibInceputCalendarInternetCredit.Visible = false;

            ibInceputCalendarNumerarCredit.Visible = false;

            ibInceputCalendarPOSCredit.Visible = false;

            ibInceputCalendarTranzactiiCredit.Visible = false;

            ibSfarsitCalendarInternetCredit.Visible = false;

            ibSfarsitCalendarNumerarCredit.Visible = false;

            ibSfarsitCalendarPOSCredit.Visible = false;

            ibSfarsitCalendarTranzactiiCredit.Visible = false;

            btnAnulareCreditInternet.Visible = false;

            btnAnulareCreditNumerar.Visible = false;

            btnAnulareCreditPOS.Visible = false;

            btnAnulareCreditTranzactii.Visible = false;


            txtLimitaNouaCreditInternet.Enabled = false;

            txtLimitaNouaCreditNumerar.Enabled = false;

            txtLimitaNouaCreditPOS.Enabled = false;

            txtLimitaNouaCreditTranzactii.Enabled = false;


            conn.Open();

            string selectDateLimite = "SELECT Tip_Limita, Limita_Standard " +
                "FROM Limite ";

            SqlCommand selectDL = new SqlCommand(selectDateLimite, conn);

            SqlDataAdapter adapterDL = new SqlDataAdapter(selectDL);

            DataTable dtDL = new DataTable();

            adapterDL.Fill(dtDL);

            txtTipLimitaCreditInternet.Text = dtDL.Rows[0][0].ToString();

            txtTipLimitaCreditNumerar.Text = dtDL.Rows[1][0].ToString();

            txtTipLimitaCreditTranzactii.Text = dtDL.Rows[2][0].ToString();

            txtTipLimitaCreditPOS.Text = dtDL.Rows[3][0].ToString();

            txtLimitaStandardCreditInternet.Text = dtDL.Rows[0][1].ToString();

            txtLimitaStandardCreditNumerar.Text = dtDL.Rows[1][1].ToString();

            txtLimitaStandardCreditTranzactii.Text = dtDL.Rows[2][1].ToString();

            txtLimitaStandardCreditPOS.Text = dtDL.Rows[3][1].ToString();




            int index = gvCarduriCredit.SelectedRow.RowIndex;

            string selectDateNoi = "SELECT FORMAT(Data_Inceput_Limita, 'dd/MM/yyyy'), FORMAT(Data_Sfarsit_Limita, 'dd/MM/yyyy'), Limita_Noua " +
                "FROM Limite_Card_Credit " +
                "WHERE Nr_Card_Credit = '" + gvCarduriCredit.Rows[index].Cells[0].Text + "'";

            SqlCommand cmdDateNoi = new SqlCommand(selectDateNoi, conn);

            SqlDataAdapter adapterDateNoi = new SqlDataAdapter(cmdDateNoi);

            DataTable dtDateNoi = new DataTable();

            adapterDateNoi.Fill(dtDateNoi);


            txtDataInceputLimitaCreditInternet.Text = dtDateNoi.Rows[0][0].ToString();

            txtDataSfarsitLimitaCreditInternet.Text = dtDateNoi.Rows[0][1].ToString();

            txtLimitaNouaCreditInternet.Text = dtDateNoi.Rows[0][2].ToString();



            txtDataInceputLimitaCreditNumerar.Text = dtDateNoi.Rows[1][0].ToString();

            txtDataSfarsitLimitaCreditNumerar.Text = dtDateNoi.Rows[1][1].ToString();

            txtLimitaNouaCreditNumerar.Text = dtDateNoi.Rows[1][2].ToString();




            txtDataInceputLimitaCreditTranzactii.Text = dtDateNoi.Rows[2][0].ToString();

            txtDataSfarsitLimitaCreditTranzactii.Text = dtDateNoi.Rows[2][1].ToString();

            txtLimitaNouaCreditTranzactii.Text = dtDateNoi.Rows[2][2].ToString();




            txtDataInceputLimitaCreditPOS.Text = dtDateNoi.Rows[3][0].ToString();

            txtDataSfarsitLimitaCreditPOS.Text = dtDateNoi.Rows[3][1].ToString();

            txtLimitaNouaCreditPOS.Text = dtDateNoi.Rows[3][2].ToString();

            conn.Close();

        }

        protected void BtnDetaliiCredit_Click(object sender, EventArgs e)
        {

            cardurileMele.Visible = false;

            vizualizareCardCredit.Visible = true;

            txtDenumireProdusCredit.Text = "Mastercard";

            int index = gvCarduriCredit.SelectedRow.RowIndex;

            conn.Open();

            string selectCardCredit = "SELECT Nr_Card_Credit, FORMAT(Data_Expirare,'MM'),FORMAT(Data_Expirare,'yy'), CVV, Limita, Card_Activ, IBAN_Cont, Sold_Card " +
                "FROM Carduri_Credit " +
                "WHERE Carduri_Credit.Nr_Card_Credit = '" + gvCarduriCredit.Rows[index].Cells[0].Text + "'";

            SqlCommand selectCD = new SqlCommand(selectCardCredit, conn);

            SqlDataReader reader = selectCD.ExecuteReader();

            reader.Read();

            string nrCard = reader[0].ToString();

            txtNumarCardCredit.Text = nrCard.Substring(0, 4) + " " + nrCard.Substring(3, 4) + " " + nrCard.Substring(7, 4) + " " + nrCard.Substring(11, 4);

            txtCVVCredit.Text = reader[3].ToString();

            txtIbanAtasatCredit.Text = reader[6].ToString();

            txtNumeTitularCredit.Text = nameLbl.Text;

            txtDataExpirareCredit.Text = reader[1].ToString() + "/" + reader[2].ToString();

            txtLimitaCardCredit.Text = reader[4].ToString();

            txtSoldCardCredit.Text = reader[7].ToString();

            lblNumarCardFizicCredit.Text = nrCard.Substring(0, 4) + " " + nrCard.Substring(3, 4) + " " + nrCard.Substring(7, 4) + " " + nrCard.Substring(11, 4);

            lblNumeTitularFizicCredit.Text = nameLbl.Text;

            lblCVVFizicCredit.Text = "CVV: " + reader[3].ToString();

            lblDataExpirareFizicCredit.Text = "EXP: " + reader[1].ToString() + "/" + reader[2].ToString();

            reader.Close();

            conn.Close();

        }

        protected void BtnExportPdfIstoric_Click(object sender, EventArgs e)
        {

            string selectedPath = "";

            Thread t = new Thread((ThreadStart)(() =>
            {
                SaveFileDialog savePdf = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf|*.pdf",
                    FilterIndex = 2,
                    RestoreDirectory = true
                };

                if (savePdf.ShowDialog() == DialogResult.OK)
                {

                    selectedPath = savePdf.FileName;

                }

                string template = "D:\\Disertatie\\Templates\\template.pdf";

                PdfReader readerPdf = new PdfReader(template);

                Rectangle size = readerPdf.GetPageSizeWithRotation(1);

                Document pdfFile = new Document(size);

                FileStream fs = new FileStream(selectedPath, FileMode.Create, FileAccess.Write); ;

                PdfWriter writer = PdfWriter.GetInstance(pdfFile, fs);

                pdfFile.Open();

                PdfContentByte cb = writer.DirectContent;

                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb.SetColorFill(BaseColor.DARK_GRAY);
                cb.SetFontAndSize(bf, 8);

                string background_pdf = "D:\\Disertatie\\Poze\\template_background.png";

                iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(background_pdf);

                png.SetAbsolutePosition(0, 0);

                png.ScaleToFit(pdfFile.PageSize.Width, pdfFile.PageSize.Height);

                pdfFile.Add(png);

                BaseFont customfont = BaseFont.CreateFont("D:\\Disertatie\\Templates\\calibri.ttf", BaseFont.CP1252, BaseFont.EMBEDDED);

                Font generalFont = new Font(customfont, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT, new Phrase("SWIFT: RBRLRO22", generalFont), 220, 742, 0);

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT, new Phrase("C.U.I. RO 50 22 670", generalFont), 220, 730, 0);

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT, new Phrase("R.B. - P.J.R. - 12 - 019 - 05.05.2022", generalFont), 220, 718, 0);

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT, new Phrase("Nr. Inreg. Reg. Com.: J12 / 4155 / 2022", generalFont), 220, 706, 0);


                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_RIGHT, new Phrase("Imprimat de: " + nameLbl.Text.ToUpper() + "", generalFont), 550, 718, 0);

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_RIGHT, new Phrase("În data de: " + DateTime.Now.Date.ToString("dd'/'MM'/'yyyy") + "", generalFont), 550, 706, 0);




                DataTable dt = new DataTable();


                if (rblPerioadaDebit.SelectedIndex == 0)
                {

                    if (dlTipTranzactieIstoricDebit.SelectedIndex == 0)
                    {

                        conn.Open();

                        string sql = "SELECT Nr_Card AS [Numar card], Tip_Operatiune AS [Tip tranzactie], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Data], Suma_Operatiune AS [Suma] " +
                            "FROM Operatiuni " +
                            "WHERE Nr_Card = '" + dlCardDebit.Text + "'";

                        SqlCommand cmd = new SqlCommand(sql, conn);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        adapter.Fill(dt);

                        conn.Close();

                    }
                    else
                    {

                        conn.Open();

                        string sql = "SELECT Nr_Card AS [Numar card], Tip_Operatiune AS [Tip tranzactie], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Data], Suma_Operatiune AS [Suma] " +
                            "FROM Operatiuni " +
                            "WHERE Nr_Card = '" + dlCardDebit.Text + "' AND Tip_Operatiune = '" + dlTipTranzactieIstoricDebit.Text + "'";

                        SqlCommand cmd = new SqlCommand(sql, conn);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        adapter.Fill(dt);

                        conn.Close();

                    }


                }
                else if (rblPerioadaDebit.SelectedIndex == 1)
                {

                    if (dlTipTranzactieIstoricDebit.SelectedIndex == 0)
                    {

                        conn.Open();

                        string sql = "SELECT Nr_Card AS [Numar card], Tip_Operatiune AS [Tip tranzactie], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Data], Suma_Operatiune AS [Suma] " +
                            "FROM Operatiuni " +
                            "WHERE Nr_Card = '" + dlCardDebit.Text + "' AND Data_Operatiune BETWEEN '" + DateTime.Parse(txtDataInceputIstoricDebit.Text).ToString("yyyy'-'MM'-'dd") + "' AND '" + DateTime.Parse(txtDataSfarsitIstoricDebit.Text).ToString("yyyy'-'MM'-'dd") + "'";

                        SqlCommand cmd = new SqlCommand(sql, conn);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        adapter.Fill(dt);

                        conn.Close();

                    }
                    else
                    {

                        conn.Open();

                        string sql = "SELECT Nr_Card AS [Numar card], Tip_Operatiune AS [Tip tranzactie], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Data], Suma_Operatiune AS [Suma] " +
                            "FROM Operatiuni " +
                            "WHERE Nr_Card = '" + dlCardDebit.Text + "' AND Tip_Operatiune = '" + dlTipTranzactieIstoricDebit.Text + "' AND Data_Operatiune BETWEEN '" + DateTime.Parse(txtDataInceputIstoricDebit.Text).ToString("yyyy'-'MM'-'dd") + "' AND '" + DateTime.Parse(txtDataSfarsitIstoricDebit.Text).ToString("yyyy'-'MM'-'dd") + "'";

                        SqlCommand cmd = new SqlCommand(sql, conn);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        adapter.Fill(dt);

                        conn.Close();

                    }


                }


                Font headerFont = new Font(customfont, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                Font titleFont = new Font(customfont, 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK);


                Paragraph titleParagraph = new Paragraph
                {
                    Alignment = Element.ALIGN_CENTER,

                    Font = titleFont

                };

                titleParagraph.Add("ISTORIC CARD");



                Paragraph criteriiCautare = new Paragraph
                {
                    Alignment = Element.ALIGN_LEFT,

                    Font = headerFont
                };

                criteriiCautare.Add("Criterii de cautare utilizate:");

                ColumnText subtitluCriterii = new ColumnText(cb);

                subtitluCriterii.SetSimpleColumn(42, 560, 500, 100);

                subtitluCriterii.AddText(criteriiCautare);

                subtitluCriterii.Go();




                Paragraph listaCriterii = new Paragraph
                {
                    Alignment = Element.ALIGN_LEFT,

                    Font = generalFont
                };

                string nrCard = dlCardDebit.Text;

                listaCriterii.Add("Card:                    " + nrCard.Substring(0, 4) + " " + nrCard.Substring(3, 4) + " " + nrCard.Substring(7, 4) + " " + nrCard.Substring(11, 4) + "\n");

                if (rblPerioadaDebit.SelectedIndex == 0)
                {

                    listaCriterii.Add("Perioada:            Nedeterminata \n");

                }
                else
                {

                    listaCriterii.Add("Perioada:            " + txtDataInceputIstoricDebit.Text + "  -  " + txtDataSfarsitIstoricDebit.Text + "\n");

                }


                if (dlTipTranzactieIstoricDebit.SelectedIndex == 0)
                {

                    listaCriterii.Add("Tip tranzactie:    Toate \n");

                }
                else
                {

                    listaCriterii.Add("Tip tranzactie:    " + dlTipTranzactieIstoricDebit.Text + "\n");

                }


                ColumnText listaC = new ColumnText(cb);

                listaC.SetSimpleColumn(42, 540, 500, 100);

                listaC.AddText(listaCriterii);

                listaC.Go();




                Paragraph tableParagraph = new Paragraph();

                PdfPTable istoricTable = new PdfPTable(dt.Columns.Count);


                PdfPCell pdfCell = new PdfPCell();

                for (int i = 0; i <= dt.Columns.Count - 1; i++)
                {

                    pdfCell = new PdfPCell(new Phrase(new Chunk(dt.Columns[i].ColumnName, headerFont)));

                    pdfCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;

                    pdfCell.BackgroundColor = new BaseColor(System.Drawing.Color.FromArgb(169, 200, 221));

                    pdfCell.FixedHeight = 20F;

                    istoricTable.AddCell(pdfCell);

                }

                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {

                    for (int j = 0; j <= dt.Columns.Count - 1; j++)
                    {

                        pdfCell = new PdfPCell(new Phrase(dt.Rows[i][j].ToString(), generalFont));

                        pdfCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;

                        istoricTable.AddCell(pdfCell);

                    }

                }


                titleParagraph.SpacingBefore = 200F;

                criteriiCautare.SpacingBefore = 50F;

                tableParagraph.SpacingBefore = 150F;

                istoricTable.WidthPercentage = 98;

                tableParagraph.Add(istoricTable);

                pdfFile.Add(titleParagraph);

                pdfFile.Add(tableParagraph);

                pdfFile.Close();

                fs.Close();

                writer.Close();

                readerPdf.Close();

            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            System.Diagnostics.Process.Start(selectedPath);

        }

        protected void ThirdBtn1_Click(object sender, EventArgs e)
        {

            platiIntreConturi.Visible = true;

            txtTransferDinContul.Text = accountDl.Text;

            txtDetaliiTransferIntreConturi.Text = "";

            txtSumaTransferIntreConturi.Text = "";

            lblEroareTransferIntreConturi.Visible = false;

            string valuta = txtTransferDinContul.Text.Substring(8, 3);

            conn.Open();

            string codClient = (string)Session["codClient"];

            string sql = "SELECT * " +
                "FROM Conturi " +
                "WHERE Cod_Client = '" + codClient + "' AND SUBSTRING(IBAN_Cont, 9, 3) = '" + valuta + "' AND IBAN_Cont <> '" + txtTransferDinContul.Text + "'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            dlTransferCatreContul.DataSource = dt;

            dlTransferCatreContul.DataTextField = "IBAN_Cont";

            dlTransferCatreContul.DataBind();

            conn.Close();

        }


        protected void BtnFinalizareTransferIntreConturi_Click(object sender, EventArgs e)
        {

            if (dlTransferCatreContul.Text == "" || txtSumaTransferIntreConturi.Text == "" || txtDetaliiTransferIntreConturi.Text == "")
            {

                lblEroareTransferIntreConturi.Visible = true;

                platiIntreConturi.Visible = true;

            }
            else
            {

                lblEroareTransferIntreConturi.Visible = false;

                conn.Open();

                string updateContPrincipal = "UPDATE Conturi " +
                    "SET Sold = Sold - " + txtSumaTransferIntreConturi.Text + " " +
                    "WHERE IBAN_Cont = '" + txtTransferDinContul.Text + "'";

                SqlCommand cmdUP = new SqlCommand(updateContPrincipal, conn);

                cmdUP.ExecuteNonQuery();

                string updateContSecundar = "UPDATE Conturi " +
                    "SET Sold = Sold + " + txtSumaTransferIntreConturi.Text + " " +
                    "WHERE IBAN_Cont = '" + dlTransferCatreContul.Text + "'";

                SqlCommand cmdUS = new SqlCommand(updateContSecundar, conn);

                cmdUS.ExecuteNonQuery();


                string contDestinatar = dlTransferCatreContul.Text;

                string contPlatitor = txtTransferDinContul.Text;

                if (contDestinatar.Substring(11,3) == "ECN")
                {

                    string selectSold = "SELECT Sold " +
                        "FROM Conturi " +
                        "WHERE IBAN_Cont = '" + contDestinatar + "'";

                    SqlCommand cmdSold = new SqlCommand(selectSold, conn);

                    SqlDataReader readerSold = cmdSold.ExecuteReader();

                    readerSold.Read();

                    decimal sold = readerSold.GetDecimal(0);

                    readerSold.Close();


                    string selectDobanda = "SELECT ID_Dobanda " +
                        "FROM Dobanda_Conturi " +
                        "WHERE " + sold + " >= Suma_Minima AND " + sold + " <= Suma_Maxima";

                    SqlCommand cmdDobanda = new SqlCommand(selectDobanda, conn);

                    SqlDataReader readerDobanda = cmdDobanda.ExecuteReader();

                    readerDobanda.Read();

                    int idDobanda = readerDobanda.GetInt32(0);

                    readerDobanda.Close();


                    string updateDobanda = "UPDATE Conturi " +
                        "SET ID_Dobanda = " + idDobanda + " " +
                        "WHERE IBAN_Cont = '" + contDestinatar + "'";

                    SqlCommand cmdUD = new SqlCommand(updateDobanda, conn);

                    cmdUD.ExecuteNonQuery();

                }


                if (contPlatitor.Substring(11, 3) == "ECN")
                {

                    string selectSold = "SELECT Sold " +
                        "FROM Conturi " +
                        "WHERE IBAN_Cont = '" + contPlatitor + "'";

                    SqlCommand cmdSold = new SqlCommand(selectSold, conn);

                    SqlDataReader readerSold = cmdSold.ExecuteReader();

                    readerSold.Read();

                    decimal sold = readerSold.GetDecimal(0);

                    readerSold.Close();


                    string selectDobanda = "SELECT ID_Dobanda " +
                        "FROM Dobanda_Conturi " +
                        "WHERE " + sold + " >= Suma_Minima AND " + sold + " <= Suma_Maxima";

                    SqlCommand cmdDobanda = new SqlCommand(selectDobanda, conn);

                    SqlDataReader readerDobanda = cmdDobanda.ExecuteReader();

                    readerDobanda.Read();

                    int idDobanda = readerDobanda.GetInt32(0);

                    readerDobanda.Close();


                    string updateDobanda = "UPDATE Conturi " +
                        "SET ID_Dobanda = " + idDobanda + " " +
                        "WHERE IBAN_Cont = '" + contPlatitor + "'";

                    SqlCommand cmdUD = new SqlCommand(updateDobanda, conn);

                    cmdUD.ExecuteNonQuery();

                }


                string selectNrOperatiuneP = "SELECT MAX(Nr_Operatiune) " +
                    "FROM Operatiuni";

                SqlCommand cmdNrP = new SqlCommand(selectNrOperatiuneP, conn);

                SqlDataReader readerNrP = cmdNrP.ExecuteReader();

                readerNrP.Read();

                int nrP;

                if (readerNrP.IsDBNull(0))
                {

                    nrP = 1;

                }
                else
                {

                    nrP = Convert.ToInt32(readerNrP[0].ToString()) + 1;

                }

                readerNrP.Close();



                string insertOperatiunePrincipal = "INSERT INTO Operatiuni(Nr_Operatiune, Suma_Operatiune, Data_Operatiune, Detalii_Operatiune, Tip_Operatiune, IBAN_Cont) " +
                    "VALUES(@nr_operatiune, @suma_operatiune, @data_operatiune, @detalii_operatiune, @tip_operatiune, @iban_cont)";

                SqlCommand cmdIOP = new SqlCommand(insertOperatiunePrincipal, conn);

                cmdIOP.Parameters.AddWithValue("@nr_operatiune", nrP);

                cmdIOP.Parameters.AddWithValue("@suma_operatiune", "-" + txtSumaTransferIntreConturi.Text);

                cmdIOP.Parameters.AddWithValue("@data_operatiune", DateTime.Now.Date);

                cmdIOP.Parameters.AddWithValue("@detalii_operatiune", txtDetaliiTransferIntreConturi.Text);

                cmdIOP.Parameters.AddWithValue("@tip_operatiune", "TRANSFER");

                cmdIOP.Parameters.AddWithValue("@iban_cont", txtTransferDinContul.Text);

                cmdIOP.ExecuteNonQuery();




                string selectNrOperatiuneS = "SELECT MAX(Nr_Operatiune) " +
                    "FROM Operatiuni";

                SqlCommand cmdNrS = new SqlCommand(selectNrOperatiuneS, conn);

                SqlDataReader readerNrS = cmdNrS.ExecuteReader();

                readerNrS.Read();

                int nrS;

                if (readerNrS.IsDBNull(0))
                {

                    nrS = 1;

                }
                else
                {

                    nrS = Convert.ToInt32(readerNrS[0].ToString()) + 1;

                }

                readerNrS.Close();



                string insertOperatiuneSecundar = "INSERT INTO Operatiuni(Nr_Operatiune, Suma_Operatiune, Data_Operatiune, Detalii_Operatiune, Tip_Operatiune, IBAN_Cont) " +
                    "VALUES(@nr_operatiune, @suma_operatiune, @data_operatiune, @detalii_operatiune, @tip_operatiune, @iban_cont)";

                SqlCommand cmdIOS = new SqlCommand(insertOperatiuneSecundar, conn);

                cmdIOS.Parameters.AddWithValue("@nr_operatiune", nrS);

                cmdIOS.Parameters.AddWithValue("@suma_operatiune", txtSumaTransferIntreConturi.Text);

                cmdIOS.Parameters.AddWithValue("@data_operatiune", DateTime.Now.Date);

                cmdIOS.Parameters.AddWithValue("@detalii_operatiune", txtDetaliiTransferIntreConturi.Text);

                cmdIOS.Parameters.AddWithValue("@tip_operatiune", "TRANSFER");

                cmdIOS.Parameters.AddWithValue("@iban_cont", dlTransferCatreContul.Text);

                cmdIOS.ExecuteNonQuery();

                conn.Close();

            }

            DropDownList_Change();

        }

        protected void BtnAnulareTransferIntreConturi_Click(object sender, EventArgs e)
        {

            platiIntreConturi.Visible = false;

        }

        protected void ThirdBtn2_Click(object sender, EventArgs e)
        {

            platiCont.Visible = true;

            txtPlataDinContul.Text = accountDl.Text;

            txtNumeBeneficiar.Text = "";

            txtContBeneficiar.Text = "";

            txtSumaPlataDinCont.Text = "";

            txtDetaliiPlata.Text = "";

            lblEroarePlataCont.Visible = false;

            string valuta = txtPlataDinContul.Text.Substring(8, 3);

            string codClient = (string)Session["codClient"];

            conn.Open();

            string selectBeneficiariPredefiniti = "SELECT * " +
               "FROM Beneficiari_Predefiniti " +
               "WHERE Cod_Client = '" + codClient + "' AND SUBSTRING(IBAN_Beneficiar, 9, 3) = '" + valuta + "'";

            SqlCommand cmdBeneficiariPredefiniti = new SqlCommand(selectBeneficiariPredefiniti, conn);

            SqlDataAdapter adapterBeneficiariPredefiniti = new SqlDataAdapter(cmdBeneficiariPredefiniti);

            DataTable dtBeneficiariPredefiniti = new DataTable();

            adapterBeneficiariPredefiniti.Fill(dtBeneficiariPredefiniti);

            dlBeneficiarPredefinit.DataSource = dtBeneficiariPredefiniti;

            dlBeneficiarPredefinit.DataTextField = "Denumire_Beneficiar";

            dlBeneficiarPredefinit.DataBind();

            dlBeneficiarPredefinit.Items.Insert(0, "");

            conn.Close();

            dlBeneficiarPredefinit.SelectedIndex = 0;

        }

        protected void DlBeneficiarPredefinit_SelectedIndexChanged(object sender, EventArgs e)
        {

            platiCont.Visible = true;

            if (dlBeneficiarPredefinit.SelectedIndex == 0)
            {

                txtNumeBeneficiar.Text = "";

                txtContBeneficiar.Text = "";

                txtNumeBeneficiar.Style.Add("pointer-events", "all");

                txtContBeneficiar.Style.Add("pointer-events", "all");

            }
            else
            {

                txtNumeBeneficiar.Style.Add("pointer-events", "none");

                txtContBeneficiar.Style.Add("pointer-events", "none");

                conn.Open();

                string selectDateBeneficiar = "SELECT Nume_Beneficiar, IBAN_Beneficiar " +
                    "FROM Beneficiari_Predefiniti " +
                    "WHERE Denumire_Beneficiar = '" + dlBeneficiarPredefinit.Text + "'";

                SqlCommand cmdDateBeneficiar = new SqlCommand(selectDateBeneficiar, conn);

                SqlDataReader readerDateBeneficiar = cmdDateBeneficiar.ExecuteReader();

                readerDateBeneficiar.Read();

                txtNumeBeneficiar.Text = readerDateBeneficiar[0].ToString();

                txtContBeneficiar.Text = readerDateBeneficiar[1].ToString();

                readerDateBeneficiar.Close();

                conn.Close();

            }

        }

        protected void BtnFinalizarePlata_Click(object sender, EventArgs e)
        {

            if (txtNumeBeneficiar.Text == "" || txtContBeneficiar.Text == "" || txtSumaPlataDinCont.Text == "" || txtDetaliiPlata.Text == "")
            {

                lblEroarePlataCont.Visible = true;

                platiCont.Visible = true;

            }
            else
            {

                conn.Open();

                string updateSoldBeneficiar = "UPDATE Conturi " +
                    "SET Sold = Sold + " + txtSumaPlataDinCont.Text + " " +
                    "WHERE IBAN_Cont = '" + txtContBeneficiar.Text + "'";

                SqlCommand cmdUpdateSoldBeneficiar = new SqlCommand(updateSoldBeneficiar, conn);

                cmdUpdateSoldBeneficiar.ExecuteNonQuery();

                string updateSoldPlatitor = "UPDATE Conturi " +
                    "SET Sold = Sold - " + txtSumaPlataDinCont.Text + " " +
                    "WHERE IBAN_Cont = '" + txtPlataDinContul.Text + "'";

                SqlCommand cmdUpdateSoldPlatitor = new SqlCommand(updateSoldPlatitor, conn);

                cmdUpdateSoldPlatitor.ExecuteNonQuery();





                string selectNrOperatiuneP = "SELECT MAX(Nr_Operatiune) " +
                    "FROM Operatiuni";

                SqlCommand cmdNrP = new SqlCommand(selectNrOperatiuneP, conn);

                SqlDataReader readerNrP = cmdNrP.ExecuteReader();

                readerNrP.Read();

                int nrP;

                if (readerNrP.IsDBNull(0))
                {

                    nrP = 1;

                }
                else
                {

                    nrP = Convert.ToInt32(readerNrP[0].ToString()) + 1;

                }

                readerNrP.Close();



                string insertOperatiunePrincipal = "INSERT INTO Operatiuni(Nr_Operatiune, Suma_Operatiune, Data_Operatiune, Detalii_Operatiune, Tip_Operatiune, IBAN_Cont) " +
                    "VALUES(@nr_operatiune, @suma_operatiune, @data_operatiune, @detalii_operatiune, @tip_operatiune, @iban_cont)";

                SqlCommand cmdIOP = new SqlCommand(insertOperatiunePrincipal, conn);

                cmdIOP.Parameters.AddWithValue("@nr_operatiune", nrP);

                cmdIOP.Parameters.AddWithValue("@suma_operatiune", "-" + txtSumaPlataDinCont.Text);

                cmdIOP.Parameters.AddWithValue("@data_operatiune", DateTime.Now.Date);

                cmdIOP.Parameters.AddWithValue("@detalii_operatiune", txtDetaliiPlata.Text);

                cmdIOP.Parameters.AddWithValue("@tip_operatiune", "PLATA");

                cmdIOP.Parameters.AddWithValue("@iban_cont", txtPlataDinContul.Text);

                cmdIOP.ExecuteNonQuery();




                string selectNrOperatiuneS = "SELECT MAX(Nr_Operatiune) " +
                     "FROM Operatiuni";

                SqlCommand cmdNrS = new SqlCommand(selectNrOperatiuneS, conn);

                SqlDataReader readerNrS = cmdNrS.ExecuteReader();

                readerNrS.Read();

                int nrS;

                if (readerNrS.IsDBNull(0))
                {

                    nrS = 1;

                }
                else
                {

                    nrS = Convert.ToInt32(readerNrS[0].ToString()) + 1;

                }

                readerNrS.Close();


                string insertOperatiuneSecundar = "INSERT INTO Operatiuni(Nr_Operatiune, Suma_Operatiune, Data_Operatiune, Detalii_Operatiune, Tip_Operatiune, IBAN_Cont) " +
                    "VALUES(@nr_operatiune, @suma_operatiune, @data_operatiune, @detalii_operatiune, @tip_operatiune, @iban_cont)";

                SqlCommand cmdIOS = new SqlCommand(insertOperatiuneSecundar, conn);

                cmdIOS.Parameters.AddWithValue("@nr_operatiune", nrS);

                cmdIOS.Parameters.AddWithValue("@suma_operatiune", txtSumaPlataDinCont.Text);

                cmdIOS.Parameters.AddWithValue("@data_operatiune", DateTime.Now.Date);

                cmdIOS.Parameters.AddWithValue("@detalii_operatiune", txtDetaliiPlata.Text);

                cmdIOS.Parameters.AddWithValue("@tip_operatiune", "INCASARE PLATA");

                cmdIOS.Parameters.AddWithValue("@iban_cont", txtContBeneficiar.Text);

                cmdIOS.ExecuteNonQuery();

                conn.Close();

            }

            DropDownList_Change();

        }

        protected void ThirdBtn3_Click(object sender, EventArgs e)
        {

            conversieBani.Visible = true;

            dlContPlataConversie.Items.Clear();

            dlContDestinatarConversie.Items.Clear();

            dlContDestinatarConversie.Style.Clear();

            dlContPlataConversie.Style.Clear();

            txtSumaConversie.Text = "";

            txtSumaDupaConversie.Text = "";

            lblEroareConversie.Visible = false;

            if (rblListaTipConversie.Items.Count == 0)
            {

                rblListaTipConversie.Items.Add("Cumpărare (Valută vs RON)");

                rblListaTipConversie.Items.Add("Vânzare (Valută vs RON)");

                rblListaTipConversie.Items.Add("Cumpărare (Valută vs Valută)");

                rblListaTipConversie.Items.Add("Vânzare (Valută vs Valută)");

            }

            rblListaTipConversie.SelectedIndex = 0;


            conn.Open();

            string valuta = accountDl.Text.Substring(8, 3);

            string codClient = (string)Session["codClient"];

            dlContPlataConversie.Style.Add("pointer-events", "none");

            if (valuta != "RON")
            {

                dlContPlataConversie.Items.Add("SELECTAȚI CONT RON");

                dlContDestinatarConversie.Style.Add("pointer-events", "none");

                txtSumaConversie.Style.Add("pointer-events", "none");

                txtSumaConversie.BackColor = System.Drawing.Color.WhiteSmoke;

                dlContDestinatarConversie.BackColor = System.Drawing.Color.WhiteSmoke;

            }
            else
            {

                dlContPlataConversie.Items.Add(accountDl.Text);

                dlContDestinatarConversie.BackColor = System.Drawing.Color.White;

                dlContPlataConversie.BackColor = System.Drawing.Color.WhiteSmoke;

                txtSumaConversie.Style.Add("pointer-events", "all");

                txtSumaConversie.BackColor = System.Drawing.Color.White;


                string selectContDestinatar = "SELECT * " +
                "FROM Conturi " +
                "WHERE Cod_Client = '" + codClient + "' AND SUBSTRING(IBAN_Cont, 9, 3) <> '" + valuta + "' AND IBAN_Cont <> '" + txtTransferDinContul.Text + "'";

                SqlCommand cmdContDestinatar = new SqlCommand(selectContDestinatar, conn);

                SqlDataAdapter adapterContDestinatar = new SqlDataAdapter(cmdContDestinatar);

                DataTable dtContDestinatar = new DataTable();

                adapterContDestinatar.Fill(dtContDestinatar);

                dlContDestinatarConversie.DataSource = dtContDestinatar;

                dlContDestinatarConversie.DataTextField = "IBAN_Cont";

                dlContDestinatarConversie.DataBind();

                dlContDestinatarConversie.Style.Add("pointer-events", "all");

            }

            conn.Close();

            Citire_Xml();

        }

        protected void TxtSumaConversie_TextChanged(object sender, EventArgs e)
        {

            conversieBani.Visible = true;

            if (rblListaTipConversie.SelectedIndex == 0)
            {

                decimal sumaDupaConversie = Math.Round(Convert.ToDecimal(txtSumaConversie.Text) / Convert.ToDecimal(lblValoareCursBanca.Text.Substring(0, 6)), 2);

                txtSumaDupaConversie.Text = Convert.ToString(sumaDupaConversie);

            }
            else if (rblListaTipConversie.SelectedIndex == 1)
            {

                decimal sumaDupaConversie = Math.Round(Convert.ToDecimal(txtSumaConversie.Text) * Convert.ToDecimal(lblValoareCursBanca.Text.Substring(0, 6)), 2);

                txtSumaDupaConversie.Text = Convert.ToString(sumaDupaConversie);

            }
            else if (rblListaTipConversie.SelectedIndex == 2)
            {

                decimal sumaDupaConversie = Math.Round(Convert.ToDecimal(txtSumaConversie.Text) / Convert.ToDecimal(lblValoareCursBanca.Text.Substring(0, 6)), 2);

                txtSumaDupaConversie.Text = Convert.ToString(sumaDupaConversie);

            }
            else if (rblListaTipConversie.SelectedIndex == 3)
            {

                decimal sumaDupaConversie = Math.Round(Convert.ToDecimal(txtSumaConversie.Text) * Convert.ToDecimal(lblValoareCursBanca.Text.Substring(0, 6)), 2);

                txtSumaDupaConversie.Text = Convert.ToString(sumaDupaConversie);

            }

        }

        protected void RblListaTipConversie_SelectedIndexChanged(object sender, EventArgs e)
        {

            conversieBani.Visible = true;

            dlContPlataConversie.Items.Clear();

            dlContDestinatarConversie.Items.Clear();

            dlContDestinatarConversie.Style.Clear();

            dlContPlataConversie.Style.Clear();

            txtSumaConversie.Text = "";

            txtSumaDupaConversie.Text = "";

            string valuta = accountDl.Text.Substring(8, 3);

            string codClient = (string)Session["codClient"];



            if (rblListaTipConversie.SelectedIndex == 0)
            {

                dlContPlataConversie.Style.Add("pointer-events", "none");

                dlContPlataConversie.Style.Add("background-color", "rgb(255, 255, 255, 0.7)");

                if (valuta != "RON")
                {

                    dlContPlataConversie.Items.Add("SELECTAȚI CONT RON");

                    dlContDestinatarConversie.Style.Add("pointer-events", "none");

                    txtSumaConversie.Style.Add("pointer-events", "none");

                    txtSumaConversie.BackColor = System.Drawing.Color.WhiteSmoke;

                    dlContDestinatarConversie.BackColor = System.Drawing.Color.WhiteSmoke;

                    dlContPlataConversie.BackColor = System.Drawing.Color.WhiteSmoke;


                }
                else
                {

                    dlContPlataConversie.Items.Add(accountDl.Text);

                    dlContDestinatarConversie.Style.Add("pointer-events", "all");

                    dlContDestinatarConversie.BackColor = System.Drawing.Color.White;

                    txtSumaConversie.Style.Add("pointer-events", "all");

                    txtSumaConversie.BackColor = System.Drawing.Color.White;


                    conn.Open();

                    string selectContDestinatar = "SELECT * " +
                    "FROM Conturi " +
                    "WHERE Cod_Client = '" + codClient + "' AND SUBSTRING(IBAN_Cont, 9, 3) <> '" + valuta + "' AND IBAN_Cont <> '" + txtTransferDinContul.Text + "'";

                    SqlCommand cmdContDestinatar = new SqlCommand(selectContDestinatar, conn);

                    SqlDataAdapter adapterContDestinatar = new SqlDataAdapter(cmdContDestinatar);

                    DataTable dtContDestinatar = new DataTable();

                    adapterContDestinatar.Fill(dtContDestinatar);

                    dlContDestinatarConversie.DataSource = dtContDestinatar;

                    dlContDestinatarConversie.DataTextField = "IBAN_Cont";

                    dlContDestinatarConversie.DataBind();

                    conn.Close();

                }

            }
            else if (rblListaTipConversie.SelectedIndex == 1)
            {

                dlContDestinatarConversie.Style.Add("pointer-events", "none");

                dlContDestinatarConversie.Style.Add("background-color", "rgb(255, 255, 255, 0.7)");

                if (valuta != "RON")
                {

                    dlContDestinatarConversie.Items.Add("SELECTAȚI CONT RON");

                    dlContPlataConversie.Style.Add("pointer-events", "none");

                    txtSumaConversie.Style.Add("pointer-events", "none");

                    txtSumaConversie.BackColor = System.Drawing.Color.WhiteSmoke;

                    dlContPlataConversie.BackColor = System.Drawing.Color.WhiteSmoke;

                    dlContDestinatarConversie.BackColor = System.Drawing.Color.WhiteSmoke;

                }
                else
                {

                    dlContDestinatarConversie.Items.Add(accountDl.Text);

                    dlContPlataConversie.Style.Add("pointer-events", "all");

                    dlContPlataConversie.BackColor = System.Drawing.Color.White;

                    txtSumaConversie.Style.Add("pointer-events", "all");

                    txtSumaConversie.BackColor = System.Drawing.Color.White;


                    conn.Open();

                    string selectContPlatitor = "SELECT * " +
                    "FROM Conturi " +
                    "WHERE Cod_Client = '" + codClient + "' AND SUBSTRING(IBAN_Cont, 9, 3) <> 'RON' AND IBAN_Cont <> '" + txtTransferDinContul.Text + "'";

                    SqlCommand cmdContPlatitor = new SqlCommand(selectContPlatitor, conn);

                    SqlDataAdapter adapterContPlatitor = new SqlDataAdapter(cmdContPlatitor);

                    DataTable dtContPlatitor = new DataTable();

                    adapterContPlatitor.Fill(dtContPlatitor);

                    dlContPlataConversie.DataSource = dtContPlatitor;

                    dlContPlataConversie.DataTextField = "IBAN_Cont";

                    dlContPlataConversie.DataBind();

                    conn.Close();

                }

            }
            else if (rblListaTipConversie.SelectedIndex == 2)
            {

                dlContPlataConversie.Style.Add("pointer-events", "none");

                dlContPlataConversie.Style.Add("background-color", "rgb(255, 255, 255, 0.7)");

                if (valuta == "RON")
                {

                    dlContPlataConversie.Items.Add("SELECTAȚI CONT VALUTA");

                    dlContDestinatarConversie.Style.Add("pointer-events", "none");

                    txtSumaConversie.Style.Add("pointer-events", "none");

                    txtSumaConversie.BackColor = System.Drawing.Color.WhiteSmoke;

                    dlContDestinatarConversie.BackColor = System.Drawing.Color.WhiteSmoke;

                    dlContPlataConversie.BackColor = System.Drawing.Color.WhiteSmoke;


                }
                else
                {

                    dlContPlataConversie.Items.Add(accountDl.Text);

                    dlContDestinatarConversie.Style.Add("pointer-events", "all");

                    dlContDestinatarConversie.BackColor = System.Drawing.Color.White;

                    txtSumaConversie.Style.Add("pointer-events", "all");

                    txtSumaConversie.BackColor = System.Drawing.Color.White;


                    conn.Open();

                    string selectContDestinatar = "SELECT * " +
                    "FROM Conturi " +
                    "WHERE Cod_Client = '" + codClient + "' AND SUBSTRING(IBAN_Cont, 9, 3) <> '" + valuta + "' AND SUBSTRING(IBAN_Cont, 9, 3) <> 'RON' AND IBAN_Cont <> '" + txtTransferDinContul.Text + "'";

                    SqlCommand cmdContDestinatar = new SqlCommand(selectContDestinatar, conn);

                    SqlDataAdapter adapterContDestinatar = new SqlDataAdapter(cmdContDestinatar);

                    DataTable dtContDestinatar = new DataTable();

                    adapterContDestinatar.Fill(dtContDestinatar);

                    dlContDestinatarConversie.DataSource = dtContDestinatar;

                    dlContDestinatarConversie.DataTextField = "IBAN_Cont";

                    dlContDestinatarConversie.DataBind();

                    conn.Close();

                }

            }
            else if (rblListaTipConversie.SelectedIndex == 3)
            {

                dlContDestinatarConversie.Style.Add("pointer-events", "none");

                dlContDestinatarConversie.Style.Add("background-color", "rgb(255, 255, 255, 0.7)");

                if (valuta == "RON")
                {

                    dlContDestinatarConversie.Items.Add("SELECTAȚI CONT VALUTA");

                    dlContPlataConversie.Style.Add("pointer-events", "none");

                    txtSumaConversie.Style.Add("pointer-events", "none");

                    txtSumaConversie.BackColor = System.Drawing.Color.WhiteSmoke;

                    dlContPlataConversie.BackColor = System.Drawing.Color.WhiteSmoke;

                    dlContDestinatarConversie.BackColor = System.Drawing.Color.WhiteSmoke;

                }
                else
                {

                    dlContDestinatarConversie.Items.Add(accountDl.Text);

                    dlContPlataConversie.Style.Add("pointer-events", "all");

                    dlContPlataConversie.BackColor = System.Drawing.Color.White;

                    txtSumaConversie.Style.Add("pointer-events", "all");

                    txtSumaConversie.BackColor = System.Drawing.Color.White;


                    conn.Open();

                    string selectContPlatitor = "SELECT * " +
                    "FROM Conturi " +
                    "WHERE Cod_Client = '" + codClient + "' AND SUBSTRING(IBAN_Cont, 9, 3) <> 'RON' AND SUBSTRING(IBAN_Cont, 9, 3) <> '" + valuta + "' AND IBAN_Cont <> '" + txtTransferDinContul.Text + "'";

                    SqlCommand cmdContPlatitor = new SqlCommand(selectContPlatitor, conn);

                    SqlDataAdapter adapterContPlatitor = new SqlDataAdapter(cmdContPlatitor);

                    DataTable dtContPlatitor = new DataTable();

                    adapterContPlatitor.Fill(dtContPlatitor);

                    dlContPlataConversie.DataSource = dtContPlatitor;

                    dlContPlataConversie.DataTextField = "IBAN_Cont";

                    dlContPlataConversie.DataBind();

                    conn.Close();

                }

            }

            Citire_Xml();

        }


        protected void DlContDestinatarConversie_SelectedIndexChanged(object sender, EventArgs e)
        {

            conversieBani.Visible = true;

            Citire_Xml();

        }

        protected void DlContPlataConversie_SelectedIndexChanged(object sender, EventArgs e)
        {

            conversieBani.Visible = true;

            Citire_Xml();

        }

        protected void Citire_Xml()
        {

            if (dlContPlataConversie.Text.Length != 24 || dlContDestinatarConversie.Text.Length != 24)
            {

                lblValoareCursBanca.Text = "-";

                lblValoareCursConversie.Text = "-";

                lblValoareDataConversie.Text = "-";

                return;

            }

            string valutaDestinatar = dlContDestinatarConversie.Text.Substring(8, 3);

            string valutaPlatitor = dlContPlataConversie.Text.Substring(8, 3);

            XmlDocument xmlDoc = new XmlDocument();

            string xmlURL = "https://www.bnr.ro/nbrfxrates.xml";

            xmlDoc.Load(xmlURL);

            XmlNodeList listRate = xmlDoc.GetElementsByTagName("Rate");

            XmlNodeList listCube = xmlDoc.GetElementsByTagName("Cube");

            DateTime dataValuta = Convert.ToDateTime(listCube[0].Attributes[0].InnerText.ToString());

            lblValoareDataConversie.Text = dataValuta.ToString("dd'/'MM'/'yyyy");

            for (int i = 0; i <= listRate.Count; i++)
            {

                string valutaDestinatarXml = listRate[i].Attributes[0].InnerText.ToString();


                if (rblListaTipConversie.SelectedIndex == 0)
                {

                    if (valutaDestinatarXml == valutaDestinatar)
                    {

                        double valoareBancaXml = Convert.ToDouble(listRate[i].InnerText.ToString()) + 0.06;

                        lblCursSchimbBanca.Text = "Curs vânzare:";

                        lblValoareCursConversie.Text = listRate[i].InnerText.ToString() + " " + valutaDestinatarXml;

                        lblValoareCursBanca.Text = valoareBancaXml + " " + valutaDestinatarXml;

                        return;

                    }

                }
                else if (rblListaTipConversie.SelectedIndex == 1)
                {

                    if (valutaDestinatarXml == valutaPlatitor)
                    {

                        double valoareBancaXml = Convert.ToDouble(listRate[i].InnerText.ToString()) - 0.06;

                        lblCursSchimbBanca.Text = "Curs cumpărare:";

                        lblValoareCursConversie.Text = listRate[i].InnerText.ToString() + " " + valutaDestinatarXml;

                        lblValoareCursBanca.Text = valoareBancaXml + " " + valutaDestinatarXml;

                        return;

                    }

                }
                else if (rblListaTipConversie.SelectedIndex == 2)
                {


                    if (valutaDestinatarXml == valutaDestinatar)
                    {

                        for (int j = 0; j <= listRate.Count; j++)
                        {

                            string valutaPlatitorXml = listRate[j].Attributes[0].InnerText.ToString();

                            if (valutaPlatitorXml == valutaPlatitor)
                            {

                                double valoareBancaXml = Math.Round(Convert.ToDouble(listRate[i].InnerText.ToString()) / Convert.ToDouble(listRate[j].InnerText.ToString()) + 0.02, 4);

                                double valoareCursXml = Math.Round(Convert.ToDouble(listRate[i].InnerText.ToString()) / Convert.ToDouble(listRate[j].InnerText.ToString()), 4);

                                lblCursSchimbBanca.Text = "Curs vânzare:";

                                lblValoareCursConversie.Text = valoareCursXml + " " + valutaDestinatarXml;

                                lblValoareCursBanca.Text = valoareBancaXml + " " + valutaDestinatarXml;

                                return;

                            }

                        }

                    }


                }
                else if (rblListaTipConversie.SelectedIndex == 3)
                {


                    if (valutaDestinatarXml == valutaDestinatar)
                    {

                        for (int j = 0; j <= listRate.Count; j++)
                        {

                            string valutaPlatitorXml = listRate[j].Attributes[0].InnerText.ToString();

                            if (valutaPlatitorXml == valutaPlatitor)
                            {

                                double valoareBancaXml = Math.Round(Convert.ToDouble(listRate[j].InnerText.ToString()) / Convert.ToDouble(listRate[i].InnerText.ToString()) - 0.02, 4);

                                double valoareCursXml = Math.Round(Convert.ToDouble(listRate[j].InnerText.ToString()) / Convert.ToDouble(listRate[i].InnerText.ToString()), 4);

                                lblCursSchimbBanca.Text = "Curs cumpărare:";

                                lblValoareCursConversie.Text = valoareCursXml + " " + valutaDestinatarXml;

                                lblValoareCursBanca.Text = valoareBancaXml + " " + valutaDestinatarXml;

                                return;

                            }

                        }

                    }

                }

            }

        }

        protected void BtnFinalizareConversie_Click(object sender, EventArgs e)
        {

            if (txtSumaConversie.Text == "" || txtSumaDupaConversie.Text == "")
            {

                lblEroareConversie.Visible = true;

                conversieBani.Visible = true;

            }
            else
            {

                conn.Open();

                string updateContPlatitor = "UPDATE Conturi " +
                    "SET Sold = Sold - " + txtSumaConversie.Text + " " +
                    "WHERE IBAN_Cont = '" + dlContPlataConversie.Text + "'";

                SqlCommand cmdContPlatitor = new SqlCommand(updateContPlatitor, conn);

                cmdContPlatitor.ExecuteNonQuery();


                string updateContBeneficiar = "UPDATE Conturi " +
                    "SET Sold = Sold + " + txtSumaDupaConversie.Text + " " +
                    "WHERE IBAN_Cont = '" + dlContDestinatarConversie.Text + "'";

                SqlCommand cmdContBeneficiar = new SqlCommand(updateContBeneficiar, conn);

                cmdContBeneficiar.ExecuteNonQuery();




                string selectNrOperatiuneP = "SELECT MAX(Nr_Operatiune) " +
                    "FROM Operatiuni";

                SqlCommand cmdNrP = new SqlCommand(selectNrOperatiuneP, conn);

                SqlDataReader readerNrP = cmdNrP.ExecuteReader();

                readerNrP.Read();

                int nrP;

                if (readerNrP.IsDBNull(0))
                {

                    nrP = 1;

                }
                else
                {

                    nrP = Convert.ToInt32(readerNrP[0].ToString()) + 1;

                }

                readerNrP.Close();



                string insertOperatiunePrincipal = "INSERT INTO Operatiuni(Nr_Operatiune, Suma_Operatiune, Data_Operatiune, Detalii_Operatiune, Tip_Operatiune, IBAN_Cont) " +
                    "VALUES(@nr_operatiune, @suma_operatiune, @data_operatiune, @detalii_operatiune, @tip_operatiune, @iban_cont)";

                SqlCommand cmdIOP = new SqlCommand(insertOperatiunePrincipal, conn);

                cmdIOP.Parameters.AddWithValue("@nr_operatiune", nrP);

                cmdIOP.Parameters.AddWithValue("@suma_operatiune", "-" + txtSumaConversie.Text);

                cmdIOP.Parameters.AddWithValue("@data_operatiune", DateTime.Now.Date);

                cmdIOP.Parameters.AddWithValue("@detalii_operatiune", lblValoareCursBanca.Text);

                cmdIOP.Parameters.AddWithValue("@tip_operatiune", "CONVERSIE");

                cmdIOP.Parameters.AddWithValue("@iban_cont", dlContPlataConversie.Text);

                cmdIOP.ExecuteNonQuery();




                string selectNrOperatiuneS = "SELECT MAX(Nr_Operatiune) " +
                    "FROM Operatiuni";

                SqlCommand cmdNrS = new SqlCommand(selectNrOperatiuneS, conn);

                SqlDataReader readerNrS = cmdNrS.ExecuteReader();

                readerNrS.Read();

                int nrS;

                if (readerNrS.IsDBNull(0))
                {

                    nrS = 1;

                }
                else
                {

                    nrS = Convert.ToInt32(readerNrS[0].ToString()) + 1;

                }

                readerNrS.Close();



                string insertOperatiuneSecundar = "INSERT INTO Operatiuni(Nr_Operatiune, Suma_Operatiune, Data_Operatiune, Detalii_Operatiune, Tip_Operatiune, IBAN_Cont) " +
                    "VALUES(@nr_operatiune, @suma_operatiune, @data_operatiune, @detalii_operatiune, @tip_operatiune, @iban_cont)";

                SqlCommand cmdIOS = new SqlCommand(insertOperatiuneSecundar, conn);

                cmdIOS.Parameters.AddWithValue("@nr_operatiune", nrS);

                cmdIOS.Parameters.AddWithValue("@suma_operatiune", txtSumaDupaConversie.Text);

                cmdIOS.Parameters.AddWithValue("@data_operatiune", DateTime.Now.Date);

                cmdIOS.Parameters.AddWithValue("@detalii_operatiune", lblValoareCursBanca.Text);

                cmdIOS.Parameters.AddWithValue("@tip_operatiune", "CONVERSIE");

                cmdIOS.Parameters.AddWithValue("@iban_cont", dlContDestinatarConversie.Text);

                cmdIOS.ExecuteNonQuery();


                conn.Close();

            }

            DropDownList_Change();

        }


        protected void ThirdBtn4_Click(object sender, EventArgs e)
        {

            platiProgramate.Visible = true;

            Plati_Programate();

            Populate_DgvPlatiProgramate();

            if (gvPlatiProgramate.Rows.Count < 1)
            {

                btnPlataNouaProgramata.Style.Add("top", "130px");

                btnAnularePlataProgramata.Style.Add("top", "130px");

            }
            else
            {

                btnPlataNouaProgramata.Style.Add("top", "100px");

                btnAnularePlataProgramata.Style.Add("top", "100px");

            }

        }

        protected void BtnPlataNouaProgramata_Click(object sender, EventArgs e)
        {

            lblEroarePlatiProgramate.Visible = false;

            platiProgramate.Visible = false;

            adaugarePlatiProgramate.Visible = true;

            cldDataInceputPlataProgramata.Visible = false;

            txtDenumirePlataProgramata.Text = "";

            txtDataInceputPlataProgramata.Text = "";

            txtContPlataProgramata.Text = "";

            txtNumeBeneficiarPlataProgramata.Text = "";

            txtContBeneficiarPlataProgramata.Text = "";

            txtDetaliiPlataProgramata.Text = "";

            txtSumaPlataProgramata.Text = "";



            cldDataInceputPlataProgramata.SelectedDate = DateTime.MinValue.Date;



            txtContPlataProgramata.Text = accountDl.Text;



            string valuta = txtContPlataProgramata.Text.Substring(8, 3);

            string codClient = (string)Session["codClient"];

            conn.Open();

            string selectBeneficiariPredefiniti = "SELECT * " +
               "FROM Beneficiari_Predefiniti " +
               "WHERE Cod_Client = '" + codClient + "' AND SUBSTRING(IBAN_Beneficiar, 9, 3) = '" + valuta + "'";

            SqlCommand cmdBeneficiariPredefiniti = new SqlCommand(selectBeneficiariPredefiniti, conn);

            SqlDataAdapter adapterBeneficiariPredefiniti = new SqlDataAdapter(cmdBeneficiariPredefiniti);

            DataTable dtBeneficiariPredefiniti = new DataTable();

            adapterBeneficiariPredefiniti.Fill(dtBeneficiariPredefiniti);

            dlBeneficiarPlataProgramata.DataSource = dtBeneficiariPredefiniti;

            dlBeneficiarPlataProgramata.DataTextField = "Denumire_Beneficiar";

            dlBeneficiarPlataProgramata.DataBind();

            dlBeneficiarPlataProgramata.Items.Insert(0, "");

            conn.Close();

            dlBeneficiarPlataProgramata.SelectedIndex = 0;

        }


        protected void IbDataInceputPlataProgramata_Click(object sender, ImageClickEventArgs e)
        {

            adaugarePlatiProgramate.Visible = true;

            cldDataInceputPlataProgramata.Visible = true;

        }

        protected void CldDataInceputPlataProgramata_SelectionChanged(object sender, EventArgs e)
        {

            if (cldDataInceputPlataProgramata.SelectedDate < DateTime.Now)
            {

                txtDataInceputPlataProgramata.Text = "DATĂ INVALIDĂ";

            }
            else
            {

                txtDataInceputPlataProgramata.Text = cldDataInceputPlataProgramata.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

            adaugarePlatiProgramate.Visible = true;

            cldDataInceputPlataProgramata.Visible = false;


        }

        protected void CldDataInceputPlataProgramata_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            adaugarePlatiProgramate.Visible = true;

            cldDataInceputPlataProgramata.Visible = true;

        }


        protected void DlBeneficiarPlataProgramata_SelectedIndexChanged(object sender, EventArgs e)
        {

            adaugarePlatiProgramate.Visible = true;


            if (dlBeneficiarPlataProgramata.SelectedIndex == 0)
            {

                txtNumeBeneficiarPlataProgramata.Text = "";

                txtContBeneficiarPlataProgramata.Text = "";

                txtNumeBeneficiarPlataProgramata.Style.Add("pointer-events", "all");

                txtContBeneficiarPlataProgramata.Style.Add("pointer-events", "all");

            }
            else
            {

                txtNumeBeneficiarPlataProgramata.Style.Add("pointer-events", "none");

                txtContBeneficiarPlataProgramata.Style.Add("pointer-events", "none");

                conn.Open();

                string selectDateBeneficiar = "SELECT Nume_Beneficiar, IBAN_Beneficiar " +
                    "FROM Beneficiari_Predefiniti " +
                    "WHERE Denumire_Beneficiar = '" + dlBeneficiarPlataProgramata.Text + "'";

                SqlCommand cmdDateBeneficiar = new SqlCommand(selectDateBeneficiar, conn);

                SqlDataReader readerDateBeneficiar = cmdDateBeneficiar.ExecuteReader();

                readerDateBeneficiar.Read();

                txtNumeBeneficiarPlataProgramata.Text = readerDateBeneficiar[0].ToString();

                txtContBeneficiarPlataProgramata.Text = readerDateBeneficiar[1].ToString();

                readerDateBeneficiar.Close();

                conn.Close();

            }

        }

        protected void BtnAdaugareProgramare_Click(object sender, EventArgs e)
        {

            if (txtDenumirePlataProgramata.Text == "" || txtDataInceputPlataProgramata.Text == "" || txtSumaPlataProgramata.Text == "" || txtContBeneficiarPlataProgramata.Text == "" || txtNumeBeneficiarPlataProgramata.Text == "" || txtDetaliiPlataProgramata.Text == "")
            {

                adaugarePlatiProgramate.Visible = true;

                lblEroarePlatiProgramate.Visible = true;

                return;

            }
            else
            {

                platiProgramate.Visible = true;

                conn.Open();

                string selectNrProgramare = "SELECT MAX(Nr_Programare) " +
                "FROM Programari_Plati";

                SqlCommand cmdNrProgramare = new SqlCommand(selectNrProgramare, conn);

                SqlDataReader readerNrProgramare = cmdNrProgramare.ExecuteReader();

                readerNrProgramare.Read();

                int nrProgramare;

                if (readerNrProgramare.IsDBNull(0))
                {

                    nrProgramare = 1;

                }
                else
                {

                    nrProgramare = readerNrProgramare.GetInt32(0) + 1;

                }

                readerNrProgramare.Close();




                string insertProgramare = "INSERT INTO Programari_Plati (Nr_Programare, Denumire_Programare, Data_Programare, Suma_Programare, Programare_Activa, Detalii_Programare, IBAN_Incasare, IBAN_Cont) " +
                    "VALUES (@nr_programare, @denumire_programare, @data_programare, @suma_programare, @programare_activa, @detalii_programare,@iban_incasare, @iban_cont)";

                SqlCommand cmdInsertProgramare = new SqlCommand(insertProgramare, conn);

                cmdInsertProgramare.Parameters.AddWithValue("@nr_programare", nrProgramare);

                cmdInsertProgramare.Parameters.AddWithValue("@denumire_programare", txtDenumirePlataProgramata.Text);

                cmdInsertProgramare.Parameters.AddWithValue("@data_programare", DateTime.Parse(txtDataInceputPlataProgramata.Text));

                cmdInsertProgramare.Parameters.AddWithValue("@suma_programare", txtSumaPlataProgramata.Text);

                cmdInsertProgramare.Parameters.AddWithValue("@programare_activa", "True");

                cmdInsertProgramare.Parameters.AddWithValue("@detalii_programare", txtDetaliiPlataProgramata.Text);

                cmdInsertProgramare.Parameters.AddWithValue("@iban_incasare", txtContBeneficiarPlataProgramata.Text);

                cmdInsertProgramare.Parameters.AddWithValue("@iban_cont", txtContPlataProgramata.Text);

                cmdInsertProgramare.ExecuteNonQuery();

                conn.Close();

            }

            Populate_DgvPlatiProgramate();

            if (gvPlatiProgramate.Rows.Count < 1)
            {

                btnPlataNouaProgramata.Style.Add("top", "130px");

                btnAnularePlataProgramata.Style.Add("top", "130px");

            }
            else
            {

                btnPlataNouaProgramata.Style.Add("top", "100px");

                btnAnularePlataProgramata.Style.Add("top", "100px");

            }

        }

        protected void BtnAnulareProgramare_Click(object sender, EventArgs e)
        {

            adaugarePlatiProgramate.Visible = false;

            platiProgramate.Visible = true;

        }


        protected void GvPlatiProgramate_SelectedIndexChanged(object sender, EventArgs e)
        {

            platiProgramate.Visible = true;

            int index = gvPlatiProgramate.SelectedRow.RowIndex;

            Session["index"] = index;

        }

        [Obsolete]
        protected void GvPlatiProgramate_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.DataItemIndex == -1)
                return;
            e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';");

            e.Row.Attributes.Add("onclick", this.GetPostBackClientEvent(gvPlatiProgramate, "Select$" + e.Row.RowIndex.ToString()));

        }

        protected void BtnAnularePlataProgramata_Click(object sender, EventArgs e)
        {

            platiProgramate.Visible = true;

            int index = gvPlatiProgramate.SelectedRow.RowIndex;

            conn.Open();

            string deleteProgramare = "DELETE FROM Programari_Plati " +
                "WHERE Denumire_Programare = '" + gvPlatiProgramate.Rows[index].Cells[0].Text + "'";

            SqlCommand cmdDeleteProgramare = new SqlCommand(deleteProgramare, conn);

            cmdDeleteProgramare.ExecuteNonQuery();

            conn.Close();

            Populate_DgvPlatiProgramate();

            if (gvPlatiProgramate.Rows.Count < 1)
            {

                btnPlataNouaProgramata.Style.Add("top", "130px");

                btnAnularePlataProgramata.Style.Add("top", "130px");

            }
            else
            {

                btnPlataNouaProgramata.Style.Add("top", "100px");

                btnAnularePlataProgramata.Style.Add("top", "100px");

            }

        }

        private void Populate_DgvPlatiProgramate()
        {

            conn.Open();

            string sql = "SELECT Denumire_Programare AS [Denumire], FORMAT(Data_Programare, 'dd/MM/yyyy') AS [Dată programare], Suma_Programare AS [Sumă programare], Programare_Activa AS [Activă], IBAN_Incasare AS [IBAN Beneficiar]  " +
                "FROM Programari_Plati " +
                "WHERE IBAN_Cont = '" + accountDl.Text + "' AND Programare_Activa = 'True'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            gvPlatiProgramate.DataSource = dt;

            gvPlatiProgramate.DataBind();

            conn.Close();

        }


        protected void BtnAdaugareBeneficiarPredefinit_Click(object sender, EventArgs e)
        {

            lblEroareBeneficiarPredefinit.Visible = false;

            beneficiariPredefiniti.Visible = false;

            adaugareBeneficiarPredefinit.Visible = true;

            txtContBeneficiarNou.Text = "";

            txtNumeBeneficiarNou.Text = "";

            txtDenumireBeneficiarNou.Text = "";

            rblValutaBeneficiarNou.SelectedIndex = -1;


            if (rblValutaBeneficiarNou.Items.Count == 0)
            {

                rblValutaBeneficiarNou.Items.Add("RON");

                rblValutaBeneficiarNou.Items.Add("EURO");

                rblValutaBeneficiarNou.Items.Add("USD");

            }

            rblValutaBeneficiarNou.RepeatDirection = RepeatDirection.Horizontal;

            foreach (System.Web.UI.WebControls.ListItem li in rblValutaBeneficiarNou.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "30px");

            }

        }

        protected void ThirdBtn5_Click(object sender, EventArgs e)
        {

            beneficiariPredefiniti.Visible = true;

            gvBeneficiariPredefiniti.SelectedIndex = -1;

            lblEroareBeneficiarPredefinit.Visible = false;

            Populate_DgvBeneficiari();

            if (gvBeneficiariPredefiniti.Rows.Count < 1)
            {

                btnAdaugareBeneficiarPredefinit.Style.Add("top", "130px");

                btnStergereBeneficiarPredefinit.Style.Add("top", "130px");

            }
            else
            {

                btnAdaugareBeneficiarPredefinit.Style.Add("top", "100px");

                btnStergereBeneficiarPredefinit.Style.Add("top", "100px");

            }

        }

        private void Populate_DgvBeneficiari()
        {

            conn.Open();

            string codClient = (string)Session["codClient"];

            string sql = "SELECT Denumire_Beneficiar AS [Denumire], Nume_Beneficiar AS [Nume], IBAN_Beneficiar AS [IBAN], Valuta_Beneficiar AS [Valută] " +
                "FROM Beneficiari_Predefiniti " +
                "WHERE Cod_Client = '" + codClient + "'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            gvBeneficiariPredefiniti.DataSource = dt;

            gvBeneficiariPredefiniti.DataBind();

            conn.Close();

        }

        protected void BtnAdaugareBeneficiarNou_Click(object sender, EventArgs e)
        {

            if (txtDenumireBeneficiarNou.Text == "" || txtNumeBeneficiarNou.Text == "" || txtContBeneficiarNou.Text == "" || rblValutaBeneficiarNou.SelectedIndex == -1)
            {

                lblEroareBeneficiarPredefinit.Visible = true;

                adaugareBeneficiarPredefinit.Visible = true;

                foreach (System.Web.UI.WebControls.ListItem li in rblValutaBeneficiarNou.Items)
                {

                    li.Attributes.CssStyle.Add("margin-right", "30px");

                }

            }
            else
            {

                conn.Open();

                string selectNrBeneficiar = "SELECT MAX(Nr_Beneficiar) " +
                    "FROM Beneficiari_Predefiniti";

                SqlCommand cmdNrBeneficiar = new SqlCommand(selectNrBeneficiar, conn);

                SqlDataReader readerNrBeneficiar = cmdNrBeneficiar.ExecuteReader();

                readerNrBeneficiar.Read();

                int nrBeneficiar;

                if (readerNrBeneficiar.IsDBNull(0))
                {

                    nrBeneficiar = 1;

                }
                else
                {

                    nrBeneficiar = readerNrBeneficiar.GetInt32(0) + 1;

                }

                readerNrBeneficiar.Close();



                string codClient = (string)Session["codClient"];

                string insertBeneficiar = "INSERT INTO Beneficiari_Predefiniti (Nr_Beneficiar, Denumire_Beneficiar, Nume_Beneficiar, IBAN_Beneficiar, Valuta_Beneficiar, Cod_Client) " +
                    "VALUES (@nr_beneficiar, @denumire_beneficiar, @nume_beneficiar, @iban_beneficiar, @valuta_beneficiar, @cod_client) ";

                SqlCommand cmdB = new SqlCommand(insertBeneficiar, conn);

                cmdB.Parameters.AddWithValue("@nr_beneficiar", nrBeneficiar);

                cmdB.Parameters.AddWithValue("@denumire_beneficiar", txtDenumireBeneficiarNou.Text);

                cmdB.Parameters.AddWithValue("@nume_beneficiar", txtNumeBeneficiarNou.Text);

                cmdB.Parameters.AddWithValue("@iban_beneficiar", txtContBeneficiarNou.Text);

                cmdB.Parameters.AddWithValue("@valuta_beneficiar", rblValutaBeneficiarNou.SelectedItem.Text);

                cmdB.Parameters.AddWithValue("@cod_client", codClient);

                cmdB.ExecuteNonQuery();

                conn.Close();

                adaugareBeneficiarPredefinit.Visible = false;

                beneficiariPredefiniti.Visible = true;

            }

            Populate_DgvBeneficiari();

            if (gvBeneficiariPredefiniti.Rows.Count < 1)
            {

                btnAdaugareBeneficiarPredefinit.Style.Add("top", "130px");

                btnStergereBeneficiarPredefinit.Style.Add("top", "130px");

            }
            else
            {

                btnAdaugareBeneficiarPredefinit.Style.Add("top", "100px");

                btnStergereBeneficiarPredefinit.Style.Add("top", "100px");

            }

        }

        protected void TxtContBeneficiarNou_TextChanged(object sender, EventArgs e)
        {

            adaugareBeneficiarPredefinit.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblValutaBeneficiarNou.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "30px");

            }

            if (txtContBeneficiarNou.Text.Length < 24 || txtContBeneficiarNou.Text == "")
            {

                rblValutaBeneficiarNou.SelectedIndex = -1;

                return;

            }
            else if (txtContBeneficiarNou.Text.Substring(8, 3) == "RON")
            {

                rblValutaBeneficiarNou.SelectedIndex = 0;

            }
            else if (txtContBeneficiarNou.Text.Substring(8, 3) == "EUR")
            {

                rblValutaBeneficiarNou.SelectedIndex = 1;

            }
            else if (txtContBeneficiarNou.Text.Substring(8, 3) == "USD")
            {

                rblValutaBeneficiarNou.SelectedIndex = 2;

            }

        }

        [Obsolete]
        protected void GvBeneficiariPredefiniti_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.DataItemIndex == -1)
                return;
            e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';");

            e.Row.Attributes.Add("onclick", this.GetPostBackClientEvent(gvBeneficiariPredefiniti, "Select$" + e.Row.RowIndex.ToString()));

        }

        protected void GvBeneficiariPredefiniti_SelectedIndexChanged(object sender, EventArgs e)
        {

            beneficiariPredefiniti.Visible = true;

            int index = gvBeneficiariPredefiniti.SelectedRow.RowIndex;

            Session["index"] = index;

        }

        protected void BtnAnulareBeneficiarNou_Click(object sender, EventArgs e)
        {

            beneficiariPredefiniti.Visible = true;

        }

        protected void BtnStergereBeneficiarPredefinit_Click(object sender, EventArgs e)
        {

            beneficiariPredefiniti.Visible = true;

            int index = gvBeneficiariPredefiniti.SelectedRow.RowIndex;

            conn.Open();

            string deleteBeneficiar = "DELETE FROM Beneficiari_Predefiniti " +
                "WHERE IBAN_Beneficiar = '" + gvBeneficiariPredefiniti.Rows[index].Cells[2].Text + "'";

            SqlCommand cmdDeleteBeneficiar = new SqlCommand(deleteBeneficiar, conn);

            cmdDeleteBeneficiar.ExecuteNonQuery();

            conn.Close();

            Populate_DgvBeneficiari();

            if (gvBeneficiariPredefiniti.Rows.Count < 1)
            {

                btnAdaugareBeneficiarPredefinit.Style.Add("top", "130px");

                btnStergereBeneficiarPredefinit.Style.Add("top", "130px");

            }
            else
            {

                btnAdaugareBeneficiarPredefinit.Style.Add("top", "100px");

                btnStergereBeneficiarPredefinit.Style.Add("top", "100px");

            }

        }

        protected void Plati_Programate()
        {

            conn.Open();

            string selectDateProgramari = "SELECT Nr_Programare, Denumire_Programare, Data_Programare, Suma_Programare, Programare_Activa, Detalii_Programare, IBAN_Incasare, IBAN_Cont " +
                "FROM Programari_Plati";

            SqlCommand cmdDateProgramari = new SqlCommand(selectDateProgramari, conn);

            SqlDataAdapter adapterDateProgramari = new SqlDataAdapter(cmdDateProgramari);

            DataTable dtDateProgramari = new DataTable();

            adapterDateProgramari.Fill(dtDateProgramari);

            for (int i = 0; i < dtDateProgramari.Rows.Count; i++)
            {

                if (Convert.ToDateTime(dtDateProgramari.Rows[i][2].ToString()).Date <= DateTime.Now.Date && dtDateProgramari.Rows[i][4].ToString() == "True")
                {

                    decimal sumaProgramare = Convert.ToDecimal(dtDateProgramari.Rows[i][3].ToString());

                    string updateContPlatitor = "UPDATE Conturi " +
                        "SET Sold = Sold - " + sumaProgramare + " " +
                        "WHERE IBAN_Cont = '" + dtDateProgramari.Rows[i][7].ToString() + "'";

                    SqlCommand cmdContPlatitor = new SqlCommand(updateContPlatitor, conn);

                    cmdContPlatitor.ExecuteNonQuery();


                    string updateContBeneficiar = "UPDATE Conturi " +
                        "SET Sold = Sold + " + sumaProgramare + " " +
                        "WHERE IBAN_Cont = '" + dtDateProgramari.Rows[i][6].ToString() + "'";

                    SqlCommand cmdContBeneficiar = new SqlCommand(updateContBeneficiar, conn);

                    cmdContBeneficiar.ExecuteNonQuery();




                    string selectNrOperatiuneP = "SELECT MAX(Nr_Operatiune) " +
                        "FROM Operatiuni";

                    SqlCommand cmdNrP = new SqlCommand(selectNrOperatiuneP, conn);

                    SqlDataReader readerNrP = cmdNrP.ExecuteReader();

                    readerNrP.Read();

                    int nrP;

                    if (readerNrP.IsDBNull(0))
                    {

                        nrP = 1;

                    }
                    else
                    {

                        nrP = Convert.ToInt32(readerNrP[0].ToString()) + 1;

                    }

                    readerNrP.Close();



                    string insertOperatiunePrincipal = "INSERT INTO Operatiuni(Nr_Operatiune, Suma_Operatiune, Data_Operatiune, Detalii_Operatiune, Tip_Operatiune, IBAN_Cont) " +
                        "VALUES(@nr_operatiune, @suma_operatiune, @data_operatiune, @detalii_operatiune, @tip_operatiune, @iban_cont)";

                    SqlCommand cmdIOP = new SqlCommand(insertOperatiunePrincipal, conn);

                    cmdIOP.Parameters.AddWithValue("@nr_operatiune", nrP);

                    cmdIOP.Parameters.AddWithValue("@suma_operatiune", "-" + dtDateProgramari.Rows[i][3].ToString());

                    cmdIOP.Parameters.AddWithValue("@data_operatiune", Convert.ToDateTime(dtDateProgramari.Rows[i][2].ToString()).Date);

                    cmdIOP.Parameters.AddWithValue("@detalii_operatiune", dtDateProgramari.Rows[i][5].ToString());

                    cmdIOP.Parameters.AddWithValue("@tip_operatiune", "PLATA PROGRAMATA");

                    cmdIOP.Parameters.AddWithValue("@iban_cont", dtDateProgramari.Rows[i][7].ToString());

                    cmdIOP.ExecuteNonQuery();




                    string selectNrOperatiuneS = "SELECT MAX(Nr_Operatiune) " +
                        "FROM Operatiuni";

                    SqlCommand cmdNrS = new SqlCommand(selectNrOperatiuneS, conn);

                    SqlDataReader readerNrS = cmdNrS.ExecuteReader();

                    readerNrS.Read();

                    int nrS;

                    if (readerNrS.IsDBNull(0))
                    {

                        nrS = 1;

                    }
                    else
                    {

                        nrS = Convert.ToInt32(readerNrS[0].ToString()) + 1;

                    }

                    readerNrS.Close();



                    string insertOperatiuneSecundar = "INSERT INTO Operatiuni(Nr_Operatiune, Suma_Operatiune, Data_Operatiune, Detalii_Operatiune, Tip_Operatiune, IBAN_Cont) " +
                        "VALUES(@nr_operatiune, @suma_operatiune, @data_operatiune, @detalii_operatiune, @tip_operatiune, @iban_cont)";

                    SqlCommand cmdIOS = new SqlCommand(insertOperatiuneSecundar, conn);

                    cmdIOS.Parameters.AddWithValue("@nr_operatiune", nrS);

                    cmdIOS.Parameters.AddWithValue("@suma_operatiune", dtDateProgramari.Rows[i][3].ToString());

                    cmdIOS.Parameters.AddWithValue("@data_operatiune", Convert.ToDateTime(dtDateProgramari.Rows[i][2].ToString()).Date);

                    cmdIOS.Parameters.AddWithValue("@detalii_operatiune", dtDateProgramari.Rows[i][5].ToString());

                    cmdIOS.Parameters.AddWithValue("@tip_operatiune", "PLATA PROGRAMATA");

                    cmdIOS.Parameters.AddWithValue("@iban_cont", dtDateProgramari.Rows[i][6].ToString());

                    cmdIOS.ExecuteNonQuery();



                    string updateProgramare = "UPDATE Programari_Plati " +
                        "SET Programare_Activa = 'False' " +
                        "WHERE Nr_Programare = " + dtDateProgramari.Rows[i][0].ToString() + "";

                    SqlCommand cmdUpdateProgramare = new SqlCommand(updateProgramare, conn);

                    cmdUpdateProgramare.ExecuteNonQuery();

                }

            }

            conn.Close();

        }

        protected void FirstBtn3_Click(object sender, EventArgs e)
        {

            istoricTranzactiiCont.Visible = true;

            cldDataInceputIstoricCont.Visible = false;

            cldDataSfarsitIstoricCont.Visible = false;

            txtDataInceputIstoricCont.Text = "";

            txtDataSfarsitIstoricCont.Text = "";

            cldDataInceputIstoricCont.SelectedDate = DateTime.MinValue;

            cldDataSfarsitIstoricCont.SelectedDate = DateTime.MinValue;

            lblEroareIstoricCont.Visible = false;

            ibDataInceputIstoricCont.Visible = false;

            ibDataSfarsitIstoricCont.Visible = false;

            txtDataInceputIstoricCont.BackColor = System.Drawing.Color.WhiteSmoke;

            txtDataSfarsitIstoricCont.BackColor = System.Drawing.Color.WhiteSmoke;

            string codClient = (string)Session["codClient"];

            string sql = "SELECT * FROM Conturi WHERE Cod_Client = '" + codClient + "'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            dlCont.DataSource = dt;

            dlCont.DataTextField = "IBAN_Cont";

            dlCont.DataBind();



            if (rblPerioadaCont.Items.Count == 0)
            {

                rblPerioadaCont.Items.Add("TOATE TRANZACȚIILE");

                rblPerioadaCont.Items.Add("SELECTARE PERIOADĂ");

                rblPerioadaCont.RepeatDirection = RepeatDirection.Horizontal;

            }

            rblPerioadaCont.SelectedIndex = 0;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaCont.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }



            if (dlTipTranzactieIstoricCont.Items.Count == 0)
            {

                dlTipTranzactieIstoricCont.Items.Add("");

                dlTipTranzactieIstoricCont.Items.Add("TRANSFER");

                dlTipTranzactieIstoricCont.Items.Add("PLATA");

                dlTipTranzactieIstoricCont.Items.Add("CONVERSIE");

                dlTipTranzactieIstoricCont.Items.Add("PLATA PROGRAMATA");

            }

            dlTipTranzactieIstoricCont.SelectedIndex = 0;



            string sqlLista = "SELECT IBAN_Cont AS [Număr Cont], Tip_Operatiune AS [Tip tranzacție], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Dată], Suma_Operatiune AS [Sumă] " +
                "FROM Operatiuni " +
                "WHERE IBAN_Cont = '" + dlCont.Text + "'";

            SqlCommand cmdLista = new SqlCommand(sqlLista, conn);

            SqlDataAdapter adapterLista = new SqlDataAdapter(cmdLista);

            DataTable dtLista = new DataTable();

            adapterLista.Fill(dtLista);

            gvListaTranzactiiIstoricCont.DataSource = dtLista;

            gvListaTranzactiiIstoricCont.DataBind();

            Session["nrColoaneIstoric"] = dtLista.Columns.Count;

            conn.Close();

            if (dtLista.Rows.Count == 0)
            {

                btnExportPdfIstoricCont.Visible = false;

            }
            else
            {

                btnExportPdfIstoricCont.Visible = true;

            }

        }

        protected void RblPerioadaCont_SelectedIndexChanged(object sender, EventArgs e)
        {

            istoricTranzactiiCont.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaCont.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

            if (rblPerioadaCont.SelectedIndex == 0)
            {

                ibDataInceputIstoricCont.Visible = false;

                ibDataSfarsitIstoricCont.Visible = false;

                txtDataInceputIstoricCont.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataSfarsitIstoricCont.BackColor = System.Drawing.Color.WhiteSmoke;

                txtDataInceputIstoricCont.Text = "";

                txtDataSfarsitIstoricCont.Text = "";

            }
            else if (rblPerioadaCont.SelectedIndex == 1)
            {

                cldDataInceputIstoricCont.SelectedDate = DateTime.MinValue.Date;

                cldDataSfarsitIstoricCont.SelectedDate = DateTime.MinValue.Date;

                ibDataInceputIstoricCont.Visible = true;

                ibDataSfarsitIstoricCont.Visible = true;

                txtDataInceputIstoricCont.BackColor = System.Drawing.Color.White;

                txtDataSfarsitIstoricCont.BackColor = System.Drawing.Color.White;

            }

        }

        protected void IbDataInceputIstoricCont_Click(object sender, ImageClickEventArgs e)
        {

            cldDataInceputIstoricCont.Visible = true;

            istoricTranzactiiCont.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaCont.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

        }

        protected void IbDataSfarsitIstoricCont_Click(object sender, ImageClickEventArgs e)
        {

            cldDataSfarsitIstoricCont.Visible = true;

            istoricTranzactiiCont.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaCont.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

        }

        protected void CldDataInceputIstoricCont_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            istoricTranzactiiCont.Visible = true;

            cldDataInceputIstoricCont.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaCont.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

        }

        protected void CldDataInceputIstoricCont_SelectionChanged(object sender, EventArgs e)
        {

            istoricTranzactiiCont.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaCont.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

            cldDataInceputIstoricCont.Visible = false;

            if (txtDataSfarsitIstoricCont.Text == "")
            {

                txtDataInceputIstoricCont.Text = cldDataInceputIstoricCont.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataSfarsitIstoricCont.Text != "" && cldDataInceputIstoricCont.SelectedDate > cldDataSfarsitIstoricCont.SelectedDate)
            {

                txtDataInceputIstoricCont.Text = "Data incorectă";

            }
            else
            {

                txtDataInceputIstoricCont.Text = cldDataInceputIstoricCont.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void CldDataSfarsitIstoricCont_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {

            istoricTranzactiiCont.Visible = true;

            cldDataSfarsitIstoricCont.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaCont.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

        }

        protected void CldDataSfarsitIstoricCont_SelectionChanged(object sender, EventArgs e)
        {

            istoricTranzactiiCont.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaCont.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

            cldDataSfarsitIstoricCont.Visible = false;

            if (txtDataInceputIstoricCont.Text == "")
            {

                txtDataSfarsitIstoricCont.Text = cldDataSfarsitIstoricCont.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }
            else if (txtDataInceputIstoricCont.Text != "" && cldDataSfarsitIstoricCont.SelectedDate < cldDataInceputIstoricCont.SelectedDate)
            {

                txtDataSfarsitIstoricCont.Text = "Data incorectă";

            }
            else
            {

                txtDataSfarsitIstoricCont.Text = cldDataSfarsitIstoricCont.SelectedDate.ToString("dd'/'MM'/'yyyy");

            }

        }

        protected void BtnVizualizareTranzactiiIstoricCont_Click(object sender, EventArgs e)
        {

            istoricTranzactiiCont.Visible = true;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaCont.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

            if (rblPerioadaCont.SelectedIndex == 0)
            {

                if (dlCont.Text == "")
                {

                    lblEroareIstoricCont.Visible = true;

                    return;

                }
                else
                {

                    lblEroareIstoricCont.Visible = false;

                }

            }
            else if (rblPerioadaCont.SelectedIndex == 1)
            {

                if (txtDataInceputIstoricCont.Text == "" || txtDataSfarsitIstoricCont.Text == "")
                {

                    lblEroareIstoricCont.Visible = true;

                    return;

                }
                else
                {

                    lblEroareIstoricCont.Visible = false;

                }

            }

            DataTable dt = new DataTable();

            if (rblPerioadaCont.SelectedIndex == 0)
            {

                if (dlTipTranzactieIstoricCont.SelectedIndex == 0)
                {

                    conn.Open();

                    string sql = "SELECT IBAN_Cont AS [Număr Cont], Tip_Operatiune AS [Tip tranzacție], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Dată], Suma_Operatiune AS [Sumă] " +
                        "FROM Operatiuni " +
                        "WHERE IBAN_Cont = '" + dlCont.Text + "'";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(dt);

                    gvListaTranzactiiIstoricCont.DataSource = dt;

                    gvListaTranzactiiIstoricCont.DataBind();

                    conn.Close();

                }
                else
                {

                    conn.Open();

                    string sql = "SELECT IBAN_Cont AS [Număr Cont], Tip_Operatiune AS [Tip tranzacție], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Dată], Suma_Operatiune AS [Sumă] " +
                        "FROM Operatiuni " +
                        "WHERE IBAN_Cont = '" + dlCont.Text + "' AND Tip_Operatiune = '" + dlTipTranzactieIstoricCont.Text + "'";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(dt);

                    gvListaTranzactiiIstoricCont.DataSource = dt;

                    gvListaTranzactiiIstoricCont.DataBind();

                    conn.Close();

                }


            }
            else if (rblPerioadaCont.SelectedIndex == 1)
            {

                if (dlTipTranzactieIstoricCont.SelectedIndex == 0)
                {

                    conn.Open();

                    string sql = "SELECT IBAN_Cont AS [Număr Cont], Tip_Operatiune AS [Tip tranzacție], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Dată], Suma_Operatiune AS [Sumă] " +
                        "FROM Operatiuni " +
                        "WHERE IBAN_Cont = '" + dlCont.Text + "' AND Data_Operatiune BETWEEN '" + DateTime.Parse(txtDataInceputIstoricCont.Text).ToString("yyyy'-'MM'-'dd") + "' AND '" + DateTime.Parse(txtDataSfarsitIstoricCont.Text).ToString("yyyy'-'MM'-'dd") + "'";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(dt);

                    gvListaTranzactiiIstoricCont.DataSource = dt;

                    gvListaTranzactiiIstoricCont.DataBind();

                    conn.Close();

                }
                else
                {

                    conn.Open();

                    string sql = "SELECT IBAN_Cont AS [Număr Cont], Tip_Operatiune AS [Tip tranzacție], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Dată], Suma_Operatiune AS [Sumă] " +
                        "FROM Operatiuni " +
                        "WHERE IBAN_Cont = '" + dlCont.Text + "' AND Tip_Operatiune = '" + dlTipTranzactieIstoricCont.Text + "' AND Data_Operatiune BETWEEN '" + DateTime.Parse(txtDataInceputIstoricCont.Text).ToString("yyyy'-'MM'-'dd") + "' AND '" + DateTime.Parse(txtDataSfarsitIstoricCont.Text).ToString("yyyy'-'MM'-'dd") + "'";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(dt);

                    gvListaTranzactiiIstoricCont.DataSource = dt;

                    gvListaTranzactiiIstoricCont.DataBind();

                    conn.Close();

                }

            }

            if (dt.Rows.Count < 1)
            {

                btnExportPdfIstoricCont.Visible = false;

            }
            else
            {

                btnExportPdfIstoricCont.Visible = true;

            }

        }

        protected void BtnExportPdfIstoricCont_Click(object sender, EventArgs e)
        {

            string selectedPath = "";

            Thread t = new Thread((ThreadStart)(() =>
            {
                SaveFileDialog savePdf = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf|*.pdf",
                    FilterIndex = 2,
                    RestoreDirectory = true
                };

                if (savePdf.ShowDialog() == DialogResult.OK)
                {

                    selectedPath = savePdf.FileName;

                }

                string template = "D:\\Disertatie\\Templates\\template.pdf";

                PdfReader readerPdf = new PdfReader(template);

                Rectangle size = readerPdf.GetPageSizeWithRotation(1);

                Document pdfFile = new Document(size);

                FileStream fs = new FileStream(selectedPath, FileMode.Create, FileAccess.Write); ;

                PdfWriter writer = PdfWriter.GetInstance(pdfFile, fs);

                pdfFile.Open();

                PdfContentByte cb = writer.DirectContent;

                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb.SetColorFill(BaseColor.DARK_GRAY);
                cb.SetFontAndSize(bf, 8);

                string background_pdf = "D:\\Disertatie\\Poze\\template_background.png";

                iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(background_pdf);

                png.SetAbsolutePosition(0, 0);

                png.ScaleToFit(pdfFile.PageSize.Width, pdfFile.PageSize.Height);

                pdfFile.Add(png);

                BaseFont customfont = BaseFont.CreateFont("D:\\Disertatie\\Templates\\calibri.ttf", BaseFont.CP1252, BaseFont.EMBEDDED);

                Font generalFont = new Font(customfont, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT, new Phrase("SWIFT: RBRLRO22", generalFont), 220, 742, 0);

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT, new Phrase("C.U.I. RO 50 22 670", generalFont), 220, 730, 0);

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT, new Phrase("R.B. - P.J.R. - 12 - 019 - 05.05.2022", generalFont), 220, 718, 0);

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT, new Phrase("Nr. Inreg. Reg. Com.: J12 / 4155 / 2022", generalFont), 220, 706, 0);


                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_RIGHT, new Phrase("Imprimat de: " + nameLbl.Text.ToUpper() + "", generalFont), 550, 718, 0);

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_RIGHT, new Phrase("În data de: " + DateTime.Now.Date.ToString("dd'/'MM'/'yyyy") + "", generalFont), 550, 706, 0);




                DataTable dt = new DataTable();


                if (rblPerioadaCont.SelectedIndex == 0)
                {

                    if (dlTipTranzactieIstoricCont.SelectedIndex == 0)
                    {

                        conn.Open();

                        string sql = "SELECT IBAN_Cont AS [Numar Cont], Tip_Operatiune AS [Tip tranzactie], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Data], Suma_Operatiune AS [Suma] " +
                            "FROM Operatiuni " +
                            "WHERE IBAN_Cont = '" + dlCont.Text + "'";

                        SqlCommand cmd = new SqlCommand(sql, conn);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        adapter.Fill(dt);

                        conn.Close();

                    }
                    else
                    {

                        conn.Open();

                        string sql = "SELECT IBAN_Cont AS [Numar Cont], Tip_Operatiune AS [Tip tranzactie], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Data], Suma_Operatiune AS [Suma] " +
                            "FROM Operatiuni " +
                            "WHERE IBAN_Cont = '" + dlCont.Text + "' AND Tip_Operatiune = '" + dlTipTranzactieIstoricCont.Text + "'";

                        SqlCommand cmd = new SqlCommand(sql, conn);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        adapter.Fill(dt);

                        conn.Close();

                    }


                }
                else if (rblPerioadaCont.SelectedIndex == 1)
                {

                    if (dlTipTranzactieIstoricCont.SelectedIndex == 0)
                    {

                        conn.Open();

                        string sql = "SELECT IBAN_Cont AS [Numar Cont], Tip_Operatiune AS [Tip tranzactie], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Data], Suma_Operatiune AS [Suma] " +
                            "FROM Operatiuni " +
                            "WHERE IBAN_Cont = '" + dlCont.Text + "' AND Data_Operatiune BETWEEN '" + DateTime.Parse(txtDataInceputIstoricCont.Text).ToString("yyyy'-'MM'-'dd") + "' AND '" + DateTime.Parse(txtDataSfarsitIstoricCont.Text).ToString("yyyy'-'MM'-'dd") + "'";

                        SqlCommand cmd = new SqlCommand(sql, conn);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        adapter.Fill(dt);

                        conn.Close();

                    }
                    else
                    {

                        conn.Open();

                        string sql = "SELECT IBAN_Cont AS [Numar Cont], Tip_Operatiune AS [Tip tranzactie], Detalii_Operatiune AS [Detalii], FORMAT(Data_Operatiune, 'dd/MM/yyyy') AS [Data], Suma_Operatiune AS [Suma] " +
                            "FROM Operatiuni " +
                            "WHERE IBAN_Cont = '" + dlCont.Text + "' AND Tip_Operatiune = '" + dlTipTranzactieIstoricCont.Text + "' AND Data_Operatiune BETWEEN '" + DateTime.Parse(txtDataInceputIstoricCont.Text).ToString("yyyy'-'MM'-'dd") + "' AND '" + DateTime.Parse(txtDataSfarsitIstoricCont.Text).ToString("yyyy'-'MM'-'dd") + "'";

                        SqlCommand cmd = new SqlCommand(sql, conn);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        adapter.Fill(dt);

                        conn.Close();

                    }


                }


                Font headerFont = new Font(customfont, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                Font titleFont = new Font(customfont, 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK);


                Paragraph titleParagraph = new Paragraph
                {
                    Alignment = Element.ALIGN_CENTER,

                    Font = titleFont

                };

                titleParagraph.Add("ISTORIC CONT");



                Paragraph criteriiCautare = new Paragraph
                {
                    Alignment = Element.ALIGN_LEFT,

                    Font = headerFont
                };

                criteriiCautare.Add("Criterii de cautare utilizate:");

                ColumnText subtitluCriterii = new ColumnText(cb);

                subtitluCriterii.SetSimpleColumn(42, 560, 500, 100);

                subtitluCriterii.AddText(criteriiCautare);

                subtitluCriterii.Go();




                Paragraph listaCriterii = new Paragraph
                {
                    Alignment = Element.ALIGN_LEFT,

                    Font = generalFont
                };


                listaCriterii.Add("Cont:                    " + dlCont.Text + "\n");

                if (rblPerioadaCont.SelectedIndex == 0)
                {

                    listaCriterii.Add("Perioada:            Nedeterminata \n");

                }
                else
                {

                    listaCriterii.Add("Perioada:            " + txtDataInceputIstoricCont.Text + "  -  " + txtDataSfarsitIstoricCont.Text + "\n");

                }


                if (dlTipTranzactieIstoricCont.SelectedIndex == 0)
                {

                    listaCriterii.Add("Tip tranzactie:    Toate \n");

                }
                else
                {

                    listaCriterii.Add("Tip tranzactie:    " + dlTipTranzactieIstoricCont.Text + "\n");

                }


                ColumnText listaC = new ColumnText(cb);

                listaC.SetSimpleColumn(42, 540, 500, 100);

                listaC.AddText(listaCriterii);

                listaC.Go();




                Paragraph tableParagraph = new Paragraph();

                PdfPTable istoricTable = new PdfPTable(dt.Columns.Count);


                PdfPCell pdfCell = new PdfPCell();

                for (int i = 0; i <= dt.Columns.Count - 1; i++)
                {

                    pdfCell = new PdfPCell(new Phrase(new Chunk(dt.Columns[i].ColumnName, headerFont)));

                    pdfCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;

                    pdfCell.BackgroundColor = new BaseColor(System.Drawing.Color.FromArgb(169, 200, 221));

                    pdfCell.FixedHeight = 20F;

                    istoricTable.AddCell(pdfCell);

                }

                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {

                    for (int j = 0; j <= dt.Columns.Count - 1; j++)
                    {

                        pdfCell = new PdfPCell(new Phrase(dt.Rows[i][j].ToString(), generalFont));

                        pdfCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;

                        istoricTable.AddCell(pdfCell);

                    }

                }


                titleParagraph.SpacingBefore = 200F;

                criteriiCautare.SpacingBefore = 50F;

                tableParagraph.SpacingBefore = 150F;

                istoricTable.WidthPercentage = 98;

                tableParagraph.Add(istoricTable);

                pdfFile.Add(titleParagraph);

                pdfFile.Add(tableParagraph);

                pdfFile.Close();

                fs.Close();

                writer.Close();

                readerPdf.Close();

            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            System.Diagnostics.Process.Start(selectedPath);

        }

        protected void FifthBtn2_Click(object sender, EventArgs e)
        {

            contractareImprumut.Visible = true;

            rblAsigurareSanatate.RepeatDirection = RepeatDirection.Horizontal;

            txtSumaImprumut.Text = "";

            lblEroareImprumut.Visible = false;

            lblEroareSuma.Visible = false;


            if (rblTipImprumut.Items.Count == 0)
            {

                rblTipImprumut.Items.Add("Nevoi personale");
                rblTipImprumut.Items.Add("Ipotecar");

            }

            if (rblTipDobanda.Items.Count == 0)
            {

                rblTipDobanda.Items.Add("Fixă");
                rblTipDobanda.Items.Add("Variabilă");

            }

            if (rblTipRata.Items.Count == 0)
            {

                rblTipRata.Items.Add("Egală");
                rblTipRata.Items.Add("Descrescătoare");

            }

            if (rblAsigurareSanatate.Items.Count == 0)
            {

                rblAsigurareSanatate.Items.Add("Da");
                rblAsigurareSanatate.Items.Add("Nu");

            }

            foreach (System.Web.UI.WebControls.ListItem li in rblAsigurareSanatate.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }


            rblAsigurareSanatate.SelectedIndex = 0;

            rblTipDobanda.SelectedIndex = 0;

            rblTipImprumut.SelectedIndex = 0;

            rblTipRata.SelectedIndex = 0;


            txtDurataImprumut.Text = "12";

            txtDataContractare.Text = DateTime.Now.Date.ToString("dd'/'MM'/'yyyy");

            txtDataScadenta.Text = DateTime.Now.Date.AddMonths(Convert.ToInt32(txtDurataImprumut.Text)).ToString("dd'/'MM'/'yyyy");


            txtContPlatitor.Text = accountDl.Text;


            string codClient = (string)Session["codClient"];

            conn.Open();

            string selectVenit = "SELECT Venit " +
                "FROM Clienti " +
                "WHERE Cod_Client = " + codClient + "";

            SqlCommand cmdVenit = new SqlCommand(selectVenit, conn);

            SqlDataReader readerVenit = cmdVenit.ExecuteReader();

            readerVenit.Read();

            txtVenit.Text = readerVenit[0].ToString();

            readerVenit.Close();

            conn.Close();


            double sumaMaxima = Convert.ToDouble(txtVenit.Text) * 0.4 * Convert.ToInt32(txtDurataImprumut.Text);

            txtSumaMaxima.Text = sumaMaxima.ToString("0.00");

        }

        protected void BtnCresteDurata_Click(object sender, EventArgs e)
        {

            contractareImprumut.Visible = true;

            txtSumaImprumut.Text = "";

            int nrLuni = Convert.ToInt32(txtDurataImprumut.Text);

            if (nrLuni <= 59)
            {

                nrLuni += 1;

                txtDurataImprumut.Text = nrLuni.ToString();

            }

            foreach (System.Web.UI.WebControls.ListItem li in rblAsigurareSanatate.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

            double sumaMaxima = Convert.ToDouble(txtVenit.Text) * 0.4 * Convert.ToInt32(txtDurataImprumut.Text);

            txtSumaMaxima.Text = sumaMaxima.ToString("0.00");


            txtDataScadenta.Text = DateTime.Now.Date.AddMonths(Convert.ToInt32(txtDurataImprumut.Text)).ToString("dd'/'MM'/'yyyy");

        }

        protected void BtnScadeDurata_Click(object sender, EventArgs e)
        {

            contractareImprumut.Visible = true;

            txtSumaImprumut.Text = "";

            int nrLuni = Convert.ToInt32(txtDurataImprumut.Text);

            if (nrLuni >= 13)
            {

                nrLuni -= 1;

                txtDurataImprumut.Text = nrLuni.ToString();

            }

            foreach (System.Web.UI.WebControls.ListItem li in rblAsigurareSanatate.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "20px");

            }

            double sumaMaxima = Convert.ToDouble(txtVenit.Text) * 0.4 * Convert.ToInt32(txtDurataImprumut.Text);

            txtSumaMaxima.Text = sumaMaxima.ToString("0.00");


            txtDataScadenta.Text = DateTime.Now.Date.AddMonths(Convert.ToInt32(txtDurataImprumut.Text)).ToString("dd'/'MM'/'yyyy");

        }

        protected void BtnContractareImprumut_Click(object sender, EventArgs e)
        {

            if (txtSumaImprumut.Text == "")
            {

                lblEroareImprumut.Visible = true;

                lblEroareSuma.Visible = false;

                contractareImprumut.Visible = true;

            }
            else if (Convert.ToDecimal(txtSumaImprumut.Text) > Convert.ToDecimal(txtSumaMaxima.Text))
            {

                lblEroareImprumut.Visible = false;

                lblEroareSuma.Visible = true;

                contractareImprumut.Visible = true;

            }
            else
            {

                conn.Open();

                string selectNrCredit = "SELECT MAX(Nr_Credit) " +
                "FROM Credite";

                SqlCommand cmdNrCredit = new SqlCommand(selectNrCredit, conn);

                SqlDataReader readerNrCredit = cmdNrCredit.ExecuteReader();

                readerNrCredit.Read();

                int nrCredit;

                if (readerNrCredit.IsDBNull(0))
                {

                    nrCredit = 1;

                }
                else
                {

                    nrCredit = readerNrCredit.GetInt32(0) + 1;

                }

                readerNrCredit.Close();



                string nrIbanCredit = "SELECT MAX(RIGHT(IBAN_Credit,2)) " +
                                "FROM Credite";

                SqlCommand cmdNr = new SqlCommand(nrIbanCredit, conn);

                SqlDataReader readerNr = cmdNr.ExecuteReader();

                readerNr.Read();

                int nrIban;

                if (readerNr.IsDBNull(0))
                {

                    nrIban = 1;

                }
                else
                {

                    nrIban = Convert.ToInt32(readerNr[0].ToString()) + 1;

                }



                readerNr.Close();

                string selectedIBAN = accountDl.Text;

                string codClient = (string)Session["codClient"];

                string creditIBAN;


                if (nrIban > 9)
                {

                    creditIBAN = selectedIBAN.Substring(0, 11) + "CRD0" + codClient + Convert.ToString(nrIban);

                }
                else
                {

                    creditIBAN = selectedIBAN.Substring(0, 11) + "CRD0" + codClient + "0" + Convert.ToString(nrIban);

                }



                string tipDobanda = "";

                if (rblTipDobanda.SelectedIndex == 0)
                {

                    tipDobanda = "Fixa";

                }
                else if (rblTipDobanda.SelectedIndex == 1)
                {

                    tipDobanda = "Variabila";

                }




                string tipRata = "";

                if (rblTipRata.SelectedIndex == 0)
                {

                    tipRata = "False";

                }
                else if (rblTipRata.SelectedIndex == 1)
                {

                    tipRata = "True";

                }



                string idDobanda = "SELECT ID_Dobanda " +
                    "FROM Dobanda_Credit " +
                    "WHERE Tip_Dobanda = '" + tipDobanda + "'";

                SqlCommand idD = new SqlCommand(idDobanda, conn);

                SqlDataReader readerId = idD.ExecuteReader();

                readerId.Read();

                int idDob = Convert.ToInt32(readerId[0].ToString());

                readerId.Close();




                string insertCredit = "INSERT INTO Credite (Nr_Credit, IBAN_Credit, Tip_Credit, Valoare_Credit, Suma_Rambursata, Sold_Credit, Data_Contractare, Data_Scadenta, Rata_Descrescatoare, Credit_Activ, IBAN_Cont, ID_Dobanda, Data_Actualizare_Urmatoare) " +
                    "VALUES (@nr_credit, @iban_credit, @tip_credit, @valoare_credit, @suma_rambursata, @sold_credit, @data_contractare, @data_scadenta, @rata_descrescatoare, @credit_activ, @iban_cont, @id_dobanda, @data_actualizare_urmatoare)";

                SqlCommand cmdInsertProgramare = new SqlCommand(insertCredit, conn);

                cmdInsertProgramare.Parameters.AddWithValue("@nr_credit", nrCredit);

                cmdInsertProgramare.Parameters.AddWithValue("@iban_credit", creditIBAN);

                cmdInsertProgramare.Parameters.AddWithValue("@tip_credit", rblTipImprumut.SelectedItem.Text);

                cmdInsertProgramare.Parameters.AddWithValue("@valoare_credit", txtSumaImprumut.Text);

                cmdInsertProgramare.Parameters.AddWithValue("@suma_rambursata", 0);

                cmdInsertProgramare.Parameters.AddWithValue("@sold_credit", txtSumaImprumut.Text);

                cmdInsertProgramare.Parameters.AddWithValue("@data_contractare", DateTime.Parse(txtDataContractare.Text));

                cmdInsertProgramare.Parameters.AddWithValue("@data_scadenta", DateTime.Parse(txtDataScadenta.Text));

                cmdInsertProgramare.Parameters.AddWithValue("@rata_descrescatoare", tipRata);

                cmdInsertProgramare.Parameters.AddWithValue("@credit_activ", "True");

                cmdInsertProgramare.Parameters.AddWithValue("@iban_cont", txtContPlatitor.Text);

                cmdInsertProgramare.Parameters.AddWithValue("@id_dobanda", idDob);

                cmdInsertProgramare.Parameters.AddWithValue("@data_actualizare_urmatoare", DateTime.Parse(txtDataContractare.Text).AddMonths(1));

                cmdInsertProgramare.ExecuteNonQuery();


                conn.Close();

                Populate_DgvCredite();

                vizualizareImprumuturi.Visible = true;

            }

        }

        protected void BtnScadentarImprumut_Click(object sender, EventArgs e)
        {

            if (txtSumaImprumut.Text == "")
            {

                lblEroareImprumut.Visible = true;

                lblEroareSuma.Visible = false;

                contractareImprumut.Visible = true;

            }
            else if (Convert.ToDouble(txtSumaImprumut.Text) > Convert.ToDouble(txtSumaMaxima.Text))
            {

                lblEroareSuma.Visible = true;

                lblEroareImprumut.Visible = false;

                contractareImprumut.Visible = true;

            }
            else
            {

                string selectedPath = "";

                Thread t = new Thread((ThreadStart)(() =>
                {
                    SaveFileDialog savePdf = new SaveFileDialog
                    {
                        Filter = "PDF Files (*.pdf|*.pdf",
                        FilterIndex = 2,
                        RestoreDirectory = true
                    };

                    if (savePdf.ShowDialog() == DialogResult.OK)
                    {

                        selectedPath = savePdf.FileName;

                    }

                    string template = "D:\\Disertatie\\Templates\\template.pdf";

                    PdfReader readerPdf = new PdfReader(template);

                    Rectangle size = readerPdf.GetPageSizeWithRotation(1);

                    Document pdfFile = new Document(size);

                    FileStream fs = new FileStream(selectedPath, FileMode.Create, FileAccess.Write); ;

                    PdfWriter writer = PdfWriter.GetInstance(pdfFile, fs);

                    pdfFile.Open();

                    PdfContentByte cb = writer.DirectContent;

                    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb.SetColorFill(BaseColor.DARK_GRAY);
                    cb.SetFontAndSize(bf, 8);

                    string background_pdf = "D:\\Disertatie\\Poze\\template_background.png";

                    iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(background_pdf);

                    png.SetAbsolutePosition(0, 0);

                    png.ScaleToFit(pdfFile.PageSize.Width, pdfFile.PageSize.Height);

                    pdfFile.Add(png);

                    BaseFont customfont = BaseFont.CreateFont("D:\\Disertatie\\Templates\\calibri.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                    BaseFont customFontScadentar = BaseFont.CreateFont("D:\\Disertatie\\Templates\\Montserrat-Bold.otf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                    BaseFont customDetaliiScadentar = BaseFont.CreateFont("D:\\Disertatie\\Templates\\Montserrat-SemiBold.otf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                    Font generalFont = new Font(customfont, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                    Font scadentar = new Font(customFontScadentar, 24, iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);

                    Font subtitluScadentarFont = new Font(customFontScadentar, 16, iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);

                    Font detaliiScadentarFont = new Font(customDetaliiScadentar, 12, iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);

                    ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT, new Phrase("SWIFT: RBRLRO22", generalFont), 220, 742, 0);

                    ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT, new Phrase("C.U.I. RO 50 22 670", generalFont), 220, 730, 0);

                    ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT, new Phrase("R.B. - P.J.R. - 12 - 019 - 05.05.2022", generalFont), 220, 718, 0);

                    ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT, new Phrase("Nr. Inreg. Reg. Com.: J12 / 4155 / 2022", generalFont), 220, 706, 0);

                    ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_RIGHT, new Phrase("Imprimat de: " + nameLbl.Text.ToUpper() + "", generalFont), 550, 718, 0);

                    ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_RIGHT, new Phrase("În data de: " + DateTime.Now.Date.ToString("dd'/'MM'/'yyyy") + "", generalFont), 550, 706, 0);





                    Font headerFont = new Font(customfont, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);


                    Paragraph titleParagraph = new Paragraph
                    {
                        Alignment = Element.ALIGN_CENTER,

                        Font = scadentar

                    };

                    titleParagraph.Add("SCADENȚAR");



                    Paragraph detaliiScadentar = new Paragraph
                    {
                        Alignment = Element.ALIGN_LEFT,

                        Font = subtitluScadentarFont
                    };

                    detaliiScadentar.Add("Detalii scadențar:");

                    ColumnText subtitluCriterii = new ColumnText(cb);

                    subtitluCriterii.SetSimpleColumn(42, 540, 500, 100);

                    subtitluCriterii.AddText(detaliiScadentar);

                    subtitluCriterii.Go();


                    conn.Open();

                    string selectDobanda = "SELECT Dobanda_Anuala " +
                    "FROM Dobanda_Credit " +
                    "WHERE Tip_Dobanda = '" + rblTipDobanda.SelectedItem.Text + "'";

                    SqlCommand cmdDobanda = new SqlCommand(selectDobanda, conn);

                    SqlDataReader readerDobanda = cmdDobanda.ExecuteReader();

                    readerDobanda.Read();

                    double valoareDobanda = Convert.ToDouble(readerDobanda.GetDecimal(0));

                    readerDobanda.Close();

                    conn.Close();

                    double sumaImprumut;

                    int nrLuni;

                    double numarator;

                    double numitor;

                    double sumaRata;

                    double sumaDobanda;

                    double sumaPrincipal;

                    if (rblTipRata.SelectedIndex == 0)
                    {

                        sumaImprumut = Convert.ToDouble(txtSumaImprumut.Text);

                        nrLuni = Convert.ToInt32(txtDurataImprumut.Text);

                        numarator = sumaImprumut * (valoareDobanda / 100);

                        numitor = 12 * (1 - Math.Pow(1 / (1 + (valoareDobanda / 100) / 12), nrLuni));

                        sumaRata = numarator / numitor;

                        sumaDobanda = (sumaImprumut * (valoareDobanda / 100) * (nrLuni / 12)) / nrLuni;

                        sumaPrincipal = sumaRata - sumaDobanda;

                    }
                    else
                    {

                        sumaImprumut = Convert.ToDouble(txtSumaImprumut.Text);

                        nrLuni = Convert.ToInt32(txtDurataImprumut.Text);

                        sumaPrincipal = sumaImprumut / nrLuni;

                        sumaDobanda = (sumaImprumut * (valoareDobanda / 100) * (nrLuni / 12)) / nrLuni;

                        sumaRata = sumaPrincipal + sumaDobanda;

                    }


                    double sumaComision = 0;



                    conn.Open();

                    string selectComision = "SELECT Valoare_Comision " +
                        "FROM Comisioane " +
                        "WHERE Tip_Comision = 'Asigurare de sanatate'";

                    SqlCommand cmdComision = new SqlCommand(selectComision, conn);

                    SqlDataReader readerComision = cmdComision.ExecuteReader();

                    if (rblAsigurareSanatate.SelectedIndex == 0)
                    {

                        readerComision.Read();

                        sumaComision = Convert.ToDouble(readerComision.GetDecimal(0)) * Convert.ToDouble(sumaImprumut);

                        readerComision.Close();

                    }
                    else if (rblAsigurareSanatate.SelectedIndex == 1)
                    {

                        sumaComision = 0.00;

                    }

                    conn.Close();




                    DataTable dtScandetar = new DataTable();

                    dtScandetar.Columns.Add("LUNA", typeof(int));
                    dtScandetar.Columns.Add("PRINCIPAL", typeof(double));
                    dtScandetar.Columns.Add("DOBÂNDĂ", typeof(double));
                    dtScandetar.Columns.Add("COMISIOANE", typeof(double));
                    dtScandetar.Columns.Add("RATĂ LUNARĂ", typeof(double));
                    dtScandetar.Columns.Add("SOLD", typeof(double));

                    Paragraph tableParagraph = new Paragraph();

                    PdfPTable istoricTable = new PdfPTable(dtScandetar.Columns.Count);


                    PdfPCell pdfCell = new PdfPCell();

                    for (int i = 0; i <= dtScandetar.Columns.Count - 1; i++)
                    {

                        pdfCell = new PdfPCell(new Phrase(new Chunk(dtScandetar.Columns[i].ColumnName, headerFont)))
                        {
                            HorizontalAlignment = PdfPCell.ALIGN_CENTER,

                            BackgroundColor = new BaseColor(System.Drawing.Color.FromArgb(169, 200, 221)),

                            FixedHeight = 20F
                        };

                        istoricTable.AddCell(pdfCell);

                    }

                    double soldImprumut = sumaImprumut - sumaPrincipal;

                    double sumaRataDescrescatoare = 0;

                    for (int i = 0; i <= Convert.ToInt32(txtDurataImprumut.Text) - 1; i++)
                    {

                        DataRow dr = dtScandetar.NewRow();

                        dr["LUNA"] = i + 1;
                        dr["PRINCIPAL"] = Math.Round(sumaPrincipal, 2);
                        dr["DOBÂNDĂ"] = Math.Round(sumaDobanda, 2);
                        dr["COMISIOANE"] = Math.Round(sumaComision, 2);
                        dr["RATĂ LUNARĂ"] = Math.Round(sumaRata, 2);
                        dr["SOLD"] = Math.Round(soldImprumut, 2);

                        dtScandetar.Rows.Add(dr);

                        for (int j = 0; j <= 6 - 1; j++)
                        {

                            pdfCell = new PdfPCell(new Phrase(dtScandetar.Rows[i][j].ToString(), generalFont))
                            {
                                HorizontalAlignment = PdfPCell.ALIGN_CENTER
                            };

                            istoricTable.AddCell(pdfCell);

                        }

                        if (rblTipRata.SelectedIndex == 0)
                        {

                            sumaDobanda = (soldImprumut * (valoareDobanda / 100) * (nrLuni / 12)) / nrLuni;

                            sumaPrincipal = sumaRata - sumaDobanda;

                            soldImprumut -= sumaPrincipal;

                            if (i == Convert.ToInt32(txtDurataImprumut.Text) - 2)
                            {

                                soldImprumut = 0;

                            }

                        }
                        else
                        {

                            sumaRataDescrescatoare += sumaRata;

                            sumaDobanda = (soldImprumut * (valoareDobanda / 100) * (nrLuni / 12)) / nrLuni;

                            sumaRata = sumaPrincipal + sumaDobanda;

                            soldImprumut -= sumaPrincipal;

                            if (i == Convert.ToInt32(txtDurataImprumut.Text) - 2)
                            {

                                soldImprumut = 0;

                            }

                        }



                    }


                    Paragraph listaDetalii = new Paragraph
                    {
                        Alignment = Element.ALIGN_LEFT,

                        Font = detaliiScadentarFont
                    };

                    if (rblTipRata.SelectedIndex == 0)
                    {

                        listaDetalii.Add("\n" + "Sumă împrumutată:               " + txtSumaImprumut.Text + " LEI" + "\n");

                        listaDetalii.Add("Sumă totală de plată:             " + Math.Round(sumaRata * nrLuni, 2) + " LEI" + "\n");

                        listaDetalii.Add("Dobânda anuală efectivă:     " + valoareDobanda.ToString() + " % \n");

                        listaDetalii.Add("Tip dobândă:                            " + rblTipDobanda.SelectedItem.Text + "\n");

                    }
                    else
                    {

                        listaDetalii.Add("\n" + "Sumă împrumutată:               " + txtSumaImprumut.Text + " LEI" + "\n");

                        listaDetalii.Add("Sumă totală de plată:             " + Math.Round(sumaRataDescrescatoare, 2) + " LEI" + "\n");

                        listaDetalii.Add("Dobânda anuală efectivă:     " + valoareDobanda.ToString() + " % \n");

                        listaDetalii.Add("Tip dobândă:                            " + rblTipDobanda.SelectedItem.Text + "\n");

                    }


                    ColumnText listaC = new ColumnText(cb);

                    listaC.SetSimpleColumn(42, 520, 500, 100);

                    listaC.AddText(listaDetalii);

                    listaC.Go();



                    titleParagraph.SpacingBefore = 200F;

                    pdfFile.Add(titleParagraph);

                    detaliiScadentar.SpacingBefore = 400F;

                    listaDetalii.SpacingBefore = 400F;

                    tableParagraph.SpacingBefore = 200F;

                    istoricTable.WidthPercentage = 98;

                    tableParagraph.Add(istoricTable);

                    pdfFile.Add(tableParagraph);

                    pdfFile.Close();

                    fs.Close();

                    writer.Close();

                    readerPdf.Close();

                }));

                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();

                System.Diagnostics.Process.Start(selectedPath);

                contractareImprumut.Visible = true;

                lblEroareImprumut.Visible = false;

                lblEroareSuma.Visible = false;

            }
        }


        private void Populate_DgvCredite()
        {

            conn.Open();

            string codClient = (string)Session["codClient"];

            string sql = "SELECT IBAN_Credit AS [ID Credit], Tip_Credit AS [Tip Credit], Valoare_Credit AS [Suma împrumutată], FORMAT(Data_Scadenta, 'dd/MM/yyyy') AS [Dată scadență] " +
                "FROM Conturi INNER JOIN Credite ON Conturi.IBAN_Cont=Credite.IBAN_Cont " +
                "WHERE Cod_Client = '" + codClient + "' AND Credit_Activ = 'True'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            gvCredite.DataSource = dt;

            gvCredite.DataBind();

            conn.Close();

        }


        protected void FifthBtn1_Click(object sender, EventArgs e)
        {

            vizualizareImprumuturi.Visible = true;

        }

        [Obsolete]
        protected void GvCredite_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.DataItemIndex == -1)
                return;
            e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';");

            e.Row.Attributes.Add("onclick", this.GetPostBackClientEvent(gvCredite, "Select$" + e.Row.RowIndex.ToString()));

        }

        protected void GvCredite_SelectedIndexChanged(object sender, EventArgs e)
        {

            vizualizareImprumuturi.Visible = true;

            int index = gvCredite.SelectedRow.RowIndex;

            Session["index"] = index;

        }

        protected void BtnDetaliiCreditContractat_Click(object sender, EventArgs e)
        {

            detaliiImprumut.Visible = true;

            int index = gvCredite.SelectedRow.RowIndex;

            conn.Open();

            string sql = "SELECT IBAN_Credit, Tip_Credit, Valoare_Credit, Suma_Rambursata, Sold_Credit, FORMAT(Data_Contractare,'dd/MM/yyyy'), FORMAT(Data_Scadenta,'dd/MM/yyyy'), Rata_Descrescatoare, Credit_Activ, IBAN_Cont, ID_Dobanda " +
                "FROM Credite " +
                "WHERE IBAN_Credit = '" + gvCredite.Rows[index].Cells[0].Text + "'";

            SqlCommand cmd = new SqlCommand(sql, conn);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {

                    txtIBANCredit.Text = reader[0].ToString();

                    txtContPlatitorCredit.Text = reader[9].ToString();

                    txtTipCredit.Text = reader[1].ToString();

                    txtValoareCredit.Text = String.Format("{0:n}", reader[2].ToString());

                    txtSumaRambursata.Text = String.Format("{0:n}", reader[3].ToString());

                    txtSumaNerambursata.Text = String.Format("{0:n}", reader[4].ToString());

                    txtDataContractareDetalii.Text = reader[5].ToString();

                    txtDataScadentaDetalii.Text = reader[6].ToString();

                    if (reader[7].ToString() == "False")
                    {

                        txtTipRataDetalii.Text = "Egală";

                    }
                    else
                    {

                        txtTipRataDetalii.Text = "Descrescătoare";

                    }

                    if (reader[10].ToString() == "1")
                    {

                        txtTipDobandaDetalii.Text = "Fixă";

                    }
                    else
                    {

                        txtTipDobandaDetalii.Text = "Variabilă";

                    }

                }

            }

            conn.Close();


        }

        protected void BtnDetaliiCreditRevenire_Click(object sender, EventArgs e)
        {

            detaliiImprumut.Visible = false;

            vizualizareImprumuturi.Visible = true;

        }

        private void Actualizare_Credite()
        {

            double valoareImprumut;

            double sumaImprumut;

            int nrLuni;

            double numarator;

            double numitor;

            double sumaRata;

            double sumaDobanda;

            double sumaPrincipal;

            double soldImprumut;

            double valoareDobanda;

            conn.Open();

            string selectNrCredit = "SELECT COUNT(Nr_Credit) " +
                "FROM Credite";

            SqlCommand cmdNrCredit = new SqlCommand(selectNrCredit, conn);

            SqlDataReader readerNrCredit = cmdNrCredit.ExecuteReader();

            readerNrCredit.Read();

            int nrCredit = Convert.ToInt32(readerNrCredit[0].ToString());

            readerNrCredit.Close();

            for (int i = 1; i <= nrCredit; i++)
            {

                string cmdDateCredit = "SELECT IBAN_Credit, Tip_Credit, Valoare_Credit, Suma_Rambursata, Sold_Credit, FORMAT(Data_Contractare,'dd/MM/yyyy'), FORMAT(Data_Scadenta,'dd/MM/yyyy'), DATEDIFF(month, Data_Contractare, GETDATE()), DATEDIFF(month, Data_Contractare, Data_Scadenta), Rata_Descrescatoare, Credit_Activ, IBAN_Cont, Dobanda_Anuala, FORMAT(Data_Actualizare_Urmatoare,'dd/MM/yyyy') " +
                     "FROM Credite INNER JOIN Dobanda_Credit ON Credite.ID_Dobanda=Dobanda_Credit.ID_Dobanda " +
                     "WHERE Nr_Credit = + " + i + "";

                SqlCommand cmdDate = new SqlCommand(cmdDateCredit, conn);

                SqlDataReader readerDate = cmdDate.ExecuteReader();

                readerDate.Read();

                if (readerDate[10].ToString() == "True")
                {

                    if (DateTime.Parse(readerDate.GetString(6)) >= DateTime.Now.Date)
                    {

                        if (readerDate[9].ToString() == "False")
                        {

                            if (DateTime.Parse(readerDate.GetString(13)) == DateTime.Now.Date)
                            {

                                valoareDobanda = Convert.ToDouble(readerDate[12].ToString());

                                valoareImprumut = Convert.ToDouble(readerDate[2].ToString());

                                sumaImprumut = Convert.ToDouble(readerDate[4].ToString());

                                nrLuni = Convert.ToInt32(readerDate[8].ToString());

                                numarator = valoareImprumut * (valoareDobanda / 100);

                                numitor = 12 * (1 - Math.Pow(1 / (1 + (valoareDobanda / 100) / 12), nrLuni));

                                sumaRata = numarator / numitor;

                                sumaDobanda = (sumaImprumut * (valoareDobanda / 100) * (nrLuni / 12)) / nrLuni;

                                sumaPrincipal = sumaRata - sumaDobanda;

                                soldImprumut = sumaImprumut - sumaPrincipal;

                                string updateCredite = "UPDATE Credite " +
                                    "SET Suma_Rambursata = Suma_Rambursata + " + sumaRata + ", Sold_Credit = + " + soldImprumut + ", Data_Actualizare_Urmatoare = DATEADD(month, 1, Data_Actualizare_Urmatoare) " +
                                    "WHERE Nr_Credit = + " + i + "";

                                SqlCommand cmdUpdateCredite = new SqlCommand(updateCredite, conn);

                                cmdUpdateCredite.ExecuteNonQuery();


                                string selectNrOperatiuneS = "SELECT MAX(Nr_Operatiune) " +
                                    "FROM Operatiuni";

                                SqlCommand cmdNrS = new SqlCommand(selectNrOperatiuneS, conn);

                                SqlDataReader readerNrS = cmdNrS.ExecuteReader();

                                readerNrS.Read();

                                int nrS;

                                if (readerNrS.IsDBNull(0))
                                {

                                    nrS = 1;

                                }
                                else
                                {

                                    nrS = Convert.ToInt32(readerNrS[0].ToString()) + 1;

                                }

                                readerNrS.Close();


                                string insertOperatiunePrincipal = "INSERT INTO Operatiuni(Nr_Operatiune, Suma_Operatiune, Data_Operatiune, Detalii_Operatiune, Tip_Operatiune, IBAN_Cont) " +
                                    "VALUES(@nr_operatiune, @suma_operatiune, @data_operatiune, @detalii_operatiune, @tip_operatiune, @iban_cont)";

                                SqlCommand cmdIOP = new SqlCommand(insertOperatiunePrincipal, conn);

                                cmdIOP.Parameters.AddWithValue("@nr_operatiune", nrS);

                                cmdIOP.Parameters.AddWithValue("@suma_operatiune", "-" + sumaRata);

                                cmdIOP.Parameters.AddWithValue("@data_operatiune", DateTime.Now.Date);

                                cmdIOP.Parameters.AddWithValue("@detalii_operatiune", "PLATA RATA");

                                cmdIOP.Parameters.AddWithValue("@tip_operatiune", "PLATA");

                                cmdIOP.Parameters.AddWithValue("@iban_cont", readerDate[11].ToString());

                                cmdIOP.ExecuteNonQuery();


                                string updateSold = "UPDATE Conturi " +
                                    "SET Sold = Sold - " + sumaRata + " " +
                                    "WHERE IBAN_Cont = + " + i + "";

                                SqlCommand cmdUpdateSold = new SqlCommand(updateSold, conn);

                                cmdUpdateSold.ExecuteNonQuery();

                            }

                        }


                        if (readerDate[9].ToString() == "True")
                        {

                            if (DateTime.Parse(readerDate.GetString(13)) == DateTime.Now.Date)
                            {

                                valoareDobanda = Convert.ToDouble(readerDate[12].ToString());

                                sumaImprumut = Convert.ToDouble(readerDate[4].ToString());

                                valoareImprumut = Convert.ToDouble(readerDate[2].ToString());

                                nrLuni = Convert.ToInt32(readerDate[8].ToString());

                                sumaDobanda = (sumaImprumut * (valoareDobanda / 100) * (nrLuni / 12)) / nrLuni;

                                sumaPrincipal = valoareImprumut / nrLuni;

                                sumaRata = sumaPrincipal + sumaDobanda;

                                soldImprumut = sumaImprumut - sumaPrincipal;

                                string updateCredite = "UPDATE Credite " +
                                    "SET Suma_Rambursata = Suma_Rambursata + " + sumaRata + ", Sold_Credit = + " + soldImprumut + ", Data_Actualizare_Urmatoare = DATEADD(month, 1, Data_Actualizare_Urmatoare) " +
                                    "WHERE Nr_Credit = + " + i + "";
                                SqlCommand cmdUpdateCredite = new SqlCommand(updateCredite, conn);

                                cmdUpdateCredite.ExecuteNonQuery();



                                string selectNrOperatiuneS = "SELECT MAX(Nr_Operatiune) " +
                                    "FROM Operatiuni";

                                SqlCommand cmdNrS = new SqlCommand(selectNrOperatiuneS, conn);

                                SqlDataReader readerNrS = cmdNrS.ExecuteReader();

                                readerNrS.Read();

                                int nrS;

                                if (readerNrS.IsDBNull(0))
                                {

                                    nrS = 1;

                                }
                                else
                                {

                                    nrS = Convert.ToInt32(readerNrS[0].ToString()) + 1;

                                }

                                readerNrS.Close();


                                string insertOperatiunePrincipal = "INSERT INTO Operatiuni(Nr_Operatiune, Suma_Operatiune, Data_Operatiune, Detalii_Operatiune, Tip_Operatiune, IBAN_Cont) " +
                                    "VALUES(@nr_operatiune, @suma_operatiune, @data_operatiune, @detalii_operatiune, @tip_operatiune, @iban_cont)";

                                SqlCommand cmdIOP = new SqlCommand(insertOperatiunePrincipal, conn);

                                cmdIOP.Parameters.AddWithValue("@nr_operatiune", nrS);

                                cmdIOP.Parameters.AddWithValue("@suma_operatiune", "-" + sumaRata);

                                cmdIOP.Parameters.AddWithValue("@data_operatiune", DateTime.Now.Date);

                                cmdIOP.Parameters.AddWithValue("@detalii_operatiune", "PLATA RATA");

                                cmdIOP.Parameters.AddWithValue("@tip_operatiune", "PLATA");

                                cmdIOP.Parameters.AddWithValue("@iban_cont", readerDate[11].ToString());

                                cmdIOP.ExecuteNonQuery();


                                string updateSold = "UPDATE Conturi " +
                                    "SET Sold = Sold - " + sumaRata + " " +
                                    "WHERE IBAN_Cont = + " + i + "";

                                SqlCommand cmdUpdateSold = new SqlCommand(updateSold, conn);

                                cmdUpdateSold.ExecuteNonQuery();

                            }

                        }

                    }
                    else if (DateTime.Parse(readerDate.GetString(6)) < DateTime.Now.Date)
                    {

                        string updateCredite = "UPDATE Credite " +
                                    "SET Credit_Activ = 1 " +
                                    "WHERE Nr_Credit = + " + readerDate[11].ToString() + "";

                        SqlCommand cmdUpdateCredite = new SqlCommand(updateCredite, conn);

                        cmdUpdateCredite.ExecuteNonQuery();

                    }

                }

                readerDate.Close();

            }

            conn.Close();

        }

        private void Statistica()
        {

            int i = 0;

            if (rblOptiuniStatistica.SelectedIndex == 0)
            {

                string dateOperatiuni = "SELECT COUNT(Nr_Operatiune) AS Count, SUBSTRING(Tip_Operatiune,1,(CHARINDEX(' ',Tip_Operatiune + ' ')-1)) " +
                "FROM Operatiuni " +
                "WHERE IBAN_Cont = '" + accountDl.Text + "' " +
                "GROUP BY Tip_Operatiune";

                SqlCommand cmdOperatiuni = new SqlCommand(dateOperatiuni, conn);

                SqlDataAdapter adapterOperatiuni = new SqlDataAdapter(cmdOperatiuni);

                DataTable dtOperatiuni = new DataTable();

                adapterOperatiuni.Fill(dtOperatiuni);

                int count = dtOperatiuni.Rows.Count;

                while (i < count)
                {

                    diagramaStatistici.ChartAreas["chartArea"].BackColor = System.Drawing.Color.Transparent;

                    diagramaStatistici.BackColor = System.Drawing.Color.Transparent;

                    diagramaStatistici.Series["series"].IsValueShownAsLabel = true;

                    diagramaStatistici.Series["series"].IsVisibleInLegend = true;

                    diagramaStatistici.Series["series"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Doughnut;

                    diagramaStatistici.Series["series"].Points.AddXY(dtOperatiuni.Rows[i][1].ToString(), dtOperatiuni.Rows[i][0].ToString());

                    i++;

                }

            }
            else if (rblOptiuniStatistica.SelectedIndex == 1)
            {

                string dateOperatiuni = "SELECT ABS(SUM(Suma_Operatiune)) AS [Suma lunara], DATENAME(m, Data_Operatiune) " +
               "FROM Operatiuni " +
               "WHERE IBAN_Cont = '" + accountDl.Text + "' AND Suma_Operatiune < 0 " +
               "GROUP BY DATENAME(m, Data_Operatiune), MONTH(Data_Operatiune) " +
               "ORDER BY MONTH(Data_Operatiune) ASC";

                SqlCommand cmdOperatiuni = new SqlCommand(dateOperatiuni, conn);

                SqlDataAdapter adapterOperatiuni = new SqlDataAdapter(cmdOperatiuni);

                DataTable dtOperatiuni = new DataTable();

                adapterOperatiuni.Fill(dtOperatiuni);

                int count = dtOperatiuni.Rows.Count;

                while (i < count)
                {

                    string lunaOperatiune = "";

                    if (dtOperatiuni.Rows[i][1].ToString() == "January")
                    {

                        lunaOperatiune = "Ianuarie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "February")
                    {

                        lunaOperatiune = "Ianuarie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "March")
                    {

                        lunaOperatiune = "Martie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "April")
                    {

                        lunaOperatiune = "Aprilie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "May")
                    {

                        lunaOperatiune = "Mai";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "June")
                    {

                        lunaOperatiune = "Iunie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "July")
                    {

                        lunaOperatiune = "Iulie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "August")
                    {

                        lunaOperatiune = "August";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "September")
                    {

                        lunaOperatiune = "Septembrie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "October")
                    {

                        lunaOperatiune = "Octombrie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "November")
                    {

                        lunaOperatiune = "Noiembrie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "December")
                    {

                        lunaOperatiune = "Decembrie";

                    }

                    diagramaStatistici.ChartAreas["chartArea"].BackColor = System.Drawing.Color.Transparent;

                    diagramaStatistici.BackColor = System.Drawing.Color.Transparent;

                    diagramaStatistici.Series["series"].IsValueShownAsLabel = true;

                    diagramaStatistici.Series["series"].IsVisibleInLegend = true;

                    diagramaStatistici.Series["series"].LegendText = "Suma";

                    diagramaStatistici.Series["series"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Column;

                    diagramaStatistici.Series["series"].Points.AddXY(lunaOperatiune, dtOperatiuni.Rows[i][0].ToString());

                    i++;

                }

            }
            else if (rblOptiuniStatistica.SelectedIndex == 2)
            {

                string dateVenit = "SELECT SUM(Suma_Operatiune) AS [Încasări], DATENAME(m, Data_Operatiune) " +
                "FROM Operatiuni " +
                "WHERE IBAN_Cont = '" + accountDl.Text + "' AND Suma_Operatiune > 0 " +
                "GROUP BY DATENAME(m, Data_Operatiune), MONTH(Data_Operatiune) " +
                "ORDER BY MONTH(Data_Operatiune) ASC";

                SqlCommand cmdVenit = new SqlCommand(dateVenit, conn);

                SqlDataAdapter adapterVenit = new SqlDataAdapter(cmdVenit);

                DataTable dtVenit = new DataTable();

                adapterVenit.Fill(dtVenit);


                string dateCheltuieli = "SELECT ABS(SUM(Suma_Operatiune)) AS [Cheltuieli], DATENAME(m, Data_Operatiune) " +
               "FROM Operatiuni " +
               "WHERE IBAN_Cont = '" + accountDl.Text + "' AND Suma_Operatiune < 0 " +
               "GROUP BY DATENAME(m, Data_Operatiune), MONTH(Data_Operatiune) " +
               "ORDER BY MONTH(Data_Operatiune) ASC";

                SqlCommand cmdCheltuieli = new SqlCommand(dateCheltuieli, conn);

                SqlDataAdapter adapterCheltuieli = new SqlDataAdapter(cmdCheltuieli);

                DataTable dtCheltuieli = new DataTable();

                adapterCheltuieli.Fill(dtCheltuieli);

                int countCheltuieli = dtCheltuieli.Rows.Count;

                while (i < countCheltuieli)
                {

                    string lunaOperatiune = "";

                    if (dtCheltuieli.Rows[i][1].ToString() == "January")
                    {

                        lunaOperatiune = "Ianuarie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "February")
                    {

                        lunaOperatiune = "Februarie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "March")
                    {

                        lunaOperatiune = "Martie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "April")
                    {

                        lunaOperatiune = "Aprilie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "May")
                    {

                        lunaOperatiune = "Mai";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "June")
                    {

                        lunaOperatiune = "Iunie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "July")
                    {

                        lunaOperatiune = "Iulie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "August")
                    {

                        lunaOperatiune = "August";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "September")
                    {

                        lunaOperatiune = "Septembrie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "October")
                    {

                        lunaOperatiune = "Octombrie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "November")
                    {

                        lunaOperatiune = "Noiembrie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "December")
                    {

                        lunaOperatiune = "Decembrie";

                    }

                    diagramaVenitCheltuieli.ChartAreas["chartArea"].BackColor = System.Drawing.Color.Transparent;

                    diagramaVenitCheltuieli.BackColor = System.Drawing.Color.Transparent;

                    diagramaVenitCheltuieli.Series["series1"].IsValueShownAsLabel = true;

                    diagramaVenitCheltuieli.Series["series1"].IsVisibleInLegend = true;

                    diagramaVenitCheltuieli.Series["series1"].LegendText = "Încasări";

                    diagramaVenitCheltuieli.Series["series2"].IsValueShownAsLabel = true;

                    diagramaVenitCheltuieli.Series["series2"].IsVisibleInLegend = true;

                    diagramaVenitCheltuieli.Series["series2"].LegendText = "Cheltuieli";

                    diagramaVenitCheltuieli.Series["series1"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.StackedColumn;

                    diagramaVenitCheltuieli.Series["series2"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.StackedColumn;

                    diagramaVenitCheltuieli.Series["series1"].Points.AddXY(lunaOperatiune, dtVenit.Rows[i][0].ToString());

                    diagramaVenitCheltuieli.Series["series2"].Points.AddXY(lunaOperatiune, dtCheltuieli.Rows[i][0].ToString());

                    i++;

                }

            }

        }


        private void Statistica_Perioada()
        {

            int i = 0;

            int nrLuna = dlLunaStatistica.SelectedIndex + 1;

            if (rblOptiuniStatistica.SelectedIndex == 0)
            {

                string dateOperatiuni = "SELECT COUNT(Nr_Operatiune) AS Count, SUBSTRING(Tip_Operatiune,1,(CHARINDEX(' ',Tip_Operatiune + ' ')-1)) " +
                "FROM Operatiuni " +
                "WHERE IBAN_Cont = '" + accountDl.Text + "' AND MONTH(Data_Operatiune) = " + nrLuna + " " +
                "GROUP BY Tip_Operatiune";

                SqlCommand cmdOperatiuni = new SqlCommand(dateOperatiuni, conn);

                SqlDataAdapter adapterOperatiuni = new SqlDataAdapter(cmdOperatiuni);

                DataTable dtOperatiuni = new DataTable();

                adapterOperatiuni.Fill(dtOperatiuni);

                int count = dtOperatiuni.Rows.Count;

                while (i < count)
                {

                    diagramaStatistici.ChartAreas["chartArea"].BackColor = System.Drawing.Color.Transparent;

                    diagramaStatistici.BackColor = System.Drawing.Color.Transparent;

                    diagramaStatistici.Series["series"].IsValueShownAsLabel = true;

                    diagramaStatistici.Series["series"].IsVisibleInLegend = true;

                    diagramaStatistici.Series["series"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Doughnut;

                    diagramaStatistici.Series["series"].Points.AddXY(dtOperatiuni.Rows[i][1].ToString(), dtOperatiuni.Rows[i][0].ToString());

                    i++;

                }

            }
            else if (rblOptiuniStatistica.SelectedIndex == 1)
            {

                string dateOperatiuni = "SELECT ABS(SUM(Suma_Operatiune)) AS [Suma lunara], DATENAME(m, Data_Operatiune) " +
               "FROM Operatiuni " +
               "WHERE IBAN_Cont = '" + accountDl.Text + "' AND Suma_Operatiune < 0 AND MONTH(Data_Operatiune) = " + nrLuna + " " +
               "GROUP BY DATENAME(m, Data_Operatiune), MONTH(Data_Operatiune) " +
               "ORDER BY MONTH(Data_Operatiune) ASC";

                SqlCommand cmdOperatiuni = new SqlCommand(dateOperatiuni, conn);

                SqlDataAdapter adapterOperatiuni = new SqlDataAdapter(cmdOperatiuni);

                DataTable dtOperatiuni = new DataTable();

                adapterOperatiuni.Fill(dtOperatiuni);

                int count = dtOperatiuni.Rows.Count;

                while (i < count)
                {

                    string lunaOperatiune = "";

                    if (dtOperatiuni.Rows[i][1].ToString() == "January")
                    {

                        lunaOperatiune = "Ianuarie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "February")
                    {

                        lunaOperatiune = "Ianuarie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "March")
                    {

                        lunaOperatiune = "Martie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "April")
                    {

                        lunaOperatiune = "Aprilie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "May")
                    {

                        lunaOperatiune = "Mai";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "June")
                    {

                        lunaOperatiune = "Iunie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "July")
                    {

                        lunaOperatiune = "Iulie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "August")
                    {

                        lunaOperatiune = "August";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "September")
                    {

                        lunaOperatiune = "Septembrie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "October")
                    {

                        lunaOperatiune = "Octombrie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "November")
                    {

                        lunaOperatiune = "Noiembrie";

                    }
                    else if (dtOperatiuni.Rows[i][1].ToString() == "December")
                    {

                        lunaOperatiune = "Decembrie";

                    }

                    diagramaStatistici.ChartAreas["chartArea"].BackColor = System.Drawing.Color.Transparent;

                    diagramaStatistici.BackColor = System.Drawing.Color.Transparent;

                    diagramaStatistici.Series["series"].IsValueShownAsLabel = true;

                    diagramaStatistici.Series["series"].IsVisibleInLegend = true;

                    diagramaStatistici.Series["series"].LegendText = "Suma";

                    diagramaStatistici.Series["series"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Column;

                    diagramaStatistici.Series["series"].Points.AddXY(lunaOperatiune, dtOperatiuni.Rows[i][0].ToString());

                    i++;

                }

            }
            else if (rblOptiuniStatistica.SelectedIndex == 2)
            {

                string dateVenit = "SELECT SUM(Suma_Operatiune) AS [Încasări], DATENAME(m, Data_Operatiune) " +
                "FROM Operatiuni " +
                "WHERE IBAN_Cont = '" + accountDl.Text + "' AND Suma_Operatiune > 0 AND MONTH(Data_Operatiune) = " + nrLuna + " " +
                "GROUP BY DATENAME(m, Data_Operatiune), MONTH(Data_Operatiune) " +
                "ORDER BY MONTH(Data_Operatiune) ASC";

                SqlCommand cmdVenit = new SqlCommand(dateVenit, conn);

                SqlDataAdapter adapterVenit = new SqlDataAdapter(cmdVenit);

                DataTable dtVenit = new DataTable();

                adapterVenit.Fill(dtVenit);


                string dateCheltuieli = "SELECT ABS(SUM(Suma_Operatiune)) AS [Cheltuieli], DATENAME(m, Data_Operatiune) " +
               "FROM Operatiuni " +
               "WHERE IBAN_Cont = '" + accountDl.Text + "' AND Suma_Operatiune < 0 AND MONTH(Data_Operatiune) = " + nrLuna + " " +
               "GROUP BY DATENAME(m, Data_Operatiune), MONTH(Data_Operatiune) " +
               "ORDER BY MONTH(Data_Operatiune) ASC";

                SqlCommand cmdCheltuieli = new SqlCommand(dateCheltuieli, conn);

                SqlDataAdapter adapterCheltuieli = new SqlDataAdapter(cmdCheltuieli);

                DataTable dtCheltuieli = new DataTable();

                adapterCheltuieli.Fill(dtCheltuieli);

                int countCheltuieli = dtCheltuieli.Rows.Count;

                while (i < countCheltuieli)
                {

                    string lunaOperatiune = "";

                    if (dtCheltuieli.Rows[i][1].ToString() == "January")
                    {

                        lunaOperatiune = "Ianuarie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "February")
                    {

                        lunaOperatiune = "Februarie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "March")
                    {

                        lunaOperatiune = "Martie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "April")
                    {

                        lunaOperatiune = "Aprilie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "May")
                    {

                        lunaOperatiune = "Mai";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "June")
                    {

                        lunaOperatiune = "Iunie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "July")
                    {

                        lunaOperatiune = "Iulie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "August")
                    {

                        lunaOperatiune = "August";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "September")
                    {

                        lunaOperatiune = "Septembrie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "October")
                    {

                        lunaOperatiune = "Octombrie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "November")
                    {

                        lunaOperatiune = "Noiembrie";

                    }
                    else if (dtCheltuieli.Rows[i][1].ToString() == "December")
                    {

                        lunaOperatiune = "Decembrie";

                    }

                    diagramaVenitCheltuieli.ChartAreas["chartArea"].BackColor = System.Drawing.Color.Transparent;

                    diagramaVenitCheltuieli.BackColor = System.Drawing.Color.Transparent;

                    diagramaVenitCheltuieli.Series["series1"].IsValueShownAsLabel = true;

                    diagramaVenitCheltuieli.Series["series1"].IsVisibleInLegend = true;

                    diagramaVenitCheltuieli.Series["series1"].LegendText = "Încasări";

                    diagramaVenitCheltuieli.Series["series2"].IsValueShownAsLabel = true;

                    diagramaVenitCheltuieli.Series["series2"].IsVisibleInLegend = true;

                    diagramaVenitCheltuieli.Series["series2"].LegendText = "Cheltuieli";

                    diagramaVenitCheltuieli.Series["series1"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.StackedColumn;

                    diagramaVenitCheltuieli.Series["series2"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.StackedColumn;

                    diagramaVenitCheltuieli.Series["series1"].Points.AddXY(lunaOperatiune, dtVenit.Rows[i][0].ToString());
                     
                    diagramaVenitCheltuieli.Series["series2"].Points.AddXY(lunaOperatiune, dtCheltuieli.Rows[i][0].ToString());

                    i++;

                }

            }

        }


        protected void SixthBtn_Click(object sender, EventArgs e)
        {

            statisticiDate.Visible = true;

            txtAnStatistica.Visible = false;

            dlLunaStatistica.Visible = false;

            diagramaVenitCheltuieli.Visible = false;

            if (rblOptiuniStatistica.Items.Count == 0)
            {

                rblOptiuniStatistica.Items.Add("Total tranzacții");

                rblOptiuniStatistica.Items.Add("Total cheltuieli");

                rblOptiuniStatistica.Items.Add("Cheltuieli VS Încasări");

                rblOptiuniStatistica.RepeatDirection = RepeatDirection.Horizontal;

            }

            rblOptiuniStatistica.SelectedIndex = 0;

            Statistica();

            foreach (System.Web.UI.WebControls.ListItem li in rblOptiuniStatistica.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "15px");

            }

            if (rblPerioadaStatistica.Items.Count == 0)
            {

                rblPerioadaStatistica.Items.Add("Nedeterminată");

                rblPerioadaStatistica.Items.Add("Determinată");

                rblPerioadaStatistica.RepeatDirection = RepeatDirection.Horizontal;

            }

            rblPerioadaStatistica.SelectedIndex = 0;

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaStatistica.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "15px");

            }

            if (dlLunaStatistica.Items.Count == 0)
            {

                if (DateTime.Now.Month == 1)
                {

                    dlLunaStatistica.Items.Add("Ianuarie");

                }
                else if (DateTime.Now.Month == 2)
                {

                    dlLunaStatistica.Items.Add("Ianuarie");
                    dlLunaStatistica.Items.Add("Februarie");

                }
                else if (DateTime.Now.Month == 3)
                {

                    dlLunaStatistica.Items.Add("Ianuarie");
                    dlLunaStatistica.Items.Add("Februarie");
                    dlLunaStatistica.Items.Add("Martie");

                }
                else if (DateTime.Now.Month == 4)
                {

                    dlLunaStatistica.Items.Add("Ianuarie");
                    dlLunaStatistica.Items.Add("Februarie");
                    dlLunaStatistica.Items.Add("Martie");
                    dlLunaStatistica.Items.Add("Aprilie");

                }
                else if (DateTime.Now.Month == 5)
                {

                    dlLunaStatistica.Items.Add("Ianuarie");
                    dlLunaStatistica.Items.Add("Februarie");
                    dlLunaStatistica.Items.Add("Martie");
                    dlLunaStatistica.Items.Add("Aprilie");
                    dlLunaStatistica.Items.Add("Mai");

                }
                else if (DateTime.Now.Month == 6)
                {

                    dlLunaStatistica.Items.Add("Ianuarie");
                    dlLunaStatistica.Items.Add("Februarie");
                    dlLunaStatistica.Items.Add("Martie");
                    dlLunaStatistica.Items.Add("Aprilie");
                    dlLunaStatistica.Items.Add("Mai");
                    dlLunaStatistica.Items.Add("Iunie");

                }
                else if (DateTime.Now.Month == 7)
                {

                    dlLunaStatistica.Items.Add("Ianuarie");
                    dlLunaStatistica.Items.Add("Februarie");
                    dlLunaStatistica.Items.Add("Martie");
                    dlLunaStatistica.Items.Add("Aprilie");
                    dlLunaStatistica.Items.Add("Mai");
                    dlLunaStatistica.Items.Add("Iunie");
                    dlLunaStatistica.Items.Add("Iulie");

                }
                else if (DateTime.Now.Month == 8)
                {

                    dlLunaStatistica.Items.Add("Ianuarie");
                    dlLunaStatistica.Items.Add("Februarie");
                    dlLunaStatistica.Items.Add("Martie");
                    dlLunaStatistica.Items.Add("Aprilie");
                    dlLunaStatistica.Items.Add("Mai");
                    dlLunaStatistica.Items.Add("Iunie");
                    dlLunaStatistica.Items.Add("Iulie");
                    dlLunaStatistica.Items.Add("August");

                }
                else if (DateTime.Now.Month == 9)
                {

                    dlLunaStatistica.Items.Add("Ianuarie");
                    dlLunaStatistica.Items.Add("Februarie");
                    dlLunaStatistica.Items.Add("Martie");
                    dlLunaStatistica.Items.Add("Aprilie");
                    dlLunaStatistica.Items.Add("Mai");
                    dlLunaStatistica.Items.Add("Iunie");
                    dlLunaStatistica.Items.Add("Iulie");
                    dlLunaStatistica.Items.Add("August");
                    dlLunaStatistica.Items.Add("Septembrie");

                }
                else if (DateTime.Now.Month == 10)
                {

                    dlLunaStatistica.Items.Add("Ianuarie");
                    dlLunaStatistica.Items.Add("Februarie");
                    dlLunaStatistica.Items.Add("Martie");
                    dlLunaStatistica.Items.Add("Aprilie");
                    dlLunaStatistica.Items.Add("Mai");
                    dlLunaStatistica.Items.Add("Iunie");
                    dlLunaStatistica.Items.Add("Iulie");
                    dlLunaStatistica.Items.Add("August");
                    dlLunaStatistica.Items.Add("Septembrie");
                    dlLunaStatistica.Items.Add("Octombrie");

                }
                else if (DateTime.Now.Month == 11)
                {

                    dlLunaStatistica.Items.Add("Ianuarie");
                    dlLunaStatistica.Items.Add("Februarie");
                    dlLunaStatistica.Items.Add("Martie");
                    dlLunaStatistica.Items.Add("Aprilie");
                    dlLunaStatistica.Items.Add("Mai");
                    dlLunaStatistica.Items.Add("Iunie");
                    dlLunaStatistica.Items.Add("Iulie");
                    dlLunaStatistica.Items.Add("August");
                    dlLunaStatistica.Items.Add("Septembrie");
                    dlLunaStatistica.Items.Add("Octombrie");
                    dlLunaStatistica.Items.Add("Noiembrie");

                }
                else if (DateTime.Now.Month == 12)
                {

                    dlLunaStatistica.Items.Add("Ianuarie");
                    dlLunaStatistica.Items.Add("Februarie");
                    dlLunaStatistica.Items.Add("Martie");
                    dlLunaStatistica.Items.Add("Aprilie");
                    dlLunaStatistica.Items.Add("Mai");
                    dlLunaStatistica.Items.Add("Iunie");
                    dlLunaStatistica.Items.Add("Iulie");
                    dlLunaStatistica.Items.Add("August");
                    dlLunaStatistica.Items.Add("Septembrie");
                    dlLunaStatistica.Items.Add("Octombrie");
                    dlLunaStatistica.Items.Add("Noiembrie");
                    dlLunaStatistica.Items.Add("Decembrie");

                }

            }

        }

        protected void RblPerioadaStatistica_SelectedIndexChanged(object sender, EventArgs e)
        {

            statisticiDate.Visible = true;

            diagramaVenitCheltuieli.Visible = false;

            if (rblOptiuniStatistica.SelectedIndex == 0 || rblOptiuniStatistica.SelectedIndex == 1)
            {

                if (rblPerioadaStatistica.SelectedIndex == 1)
                {

                    txtAnStatistica.Visible = true;

                    dlLunaStatistica.Visible = true;

                    dlLunaStatistica.SelectedIndex = dlLunaStatistica.Items.Count - 1;

                    Statistica_Perioada();

                }
                else if (rblPerioadaStatistica.SelectedIndex == 0)
                {

                    txtAnStatistica.Visible = false;

                    dlLunaStatistica.Visible = false;

                    diagramaStatistici.Visible = true;

                    Statistica();

                }

            }
            else if (rblOptiuniStatistica.SelectedIndex == 2)
            {

                if (rblPerioadaStatistica.SelectedIndex == 1)
                {

                    txtAnStatistica.Visible = true;

                    dlLunaStatistica.Visible = true;

                    diagramaStatistici.Visible = false;

                    diagramaVenitCheltuieli.Visible = true;

                    dlLunaStatistica.SelectedIndex = dlLunaStatistica.Items.Count - 1;

                    Statistica_Perioada();

                }
                else if (rblPerioadaStatistica.SelectedIndex == 0)
                {

                    txtAnStatistica.Visible = false;

                    dlLunaStatistica.Visible = false;

                    diagramaStatistici.Visible = false;

                    diagramaVenitCheltuieli.Visible = true;

                    Statistica();

                }
            }

            foreach (System.Web.UI.WebControls.ListItem li in rblOptiuniStatistica.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "15px");

            }

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaStatistica.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "15px");

            }

        }

        protected void DlLunaStatistica_SelectedIndexChanged(object sender, EventArgs e)
        {

            statisticiDate.Visible = true;

            Statistica_Perioada();

            if (rblOptiuniStatistica.SelectedIndex == 2)
            {

                diagramaVenitCheltuieli.Visible = true;

                diagramaStatistici.Visible = false;

            }
            else
            {

                diagramaVenitCheltuieli.Visible = false;

                diagramaStatistici.Visible = true;

            }

            foreach (System.Web.UI.WebControls.ListItem li in rblOptiuniStatistica.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "15px");

            }

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaStatistica.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "15px");

            }

        }

        protected void RblOptiuniStatistica_SelectedIndexChanged(object sender, EventArgs e)
        {

            statisticiDate.Visible = true;

            diagramaVenitCheltuieli.Visible = false;


            if (rblOptiuniStatistica.SelectedIndex == 0 || rblOptiuniStatistica.SelectedIndex == 1)
            {

                if (rblPerioadaStatistica.SelectedIndex == 1)
                {

                    txtAnStatistica.Visible = true;

                    dlLunaStatistica.Visible = true;

                    dlLunaStatistica.SelectedIndex = dlLunaStatistica.Items.Count - 1;

                    Statistica_Perioada();

                }
                else if (rblPerioadaStatistica.SelectedIndex == 0)
                {

                    txtAnStatistica.Visible = false;

                    dlLunaStatistica.Visible = false;

                    diagramaStatistici.Visible = true;

                    Statistica();

                }

            }
            else if (rblOptiuniStatistica.SelectedIndex == 2)
            {

                if (rblPerioadaStatistica.SelectedIndex == 1)
                {

                    txtAnStatistica.Visible = true;

                    dlLunaStatistica.Visible = true;

                    diagramaStatistici.Visible = false;

                    diagramaVenitCheltuieli.Visible = true;

                    dlLunaStatistica.SelectedIndex = dlLunaStatistica.Items.Count - 1;

                    Statistica_Perioada();

                }
                else if (rblPerioadaStatistica.SelectedIndex == 0)
                {

                    txtAnStatistica.Visible = false;

                    dlLunaStatistica.Visible = false;

                    diagramaStatistici.Visible = false;

                    diagramaVenitCheltuieli.Visible = true;

                    Statistica();

                }

            }

            foreach (System.Web.UI.WebControls.ListItem li in rblOptiuniStatistica.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "15px");

            }

            foreach (System.Web.UI.WebControls.ListItem li in rblPerioadaStatistica.Items)
            {

                li.Attributes.CssStyle.Add("margin-right", "15px");

            }

        }

        protected void BtnConturiActiuneRapida_Click(object sender, EventArgs e)
        {

            dataContainer.Visible = true;

            Session["index"] = -1;

            btnCopiere.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnCopiere.ForeColor = System.Drawing.Color.White;


            btnCopiereEconomii.BackColor = System.Drawing.Color.FromArgb(113, 131, 140);

            btnCopiereEconomii.ForeColor = System.Drawing.Color.White;


            gvConturi.SelectedIndex = -1;

            gvConturiEconomii.SelectedIndex = -1;

        }

        protected void BtnDeschideContActiuneRapida_Click(object sender, EventArgs e)
        {

            deschideCont.Visible = true;

            if (dlTipContNou.Items.Count == 0)
            {

                dlTipContNou.Items.Add("CONT CURENT");

                dlTipContNou.Items.Add("CONT ECONOMII");

            }

            if (dlValutaContNou.Items.Count == 0)
            {

                dlValutaContNou.Items.Add("RON");

                dlValutaContNou.Items.Add("EURO");

            }

            dlValutaContNou.SelectedIndex = 0;

            dlTipContNou.SelectedIndex = 0;

            lblEroareContNou.Text = "";

            cbAcordContNou.Checked = false;

        }

        protected void BtnCarduriActiuneRapida_Click(object sender, EventArgs e)
        {

            cardurileMele.Visible = true;

            gvCarduriDebit.SelectedIndex = -1;

            gvCarduriCredit.SelectedIndex = -1;

            cldInceputInternet.SelectedDate = DateTime.Now;

            cldInceputNumerar.SelectedDate = DateTime.Now;

            cldInceputPOS.SelectedDate = DateTime.Now;

            cldInceputTranzactii.SelectedDate = DateTime.Now;

            cldSfarsitInternet.SelectedDate = DateTime.Now;

            cldSfarsitNumerar.SelectedDate = DateTime.Now;

            cldSfarsitTranzactii.SelectedDate = DateTime.Now;

            cldSfarsitPOS.SelectedDate = DateTime.Now;

        }

        protected void BtnPlataActiuneRapida_Click(object sender, EventArgs e)
        {

            platiCont.Visible = true;

            txtPlataDinContul.Text = accountDl.Text;

            txtNumeBeneficiar.Text = "";

            txtContBeneficiar.Text = "";

            txtSumaPlataDinCont.Text = "";

            txtDetaliiPlata.Text = "";

            lblEroarePlataCont.Visible = false;

            string valuta = txtPlataDinContul.Text.Substring(8, 3);

            string codClient = (string)Session["codClient"];

            conn.Open();

            string selectBeneficiariPredefiniti = "SELECT * " +
               "FROM Beneficiari_Predefiniti " +
               "WHERE Cod_Client = '" + codClient + "' AND SUBSTRING(IBAN_Beneficiar, 9, 3) = '" + valuta + "'";

            SqlCommand cmdBeneficiariPredefiniti = new SqlCommand(selectBeneficiariPredefiniti, conn);

            SqlDataAdapter adapterBeneficiariPredefiniti = new SqlDataAdapter(cmdBeneficiariPredefiniti);

            DataTable dtBeneficiariPredefiniti = new DataTable();

            adapterBeneficiariPredefiniti.Fill(dtBeneficiariPredefiniti);

            dlBeneficiarPredefinit.DataSource = dtBeneficiariPredefiniti;

            dlBeneficiarPredefinit.DataTextField = "Denumire_Beneficiar";

            dlBeneficiarPredefinit.DataBind();

            dlBeneficiarPredefinit.Items.Insert(0, "");

            conn.Close();

            dlBeneficiarPredefinit.SelectedIndex = 0;

        }

        protected void BtnConversieActiuneRapida_Click(object sender, EventArgs e)
        {

            conversieBani.Visible = true;

            dlContPlataConversie.Items.Clear();

            dlContDestinatarConversie.Items.Clear();

            dlContDestinatarConversie.Style.Clear();

            dlContPlataConversie.Style.Clear();

            txtSumaConversie.Text = "";

            txtSumaDupaConversie.Text = "";

            lblEroareConversie.Visible = false;

            if (rblListaTipConversie.Items.Count == 0)
            {

                rblListaTipConversie.Items.Add("Cumpărare (Valută vs RON)");

                rblListaTipConversie.Items.Add("Vânzare (Valută vs RON)");

                rblListaTipConversie.Items.Add("Cumpărare (Valută vs Valută)");

                rblListaTipConversie.Items.Add("Vânzare (Valută vs Valută)");

            }

            rblListaTipConversie.SelectedIndex = 0;


            conn.Open();

            string valuta = accountDl.Text.Substring(8, 3);

            string codClient = (string)Session["codClient"];

            dlContPlataConversie.Style.Add("pointer-events", "none");

            if (valuta != "RON")
            {

                dlContPlataConversie.Items.Add("SELECTAȚI CONT RON");

                dlContDestinatarConversie.Style.Add("pointer-events", "none");

                txtSumaConversie.Style.Add("pointer-events", "none");

                txtSumaConversie.BackColor = System.Drawing.Color.WhiteSmoke;

                dlContDestinatarConversie.BackColor = System.Drawing.Color.WhiteSmoke;

            }
            else
            {

                dlContPlataConversie.Items.Add(accountDl.Text);

                dlContDestinatarConversie.BackColor = System.Drawing.Color.White;

                dlContPlataConversie.BackColor = System.Drawing.Color.WhiteSmoke;

                txtSumaConversie.Style.Add("pointer-events", "all");

                txtSumaConversie.BackColor = System.Drawing.Color.White;


                string selectContDestinatar = "SELECT * " +
                "FROM Conturi " +
                "WHERE Cod_Client = '" + codClient + "' AND SUBSTRING(IBAN_Cont, 9, 3) <> '" + valuta + "' AND IBAN_Cont <> '" + txtTransferDinContul.Text + "'";

                SqlCommand cmdContDestinatar = new SqlCommand(selectContDestinatar, conn);

                SqlDataAdapter adapterContDestinatar = new SqlDataAdapter(cmdContDestinatar);

                DataTable dtContDestinatar = new DataTable();

                adapterContDestinatar.Fill(dtContDestinatar);

                dlContDestinatarConversie.DataSource = dtContDestinatar;

                dlContDestinatarConversie.DataTextField = "IBAN_Cont";

                dlContDestinatarConversie.DataBind();

                dlContDestinatarConversie.Style.Add("pointer-events", "all");

            }

            conn.Close();

            Citire_Xml();

        }
    }

}