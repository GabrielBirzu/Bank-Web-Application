 <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ROBANK.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ROBANK</title>

    <link href="../Styles/Style.css" rel="stylesheet" />

</head>
<body>

    <div class="background">
        <div class="divLogo" draggable="false">
            <img class="imgLogo" src="../Media/Login/logo.png" draggable="false" />
            <p class="mesajIntampinare" draggable="false">Bine ati venit!</p>
            <form id="formLogare" runat="server">
                <asp:Label ID="wrongLbl" runat="server"></asp:Label>
                <asp:TextBox ID="idLogare" name="text" PlaceHolder="ID Logare" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:TextBox ID="parolaLogare" PlaceHolder="Parola" type="password" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:HyperLink ID="hlRecuperareParola" runat="server" NavigateUrl="~/Pages/Password.aspx" Text="Ai uitat parola?"></asp:HyperLink>
                <br />
                <asp:Button ID="butonLogare" runat="server" Text="LOGIN" OnClick="butonLogare_Click" />
                <br />
                <br />
            </form>
        </div>
        <div class="divCopyright">
            <p class="copyright" draggable="false">© Copyright 2022 Banca Robank. Toate drepturile sunt rezervate.</p>
        </div> 
    </div>

    <script src="../JavaScript/preventResubmission.js"></script>

</body>
</html>
