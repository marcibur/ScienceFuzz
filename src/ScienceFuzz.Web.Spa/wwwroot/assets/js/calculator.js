function setDisciplinesChart(data) {
    if (window.disciplineChart != null) {
        window.disciplineChart.destroy();
    }

    var ctx = document.getElementById('chart-disciplines-bars');
    window.disciplineChart = new Chart(ctx, {
        type: 'horizontalBar',
        data: data,
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

function setDomainsChart(data) {
    if (window.domainChart != null) {
        window.domainChart.destroy();
    }
    console.log(data.labels);
    console.log(data.datasets[0].data);
    ctx = document.getElementById('chart-domains-radar');
    window.domainChart = new Chart(ctx, {
        type: 'radar',
        data: {
            labels: data.labels,
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
                data: data.datasets[0].data
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



function initCharts() {
    var ctx = document.getElementById('chart-disciplines-bars');
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


