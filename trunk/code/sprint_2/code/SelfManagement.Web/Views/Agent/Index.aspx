<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CallCenter.SelfManagement.Web.ViewModels.AgentDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SelfManagement - Agentes
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="info">
        <span style="float: left;">
            <%= this.Model.DisplayName %></span> <span style="float: right;">
                <%: Html.LabelFor(model => model.CurrentSupervisor) %>
                <a href="#">
                    <%= this.Model.CurrentSupervisor %></a> </span>
    </h1>
    <%: Html.HiddenFor(model => model.AgentId) %>

    <div style="clear: both; height: 1px">
    </div>
    <div id="mainPanel" class="panel">
        <div class="innerPanel">
            <h2>
                <span>Liquidación de Sueldo</span></h2>
            <div class="content">
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Salary.GrossSalary) %>
                    <%: Html.TextBoxFor(model => model.Salary.GrossSalary, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="float: right;">
                    <%: Html.LabelFor(model => model.CurrentMonth) %>
                    <% 
                        var id = 0;
                        var availableMonths = this.Model.AvailableMonths.Select(am => new SelectListItem { Text = am, Value = (id++).ToString() });
                    %>
                    <%: Html.DropDownListFor(model => model.CurrentMonth, availableMonths, new { Class = "uservalue", onchange="refreshSalary()" })%>
                </div>
                <div style="clear: both; height: 1px">
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Salary.VariableSalary) %>
                    <%: Html.TextBoxFor(model => model.Salary.VariableSalary, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="float: right;">
                    <%: Html.LabelFor(model => model.Salary.TotalSalary) %>
                    <%: Html.TextBoxFor(model => model.Salary.TotalSalary, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="clear: both; height: 1px">
                </div>
            </div>
        </div>
    </div>
    <div id="secondPanel" class="panel">
        <div class="innerPanel">
            <h2>
                <span>Métricas de Campañas</span></h2>
            <div class="content">
                <div style="float: right;">
                    <%: Html.LabelFor(model => model.CurrentCampaingId) %>
                    <% 
                        var availableCampaings = this.Model.AgentCampaings.Select(ac => new SelectListItem { Text = ac.DisplayName, Value = ac.Id.ToString() });
                    %>
                    <%: Html.DropDownListFor(model => model.CurrentCampaingId, availableCampaings, new { onclick = "refreshMetricValues()" })%>
                </div>
                <div style="clear: both; height: 1px"></div>

                <div id="metricvaluescontainer" style="padding: 0px; margin: 20px 0px 0px 0px;" >
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
            </div>
        </div>
    </div>
</asp:Content>
