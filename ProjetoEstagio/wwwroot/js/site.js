$(document).ready(function () {

    console.log('site.js carregado!');

    // --- DataTables ---
    getDataTable('#empresaTable');
    getDataTable('#usuarioTable');
    getDataTable('#estagiarioTable');
    getDataTable('#supervisorTable');
    getDataTable('#solicitacoesTable');

    // ==========================================================
    //  !!!! PASSO 1: APLIQUE AS MÁSCARAS NO CARREGAMENTO DA PÁGINA !!!!
    // ==========================================================
    // Isso vai consertar seu formulário de Estagiário (e qualquer
    // outra página estática que use as classes de máscara)
    aplicarMascarasGlobais('body');
    // ==========================================================


    // --- Toggle de Senha ---
    $(document).on('click', ".input-icon-toggle", function () {
        // 'this' é o <span> que foi clicado
        var $icon = $(this).find('i');

        // O input é o "irmão" (sibling) do span que tem a classe .form-control
        var $passwordInput = $(this).siblings('input.form-control');

        // Se o tipo for 'password'
        if ($passwordInput.attr('type') === 'password') {
            // Muda para 'text' (mostrar senha)
            $passwordInput.attr('type', 'text');
            // Muda o ícone para 'olho cortado'
            $icon.removeClass('bi-eye').addClass('bi-eye-slash');
        }
        // Se for 'text'
        else {
            // Muda para 'password' (esconder senha)
            $passwordInput.attr('type', 'password');
            // Muda o ícone para 'olho normal'
            $icon.removeClass('bi-eye-slash').addClass('bi-eye');
        }
    });

    // ===================================================================
    // PROCESSO ESTAGIÁRIO - AUTOCOMPLETE DE EMPRESA
    // ===================================================================

    // 1. Função Debounce (Impede muitas chamadas à API)
    //    (Pode deixar esta função como está, fora do document.ready se quiser)
    let timeoutId_empresa = null;
    const debounceBusca = (funcao, delay) => {
        return function (...args) {
            clearTimeout(timeoutId_empresa);
            timeoutId_empresa = setTimeout(() => {
                funcao.apply(this, args);
            }, delay);
        };
    };

    // 2. Função principal que busca com AJAX (jQuery)
    const buscarEmpresas = (searchTerm) => {
        const $resultadosDiv = $('#busca-empresa-resultados'); // Pega a div de resultados

        if (searchTerm.length < 3) {
            $resultadosDiv.html(''); // Limpa se for muito curto
            return;
        }

        $.ajax({
            url: `/api/empresa/buscar?termo=${searchTerm}`, // Chama sua API
            type: 'GET',
            success: function (empresas) {
                renderizarResultados(empresas);
            },
            error: function (error) {
                console.error("Erro ao buscar empresas:", error);
                $resultadosDiv.html('<a href="#" class="list-group-item text-danger">Erro ao buscar.</a>');
            }
        });
    };

    // 3. Função que renderiza os resultados
    const renderizarResultados = (empresas) => {
        const $resultadosDiv = $('#busca-empresa-resultados');
        $resultadosDiv.html(''); // Limpa resultados antigos

        if (!empresas || empresas.length === 0) {
            $resultadosDiv.html('<a href="#" class="list-group-item list-group-item-action disabled">Nenhuma empresa encontrada.</a>');
            return;
        }

        empresas.forEach(empresa => {
            // Cria um item de lista (link) com jQuery
            const $item = $('<a></a>')
                .attr('href', '#')
                .addClass('list-group-item list-group-item-action item-busca-empresa')
                .html(`${empresa.nome} <br> <small class="text-muted">${empresa.cnpj}</small>`);

            // Salva os dados no próprio elemento
            $item.data('id', empresa.id);
            $item.data('nome', empresa.nome);

            $resultadosDiv.append($item);
        });
    };

    // 4. Evento de DIGITAÇÃO (keyup) no input
    //    Usa o debounce para esperar 300ms
    $('#busca-empresa-input').on('keyup', debounceBusca((e) => {
        buscarEmpresas(e.target.value);
    }, 300));

    // 5. Evento de CLIQUE no item de resultado
    //    Usa $(document).on() para funcionar em itens criados dinamicamente
    $(document).on('click', '.item-busca-empresa', function (e) {
        e.preventDefault(); // Impede o link de navegar

        var $item = $(this);
        var id = $item.data('id');
        var nome = $item.data('nome');

        // Preenche os inputs
        $('#empresa-id-selecionada').val(id);
        $('#busca-empresa-input').val(nome);

        // Limpa e esconde os resultados
        $('#busca-empresa-resultados').html('');
    });

    // ===================================================================
    // PROCESSO ESTAGIÁRIO - FECHAR AUTOCOMPLETE AO CLICAR FORA
    // ===================================================================
    $(document).on('click', function (e) {
        // Seleciona os elementos da busca
        const $inputBusca = $('#busca-empresa-input');
        const $resultadosDiv = $('#busca-empresa-resultados');

        // Verifica se o alvo do clique (e.target)
        // NÃO é o input de busca E
        // NÃO é um descendente da div de resultados
        if (!$inputBusca.is(e.target) && $resultadosDiv.has(e.target).length === 0) {
            $resultadosDiv.html(''); // Limpa os resultados
        }
    });

    // ===================================================================
    // FUNÇÃO AUXILIAR (setupModalForm)
    // ===================================================================
    function setupModalForm(modalBodySelector) {
        var $modalBody = $(modalBodySelector);
        if (!$modalBody.length) return;

        var $form = $modalBody.find('form');
        if (!$form.length) return;

        // 1. Remova validadores antigos (Correto)
        $form.removeData("validator");
        $form.removeData("unobtrusiveValidation");

        // 2. "Liga" a validação (Correto)
        if ($.validator && $.validator.unobtrusive) {
            $.validator.unobtrusive.parse($form);
        }

        // ==========================================================
        //  !!!! PASSO 2: CHAME A FUNÇÃO DE MÁSCARA AQUI TAMBÉM !!!!
        // ==========================================================
        // Em vez de duplicar o código, apenas chamamos a nova função.
        // Ela vai aplicar as máscaras *apenas* dentro do modal.
        aplicarMascarasGlobais(modalBodySelector);
        // ==========================================================

        // 3. Correção do [Remote] (Correto)
        $form.find('input[data-val-remote]').on('blur', function () {
            var $input = $(this);
            setTimeout(function () {
                $input.valid();
            }, 100);
        });
    }

    // ===================================================================
    //  !!!! PASSO 3: CRIE A NOVA FUNÇÃO DE MÁSCARAS !!!!
    // ===================================================================
    function aplicarMascarasGlobais(seletorContexto) {
        // O 'seletorContexto' garante que só aplicamos máscaras
        // no conteúdo novo (seja 'body' ou o '#modal .modal-body')
        var $contexto = $(seletorContexto);
        if (!$contexto.length) $contexto = $('body'); // Garante um contexto

        console.log('Aplicando máscaras em:', seletorContexto);

        // Adicionamos { clearIfNotMatch: true } para FORÇAR
        // o usuário a não digitar mais do que a máscara permite.
        // Isso resolve seu problema de "ditar mais dígitos".
        var options = { clearIfNotMatch: true };

        $contexto.find('.cpf').mask('000.000.000-00', options);
        $contexto.find('.cnpj').mask('00.000.000/0000-00', options);
        $contexto.find('.codigo').mask('00', options);
        $contexto.find('.telefone').mask('00009-0000', options);
        $contexto.find('.telefone_com_ddd').mask('(00) 00009-0000', options);

        // Máscara de nome não precisa do 'clearIfNotMatch'
        $contexto.find('.nome').mask('A', {
            'translation': {
                A: {
                    pattern: /[A-Za-z\s+]/,
                    recursive: true
                }
            }
        });
    }

    // ===================================================================
    // SUPERVISOR - CADASTRAR
    // ===================================================================
    // MUDANÇA: Usar $(document).on() em vez de .click() direto
    $(document).on('click', '.btn-cadastrar-supervisor', function (e) {
        e.preventDefault();
        console.log('Botão cadastrar supervisor clicado!');

        $.ajax({
            url: '/Supervisor/Cadastrar',
            type: 'GET',
            beforeSend: function () {
                console.log('Carregando formulário de cadastro...');
            },
            success: function (result) {
                console.log('Formulário carregado com sucesso!');
                var modalBodySelector = '#cadastrarSupervisorModal .modal-body';
                $(modalBodySelector).html(result);
                setupModalForm(modalBodySelector);

                var modal = new bootstrap.Modal(document.getElementById('cadastrarSupervisorModal'));
                modal.show();
            },
            error: function (xhr, status, error) {
                console.error('Erro ao carregar formulário:', xhr.status, xhr.responseText);

                if (xhr.status === 401) {
                    alert("Sua sessão expirou. Você será redirecionado para a página de login.");
                    window.location.href = '/Login/Index';
                } else {
                    alert("Erro ao carregar o formulário. Tente novamente.");
                }
            }
        });
    });

    // ===================================================================
    // SUPERVISOR - EDITAR
    // ===================================================================
    // MUDANÇA: Usar $(document).on() em vez de .click() direto
    $(document).on('click', '.btn-edit-supervisor', function (e) {
        e.preventDefault();
        console.log('Botão editar supervisor clicado!');

        var id = $(this).data('id');
        console.log('ID do supervisor:', id);

        $.ajax({
            url: '/Supervisor/Editar/' + id,
            type: 'GET',
            beforeSend: function () {
                console.log('Carregando dados do supervisor...');
            },
            success: function (result) {
                console.log('Dados carregados com sucesso!');
                var modalBodySelector = '#editarSupervisorModal .modal-body';
                $(modalBodySelector).html(result);
                setupModalForm(modalBodySelector);

                var modal = new bootstrap.Modal(document.getElementById('editarSupervisorModal'));
                modal.show();
            },
            error: function (xhr, status, error) {
                console.error('Erro ao carregar edição:', xhr.status, xhr.responseText);

                if (xhr.status === 401) {
                    alert("Sua sessão expirou. Você será redirecionado para a página de login.");
                    window.location.href = '/Login/Index';
                } else if (xhr.status === 403) {
                    alert("Você não tem permissão para editar este item.");
                } else if (xhr.status === 404) {
                    alert("O supervisor que você está tentando editar não foi encontrado.");
                } else {
                    alert("Erro ao carregar o formulário de edição.");
                }
            }
        });
    });

    // Em: wwwroot/js/site.js

    // ===================================================================
    // SUPERVISOR - SUBMIT CADASTRO
    // ===================================================================
    $(document).on('submit', '#form-cadastrar-supervisor', function (e) {
        e.preventDefault();
        console.log('Formulário de cadastro submetido!');
        var $form = $(this);
        if (!$form.valid()) { console.log('Formulário inválido!'); return; }

        // --- INÍCIO DA CORREÇÃO ---
        var $cpfInput = $form.find('.cpf');
        var $telefoneInput = $form.find('.telefone_com_ddd');

        // Salva os valores originais (com máscara)
        var originalCpf = $cpfInput.val();
        var originalTelefone = $telefoneInput.val();

        // Limpa os valores manualmente, SÓ SE ELES EXISTIREM
        if (originalCpf) {
            $cpfInput.val(originalCpf.replace(/\D/g, ''));
        }
        if (originalTelefone) {
            $telefoneInput.val(originalTelefone.replace(/\D/g, ''));
        }
        // --- FIM DA CORREÇÃO ---

        var serializedData = $form.serialize();
        console.log('Enviando dados (limpos)...', serializedData);

        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: serializedData,
            success: function (result) {
                // (Seu código de 'success' continua igual)
                console.log('Cadastro realizado com sucesso!');
                window.location.reload();
            },
            error: function (xhr, status, error) {
                console.error('Erro no submit:', xhr.status, xhr.responseText);

                // Restaura os valores mascarados para o usuário ver
                if (originalCpf) $cpfInput.val(originalCpf);
                if (originalTelefone) $telefoneInput.val(originalTelefone);

                if (xhr.status === 400) {
                    // Erro de validação (ex: CPF inválido, senhas não conferem)
                    try {
                        var response = JSON.parse(xhr.responseText);
                        // Procura por uma msg de CPF, ou Senha, ou a primeira msg de erro
                        var msg = response.CPF ? response.CPF[0] :
                            (response.ConfirmarSenha ? response.ConfirmarSenha[0] :
                                (response[Object.keys(response)[0]] ? response[Object.keys(response)[0]][0] : "Verifique os dados."));
                        alert("Erro de validação: " + msg);
                    } catch (e) {
                        alert("Erro de validação. Verifique os campos em vermelho.");
                    }
                } else if (xhr.status === 401) {
                    // Erro de Sessão Expirada
                    alert("Sua sessão expirou. Você será redirecionado para a página de login.");
                    window.location.href = '/Login/Index';
                } else {
                    // Outros erros (500, etc)
                    alert("Ocorreu um erro no servidor. Tente novamente.\nDetalhe: " + xhr.status + " " + error);
                }
            }
        });
    });

    // ===================================================================
    // SUPERVISOR - SUBMIT EDIÇÃO (A FONTE DO SEU ERRO)
    // ===================================================================
    $(document).on('submit', '#form-editar-supervisor', function (e) {
        e.preventDefault();
        console.log('Formulário de edição submetido!'); // Esta é a linha 365 do seu log
        var $form = $(this);
        if (!$form.valid()) { console.log('Formulário inválido!'); return; }

        // --- INÍCIO DA CORREÇÃO ---
        var $cpfInput = $form.find('.cpf');
        var $telefoneInput = $form.find('.telefone_com_ddd');

        // Salva os valores originais (com máscara)
        var originalCpf = $cpfInput.val(); // Esta é a linha ~377
        var originalTelefone = $telefoneInput.val();

        // Limpa os valores manualmente, SÓ SE ELES EXISTIREM
        // A linha 379 agora é este 'if'
        if (originalCpf) {
            $cpfInput.val(originalCpf.replace(/\D/g, ''));
        }
        if (originalTelefone) {
            $telefoneInput.val(originalTelefone.replace(/\D/g, ''));
        }
        // --- FIM DA CORREÇÃO ---

        var serializedData = $form.serialize();
        console.log('Enviando dados (limpos)...', serializedData);

        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: serializedData,
            success: function (result) {
                // (Seu código de 'success' continua igual)
                console.log('Edição realizada com sucesso!');
                window.location.reload();
            },
            error: function (xhr, status, error) {
                console.error('Erro no submit:', xhr.status, xhr.responseText);

                // Restaura os valores mascarados para o usuário ver
                if (originalCpf) $cpfInput.val(originalCpf);
                if (originalTelefone) $telefoneInput.val(originalTelefone);

                if (xhr.status === 400) {
                    // Erro de validação (ex: CPF inválido)
                    try {
                        var response = JSON.parse(xhr.responseText);
                        // Procura por uma msg de CPF ou a primeira msg de erro
                        var msg = response.CPF ? response.CPF[0] :
                            (response[Object.keys(response)[0]] ? response[Object.keys(response)[0]][0] : "Verifique os dados.");
                        alert("Erro de validação: " + msg);
                    } catch (e) {
                        alert("Erro de validação. Verifique os campos em vermelho.");
                    }
                } else if (xhr.status === 401) {
                    // Erro de Sessão Expirada
                    alert("Sua sessão expirou. Você será redirecionado para a página de login.");
                    window.location.href = '/Login/Index';
                } else {
                    // Outros erros (500, etc)
                    alert("Ocorreu um erro no servidor. Tente novamente.\nDetalhe: " + xhr.status + " " + error);
                }
            }
        });
    });

    // ===================================================================
    // SUPERVISOR - ABRIR MODAL DELETAR (CARREGAMENTO DINÂMICO)
    // ===================================================================
    $(document).on('click', '.btn-deletar-supervisor', function (e) {
        e.preventDefault();
        console.log('Botão deletar supervisor clicado!');

        var id = $(this).data('id');
        console.log('ID do supervisor:', id);

        $.ajax({
            url: '/Supervisor/Deletar/' + id, // Nova URL [HttpGet]
            type: 'GET',
            beforeSend: function () {
                console.log('Carregando dados para exclusão...');
            },
            success: function (result) {
                console.log('Formulário de exclusão carregado!');
                var modalBodySelector = '#deletarSupervisorModal .modal-body';
                $(modalBodySelector).html(result);

                // Re-aplica máscaras e validação (para a máscara de CPF)
                setupModalForm(modalBodySelector);

                var modal = new bootstrap.Modal(document.getElementById('deletarSupervisorModal'));
                modal.show();
            },
            error: function (xhr, status, error) {
                console.error('Erro ao carregar exclusão:', xhr.status, xhr.responseText);
                if (xhr.status === 401) {
                    alert("Sua sessão expirou.");
                    window.location.href = '/Login/Index';
                } else if (xhr.status === 403) {
                    alert("Você não tem permissão para ver este item.");
                } else if (xhr.status === 404) {
                    alert("O supervisor não foi encontrado.");
                } else {
                    alert("Erro ao carregar o formulário de exclusão.");
                }
            }
        });
    });


    // ===================================================================
    // SUPERVISOR - SUBMIT DELETAR
    // ===================================================================
    $(document).on('submit', '#form-deletar-supervisor', function (e) {
        e.preventDefault();
        console.log('Formulário de exclusão submetido!');

        var $form = $(this);
        var serializedData = $form.serialize();

        console.log('Enviando dados para exclusão...');
        console.log('Dados serializados:', serializedData);

        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: serializedData,
            success: function (result) {
                console.log('Resposta recebida:', result);
                if (typeof result === 'object' && result.sucesso === true) {
                    console.log('Exclusão realizada com sucesso!');
                    var modalEl = document.getElementById('deletarSupervisorModal');
                    bootstrap.Modal.getInstance(modalEl).hide();
                    window.location.reload();
                } else {
                    // Isso não deve acontecer no modo "nuclear"
                    console.error('Resposta inesperada:', result);
                    alert('Erro inesperado ao excluir.');
                }
            },
            error: function (xhr, status, error) {
                console.error('Erro no submit de exclusão:', xhr.status, xhr.responseText);
                var modalEl = document.getElementById('deletarSupervisorModal');
                bootstrap.Modal.getInstance(modalEl).hide();

                if (xhr.status === 401) {
                    alert("Sua sessão expirou.");
                    window.location.href = '/Login/Index';
                } else if (xhr.status === 403) {
                    alert("Acesso negado: Você não tem permissão para excluir este item.");
                } else if (xhr.status === 404) {
                    alert("Erro: O supervisor que você tentou excluir não foi encontrado.");
                } else {
                    alert("Ocorreu um erro no servidor. Tente novamente.\nDetalhe: " + (xhr.responseText || error));
                }
            }
        });
    });


}); // Fim do $(document).ready()


// ===================================================================
// FUNÇÕES AUXILIARES GLOBAIS
// ===================================================================

function getDataTable(id) {
    $(id).DataTable({
        "ordering": true,
        "searching": true,
        "paging": true,
        "pageLength": 5,
        "lengthMenu": [
            [5, 10, 25, -1], // Valores reais para a opção "Mostrar tudo" use -1
            [5, 10, 25, "Tudo"] // Textos exibidos no menu
        ],
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

