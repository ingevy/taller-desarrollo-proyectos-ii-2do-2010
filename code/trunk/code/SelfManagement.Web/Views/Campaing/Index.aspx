<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CallCenter.SelfManagement.Web.ViewModels.CampaingDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SelfManagement - Campañas
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h1 class="info">
        <span style="float: left;">
            <%= this.Model.DisplayName %></span>
            <span style="float: right;">
                <%: Html.ActionLink("Crear nueva Campaña", "Create", "Campaing", null, new { Class = "btn add", rel = "nofollow", title = "Crear nueva Campaña" }) %>
                <%: Html.ActionLink("Editar Campaña", "Edit", "Campaing", new { campaingId = this.Model.CampaingId }, new { Class = "btn edit", rel = "nofollow", title = "Editar Campaña" }) %>
                <%
                    if (this.Model.ShowEndCampaing)
                    {
                %>
                <%: Html.ActionLink("Terminar Campaña", "End", "Campaing", new { campaingId = this.Model.CampaingId, pageNumber = this.Model.PageNumber }, new { Class = "btn remove", rel = "nofollow", title = "Terminar Campaña", onclick = "return confirm('Los Agentes y Supervisores asignados a la campaña se liberarán.\n¿Realmente desea terminar la campaña el día de hoy?');" })%>
                <%
                    }
                %>

            </span>
    </h1>
    
    <%: Html.HiddenFor(model => model.CampaingId) %>

    <div style="clear: both; height: 1px"></div>
    
    <div id="mainPanel" class="panel">
        <div class="innerPanel">
            <h2>
                <span>Datos Generales</span></h2>
            <fieldset>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Customer) %>
                    <%: Html.TextBoxFor(model => model.Customer, new { Class = "uservalue", disabled = "true" }) %>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.CampaingType) %>
                    <%: Html.TextBoxFor(model => model.CampaingType, new { Class = "uservalue", disabled = "true" }) %>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.SupervisorsCount) %>
                    <%: Html.TextBoxFor(model => model.SupervisorsCount, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.BeginDate) %>
                    <%: Html.TextBoxFor(model => model.BeginDate, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.EndDate) %>
                    <%: Html.TextBoxFor(model => model.EndDate, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.AgentsCount) %>
                    <%: Html.TextBoxFor(model => model.AgentsCount, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.OptimalHourlyValue) %>
                    <%: Html.TextBoxFor(model => model.OptimalHourlyValue, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.ObjectiveHourlyValue) %>
                    <%: Html.TextBoxFor(model => model.ObjectiveHourlyValue, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.MinimumHourlyValue) %>
                    <%: Html.TextBoxFor(model => model.MinimumHourlyValue, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="clear: both; height: 1px">                
                </div>
                <div>
                    <%: Html.LabelFor(model => model.Description) %>
                    <%: Html.TextAreaFor(model => model.Description,  new { disabled = "true" }) %>
                </div>
            </fieldset>
        </div>
    </div>
    
    <div id="secondPanel" class="panel">
        <div class="innerPanel">
            <h2>
                <span>Métricas</span>
            </h2>
            <div class="content">
                 <h3>Resumen Actual de Métricas</h3>
                 <div id="metricvaluescontainer" style="padding: 0px; margin: 0px;" >
                    <div style="float: left;">
                         Nivel Actual: <span class="<%= this.Model.CurrentMetricLevel.GetCssClass() %>"><%= this.Model.CurrentMetricLevel.GetDescription() %></span>
                    </div>
                    <div style="float: right;">
                         Nivel Proyectado: <span class="<%= this.Model.ProjectedMetricLevel.GetCssClass() %>"><%= this.Model.ProjectedMetricLevel.GetDescription()%></span>
                    </div>
                    <div style="clear: both; height: 5px"></div>
                    <table cellpadding="0" cellspacing="0" id="metricvalues">
                        <tbody>
                            <tr>
                                <th>Metrica</th>
                                <th>Tipo</th>
                                <th>Nivel Optimo</th>
                                <th>Nivel Objectivo</th>
                                <th>Nivel Minimo</th>
                                <th>Valor Actual</th>
                                <th>Valor Proyectado</th>
                            </tr>
                            <% 
                                foreach (var metricValues in this.Model.MetricValues)
                                {
                            %>
                            <tr>
                                <td><%= metricValues.MetricName %></td>
                                <td><%= metricValues.Format %></td>
                                <td style="text-align: right;"><%= metricValues.OptimalValue %></td>
                                <td style="text-align: right;"><%= metricValues.ObjectiveValue %></td>
                                <td style="text-align: right;"><%= metricValues.MinimumValue %></td>
                                <td style="text-align: right;"><%= metricValues.CurrentValue %></td>
                                <td style="text-align: right;"><%= metricValues.ProjectedValue %></td>
                            </tr>
                            <% 
                                }
                            %>
                        </tbody>
                    </table>
                 </div>
                 <hr style="background-color:#C2C2C2;border:none;display:block;margin-bottom:0px;margin-top:15px;padding:0px;" />                 
                 <h3>
                    <span>Gráficos</span>
                    <% 
                        var metricMonthIndex = 0;
                        var availableMetricMonths = this.Model.AvailableMetricMonths.Select(am => new SelectListItem { Text = am, Value = (metricMonthIndex++).ToString() });
                    %>
                    <%: Html.DropDownListFor(model => model.CurrentMetricMonthIndex, availableMetricMonths, new { Class = "uservalue", onchange = "refreshMetricCharts()", style = "float:right;" })%>
                 </h3>
                 <div id="metricchartscontainer" style="padding: 0px; margin: 0px;">
                    <%
                        foreach (var metricValues in this.Model.MetricValues)
                        {
                    %>
                        <img src="<%= this.Url.Action("MetricsChart", "Campaing", new { campaingId = metricValues.CampaingId, metricId = metricValues.MetricId, month = this.Model.AvailableMetricMonths[this.Model.CurrentMetricMonthIndex] }) %>" alt="<%= metricValues.MetricName %>" width="952" height="350" />
                    <%
                        }
                    %>
                </div>
            </div>
        </div>
    </div>

    <div id="pager">
        <ul>
            <%
               if (this.Model.PageNumber > 1)
               {
            %>
                <li> <%: Html.ActionLink("< ", "Index", "Campaing", new { pageNumber = this.Model.PageNumber - 1 }, new { title = "Página Anterior" }) %></li>
            <%
               }
            %>
                <li> <%: Html.ActionLink(string.Format("{0}/{1}", this.Model.PageNumber, this.Model.TotalPages), "Index", "Campaing", new { pageNumber = this.Model.PageNumber }, new { title = "Página Actual" })%></li>
            <%
               if (this.Model.PageNumber < this.Model.TotalPages)
               {
            %>
                <li> <%: Html.ActionLink(" >", "Index", "Campaing", new { pageNumber = this.Model.PageNumber + 1 }, new { title = "Página Siguiente" })%></li>
            <%
               }
            %>
        </ul>
    </div>

</asp:Content>

