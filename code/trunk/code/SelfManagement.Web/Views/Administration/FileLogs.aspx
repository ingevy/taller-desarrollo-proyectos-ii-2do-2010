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
                <td><a href="#" onclick="showFileLog(this, event);">${Id}</a></td>
                <td style="text-align: center;"><span class="${CssClass}" title="${State}"></span></td>
                <td>${Path}</td>
                <td style="text-align: center;" >${FileType}</td>
                <td>${DateData}</td>
                <td>${DateProcessed}</td>
                <td>${DateLastModified}</td>
            </tr>
    </script>

    <script type="text/html" id="fileLogTemplate">
            <p>${Value}</p>
    </script>
    
    <div id="mainPanel" class="panel">
        <div class="innerPanel">
            <h2>
                <span>Filtros de Archivos</span></h2>
            <fieldset id="fileFilters">
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.DateData) %>
                    <%: Html.TextBoxFor(model => model.DateData, new { datepicker = "true", datepicker_format = "DD/MM/YYYY" })%>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.DateProcessed)%>
                    <%: Html.TextBoxFor(model => model.DateProcessed, new { datepicker = "true", datepicker_format = "DD/MM/YYYY" })%>
                </div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.DateModified)%>
                    <%: Html.TextBoxFor(model => model.DateModified, new { datepicker = "true", datepicker_format = "DD/MM/YYYY" })%>
                </div>
                <div style="clear: both; height: 1px"></div>
                <div style="float: left;">
                    <%: Html.LabelFor(model => model.FileType)%>
                    <%
                        var fileTypes = new List<SelectListItem> { new SelectListItem { Text = "Todos", Value = "5", Selected = true }, new SelectListItem { Text = "Summary", Value = "0" }, new SelectListItem { Text = "QA", Value = "1" }, new SelectListItem { Text = "TTS", Value = "2" }, new SelectListItem { Text = "STS", Value = "3" }, new SelectListItem { Text = "HF", Value = "4" } };
                    %>
                    <%: Html.DropDownListFor(model => model.FileType, fileTypes)%>
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
            <div class="content" style="padding: 9px;">
                <div id="loadingMessage" class="loading" title="Filtrando..." style="min-height: 70px;">
                </div>
                <div id="errorMessage" style="display: none; text-align:center; min-height: 70px;">
                    <p style="font-weight:bold;">Hubo un error al consultar los archivos procesados. Por favor, intente más tarde.</p>
			    </div>
                <div id="noResultsMessage" style="display: none; text-align:center; min-height: 70px;">
                    <p style="font-weight:bold;">No se encontraron archivos para los filtros usados.</p>
			    </div>
                <div id="fileValuesContainer" style="display: none; min-height: 70px; padding:0; font-size: 85%;">
                    <table cellpadding="0" cellspacing="0" id="fileValues">
                            <tbody>
                                <tr>
                                    <th>Id</th>
                                    <th>Estado</th>
                                    <th>Ruta</th>
                                    <th style="text-align: center;">Tipo</th>
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

    <div id="dialogContainer" style="display: none; padding: 0px;" title="Log de Archivo">
        <div id="loadingMessageLog" title="Cargando..." class="loading" style="min-height: 70px; margin-top: 10px;">
        </div>
        <div id="errorMessageLog" style="display: none; text-align:center">
            <p style="font-weight:bold;">Hubo un error al consultar el log del archivo. Por favor, intente más tarde.</p>
        </div>
        <div id="noResultsMessageLog" style="display: none; text-align:center">
            <p style="font-weight:bold;">El archivo seleccionado no posee log.</p>
        </div>
        <div id="logInfoContainer" style="display: none">
            <div id="logInfo" style="font-weight:bold;">log</div>
        </div>
    </div>

</asp:Content>

