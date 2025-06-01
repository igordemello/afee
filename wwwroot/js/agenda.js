var calendar;

document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');
    calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        locale: 'pt-br',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        events: '/Treino/GetEvents',
        height: 'auto', // Ajusta automaticamente a altura
        dateClick: function (info) {
            $('#createModal').modal('show');

            const clickedDate = info.dateStr;

            flatpickr("#Horario", {
                defaultDate: clickedDate + "T09:00",
                enableTime: true,
                altInput: true,
                altFormat: "d/m/Y H:i",
                dateFormat: "Y-m-d H:i"
            }).setDate(clickedDate + "T09:00");

            flatpickr("#HorarioFim", {
                defaultDate: clickedDate + "T10:00",
                enableTime: true,
                altInput: true,
                altFormat: "d/m/Y H:i",
                dateFormat: "Y-m-d H:i"
            }).setDate(clickedDate + "T10:00");

            $.get('/Treino/Create?date=' + info.dateStr, function (data) {
                $('#modalContent').html(data);
            });
        },
        eventClick: function (info) {
            $('#myModal').modal('show');
            $.get('/Treino/Details?id=' + info.event.id, function (data) {
                $('#modalContent').html(data);
            });
        },
        eventDidMount: function (info) {
            if (info.event.extendedProps.type === 'Individual') {
                info.el.style.backgroundColor = 'purple';
                info.el.style.color = 'white';
            } else if (info.event.extendedProps.type === 'Coletivo') {
                info.el.style.backgroundColor = 'yellow';
                info.el.style.color = 'black';
            }
        }
    });
    calendar.render();

    const tipoSelect = document.getElementById('Tipo');
    const jogadoresSelect = document.getElementById('Jogadores');

    function toggleMultipleSelect() {
        if (tipoSelect.value === 'Individual') {
            jogadoresSelect.setAttribute('multiple', 'multiple');
        } else {
            jogadoresSelect.removeAttribute('multiple');
            Array.from(jogadoresSelect.options).forEach(option => option.selected = false);
        }
    }

    tipoSelect.addEventListener('change', toggleMultipleSelect);
    toggleMultipleSelect();
});

function deleteTreinomento(treinoId) {
    if (confirm("Tem certeza de que deseja excluir este treino?")) {
        $.ajax({
            url: '/Treino/DeleteTreino/' + treinoId,
            type: 'POST',
            success: function (result) {
                if (result.success) {
                    alert("Treino excluído com sucesso!");
                    $('#myModal').modal('hide');
                    calendar.refetchEvents();
                } else {
                    alert("Erro ao excluir o treino: " + result.message);
                }
            },
            error: function () {
                alert("Erro ao tentar excluir o treino.");
            }
        });
    }
}

document.getElementById('createForm').addEventListener('submit', function (event) {
    const startDateTime = new Date(document.getElementById('Horario').value);
    const endDateTime = new Date(document.getElementById('HorarioFim').value);

    if (endDateTime <= startDateTime) {
        event.preventDefault();
        alert("O horário final deve ser posterior ao horário inicial.");
    }
});

function toggleOptions() {
    var tipo = document.getElementById("Tipo").value;
    var playerOptions = document.getElementById("playerOptions");
    var groupOptions = document.getElementById("groupOptions");

    if (tipo === "Individual") {
        playerOptions.style.display = "block";
        groupOptions.style.display = "none";
    } else if (tipo === "Coletivo") {
        playerOptions.style.display = "none";
        groupOptions.style.display = "block";
    }
}

document.addEventListener("DOMContentLoaded", function () {
    toggleOptions();
});

document.addEventListener('DOMContentLoaded', function () {
    flatpickr("#Horario", {
        defaultDate: new Date(),
        enableTime: true,
        altInput: true,
        altFormat: "d/m/Y H:i"
    });
    flatpickr("#HorarioFim", {
        defaultDate: new Date(),
        enableTime: true,
        altInput: true,
        altFormat: "d/m/Y H:i"
    });
});
