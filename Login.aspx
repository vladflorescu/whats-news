<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="LogIn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div class="ugly-login-panel">
    <asp:LoginView ID="LWLoginPanel" runat="server">
      <AnonymousTemplate>
        <asp:Login ID="Login1" runat="server" DestinationPageUrl="~/Index.aspx"></asp:Login>
      </AnonymousTemplate>
      <LoggedInTemplate>
        <div class="u-margin-Tm">
          <asp:Literal ID="Literal1" runat="server" Text="You are already connected to the website. "></asp:Literal>
          <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Index.aspx">Go to the main page.</asp:HyperLink>
        </div>
      </LoggedInTemplate>
    </asp:LoginView>
  </div>
</asp:Content>

