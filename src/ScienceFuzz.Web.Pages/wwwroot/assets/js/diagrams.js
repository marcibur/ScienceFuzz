
function initRadarDisciplinesChart(labels, data) {
    var ctx = document.getElementById('chart-disciplines-radar').getContext('2d');

    new Chart(ctx, {
        type: 'radar',
        data: {
            labels: labels,
            datasets: [{
                data: data,
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
                    suggestedMin: 0,
                    max: 1
                }
            }
        }
    });
}

function initBarDisciplinesChart(labels, data) {
    ctx = document.getElementById('chart-disciplines-bars').getContext('2d');

    var colors = new Array();
    for (var i = 0; i < data.length; i++) {
        colors.push('rgba(13, 162, 0, .5)');
    }

    new Chart(ctx, {
        type: 'horizontalBar',
        data: {
            labels: labels,
            datasets: [{
                backgroundColor: colors,
                data: data
            }]
        },
        options: {
            legend: {
                display: false
            },
            tooltips: {
                displayColors: false
            },
            scales: {
                xAxes: [{
                    display: true,
                    ticks: {
                        suggestedMin: 0, 
                        max: 1
                    }
                }]
            }
        }
    });
}

function initRadarDomainsChart(labels, data) {
    var ctx = document.getElementById('chart-domains-radar');
    new Chart(ctx, {
        type: 'radar',
        data: {
            labels: labels,
            datasets: [{
                data: data,
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
                    suggestedMin: 0,
                    max: 1
                }
            }
        }
    });
}

function initBarDomainsChart(labels, data) {
    ctx = document.getElementById('chart-domains-bars');

    var colors = new Array();
    for (var i = 0; i < data.length; i++) {
        colors.push('rgba(13, 162, 0, .5)');
    }

    new Chart(ctx, {
        type: 'horizontalBar',
        data: {
            labels: labels,
            datasets: [{
                backgroundColor: colors,
                data: data
            }]
        },
        options: {
            legend: {
                display: false
            },
            tooltips: {
                displayColors: false
            },
            scales: {
                xAxes: [{
                    display: true,
                    ticks: {
                        suggestedMin: 0,
                        max: 1
                    }
                }]
            }
        }
    });
}

    ctx = document.getElementById('chart-tsne');
    new Chart(ctx, {
        type: 'bubble',
        data: {
            datasets: [
                {
                    label: 'Dataset_1',
                    backgroundColor: 'rgba(255, 0, 0, 0.8)',
                    data: [
                         { x: 1, y: 1, r: 10 },
                         { x: 2, y: 2, r: 10 },
                         { x: 3, y: 3, r: 10 }
                    ]
                },
                 {
                    label: 'Dataset_2',
                    backgroundColor: 'rgba(0, 255, 0, 0.8)',
                    data: [
                         { x: 5, y: 5, r: 10 },
                         { x: 6, y: 6, r: 10 },
                         { x: 7, y: 7, r: 10 }
                    ]
                },
                 {
                    label: 'Dataset_3',
                    backgroundColor: 'rgba(0, 0, 255, 0.8)',
                    data: [
                         { x: 10, y: 10, r: 10 },
                         { x: 11, y: 11, r: 10 },
                         { x: 12, y: 12, r: 10 }
                    ]
                }
            ]
        },
        options: {
            legend: {
                display: true,
                position: 'right'
            },
            tooltips: {
                displayColors: false
            }
        }
    });