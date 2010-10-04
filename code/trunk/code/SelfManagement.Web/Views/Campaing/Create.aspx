<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CallCenter.SelfManagement.Web.ViewModels.CampaingViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Create
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("input#CustomerName").autocomplete('<%= Url.Action("FindCustomer", "Campaing") %>', { loadingClass: "acLoading" });
        }); 
    </script>
    <!-- Español -->
    <input type="hidden" id="DPC_TODAY_TEXT" value="Hoy" />
    <input type="hidden" id="DPC_BUTTON_TITLE" value="Abric calendario..." />
    <input type="hidden" id="DPC_MONTH_NAMES" value="['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre']" />
    <input type="hidden" id="DPC_DAY_NAMES" value="['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sa']" />
    <!-- Español -->
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true)%>
    <div id="mainPanel" class="panel">
        <div class="innerPanel">
            <h2>
                <span>Datos Generales</span></h2>
            <fieldset>
                <div style="float: left;">
                    <label for="Name">
                        Nombre</label>
                    <%: Html.TextBoxFor(model => model.Name, new { Class = "uservalue" })%>
                    <%: Html.ValidationMessageFor(model => model.Name)%>
                </div>
                <div style="float: left;">
                    <label for="CampaingType">
                        Tipo</label>
                    <%
                        var campaingTypes = new List<SelectListItem> { new SelectListItem { Text = "De Entrada", Value = "0", Selected = true }, new SelectListItem { Text = "De Salida", Value = "1" } };
                    %>
                    <%: Html.DropDownListFor(model => model.CampaingType, campaingTypes, new { Class = "uservalue" })%>
                    <%: Html.ValidationMessageFor(model => model.CampaingType)%>
                </div>
                <div style="float: left;">
                    <label for="CustomerName">
                        Cliente</label>
                    <%: Html.TextBoxFor(model => model.CustomerName, new { Class = "uservalue" })%>
                    <%: Html.ValidationMessageFor(model => model.CustomerName)%>
                </div>
                <div style="float: left;">
                    <label for="BeginDate">
                        Fecha de Inicio</label>
                    <%: Html.TextBoxFor(model => model.BeginDate, new { Class = "uservalue", datepicker = "true", datepicker_format = "DD/MM/YYYY" })%>
                    <%: Html.ValidationMessageFor(model => model.BeginDate)%>
                </div>
                <div style="float: left;">
                    <label for="EndDate">
                        Fecha de Fin</label>
                    <%: Html.TextBoxFor(model => model.EndDate, new { Class = "uservalue", datepicker = "true", datepicker_format = "DD/MM/YYYY" })%>
                    <%: Html.ValidationMessageFor(model => model.EndDate)%>
                </div>
                <div style="clear: both; height: 1px">
                </div>
                <div>
                    <label for="Description">
                        Descripción</label>
                    <%: Html.TextAreaFor(model => model.Description)%>
                    <%: Html.ValidationMessageFor(model => model.Description)%>
                </div>
            </fieldset>
        </div>
    </div>
    <hr />
    <div class="dual">
        <div class="panel" id="campaingMetrics">
            <div class="innerPanel">
                <h2>
                    <span id="itineraryName">Metricas</span>
                    <%: Html.ValidationMessageFor(model => model.CampaingMetricLevelsCount)%>
                </h2>
                <div class="" id="itineraryDynamic">
                    <div class="items">

                        <ul class="campaingElement ui-sortable" id="metricLevelsList">
                            <%
                                for (var index = 0; index < this.Model.CampaingMetricLevels.Count(); index++)
                                {
                            %>
                            <li class="ui-state-default"><span class="off id">
                                <%: Html.HiddenFor(model => this.Model.CampaingMetricLevels[index].Id)%></span>
                                <h3>
                                    <a href="#">
                                        <%= this.Model.CampaingMetricLevels[index].Name %></a></h3>
                                <p>
                                    <%= this.Model.CampaingMetricLevels[index].Description %></p>
                                <%: Html.HiddenFor(model => this.Model.CampaingMetricLevels[index].Name)%>
                                <%: Html.HiddenFor(model => this.Model.CampaingMetricLevels[index].Description)%>
                                <%: Html.HiddenFor(model => this.Model.CampaingMetricLevels[index].FormatType)%>
                                <%: Html.HiddenFor(model => this.Model.CampaingMetricLevels[index].IsHighestToLowest)%>
                                <div style="height: 12px;">
                                </div>
                                <div class="inline metricLevels" id="<%= this.Model.CampaingMetricLevels[index].Id %>"
                                    style="<%= this.Model.CampaingMetricLevels[index].Selected ? string.Empty: "display: none;" %>">
                                    Optimo:
                                    <%: Html.TextBoxFor(model => this.Model.CampaingMetricLevels[index].OptimalLevel, new { size = "4" })%>
                                    <% if (this.Model.CampaingMetricLevels[index].Selected)
                                       {
                                    %>
                                    <%: Html.ValidationMessageFor(model => this.Model.CampaingMetricLevels[index].OptimalLevel)%>
                                    <% } %>
                                    &nbsp;&nbsp;&nbsp;&nbsp; Objetivo:
                                    <%: Html.TextBoxFor(model => this.Model.CampaingMetricLevels[index].ObjectiveLevel, new { size = "4" })%>
                                    <% if (this.Model.CampaingMetricLevels[index].Selected)
                                       {
                                    %>
                                    <%: Html.ValidationMessageFor(model => this.Model.CampaingMetricLevels[index].ObjectiveLevel)%>
                                    <% } %>
                                    &nbsp;&nbsp;&nbsp;&nbsp; Minimo:
                                    <%: Html.TextBoxFor(model => this.Model.CampaingMetricLevels[index].MinimumLevel, new { size = "4" })%>
                                    <% if (this.Model.CampaingMetricLevels[index].Selected)
                                       {
                                    %>
                                    <%: Html.ValidationMessageFor(model => this.Model.CampaingMetricLevels[index].MinimumLevel)%>
                                    <% } %>
                                </div>
                                <p class="options">
                                    <% var metricId = this.Model.CampaingMetricLevels[index].Id; %>
                                    <%: Html.CheckBoxFor(model => this.Model.CampaingMetricLevels[index].Selected, new { onclick = "showOrHideMetricLevels(event, '" + metricId +"')" })%>
                                </p>
                            </li>
                            <%  } %>
                        </ul>
                    </div>
                </div>
                <div class="toolbox">
                </div>
            </div>
        </div>
        <div class="panel" id="campaingSupervisors">
            <div class="innerPanel">
                <h2>
                    <span id="Span1">Supervisores</span>
                    <%: Html.ValidationMessageFor(model => model.CampaingSupervisorsCount)%>
                </h2>
                <div class="">
                    <div class="items">
                    <%
                        if (this.Model.CampaingSupervisors.Count() == 0)
                        {
                    %>
                        <h3>No hay supervisores disponibles en el rango de fechas elegido...</h3>
                    <%
                        }
                        else
                        {
                    %>
                        <ul class="campaingElement ui-sortable" id="supervisorsList">
                            <%
                                for (var index = 0; index < this.Model.CampaingSupervisors.Count(); index++)
                                {
                            %>
                            <li class="ui-state-default"><span class="off id">
                                <%: Html.HiddenFor(model => this.Model.CampaingSupervisors[index].Id) %></span>
                                <h3>
                                    <a href="#">
                                        <%= this.Model.CampaingSupervisors[index].DisplayName %></a></h3>
                                <%: Html.HiddenFor(model => this.Model.CampaingSupervisors[index].DisplayName )%>
                                <p class="options">
                                    <%: Html.CheckBoxFor(model => this.Model.CampaingSupervisors[index].Selected, new { onclick = "" }) %>
                                </p>
                            </li>
                            <%  } %>
                        </ul>
                    <%  } %>
                    </div>
                </div>
                <div class="toolbox">
                </div>
            </div>
        </div>
    </div>
    <div id="buttonsPanel">
        <%
            if (this.Model.CampaingSupervisors.Count() == 0)
            {
        %>
        <input type="submit" value="Guardar" disabled="disabled" />
        <%
            }
            else
            {
        %>
        <input type="submit" value="Guardar" />
        <%
            }
        %>
        <input type="reset" value="Cancelar" />
    </div>
    <% } %>
</asp:Content>
