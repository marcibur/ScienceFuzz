let chartReferences = {};

function loadChart(id, input) {
    var existingChart = chartReferences[id];
    if (existingChart != null) {
        existingChart.destroy();
        existingChart[id] = null;
    }
    var ctx = document.getElementById(id);
    var chart = new Chart(ctx, input);
    chartReferences[id] = chart;
}