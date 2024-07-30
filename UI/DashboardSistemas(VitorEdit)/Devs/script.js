
	// Chamando a API
	function clickColaboradorFuncao(id) {
		document.idColaborador = id;
		var apiUrl = 'https://localhost:7052/Colaborador/ColaboradorDashboard?id=' + id;
		$.get(apiUrl, function (data) {

			function formatarHora(str) {
				return String(Math.trunc(str)).padStart(2, '0') + ':' + String(Math.trunc(60 * (str - Math.trunc(str)))).padStart(2, '0');
			}
			document.engajamento = data.engajamento;
			document.lista = data.atividades;
			document.devOps = data.devOps;


			$('#nome_colaborador').text(data.nome);
			$('#email_colaborador').text(data.email);
			$('#img_colaborador').attr('src', 'data:image/jpeg;base64,' + data.fotoColaborador);
			$('#time_colaborador').attr('src', 'data:image/jpeg;base64,' + data.fotoTime);
			$('#TIME').text(data.time);
			$('#cargo_colaborador').text(data.cargo);
			$('#valorEngajamento').text(data.engajamento + '%');
			$('#txt_horas_individuais').text(data.hE_Individual+ 'h');
			$('#txt_horasEquipe').text(data.hE_Equipe + 'h');
			$('#devOps').text(data.devOps);
			$('#tmetric').text(formatarHora(data.totalApropriado));
			$('#trabalhado').text(formatarHora(data.totalPonto));
			$('#pj').css("display", data.pj ? "" : "none");
			$('#clt').css("display", data.pj ? "none" : "");
			$('#horasPJ').text(data.hE_Individual + "h ");


			// Obtendo os valores das strings dos elementos HTML
			var valorHorasIndividuaisString = document.getElementById("txt_horas_individuais").innerText;
			var valorHorasEquipeString = document.getElementById("txt_horasEquipe").innerText;

			var corHorasIndividuais = valorHorasIndividuaisString.startsWith('-') ? 'red' : 'green';
			var corHorasEquipe = valorHorasEquipeString.startsWith('-') ? 'red' : 'green';

			document.getElementById("txt_horas_individuais").style.color = corHorasIndividuais;
			document.getElementById("txt_horasEquipe").style.color = corHorasEquipe;


			// Grafico apropriação
			// Assume data.resumoApropriacao is available and contains the necessary data
			var columns = [];
			var colors = {};

			// Itera sobre data.resumoApropriacao e popula as colunas e cores
			$.each(data.resumoApropriacao, function (index, apropriacao) {
				columns.push([apropriacao.tipo, apropriacao.valor]);
				colors[apropriacao.tipo] = apropriacao.color;
			});

			function trocaCor() {
				var elemento = $('#valorEngajamento');
				var valor = parseInt(elemento.text().replace('%', ''));

				if (valor > 100) {
					elemento.css('color', 'red');
				}
				else {
					elemento.css('color', 'black');
				}
			}

			trocaCor();

			// Gera o gráfico
			function renderChart() {
				var chart = bb.generate({
					size: {
						width: document.getElementById("piechart").offsetWidth,
						height: 310 // Defina uma altura adequada para o gráfico
					},
					data: {
						columns: columns,
						type: "pie",
						onclick: function (d, i) {
							console.log("onclick", d, i);
						},
						onover: function (d, i) {
							console.log("onover", d, i);
						},
						onout: function (d, i) {
							console.log("onout", d, i);
						},
						colors: colors
					},
					bindto: "#piechart"
				});
				return chart;
			}

			// Renderiza o gráfico inicialmente
			var chart = renderChart();

			// Atualiza o tamanho do gráfico quando a janela é redimensionada
			window.addEventListener("resize", function () {
				chart.resize({
					width: document.getElementById("piechart").offsetWidth,
					height: 310
				});
			});


			// DESENHA A TABELA
			var trs = document.getElementById("atividades").getElementsByTagName("tr");

			// Remove all rows
			while (trs.length > 0) {
				trs[0].parentNode.removeChild(trs[0]);
			}

			$('#atividades').append('<tr><th scope="col">ID</th><th scope="col">Atividade</th><th scope="col">Ticket</th><th scope="col">Tipo</th><th scope="col">Data</th><th scope="col">Horas</th></tr>');
			var indice = 1;
			$.each(data.atividades, function (index, chapter) {
				$('#atividades').append('<tr><th scope="row">' + indice++ + '</th><td>' + chapter.atividade + '</td><td>' + chapter.ticket + '</td><td>' + chapter.tipo + '</td><td>' + chapter.data.substr(0, 10) + '</td><td>' + moment(chapter.horas).format("HH:mm") + '</td></tr>');
			});


			// Gráfico de ponteiro
			var chart2 = bb.generate({
				data: {
					columns: [
						["Péssimo", 50],
						["Ruim", 40],
						["Excelente", 10]
					],
					type: "gauge", // for ESM specify as: gauge()
				},
				size: {
					height: 120

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
						value: data.engajamento
					}
				},
				color: {
					pattern: [
						"#FF0000",
						"#FFFF00",
						"#008000"
					]
				},
				bindto: "#gaugeNeedle_1"
			});
// DevOps
var chart = bb.generate({
  data: {
    columns: [
	["DevOps x TMetric", data.devOps]
    ],
    type: "gauge", // for ESM specify as: gauge()
    onclick: function(d, i) {
	console.log("onclick", d, i);
   }
},
  gauge: {},
  color: {
    pattern: [
      "orange","#00b050"],
    threshold: {
      values: [
        94.9
      ]
    }
  },
  
  size: {
    height: 140
  },
  bindto: "#gaugeChart"
});

			// Grafico Evolução
			var listaEvolucao = [
				['Year', 'Engajamento']];
			$.each(data.evolucaoEngajamento, function (index, evolucao) {
				listaEvolucao.push([evolucao.data, evolucao.valor]);
			});
			google.charts.load('current', { 'packages': ['corechart'] });
			google.charts.setOnLoadCallback(drawChart);

			function drawChart() {
				var data = google.visualization.arrayToDataTable(listaEvolucao);

				var view = new google.visualization.DataView(data);
				view.setColumns([0, 1, {
					calc: 'stringify',
					sourceColumn: 1,
					type: 'string',
					role: 'annotation',
				}]);

				var options = {
					height: 150,
					width: 300,
					title: '',
					curveType: 'function',
					legend: { position: 'none' },
					backgroundColor: 'transparent',
					hAxis: { gridlines: { count: 0 } },
					vAxis: { gridlines: { count: 0 }, textPosition: 'none' },
					annotations: {
						textStyle: {
							fontSize: 14,
							bold: true
						}
					}


				};


				var chart = new google.visualization.LineChart(document.getElementById('curve_chart'));

				chart.draw(view, options);
			}

			


			// var top4Selecionados = top3.slice(0, 3);
			// top4Selecionados.splice(0,0,['Time ERP', 52, './Content/Images/time-erp.jpg']);
			$('#primeiro').html('<img class="rounded-circle me-1" width="24" height="24" src="data:image/jpeg;base64,' + data.fotoTime + '" alt="rounded-circle me-1 flex">' + data.time + ' ' + data.engajamentoTime + '%');
			data.topEngajamento.forEach(function (colaborador, indice) {
				var elemento = '#segundo';
				if (indice == 1) {
					elemento = '#terceiro';
				} else if (indice == 2) {
					elemento = '#quarto';
				} else if (indice == 3) {
					elemento = '#quinto';
				}
				$(elemento).html('<img class="rounded-circle me-1" width="24" height="24" src="data:image/jpeg;base64,' + colaborador.foto + '" alt="rounded-circle me-1 flex">' + colaborador.nome + ' ' + colaborador.percentual + '%');
			});
		});



	}
	function colaboradorFuncao(nomeColaborador, cargoColaborador, imagemColaborador, imagemTime, valorEngajamento, txt_horas_individuais, horasDaEquipe, totalIndividual, totalPonto, lancamentoPorTipo) {
		// $('#nome_colaborador').text(nomeColaborador);
		$('#cargo_colaborador').text(cargoColaborador);
		$('#img_colaborador').attr('src', imagemColaborador);
		$('#time_colaborador').attr('src', imagemTime);
		$('#valorEngajamento').text(valorEngajamento + '%');
		$('#txt_horas_individuais').text(txt_horas_individuais);
		$('#txt_horasEquipe').text(horasDaEquipe);
		$('#totalIndividual').text(' Engajamento: ' + totalIndividual + 'h / ' + totalPonto + 'h');


		// DESENHA A TABELA
		var trs = document.getElementById("atividades").getElementsByTagName("tr");

		// Remove all rows
		while (trs.length > 0) {
			trs[0].parentNode.removeChild(trs[0]);
		}

		var a = listaAtividade.filter(function (obj) { return obj.nomeColaborador == nomeColaborador; });
		var indice = 1;
		$.each(a, function (index, chapter) {
			$('#atividades').append('<tr><th scope="row">' + indice++ + '</th><td>' + chapter.atividade + '</td><td>' + chapter.ticket + '</td><td>' + chapter.tipo + '</td><td>' + chapter.data + '</td><td>' + chapter.horas + '</td></tr>');
		});

	}



	clickColaboradorFuncao('3F7E1A71-815A-4397-D725-08DC62DFD032');




	//const nome = document.getElementById("nome_colaborador").innerText;
	var sendMail = (oficial) => {
		const content = document.querySelector("#capture");
		let id = document.idColaborador;
		// Renderizar o conteúdo HTML como uma imagem de canvas
		html2canvas(content, {
			scale: 2, // Define a escala para melhor qualidade (opcional)
			logging: false // Desativa os logs para evitar excesso de mensagens no console (opcional)
		}).then(canvas => {
			// Converta o canvas para uma URL de dados no formato JPG
			const jpgDataUrl = canvas.toDataURL("image/jpeg");

			// Abra um link com a URL de dados para que o usuário possa salvar a imagem
			// const link = document.createElement('a');
			// link.href = jpgDataUrl;
			// link.download = "c:\\Temp\\" + id + ".jpg"; // Nome do arquivo JPG a ser baixado
			// link.click();
			var apiUrl = 'https://localhost:7052/Colaborador/SendEmail';
			//  $.post(apiUrl,
			//  {
			// 	foto: "jpgDataUrl",
			// 	id: "",
			// 	periodo: "",
			// 	engajamento: "",
			//  }
			//  ,
			//   function (data) {

			//  });


			jQuery.ajax({
				'type': 'POST',
				'url': apiUrl,
				'contentType': 'application/json',
				'data': JSON.stringify({
					foto: jpgDataUrl,
					id: document.idColaborador,
					periodo: document.periodo,
					lista: document.lista,
					engajamento: document.engajamento,
					devOps: document.devOps,
					oficial: oficial
				}),
				'dataType': 'json',
				'success': function (data) { }
			});
		});
		alert("Email Enviado");
	};
	// Email
	const btnGenerate = document.querySelector("#sendMailButton");
	btnGenerate.addEventListener("click",() => {sendMail(true);});


	const generateTeste = document.querySelector("#sendMailTeste");
	generateTeste.addEventListener("click",() => {sendMail(false);});





// // Define the value that is missing for data1 to reach 100%
// var data1Remaining = 20;
// var data1Completed = 80; // This represents the completed portion

// // Define the chart data
// var chart = bb.generate({
//   data: {
//     columns: [
//       ["data1_completed", data1Completed], 
//       ["data1_remaining", data1Remaining] 
//     ],
//     type: "donut", // for ESM specify as: donut()
//     colors: {
//       data1_completed: "green", // Color for the completed portion (orange)
//       data1_remaining: "#f4f2f0" // Color for the remaining portion (grey)
//     }
//   },
//   legend: {
//     show: false // Remove the legend
//   },
//   donut: {
//     title: "DevOps", // Display the completion percentage in the center of the donut
//     width: 45 // Adjust this width as needed
//   },
//   size: {
//     width: 180
//   },
//   bindto: "#multilineTitle"
// });
