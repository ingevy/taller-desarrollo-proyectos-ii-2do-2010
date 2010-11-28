<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CallCenter.SelfManagement.Web.ViewModels.ChangePasswordViewModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    SelfManagement - Cambiar Contraseña
</asp:Content>

<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dual">
        <div class="panel" id="campaingMetrics">
            <div class="innerPanel">
                <h2>
                    <span id="itineraryName">Cambiar Contraseña</span>
                    <%: Html.ValidationMessage("ChangePasswordViewModel")%>
                    <%: Html.ValidationMessage("")%>
                </h2>
                <div class="" id="itineraryDynamic">
                    <div class="items">
                        <p style="margin-top: 1px; margin-bottom: 4px;">
                            La contraseña debe tener como mínimo <%: ViewData["PasswordLength"]%> caracteres.
                        </p>
                        <% using (Html.BeginForm())
                           { %>
                        <div>
                            <fieldset>
                                <div class="editor-label">
                                    <%: Html.LabelFor(m => m.OldPassword)%>
                                </div>
                                <div class="editor-field">
                                    <%: Html.PasswordFor(m => m.OldPassword)%>
                                    <%: Html.ValidationMessageFor(m => m.OldPassword)%>
                                </div>
                                <div class="editor-label">
                                    <%: Html.LabelFor(m => m.NewPassword)%>
                                </div>
                                <div class="editor-field">
                                    <%: Html.PasswordFor(m => m.NewPassword)%>
                                    <%: Html.ValidationMessageFor(m => m.NewPassword)%>
                                </div>
                                <div class="editor-label">
                                    <%: Html.LabelFor(m => m.ConfirmPassword)%>
                                </div>
                                <div class="editor-field">
                                    <%: Html.PasswordFor(m => m.ConfirmPassword)%>
                                    <%: Html.ValidationMessageFor(m => m.ConfirmPassword)%>
                                </div>
                                <p>
                                    <input type="submit" value="Cambiar Contraseña" />
                                </p>
                            </fieldset>
                        </div>
                        <% } %>
                    </div>
                </div>
                <div class="toolbox">
                </div>
            </div>
        </div>
    </div>
</asp:Content>
