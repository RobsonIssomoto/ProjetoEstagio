
$(document).ready(function () {
    // Aplica as máscaras nos elementos com os IDs correspondentes
    $('.cpf').mask('000.000.000-00');
    $('.cnpj').mask('00.000.000/0000-00');
    $('.codigo').mask('00');
    $('.telefone').mask('00000-0000');
    $('.telefone_com_ddd').mask('(00) 00000-0000');

    $('.nome').mask('A', {
        'translation': {
            A: {
                pattern: /[A-Za-z\s+]/,
                recursive: true
            }
        }
    });
});


$(document).ready(function () {
    getDataTable('#empresaTable');
    getDataTable('#usuarioTable');
    getDataTable('#supervisorTable');
    getDataTable('#estagiarioTable');
});

function getDataTable(id) {
    $(id).DataTable({
        "ordering": true,
        "searching": true,
        "paging": true,
        "oLanguage": {
            "sEmptyTable": "Nenhum registro encontrado na tabela",
            "sInfo": "Mostra _START_ até _END_ de _TOTAL_ registros",
            "sInfoEmpty": "Mostar 0 até 0 de 0 Registros",
            "sInfoPostFix": "",
            "sInfoThousands": ".",
            "sLengthMenu": "Mostrar _MENU_ registros por página",
            "sLoadingRecords": "Carregando",
            "sZeroRecords": "Nenhum registro encontrado",
            "sSearch": "Pesquisar",
            "oPaginate": {
                "sNext": "Próximo",
                "sPrevious": "Anterior",
                "sFirst": "Primeira",
                "sLast": "Última"
            }
        }
    });
}


const alertPlaceholder = document.getElementById('liveAlertPlaceholder')
const appendAlert = (message, type) => {
    const wrapper = document.createElement('div')
    wrapper.innerHTML = [
        `<div class="alert alert-${type} alert-dismissible" role="alert">`,
        `   <div>${message}</div>`,
        '   <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>',
        '</div>'
    ].join('')

    alertPlaceholder.append(wrapper)
}

const alertTrigger = document.getElementById('liveAlertBtn')
if (alertTrigger) {
    alertTrigger.addEventListener('click', () => {
        appendAlert('Nice, you triggered this alert message!', 'success')
    })
}

$(document).ready(function () {
    // "Escuta" o clique no span que tem o ícone
    $("#togglePassword").on('click', function () {

        // Pega o input da senha
        var $passwordInput = $("#senha");

        // Pega o ícone que está dentro do span
        var $icon = $(this).find('i');

        // Verifica o tipo atual do input
        if ($passwordInput.attr('type') === 'password') {
            // Se for 'password', muda para 'text' (mostrar)
            $passwordInput.attr('type', 'text');

            // Troca o ícone para "olho cortado"
            $icon.removeClass('bi-eye').addClass('bi-eye-slash');
        } else {
            // Se for 'text', muda para 'password' (esconder)
            $passwordInput.attr('type', 'password');

            // Troca o ícone de volta para "olho"
            $icon.removeClass('bi-eye-slash').addClass('bi-eye');
        }
    });
});