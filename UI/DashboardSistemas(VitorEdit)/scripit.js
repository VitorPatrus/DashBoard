// Gráfico ponteiro
//Amanda
var valorEngajamento = 14;

//João
//var valorEngajamento = 10;


var chart2 = bb.generate({
    data: {
        columns: [
            ["Péssimo", 25],
            ["Ruim", 25],
            ["Bom", 25],
            ["Excelente", 25]
        ],
        type: "gauge", // for ESM specify as: gauge()
    },
    size: {
        height: 125
    },
    interaction: {
        enabled: false
    },
    legend: {
        show: false
    },
    gauge: {
        width: 40,
        //title: "\n\n\n\n\n{=NEEDLE_VALUE}%",
        label: {
            format: function (value, ratio, id) { return ""; }
        }
    },
    arc: {
        needle: {
            show: true,
            value: valorEngajamento
        }
    },
    color: {
        pattern: [
            "#FF0000",
            "#FFA500",
            "#FFFF00",
            "#008000"
        ]
    },
    bindto: "#gaugeNeedle_1"
});
// google charts

google.charts.load('current', { 'packages': ['corechart'] });
google.charts.setOnLoadCallback(drawCharts);

function drawCharts() {
    drawChart1();
    drawChart2();
}

function drawChart1() {
    var data = google.visualization.arrayToDataTable([
        ['Task', 'Hours per Day'],
        ['Novo Roterizador', 11],
        ['Agenda', 2],
        ['Coleta', 2],

    ]);

    var options = {
        title: '',
        chartArea: { width: '100%', height: '75%' },
        legend: { position: 'bottom' }
    };

    var chart = new google.visualization.PieChart(document.getElementById('piechart'));

    chart.draw(data, options);
}

function drawChart2() {
    var data = google.visualization.arrayToDataTable([
        ['Task', 'Hours per Day'],
        ['COT', 11],
        ['OPEX', 2],
        ['CAP', 2],

    ]);

    var options = {
        title: '',
        chartArea: { width: '100%', height: '75%' },
        legend: { position: 'bottom' }
    };

    var chart = new google.visualization.PieChart(document.getElementById('piechart2'));

    chart.draw(data, options);
}
