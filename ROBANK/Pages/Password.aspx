<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Password.aspx.cs" Inherits="ROBANK.Pages.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ROBANK</title>

    <link href="../Styles/Style.css" rel="stylesheet" />

</head>
<body>

    <div class="background_password">
        <div class="divLogo" draggable="false">
            <img class="imgLogo" src="../Media/Login/logo.png" draggable="false" />
            <form id="formLogare" runat="server">
                <asp:Label ID="wrongLbl" runat="server"></asp:Label>
                <asp:TextBox ID="txtNume" name="text" PlaceHolder="Nume" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:TextBox ID="txtPrenume" name="text" PlaceHolder="Prenume" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:TextBox ID="txtCNP" name="text" PlaceHolder="CNP" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:Image ID="imgCodSecuritate" runat="server" />
                <br />
                <asp:Button ID="btnGenerareCod" runat="server" Text="Generează cod nou" OnClick="BtnGenerareCod_Click" />
                <br />
                <asp:TextBox ID="txtCodSecuritate" PlaceHolder="Cod securitate"  runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:Label ID="lblEroareDate" runat="server" Text="Date incorecte!"></asp:Label>

                <asp:Button ID="btnRevenire" runat="server" Text="Revenire" OnClick="BtnRevenire_Click"/>

                <asp:Button ID="btnContinuare" runat="server" Text="Continuare" OnClick="BtnContinuare_Click" />

            </form>
        </div>
        <div class="divCopyright">
            <p class="copyright" draggable="false">© Copyright 2022 Banca Robank. Toate drepturile sunt rezervate.</p>
        </div> 
    </div>

    <script src="../JavaScript/Profil.js"></script>

    <script src="../JavaScript/preventResubmission.js"></script>

</body>
</html>
