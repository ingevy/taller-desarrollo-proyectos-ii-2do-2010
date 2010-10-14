<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <br />
    <%: Html.ActionLink("Crear nueva Campaña", "Create", "Campaing", null, new { Class = "btn add", rel = "nofollow", title = "Crear nueva Campaña" }) %>
    <br />
    <br />
    <h2>TODO</h2>

</asp:Content>
