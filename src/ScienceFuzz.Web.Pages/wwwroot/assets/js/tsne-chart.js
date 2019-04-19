function initChart(chartData) {
    console.log('Initializing tsne-chart with data: ', null);
    var chart = $('.tsne-chart');
    console.log(chart);

    new Chart(chart, {
        type: 'bubble',
        data: {
            datasets: chartData
        },
        options: {
            title: {
                display: true,
                text: 'T-SNE',
                fontSize: 20
            },
            legend: {
                display: true,
                position: 'right'
            },
            tooltips: {
                displayColors: false
            }
        }
    });
}