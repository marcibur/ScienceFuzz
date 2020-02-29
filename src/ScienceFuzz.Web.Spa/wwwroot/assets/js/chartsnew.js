let chartReferences = {
    tsneChart: null,
    kmeansChart: null,
    chartDomains: null,
    chartDisciplines: null
};

function loadChart(id, input) {
    console.log('input', input);
    var existingChart = chartReferences[id];
    if (existingChart !== null) {
        existingChart.destroy();
        existingChart[id] = null;
    }
    var temp = document.getElementById(id);
    console.log('temp', temp);
    var ctx = document.getElementById(id).getContext('2d');
    var chart = new Chart(ctx, input);
    console.log('chart', chart);
    chartReferences[id] = chart;
}