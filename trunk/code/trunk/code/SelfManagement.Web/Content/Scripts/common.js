function refreshMetricValues() {
    var index = $("#CurrentCampaingId")[0].selectedIndex;
    var campaingId = $("#CurrentCampaingId").children()[index].value;
    var metricValues = $("#metricvaluescontainer");

    metricValues[0].innerHTML = " ";
    metricValues.addClass("loading");

    var url = getBaseUrl() + "Agent/CampaingMetricValues?campaingId=" + encodeURIComponent(campaingId);

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