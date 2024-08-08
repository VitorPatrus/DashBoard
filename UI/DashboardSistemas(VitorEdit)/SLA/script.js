
var chart2 = bb.generate({
  data: {
    columns: [
      ["SLA", 95]
    ],
    type: "gauge"
  },
  size: {
    height: 130,
    width: 200
  },
  color: {
    pattern: ["red", "#f7e911", "#13ec00"], 
    threshold: {
      values: [70, 91] 
    }
  },
  label: {
    format: function(value) {
      return value + "%";
    }
  },
  arc: {
    needle: {
      show: false,
      length: 80,
      width: 5
    }
  },
  gauge: {
    label: {
      extents: function(value) {
        return value + "%";
      },
      format: function(value) {
        return value + "%";
      }
    }
  },
  bindto: "#gaugeNeedle_2"
});


// GRAFICO DE BARRAS
am5.ready(function() {
  var rootBarChart = am5.Root.new("chartdiv");

  var myTheme = am5.Theme.new(rootBarChart);
  myTheme.rule("Grid", ["base"]).setAll({
    strokeOpacity: 0.1
  });

  rootBarChart.setThemes([
    am5themes_Animated.new(rootBarChart),
    myTheme
  ]);

  var chart = rootBarChart.container.children.push(
    am5xy.XYChart.new(rootBarChart, {
      panX: false,
      panY: false,
      wheelX: "none",
      wheelY: "none",
      paddingLeft: 0
    })
  );

  var yRenderer = am5xy.AxisRendererY.new(rootBarChart, {
    minGridDistance: 30,
    minorGridEnabled: true
  });
  yRenderer.grid.template.set("location", 1);

  var yAxis = chart.yAxes.push(
    am5xy.CategoryAxis.new(rootBarChart, {
      maxDeviation: 0,
      categoryField: "country",
      renderer: yRenderer
    })
  );

  var xAxis = chart.xAxes.push(
    am5xy.ValueAxis.new(rootBarChart, {
      maxDeviation: 0,
      min: 0,
      renderer: am5xy.AxisRendererX.new(rootBarChart, {
        visible: true,
        strokeOpacity: 0.1,
        minGridDistance: 80
      })
    })
  );

  var series = chart.series.push(
    am5xy.ColumnSeries.new(rootBarChart, {
      name: "Series 1",
      xAxis: xAxis,
      yAxis: yAxis,
      valueXField: "value",
      sequencedInterpolation: true,
      categoryYField: "country"
    })
  );

  var columnTemplate = series.columns.template;

  columnTemplate.setAll({
    draggable: true,
    cursorOverStyle: "pointer",
    tooltipText: "arraste para reposicionar",
    cornerRadiusBR: 10,
    cornerRadiusTR: 10,
    strokeOpacity: 0
  });
  columnTemplate.adapters.add("fill", (fill, target) => {
    return chart.get("colors").getIndex(series.columns.indexOf(target));
  });

  columnTemplate.adapters.add("stroke", (stroke, target) => {
    return chart.get("colors").getIndex(series.columns.indexOf(target));
  });

  columnTemplate.events.on("dragstop", () => {
    sortCategoryAxis();
  });

  function getSeriesItem(category) {
    for (var i = 0; i < series.dataItems.length; i++) {
      var dataItem = series.dataItems[i];
      if (dataItem.get("categoryY") == category) {
        return dataItem;
      }
    }
  }

  function sortCategoryAxis() {
    series.dataItems.sort(function(x, y) {
      return y.get("graphics").y() - x.get("graphics").y();
    });

    var easing = am5.ease.out(am5.ease.cubic);

    am5.array.each(yAxis.dataItems, function(dataItem) {
      var seriesDataItem = getSeriesItem(dataItem.get("category"));

      if (seriesDataItem) {
        var index = series.dataItems.indexOf(seriesDataItem);
        var column = seriesDataItem.get("graphics");

        var fy =
          yRenderer.positionToCoordinate(yAxis.indexToPosition(index)) -
          column.height() / 2;

        if (index != dataItem.get("index")) {
          dataItem.set("index", index);

          var x = column.x();
          var y = column.y();

          column.set("dy", -(fy - y));
          column.set("dx", x);

          column.animate({ key: "dy", to: 0, duration: 600, easing: easing });
          column.animate({ key: "dx", to: 0, duration: 600, easing: easing });
        } else {
          column.animate({ key: "y", to: fy, duration: 600, easing: easing });
          column.animate({ key: "x", to: 0, duration: 600, easing: easing });
        }
      }
    });

    yAxis.dataItems.sort(function(x, y) {
      return x.get("index") - y.get("index");
    });
  }

  var data = [
    { country: "Gestão Vol", value: 99 },
    { country: "CTE", value: 89 },
    { country: "Nota Fiscal", value: 92 },
    { country: "Tab Preço", value: 90 },
    { country: "SMP", value: 97 }
  ];

  data.sort(function(a, b) {
    return a.value - b.value;
  });

  yAxis.data.setAll(data);
  series.data.setAll(data);

  series.appear(1000);
  chart.appear(1000, 100);
}); // end am5.ready()


// GRAFICO PONTEIRO
am5.ready(function() {
  var rootPieChart = am5.Root.new("meugrafico");

  rootPieChart.setThemes([
    am5themes_Animated.new(rootPieChart)
  ]);

  var chart = rootPieChart.container.children.push(
    am5percent.PieChart.new(rootPieChart, {
      endAngle: 270,
      layout: rootPieChart.verticalLayout,
    })
  );

  var series = chart.series.push(
    am5percent.PieSeries.new(rootPieChart, {
      valueField: "value",
      categoryField: "category",
      endAngle: 270
    })
  );
  series.set("colors", am5.ColorSet.new(rootPieChart, {
    colors: [
      am5.color(0x00ff00),  // Verde
      am5.color(0xff0000),  // Vermelho
      am5.color(0xffa500)   // Laranja
    ]
}));

var gradient = am5.RadialGradient.new(rootPieChart, {
    stops: [
      { color: am5.color(0x00ff00) },  // Verde
      { color: am5.color(0xff0000) },  // Vermelho
      { color: am5.color(0xffa500) }   // Laranja
    ]
});

series.slices.template.setAll({
    strokeWidth: 2,
    stroke: am5.color(0xffffff),
    cornerRadius: 10,
    shadowOpacity: 0.1,
    shadowOffsetX: 2,
    shadowOffsetY: 2,
    shadowColor: am5.color(0x000000),
    fillPattern: am5.GrainPattern.new(rootPieChart, {
      maxOpacity: 0.2,
      density: 0.5,
      colors: [
        am5.color(0x00ff00),  // Verde
        am5.color(0xff0000),  // Vermelho
        am5.color(0xffa500)   // Laranja
      ]
    })
});

  series.slices.template.states.create("hover", {
    shadowOpacity: 1,
    shadowBlur: 10
  });

  series.ticks.template.setAll({
    strokeOpacity: 0.4,
    strokeDasharray: [2, 2]
  });

  series.states.create("hidden", {
    endAngle: -90
  });

  series.data.setAll([
    { category: "Dentro", value: 30 },
    { category: "Pendente", value: 35 },
    { category: "Fora", value: 35 }
  ]);

  var legend = chart.children.push(am5.Legend.new(rootPieChart, {
    centerX: am5.percent(0),
    x: am5.percent(0),
    marginTop: 15,
    marginBottom: 15
  }));
  legend.markerRectangles.template.adapters.add("fillGradient", function() {
    return undefined;
  });

  series.appear(1000, 100);
}); // end am5.ready()

