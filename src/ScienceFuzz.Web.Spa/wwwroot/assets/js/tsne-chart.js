//function initTsneChart() {
//    ctx = document.getElementById('chart-tsne');
//    new Chart(ctx, {
//        type: 'bubble',
//        data: {
//            datasets: [
//                {
//                    label: 'Dataset_1',
//                    backgroundColor: 'rgba(255, 0, 0, 0.8)',
//                    data: [
//                        { x: 1, y: 1, r: 10 },
//                        { x: 2, y: 2, r: 10 },
//                        { x: 3, y: 3, r: 10 }
//                    ]
//                },
//                {
//                    label: 'Dataset_2',
//                    backgroundColor: 'rgba(0, 255, 0, 0.8)',
//                    data: [
//                        { x: 5, y: 5, r: 10 },
//                        { x: 6, y: 6, r: 10 },
//                        { x: 7, y: 7, r: 10 }
//                    ]
//                },
//                {
//                    label: 'Dataset_3',
//                    backgroundColor: 'rgba(0, 0, 255, 0.8)',
//                    data: [
//                        { x: 10, y: 10, r: 10 },
//                        { x: 11, y: 11, r: 10 },
//                        { x: 12, y: 12, r: 10 }
//                    ]
//                }
//            ]
//        },
//        options: {
//            legend: {
//                display: true,
//                position: 'right'
//            },
//            tooltips: {
//                displayColors: false
//            }
//        }
//    });
//}



function setTsne(input) {
    var opt = {};
    opt.epsilon = 10;
    opt.perplexity = 3;
    opt.dim = 2;

    var tsne = new tsnejs.tSNE(opt);
    var dists = inputToDists(input);
    tsne.initDataRaw(dists);

    for (var k = 0; k < 10000; k++) {
        tsne.step();
    }

    var solution = tsne.getSolution();
    var chartData = solutionToChart(input, solution);

    setChart(chartData);
}



function setChart(data) {
    if (window.tsneChart != null) {
        window.tsneChart.destroy();
    }

    ctx = document.getElementById('chart-tsne');
    window.tsneChart = new Chart(ctx, {
        type: 'bubble',
        data: {
            datasets: data
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
}



function inputToDists(input) {
    var arr = new Array();

    input.forEach(function (scientist) {
        arr.push(scientist.contributions);
    });

    return arr;
}

function solutionToChart(input, solution) {
    
    var arr = new Array();

    for (var i = 0; i < input.length; i++) {
        var colors = randColor();

        arr.push({
            label: input[i].scientist,
            data: [{ x: solution[i][0], y: solution[i][1], r: 10 }],
            backgroundColor: colors.color,
            hoverBackgroundColor: colors.hover
        });
    }
  
    return arr;
}

function randColor() {
    var r = Math.random() * 255;
    var g = Math.random() * 255;
    var b = Math.random() * 255;

    var color = 'rgba(' + r + ', ' + g + ', ' + b + ', 0.5)';
    var hover = 'rgba(' + r + ', ' + g + ', ' + b + ', 0.8)';

    return {
        color: color,
        hover: hover
    };
}