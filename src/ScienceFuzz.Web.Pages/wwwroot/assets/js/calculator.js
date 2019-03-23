function initChart(data) {
    console.log('Initializing results-chart with data: ', data);
    var ctx = document.getElementById("results-chart").getContext("2d");
    var myChart = new Chart(ctx, {
        type: 'radar',
        data: {
            labels: ["Humanities", "Social", "Health", "Technology", "Exact", "Natural", "Agriculture", "Arts"],
            datasets: [{
                data: [],
                backgroundColor: [
                    'rgba(13, 162, 0, .5)'
                ],
                borderColor: [
                    'rgb(10, 129, 0)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            legend: {
                display: false
            },
            tooltips: {
                displayColors: false
            }
        }
    });
}