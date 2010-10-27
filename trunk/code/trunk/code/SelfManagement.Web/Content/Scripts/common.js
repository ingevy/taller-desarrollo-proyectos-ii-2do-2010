function refreshMetricValues() {
    var index = $("#CurrentCampaingId")[0].selectedIndex;
    var campaingId = $("#CurrentCampaingId").children()[index].value;
    var metricValues = $("#metricvaluescontainer");
    var innerUserId = $("#AgentId")[0].value;

    metricValues[0].innerHTML = " ";
    metricValues.addClass("loading");

    var url = getBaseUrl() + "Agent/CampaingMetricValues?innerUserId=" + encodeURIComponent(innerUserId) + "&campaingId=" + encodeURIComponent(campaingId)

    $.ajax({
        url: url,
        method: "GET",
        type: "GET",
        dataType: "json",
        beforeSend: function (xhr) { xhr.setRequestHeader("Content-Type", "application/json"); },
        success: function (json) {
            var metricValues = $("#metricvaluescontainer");
            if (json.Status && json.Status == "error") {
                metricValues[0].innerHTML = "<h3>Ups! Ocurrió un error...</h3>";
            } else if (json.CampaingMetricValues.length == 0) {
                metricValues[0].innerHTML = "<h3>No metricas disponibles para la Campaña elegida...</h3>";
            } else {

                var html = "<table cellpadding=\"0\" cellspacing=\"0\" id=\"metricvalues\">";
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
                    html += "<td>" + campaingMetricValue.MetricName + "</td>";
                    html += "<td>" + campaingMetricValue.Format + "</td>";
                    html += "<td>" + campaingMetricValue.OptimalValue + "</td>";
                    html += "<td>" + campaingMetricValue.ObjectiveValue + "</td>";
                    html += "<td>" + campaingMetricValue.MinimumValue + "</td>";
                    html += "<td>" + campaingMetricValue.CurrentValue + "</td>";
                    html += "<td>" + campaingMetricValue.ProjectedValue + "</td>";
                    html += "</tr>";
                }
                html += "</tbody>";
                html += "</table>";

                metricValues[0].innerHTML = html;
            }

            metricValues.removeClass("loading");
        }
    });
}

function refreshSalary() {
    var index = $("#CurrentMonth")[0].selectedIndex;
    var month = $("#CurrentMonth").children()[index].text;
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

            if (json.Status && json.Status == "error") {
                inputs[0].value = "Ups! Ocurrió un error...";
                inputs[1].value = "Ups! Ocurrió un error...";
                inputs[2].value = "Ups! Ocurrió un error...";
                inputs[3].value = "Ups! Ocurrió un error...";
                inputs[4].value = "Ups! Ocurrió un error...";
                inputs[5].value = "Ups! Ocurrió un error...";
                inputs[6].value = "Ups! Ocurrió un error...";
                inputs[7].value = "Ups! Ocurrió un error...";
                inputs[8].value = "Ups! Ocurrió un error...";
                inputs[9].value = "Ups! Ocurrió un error...";
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
                inputs[7].value = json.Salary.CurrentExtraHours50Worked;
                inputs[8].value = json.Salary.CurrentExtraHours100Worked;
                inputs[9].value = json.Salary.TotalSalary;

                var index = $("#CurrentMonth")[0].selectedIndex;
                if (index + 1 == $("#CurrentMonth").children().length) {
                    labels[2].innerHTML = "Parte Variable Proyectada";
                    labels[3].innerHTML = "Total Horas Proyectadas";
                    labels[4].innerHTML = "Sueldo Horas Extra 50% Proyectado";
                    labels[5].innerHTML = "Horas Extra 50% Proyectadas";
                    labels[6].innerHTML = "Sueldo Horas Extra 100% Proyectado";
                    labels[7].innerHTML = "Horas Extra 100% Proyectadas";
                    labels[10].innerHTML = "Sueldo Total Proyectado";
                }
                else {
                    labels[2].innerHTML = "Parte Variable";
                    labels[3].innerHTML = "Total Horas";
                    labels[4].innerHTML = "Sueldo Horas Extra 50%";
                    labels[5].innerHTML = "Horas Extra 50%";
                    labels[6].innerHTML = "Sueldo Horas Extra 100%";
                    labels[7].innerHTML = "Horas Extra 100%";
                    labels[10].innerHTML = "Sueldo Total";
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
                    supervisors[0].innerHTML = "<h3>Ups! Ocurrió un error...</h3>";
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
    var baseUrl = "";
    var baseElement = document.getElementsByTagName('base')[0];

    if (baseElement && baseElement.href && baseElement.href.length > 0) {
        baseUrl = baseElement.href;
    }
    else {
        baseUrl = document.URL;
    }

    var qsStart = baseUrl.indexOf('?');
    if (qsStart !== -1) {
        baseUrl = baseUrl.substr(0, qsStart);
    }

    qsStart = baseUrl.indexOf('#');
    if (qsStart !== -1) {
        baseUrl = baseUrl.substr(0, qsStart);
    }

    return baseUrl.substr(0, baseUrl.lastIndexOf('/') + 1);
}