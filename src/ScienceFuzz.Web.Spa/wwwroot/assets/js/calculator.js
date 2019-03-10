window.initChart = () => {
    console.log("Initializing chart...");

    var ctx = document.getElementById("results-chart").getContext("2d");
    console.log("Got chart conext.", ctx);

    window.chart = new Chart(ctx, {
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
            },
            scale: {
                ticks: {
                    beginAtZero: true,
                    min: 0,
                    max: 1,
                    stepSize: .1
                },
                pointLabels: {
                    fontSize: 18
                }
            }
        }
    });

    console.log("Chart created.", window.chart);
};

window.updateChart = (data) => {
    console.log("Clearing chart...", window.chart);
    window.chart.data.datasets.forEach((dataset) => {
        dataset.data = [];
    });

    console.log("Adding new data...", data);
    window.chart.data.datasets.forEach((dataset) => {
        data.forEach((value) => {
            dataset.data.push(value);
        });
    });

    window.chart.update();
    console.log("Chart updated.");
};