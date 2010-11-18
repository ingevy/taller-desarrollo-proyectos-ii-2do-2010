<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CallCenter.SelfManagement.Web.ViewModels.SupervisorDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SelfManagement - Supervisors
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="search">
        <%= this.Html.TextBoxFor(model => model.SearchCriteria, new { onkeypress = "searchSupervisorsKeyPressed(this, event)" })%>
        <input type="submit" value="Buscar" onclick="redirectSearchSupervisors();" />
    </div>

    <% 
        if (this.Model.ShouldPaginate)
        {
    %>
        <div class="pager" style="float:right">
            <span>Páginas</span>
            <ul>
             <% if (this.Model.PageNumber > 1)
                {
             %>
             <li> <%: Html.ActionLink("< ", "Search", "Supervisor", new { pageNumber = this.Model.PageNumber - 1, criteria = this.Model.SearchCriteria }, new { title = "Página Anterior" })%></li>
             <%
                }
             %>             

             <li> <%: Html.ActionLink(string.Format("{0}/{1}", this.Model.PageNumber, this.Model.TotalPages), "Search", "Supervisor", new { pageNumber = this.Model.PageNumber, criteria = this.Model.SearchCriteria }, new { title = "Página Actual" })%></li>

             <%
                if (this.Model.PageNumber < this.Model.TotalPages)
                {
             %>
             <li> <%: Html.ActionLink(" >", "Search", "Supervisor", new { pageNumber = this.Model.PageNumber + 1, criteria = this.Model.SearchCriteria }, new { title = "Página Siguiente" })%></li>
             <%
                }
             %>
            </ul>
        </div>
    <% 
        }
    %>

    <% 
        if (this.User.IsInRole("AccountManager"))
        {
    %>
    <div style="clear: both; height: 1px"></div>
    <% 
        }
    %>

    <h1 class="info">
        <span style="float: left;">
            <%= this.Model.DisplayName %></span>
        <span style="float: right;">
            <%: Html.LabelFor(model => model.AgentsCount) %>
            <%: Html.ActionLink(this.Model.AgentsCount, "Index", "Agent", new { supervisorId = this.Model.SupervisorId }, new { Class = "uservalue", style = "display:inline-block;" })%>
        </span>
    </h1>

    <%: Html.HiddenFor(model => model.SupervisorId) %>

    <div style="clear: both; height: 1px"></div>

    <% 
        if (this.Model.CurrentCampaingId != 0)
        {
    %>
    <div id="secondPanel" class="panel">
        <div class="innerPanel">
            <h2>
                <span>Métricas de Campañas</span>
                <% 
                    var availableCampaings = this.Model.SupervisorCampaings.Select(ac => new SelectListItem { Text = ac.DisplayName, Value = ac.Id.ToString() });
                %>
                <%: Html.DropDownListFor(model => model.CurrentCampaingId, availableCampaings, new { onchange = "refreshSupervisorMetricValues()", style = "float:right; margin-right:15px;" })%>
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
                                foreach (var metricValues in this.Model.CurrentCampaingMetricValues)
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
                        foreach (var metricValues in this.Model.CurrentCampaingMetricValues)
                        {
                    %>
                        <img src="<%= this.Url.Action("MetricsChart", "Supervisor", new { innerUserId = this.Model.SupervisorId, campaingId = metricValues.CampaingId, metricId = metricValues.MetricId, month = this.Model.AvailableMetricMonths[this.Model.CurrentMetricMonthIndex] }) %>" alt="<%= metricValues.MetricName %>" width="952" height="350" />
                    <%
                        }
                    %>
                </div>
            </div>
        </div>
    </div>
    <% 
        }
        else
        {
    %>
    <h1 class="info" style="margin: 30px 10px 10px 10px; text-align: center">
        <span>No se encontraron campañas para el Supervisor</span>
    </h1>
    <% 
        }
    %>

    <% 
        if (this.Model.ShouldPaginate)
        {
    %>
        <div class="pager">
            <span>Páginas</span>
            <ul>
             <% if (this.Model.PageNumber > 1)
                {
             %>
             <li> <%: Html.ActionLink("< ", "Search", "Supervisor", new { pageNumber = this.Model.PageNumber - 1, criteria = this.Model.SearchCriteria }, new { title = "Página Anterior" })%></li>
             <%
                }
             %>             

             <li> <%: Html.ActionLink(string.Format("{0}/{1}", this.Model.PageNumber, this.Model.TotalPages), "Search", "Supervisor", new { pageNumber = this.Model.PageNumber, criteria = this.Model.SearchCriteria }, new { title = "Página Actual" })%></li>

             <%
                if (this.Model.PageNumber < this.Model.TotalPages)
                {
             %>
             <li> <%: Html.ActionLink(" >", "Search", "Supervisor", new { pageNumber = this.Model.PageNumber + 1, criteria = this.Model.SearchCriteria }, new { title = "Página Siguiente" })%></li>
             <%
                }
             %>
            </ul>
        </div>
    <% 
        }
    %>

</asp:Content>
