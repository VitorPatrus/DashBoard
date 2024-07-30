// API
var listaAtualizacao = [];
var indice = 0;
var valorTotalAcumulado = 0;
var valorTotalAcumuladoTMS = 0;
var valorTotalAcumuladoERP = 0;
var valorMetaEconometro = 0;
var valorTotalEstimativa = 0;


var getDados = async function () {
  // URL do endpoint do seu controller
  var url = 'https://localhost:7052/econometro';
  // Fazendo a requisição GET usando fetch
  await fetch(url)
    .then(response => response.json()) // Parseia a resposta como JSON
    .then(data => {

      listaAtualizacao = data.atualizacoes;
      valorTotalAcumulado = data.valorTotalAcumulado;
      valorTotalAcumuladoTMS = data.valorTotalAcumuladoTMS;
      valorTotalAcumuladoERP = data.valorTotalAcumuladoERP;
      valorMetaEconometro = data.meta;
      valorTotalEstimativa = data.estimativa;

      var acumuladoMensal = ["x"];
      var acumulaMensalX = ["Evolução Mensal"];

      data.totalAcumulado.forEach((area) => {
        acumuladoMensal.push(area.chave);
        acumulaMensalX.push(area.valor)
      });

      var acumulaPorMes = [acumuladoMensal, acumulaMensalX];
      var acumula = [];
      data.totalAreas.forEach((area) => acumula.push([area.chave, area.valor]));


      var lbleconometroAnoAnterior = document.getElementById('econometroAnoAnterior');
      lbleconometroAnoAnterior.innerHTML = data.valorAnoAnterior.toLocaleString('pt-br', { style: 'currency', currency: 'BRL' });

      var lblmeta = document.getElementById('meta');
      lblmeta.innerHTML = data.meta.toLocaleString('pt-br', { style: 'currency', currency: 'BRL' });

      var lblpercentual = document.getElementById('percentual');
      lblpercentual.innerHTML = data.percentualAumento + '%';

      var lblestimativa = document.getElementById('estimativa');
      lblestimativa.innerHTML = data.estimativa.toLocaleString('pt-br', { style: 'currency', currency: 'BRL' });

      var lblvalorTotalAcumuladoTMS = document.getElementById('valorTotalAcumuladoTMS');
      lblvalorTotalAcumuladoTMS.innerHTML = data.valorTotalAcumuladoTMS.toLocaleString('pt-br', { style: 'currency', currency: 'BRL' });

      var lblvalorTotalAcumuladoERP = document.getElementById('valorTotalAcumuladoERP');
      lblvalorTotalAcumuladoERP.innerHTML = data.valorTotalAcumuladoERP.toLocaleString('pt-br', { style: 'currency', currency: 'BRL' });

      var lbltotalAcumulado = document.getElementById('totalAcumulado');
      lbltotalAcumulado.innerHTML = (data.valorTotalAcumuladoERP + data.valorTotalAcumuladoTMS).toLocaleString('pt-br', { style: 'currency', currency: 'BRL' });

      var lblpercentualEstimativa = document.getElementById('percentualEstimativa');
      lblpercentualEstimativa.innerHTML = data.percentualEstimativa + '%';

      // chart 1
      var chart = bb.generate({
        size: {
          width: 470,
          height: 245
        },
        data: {
          columns: acumula,
          type: "pie", // for ESM specify as: pie()
          colors: {
            ADM: "#FF4500",   // Laranja-Vermelho
            COM: "#32CD32",   // Verde-Limão
            COT: "#1E90FF",   // Azul-Dodger
            EDI: "#FFD700",   // Dourado
            FIN: "#9400D3",   // Violeta-Escuro
            FIS: "#00CED1",   // Turquesa-Escuro
            OPEX: "#e70432",  // Magenta
            RH: "#FFA500",    // Laranja
            TI: "#FF1493"     // Rosa-Intenso
        }
        
        },
        pie: {
          label: {
        format: function (value, ratio, id) {
          if (Math.abs(value) >= 1e6) {
            return (value / 1e6).toFixed(1) + 'M';
          }
          if (Math.abs(value) >= 1e3) {
            return (value / 1e3).toFixed(1) + 'k';
          }
          return value;
        },
        ratio: 1.3,
          },
        },
        bindto: "#cornerRadius_1"
      });


      // chart 2
      var chart2 = bb.generate({
        size: {
            width: 450,
            height: 220
        },
        data: {
            columns: acumulaPorMes,
            type: "bar",
            color: function (color, d) {
                return "#268fff";
            },
            arc: {
                radius: 110
            },
            labels: {
                colors: "#fff",
                format: function (valor) {
                    function formatarNumero(valor) {
                        const sufixos = ['', 'k', 'M', 'B'];
                        const magnitude = Math.floor(Math.log10(Math.abs(valor)) / 3);
                        const scaled = valor / Math.pow(10, magnitude * 3);
                        const formatted = scaled.toFixed(1);
                        return formatted + sufixos[magnitude];
                    }
                    if (isNaN(valor) || valor === null || valor === undefined) {
                        return "";
                    } else {
                        return formatarNumero(valor);
                    }
                }
            }
        },
        bar: {
            width: {
                ratio: 0.9
            },
            indices: {
                removeNull: false
            }
        },
        bindto: "#barIndices_2",
        legend: {
            hide: true
        },
        axis: {
            x: {
                type: "category",
                categories: ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"]
            },
            y: {
                tick: {
                    format: function (valor) {
                        function formatarNumero(valor) {
                            const sufixos = ['', 'k', 'M', 'B', 'T'];
                            const magnitude = Math.floor(Math.log10(Math.abs(valor)) / 3);
                            const scaled = valor / Math.pow(10, magnitude * 3);
                            const formatted = scaled.toFixed(1);
                            return formatted + sufixos[magnitude];
                        }
                        return formatarNumero(valor);
                    }
                }
            }
        },
      
    });
    })
    .catch(error => {
      // Trata erros de requisição
      console.error('Erro na requisição:', error);
    });
}

var atualizar = async function () {
  if (listaAtualizacao.length === 0 || indice == listaAtualizacao.length) {
    await getDados();
  }

  const totalEconometroAtualizacao = (valorTotalAcumulado + listaAtualizacao[indice].valor);
  const totalEconometroAtualizacaoTMS = (valorTotalAcumuladoTMS + listaAtualizacao[indice].valorTMS);
  const totalEconometroAtualizacaoERP = (valorTotalAcumuladoERP + listaAtualizacao[indice].valorERP);

  var lblatualizacao = document.getElementById('totalAcumulado');
  var lblatualizacaoTMS = document.getElementById('valorTotalAcumuladoTMS');
  var lblatualizacaoERP = document.getElementById('valorTotalAcumuladoERP');
  lblatualizacao.innerHTML = totalEconometroAtualizacao.toLocaleString('pt-br', { style: 'currency', currency: 'BRL' });
  lblatualizacaoTMS.innerHTML = totalEconometroAtualizacaoTMS.toLocaleString('pt-br', { style: 'currency', currency: 'BRL' });;
  lblatualizacaoERP.innerHTML = totalEconometroAtualizacaoERP.toLocaleString('pt-br', { style: 'currency', currency: 'BRL' });

  indice++;


  // ECONOMETRO
  const econometroProgressBar = document.getElementById('econometro');
  const econometro = (totalEconometroAtualizacao / valorMetaEconometro * 100);
  const econometroFormatado = econometro.toFixed(2);

  econometroProgressBar.setAttribute('aria-valuenow', econometroFormatado);
  econometroProgressBar.style.width = `${econometroFormatado}%`;
  econometroProgressBar.textContent = `${econometroFormatado}%`;

  const econometro1ProgressBar = document.getElementById('econometro1');
  econometro1ProgressBar.setAttribute('aria-valuenow', econometroFormatado);
  econometro1ProgressBar.style.width = `${econometroFormatado}%`;
  econometro1ProgressBar.textContent = `${econometroFormatado}%`;

  if (econometroFormatado >= 100) {
    econometroProgressBar.classList.add('bg-success');
    econometro1ProgressBar.classList.add('bg-success');
  }

  // TMS
  const progressBarTMS = document.getElementById('TMS');
  const percentualTMS = totalEconometroAtualizacaoTMS / totalEconometroAtualizacao * 100;
  const percentualTMSFormatado = percentualTMS.toFixed(0);

  progressBarTMS.setAttribute('aria-valuenow', percentualTMSFormatado);
  progressBarTMS.style.width = `${percentualTMSFormatado}%`;
  progressBarTMS.textContent = `${percentualTMSFormatado}%`;
  if (percentualTMSFormatado >= 100) {
    progressBarTMS.classList.add('bg-success');
  }

  // ERP 
  const progressBarERP = document.getElementById('ERP');
  const percentualERP = totalEconometroAtualizacaoERP / totalEconometroAtualizacao * 100;
  const percentualERPFormatado = percentualERP.toFixed(0);

  progressBarERP.setAttribute('aria-valuenow', percentualERPFormatado);
  progressBarERP.style.width = `${percentualERPFormatado}%`;
  progressBarERP.textContent = `${percentualERPFormatado}%`;
  if (percentualERPFormatado >= 100) {
    progressBarERP.classList.add('bg-success');
  }

  // ESTIMATIVA 2024
  var date1 = new Date("01/01/" + new Date().getFullYear());
  var date2 = new Date();
  date2.setDate(date2.getDate() - 1);
  var timeDiff = Math.abs(date2.getTime() - date1.getTime());
  var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));

  const progressBarEstimativa = document.getElementById('Estimativa');
  const percentualEstimativa = valorTotalEstimativa / valorMetaEconometro * 100;
  const percentualEstimativaFormatado = percentualEstimativa.toFixed(1);

  progressBarEstimativa.setAttribute('aria-valuenow', percentualEstimativaFormatado);
  progressBarEstimativa.style.width = `${percentualEstimativaFormatado}%`;
  progressBarEstimativa.textContent = `${percentualEstimativaFormatado}%`;
  if (percentualEstimativaFormatado >= 100) {
    progressBarEstimativa.classList.add('bg-success');
  }
};

atualizar();

setInterval(function () {
  atualizar();
}, 5000);



// Relógio

const showTimeNow = () =>{
  //Selecinando a tag de destino
    const clockTag = document.querySelector('time');
    
  //Instanciando a classe Date
    let dateNow = new Date();
  //pegando os valores desejados
    let hh = dateNow.getHours();
    let mm = dateNow.getMinutes();
    let ss = dateNow.getSeconds();
    
  //validando a necessidade de adicionar zero na exibição
    hh = hh < 10 ? '0'+ hh : hh; 
    mm = mm < 10 ? '0'+ mm : mm; 
    ss = ss < 10 ? '0'+ ss : ss; 
     
  // atribuindo os valores e montando o formato da hora a ser exibido
    clockTag.innerText = hh +':'+ mm +':'+ ss;
  }
  //executando a funcao a cada 1 segundo
  showTimeNow()
  setInterval(showTimeNow, 1000);