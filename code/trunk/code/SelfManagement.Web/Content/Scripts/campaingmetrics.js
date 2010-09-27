function removeMetricLevel(id) {
    if ($("#" + id + " > input")[0].value == "New") {
        $("#" + id + " > input")[0].value = "None";
    }
    else {
        $("#" + id + " > input")[0].value = "Remove";
    }

    $("#" + id)[0].style.display = "none";
}

function addMetricLevel(listId, metricId, metricName, metricDescription, metricFormatType) {
    var metricElement = $("#" + metricId)[0];

    if (metricElement == undefined) {
        var index = $("#" + listId)[0].children.length;

        var html = "<li style=\"display: block;\" id=\"" + metricId.toString() + "\" class=\"ui-state-default\">"
+ "<input type=\"hidden\" value=\"New\" name=\"CampaingMetrics[" + index.toString() + "].MetricLevelStatus\" id=\"CampaingMetrics_" + index.toString() + "__MetricLevelStatus\">"
+ "<input type=\"hidden\" value=" + metricId.toString() + " name=\"CampaingMetrics[" + index.toString() + "].MetricId\" id=\"CampaingMetrics_" + index.toString() + "__MetricId\">"
+ "<input type=\"hidden\" value=" + metricName.toString() + " name=\"CampaingMetrics[" + index.toString() + "].Name\" id=\"CampaingMetrics_" + index.toString() + "__Name\">"
+ "<input type=\"hidden\" value=" + metricDescription + " name=\"CampaingMetrics[" + index.toString() + "].Description\" id=\"CampaingMetrics_" + index.toString() + "__Description\">"
+ "<h3><a href=\"#\">" + metricName.toString() + "</a></h3>"
+ "<p>" + metricDescription + "</p>"
+ "<input type=\"hidden\" value=\"" + metricFormatType.toString() + "\" name=\"CampaingMetrics[" + index.toString() + "].FormatType\" id= \"CampaingMetrics_" + index.toString() + "__FormatType\">"
+ "<div class=\"inline estimatedMinutes\">"
+ "Optimo: <input type=\"text\" value=\"0\" size=\"3\" name=\"CampaingMetrics[" + index.toString() + "].OptimalLevel\" id=\"CampaingMetrics_" + index.toString() + "__OptimalLevel\" isdatepicker=\"true\">"
+ "&nbsp;&nbsp;&nbsp;"
+ "Objetivo: <input type=\"text\" value=\"0\" size=\"3\" name=\"CampaingMetrics[" + index.toString() + "].ObjectiveLevel\" id=\"CampaingMetrics_" + index.toString() + "__ObjectiveLevel\" isdatepicker=\"true\">"
+ "&nbsp;&nbsp;&nbsp;"
+ "Minimo: <input type=\"text\" value=\"0\" size=\"3\" name=\"CampaingMetrics[" + index.toString() + "].MinimumLevel\" id=\"CampaingMetrics_" + index.toString() + "__MinimumLevel\" isdatepicker=\"true\">"
+ "</div>"
+ "<p class=\"options\">"
+ "<a href=\"JavaScript:removeMetricLevel('" + metricId.toString() + "')\" title=\"Remover Métrica\" class=\"btn remove\">Remover Métrica</a>"
+ "</p>"
+ "</li>";

        $("#" + listId)[0].innerHTML += html;
    }
    else if (metricElement.children[0].value == "Remove") {
        metricElement.children[0].value == "New"
        metricElement.display = "block";
    }
    else if (metricElement.children[0].value == "None") {
        metricElement.display = "block";
    }



//    var textSelected = $("#" + comboId).children()[index].text;
//    var valueSelected = $("#" + comboId).children()[index].value;
//    var newIndex = $("#" + listId)[0].children.length;

//    if (valueSelected != null && valueSelected != "") {
//        if ($("#" + valueSelected).length == 0) {
//            var html = "<li id=\"" + valueSelected + "\">"
//              + "<label>" + textSelected + "</label>"
//              + "<a class=\"deletelink\" href=\"JavaScript:removeAnimal('" + valueSelected + "')\">Remover</a>"
//              + "<input type=\"hidden\" value=\"New\" name=\"Animals[" + newIndex.toString() + "].AnimalStatus\" id=\"Animals[" + newIndex.toString() + "]_AnimalStatus\" />"
//              + "<input type=\"hidden\" value=\"" + valueSelected + "\" name=\"Animals[" + newIndex.toString() + "].AnimalId\" id=\"Animals[" + newIndex.toString() + "]_AnimalId\" />"
//              + "<div class=\"clear\"></div>"
//              + "</li>";

//            $("#" + listId)[0].innerHTML += html;
//        }
//        else {
//            $("#" + valueSelected + " input")[0].value = "New";
//            $("#" + valueSelected)[0].style.display = "";
//        }
//    }
}