 <%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Menu.aspx.cs" Inherits="ROBANK.Pages.WebForm2" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title></title>

    <link href="../Styles/Style.css" rel="stylesheet" />

    <link href="../Styles/SecondStyle.css" rel="stylesheet" />
    
    <link href="../Styles/StergereDepozitStyle.css" rel="stylesheet" />

</head>

<body>

    <div class="second_background">

        <form runat="server">

            <div class="first_container">

                <div class="profilePhoto">

                    <asp:Image ID="profileImg" runat="server" Height="100%" Width="100%" Visible="false" />
                    <input type="file" id="file" runat="server" />
                    <label for="file" id="chooseBtn" runat="server">Alege Poză</label>
                    <img id="photo" src="~/Media/Second_Page/profil.png" runat="server" />

                </div>

                <asp:DropDownList ID="accountDl" runat="server" AutoPostBack="true"></asp:DropDownList>

                <asp:Button ID="saveBtn" runat="server" Text="Salvare" OnClick="SaveBtn_Click" />

                <asp:Label ID="nameLbl" runat="server"></asp:Label>

                <div class="maineMenu">

                    <ul id="navMenu">

                        <li><asp:Button ID="firstBtn" runat="server" Text="CONTURI"/>
                            <ul class="nav1">
                                <li><asp:Button ID="firstBtn1" runat="server" Text="Vizualizare rapidă" OnClick="FirstBtn1_Click" /></li>
                                <li><asp:Button ID="firstBtn2" runat="server" Text="Deschidere cont" OnClick="FirstBtn2_Click" /></li>
                                <li><asp:Button ID="firstBtn3" runat="server" Text="Istoric tranzactii" OnClick="FirstBtn3_Click" /></li>
                            </ul>
                            
                        </li>

                        <li><asp:Button ID="secondBtn" runat="server" Text="DEPOZITE"/>
                            <ul class="nav2">
                                <li><asp:Button ID="secondBtn1" runat="server" Text="Depozitele mele" OnClick="SecondBtn1_Click" /></li>
                                <li><asp:Button ID="secondBtn2" runat="server" Text="Deschide depozit" OnClick="SecondBtn2_Click" /></li>
                            </ul>
                            
                        </li>

                        <li><asp:Button ID="thirdBtn" runat="server" Text="TRANSFERURI"/>
                            <ul class="nav3">
                                <li><asp:Button ID="thirdBtn1" runat="server" Text="Transfer conturi proprii" OnClick="ThirdBtn1_Click" /></li>
                                <li><asp:Button ID="thirdBtn2" runat="server" Text="Plăți" OnClick="ThirdBtn2_Click" /></li>
                                <li><asp:Button ID="thirdBtn3" runat="server" Text="Schimb valutar" OnClick="ThirdBtn3_Click" /></li>
                                <li><asp:Button ID="thirdBtn4" runat="server" Text="Plăți programate" OnClick="ThirdBtn4_Click" /></li>
                                <li><asp:Button ID="thirdBtn5" runat="server" Text="Beneficiari predefiniți" OnClick="ThirdBtn5_Click" /></li>
                            </ul>
                            
                        </li>

                        <li><asp:Button ID="fourthBtn" runat="server" Text="CARDURILE MELE"/>                      
                            <ul class="nav4">
                                <li><asp:Button ID="fourthBtn1" runat="server" Text="Vizualizare carduri" OnClick="FourthBtn1_Click"/></li>
                                <li><asp:Button ID="fourthBtn2" runat="server" Text="Istoric tranzacții" OnClick="FourthBtn2_Click" /></li>
                            </ul>
                            
                        </li>

                        <li><asp:Button ID="fifthBtn" runat="server" Text="CREDITE"/>                      
                            <ul class="nav5">
                                <li><asp:Button ID="fifthBtn1" runat="server" Text="Vizualizare credite" OnClick="FifthBtn1_Click"/></li>
                                <li><asp:Button ID="fifthBtn2" runat="server" Text="Contractare credit" OnClick="FifthBtn2_Click" /></li>
                            </ul>
                            
                        </li>

                        <li><asp:Button ID="sixthBtn" runat="server" Text="STATISTICI" OnClick="SixthBtn_Click"/>                      
                                                        
                        </li>

                    </ul>

                </div>

                <div class="second_container">

                    <div class="personal_details">

                        <asp:Image ID="imgDateCont" ImageUrl="~/Media/Second_Page/background_date.png" runat="server" />

                        <asp:TextBox ID="txtSold" runat="server"></asp:TextBox>

                        <asp:TextBox ID="txtValuta" runat="server"></asp:TextBox>

                        <asp:Button ID="btnConturiActiuneRapida" runat="server" OnClick="BtnConturiActiuneRapida_Click"/>

                        <asp:Label ID="lblConturiActiuneRapida" runat="server" Text="Conturile mele"></asp:Label>

                        <asp:Button ID="btnDeschideContActiuneRapida" runat="server" OnClick="BtnDeschideContActiuneRapida_Click"/>

                        <asp:Label ID="lblDeschideContActiuneRapida" runat="server" Text="Deschide cont"></asp:Label>

                        <asp:Button ID="btnCarduriActiuneRapida" runat="server" OnClick="BtnCarduriActiuneRapida_Click"/>

                        <asp:Label ID="lblCarduriActiuneRapida" runat="server" Text="Cardurile mele"></asp:Label>

                        <asp:Button ID="btnPlataActiuneRapida" runat="server" OnClick="BtnPlataActiuneRapida_Click"/>

                        <asp:Label ID="lblPlataActiuneRapida" runat="server" Text="Efectuare plată"></asp:Label>

                        <asp:Button ID="btnConversieActiuneRapida" runat="server" OnClick="BtnConversieActiuneRapida_Click"/>

                        <asp:Label ID="lblConversieActiuneRapida" runat="server" Text="Conversie"></asp:Label>

                    </div>

                    <div class ="data_container" id="dataContainer" runat="server">

                        <asp:Label ID="lblTitlu" runat="server" Text="Vizualizare rapidă"></asp:Label>

                        <asp:Label ID="lblSubtitluConturi" runat="server" Text="CONTURI CURENTE"></asp:Label>

                        <asp:GridView ID="gvConturi" runat="server" HeaderStyle-BackColor="#71838C" GridLines="None" BackColor="#EBF1F4" HeaderStyle-Height="25" HeaderStyle-ForeColor="White"  SelectedRowStyle-BackColor="#BFCFDB" OnSelectedIndexChanged="GvConturi_SelectedIndexChanged" OnRowDataBound="GvConturi_RowDataBound" />

                        <asp:Button ID="btnCopiere" runat="server" Text="Copiază IBAN" OnClick="BtnCopiere_Click" />

                        <asp:Label ID="lblSubtitluEconomii" runat="server" Text="CONTURI DE ECONOMII"></asp:Label>

                        <asp:GridView ID="gvConturiEconomii" runat="server" HeaderStyle-BackColor="#71838C" GridLines="None" BackColor="#EBF1F4" HeaderStyle-Height="25" HeaderStyle-ForeColor="White"  SelectedRowStyle-BackColor="#BFCFDB" OnSelectedIndexChanged="GvConturiEconomii_SelectedIndexChanged" OnRowDataBound="GvConturiEconomii_RowDataBound" />

                        <asp:Button ID="btnDetaliiEconomii" runat="server" Text="Detalii cont" OnClick="BtnDetaliiEconomii_Click"/>

                        <asp:Button ID="btnCopiereEconomii" runat="server" Text="Copiază IBAN" OnClick="BtnCopiereEconomii_Click" />

                        <asp:Button ID="btnDesfiintareEconomii" runat="server" Text="Desființare" OnClick="BtnDesfiintareEconomii_Click" />

                    </div>

                    <div class ="data_container" id="detaliiEconomii" runat="server">

                        <asp:Label ID="lblEconomii" runat="server" Text="Contul meu de economii"></asp:Label>

                        <asp:Label ID="lblDataCreare" CssClass="nextTo" runat="server" Text="Dată creare:"></asp:Label>

                        <asp:TextBox ID="txtDataCreare" runat="server"></asp:TextBox>

                        <asp:Label ID="lblSoldEconomii" CssClass="nextTo" runat="server" Text="Sold:"></asp:Label>

                        <asp:TextBox ID="txtSoldEconomii" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblValutaEconomii" CssClass="nextTo" runat="server" Text="Valută:"></asp:Label>

                        <asp:TextBox ID="txtValutaEconomii" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblDobandaEconomii" CssClass="nextTo" runat="server" Text="Rata dobânzii:"></asp:Label>

                        <asp:TextBox ID="txtDobandaEconomii" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblSumaDobandaEconomii" CssClass="nextTo" runat="server" Text="Sumă încasată din dobândă:"></asp:Label>

                        <asp:TextBox ID="txtSumaDobandaEconomii" runat="server"></asp:TextBox>

                        <asp:Button ID="btnEconomiiRevenire" runat="server" Text="Înapoi" OnClick="BtnEconomiiRevenire_Click"/>

                        <asp:Image ID="imgBackgroundDetaliiEconomii" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                    </div>

                    <div class="data_container" id="dateDepozite" runat="server">

                        <asp:Label ID="lblTitluDepozite" runat="server" Text="Vizualizare depozite"></asp:Label>

                        <asp:Label ID="lblSubtitluDepozite" runat="server" Text="DEPOZITE ACTIVE"></asp:Label>

                        <asp:GridView ID="gvDepozite" runat="server" HeaderStyle-BackColor="#71838C" GridLines="None" BackColor="#EBF1F4" HeaderStyle-Height="25" HeaderStyle-ForeColor="White"  SelectedRowStyle-BackColor="#BFCFDB" OnSelectedIndexChanged="GvDepozite_SelectedIndexChanged" OnRowDataBound="GvDepozite_RowDataBound"/>

                        <asp:Label ID="lblSubtitluDepoziteInactive" runat="server" Text="DEPOZITE VIITOARE"></asp:Label>

                        <asp:GridView ID="gvDepoziteInactive" runat="server" HeaderStyle-BackColor="#71838C" GridLines="None" BackColor="#EBF1F4" HeaderStyle-Height="25" HeaderStyle-ForeColor="White"  SelectedRowStyle-BackColor="#BFCFDB"/>

                        <asp:Button ID="btnDetaliiDepozit" runat="server" Text="Detalii depozit" OnClick="BtnDetaliiDepozit_Click"/>
                        
                        <asp:Button ID="btnStergereDepozit" runat="server" Text="Desființare" OnClick="BtnStergereDepozit_Click"/>

                    </div>

                    <div class="data_container" id="detaliiDepozite" runat="server">

                        <asp:Label ID="lblDepozitulMeu" runat="server" Text="Depozitul meu"></asp:Label>

                        <asp:Label ID="lblPerioadaDepozit" CssClass="nextTo" runat="server" Text="Perioadă:"></asp:Label>

                        <asp:TextBox ID="txtPerioadaDepozit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblSoldDepozit" CssClass="nextTo" runat="server" Text="Sold:"></asp:Label>

                        <asp:TextBox ID="txtSoldDepozit" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblValutaDepozit" CssClass="nextTo" runat="server" Text="Valută:"></asp:Label>

                        <asp:TextBox ID="txtValutaDepozit" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblRataDobanda" CssClass="nextTo" runat="server" Text="Rata dobânzii:"></asp:Label>

                        <asp:TextBox ID="txtRataDobanda" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblSumaDobanda" CssClass="nextTo" runat="server" Text="Rata dobânzii la zi (sumă):"></asp:Label>

                        <asp:TextBox ID="txtSumaDobanda" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblDataScadentaDepozit" CssClass="nextTo" runat="server" Text="Dată scadență:"></asp:Label>

                        <asp:TextBox ID="txtDataScadentaDepozit" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblDataPrelungireDepozit" CssClass="nextTo" runat="server" Text="Data ultimei prelungiri:"></asp:Label>

                        <asp:TextBox ID="txtDataPrelungireDepozit" runat="server"></asp:TextBox>

                        <asp:Button ID="btnDepozitRevenire" runat="server" Text="Înapoi" OnClick="BtnDepozitRevenire_Click" />

                        <asp:Image ID="imgBackgroundDetaliiDepozite" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                    </div>

                    <div class="data_container" id="stergereDepozit" runat="server">

                        <asp:Label ID="lblStergereDepozit" runat="server" Text="Desființare depozit"></asp:Label>

                        <asp:Label ID="lblMesaj" CssClass="nextTo" runat="server"></asp:Label>

                        <asp:CheckBox ID="cbAcordDesfiintare" runat="server" />

                        <asp:Label ID="lblAcord" runat="server" Text="Sunt de acord să desființez depozitul"></asp:Label>

                        <asp:Button ID="btnContinuareDesfiintare" runat="server" Text="Continuare" OnClick="BtnContinuareDesfiintare_Click" />

                        <asp:Button ID="btnAnulareDesfiintare" runat="server" Text="Anulare" OnClick="BtnAnulareDesfiintare_Click" />

                        <asp:Label ID="lblMesajEroare" runat="server" ></asp:Label>

                    </div>

                     <div class="data_container" id="desfiintareEconomii" runat="server">

                        <asp:Label ID="lblDesfiintareCont" runat="server" Text="Desființare cont de economii"></asp:Label>

                        <asp:Label ID="lblMesajEconomii" CssClass="nextTo" runat="server"></asp:Label>

                        <asp:CheckBox ID="cbAcordDesfiintareEconomii" runat="server" />

                        <asp:Label ID="lblAcordEconomii" runat="server" Text="Sunt de acord să desființez contul de economii"></asp:Label>

                        <asp:Button ID="btnContinuareEconomii" runat="server" Text="Continuare" OnClick="BtnContinuareEconomii_Click" />

                        <asp:Button ID="btnAnulareEconomii" runat="server" Text="Anulare" OnClick="BtnAnulareEconomii_Click"/>

                        <asp:Label ID="lblMesajEroareEconomii" runat="server" ></asp:Label>

                    </div>

                    <div class="data_container" id="deschidereDepozit" onclick="divDepozitNouClick()" runat="server">

                        <asp:Label ID="lblDeschidereDepozitNou" runat="server" Text="Deschidere depozit nou"></asp:Label>

                        <asp:Label ID="lblContTransferNou" CssClass="nextTo" runat="server" Text="Cont selectat pentru transfer:"></asp:Label>

                        <asp:TextBox ID="txtContTransferNou" runat="server"></asp:TextBox>

                        <asp:Label ID="lblTipDepozitNou" CssClass="nextTo" runat="server" Text="Tip depozit"></asp:Label>

                        <asp:DropDownList ID="dlTipDepozitNou" AutoPostBack="true" OnSelectedIndexChanged="DlTipDepozitNou_SelectedIndexChanged" runat="server"></asp:DropDownList>
                        
                        <asp:Label ID="lblValoareDepozitNou" CssClass="nextTo" runat="server" Text="Valoare depozit:"></asp:Label>

                        <asp:TextBox ID="txtValoareDepozitNou" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblValutaDepozitNou" CssClass="nextTo" runat="server" Text="Valută depozit:"></asp:Label>

                        <asp:TextBox ID="txtValutaDepozitNou" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblPerioadaDepozitNou" CssClass="nextTo" runat="server" Text="Perioadă depozit (luni):"></asp:Label>

                        <asp:TextBox ID="txtPerioadaDepozitNou" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblRataDobandaDepozitNou" CssClass="nextTo" runat="server" Text="Rată dobândă:"></asp:Label>

                        <asp:TextBox ID="txtRataDobandaDepozitNou" runat="server"></asp:TextBox>

                        <asp:CheckBox ID="cbPrelungire" AutoPostBack="true" OnCheckedChanged="CbPrelungire_CheckedChanged" runat="server" />

                        <asp:Label ID="lblPrelungireDepozitNou" CssClass="nextTo" runat="server" Text="Prelungire"></asp:Label>

                        <asp:CheckBox ID="cbCapitalizare" runat="server" />

                        <asp:Label ID="lblCapitalizareDepozitNou" CssClass="nextTo" runat="server" Text="Capitalizare"></asp:Label>
                        
                        <asp:Label ID="lblDataInceput" CssClass="nextTo" runat="server" Text="Dată început:"></asp:Label>

                        <asp:TextBox ID="txtDataInceput" runat="server" ></asp:TextBox>

                        <asp:ImageButton ID="ibCalendar" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbCalendar_Click"/>

                        <asp:Calendar ID="cldDepozitNou" runat="server" OnVisibleMonthChanged="CldDepozitNou_VisibleMonthChanged" OnSelectionChanged="CldDepozitNou_SelectionChanged" BorderStyle="NotSet"></asp:Calendar>

                        <asp:Label ID="lblEroareCalendar" runat="server" Text="Data selectată mai mică decat cea curentă"></asp:Label>

                        <asp:CheckBox ID="cbAcordDepozitNou" runat="server" />

                        <asp:Label ID="lblAcordDepozitNou" runat="server" Text="Sunt de acord"></asp:Label>

                        <asp:Label ID="lblDateIncomplete" runat="server" ></asp:Label>

                        <asp:Button ID="btnFinalizareDepozitNou" runat="server" Text="DESCHIDE DEPOZIT" OnClick="BtnFinalizareDepozitNou_Click" />

                        <asp:Button ID="btnAnulareDepozitNou" runat="server" Text="ANULARE" OnClick="BtnAnulareDepozitNou_Click" />

                        <asp:Image ID="imgBackgroundDepozit" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                    </div>

                    <div class="data_container" id="deschideCont" runat="server">

                        <asp:Label ID="lblTitluDeschidere" runat="server" Text="Deschide un cont nou"></asp:Label>

                        <asp:Label ID="lblTipContNou" runat="server" Text="Tip cont:"></asp:Label>

                        <asp:DropDownList ID="dlTipContNou" runat="server"></asp:DropDownList>

                        <asp:Label ID="lblValutaContNou" runat="server" Text="Valuta cont:"></asp:Label>

                        <asp:DropDownList ID="dlValutaContNou" runat="server"></asp:DropDownList>

                        <asp:CheckBox ID="cbAcordContNou" runat="server" />

                        <asp:Label ID="lblAcordContNou" runat="server" Text="Sunt de acord"></asp:Label>

                        <asp:Label ID="lblEroareContNou" runat="server" ></asp:Label>

                        <asp:Button ID="btnFinalizareContNou" runat="server" Text="CREAZĂ CONT" OnClick="BtnFinalizareContNou_Click" />

                        <asp:Button ID="btnAnulareContNou" runat="server" Text="ANULARE" OnClick="BtnAnulareContNou_Click" />

                        <asp:Image ID="imgBackgroundCont" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                    </div>

                    <div class="data_container" id="cardurileMele" runat="server">

                        <asp:Label ID="lblVizualizareCarduri" runat="server" Text="Vizualizare carduri"></asp:Label>

                        <asp:Label ID="lblCarduriDebit" runat="server" Text="Carduri debit"></asp:Label>

                        <asp:GridView ID="gvCarduriDebit" runat="server" HeaderStyle-BackColor="#71838C" GridLines="None" BackColor="#EBF1F4" HeaderStyle-Height="25" HeaderStyle-ForeColor="White"  SelectedRowStyle-BackColor="#BFCFDB" OnSelectedIndexChanged="GvCarduriDebit_SelectedIndexChanged" OnRowDataBound="GvCarduriDebit_RowDataBound"/>
                         
                        <asp:Button ID="btnDetaliiDebit" runat="server" Text="Vizualizare" OnClick="BtnDetaliiDebit_Click" />

                        <asp:Button ID="btnLimiteDebit" runat="server" Text="Limite" OnClick="BtnLimiteDebit_Click" />

                        <asp:Button ID="btnLinieCredit" runat="server" Text="Setare linie de credit" OnClick="BtnLinieCredit_Click" />

                        <asp:Label ID="lblCarduriCredit" runat="server" Text="Carduri credit"></asp:Label>

                        <asp:GridView ID="gvCarduriCredit" runat="server" HeaderStyle-BackColor="#71838C" GridLines="None" BackColor="#EBF1F4" HeaderStyle-Height="25" HeaderStyle-ForeColor="White"  SelectedRowStyle-BackColor="#BFCFDB" OnSelectedIndexChanged="GvCarduriCredit_SelectedIndexChanged" OnRowDataBound="GvCarduriCredit_RowDataBound"/>

                         <asp:Button ID="btnDetaliiCredit" runat="server" Text="Vizualizare" OnClick="BtnDetaliiCredit_Click" />

                        <asp:Button ID="btnLimiteCredit" runat="server" Text="Limite" OnClick="BtnLimiteCredit_Click" />

                    </div>

                    <div class="data_container" id="vizualizareCardDebit" runat="server">

                        <asp:Label ID="lblVizualizareCardDebit" runat="server" Text="Vizualizare card"></asp:Label>

                        <asp:Label ID="lblDenumireProdusDebit" runat="server" Text="Denumire produs:"></asp:Label>

                        <asp:TextBox ID="txtDenumireProdusDebit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblStareCardDebit" runat="server" Text="Stare card:"></asp:Label>

                        <asp:TextBox ID="txtStareCardDebit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblNumarCardDebit" runat="server" Text="Număr card:"></asp:Label>

                        <asp:TextBox ID="txtNumarCardDebit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblCVVDebit" runat="server" Text="CVV:"></asp:Label>

                        <asp:TextBox ID="txtCVVDebit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblIbanAtasatDebit" runat="server" Text="IBAN Cont atașat:"></asp:Label>
                        
                        <asp:TextBox ID="txtIbanAtasatDebit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblNumeTitularDebit" runat="server" Text="Nume titular:"></asp:Label>

                        <asp:TextBox ID="txtNumeTitularDebit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataExpirareDebit" runat="server" Text="Dată expirare:"></asp:Label>

                        <asp:TextBox ID="txtDataExpirareDebit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLinieCreditDebit" runat="server" Text="Linie credit:"></asp:Label>

                        <asp:TextBox ID="txtLinieCreditDebit" runat="server"></asp:TextBox>

                        <asp:Image ID="imgCardDebitDebit" ImageUrl="~/Media/Second_Page/card_debit.png" runat="server" />

                        <asp:Label ID="lblNumarCardFizicDebit" runat="server"></asp:Label>

                        <asp:Label ID="lblNumeTitularFizicDebit" runat="server"></asp:Label>

                        <asp:Label ID="lblCVVFizicDebit" runat="server"></asp:Label>

                        <asp:Label ID="lblDataExpirareFizicDebit" runat="server"></asp:Label>

                        <asp:Label ID="lblTipCardFizicDebit" runat="server"></asp:Label>

                    </div>

                    <div class="data_container" id="limiteDebit" runat="server">

                        <asp:Label ID="lblLimiteDebit" runat="server" Text="Limite card debit"></asp:Label>

                        <asp:Label ID="lblTipLimitaDebitInternet" runat="server" Text="Tip limită"></asp:Label>

                        <asp:TextBox ID="txtTipLimitaDebitInternet" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaStandardDebitInternet" runat="server" Text="Limită standard"></asp:Label>

                        <asp:TextBox ID="txtLimitaStandardDebitInternet" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataInceputLimitaDebitInternet" runat="server" Text="Dată început"></asp:Label>

                        <asp:TextBox ID="txtDataInceputLimitaDebitInternet" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataSfarsitLimitaDebitInternet" runat="server" Text="Dată sfârșit"></asp:Label>

                        <asp:TextBox ID="txtDataSfarsitLimitaDebitInternet" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaNouaDebitInternet" runat="server" Text="Limită nouă"></asp:Label>

                        <asp:TextBox ID="txtLimitaNouaDebitInternet" runat="server"></asp:TextBox>

                        <asp:Button ID="btnModificareDebitInternet" runat="server" Text="Modifică" OnClick="BtnModificareDebitInternet_Click" />

                        <asp:Button ID="btnAnulareDebitInternet" runat="server" Text="Resetare" OnClick="BtnAnulareDebitInternet_Click" />
                       
                        <asp:ImageButton ID="ibInceputCalendarInternet" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbInceputCalendarInternet_Click"/>

                        <asp:ImageButton ID="ibSfarsitCalendarInternet" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbSfarsitCalendarInternet_Click"/>

                        <asp:Calendar ID="cldInceputInternet" runat="server" OnVisibleMonthChanged="CldInceputInternet_VisibleMonthChanged" OnSelectionChanged="CldInceputInternet_SelectionChanged"></asp:Calendar>

                        <asp:Calendar ID="cldSfarsitInternet" runat="server" OnVisibleMonthChanged="CldSfarsitInternet_VisibleMonthChanged" OnSelectionChanged="CldSfarsitInternet_SelectionChanged"></asp:Calendar>

                        <asp:Button ID="btnLimiteInapoi" runat="server" Text="←" OnClick="BtnLimiteInapoi_Click"/>




                        <asp:Label ID="lblTipLimitaDebitNumerar" runat="server" Text="Tip limită"></asp:Label>

                        <asp:TextBox ID="txtTipLimitaDebitNumerar" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaStandardDebitNumerar" runat="server" Text="Limită standard"></asp:Label>

                        <asp:TextBox ID="txtLimitaStandardDebitNumerar" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataInceputLimitaDebitNumerar" runat="server" Text="Dată început"></asp:Label>

                        <asp:TextBox ID="txtDataInceputLimitaDebitNumerar" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataSfarsitLimitaDebitNumerar" runat="server" Text="Dată sfârșit"></asp:Label>

                        <asp:TextBox ID="txtDataSfarsitLimitaDebitNumerar" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaNouaDebitNumerar" runat="server" Text="Limită nouă"></asp:Label>

                        <asp:TextBox ID="txtLimitaNouaDebitNumerar" runat="server"></asp:TextBox>

                        <asp:Button ID="btnModificareDebitNumerar" runat="server" Text="Modifică" OnClick="BtnModificareDebitNumerar_Click"/>

                        <asp:Button ID="btnAnulareDebitNumerar" runat="server" Text="Resetare" OnClick="BtnAnulareDebitNumerar_Click"/>

                        <asp:ImageButton ID="ibInceputCalendarNumerar" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbInceputCalendarNumerar_Click"/>

                        <asp:ImageButton ID="ibSfarsitCalendarNumerar" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbSfarsitCalendarNumerar_Click"/>

                        <asp:Calendar ID="cldInceputNumerar" runat="server" OnVisibleMonthChanged="CldInceputNumerar_VisibleMonthChanged" OnSelectionChanged="CldInceputNumerar_SelectionChanged"></asp:Calendar>

                        <asp:Calendar ID="cldSfarsitNumerar" runat="server" OnVisibleMonthChanged="CldSfarsitNumerar_VisibleMonthChanged" OnSelectionChanged="CldSfarsitNumerar_SelectionChanged"></asp:Calendar>
                        



                        <asp:Label ID="lblTipLimitaDebitTranzactii" runat="server" Text="Tip limită"></asp:Label>

                        <asp:TextBox ID="txtTipLimitaDebitTranzactii" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaStandardDebitTranzactii" runat="server" Text="Limită standard"></asp:Label>

                        <asp:TextBox ID="txtLimitaStandardDebitTranzactii" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataInceputLimitaDebitTranzactii" runat="server" Text="Dată început"></asp:Label>

                        <asp:TextBox ID="txtDataInceputLimitaDebitTranzactii" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataSfarsitLimitaDebitTranzactii" runat="server" Text="Dată sfârșit"></asp:Label>

                        <asp:TextBox ID="txtDataSfarsitLimitaDebitTranzactii" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaNouaDebitTranzactii" runat="server" Text="Limită nouă"></asp:Label>

                        <asp:TextBox ID="txtLimitaNouaDebitTranzactii" runat="server"></asp:TextBox>

                        <asp:Button ID="btnModificareDebitTranzactii" runat="server" Text="Modifică" OnClick="BtnModificareDebitTranzactii_Click" />

                        <asp:Button ID="btnAnulareDebitTranzactii" runat="server" Text="Resetare" OnClick="BtnAnulareDebitTranzactii_Click" />

                        <asp:ImageButton ID="ibInceputCalendarTranzactii" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbInceputCalendarTranzactii_Click"/>

                        <asp:ImageButton ID="ibSfarsitCalendarTranzactii" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbSfarsitCalendarTranzactii_Click"/>

                        <asp:Calendar ID="cldInceputTranzactii" runat="server" OnVisibleMonthChanged="CldInceputTranzactii_VisibleMonthChanged" OnSelectionChanged="CldInceputTranzactii_SelectionChanged"></asp:Calendar>

                        <asp:Calendar ID="cldSfarsitTranzactii" runat="server" OnVisibleMonthChanged="CldSfarsitTranzactii_VisibleMonthChanged" OnSelectionChanged="CldSfarsitTranzactii_SelectionChanged"></asp:Calendar>


                        
                        
                        <asp:Label ID="lblTipLimitaDebitPOS" runat="server" Text="Tip limită"></asp:Label>

                        <asp:TextBox ID="txtTipLimitaDebitPOS" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaStandardDebitPOS" runat="server" Text="Limită standard"></asp:Label>

                        <asp:TextBox ID="txtLimitaStandardDebitPOS" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataInceputLimitaDebitPOS" runat="server" Text="Dată început"></asp:Label>

                        <asp:TextBox ID="txtDataInceputLimitaDebitPOS" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataSfarsitLimitaDebitPOS" runat="server" Text="Dată sfârșit"></asp:Label>

                        <asp:TextBox ID="txtDataSfarsitLimitaDebitPOS" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaNouaDebitPOS" runat="server" Text="Limită nouă"></asp:Label>

                        <asp:TextBox ID="txtLimitaNouaDebitPOS" runat="server"></asp:TextBox>

                        <asp:Button ID="btnModificareDebitPOS" runat="server" Text="Modifică" OnClick="BtnModificareDebitPOS_Click" />

                        <asp:Button ID="btnAnulareDebitPOS" runat="server" Text="Resetare" OnClick="BtnAnulareDebitPOS_Click" />

                        <asp:ImageButton ID="ibInceputCalendarPOS" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbInceputCalendarPOS_Click"/>

                        <asp:ImageButton ID="ibSfarsitCalendarPOS" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbSfarsitCalendarPOS_Click"/>
                    
                        <asp:Calendar ID="cldInceputPOS" runat="server" OnVisibleMonthChanged="CldInceputPOS_VisibleMonthChanged" OnSelectionChanged="CldInceputPOS_SelectionChanged"></asp:Calendar>

                        <asp:Calendar ID="cldSfarsitPOS" runat="server" OnVisibleMonthChanged="CldSfarsitPOS_VisibleMonthChanged" OnSelectionChanged="CldSfarsitPOS_SelectionChanged"></asp:Calendar>

                    </div>

                    <div class="data_container" id="istoricTranzactiiDebit" runat="server">

                        <asp:Label ID="lblIstoricTranzactiiDebit" runat="server" Text="Istoric tranzacții"></asp:Label>

                        <asp:Label ID="lblCard" runat="server" Text="Card"></asp:Label>

                        <asp:DropDownList ID="dlCardDebit" runat="server"></asp:DropDownList>

                        <asp:RadioButtonList ID="rblPerioadaDebit" runat="server" AutoPostBack="true" OnSelectedIndexChanged="RblPerioadaDebit_SelectedIndexChanged" ></asp:RadioButtonList>

                        <asp:Label ID="lblDataInceputIstoricDebit" runat="server" Text="Dată început"></asp:Label>

                        <asp:TextBox ID="txtDataInceputIstoricDebit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLinieIstoricDebit" runat="server" Text="━"></asp:Label>

                        <asp:Label ID="lblDataSfarsitIstoricDebit" runat="server" Text="Dată sfârșit"></asp:Label>

                        <asp:TextBox ID="txtDataSfarsitIstoricDebit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblTipTranzactieIstoricDebit" runat="server" Text="Tip tranzacție"></asp:Label>

                        <asp:DropDownList ID="dlTipTranzactieIstoricDebit" runat="server"></asp:DropDownList>

                        <asp:Label ID="lblListaTranzactiiIstoricDebit" runat="server" Text="LISTĂ TRANZACȚII"></asp:Label>

                        <asp:GridView ID="gvListaTranzactiiIstoricDebit" runat="server" HeaderStyle-BackColor="#71838C" GridLines="None" BackColor="#EBF1F4" HeaderStyle-Height="25" HeaderStyle-ForeColor="White"  SelectedRowStyle-BackColor="#BFCFDB"></asp:GridView>

                        <asp:ImageButton ID="ibDataInceputIstoricDebit" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbDataInceputIstoricDebit_Click"/>

                        <asp:ImageButton ID="ibDataSfarsitIstoricDebit" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbDataSfarsitIstoricDebit_Click" />

                        <asp:Calendar ID="cldDataInceputIstoricDebit" runat="server" OnVisibleMonthChanged="CldDataInceputIstoricDebit_VisibleMonthChanged" OnSelectionChanged="CldDataInceputIstoricDebit_SelectionChanged"></asp:Calendar>

                        <asp:Calendar ID="cldDataSfarsitIstoricDebit" runat="server" OnVisibleMonthChanged="CldDataSfarsitIstoricDebit_VisibleMonthChanged" OnSelectionChanged="CldDataSfarsitIstoricDebit_SelectionChanged"></asp:Calendar>

                        <asp:Button ID="btnVizualizareTranzactiiIstoricDebit" runat="server" Text="Vizualizare" OnClick="BtnVizualizareTranzactiiIstoricDebit_Click" />
                        
                        <asp:Label ID="lblEroareIstoric" runat="server" Text="Introduceți toate datele!"></asp:Label>

                        <asp:Button ID="btnExportPdfIstoric" runat="server" Text="Export PDF" OnClick="BtnExportPdfIstoric_Click" />

                    </div>

                    <div class="data_container" id="linieCredit" runat="server">

                        <asp:Label ID="lblLinieCredit" runat="server" Text="Linie credit"></asp:Label>

                        <asp:Label ID="lblNrCardLinie" runat="server" Text="Card"></asp:Label>

                        <asp:TextBox ID="txtNrCardLinie" runat="server"></asp:TextBox>

                        <asp:Label ID="lblPlafonLinie" runat="server" Text="Plafon (salarii)"></asp:Label>

                        <asp:DropDownList ID="dlPlafonLinie" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DlPlafonLinie_SelectedIndexChanged"></asp:DropDownList>

                        <asp:Label ID="lblSumaLinie" runat="server" Text="Sumă"></asp:Label>

                        <asp:TextBox ID="txtSumaLinie" runat="server"></asp:TextBox>

                        <asp:Button ID="btnFinalizareLinie" runat="server" Text="Finalizare" OnClick="BtnFinalizareLinie_Click"/>

                        <asp:Button ID="btnAnulareLinie" runat="server" Text="Anulare" OnClick="BtnAnulareLinie_Click"/>

                        <asp:Image ID="imgBackgroundLinie" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                        <ul class="infoLinie" runat="server">
                            
                            <li style="padding-left:1em; padding-bottom:2em"><asp:Label ID="lblValoareDobandaInfo" runat="server" Text="Dobânda este fixă, de 14%/an."></asp:Label></li>

                            <li style="padding-left:1em; padding-bottom:2em"><asp:Label ID="lblModRambursareInfo" runat="server" Text="Rambursarea liniei de credit se face automat din soldul disponibil în contul curent."></asp:Label></li>

                            <li style="padding-left:1em; padding-bottom:2em"><asp:Label ID="lblDobandaInfo" runat="server" Text="Dobânda se calculează zilnic la suma utilizată din limita creditului acordat."></asp:Label></li>

                            <li style="padding-left:1em; padding-bottom:2em"><asp:Label ID="lblRambursareInfo" runat="server" Text="De fiecare dată când alimentezi contul tău curent, rambursezi parțial creditul și îți diminuezi limita de credit utilizată."></asp:Label></li>
                        
                        </ul>

                    </div>

                        <div class="data_container" id="limiteCredit" runat="server">

                        <asp:Label ID="lblLimiteCredit" runat="server" Text="Limite card credit"></asp:Label>

                        <asp:Label ID="lblTipLimitaCreditInternet" runat="server" Text="Tip limită"></asp:Label>

                        <asp:TextBox ID="txtTipLimitaCreditInternet" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaStandardCreditInternet" runat="server" Text="Limită standard"></asp:Label>

                        <asp:TextBox ID="txtLimitaStandardCreditInternet" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataInceputLimitaCreditInternet" runat="server" Text="Dată început"></asp:Label>

                        <asp:TextBox ID="txtDataInceputLimitaCreditInternet" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataSfarsitLimitaCreditInternet" runat="server" Text="Dată sfârșit"></asp:Label>

                        <asp:TextBox ID="txtDataSfarsitLimitaCreditInternet" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaNouaCreditInternet" runat="server" Text="Limită nouă"></asp:Label>

                        <asp:TextBox ID="txtLimitaNouaCreditInternet" runat="server"></asp:TextBox>

                        <asp:Button ID="btnModificareCreditInternet" runat="server" Text="Modifică" OnClick="BtnModificareCreditInternet_Click" />

                        <asp:Button ID="btnAnulareCreditInternet" runat="server" Text="Resetare" OnClick="BtnAnulareCreditInternet_Click" />
                       
                        <asp:ImageButton ID="ibInceputCalendarInternetCredit" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbInceputCalendarInternetCredit_Click"/>

                        <asp:ImageButton ID="ibSfarsitCalendarInternetCredit" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbSfarsitCalendarInternetCredit_Click"/>

                        <asp:Calendar ID="cldInceputInternetCredit" runat="server" OnVisibleMonthChanged="CldInceputInternetCredit_VisibleMonthChanged" OnSelectionChanged="CldInceputInternetCredit_SelectionChanged"></asp:Calendar>

                        <asp:Calendar ID="cldSfarsitInternetCredit" runat="server" OnVisibleMonthChanged="CldSfarsitInternetCredit_VisibleMonthChanged" OnSelectionChanged="CldSfarsitInternetCredit_SelectionChanged"></asp:Calendar>

                        <asp:Button ID="btnLimiteInapoiCredit" runat="server" Text="←" OnClick="BtnLimiteInapoiCredit_Click"/>




                        <asp:Label ID="lblTipLimitaCreditNumerar" runat="server" Text="Tip limită"></asp:Label>

                        <asp:TextBox ID="txtTipLimitaCreditNumerar" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaStandardCreditNumerar" runat="server" Text="Limită standard"></asp:Label>

                        <asp:TextBox ID="txtLimitaStandardCreditNumerar" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataInceputLimitaCreditNumerar" runat="server" Text="Dată început"></asp:Label>

                        <asp:TextBox ID="txtDataInceputLimitaCreditNumerar" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataSfarsitLimitaCreditNumerar" runat="server" Text="Dată sfârșit"></asp:Label>

                        <asp:TextBox ID="txtDataSfarsitLimitaCreditNumerar" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaNouaCreditNumerar" runat="server" Text="Limită nouă"></asp:Label>

                        <asp:TextBox ID="txtLimitaNouaCreditNumerar" runat="server"></asp:TextBox>

                        <asp:Button ID="btnModificareCreditNumerar" runat="server" Text="Modifică" OnClick="BtnModificareCreditNumerar_Click"/>

                        <asp:Button ID="btnAnulareCreditNumerar" runat="server" Text="Resetare" OnClick="BtnAnulareCreditNumerar_Click"/>

                        <asp:ImageButton ID="ibInceputCalendarNumerarCredit" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbInceputCalendarNumerarCredit_Click"/>

                        <asp:ImageButton ID="ibSfarsitCalendarNumerarCredit" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbSfarsitCalendarNumerarCredit_Click"/>

                        <asp:Calendar ID="cldInceputNumerarCredit" runat="server" OnVisibleMonthChanged="CldInceputNumerarCredit_VisibleMonthChanged" OnSelectionChanged="CldInceputNumerarCredit_SelectionChanged"></asp:Calendar>

                        <asp:Calendar ID="cldSfarsitNumerarCredit" runat="server" OnVisibleMonthChanged="CldSfarsitNumerarCredit_VisibleMonthChanged" OnSelectionChanged="CldSfarsitNumerarCredit_SelectionChanged"></asp:Calendar>
                        



                        <asp:Label ID="lblTipLimitaCreditTranzactii" runat="server" Text="Tip limită"></asp:Label>

                        <asp:TextBox ID="txtTipLimitaCreditTranzactii" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaStandardCreditTranzactii" runat="server" Text="Limită standard"></asp:Label>

                        <asp:TextBox ID="txtLimitaStandardCreditTranzactii" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataInceputLimitaCreditTranzactii" runat="server" Text="Dată început"></asp:Label>

                        <asp:TextBox ID="txtDataInceputLimitaCreditTranzactii" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataSfarsitLimitaCreditTranzactii" runat="server" Text="Dată sfârșit"></asp:Label>

                        <asp:TextBox ID="txtDataSfarsitLimitaCreditTranzactii" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaNouaCreditTranzactii" runat="server" Text="Limită nouă"></asp:Label>

                        <asp:TextBox ID="txtLimitaNouaCreditTranzactii" runat="server"></asp:TextBox>

                        <asp:Button ID="btnModificareCreditTranzactii" runat="server" Text="Modifică" OnClick="BtnModificareCreditTranzactii_Click" />

                        <asp:Button ID="btnAnulareCreditTranzactii" runat="server" Text="Resetare" OnClick="BtnAnulareCreditTranzactii_Click" />

                        <asp:ImageButton ID="ibInceputCalendarTranzactiiCredit" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbInceputCalendarTranzactiiCredit_Click"/>

                        <asp:ImageButton ID="ibSfarsitCalendarTranzactiiCredit" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbSfarsitCalendarTranzactiiCredit_Click"/>

                        <asp:Calendar ID="cldInceputTranzactiiCredit" runat="server" OnVisibleMonthChanged="CldInceputTranzactiiCredit_VisibleMonthChanged" OnSelectionChanged="CldInceputTranzactiiCredit_SelectionChanged"></asp:Calendar>

                        <asp:Calendar ID="cldSfarsitTranzactiiCredit" runat="server" OnVisibleMonthChanged="CldSfarsitTranzactiiCredit_VisibleMonthChanged" OnSelectionChanged="CldSfarsitTranzactiiCredit_SelectionChanged"></asp:Calendar>


                        
                        
                        <asp:Label ID="lblTipLimitaCreditPOS" runat="server" Text="Tip limită"></asp:Label>

                        <asp:TextBox ID="txtTipLimitaCreditPOS" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaStandardCreditPOS" runat="server" Text="Limită standard"></asp:Label>

                        <asp:TextBox ID="txtLimitaStandardCreditPOS" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataInceputLimitaCreditPOS" runat="server" Text="Dată început"></asp:Label>

                        <asp:TextBox ID="txtDataInceputLimitaCreditPOS" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataSfarsitLimitaCreditPOS" runat="server" Text="Dată sfârșit"></asp:Label>

                        <asp:TextBox ID="txtDataSfarsitLimitaCreditPOS" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaNouaCreditPOS" runat="server" Text="Limită nouă"></asp:Label>

                        <asp:TextBox ID="txtLimitaNouaCreditPOS" runat="server"></asp:TextBox>

                        <asp:Button ID="btnModificareCreditPOS" runat="server" Text="Modifică" OnClick="BtnModificareCreditPOS_Click" />

                        <asp:Button ID="btnAnulareCreditPOS" runat="server" Text="Resetare" OnClick="BtnAnulareCreditPOS_Click" />

                        <asp:ImageButton ID="ibInceputCalendarPOSCredit" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbInceputCalendarPOSCredit_Click"/>

                        <asp:ImageButton ID="ibSfarsitCalendarPOSCredit" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbSfarsitCalendarPOSCredit_Click"/>
                    
                        <asp:Calendar ID="cldInceputPOSCredit" runat="server" OnVisibleMonthChanged="CldInceputPOSCredit_VisibleMonthChanged" OnSelectionChanged="CldInceputPOSCredit_SelectionChanged"></asp:Calendar>

                        <asp:Calendar ID="cldSfarsitPOSCredit" runat="server" OnVisibleMonthChanged="CldSfarsitPOSCredit_VisibleMonthChanged" OnSelectionChanged="CldSfarsitPOSCredit_SelectionChanged"></asp:Calendar>

                    </div>

                    <div class="data_container" id="vizualizareCardCredit" runat="server">

                        <asp:Label ID="lblVizualizareCardCredit" runat="server" Text="Vizualizare card"></asp:Label>

                        <asp:Label ID="lblDenumireProdusCredit" runat="server" Text="Denumire produs:"></asp:Label>

                        <asp:TextBox ID="txtDenumireProdusCredit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblSoldCardCredit" runat="server" Text="Sold card:"></asp:Label>

                        <asp:TextBox ID="txtSoldCardCredit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblNumarCardCredit" runat="server" Text="Număr card:"></asp:Label>

                        <asp:TextBox ID="txtNumarCardCredit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblCVVCredit" runat="server" Text="CVV:"></asp:Label>

                        <asp:TextBox ID="txtCVVCredit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblIbanAtasatCredit" runat="server" Text="IBAN Cont atașat:"></asp:Label>
                        
                        <asp:TextBox ID="txtIbanAtasatCredit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblNumeTitularCredit" runat="server" Text="Nume titular:"></asp:Label>

                        <asp:TextBox ID="txtNumeTitularCredit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataExpirareCredit" runat="server" Text="Dată expirare:"></asp:Label>

                        <asp:TextBox ID="txtDataExpirareCredit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLimitaCardCredit" runat="server" Text="Limită credit:"></asp:Label>

                        <asp:TextBox ID="txtLimitaCardCredit" runat="server"></asp:TextBox>

                        <asp:Image ID="imgCardCreditCredit" ImageUrl="~/Media/Second_Page/card_credit.png" runat="server" />

                        <asp:Label ID="lblNumarCardFizicCredit" runat="server"></asp:Label>

                        <asp:Label ID="lblNumeTitularFizicCredit" runat="server"></asp:Label>

                        <asp:Label ID="lblCVVFizicCredit" runat="server"></asp:Label>

                        <asp:Label ID="lblDataExpirareFizicCredit" runat="server"></asp:Label>

                    </div>

                    <div class="data_container" id="platiIntreConturi" runat="server">

                        <asp:Label ID="lblTransferIntreConturi" runat="server" Text="Transfer între conturile mele"></asp:Label>

                        <asp:Label ID="lblTransferDinContul" runat="server" Text="Transfer din contul"></asp:Label>

                        <asp:TextBox ID="txtTransferDinContul" runat="server"></asp:TextBox>

                        <asp:Label ID="lblTransferCatreContul" runat="server" Text="Către contul"></asp:Label>

                        <asp:DropDownList ID="dlTransferCatreContul" runat="server"></asp:DropDownList>

                        <asp:Label ID="lblSumaTransferIntreConturi" runat="server" Text="Suma"></asp:Label>

                        <asp:TextBox ID="txtSumaTransferIntreConturi" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDetaliiTransferIntreConturi" runat="server" Text="Detalii transfer"></asp:Label>

                        <asp:TextBox ID="txtDetaliiTransferIntreConturi" runat="server"></asp:TextBox>

                        <asp:Label ID="lblEroareTransferIntreConturi" runat="server" Text="Introduceți toate datele!"></asp:Label>

                        <asp:Button ID="btnFinalizareTransferIntreConturi" runat="server" Text="Finalizare transfer" OnClick="BtnFinalizareTransferIntreConturi_Click" />

                        <asp:Button ID="btnAnulareTransferIntreConturi" runat="server" Text="Anulare" OnClick="BtnAnulareTransferIntreConturi_Click" />

                        <asp:Image ID="imgBackgroundIntreConturi" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                    </div>

                    <div class="data_container" id="platiCont" runat="server">

                        <asp:Label ID="lblTitluPlatiCont" runat="server" Text="Plată"></asp:Label>

                        <asp:Label ID="lblPlataDinCont" runat="server" Text="Plată din contul"></asp:Label>

                        <asp:TextBox ID="txtPlataDinContul" runat="server"></asp:TextBox>

                        <asp:Label ID="lblBeneficiarPredefinit" runat="server" Text="Beneficiar predefinit"></asp:Label>

                        <asp:DropDownList ID="dlBeneficiarPredefinit" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DlBeneficiarPredefinit_SelectedIndexChanged"></asp:DropDownList>

                        <asp:Label ID="lblNumeBeneficiar" runat="server" Text="Nume beneficiar"></asp:Label>

                        <asp:TextBox ID="txtNumeBeneficiar" runat="server"></asp:TextBox>

                        <asp:Label ID="lblContBeneficiar" runat="server" Text="Cont beneficiar"></asp:Label>

                        <asp:TextBox ID="txtContBeneficiar" runat="server"></asp:TextBox>

                        <asp:Label ID="lblSumaPlataDinCont" runat="server" Text="Suma"></asp:Label>

                        <asp:TextBox ID="txtSumaPlataDinCont" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDetaliiPlata" runat="server" Text="Detalii plată"></asp:Label>

                        <asp:TextBox ID="txtDetaliiPlata" runat="server"></asp:TextBox>

                        <asp:Label ID="lblEroarePlataCont" runat="server" Text="Introduceți toate datele!"></asp:Label>

                        <asp:Button ID="btnFinalizarePlata" runat="server" Text="Finalizare plată" OnClick="BtnFinalizarePlata_Click" />

                        <asp:Button ID="btnAnularePlata" runat="server" Text="Anulare" />

                        <asp:Image ID="imgBackgroundPlatiCont" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                    </div>

                    <div class="data_container" id="conversieBani" runat="server">

                        <asp:Label ID="lblTitluConversie" runat="server" Text="Conversie"></asp:Label>

                        <asp:Label ID="lblTipConversie" runat="server" Text="Tip operațiune"></asp:Label>

                        <asp:RadioButtonList ID="rblListaTipConversie" runat="server" AutoPostBack="true" OnSelectedIndexChanged="RblListaTipConversie_SelectedIndexChanged" ></asp:RadioButtonList>

                        <asp:Label ID="lblContPlataConversie" runat="server" Text="Plată din contul"></asp:Label>

                        <asp:DropDownList ID="dlContPlataConversie" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DlContPlataConversie_SelectedIndexChanged"></asp:DropDownList>

                        <asp:Label ID="lblContDestinatarConversie" runat="server" Text="Către contul"></asp:Label>

                        <asp:DropDownList ID="dlContDestinatarConversie" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DlContDestinatarConversie_SelectedIndexChanged" ></asp:DropDownList>

                        <asp:Label ID="lblSumaConversie" runat="server" Text="Sumă"></asp:Label>

                        <asp:TextBox ID="txtSumaConversie" runat="server" AutoPostBack="true" OnTextChanged="TxtSumaConversie_TextChanged"></asp:TextBox>

                        <asp:Label ID="lblCursSchimbConversie" runat="server" Text="Curs valutar BNR:"></asp:Label>

                        <asp:Label ID="lblDataCursConversie" runat="server" Text="Dată curs valutar:"></asp:Label>

                        <asp:Label ID="lblCursSchimbBanca" runat="server"></asp:Label>
                        
                        <asp:Label ID="lblValoareCursConversie" runat="server"></asp:Label>

                        <asp:Label ID="lblValoareDataConversie" runat="server"></asp:Label>

                        <asp:Label ID="lblValoareCursBanca" runat="server"></asp:Label>

                        <asp:Label ID="lblSumaDupaConversie" runat="server" Text="Sumă după conversie"></asp:Label>

                        <asp:TextBox ID="txtSumaDupaConversie" runat="server"></asp:TextBox>

                        <asp:Label ID="lblEroareConversie" runat="server" Text="Introduceți toate datele!"></asp:Label>

                        <asp:Button ID="btnFinalizareConversie" runat="server" Text="Finalizare conversie" OnClick="BtnFinalizareConversie_Click" />

                        <asp:Button ID="btnAnulareConversie" runat="server" Text="Anulare" />

                        <asp:Image ID="imgBackgroundConversie" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                    </div>

                    <div class="data_container" id="platiProgramate" runat="server">

                        <asp:Label ID="lblTitluPlatiProgramate" runat="server" Text="Plăți programate"></asp:Label>

                        <asp:Label ID="lblPlatileMeleProgramate" runat="server" Text="Plățile mele programate"></asp:Label>

                        <asp:GridView ID="gvPlatiProgramate" runat="server" HeaderStyle-BackColor="#71838C" GridLines="None" BackColor="#EBF1F4" HeaderStyle-Height="25" HeaderStyle-ForeColor="White"  SelectedRowStyle-BackColor="#BFCFDB" OnSelectedIndexChanged="GvPlatiProgramate_SelectedIndexChanged" OnRowDataBound="GvPlatiProgramate_RowDataBound"></asp:GridView>

                        <asp:Button ID="btnPlataNouaProgramata" runat="server" Text="ADAUGĂ PROGRAMARE" OnClick="BtnPlataNouaProgramata_Click" />

                        <asp:Button ID="btnAnularePlataProgramata" runat="server" Text="ANULARE PROGRAMARE" OnClick="BtnAnularePlataProgramata_Click" />

                        <asp:Image ID="imgBackgroundPlatiProgramare" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                    </div>

                    <div class="data_container" id="adaugarePlatiProgramate" runat="server">

                        <asp:Label ID="lblTitluAdaugarePlataProgramata" runat="server" Text="Programare nouă"></asp:Label>

                        <asp:Label ID="lblDenumirePlataProgramata" runat="server" Text="Denumire plată"></asp:Label>

                        <asp:TextBox ID="txtDenumirePlataProgramata" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataInceputPlataProgramata" runat="server" Text="Dată programare"></asp:Label>

                        <asp:TextBox ID="txtDataInceputPlataProgramata" runat="server"></asp:TextBox>

                        <asp:Label ID="lblContPlataProgramata" runat="server" Text="Plată din contul"></asp:Label>

                        <asp:TextBox ID="txtContPlataProgramata" runat="server"></asp:TextBox>

                        <asp:Label ID="lblBeneficiarPlataProgramata" runat="server" Text="Beneficiar predefinit"></asp:Label>

                        <asp:DropDownList ID="dlBeneficiarPlataProgramata" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DlBeneficiarPlataProgramata_SelectedIndexChanged" ></asp:DropDownList>

                         <asp:Label ID="lblNumeBeneficiarPlataProgramata" runat="server" Text="Nume beneficiar"></asp:Label>

                        <asp:TextBox ID="txtNumeBeneficiarPlataProgramata" runat="server"></asp:TextBox>

                        <asp:Label ID="lblContBeneficiarPlataProgramata" runat="server" Text="Cont beneficiar"></asp:Label>

                        <asp:TextBox ID="txtContBeneficiarPlataProgramata" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDetaliiPlataProgramata" runat="server" Text="Detalii plată"></asp:Label>

                        <asp:TextBox ID="txtDetaliiPlataProgramata" runat="server"></asp:TextBox>

                        <asp:Label ID="lblSumaPlataProgramata" runat="server" Text="Sumă"></asp:Label>

                        <asp:TextBox ID="txtSumaPlataProgramata" runat="server"></asp:TextBox>

                        <asp:Label ID="lblEroarePlatiProgramate" runat="server" Text="Introduceți toate datele!"></asp:Label>

                        <asp:Button ID="btnAdaugareProgramare" runat="server" Text="Adaugă programare" OnClick="BtnAdaugareProgramare_Click" />

                        <asp:Button ID="btnAnulareProgramare" runat="server" Text="Anulare" OnClick="BtnAnulareProgramare_Click" />

                        <asp:ImageButton ID="ibDataInceputPlataProgramata" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbDataInceputPlataProgramata_Click"/>                 
                    
                        <asp:Calendar ID="cldDataInceputPlataProgramata" runat="server" OnVisibleMonthChanged="CldDataInceputPlataProgramata_VisibleMonthChanged" OnSelectionChanged="CldDataInceputPlataProgramata_SelectionChanged"></asp:Calendar>

                        <asp:Image ID="imgBackgroundProgramare" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                    </div>

                    <div class="data_container" id="beneficiariPredefiniti" runat="server">

                        <asp:Label ID="lblTitluBeneficiariPredefiniti" runat="server" Text="Beneficiari predefiniți"></asp:Label>

                        <asp:Label ID="lblBeneficiariExistenti" runat="server" Text="BENEFICIARI EXISTENȚI"></asp:Label>

                        <asp:GridView ID="gvBeneficiariPredefiniti" runat="server" HeaderStyle-BackColor="#71838C" GridLines="None" BackColor="#EBF1F4" HeaderStyle-Height="25" HeaderStyle-ForeColor="White"  SelectedRowStyle-BackColor="#BFCFDB" OnSelectedIndexChanged="GvBeneficiariPredefiniti_SelectedIndexChanged" OnRowDataBound="GvBeneficiariPredefiniti_RowDataBound"></asp:GridView>

                        <asp:Button ID="btnAdaugareBeneficiarPredefinit" runat="server" Text="ADAUGĂ BENEFICIAR" OnClick="BtnAdaugareBeneficiarPredefinit_Click" />

                        <asp:Button ID="btnStergereBeneficiarPredefinit" runat="server" Text="ȘTERGE BENEFICIAR" OnClick="BtnStergereBeneficiarPredefinit_Click" />

                        <asp:Image ID="imgBackgroundBeneficiari" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                    </div>

                    <div class="data_container" id="adaugareBeneficiarPredefinit" runat="server">

                        <asp:Label ID="lblTitluAdaugareBeneficiar" runat="server" Text="Beneficiar nou"></asp:Label>

                        <asp:Label ID="lblDenumireBeneficiarNou" runat="server" Text="Denumire beneficiar"></asp:Label>

                        <asp:TextBox ID="txtDenumireBeneficiarNou" runat="server"></asp:TextBox>

                        <asp:Label ID="lblNumeBeneficiarNou" runat="server" Text="Nume beneficiar"></asp:Label>

                        <asp:TextBox ID="txtNumeBeneficiarNou" runat="server"></asp:TextBox>

                        <asp:Label ID="lblContBeneficiarNou" runat="server" Text="Cont beneficiar"></asp:Label>

                        <asp:TextBox ID="txtContBeneficiarNou" runat="server" AutoPostBack="true" OnTextChanged="TxtContBeneficiarNou_TextChanged"></asp:TextBox>

                        <asp:Label ID="lblValutaBeneficiarNou" runat="server" Text="Valută"></asp:Label>

                        <asp:RadioButtonList ID="rblValutaBeneficiarNou" runat="server"></asp:RadioButtonList>

                        <asp:Label ID="lblEroareBeneficiarPredefinit" runat="server" Text="Introduceți toate datele!"></asp:Label>

                        <asp:Button ID="btnAdaugareBeneficiarNou" runat="server" Text="Adaugă beneficiar" OnClick="BtnAdaugareBeneficiarNou_Click" />

                        <asp:Button ID="btnAnulareBeneficiarNou" runat="server" Text="Anulare" OnClick="BtnAnulareBeneficiarNou_Click" />

                        <asp:Image ID="imgBackgroundBeneficiarNou" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                    </div>

                    <div class="data_container" id="istoricTranzactiiCont" runat="server">

                        <asp:Label ID="lblIstoricTranzactiiCont" runat="server" Text="Istoric tranzacții"></asp:Label>

                        <asp:Label ID="lblCont" runat="server" Text="Cont"></asp:Label>

                        <asp:DropDownList ID="dlCont" runat="server"></asp:DropDownList>

                        <asp:RadioButtonList ID="rblPerioadaCont" runat="server" AutoPostBack="true" OnSelectedIndexChanged="RblPerioadaCont_SelectedIndexChanged" ></asp:RadioButtonList>

                        <asp:Label ID="lblDataInceputIstoricCont" runat="server" Text="Dată început"></asp:Label>

                        <asp:TextBox ID="txtDataInceputIstoricCont" runat="server"></asp:TextBox>

                        <asp:Label ID="lblLinieIstoricCont" runat="server" Text="━"></asp:Label>

                        <asp:Label ID="lblDataSfarsitIstoricCont" runat="server" Text="Dată sfârșit"></asp:Label>

                        <asp:TextBox ID="txtDataSfarsitIstoricCont" runat="server"></asp:TextBox>

                        <asp:Label ID="lblTipTranzactieIstoricCont" runat="server" Text="Tip tranzacție"></asp:Label>

                        <asp:DropDownList ID="dlTipTranzactieIstoricCont" runat="server"></asp:DropDownList>

                        <asp:Label ID="lblListaTranzactiiIstoricCont" runat="server" Text="LISTĂ TRANZACȚII"></asp:Label>

                        <asp:GridView ID="gvListaTranzactiiIstoricCont" runat="server" HeaderStyle-BackColor="#71838C" GridLines="None" BackColor="#EBF1F4" HeaderStyle-Height="25" HeaderStyle-ForeColor="White"  SelectedRowStyle-BackColor="#BFCFDB"></asp:GridView>

                        <asp:ImageButton ID="ibDataInceputIstoricCont" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbDataInceputIstoricCont_Click"/>

                        <asp:ImageButton ID="ibDataSfarsitIstoricCont" runat="server" ImageUrl="~/Media/Second_Page/Icons/calendar_icon.png" OnClick="IbDataSfarsitIstoricCont_Click" />

                        <asp:Calendar ID="cldDataInceputIstoricCont" runat="server" OnVisibleMonthChanged="CldDataInceputIstoricCont_VisibleMonthChanged" OnSelectionChanged="CldDataInceputIstoricCont_SelectionChanged"></asp:Calendar>

                        <asp:Calendar ID="cldDataSfarsitIstoricCont" runat="server" OnVisibleMonthChanged="CldDataSfarsitIstoricCont_VisibleMonthChanged" OnSelectionChanged="CldDataSfarsitIstoricCont_SelectionChanged"></asp:Calendar>

                        <asp:Button ID="btnVizualizareTranzactiiIstoricCont" runat="server" Text="Vizualizare" OnClick="BtnVizualizareTranzactiiIstoricCont_Click" />
                        
                        <asp:Label ID="lblEroareIstoricCont" runat="server" Text="Introduceți toate datele!"></asp:Label>

                        <asp:Button ID="btnExportPdfIstoricCont" runat="server" Text="Export PDF" OnClick="BtnExportPdfIstoricCont_Click" />

                    </div>

                    <div class="data_container" id="contractareImprumut" runat="server">

                        <asp:Label ID="lblTitluImprumut" runat="server" Text="Contractare împrumut"></asp:Label>

                        <asp:Label ID="lblTipImprumut" runat="server" Text="Tipul de împrumut"></asp:Label>

                        <asp:RadioButtonList ID="rblTipImprumut" runat="server"></asp:RadioButtonList>

                        <asp:Label ID="lblTipDobanda" runat="server" Text="Tipul de dobândă"></asp:Label>

                        <asp:RadioButtonList ID="rblTipDobanda" runat="server"></asp:RadioButtonList>
                        
                        <asp:Label ID="lblTipRata" runat="server" Text="Tipul de rată"></asp:Label>

                        <asp:RadioButtonList ID="rblTipRata" runat="server"></asp:RadioButtonList>

                        <asp:Label ID="lblDurataImprumut" runat="server" Text="Durată împrumut (luni)"></asp:Label>

                        <asp:TextBox ID="txtDurataImprumut" runat="server"></asp:TextBox>

                        <asp:Button ID="btnCresteDurata" runat="server" OnClick="BtnCresteDurata_Click"/>

                        <asp:Button ID="btnScadeDurata" runat="server" OnClick="BtnScadeDurata_Click"/>

                        <asp:Label ID="lblDataContractare" runat="server" Text="Data contractare"></asp:Label>
                        
                        <asp:TextBox ID="txtDataContractare" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataScadenta" runat="server" Text="Data scadenta"></asp:Label>

                        <asp:TextBox ID="txtDataScadenta" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblVenit" runat="server" Text="Venit"></asp:Label>

                        <asp:TextBox ID="txtVenit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblSumaMaxima" runat="server" Text="Sumă maximă"></asp:Label>

                        <asp:TextBox ID="txtSumaMaxima" runat="server"></asp:TextBox>

                        <asp:Label ID="lblSumaImprumut" runat="server" Text="Sumă împrumut"></asp:Label>

                        <asp:TextBox ID="txtSumaImprumut" runat="server"></asp:TextBox>

                        <asp:Label ID="lblAsigurareSanatate" runat="server" Text="Asigurare de sănătate:"></asp:Label>

                        <asp:RadioButtonList ID="rblAsigurareSanatate" runat="server"></asp:RadioButtonList>

                        <asp:Label ID="lblContPlatitor" runat="server" Text="Cont plătitor"></asp:Label>

                        <asp:TextBox ID="txtContPlatitor" runat="server"></asp:TextBox>

                        <asp:Label ID="lblEroareImprumut" runat="server" Text="Introduceți toate datele!"></asp:Label>

                        <asp:Label ID="lblEroareSuma" runat="server" Text="Sumă prea mare!"></asp:Label>

                        <asp:Button ID="btnContractareImprumut" runat="server" Text="Finalizare" OnClick="BtnContractareImprumut_Click"/>

                        <asp:Button ID="btnAnulareImprumut" runat="server" Text="Anulare" />

                        <asp:Button ID="btnScadentarImprumut" runat="server" Text="Vizualizare scadențar" OnClick="BtnScadentarImprumut_Click"/>

                        <asp:Image ID="imgBackgroundImprumut" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                    </div>

                    <div class="data_container" id="vizualizareImprumuturi" runat="server">

                        <asp:Label ID="lblVizualizareCredite" runat="server" Text="Vizualizare credite"></asp:Label>

                        <asp:Label ID="lblCrediteleMele" runat="server" Text="CREDITE"></asp:Label>

                        <asp:GridView ID="gvCredite" runat="server" HeaderStyle-BackColor="#71838C" GridLines="None" BackColor="#EBF1F4" HeaderStyle-Height="25" HeaderStyle-ForeColor="White"  SelectedRowStyle-BackColor="#BFCFDB" OnSelectedIndexChanged="GvCredite_SelectedIndexChanged" OnRowDataBound="GvCredite_RowDataBound" />

                        <asp:Button ID="btnDetaliiCreditContractat" runat="server" Text="Detalii credit" OnClick="BtnDetaliiCreditContractat_Click" />

                    </div>

                    <div class="data_container" id="detaliiImprumut" runat="server">

                        <asp:Label ID="lblDetaliiImprumut" runat="server" Text="Detalii împrumut"></asp:Label>

                        <asp:Label ID="lblIBANCredit" runat="server" Text="ID Credit:"></asp:Label>

                        <asp:TextBox ID="txtIBANCredit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblContPlatitorCredit" runat="server" Text="Cont plătitor:"></asp:Label>

                        <asp:TextBox ID="txtContPlatitorCredit" runat="server"></asp:TextBox>

                        <asp:Label ID="lblTipCredit" runat="server" Text="Tip credit:"></asp:Label>

                        <asp:TextBox ID="txtTipCredit" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblValoareCredit" runat="server" Text="Sumă împrumutată:"></asp:Label>

                        <asp:TextBox ID="txtValoareCredit" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblSumaRambursata" runat="server" Text="Sumă rambursată:"></asp:Label>

                        <asp:TextBox ID="txtSumaRambursata" runat="server"></asp:TextBox>
                        
                        <asp:Label ID="lblSumaNerambursata" runat="server" Text="Sumă nerambursată:"></asp:Label>

                        <asp:TextBox ID="txtSumaNerambursata" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataContractareDetalii" runat="server" Text="Dată contractare:"></asp:Label>

                        <asp:TextBox ID="txtDataContractareDetalii" runat="server"></asp:TextBox>

                        <asp:Label ID="lblDataScadentaDetalii" runat="server" Text="Dată scadență:"></asp:Label>

                        <asp:TextBox ID="txtDataScadentaDetalii" runat="server"></asp:TextBox>

                        <asp:Label ID="lblTipRataDetalii" runat="server" Text="Tip rată:"></asp:Label>

                        <asp:TextBox ID="txtTipRataDetalii" runat="server"></asp:TextBox>

                        <asp:Label ID="lblTipDobandaDetalii" runat="server" Text="Tip dobândă:"></asp:Label>

                        <asp:TextBox ID="txtTipDobandaDetalii" runat="server"></asp:TextBox>

                        <asp:Button ID="btnDetaliiCreditRevenire" runat="server" Text="Înapoi" OnClick="BtnDetaliiCreditRevenire_Click"/>

                        <asp:Image ID="imgDetaliiImprumut" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                    </div>

                    <div class="data_container" id="statisticiDate" runat="server">

                        <asp:Label ID="lblTitluStatistici" runat="server" Text="Vizualizare statistici"></asp:Label>

                        <asp:Label ID="lblAlegereStatistica" runat="server" Text="Opțiune statistică:"></asp:Label>

                        <asp:RadioButtonList ID="rblOptiuniStatistica" runat="server" AutoPostBack="true" OnSelectedIndexChanged="RblOptiuniStatistica_SelectedIndexChanged"></asp:RadioButtonList>

                        <asp:Label ID="lblPerioadaStatistica" runat="server" Text="Perioadă statistică:"></asp:Label>

                        <asp:RadioButtonList ID="rblPerioadaStatistica" runat="server" AutoPostBack="true" OnSelectedIndexChanged="RblPerioadaStatistica_SelectedIndexChanged"></asp:RadioButtonList>

                        <asp:TextBox ID="txtAnStatistica" runat="server"></asp:TextBox>

                        <asp:DropDownList ID="dlLunaStatistica" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DlLunaStatistica_SelectedIndexChanged"></asp:DropDownList>

                        <asp:Chart ID="diagramaStatistici" runat="server" Width="500">
                            <Series>
                                <asp:Series Name="series"></asp:Series>
                            </Series>
                            <Legends>
                                <asp:Legend Name="legend"></asp:Legend>
                            </Legends>
                            <ChartAreas>
                                <asp:ChartArea Name="chartArea">
                                    <Area3DStyle
                                         Enable3D="true"
                                         Perspective="2" 
                                         IsRightAngleAxes="False" 
                                         Inclination="8" 
                                         WallWidth="0" 
                                         IsClustered="False"
                                    />
                                </asp:ChartArea>
                            </ChartAreas>
                        </asp:Chart>

                        <asp:Chart ID="diagramaVenitCheltuieli" runat="server" Width="500">
                            <Series>
                                <asp:Series Name="series1"></asp:Series>
                                <asp:Series Name="series2"></asp:Series>
                            </Series>
                            <Legends>
                                <asp:Legend Name="legend1"></asp:Legend>
                            </Legends>
                            <ChartAreas>
                                <asp:ChartArea Name="chartArea">
                                    <Area3DStyle
                                         Enable3D="true"
                                         Perspective="2" 
                                         IsRightAngleAxes="False" 
                                         Inclination="8" 
                                         WallWidth="0" 
                                         IsClustered="False"
                                    />
                                </asp:ChartArea>
                            </ChartAreas>
                        </asp:Chart>

                        <asp:Image ID="imgBackgroundStatistici" ImageUrl="~/Media/Second_Page/div_background.png" runat="server" />

                    </div>

                </div>

            </div>

        </form>

    </div>

    <script src="../JavaScript/Profil.js"></script>

    <script src="../JavaScript/preventResubmission.js"></script>

</body>
</html>
