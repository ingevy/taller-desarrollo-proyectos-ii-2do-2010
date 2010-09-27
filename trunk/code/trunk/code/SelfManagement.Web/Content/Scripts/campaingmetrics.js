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
        alert('undefined');
    }
    else if (metricElement.children[0].value == "Remove") {
        metricElement.children[0].value == "New"
        metricElement.display = "block";
    }
    else if (metricElement.children[0].value == "None") {
        metricElement.display = "block";
    }



//<li style="display: block;" id="20" class="ui-state-default">
//                        <input type="hidden" value="New" name="CampaingMetrics[0].MetricLevelStatus" id="CampaingMetrics_0__MetricLevelStatus">
//                        <input type="hidden" value="20" name="CampaingMetrics[0].MetricId" id="CampaingMetrics_0__MetricId">
//                        <h3><a href="#">I2C_PCT</a></h3>
//                        <p>Interaction to Call Percent</p>
//                        <input type="hidden" value="0" name="CampaingMetrics[0].FormatType" id="CampaingMetrics_0__FormatType">
//                        <div class="inline estimatedMinutes">
//                            Optimo: <input type="text" value="10" size="3" name="CampaingMetrics[0].OptimalLevel" id="CampaingMetrics_0__OptimalLevel" isdatepicker="true">
//                            &nbsp;&nbsp;&nbsp;
//                            Objetivo: <input type="text" value="20" size="3" name="CampaingMetrics[0].ObjectiveLevel" id="CampaingMetrics_0__ObjectiveLevel" isdatepicker="true">
//                            &nbsp;&nbsp;&nbsp;
//                            Mínimo: <input type="text" value="25" size="3" name="CampaingMetrics[0].MinimumLevel" id="CampaingMetrics_0__MinimumLevel" isdatepicker="true">
//                        </div>
//                        <p class="options">
//                            <a href="JavaScript:removeMetricLevel('20')" title="Remover Métrica" class="btn remove">Remover Métrica</a>
//                        </p>
//                    </li>

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