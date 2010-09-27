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

    <% using (Html.BeginForm()) {%>
    <%: Html.ValidationSummary(true) %>

    <div id="mainPanel" class="panel">
    <div class="innerPanel">
        <h2><span>Datos Generales</span></h2>
        
        <fieldset>         
            <div style="float: left;">
                <label for="Name">Nombre</label>
                <%: Html.TextBoxFor(model => model.Name, new { Class = "uservalue"})%>
                <%: Html.ValidationMessageFor(model => model.Name) %>
            </div>
             
            <div style="float: left;">
                <label for="BeginDate">Fecha de Inicio</label>
                <%: Html.TextBoxFor(model => model.BeginDate, new { Class = "uservalue", datepicker = "true", datepicker_format = "DD/MM/YYYY" })%>
                <%: Html.ValidationMessageFor(model => model.BeginDate) %>
            </div>
            
            <div style="float: left;">
                <label for="SupervisorId">Supervisor</label>
                <%
                    var supervisors = this.Model.Supervisors.Select(s => new SelectListItem { Text = s.DisplayName, Value = s.Id.ToString() });
                    if (supervisors.Count() > 0)
                    {
                        supervisors.FirstOrDefault().Selected = true;
                    }
                %>
                <%: Html.DropDownListFor(model => model.SupervisorId, supervisors, new { Class = "uservalue" })%>
                <%: Html.ValidationMessageFor(model => model.SupervisorId) %>
            </div>

            <div style="float: left;">
                <label for="CustomerName">Cliente</label>
                <%: Html.TextBoxFor(model => model.CustomerName, new { Class = "uservalue"})%>
                <%: Html.ValidationMessageFor(model => model.CustomerName)%>
            </div>

            <div style="float: left;">            
                <label for="EndDate">Fecha de Fin</label>
                <%: Html.TextBoxFor(model => model.EndDate, new { Class = "uservalue", datepicker = "true", datepicker_format = "DD/MM/YYYY" })%>
                <%: Html.ValidationMessageFor(model => model.EndDate) %>
            </div>

            <div style="float: left;">
                <label for="CampaingType">Tipo</label>
                <%
                    var campaingTypes = new List<SelectListItem> { new SelectListItem { Text = "De Entrada", Value = "0", Selected = true}, new SelectListItem { Text = "De Salida", Value = "1" } };
                %>
                <%: Html.DropDownListFor(model => model.CampaingType, campaingTypes, new { Class = "uservalue" })%>
                <%: Html.ValidationMessageFor(model => model.CampaingType) %>
            </div>

            <div style="clear: both; height:1px"></div>
            
            <div>
                <label for="Description">Descripción</label>
                <%: Html.TextAreaFor(model => model.Description)%>
                <%: Html.ValidationMessageFor(model => model.Description) %>
            </div>

        </fieldset>
    </div>
    </div>     
    
    <hr />

    <div class="dual">

    <div class="panel" id="availablemetrics">
    <div class="innerPanel">
        <h2><span id="itineraryName">Metricas Disponibles</span></h2>
        <div class="" style="display: block;" id="itineraryDynamic">
        <div class="items">
                <ul style="height: 315px;" class="activities ui-sortable" id="itineraryDynamicList">
                    <%
                       foreach (var metric in this.Model.Metrics)
                       {
                    %>
                    <li class="ui-state-default">
                        <span class="off id"><%= metric.Id %></span>
                        <h3><a href="#"><%= metric.Name %></a></h3>
                        <p><%= metric.Description %></p>
                        <input type="hidden" value="<%= metric.FormatType %>" />
                        <p class="options">
                            <a class="btn add" title="Agregar Métrica" href="#">Agregar Métrica</a>
                        </p>
                    </li>
                    <% } %>
                </ul>
            </div>
         </div>
         
        <script type="text/javascript">document.getElementById("itineraryStatic").style.display = 'none'</script>
        <script type="text/javascript">document.getElementById("itineraryDynamic").style.display = 'block'</script>
        <div class="toolbox"></div>
        
    </div>
</div>

    <div class="panel" id="campaingmetrics">
    <div class="innerPanel">
        <h2><span id="Span1">Metricas de la Campaña</span></h2>
        <div class="" style="display: block;" id="Div2">
        <div class="items">
                <ul style="height: 315px;" class="activities ui-sortable" id="Ul1">
                    <%
                       var index = 1;
                       foreach (var campaingMetric in this.Model.CampaingMetrics)
                       {
                    %>
                    <li class="ui-state-default">
                        <%= Html.Hidden("CampaingMetrics[" + (index - 1) + "].MetricId", campaingMetric.MetricId)%>
                        <h3><a href="#"><%= campaingMetric.Name%></a></h3>
                        <p><%= campaingMetric.Description%></p>
                        <%= Html.Hidden("CampaingMetrics[" + (index - 1) + "].FormatType", campaingMetric.FormatType)%>
                        <div class="inline estimatedMinutes">
                            Optimo: <%= Html.TextBox("CampaingMetrics[" + (index - 1) + "].OptimalLevel", campaingMetric.OptimalLevel, new { size = "3" })%>
                            &nbsp;&nbsp;&nbsp;
                            Objetivo: <%= Html.TextBox("CampaingMetrics[" + (index - 1) + "].ObjectiveLevel", campaingMetric.ObjectiveLevel, new { size = "3" })%>
                            &nbsp;&nbsp;&nbsp;
                            Mínimo: <%= Html.TextBox("CampaingMetrics[" + (index - 1) + "].MinimumLevel", campaingMetric.MinimumLevel, new { size = "3" })%>
                        </div>
                        <p class="options">
                            <a class="btn remove" title="Remover Métrica" href="#">Remover Métrica</a>
                        </p>
                    </li>
                    <%
                           index++;
                       } %>
                </ul>
            </div>
         </div>
         
        <script type="text/javascript">            document.getElementById("itineraryStatic").style.display = 'none'</script>
        <script type="text/javascript">            document.getElementById("itineraryDynamic").style.display = 'block'</script>
        <div class="toolbox"></div>
        
    </div>
</div>

    </div>
    
    <div id="buttonsPanel">
        <input type="submit" value="Guardar" />
        <input type="reset" value="Cancelar" />
    </div>
    <% } %>
</asp:Content>

