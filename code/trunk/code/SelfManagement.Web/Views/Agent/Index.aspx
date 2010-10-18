﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CallCenter.SelfManagement.Web.ViewModels.AgentDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SelfManagement - Agentes
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h1 class="info">
        <span style="float: left;"><%= this.Model.DisplayName %></span>
        <span style="float: right;">
            <%: Html.LabelFor(model => model.CurrentSupervisor) %>
            <a href="#"><%= this.Model.CurrentSupervisor %></a>
        </span>
    </h1>

    <div style="clear: both; height: 1px"></div>

    <div id="mainPanel" class="panel">
        <div class="innerPanel">
            <h2><span>Liquidación de Sueldo</span></h2>
            <div id="content">
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
                    <%: Html.DropDownListFor(model => model.CurrentMonth, availableMonths, new { Class = "uservalue" })%>
                </div>
                <div style="clear: both; height: 1px"></div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Salary.VariableSalary) %>
                    <%: Html.TextBoxFor(model => model.Salary.VariableSalary, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="float: right;">
                    <%: Html.LabelFor(model => model.Salary.TotalSalary) %>
                    <%: Html.TextBoxFor(model => model.Salary.TotalSalary, new { Class = "uservalue", disabled = "true" })%>
                </div>
                <div style="clear: both; height: 1px"></div>
            </div>

        </div>
    </div>

</asp:Content>
