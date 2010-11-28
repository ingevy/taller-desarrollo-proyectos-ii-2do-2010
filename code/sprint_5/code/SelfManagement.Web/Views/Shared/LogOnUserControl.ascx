<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<p>
<%
    if (Request.IsAuthenticated) {
%>
        <b><%: Page.User.Identity.Name %></b> (<%: Page.User.GetRole() %>) | 
        <%: Html.ActionLink("Contraseña", "ChangePassword", "Account", null, new { id = "changePasswordLink" })%> | 
        <%: Html.ActionLink("Salir", "LogOff", "Account", null, new { id = "loginLink" })%>
<%
    }
    else
    {
%>
        <%: Html.ActionLink("Iniciar Sesión", "LogOn", "Account", null, new { id = "loginLink" })%>
<%
    }
%>
</p>
