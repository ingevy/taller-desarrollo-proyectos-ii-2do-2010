<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CallCenter.SelfManagement.Web.ViewModels.CampaingDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SelfManagement - Campañas
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="search">
        <%= this.Html.TextBoxFor(model => model.SearchCriteria, new { onkeypress = "searchCampaingsKeyPressed(this, event)" }) %>
        <input type="submit" value="Buscar" onclick="redirectSearchCampaings();" />
    </div>

    <div style="clear: both; height: 1px"></div>

    <h1 class="info" style="margin: 10px;">
        <span>No se encontraron resultados</span>
    </h1>
</asp:Content>
