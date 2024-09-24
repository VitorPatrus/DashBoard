let pieChartRoot;

function clickColaboradorFuncao(id) {
  var apiUrl = 'https://localhost:7052/ColaboradorSLA/ColaboradorSLADashboard?id=' + id;

  $.get(apiUrl, function (responseData) {
    // Atualização de informações do colaborador
    $('#nome_colaborador').text(responseData.nome);
    $('#email_colaborador').text(responseData.email);
    $('#img_colaborador').attr('src', 'data:image/jpeg;base64,' + responseData.fotoColaborador);
    $('#time_colaborador').attr('src', 'data:image/jpeg;base64,' + responseData.fotoTime);
    $('#time_colaborador img').attr('src', 'data:image/jpeg;base64,' + responseData.fotoTime);
    $('#TIME').text(responseData.time);
    $('#cargo_colaborador').text(responseData.cargo);
    $('#time_SLA').text(responseData.slA_Time + '%');
    $('#individual_SLA').text(responseData.slA_Individual + '%');

    $('#pessoal').text(responseData.pessoal);
    $('#setorial').text(responseData.setorial);
    $('#sistemas').text(responseData.sistemas);

    $('#fechadosPessoal').text(responseData.fechadosPessoal);
    $('#fechadosEquipe').text(responseData.fechadosEquipe);
    $('#fechadosSistema').text(responseData.fechadosSistemas);
    $('#aguardando').text('Aguardando usuário: ' + responseData.aguardando);
    $('#total').text('Total de Chamados: ' + (responseData.pessoal + responseData.fechadosPessoal));

    $('#compensavel').text(responseData.hE_Compensavel + 'h');
    $('#nao_compensavel').text(responseData.hE_NaoCompensavel + 'h');
    $('#total_Horas').text(responseData.totalHoras + 'h');

    $('#primeiroNome').text(responseData.topSLA[0].nome);
    $('#primeiro').text(responseData.topSLA[0].percentual + '%');
    $('#segundoNome').text(responseData.topSLA[1].nome);
    $('#segundo').text(responseData.topSLA[1].percentual + '%');
    $('#terceiroNome').text(responseData.topSLA[2].nome);
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

bb.generate({
  data: {
    columns: [
      evolucaoAbertos,
      evolucaoFechados
    ],
    type: "line",
    colors: {
      Abertos: "#ff0000",
      Fechados: "#13ec00" 
    },
    labels: {
      backgroundColors: {
        Abertos: "white",
        Fechados: "white"
      },
      colors: {
        Abertos: "black",
        Fechados: "black"
      }
    }
  },
  axis: {
    x: {
      type: "category",
      categories: lastMondays
    },
    y: {
      min: Math.min(...evolucaoAbertos),
      max: Math.max(...evolucaoAbertos)
    }
  },
  bindto: "#dataLabelColors_2"
});

    

    var header = document.getElementById('card-header-evolucao');
    var penultimoValor = responseData.evolucaoChamadosAbertos[responseData.evolucaoChamadosAbertos[1]];
    var ultimoValor = responseData.evolucaoChamadosAbertos[responseData.evolucaoChamadosAbertos[0]];

    if (ultimoValor > penultimoValor) {
      header.style.backgroundColor = 'red';
    } else {
      header.style.backgroundColor = 'green';
    }

    // Atualizando o gráfico de pizza
    am5.ready(function () {
      if (pieChartRoot) {
        pieChartRoot.container.children.each(function (child) {
          if (child instanceof am5percent.PieChart) {
            var series = child.series.getIndex(0);
            series.data.setAll([
              { category: "Dentro", value: responseData.dentroPrazo },
              { category: "Fora", value: responseData.foraPrazo }
            ]);
          }
        });
      } else {
        pieChartRoot = am5.Root.new("meugrafico");

        pieChartRoot.setThemes([
          am5themes_Animated.new(pieChartRoot)
        ]);

        var chart = pieChartRoot.container.children.push(
          am5percent.PieChart.new(pieChartRoot, {
            endAngle: 270,
            layout: pieChartRoot.verticalLayout,
          })
        );

        var series = chart.series.push(
          am5percent.PieSeries.new(pieChartRoot, {
            valueField: "value",
            categoryField: "category",
            endAngle: 270,
          })
        );


        series.set("colors", am5.ColorSet.new(pieChartRoot, {
          colors: [
            am5.color(0x00ff00),
            am5.color(0xff0000),
            am5.color(0xffa500)
          ]
        }));

        series.slices.template.setAll({
          strokeWidth: 2,
          stroke: am5.color(0xf2f2f2),
          cornerRadius: 10,
          shadowOpacity: 0.2,
          shadowOffsetX: 2,
          shadowOffsetY: 2,
          shadowColor: am5.color(0x00ff00)
        });
        series.data.setAll([
          { category: "Dentro", value: responseData.dentroPrazo },
          { category: "Fora", value: responseData.foraPrazo }
        ]);

        var legend = chart.children.push(am5.Legend.new(pieChartRoot, {
          centerX: am5.percent(0),
          x: am5.percent(0),
          marginTop: 15,
          marginBottom: 15,
        }));

        series.appear(1000, 100);
      }
    });
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
    tdTicket.textContent = '#' + chamado.numero;
    tr.appendChild(tdTicket);

    var solicitante = document.createElement('td');
    solicitante.textContent = chamado.solicitante;
    tr.appendChild(solicitante);

    var tdAtividade = document.createElement('td');
    tdAtividade.textContent = chamado.assunto;
    tr.appendChild(tdAtividade);

    var tdTipo = document.createElement('td');
    tdTipo.textContent = chamado.servico;
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
    tdTicket.textContent = '#' + chamado.numero;
    tr.appendChild(tdTicket);

    var solicitante = document.createElement('td');
    solicitante.textContent = chamado.solicitante;
    tr.appendChild(solicitante);

    var tdAtividade = document.createElement('td');
    tdAtividade.textContent = chamado.assunto;
    tr.appendChild(tdAtividade);

    var tdTipo = document.createElement('td');
    tdTipo.textContent = chamado.servico;
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
  clickColaboradorFuncao('4D143095-82BC-42C5-EFFE-08DCBC682A35');

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
console.log(lastMondays);
