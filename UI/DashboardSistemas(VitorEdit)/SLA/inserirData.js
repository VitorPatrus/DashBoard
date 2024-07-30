
function insertPreviousWeekDates() {
    var dataAtual = new Date();
    var diaAtual = dataAtual.getDay(); // 0 = Domingo, 1 = Segunda-feira, ..., 6 = Sábado
    var daysToSubtract = diaAtual + 6; // Subtraindo os dias da semana atual até sábado
    var segundaPassada = new Date(dataAtual.getTime() - (daysToSubtract * 24 * 60 * 60 * 1000)); // Pegando a segunda-feira da semana anterior
    var sextaPassada = new Date(segundaPassada.getTime() + (4 * 24 * 60 * 60 * 1000)); // Adicionando 4 dias para chegar a sexta-feira

    var formattedMonday = segundaPassada.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' });
    var formattedFriday = sextaPassada.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' });

    var placeholderElement1 = document.getElementById('date-placeholder-1');
    var placeholderElement2 = document.getElementById('date-placeholder-2');

    placeholderElement1.innerHTML = `Atividades trabalhadas durante o período ${formattedMonday} à ${formattedFriday}`;
    placeholderElement2.innerHTML = `Quantidade de chamados Pendentes no prazo de ${formattedMonday} à ${formattedFriday}`;
    
    document.periodo = `${formattedMonday} à ${formattedFriday}`;
}

insertPreviousWeekDates();

