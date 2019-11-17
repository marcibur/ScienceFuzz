function initCharts() {
    var ctx = document.getElementById('chart-disciplines-bars');
    console.log(ctx);
    new Chart(ctx, {
        type: 'horizontalBar',
        data: {
            labels: ["Humanities", "Social", "Health", "Technology", "Exact", "Natural", "Agriculture", "Arts"],
            datasets: [{
                backgroundColor: [
                    'rgba(13, 162, 0, .5)',
                    'rgba(13, 162, 0, .5)',
                    'rgba(13, 162, 0, .5)',
                    'rgba(13, 162, 0, .5)',
                    'rgba(13, 162, 0, .5)',
                    'rgba(13, 162, 0, .5)',
                    'rgba(13, 162, 0, .5)',
                    'rgba(13, 162, 0, .5)'
                ],
                data: [1, 2, 3, 4, 5, 6, 7, 8]
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

    ctx = document.getElementById('chart-domains-radar');
    new Chart(ctx, {
        type: 'radar',
        data: {
            labels: ["Humanities", "Social", "Health", "Technology", "Exact", "Natural", "Agriculture", "Arts"],
            datasets: [{
                data: [1, 2, 3, 4, 5, 6, 7, 8],
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