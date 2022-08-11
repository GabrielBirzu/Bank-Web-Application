<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewPassword.aspx.cs" Inherits="ROBANK.Pages.WebForm3" %>

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
                <asp:TextBox ID="txtCodEmail" name="text" PlaceHolder="Cod" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:TextBox ID="txtParolaNoua" name="text" type="password" PlaceHolder="Introduceți parola nouă" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:TextBox ID="txtVerificareParolaNoua" name="text" type="password" PlaceHolder="Reintroduceți parola nouă" runat="server"></asp:TextBox>
                <br />
                <br />

                <asp:Label ID="lblEroareDateParolaNoua" runat="server" Text="Introduceți toate datele!"></asp:Label>

                <asp:Button ID="btnFinalizareSchimbareParola" runat="server" Text="Finalizare" OnClick="BtnFinalizareSchimbareParola_Click"/>

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
