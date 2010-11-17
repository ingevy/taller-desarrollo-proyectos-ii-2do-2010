<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SelfManagement - Inicio
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="mainPanel" class="panel">
    <div class="innerPanel">
        <h2><span>Inicio</span></h2>
        
        <p><%: ViewData["WelcomeMessage"] %></p>
        <p><img src="content/images/selfmanagement_logo.png" alt="SelfManagement" /></p>
    </div>
</div>

</asp:Content>
