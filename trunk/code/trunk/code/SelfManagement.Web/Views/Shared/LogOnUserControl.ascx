<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<p>
<%
    if (Request.IsAuthenticated) {
%>
        <b><%: Page.User.Identity.Name %></b> (<%: Page.User.GetRole() %>) | 
        <%: Html.ActionLink("Cerrar Sesión", "LogOff", "Account", null, new { id = "loginLink" })%>
<%
    }
    else {
%>
        <%: Html.ActionLink("Iniciar Sesión", "LogOn", "Account", null, new { id = "loginLink" })%>
<%
    }
%>
</p>
