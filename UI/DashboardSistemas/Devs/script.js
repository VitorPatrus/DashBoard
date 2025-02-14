// Função principal chamada ao clicar em um colaborador
function clickColaboradorFuncao(id) {
	document.idColaborador = id;
	const apiUrl = `https://localhost:7052/Colaborador/ColaboradorDashboard?id=${id}`;

	$.get(apiUrl, function (data) {


		function formatarHora(str) {
			const negativo = str < 0;
			str = Math.abs(str);

			const horas = String(Math.trunc(str)).padStart(2, '0');
			const minutos = String(Math.trunc(60 * (str - Math.trunc(str)))).padStart(2, '0');

			return (negativo ? '-' : '') + `${horas}:${minutos}`;
		}

		function trocaCor() {
			const elemento = $('#valorEngajamento');
			const valor = parseInt(elemento.text().replace('%', ''));

			elemento.css('color', valor > 100 ? 'red' : 'black');
		}

		function getIcone(tipo) {
			switch (tipo) {
				case 'Feature': return '<i class="fas fa-trophy" style="color: #800080;"></i>';
				case 'Task': return '<i class="fas fa-clipboard-check" style="color: #FFD700;"></i>';
				case 'Bug': return '<i class="fas fa-bug" style="color: #FF0000;"></i>';
				case 'UserStory': return '<i class="fas fa-book-open" style="color: #0000FF;"></i>';
				case 'Epic': return '<i class="fas fa-crown" style="color: #FFA500;"></i>';
				case 'Change': return '<i class="fa-solid fa-droplet fa-flip-vertical" style="color: #339947;"></i>';
				default: return 'Erro';
			}
		}

		function renderChart(columns, colors) {
			return bb.generate({
				size: { width: $('#resumoApropriacao').width(), height: 310 },
				data: {
					columns: columns,
					type: "pie",
					colors: colors,
				},
				bindto: "#resumoApropriacao"
			});
		}

		function gerarGraficoDevOps(bindtoId, devOps) {
			bb.generate({
				data: { columns: [["DevOps x TMetric", devOps]], type: "gauge" },
				gauge: {},
				color: {
					pattern: ["orange", "#00b050"],
					threshold: { values: [94.9] }
				},
				size: { height: 140 },
				bindto: bindtoId
			});
		}

		function renderTabelaAtividades(atividades) {
			const $atividades = $('#atividades');
			$atividades.empty().append('<tr><th scope="col">ID</th><th scope="col">Atividade</th><th scope="col">Ticket</th><th scope="col">Tipo</th><th scope="col">Data</th><th scope="col">Horas</th></tr>');

			let indice = 1;
			atividades.forEach(chapter => {
				$atividades.append(`<tr><th scope="row">${indice++}</th><td>${chapter.atividade}</td><td>${chapter.ticket}</td><td>${chapter.tipo}</td><td>${chapter.data.substr(0, 10)}</td><td>${moment(chapter.horas).format("HH:mm")}</td></tr>`);
			});
		}

		function renderTabelaParents(parents, teto = 10) {
			const $parents = $('#parents');
			$parents.empty();

			let count = 0;
			parents.forEach(chapter => {
				if (count < teto) {
					$parents.append(`<tr><th scope="row">${getIcone(chapter.tipo)}</th><td>${chapter.titulo}</td><td>${formatarHora(chapter.horas)}</td></tr>`);
					count++;
				}
			});
		}

		// Atualiza informações do colaborador
		$('#nome_colaborador').text(data.nome);
		$('#email_colaborador').text(data.email);
		$('#img_colaborador').attr('src', `data:image/jpeg;base64,${data.fotoColaborador}`);
		$('#time_colaborador').attr('src', `data:image/jpeg;base64,${data.fotoTime}`);
		$('#TIME').text(data.time);
		$('#cargo_colaborador').text(data.cargo);
		$('#valorEngajamento').text(`${data.engajamento}%`);
		$('#devOps').text(data.devOps);
		$('#menssage').text(data.menssagem);
		$('#tmetric').text(formatarHora(data.totalApropriado));
		$('#trabalhado').text(formatarHora(data.totalPonto));
		$('#horasPJ').text(data.hE_Individual);
		$('#lbl_horas_individuais').text(data.pj ? "Carga Horaria: " : "Horas Extras: ");
		$('#txt_horas_individuais').text(formatarHora(data.hE_Individual) + 'h');
		$('#horasPJ').text(`${data.hE_Individual}h`);

		document.lista = data.atividades;
		document.engajamento = data.engajamento;
		document.devOps = data.devOps;

		// Define a cor do texto de horas individuais
		const valorHorasIndividuaisString = $('#txt_horas_individuais').text();
		const corHorasIndividuais = valorHorasIndividuaisString.startsWith('-') ? 'red' : 'green';
		$('#txt_horas_individuais').css('color', corHorasIndividuais);
		console.log(data.resumoApropriacao);

		// Paleta de cores fixa
		const colorPalette = {
			"Discovery": "#ffa500",   // Laranja amarelado intenso.
			"Ceremony": "#006600",   // Verde musgo profundo.
			"Management": "#0046B8", // Azul cobalto forte.
			"Delivery": "#6A00B9",   // Roxo vibrante.
			"Out of Office": "#e60aaf", // Rosa avermelhado intenso.
			"Bug": "#ff0000",        // Vermelho escuro vibrante.
			"Coaching": "#5C9A00"    // Verde oliva vivo.
		};
		
		// Colunas e cores para o gráfico
		const columns = data.resumoApropriacao.map(apropriacao => [`${apropriacao.tipo} (${apropriacao.horas}h)`, apropriacao.valor]);

		// Gráfico Pizza
		const colors = data.resumoApropriacao.reduce((acc, apropriacao) => {
			const label = `${apropriacao.tipo} (${apropriacao.horas}h)`;
			acc[label] = colorPalette[apropriacao.tipo] || "#CCCCCC"; // Cor padrão caso o tipo não esteja na paleta
			return acc;
		}, {});

		const chart = renderChart(columns, colors);

		gerarGraficoDevOps("#gaugeChart1", data.devOps);

		window.addEventListener("resize", () => chart.resize({
			width: $('#resumoApropriacao').width(),
			height: 310
		}));

		renderTabelaAtividades(data.atividades);
		renderTabelaParents(data.parents);

		$('#primeiro').html(`<img class="rounded-circle me-1" width="24" height="24" src="data:image/jpeg;base64,${data.fotoTime}" alt="rounded-circle me-1 flex">${data.time} ${data.engajamentoTime}%`);

		data.topEngajamento.forEach((colaborador, indice) => {
			const elemento = ['#segundo', '#terceiro', '#quarto', '#quinto'][indice] || '#primeiro';
			$(elemento).html(`<img class="rounded-circle me-1" width="24" height="24" src="data:image/jpeg;base64,${colaborador.foto}" alt="rounded-circle me-1 flex">${colaborador.nome} ${colaborador.percentual}%`);
		});

		// Gráfico de ponteiro
		bb.generate({
			data: { columns: [["Péssimo", 50], ["Ruim", 40], ["Excelente", 10]], type: "gauge" },
			size: { height: 120 },
			interaction: { enabled: false },
			legend: { show: false },
			gauge: {
				width: 40,
				label: { format: () => "" }
			},
			arc: {
				needle: { show: true, value: data.engajamento }
			},
			color: {
				pattern: ["#FF0000", "#FFFF00", "#008000"]
			},
			bindto: "#gaugeNeedle_1"
		});
	});
}

// Função para enviar email
function sendMail(oficial) {
	const content = document.querySelector("#capture");
	const id = document.idColaborador;
	debugger;
	console.log(document);

	html2canvas(content, {
		scale: 2,
		logging: false
	}).then(canvas => {
		const jpgDataUrl = canvas.toDataURL("image/jpeg");
		const apiUrl = 'https://localhost:7052/Colaborador/SendEmail';

		$.ajax({
			type: 'POST',
			url: apiUrl,
			contentType: 'application/json',
			data: JSON.stringify({
				foto: jpgDataUrl,
				id: id,
				periodo: document.periodo,
				lista: document.lista,
				engajamento: document.engajamento,
				devOps: document.devOps,
				oficial: oficial
			}),
			dataType: 'json',
		});
	});
}


// Event listeners para envio de email
$("#sendMailButton").click(() => sendMail(true));
$("#sendMailTeste").click(() => sendMail(false));

// Gerar Data Automática
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
		const month = String(date.getMonth() + 1).padStart(2, '0'); // Mês começa em 0
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

updateMessage('message');
