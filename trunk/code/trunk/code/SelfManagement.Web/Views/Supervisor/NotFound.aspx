<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CallCenter.SelfManagement.Web.ViewModels.SupervisorDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SelfManagement - Supervisors
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <% 
        if (string.IsNullOrWhiteSpace(this.Model.SearchCriteria) || this.User.IsInRole("AccountManager"))
        {
    %>
    <div id="search">
        <%= this.Html.TextBoxFor(model => model.SearchCriteria, new { onkeypress = "searchSupervisorsKeyPressed(this, event)" })%>
        <input type="submit" value="Buscar" onclick="redirectSearchSupervisors();" />
    </div>
    <%
        }
    %>

    <div style="clear: both; height: 1px"></div>

    <h1 class="info" style="margin: 10px;">
        <span>No se encontraron resultados</span>
    </h1>
</asp:Content>
