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
                <div style="float: right;">
                    <%: Html.LabelFor(model => model.Salary.GrossSalary) %>
                    <%: Html.TextBoxFor(model => model.Salary.GrossSalary, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.CurrentSalaryMonthIndex)%>
                    <% 
                        var salaryMonthIndex = 0;
                        var availableSalaryMonths = this.Model.AvailableSalaryMonths.Select(am => new SelectListItem { Text = am, Value = (salaryMonthIndex++).ToString() });
                    %>
                    <%: Html.DropDownListFor(model => model.CurrentSalaryMonthIndex, availableSalaryMonths, new { Class = "uservalue", onchange = "refreshSalary()" })%>
                </div>
                <div style="clear: both; height: 1px"></div>
                <div style="float: right;">
                    <%: Html.LabelFor(model => model.Salary.VariableSalary) %>
                    <%: Html.TextBoxFor(model => model.Salary.VariableSalary, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Salary.TotalHoursWorked) %>
                    <%: Html.TextBoxFor(model => model.Salary.TotalHoursWorked, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="clear: both; height: 1px"></div>
                <div style="float: right;">
                    <%: Html.LabelFor(model => model.Salary.Extra50Salary) %>
                    <%: Html.TextBoxFor(model => model.Salary.Extra50Salary, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Salary.ExtraHours50Worked) %>
                    <%: Html.TextBoxFor(model => model.Salary.ExtraHours50Worked, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="clear: both; height: 1px"></div>
                <div style="float: right;">
                    <%: Html.LabelFor(model => model.Salary.Extra100Salary) %>
                    <%: Html.TextBoxFor(model => model.Salary.Extra100Salary, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Salary.ExtraHours100Worked) %>
                    <%: Html.TextBoxFor(model => model.Salary.ExtraHours100Worked, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="clear: both; height: 1px"></div>
                <hr style="background-color:#C2C2C2;border:none;display:block;margin-bottom:0px;margin-top:15px;padding:0px;" />
                <div style="float: right;">
                    <strong>
                    <%: Html.LabelFor(model => model.Salary.TotalSalary) %>
                    <%: Html.TextBoxFor(model => model.Salary.TotalSalary, new { Class = "uservalue", disabled = "true" })%>
                    </strong>
                </div>
                <div style="clear: both; height: 1px"></div>
                <hr style="background-color:#C2C2C2;border:none;display:block;margin-bottom:0px;margin-top:15px;padding:0px;" />
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Salary.CurrentExtraHours50Worked) %>
                    <%: Html.TextBoxFor(model => model.Salary.CurrentExtraHours50Worked, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="float: right;">
                    <%: Html.LabelFor(model => model.Salary.CurrentExtraHours100Worked)%>
                    <%: Html.TextBoxFor(model => model.Salary.CurrentExtraHours100Worked, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="clear: both; height: 1px">
                </div>
            </div>
        </div>
    </div>
    <div id="secondPanel" class="panel">
        <div class="innerPanel">
            <h2>
                <span>Métricas de Campañas</span>
                <% 
                    var availableCampaings = this.Model.AgentCampaings.Select(ac => new SelectListItem { Text = ac.DisplayName, Value = ac.Id.ToString() });
                %>
                <%: Html.DropDownListFor(model => model.CurrentCampaingId, availableCampaings, new { onchange = "refreshMetricValues()", style = "float:right; margin-right:15px;" })%>
            </h2>
            <div class="content">
                 <h3>Valores por Hora</h3>
                 <div style="float: left;">
                    <%: Html.LabelFor(model => model.OptimalHourlyValue) %>
                    <%: Html.TextBoxFor(model => model.OptimalHourlyValue, new { Class = "uservalue", disabled = "true" }) %>
                 </div>
                 <div style="float: left;">
                    <%: Html.LabelFor(model => model.ObjectiveHourlyValue) %>
                    <%: Html.TextBoxFor(model => model.ObjectiveHourlyValue, new { Class = "uservalue", disabled = "true" }) %>
                 </div>
                 <div style="float: left;">
                    <%: Html.LabelFor(model => model.MinimumHourlyValue) %>
                    <%: Html.TextBoxFor(model => model.MinimumHourlyValue, new { Class = "uservalue", disabled = "true" }) %>
                 </div>
                 <div style="clear: both; height: 5px"></div>
                 <hr style="background-color:#C2C2C2;border:none;display:block;margin-bottom:0px;margin-top:15px;padding:0px;" />
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
                        foreach (var metricValues in this.Model.CurrentCampaingMetricValues)
                        {
                    %>
                        <img src="<%= this.Url.Action("MetricsChart", "Agent", new { innerUserId = this.Model.AgentId, campaingId = metricValues.CampaingId, metricId = metricValues.MetricId, month = this.Model.AvailableMetricMonths[this.Model.CurrentMetricMonthIndex] }) %>" alt="<%= metricValues.MetricName %>" width="952" height="350" />
                    <%
                        }
                    %>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
