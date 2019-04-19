function initChart(className, title, chartData) {

    console.log('Initializing ' + className + ' with data: ', chartData);
    var chart = $('.' + className);

    new Chart(chart, {
        type: 'bubble',
        data: {
            datasets: chartData
        },
        options: {
            title: {
                display: true,
                text: title,
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