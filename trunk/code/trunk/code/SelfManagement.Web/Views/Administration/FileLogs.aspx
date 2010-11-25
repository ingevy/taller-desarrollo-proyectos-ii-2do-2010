<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CallCenter.SelfManagement.Web.ViewModels.FileFilterViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	SelfManagement - Logs
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Español -->
    <input type="hidden" id="DPC_TODAY_TEXT" value="Hoy" />
    <input type="hidden" id="DPC_BUTTON_TITLE" value="Abric calendario..." />
    <input type="hidden" id="DPC_MONTH_NAMES" value="['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre']" />
    <input type="hidden" id="DPC_DAY_NAMES" value="['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sa']" />
    <!-- Español -->

    <script type="text/javascript">
        $(function () {

            filterFiles();

            $("#filterButton").click(function (event) {
                // alert("Filtar");
                filterFiles();
            });
        });
    </script>

    <script type="text/html" id="fileRowTemplate">
            <tr>
                <td>${Id}</td>
                <td>${HasErrors}</td>
                <td>${FileType}</td>
                <td>TODO</td>
                <td>${DateProcessed}</td>
                <td>${DateLastModified}</td>
            </tr>
        </script>
    
    <div id="mainPanel" class="panel">
        <div class="innerPanel">
            <h2>
                <span>Filtros de Archivos</span></h2>
            <fieldset id="fileFilters">
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.DataDate) %>
                    <%: Html.TextBoxFor(model => model.DataDate, new { datepicker = "true", datepicker_format = "DD/MM/YYYY" }) %>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.ProcessingDate) %>
                    <%: Html.TextBoxFor(model => model.ProcessingDate, new { datepicker = "true", datepicker_format = "DD/MM/YYYY" }) %>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.ModifiedDate) %>
                    <%: Html.TextBoxFor(model => model.ModifiedDate, new { datepicker = "true", datepicker_format = "DD/MM/YYYY" })%>
                </div>
                <div style="clear: both; height: 1px"></div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.Type) %>
                    <%
                        var fileTypes = new List<SelectListItem> { new SelectListItem { Text = "Todos", Value = "5", Selected = true }, new SelectListItem { Text = "Summary", Value = "0" }, new SelectListItem { Text = "QA", Value = "1" }, new SelectListItem { Text = "TTS", Value = "2" }, new SelectListItem { Text = "STS", Value = "3" }, new SelectListItem { Text = "HF", Value = "4" } };
                    %>
                    <%: Html.DropDownListFor(model => model.Type, fileTypes) %>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.State) %>
                    <%
                        var fileStates = new List<SelectListItem> { new SelectListItem { Text = "Todos", Value = "2", Selected = true }, new SelectListItem { Text = "Sin Errores", Value = "0" }, new SelectListItem { Text = "Con Errores", Value = "1" } };
                    %>
                    <%: Html.DropDownListFor(model => model.State, fileStates)%>
                </div>
                <div id="buttonsPanel" style="text-align: right;">
                    <input id="filterButton" type="button" value="Filtrar" />
                </div>
                <div style="clear: both; height: 1px"></div>
            </fieldset>
        </div>
    </div>
    
    <div id="secondPanel" class="panel">
        <div class="innerPanel">
            <h2>
                <span>Resultados</span>
            </h2>
            <div class="content">
                <div id="loadingMessage" class="loading" title="Filtrando..." style="min-height: 70px;">
                </div>
                <div id="errorMessage" style="display: none; text-align:center; min-height: 70px;">
                    <p style="font-weight:bold;">Hubo un error al consultar los archivos procesados. Por favor, intente más tarde.</p>
			    </div>
                <div id="noResultsMessage" style="display: none; text-align:center; min-height: 70px;">
                    <p style="font-weight:bold;">No se encontraron archivos para los filtros usados.</p>
			    </div>
                <div id="fileValuesContainer" style="display: none; min-height: 70px;">
                    <table cellpadding="0" cellspacing="0" id="fileValues">
                            <tbody>
                                <tr>
                                    <th>Id</th>
                                    <th>Estado</th>
                                    <th>Ruta</th>
                                    <th>Tipo</th>
                                    <th>Fecha Datos</th>
                                    <th>Fecha Procesado</th>
                                    <th>Fecha Modificado</th>
                                </tr>
                            </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

