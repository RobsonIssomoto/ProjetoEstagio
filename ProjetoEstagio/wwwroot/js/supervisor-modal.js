$(document).ready(function () {

    console.log('Script supervisor-modal.js carregado!');
    console.log('Botões encontrados:', $('.btn-cadastrar-supervisor').length);

    // ===== CADASTRAR SUPERVISOR =====
    $('.btn-cadastrar-supervisor').on('click', function (e) {
        e.preventDefault();
        console.log('Botão cadastrar clicado!');

        $.ajax({
            url: '/Supervisor/Cadastrar',
            type: 'GET',
            beforeSend: function () {
                console.log('Enviando requisição GET para /Supervisor/Cadastrar');
            },
            success: function (result) {
                console.log('Resposta recebida com sucesso:', result);
                $('#cadastrarSupervisorModal .modal-body').html(result);
                $('#cadastrarSupervisorModal').modal('show');

                // Aplicar máscara no CPF
                $('.cpf').mask('000.000.000-00');
            },
            error: function (xhr, status, error) {
                console.error('Erro na requisição:', {
                    status: xhr.status,
                    statusText: xhr.statusText,
                    responseText: xhr.responseText,
                    error: error
                });

                if (xhr.status === 401) {
                    alert('Sessão expirada. Faça login novamente.');
                    window.location.href = '/Login/Index';
                } else if (xhr.status === 403) {
                    alert('Acesso negado: ' + xhr.responseText);
                } else {
                    alert('Erro ao carregar o formulário: ' + xhr.status + ' - ' + xhr.statusText);
                }
            }
        });
    });

    // Submit do formulário de CADASTRO via AJAX
    $(document).on('submit', '#form-cadastrar-supervisor', function (e) {
        e.preventDefault();
        console.log('Formulário de cadastro submetido!');

        var form = $(this);
        var url = form.attr('action');
        var formData = form.serialize();

        console.log('URL:', url);
        console.log('Dados:', formData);

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            beforeSend: function () {
                console.log('Enviando POST para cadastrar...');
            },
            success: function (response) {
                console.log('Resposta do cadastro:', response);

                if (response.sucesso) {
                    console.log('Cadastro realizado com sucesso!');
                    $('#cadastrarSupervisorModal').modal('hide');
                    location.reload();
                } else {
                    console.log('Atualizando modal com erros de validação');
                    $('#cadastrarSupervisorModal .modal-body').html(response);
                    $('.cpf').mask('000.000.000-00');
                }
            },
            error: function (xhr, status, error) {
                console.error('Erro no submit:', {
                    status: xhr.status,
                    statusText: xhr.statusText,
                    responseText: xhr.responseText,
                    error: error
                });

                if (xhr.status === 401) {
                    alert('Sessão expirada. Faça login novamente.');
                    window.location.href = '/Login/Index';
                } else if (xhr.status === 403) {
                    alert('Erro: ' + xhr.responseText);
                    $('#cadastrarSupervisorModal').modal('hide');
                } else {
                    // Se retornou HTML (partial view com erros), atualiza o body do modal
                    $('#cadastrarSupervisorModal .modal-body').html(xhr.responseText);
                    $('.cpf').mask('000.000.000-00');
                }
            }
        });
    });

    // ===== EDITAR SUPERVISOR =====
    $(document).on('click', '.btn-edit-supervisor', function (e) {
        e.preventDefault();
        console.log('Botão editar clicado!');

        var supervisorId = $(this).data('id');
        console.log('ID do supervisor:', supervisorId);

        $.ajax({
            url: '/Supervisor/Editar/' + supervisorId,
            type: 'GET',
            beforeSend: function () {
                console.log('Enviando requisição GET para /Supervisor/Editar/' + supervisorId);
            },
            success: function (result) {
                console.log('Dados do supervisor recebidos:', result);
                $('#editarSupervisorModal .modal-body').html(result);
                $('#editarSupervisorModal').modal('show');

                // Aplicar máscara no CPF
                $('.cpf').mask('000.000.000-00');
            },
            error: function (xhr, status, error) {
                console.error('Erro ao carregar edição:', {
                    status: xhr.status,
                    statusText: xhr.statusText,
                    responseText: xhr.responseText,
                    error: error
                });

                if (xhr.status === 401) {
                    alert('Sessão expirada. Faça login novamente.');
                    window.location.href = '/Login/Index';
                } else if (xhr.status === 403) {
                    alert('Acesso negado: ' + xhr.responseText);
                } else if (xhr.status === 404) {
                    alert('Supervisor não encontrado.');
                } else {
                    alert('Erro ao carregar o formulário: ' + xhr.status + ' - ' + xhr.statusText);
                }
            }
        });
    });

    // Submit do formulário de EDIÇÃO via AJAX
    $(document).on('submit', '#form-editar-supervisor', function (e) {
        e.preventDefault();
        console.log('Formulário de edição submetido!');

        var form = $(this);
        var url = form.attr('action');
        var formData = form.serialize();

        console.log('URL:', url);
        console.log('Dados:', formData);

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            beforeSend: function () {
                console.log('Enviando POST para editar...');
            },
            success: function (response) {
                console.log('Resposta da edição:', response);

                if (response.sucesso) {
                    console.log('Edição realizada com sucesso!');
                    $('#editarSupervisorModal').modal('hide');
                    location.reload();
                } else {
                    console.log('Atualizando modal com erros de validação');
                    $('#editarSupervisorModal .modal-body').html(response);
                    $('.cpf').mask('000.000.000-00');
                }
            },
            error: function (xhr, status, error) {
                console.error('Erro no submit da edição:', {
                    status: xhr.status,
                    statusText: xhr.statusText,
                    responseText: xhr.responseText,
                    error: error
                });

                if (xhr.status === 401) {
                    alert('Sessão expirada. Faça login novamente.');
                    window.location.href = '/Login/Index';
                } else if (xhr.status === 403) {
                    alert('Acesso negado: ' + xhr.responseText);
                    $('#editarSupervisorModal').modal('hide');
                } else {
                    // Se retornou HTML (partial view com erros), atualiza o body do modal
                    $('#editarSupervisorModal .modal-body').html(xhr.responseText);
                    $('.cpf').mask('000.000.000-00');
                }
            }
        });
    });

    // ===== DATATABLE =====
    if ($('#supervisorTable').length) {
        $('#supervisorTable').DataTable({
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json'
            },
            pageLength: 10,
            order: [[0, 'asc']]
        });
    }
});