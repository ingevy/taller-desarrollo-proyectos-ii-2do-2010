<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CallCenter.SelfManagement.Web.ViewModels.UserViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	SelfManagement - Crear Usuario
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% using (Html.BeginForm())
       {
    %>
    <div id="mainPanel" class="panel">
        <div class="innerPanel">
            <h2>
                <span>Datos del Usuario</span>
                <%: Html.ValidationMessageFor(model => model.GlobalError) %>
            </h2>
            <div class="content2">
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Id) %>
                    <%: Html.TextBoxFor(model => model.Id, new { Class = "uservalue" }) %>
                    <%: Html.ValidationMessageFor(model => model.Id) %>
                </div>
                <div style="float: right;">
                    <%: Html.LabelFor(model => model.Password) %>
                    <%: Html.PasswordFor(model => model.Password, new { Class = "uservalue" }) %>
                    <%: Html.ValidationMessageFor(model => model.Password) %>
                </div>
                <div style="clear: both; height: 1px"></div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Username) %>
                    <%: Html.TextBoxFor(model => model.Username, new { Class = "uservalue" }) %>
                    <%: Html.ValidationMessageFor(model => model.Username)%>
                </div>
                <div style="float: right;">
                    <%: Html.LabelFor(model => model.ConfirmPassword) %>
                    <%: Html.PasswordFor(model => model.ConfirmPassword, new { Class = "uservalue" })%>
                    <%: Html.ValidationMessageFor(model => model.ConfirmPassword) %>
                </div>
                <div style="clear: both; height: 1px"></div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Role) %>
                    <% 
                        var index = 0;
                        var availableRoles = ExtensionMethodHelper.GetRoles().Select(r => new SelectListItem { Text = r, Value = (index++).ToString() });
                    %>
                    <%: Html.DropDownListFor(model => model.Role, availableRoles, new { Class = "uservalue" }) %>
                    <%: Html.ValidationMessageFor(model => model.Role) %>
                </div>
                <div style="clear: both; height: 1px"></div>
            </div>
        </div>
    </div>

    <div id="secondPanel" class="panel">
        <div class="innerPanel">
            <h2>
                <span>Perfil</span>
            </h2>
            <div class="content2">
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Names) %>
                    <%: Html.TextBoxFor(model => model.Names, new { Class = "uservalue" }) %>
                    <%: Html.ValidationMessageFor(model => model.Names)%>
                </div>
                <div style="float: right;">
                    <%: Html.LabelFor(model => model.LastName) %>
                    <%: Html.TextBoxFor(model => model.LastName, new { Class = "uservalue" })%>
                    <%: Html.ValidationMessageFor(model => model.LastName)%>
                </div>
                <div style="clear: both; height: 1px"></div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Dni) %>
                    <%: Html.TextBoxFor(model => model.Dni, new { Class = "uservalue" }) %>
                    <%: Html.ValidationMessageFor(model => model.Dni) %>
                </div>
                <div style="float: right;">
                    <%: Html.LabelFor(model => model.Email) %>
                    <%: Html.TextBoxFor(model => model.Email, new { Class = "uservalue" }) %>
                    <%: Html.ValidationMessageFor(model => model.Email) %>
                </div>
                <div style="clear: both; height: 1px"></div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.GrossSalary) %>
                    <%: Html.TextBoxFor(model => model.GrossSalary, new { Class = "uservalue" }) %>
                    <%: Html.ValidationMessageFor(model => model.GrossSalary) %>
                    <%: Html.ValidationMessageFor(model => model.GrossSalaryRequired) %>
                </div>
                <div style="float: right;">
                    <%: Html.LabelFor(model => model.Workday) %>
                    <% 
                        var availableWorkdays = new List<SelectListItem> { new SelectListItem { Text = "PTE", Value = "0" }, new SelectListItem { Text = "FTE", Value = "1" } };
                    %>
                    <%: Html.DropDownListFor(model => model.Workday, availableWorkdays, new { Class = "uservalue" }) %>
                    <%: Html.ValidationMessageFor(model => model.Workday) %>
                </div>
                <div style="clear: both; height: 1px"></div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Status) %>
                    <% 
                        var availableStatus = new List<SelectListItem> { new SelectListItem { Text = "Activo", Value = "0" }, new SelectListItem { Text = "Inactivo", Value = "1" } };                        
                    %>
                    <%: Html.DropDownListFor(model => model.Status, availableStatus, new { Class = "uservalue" })%>
                    <%: Html.ValidationMessageFor(model => model.Status) %>
                </div>
                <div style="clear: both; height: 1px"></div>
            </div>
        </div>
    </div>

    <div id="buttonsPanel">
        <input type="submit" value="Guardar" />
        <input type="reset" value="Cancelar" />
    </div>

    <% } %>
</asp:Content>
