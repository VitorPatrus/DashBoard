let pieChartRoot;
let barChartRoot;
let idColaborador;
let slaColaborador;
let listaForaPrazo;

function truncarTexto(texto, limite) {
  return texto.length > limite ? texto.substring(0, limite) + '...' : texto;
}

function clickColaboradorFuncao(id) {
  var apiUrl = 'https://localhost:7052/ColaboradorSLA/ColaboradorSLADashboard?id=' + id;
  idColaborador = id;

  $.get(apiUrl, function (responseData) {
    // Atualização de informações do colaborador
    $('#nome_colaborador').text(responseData.nome);
    $('#email_colaborador').text(responseData.email);
    $('#img_colaborador').attr('src', 'data:image/jpeg;base64,' + responseData.fotoColaborador);
    $('#time_colaborador').attr('src', 'data:image/jpeg;base64,' + responseData.fotoTime);
    $('#time_colaborador img').attr('src', 'data:image/jpeg;base64,' + responseData.fotoTime);
    $('#TIME').text(responseData.time);
    $('#cargo_colaborador').text(responseData.cargo);
    $('#individual_SLA').text(responseData.slA_Individual + '%');
    $('#time_SLA').text('Time: ' + responseData.slA_Time + '%');
    $('#sistemas_SLA').text('Sistemas: ' + responseData.slA_Sistemas + '%');

    $('#pessoal').text(responseData.pessoal);
    $('#setorial').text(responseData.setorial);
    $('#sistemas').text(responseData.sistemas);

    slaColaborador = responseData.slA_Individual;
    listaForaPrazo = responseData.tabelaForaPrazo;


    function decimalParaHoras(decimal) {
      const horas = Math.floor(Math.abs(decimal));
      const minutos = Math.round((Math.abs(decimal) - horas) * 60);
      return `${horas.toString().padStart(2, '0')}:${minutos.toString().padStart(2, '0')}`;
    }

    const decimalHoras = responseData.he ? responseData.he.horas : 0.0;
    const horasFormatadas = decimalParaHoras(decimalHoras);
    const s = decimalHoras < 0
      ? `-${horasFormatadas}h`
      : `${horasFormatadas}h`;

    $('#he').text(s);
    if (decimalHoras >= 0) {
      $('#he').css('color', 'green');
    }
    else {
      $('#he').css('color', 'red');
    }

    $('#fechadosPessoal').html(responseData.fechadosPessoal + '&nbsp;');
    $('#fechadosEquipe').html(responseData.fechadosEquipe + '&nbsp;');
    $('#fechadosSistema').html(responseData.fechadosSistemas + '&nbsp; ');
    $('#aguardando').text('Aguardando usuário: ' + responseData.aguardando);
    $('#total').text('Total de Chamados: ' + (responseData.pessoal + responseData.fechadosPessoal));

    $('#compensavel').text(responseData.hE_Compensavel + 'h');
    $('#nao_compensavel').text(responseData.hE_NaoCompensavel + 'h');
    $('#total_Horas').text(responseData.totalHoras + 'h');

    $('#primeiroNome').text(truncarTexto(responseData.topSLA[0].nome, 20));
    $('#primeiro').text(responseData.topSLA[0].percentual + '%');
    $('#segundoNome').text(truncarTexto(responseData.topSLA[1].nome, 20));
    $('#segundo').text(responseData.topSLA[1].percentual + '%');
    $('#terceiroNome').text(truncarTexto(responseData.topSLA[2].nome, 20));
    $('#terceiro').text(responseData.topSLA[2].percentual + '%');


    $('#leadTimePessoal').text(responseData.leadTime + ' dias');
    $('#leadTimeSistemas').text(responseData.leadTimeSistemas + ' dias');
    $('#leadTimeEquipe').text(responseData.leadTimeEquipe + ' dias');

    var nivelSLA = bb.generate({
      data: {
        columns: [
          ["SLA", responseData.slA_Individual]
        ],
        type: "gauge"
      },
      size: {
        height: 160,
        width: 160
      },
      color: {
        pattern: ["red", "#f7e911", "#13ec00"],
        threshold: {
          values: [70, 91]
        }
      },
      label: {
        format: function (value) {
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
          extents: function (value) {
            return value + "%";
          },
          format: function (value) {
            return value + "%";
          }
        }
      },
      bindto: "#graficoSLA"
    });

    preencherTabela(responseData.tabelaForaPrazo);
    preencherTabela2(responseData.tabelaPendentes);
    preencherTabelaServicos(responseData.servicos);

    var evolucaoAbertos = ["Abertos"].concat(responseData.evolucaoChamadosAbertos);
    var evolucaoFechados = ["Fechados"].concat(responseData.evolucaoChamadosFechados);


    var header = document.getElementById('card-header-evolucao');
    var penultimoValor = responseData.evolucaoChamadosAbertos[responseData.evolucaoChamadosAbertos[1]];
    var ultimoValor = responseData.evolucaoChamadosAbertos[responseData.evolucaoChamadosAbertos[0]];

    ultimoValor > penultimoValor ? header.style.backgroundColor = 'red' : header.style.backgroundColor = 'green';

    // Atualizando o gráfico de pizza
    if (pieChartRoot) {
      pieChartRoot.container.children.each(function (child) {
        if (child instanceof am5percent.PieChart) {
          const series = child.series.getIndex(0);
          series.data.setAll([
            { category: "Dentro", value: responseData.dentroPrazo },
            { category: "Fora", value: responseData.foraPrazo }
          ]);
        }
      });
    } else {
      pieChartRoot = am5.Root.new("meugrafico");
      pieChartRoot.setThemes([am5themes_Animated.new(pieChartRoot)]);


      const pieChart = pieChartRoot.container.children.push(
        am5percent.PieChart.new(pieChartRoot, {
          endAngle: 270,
          layout: pieChartRoot.verticalLayout,
        })
      );

      const pieSeries = pieChart.series.push(
        am5percent.PieSeries.new(pieChartRoot, {
          valueField: "value",
          categoryField: "category",
          endAngle: 270,
        })
      );

      pieSeries.set("colors", am5.ColorSet.new(pieChartRoot, {
        colors: [
          am5.color(0x00ff00),
          am5.color(0xff0000),
          am5.color(0xffa500)
        ]
      }));

      pieSeries.slices.template.setAll({
        strokeWidth: 2,
        stroke: am5.color(0xf2f2f2),
        cornerRadius: 10,
        shadowOpacity: 0.2,
        shadowOffsetX: 2,
        shadowOffsetY: 2,
        shadowColor: am5.color(0x00ff00)
      });

      pieSeries.data.setAll([
        { category: "Dentro", value: responseData.dentroPrazo },
        { category: "Fora", value: responseData.foraPrazo }
      ]);

      const legend = pieChart.children.push(am5.Legend.new(pieChartRoot, {
        centerX: am5.percent(0),
        x: am5.percent(0),
        marginTop: 15,
        marginBottom: 15,
      }));
      pieSeries.appear(1000, 100);
    }

    // Configurando o gráfico XY
    const processedMondays = lastMondays.slice(1).map(function (date) {
      const parts = date.split("/");
      return `2023-${parts[1]}-${parts[0]}`;
    });

    const openEvolutionData = evolucaoAbertos.slice(1).map(Number);
    const closedEvolutionData = evolucaoFechados.slice(1).map(Number);

    const chartData = processedMondays.map(function (date, index) {
      return {
        date: date,
        abertos: openEvolutionData[index],
        fechados: closedEvolutionData[index],
        total: openEvolutionData[index] + closedEvolutionData[index],
      };
    });

    barChartRoot = barChartRoot ?? am5.Root.new("chartdiv");
    const root = barChartRoot;
    root.setThemes([am5themes_Animated.new(root)]);
    root.dateFormatter.setAll({
      dateFormat: "yyyy-MM-dd",
      dateFields: ["valueX"]
    });

    root.container.children.clear();
    const xyChart = root.container.children.push(
      am5xy.XYChart.new(root, {
        panX: false,
        panY: false,
        wheelX: "panX",
        wheelY: "zoomX",
        layout: root.verticalLayout
      })
    );

    // Adicionar cursor
    const chartCursor = xyChart.set("cursor", am5xy.XYCursor.new(root, {
      behavior: "zoomX"
    }));
    chartCursor.lineY.set("visible", false);

    // Criar eixos
    xyChart.xAxes.clear();
    const xAxis = xyChart.xAxes.push(
      am5xy.DateAxis.new(root, {
        baseInterval: { timeUnit: "week", count: 1 },
        renderer: am5xy.AxisRendererX.new(root, {
          minorGridEnabled: true
        }),
        tooltip: am5.Tooltip.new(root, {}),
        tooltipDateFormat: "yyyy-MM-dd"
      })
    );

    xyChart.yAxes.clear();
    const primaryYAxis = xyChart.yAxes.push(
      am5xy.ValueAxis.new(root, {
        renderer: am5xy.AxisRendererY.new(root, {
          pan: "zoom"
        })
      })
    );

    const secondaryYAxisRenderer = am5xy.AxisRendererY.new(root, {
      opposite: true
    });
    secondaryYAxisRenderer.grid.template.set("forceHidden", true);

    const secondaryYAxis = xyChart.yAxes.push(
      am5xy.ValueAxis.new(root, {
        renderer: secondaryYAxisRenderer,
        syncWithAxis: primaryYAxis
      })
    );

    // Adicionar séries de dados
    const closedSeries = xyChart.series.push(
      am5xy.ColumnSeries.new(root, {
        name: "Total",
        xAxis: xAxis,
        yAxis: primaryYAxis,
        valueYField: "total",
        valueXField: "date",
        clustered: false,
        tooltip: am5.Tooltip.new(root, {
          pointerOrientation: "horizontal",
          labelText: "{name}: {valueY}"
        })
      })
    );

    closedSeries.columns.template.setAll({
      width: am5.percent(60),
      fillOpacity: 0.5,
      strokeOpacity: 0
    });

    closedSeries.data.processor = am5.DataProcessor.new(root, {
      dateFields: ["date"],
      dateFormat: "yyyy-MM-dd"
    });

    const openSeries = xyChart.series.push(
      am5xy.ColumnSeries.new(root, {
        name: "Fechados",
        xAxis: xAxis,
        yAxis: primaryYAxis,
        valueYField: "fechados",
        valueXField: "date",
        clustered: false,
        tooltip: am5.Tooltip.new(root, {
          pointerOrientation: "horizontal",
          labelText: "{name}: {valueY}"
        })
      })
    );

    openSeries.columns.template.set("width", am5.percent(40));
    const totalCallsSeries = xyChart.series.push(
      am5xy.SmoothedXLineSeries.new(root, {
        name: "Abertos",
        xAxis: xAxis,
        yAxis: primaryYAxis,
        valueYField: "abertos",
        valueXField: "date",
        tooltip: am5.Tooltip.new(root, {
          pointerOrientation: "horizontal",
          labelText: "{name}: {valueY}"
        })
      })
    );

    totalCallsSeries.strokes.template.setAll({
      strokeWidth: 2,
      stroke: am5.color(0xFF0000) // Cor vermelha
    });

    // Definir cores para a série de vendas abertas
    openSeries.columns.template.setAll({
      width: am5.percent(40),
      fill: am5.color(0x33FF57), // Cor verde
      strokeOpacity: 0
    });

    totalCallsSeries.strokes.template.setAll({
      strokeWidth: 2
    });

    totalCallsSeries.bullets.push(function () {
      return am5.Bullet.new(root, {
        sprite: am5.Circle.new(root, {
          stroke: totalCallsSeries.get("fill"),
          strokeWidth: 2,
          fill: root.interfaceColors.get("background"),
          radius: 5
        })
      });
    });
    const legend = xyChart.children.push(
      am5.Legend.new(root, {
        x: am5.p50,
        centerX: am5.p50,
        width: am5.percent(80),
        marginBottom: -20,
        layout: root.horizontalLayout // Define o layout para horizontal
      })
    );

    legend.data.setAll(xyChart.series.values);

    // Definir os dados para as séries
    closedSeries.data.setAll(chartData);
    openSeries.data.setAll(chartData);
    totalCallsSeries.data.setAll(chartData);

    // Animação de carregamento
    totalCallsSeries.appear(1000);
    xyChart.appear(1000, 100);
    //}); // fim de am5.ready()
  });
}

function preencherTabelaServicos(servicos) {
  var tbody = document.getElementById('servicos');
  tbody.innerHTML = '';

  servicos.forEach(function (servico) {
    var tr = document.createElement('tr');
    var tdServico = document.createElement('td');
    tdServico.textContent = servico.servico || 'N/A';
    tr.appendChild(tdServico);

    var tdNumero = document.createElement('td');
    tdNumero.textContent = servico.quantidade;
    tr.appendChild(tdNumero);

    tbody.appendChild(tr);
  });
}

function preencherTabela(tabelaForaPrazo) {
  var tbody = document.getElementById('atividades');
  tbody.innerHTML = '';

  tabelaForaPrazo.forEach(function (chamado) {
      var tr = document.createElement('tr');

      var tdTicket = document.createElement('td');
      tdTicket.textContent = chamado.numero;
      tr.appendChild(tdTicket);

      var solicitante = document.createElement('td');
      solicitante.textContent = truncarTexto(chamado.solicitante, 8);
      tr.appendChild(solicitante);

      var tdAtividade = document.createElement('td');
      tdAtividade.textContent = truncarTexto(chamado.assunto, 15);
      tr.appendChild(tdAtividade);

      var tdTipo = document.createElement('td');
      tdTipo.textContent = truncarTexto(chamado.servico, 13);
      tr.appendChild(tdTipo);

      var tdData = document.createElement('td');
      tdData.textContent = chamado.dataAbertura;
      tr.appendChild(tdData);

      var tdHoras = document.createElement('td');
      tdHoras.textContent = chamado.dataFechamento;
      tr.appendChild(tdHoras);

      tbody.appendChild(tr);
  });
}


function preencherTabela2(tabelaPendentes) {

  var tbody = document.getElementById('pendentes');
  tbody.innerHTML = '';

  tabelaPendentes.forEach(function (chamado) {
    var tr = document.createElement('tr');

    var tdTicket = document.createElement('td');
    tdTicket.textContent = chamado.numero;
    tr.appendChild(tdTicket);

    var solicitante = document.createElement('td');
    solicitante.textContent = truncarTexto(chamado.solicitante, 8);
    tr.appendChild(solicitante);

    var tdAtividade = document.createElement('td');
    tdAtividade.textContent = truncarTexto(chamado.assunto, 15);
    tr.appendChild(tdAtividade);

    var tdTipo = document.createElement('td');
    tdTipo.textContent = truncarTexto(chamado.servico, 15);
    tr.appendChild(tdTipo);

    var tdData = document.createElement('td');
    tdData.textContent = chamado.dataAbertura;
    tr.appendChild(tdData);

    tbody.appendChild(tr);
  });
}

function getLastWeekDays() {
  const today = new Date();
  const currentDay = today.getDay();
  const lastSunday = new Date(today);
  lastSunday.setDate(today.getDate() - currentDay - 7);

  const lastMonday = new Date(lastSunday);
  lastMonday.setDate(lastSunday.getDate() + 1);

  const lastFriday = new Date(lastSunday);
  lastFriday.setDate(lastSunday.getDate() + 5);
  const formatDate = (date) => {
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    return `${day}/${month}`;
  };
  return {
    monday: formatDate(lastMonday),
    friday: formatDate(lastFriday)
  };
}
function updateMessage(elementId) {
  const lastWeekDays = getLastWeekDays();
  const message = `Dados referentes aos dias ${lastWeekDays.monday} à ${lastWeekDays.friday}`;
  document.getElementById(elementId).textContent = message;
  document.periodo = `${lastWeekDays.monday} à ${lastWeekDays.friday}`
}
updateMessage('message1');
updateMessage('message2');

window.onload = function () {
  fetch('https://localhost:7052/ColaboradorSLA/GetColaboradores')
    .then(response => response.json())
    .then(data => {
      if (data && data.nomeColab) {
        $('#nome').text(data.nomeColab);
      }
      preencherSup(data);

      function preencherSup(data) {
        var timeSup = document.getElementById('timeSup');
        timeSup.innerHTML = '';

        data.sup.forEach(colaborador => {
          var listItem = document.createElement('li');
          listItem.style.borderRadius = '5px';
          listItem.addEventListener('mouseover', function () {
            listItem.style.border = '4px solid #c81238';
            listItem.style.backgroundColor = "#c81238";
            listItem.style.color = "#fff";
          });
          listItem.addEventListener('mouseout', function () {
            listItem.style.backgroundColor = "";
            listItem.style.color = "";
            listItem.style.border = '';
          });
          listItem.style.display = 'flex';
          listItem.style.alignItems = 'center';
          listItem.style.marginBottom = '10px';

          var img = document.createElement('img');
          img.src = 'data:image/jpeg;base64,' + colaborador.fotoColab;
          img.alt = colaborador.nomeColab;
          img.style.width = '30px';
          img.style.height = '30px';
          img.style.borderRadius = '50%';
          img.style.marginRight = '5px';
          img.style.cursor = 'pointer';

          var text = document.createElement('span');
          text.textContent = colaborador.nomeColab;
          text.style.cursor = 'pointer';

          listItem.appendChild(img);
          listItem.appendChild(text);

          listItem.onclick = function () {
            clickColaboradorFuncao(colaborador.idColaborador);
          };
          timeSup.appendChild(listItem);
        });
      }
    })
    .catch(error => console.error('Erro ao carregar colaboradores:', error));
}


function getLastMonday(date) {
  const day = date.getDay();
  const diff = (day === 0 ? -6 : 1) - day;
  return new Date(date.getFullYear(), date.getMonth(), date.getDate() + diff);
}

function getLastMondays(count) {
  const mondays = [];
  const today = new Date();
  let monday = getLastMonday(new Date(today));

  if (today.getDate() <= monday.getDate()) {
    monday.setDate(monday.getDate() - 7);
  }

  for (let i = 0; i < count; i++) {
    mondays.push(monday.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' }));
    monday.setDate(monday.getDate() - 7);
  }
  return mondays.reverse();
}
const lastMondays = getLastMondays(5);

function sendMail(oficial) {
  const content = document.querySelector("#capture");
  const id = document.idColaborador;

  html2canvas(content, {
    scale: 2,
    logging: false
  }).then(canvas => {
    const jpgDataUrl = canvas.toDataURL("image/jpeg");
    const apiUrl = 'https://localhost:7052/ColaboradorSLA/SendSLAEmail';

    $.ajax({
      type: 'POST',
      url: apiUrl,
      contentType: 'application/json',
      data: JSON.stringify({
        foto: jpgDataUrl,
        id: idColaborador,
        sla: slaColaborador,
        ListaForaPrazo: listaForaPrazo,
        oficial: oficial
      }),
      dataType: 'json',
    });
  });
}


/*
async function sendMail(oficial) {
  try {    
  const content = document.querySelector("#capture");
  html2canvas(content, {
    scale: 2,
    logging: false
  }).then(canvas => {
    const jpgDataUrl = canvas.toDataURL("image/jpeg");

      const jpgDataUrl = getJpgDataUrl();

      const apiUrl = 'https://localhost:7052/ColaboradorSLA/SendSLAEmail';

      const requestBody = {
          foto: jpgDataUrl,
          id: idColaborador,
          sla: slaColaborador,
          ListaForaPrazo: listaForaPrazo,
          oficial: oficial
      };

      const response = await fetch(apiUrl, {
          method: 'POST',
          headers: {
              'Content-Type': 'application/json'
          },
          body: JSON.stringify(requestBody)
      });

      if (!response.ok) {
          throw new Error('Erro na resposta do servidor');
      }

      const data = await response.json();
      console.log('E-mail enviado com sucesso:', data);
      alert('E-mail enviado com sucesso!');
      return true; // Sucesso
      
  });
  } catch (error) {
      console.error('Erro ao enviar e-mail:', error);
      alert('Falha ao enviar o e-mail. Por favor, tente novamente.');
      return false; // Falha
  }
}

function getJpgDataUrl() {
  const content = document.querySelector("#capture");
  const canvas = html2canvas(content, {
      scale: 2,
      logging: false
  });

  return canvas.toDataURL("image/jpeg");
}
*/