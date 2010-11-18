function refreshAgentMetricValues() {
    var index = $("#CurrentCampaingId")[0].selectedIndex;
    var campaingId = $("#CurrentCampaingId").children()[index].value;
    var metricValues = $("#metricvaluescontainer");
    var metricCharts = $("#metricchartscontainer");
    var metricMonths = $("#CurrentMetricMonthIndex");
    var innerUserId = $("#AgentId")[0].value;

    var labels = $("#secondPanel .content label");

    metricValues[0].innerHTML = " ";
    metricValues.addClass("loading");

    metricCharts[0].innerHTML = " ";
    metricCharts.addClass("loading");

    metricMonths[0].innerHTML = " ";
    metricMonths.addClass("loading");

    labels.addClass("loadingLabel");

    var url = getBaseUrl() + "Agent/CampaingMetricValues?innerUserId=" + encodeURIComponent(innerUserId) + "&campaingId=" + encodeURIComponent(campaingId)

    $.ajax({
        url: url,
        method: "GET",
        type: "GET",
        dataType: "json",
        beforeSend: function (xhr) { xhr.setRequestHeader("Content-Type", "application/json"); },
        success: function (json) {
            var metricValues = $("#metricvaluescontainer");
            var optimalHourlyElement = $("#OptimalHourlyValue")[0];
            var objectiveHourlyElement = $("#ObjectiveHourlyValue")[0];
            var minimumHourlyElement = $("#MinimumHourlyValue")[0];
            var metricMonths = $("#CurrentMetricMonthIndex");
            var innerUserId = $("#AgentId")[0].value;

            if (json.Status && json.Status == "error") {
                metricValues[0].innerHTML = "<h3>Ups! Ocurrio un error...</h3>";
                optimalHourlyElement.value = "Ups! Ocurrio un error...";
                objectiveHourlyElement.value = "Ups! Ocurrio un error...";
                minimumHourlyElement.value = "Ups! Ocurrio un error...";
            }
            else {
                if ((json.CampaingMetricValues == null) || (json.CampaingMetricValues.length == 0)) {
                    metricValues[0].innerHTML = "<h3>No hay metricas disponibles para la Campaña elegida...</h3>";
                }
                else {
                    var html = "<div style=\"float: left;\">";
                    html += "Nivel Actual: <span class=\"" + json.CurrentMetricLevelCssClass + "\">" + json.CurrentMetricLevelDescription + "</span>";
                    html += "</div>";
                    html += "<div style=\"float: right;\">";
                    html += "Nivel Proyectado: <span class=\"" + json.ProjectedMetricLevelCssClass + "\">" + json.ProjectedMetricLevelDescription + "</span>";
                    html += "</div>";
                    html += "<div style=\"clear: both; height: 5px\"></div>";
                    html += "<table cellpadding=\"0\" cellspacing=\"0\" id=\"metricvalues\">";
                    html += "<tbody>";
                    html += "<tr>";
                    html += "<th>Metrica</th>";
                    html += "<th>Tipo</th>";
                    html += "<th>Nivel Optimo</th>";
                    html += "<th>Nivel Objectivo</th>";
                    html += "<th>Nivel Minimo</th>";
                    html += "<th>Valor Actual</th>";
                    html += "<th>Valor Proyectado</th>";
                    html += "</tr>";

                    for (var i = 0; i < json.CampaingMetricValues.length; i++) {
                        var campaingMetricValue = json.CampaingMetricValues[i];
                        html += "<tr>";
                        html += "<td style=\"text-align: right;\">" + campaingMetricValue.MetricName + "</td>";
                        html += "<td style=\"text-align: right;\">" + campaingMetricValue.Format + "</td>";
                        html += "<td style=\"text-align: right;\">" + campaingMetricValue.OptimalValue + "</td>";
                        html += "<td style=\"text-align: right;\">" + campaingMetricValue.ObjectiveValue + "</td>";
                        html += "<td style=\"text-align: right;\">" + campaingMetricValue.MinimumValue + "</td>";
                        html += "<td style=\"text-align: right;\">" + campaingMetricValue.CurrentValue + "</td>";
                        html += "<td style=\"text-align: right;\">" + campaingMetricValue.ProjectedValue + "</td>";
                        html += "</tr>";
                    }

                    html += "</tbody>";
                    html += "</table>";

                    metricValues[0].innerHTML = html;
                }

                if (json.OptimalHourlyValue == null) {
                    optimalHourlyElement.value = "Ups! Ocurrio un error...";
                }
                else {
                    optimalHourlyElement.value = json.OptimalHourlyValue;
                }

                if (json.ObjectiveHourlyValue == null) {
                    objectiveHourlyElement.value = "Ups! Ocurrio un error...";
                }
                else {
                    objectiveHourlyElement.value = json.ObjectiveHourlyValue;
                }

                if (json.MinimumHourlyValue == null) {
                    minimumHourlyElement.value = "Ups! Ocurrio un error...";
                }
                else {
                    minimumHourlyElement.value = json.MinimumHourlyValue;
                }

                if ((json.AvailableMetricMonths == null) || (json.AvailableMetricMonths.length == 0) || (json.CurrentMetricMonthIndex == null)) {
                    metricMonths[0].innerHTML = "<option selected=\"selected\" value=\"0\">No hay meses disponibles</option>";
                }
                else {
                    var html = "";

                    for (var i = 0; i < json.AvailableMetricMonths.length; i++) {
                        html += "<option value=\"" + i.toString() + "\" ";
                        if (i == json.CurrentMetricMonthIndex) {
                            html += "selected=\"selected\"";
                        }

                        html += ">" + json.AvailableMetricMonths[i] + "</option>";
                    }

                    metricMonths[0].innerHTML = html;
                }

                if ((json.CampaingMetricValues == null) || (json.CampaingMetricValues.length == 0) || (json.AvailableMetricMonths == null) || (json.AvailableMetricMonths.length == 0) || (json.CurrentMetricMonthIndex == null)) {
                    metricCharts[0].innerHTML = "<h3>No hay graficos disponibles para las metricas de la Campaña elegida...</h3>";
                }
                else {
                    var html = "";
                    var currentMonth = json.AvailableMetricMonths[json.CurrentMetricMonthIndex];

                    for (var i = 0; i < json.CampaingMetricValues.length; i++) {
                        var campaingMetricValue = json.CampaingMetricValues[i];

                        html += "<img width=\"952\" height=\"350\" alt=\"" + campaingMetricValue.MetricName + "\" src=\"/Agent/MetricsChart?innerUserId=" + innerUserId.toString() + "&campaingId=" + campaingMetricValue.CampaingId + "&metricId=" + campaingMetricValue.MetricId + "&month=" + currentMonth + "\">";
                    }

                    metricCharts[0].innerHTML = html;
                }
            }

            metricValues.removeClass("loading");
            metricCharts.removeClass("loading");
            metricMonths.removeClass("loading");
            labels.removeClass("loadingLabel");
        }
    });
}

function refreshSupervisorMetricValues() {
    var index = $("#CurrentCampaingId")[0].selectedIndex;
    var campaingId = $("#CurrentCampaingId").children()[index].value;
    var metricValues = $("#metricvaluescontainer");
    var metricCharts = $("#metricchartscontainer");
    var metricMonths = $("#CurrentMetricMonthIndex");
    var innerUserId = $("#SupervisorId")[0].value;

    var labels = $("#secondPanel .content label");

    metricValues[0].innerHTML = " ";
    metricValues.addClass("loading");

    metricCharts[0].innerHTML = " ";
    metricCharts.addClass("loading");

    metricMonths[0].innerHTML = " ";
    metricMonths.addClass("loading");

    labels.addClass("loadingLabel");

    var url = getBaseUrl() + "Supervisor/CampaingMetricValues?innerUserId=" + encodeURIComponent(innerUserId) + "&campaingId=" + encodeURIComponent(campaingId)

    $.ajax({
        url: url,
        method: "GET",
        type: "GET",
        dataType: "json",
        beforeSend: function (xhr) { xhr.setRequestHeader("Content-Type", "application/json"); },
        success: function (json) {
            var metricValues = $("#metricvaluescontainer");
            var metricMonths = $("#CurrentMetricMonthIndex");
            var innerUserId = $("#SupervisorId")[0].value;

            if (json.Status && json.Status == "error") {
                metricValues[0].innerHTML = "<h3>Ups! Ocurrio un error...</h3>";
                optimalHourlyElement.value = "Ups! Ocurrio un error...";
                objectiveHourlyElement.value = "Ups! Ocurrio un error...";
                minimumHourlyElement.value = "Ups! Ocurrio un error...";
            }
            else {
                if ((json.CampaingMetricValues == null) || (json.CampaingMetricValues.length == 0)) {
                    metricValues[0].innerHTML = "<h3>No hay metricas disponibles para la Campaña elegida...</h3>";
                }
                else {
                    var html = "<div style=\"float: left;\">";
                    html += "Nivel Actual: <span class=\"" + json.CurrentMetricLevelCssClass + "\">" + json.CurrentMetricLevelDescription + "</span>";
                    html += "</div>";
                    html += "<div style=\"float: right;\">";
                    html += "Nivel Proyectado: <span class=\"" + json.ProjectedMetricLevelCssClass + "\">" + json.ProjectedMetricLevelDescription + "</span>";
                    html += "</div>";
                    html += "<div style=\"clear: both; height: 5px\"></div>";
                    html += "<table cellpadding=\"0\" cellspacing=\"0\" id=\"metricvalues\">";
                    html += "<tbody>";
                    html += "<tr>";
                    html += "<th>Metrica</th>";
                    html += "<th>Tipo</th>";
                    html += "<th>Nivel Optimo</th>";
                    html += "<th>Nivel Objectivo</th>";
                    html += "<th>Nivel Minimo</th>";
                    html += "<th>Valor Actual</th>";
                    html += "<th>Valor Proyectado</th>";
                    html += "</tr>";

                    for (var i = 0; i < json.CampaingMetricValues.length; i++) {
                        var campaingMetricValue = json.CampaingMetricValues[i];
                        html += "<tr>";
                        html += "<td style=\"text-align: right;\">" + campaingMetricValue.MetricName + "</td>";
                        html += "<td style=\"text-align: right;\">" + campaingMetricValue.Format + "</td>";
                        html += "<td style=\"text-align: right;\">" + campaingMetricValue.OptimalValue + "</td>";
                        html += "<td style=\"text-align: right;\">" + campaingMetricValue.ObjectiveValue + "</td>";
                        html += "<td style=\"text-align: right;\">" + campaingMetricValue.MinimumValue + "</td>";
                        html += "<td style=\"text-align: right;\">" + campaingMetricValue.CurrentValue + "</td>";
                        html += "<td style=\"text-align: right;\">" + campaingMetricValue.ProjectedValue + "</td>";
                        html += "</tr>";
                    }

                    html += "</tbody>";
                    html += "</table>";

                    metricValues[0].innerHTML = html;
                }
                
                if ((json.AvailableMetricMonths == null) || (json.AvailableMetricMonths.length == 0) || (json.CurrentMetricMonthIndex == null)) {
                    metricMonths[0].innerHTML = "<option selected=\"selected\" value=\"0\">No hay meses disponibles</option>";
                }
                else {
                    var html = "";

                    for (var i = 0; i < json.AvailableMetricMonths.length; i++) {
                        html += "<option value=\"" + i.toString() + "\" ";
                        if (i == json.CurrentMetricMonthIndex) {
                            html += "selected=\"selected\"";
                        }

                        html += ">" + json.AvailableMetricMonths[i] + "</option>";
                    }

                    metricMonths[0].innerHTML = html;
                }

                if ((json.CampaingMetricValues == null) || (json.CampaingMetricValues.length == 0) || (json.AvailableMetricMonths == null) || (json.AvailableMetricMonths.length == 0) || (json.CurrentMetricMonthIndex == null)) {
                    metricCharts[0].innerHTML = "<h3>No hay graficos disponibles para las metricas de la Campaña elegida...</h3>";
                }
                else {
                    var html = "";
                    var currentMonth = json.AvailableMetricMonths[json.CurrentMetricMonthIndex];

                    for (var i = 0; i < json.CampaingMetricValues.length; i++) {
                        var campaingMetricValue = json.CampaingMetricValues[i];

                        html += "<img width=\"952\" height=\"350\" alt=\"" + campaingMetricValue.MetricName + "\" src=\"/Agent/MetricsChart?innerUserId=" + innerUserId.toString() + "&campaingId=" + campaingMetricValue.CampaingId + "&metricId=" + campaingMetricValue.MetricId + "&month=" + currentMonth + "\">";
                    }

                    metricCharts[0].innerHTML = html;
                }
            }

            metricValues.removeClass("loading");
            metricCharts.removeClass("loading");
            metricMonths.removeClass("loading");
            labels.removeClass("loadingLabel");
        }
    });
}

function refreshMetricCharts() {
    var index = $("#CurrentMetricMonthIndex")[0].selectedIndex;
    var month = $("#CurrentMetricMonthIndex").children()[index].text;
    var images = $("#metricchartscontainer img");

    images.addClass("loading");

    for (var i = 0; i < images.length; i++) {
        var originalUrl = images[i].src.toString();
        var index = originalUrl.length - 7;

        var newUrl = originalUrl.substring(0, index);
        newUrl += month;

        images[i].src = newUrl;
    }

    images.removeClass("loading");
}

function refreshSalary() {
    var index = $("#CurrentSalaryMonthIndex")[0].selectedIndex;
    var month = $("#CurrentSalaryMonthIndex").children()[index].text;
    var innerUserId = $("#AgentId")[0].value;

    var labels = $("#mainPanel .content label");
    var inputs = $("#mainPanel .content input");

    labels.addClass("loadingLabel");

    var url = getBaseUrl() + "Agent/Salary?innerUserId=" + encodeURIComponent(innerUserId) + "&month=" + encodeURIComponent(month);

    $.ajax({
        url: url,
        method: "GET",
        type: "GET",
        dataType: "json",
        beforeSend: function (xhr) { xhr.setRequestHeader("Content-Type", "application/json"); },
        success: function (json) {
            var labels = $("#mainPanel .content label");
            var inputs = $("#mainPanel .content input");
            var hrs = $("#mainPanel .content hr");

            if (json.Status && json.Status == "error") {
                inputs[0].value = "Ups! Ocurrio un error...";
                inputs[1].value = "Ups! Ocurrio un error...";
                inputs[2].value = "Ups! Ocurrio un error...";
                inputs[3].value = "Ups! Ocurrio un error...";
                inputs[4].value = "Ups! Ocurrio un error...";
                inputs[5].value = "Ups! Ocurrio un error...";
                inputs[6].value = "Ups! Ocurrio un error...";
                inputs[7].value = "Ups! Ocurrio un error...";
                inputs[8].value = "Ups! Ocurrio un error...";
                inputs[9].value = "Ups! Ocurrio un error...";
            } else if (json.Salary == null) {
                inputs[0].value = "No se encontraron datos";
                inputs[1].value = "No se encontraron datos";
                inputs[2].value = "No se encontraron datos";
                inputs[3].value = "No se encontraron datos";
                inputs[4].value = "No se encontraron datos";
                inputs[5].value = "No se encontraron datos";
                inputs[6].value = "No se encontraron datos";
                inputs[7].value = "No se encontraron datos";
                inputs[8].value = "No se encontraron datos";
                inputs[9].value = "No se encontraron datos";
            } else {
                inputs[0].value = json.Salary.GrossSalary;
                inputs[1].value = json.Salary.VariableSalary;
                inputs[2].value = json.Salary.TotalHoursWorked;
                inputs[3].value = json.Salary.Extra50Salary;
                inputs[4].value = json.Salary.ExtraHours50Worked;
                inputs[5].value = json.Salary.Extra100Salary;
                inputs[6].value = json.Salary.ExtraHours100Worked;
                inputs[7].value = json.Salary.TotalSalary;
                inputs[8].value = json.Salary.CurrentExtraHours50Worked;
                inputs[9].value = json.Salary.CurrentExtraHours100Worked;

                var index = $("#CurrentSalaryMonthIndex")[0].selectedIndex;
                if (index + 1 == $("#CurrentSalaryMonthIndex").children().length) {
                    labels[2].innerHTML = "Sueldo Variable Proyectado";
                    labels[3].innerHTML = "Total Horas Proyectadas";
                    labels[4].innerHTML = "Sueldo Horas Extra 50% Proyectado";
                    labels[5].innerHTML = "Horas Extra 50% Proyectadas";
                    labels[6].innerHTML = "Sueldo Horas Extra 100% Proyectado";
                    labels[7].innerHTML = "Horas Extra 100% Proyectadas";
                    labels[8].innerHTML = "Sueldo Total Proyectado";
                    hrs[1].style.display = "block";
                    labels[9].style.display = "inline-block";
                    inputs[8].style.display = "inline-block";
                    labels[10].style.display = "inline-block";
                    inputs[9].style.display = "inline-block";
                }
                else {
                    labels[2].innerHTML = "Sueldo Variable";
                    labels[3].innerHTML = "Total Horas";
                    labels[4].innerHTML = "Sueldo Horas Extra 50%";
                    labels[5].innerHTML = "Horas Extra 50%";
                    labels[6].innerHTML = "Sueldo Horas Extra 100%";
                    labels[7].innerHTML = "Horas Extra 100%";
                    labels[8].innerHTML = "Sueldo Total";
                    hrs[1].style.display = "none";
                    labels[9].style.display = "none";
                    inputs[8].style.display = "none";
                    labels[10].style.display = "none";
                    inputs[9].style.display = "none";
                }
            }

            labels.removeClass("loadingLabel");
        }
    });
}

function showOrHideMetricLevels(event, id) {
    metricLevelsElement = $("#" + id);

    if (metricLevelsElement[0].style.display == "none") {
        metricLevelsElement.show();
    }
    else {
        metricLevelsElement.hide();
    }
}

function updateSupervisorsList(beginDate, endDate) {
    var supervisors = $("#supervisors");

    if (!beginDate || (beginDate == "")) {
        supervisors[0].innerHTML = "<h3>No hay supervisores disponibles en el rango de fechas elegido...</h3>";
    }
    else if (endDate && (endDate != "") && (endDate <= beginDate)) {
        supervisors[0].innerHTML = "<h3>La fecha de inicio tiene que ser menor que la de fin para determinar los Supervisores disponibles...</h3>";
    }
    else {
        supervisors[0].innerHTML = " ";
        supervisors.addClass("loading");

        var url = getBaseUrl() + "AvailableSupervisores?beginDate=" + encodeURIComponent(beginDate);
        if (endDate && (endDate != "")) {
            url += "&endDate=" + encodeURIComponent(endDate);
        }

        $.ajax({
            url: url,
            method: "GET",
            type: "GET",
            dataType: "json",
            beforeSend: function (xhr) { xhr.setRequestHeader("Content-Type", "application/json"); },
            success: function (json) {
                var supervisors = $("#supervisors");
                if (json.Status && json.Status == "error") {
                    supervisors[0].innerHTML = "<h3>Ups! Ocurrio un error...</h3>";
                } else if (json.Supervisors.length == 0) {
                    supervisors[0].innerHTML = "<h3>No hay supervisores disponibles en el rango de fechas elegido...</h3>";
                } else {
                    var html = "<ul class=\"campaingElement ui-sortable\" id=\"supervisorsList\">";
                    for (var i = 0; i < json.Supervisors.length; i++) {
                        var supervisor = json.Supervisors[i];

                        html += "<li class=\"ui-state-default\"><span class=\"off id\">";
                        html += "<input id=\"CampaingSupervisors_" + i.toString() +"__Id\" name=\"CampaingSupervisors[" + i.toString() + "].Id\" type=\"hidden\" value=\"" + supervisor.Id + "\" /></span>";
                        html += "<h3>";
                        html += "<a href=\"#\">" + supervisor.DisplayName  + "</a></h3>";
                        html += "<input id=\"CampaingSupervisors_" + i.toString() +"__DisplayName\" name=\"CampaingSupervisors[" + i.toString() + "].DisplayName\" type=\"hidden\" value=\"" + supervisor.DisplayName + "\" />";
                        html += "<p class=\"options\">";
                        html += "<input id=\"CampaingSupervisors_" + i.toString() + "__Selected\" name=\"CampaingSupervisors[" + i.toString() + "].Selected\" onclick=\"\" type=\"checkbox\" value=\"true\" /><input name=\"CampaingSupervisors[" + i.toString() + "].Selected\" type=\"hidden\" value=\"" + supervisor.Selected + "\" />";
                        html += "</p>";
                        html += "</li>";
                    }
                    
                    html += "</ul>";                    
                    supervisors[0].innerHTML = html;
                }

                supervisors.removeClass("loading");
            }
        });
    }
}

function getBaseUrl() {
    return document.location.protocol + "//" + document.location.host + "/";
}

function searchAgentsKeyPressed(control, e) {
    var key = (document.all) ? e.keyCode : e.which;

    if (key == 13) {
        redirectSearchAgents();
    }
}

function searchSupervisorsKeyPressed(control, e) {
    var key = (document.all) ? e.keyCode : e.which;

    if (key == 13) {
        redirectSearchSupervisors();
    }
}

function searchSupervisorsKeyPressed(control, e) {
    var key = (document.all) ? e.keyCode : e.which;

    if (key == 13) {
        redirectSearchCampaings();
    }
}

function redirectSearchAgents() {
    var searchCriteria = $("#SearchCriteria")[0].value;

    window.location = "/Agent/Search/" + searchCriteria;
}

function redirectSearchSupervisors() {
    var searchCriteria = $("#SearchCriteria")[0].value;

    window.location = "/Supervisor/Search/" + searchCriteria;
}

function redirectSearchCampaings() {
    var searchCriteria = $("#SearchCriteria")[0].value;

    window.location = "/Campaing/Search/" + searchCriteria;
}