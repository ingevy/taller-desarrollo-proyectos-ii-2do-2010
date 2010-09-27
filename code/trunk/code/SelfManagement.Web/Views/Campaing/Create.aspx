<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CallCenter.SelfManagement.Web.ViewModels.CampaingViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>

        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Name) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Name) %>
                <%: Html.ValidationMessageFor(model => model.Name) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.CustomerName) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.CustomerName)%>
                <%: Html.ValidationMessageFor(model => model.CustomerName)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.CampaingType) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.CampaingType) %>
                <%: Html.ValidationMessageFor(model => model.CampaingType) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.SupervisorId) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.SupervisorId) %>
                <%: Html.ValidationMessageFor(model => model.SupervisorId) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.BeginDate) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.BeginDate)%>
                <%: Html.ValidationMessageFor(model => model.BeginDate) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.EndDate) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.EndDate)%>
                <%: Html.ValidationMessageFor(model => model.EndDate) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Description) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Description) %>
                <%: Html.ValidationMessageFor(model => model.Description) %>
            </div>
            
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

