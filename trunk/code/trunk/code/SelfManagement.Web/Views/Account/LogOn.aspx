<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CallCenter.SelfManagement.Web.ViewModels.LogOnViewModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Iniciar Sesión
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dual">
        <div class="panel" id="campaingMetrics">
            <div class="innerPanel">
                <h2>
                    <span id="itineraryName">Iniciar Sesión</span>
                    <%: Html.ValidationMessage("LogOnViewModel")%>
                </h2>
                <div class="" id="itineraryDynamic">
                    <div class="items">
                        <% using (Html.BeginForm())
                           { %>
                        <div>
                            <fieldset>
                                <div class="editor-label">
                                    <%: Html.LabelFor(m => m.UserName) %>
                                </div>
                                <div class="editor-field">
                                    <%: Html.TextBoxFor(m => m.UserName) %>
                                    <%: Html.ValidationMessageFor(m => m.UserName) %>
                                </div>
                                <div class="editor-label">
                                    <%: Html.LabelFor(m => m.Password) %>
                                </div>
                                <div class="editor-field">
                                    <%: Html.PasswordFor(m => m.Password) %>
                                    <%: Html.ValidationMessageFor(m => m.Password) %>
                                </div>
                                <div class="editor-label">
                                    <%: Html.CheckBoxFor(m => m.RememberMe) %>
                                    <%: Html.LabelFor(m => m.RememberMe) %>
                                </div>
                                <p>
                                    <input type="submit" value="Iniciar Sesión" />
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
