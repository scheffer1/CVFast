# CVFast - Especificações de Casos de Uso

## Índice de Casos de Uso

### 1. Autenticação e Autorização
- UC01: Cadastrar usuário
- UC02: Fazer login no sistema
- UC03: Logout do sistema

### 2. Gestão de Currículos
- UC04: Criar currículo
- UC05: Editar currículo
- UC06: Visualizar currículo
- UC07: Excluir currículo
- UC08: Listar currículos do usuário

### 3. Sistema de Links Curtos
- UC09: Gerar link curto para currículo
- UC10: Acessar currículo via link curto
- UC11: Revogar link curto

### 4. Gestão de Conteúdo
- UC12: Adicionar experiência profissional
- UC13: Adicionar formação acadêmica
- UC14: Adicionar habilidade técnica

### 5. Analytics e Integração
- UC15: Visualizar estatísticas de acesso
- UC16: Consultar currículo via API pública

---

## UC01 - Cadastrar usuário

| **Cadastrar usuário (Ator - Visitante)** |
|-------------------------------------------|
| **Pré-condições** |
| Não ter conta cadastrada no sistema |
| Ter acesso à internet |
| **Fluxo básico** |
| O visitante acessa a tela de cadastro |
| O visitante preenche os campos obrigatórios: nome, email e senha |
| O visitante confirma a senha |
| O visitante clica no botão "Cadastrar" |
| O sistema valida os dados informados |
| O sistema verifica se o email não está em uso |
| O sistema cria a conta do usuário |
| O sistema redireciona para a tela de login |
| **Fluxo alternativo** |
| Caso o visitante informe um email já cadastrado, o sistema exibe mensagem de erro |
| Caso a confirmação de senha não confira, o sistema exibe mensagem de erro |
| Caso algum campo obrigatório não seja preenchido, o sistema exibe mensagem de validação |
| Caso o visitante cancele o processo, ele pode sair do formulário |
| **Pós-condições** |
| O sistema registra o novo usuário no banco de dados |
| O usuário pode fazer login no sistema |
| O usuário recebe confirmação de cadastro realizado com sucesso |

---

## UC02 - Fazer login no sistema

| **Fazer login no sistema (Ator - Usuário)** |
|---------------------------------------------|
| **Pré-condições** |
| Ter conta cadastrada no sistema |
| Não estar logado no sistema |
| **Fluxo básico** |
| O usuário acessa a tela de login |
| O usuário informa email e senha |
| O usuário clica no botão "Entrar" |
| O sistema valida as credenciais |
| O sistema gera token JWT de autenticação |
| O sistema redireciona para o dashboard principal |
| **Fluxo alternativo** |
| Caso as credenciais sejam inválidas, o sistema exibe mensagem de erro |
| Caso o usuário esqueça a senha, pode acessar a funcionalidade de recuperação |
| Caso o usuário cancele o login, retorna à página inicial |
| **Pós-condições** |
| O usuário está autenticado no sistema |
| O token JWT é armazenado para sessão |
| O usuário tem acesso às funcionalidades protegidas |

---

## UC04 - Criar currículo

| **Criar currículo (Ator - Usuário autenticado)** |
|--------------------------------------------------|
| **Pré-condições** |
| Estar logado no sistema |
| Ter acesso à área de currículos |
| **Fluxo básico** |
| O usuário acessa a seção "Meus Currículos" |
| O usuário clica em "Novo Currículo" |
| O usuário preenche as informações básicas: título e resumo pessoal |
| O usuário adiciona informações de contato |
| O usuário adiciona experiências profissionais |
| O usuário adiciona formação acadêmica |
| O usuário adiciona habilidades técnicas |
| O usuário salva o currículo |
| O sistema gera automaticamente um link curto para o currículo |
| **Fluxo alternativo** |
| O usuário pode salvar o currículo como rascunho a qualquer momento |
| O usuário pode cancelar a criação e perder as alterações não salvas |
| Caso ocorra erro de validação, o sistema exibe as mensagens específicas |
| **Pós-condições** |
| O currículo é salvo no banco de dados |
| Um link curto é gerado automaticamente |
| O usuário pode compartilhar o link do currículo |
| O currículo aparece na lista de currículos do usuário |

---

## UC05 - Editar currículo

| **Editar currículo (Ator - Usuário autenticado)** |
|---------------------------------------------------|
| **Pré-condições** |
| Estar logado no sistema |
| Ter pelo menos um currículo criado |
| Ser o proprietário do currículo |
| **Fluxo básico** |
| O usuário acessa "Meus Currículos" |
| O usuário seleciona o currículo a ser editado |
| O usuário clica em "Editar" |
| O sistema carrega os dados atuais do currículo |
| O usuário modifica as informações desejadas |
| O usuário salva as alterações |
| O sistema atualiza o currículo no banco de dados |
| **Fluxo alternativo** |
| O usuário pode cancelar a edição sem salvar |
| Caso ocorra erro de validação, o sistema exibe mensagens específicas |
| O usuário pode adicionar novas seções durante a edição |
| O usuário pode remover seções existentes |
| **Pós-condições** |
| O currículo é atualizado com as novas informações |
| A data de última modificação é atualizada |
| O link curto permanece o mesmo |
| As alterações ficam disponíveis imediatamente via link público |

---

## UC09 - Gerar link curto para currículo

| **Gerar link curto para currículo (Ator - Sistema)** |
|------------------------------------------------------|
| **Pré-condições** |
| Currículo deve estar criado no sistema |
| Currículo deve ter status ativo |
| **Fluxo básico** |
| O sistema é acionado automaticamente após criação/edição do currículo |
| O sistema gera um GUID único |
| O sistema converte o GUID para base64 |
| O sistema cria o hash do link curto |
| O sistema salva o link curto no banco de dados |
| O sistema associa o link ao currículo correspondente |
| **Fluxo alternativo** |
| Caso já exista um link ativo para o currículo, o sistema mantém o existente |
| Caso ocorra erro na geração, o sistema tenta novamente |
| **Pós-condições** |
| Um link curto único é criado para o currículo |
| O link fica disponível para compartilhamento |
| O sistema registra a criação do link nos logs |

---

## UC10 - Acessar currículo via link curto

| **Acessar currículo via link curto (Ator - Visitante/Recrutador)** |
|--------------------------------------------------------------------|
| **Pré-condições** |
| Ter o link curto válido |
| Link não deve estar revogado |
| Currículo deve estar ativo |
| **Fluxo básico** |
| O visitante acessa o link curto fornecido |
| O sistema valida se o link existe e está ativo |
| O sistema busca o currículo associado ao link |
| O sistema registra o acesso nos logs |
| O sistema exibe o currículo em formato público |
| O visitante pode visualizar todas as informações do currículo |
| **Fluxo alternativo** |
| Caso o link seja inválido, o sistema exibe página de erro 404 |
| Caso o link esteja revogado, o sistema informa que não está mais disponível |
| Caso o currículo esteja inativo, o sistema informa indisponibilidade |
| **Pós-condições** |
| O acesso é registrado nos logs com data/hora e IP |
| O visitante tem acesso completo às informações do currículo |
| O proprietário pode visualizar estatísticas de acesso |

---

## UC06 - Visualizar currículo

| **Visualizar currículo (Ator - Usuário autenticado)** |
|-------------------------------------------------------|
| **Pré-condições** |
| Estar logado no sistema |
| Ter pelo menos um currículo criado |
| Ser o proprietário do currículo |
| **Fluxo básico** |
| O usuário acessa "Meus Currículos" |
| O usuário seleciona o currículo desejado |
| O usuário clica em "Visualizar" |
| O sistema carrega todas as informações do currículo |
| O sistema exibe o currículo em formato de visualização |
| O usuário pode ver como o currículo aparece para visitantes |
| **Fluxo alternativo** |
| O usuário pode acessar o link público do próprio currículo |
| O usuário pode imprimir ou exportar o currículo |
| O usuário pode compartilhar o link diretamente da visualização |
| **Pós-condições** |
| O usuário visualiza seu currículo completo |
| O usuário pode verificar como as informações são apresentadas |
| O usuário tem acesso ao link de compartilhamento |

---

## UC07 - Excluir currículo

| **Excluir currículo (Ator - Usuário autenticado)** |
|----------------------------------------------------|
| **Pré-condições** |
| Estar logado no sistema |
| Ter pelo menos um currículo criado |
| Ser o proprietário do currículo |
| **Fluxo básico** |
| O usuário acessa "Meus Currículos" |
| O usuário seleciona o currículo a ser excluído |
| O usuário clica em "Excluir" |
| O sistema solicita confirmação da exclusão |
| O usuário confirma a exclusão |
| O sistema remove o currículo e todos os dados associados |
| O sistema revoga automaticamente todos os links curtos |
| **Fluxo alternativo** |
| O usuário pode cancelar a exclusão |
| Caso existam acessos recentes, o sistema pode alertar sobre o impacto |
| **Pós-condições** |
| O currículo é removido permanentemente do sistema |
| Todos os links curtos associados são revogados |
| Os logs de acesso são mantidos para auditoria |
| O currículo não aparece mais na lista do usuário |

---

## UC08 - Listar currículos do usuário

| **Listar currículos do usuário (Ator - Usuário autenticado)** |
|---------------------------------------------------------------|

| **Pré-condições** |
|-------------------|
| Estar logado no sistema |
| Ter acesso à área de currículos |

| **Fluxo básico** |
|------------------|
| O usuário acessa "Meus Currículos" |
| O sistema busca todos os currículos do usuário |
| O sistema exibe a lista com informações resumidas |
| O usuário pode ver título, data de criação e status de cada currículo |
| O usuário pode ordenar a lista por diferentes critérios |

| **Fluxo alternativo** |
|----------------------|
| Caso não tenha currículos, o sistema exibe opção para criar o primeiro |
| O usuário pode filtrar currículos por status |
| O usuário pode buscar currículos por título |

| **Pós-condições** |
|-------------------|
| O usuário visualiza todos os seus currículos |
| O usuário pode acessar ações para cada currículo |
| O usuário tem visão geral do seu portfólio de currículos |

---

## UC11 - Revogar link curto

| **Revogar link curto (Ator - Usuário autenticado)** |
|----------------------------------------------------- |

| **Pré-condições** |
|-------------------|
| Estar logado no sistema |
| Ter currículo com link curto ativo |
| Ser o proprietário do currículo |

| **Fluxo básico** |
|------------------|
| O usuário acessa as configurações do currículo |
| O usuário visualiza os links curtos ativos |
| O usuário seleciona o link a ser revogado |
| O usuário clica em "Revogar Link" |
| O sistema solicita confirmação |
| O usuário confirma a revogação |
| O sistema marca o link como revogado |

| **Fluxo alternativo** |
|----------------------|
| O usuário pode cancelar a revogação |
| O usuário pode gerar um novo link após revogar |

| **Pós-condições** |
|-------------------|
| O link curto fica inativo |
| Acessos futuros ao link retornam erro |
| O histórico de acessos é mantido |
| O usuário pode gerar um novo link se necessário |

---

## UC12 - Adicionar experiência profissional

| **Adicionar experiência profissional (Ator - Usuário autenticado)** |
|---------------------------------------------------------------------|

| **Pré-condições** |
|-------------------|
| Estar logado no sistema |
| Estar editando um currículo |
| Ter acesso à seção de experiências |

| **Fluxo básico** |
|------------------|
| O usuário acessa a seção "Experiências Profissionais" |
| O usuário clica em "Adicionar Experiência" |
| O usuário preenche os campos: empresa, cargo, período, descrição |
| O usuário pode marcar se ainda trabalha na empresa |
| O usuário salva a experiência |
| O sistema valida os dados informados |
| O sistema adiciona a experiência ao currículo |

| **Fluxo alternativo** |
|----------------------|
| O usuário pode cancelar a adição |
| Caso a data de início seja posterior à data de fim, o sistema exibe erro |
| O usuário pode adicionar múltiplas experiências em sequência |

| **Pós-condições** |
|-------------------|
| A experiência é adicionada ao currículo |
| A experiência aparece na visualização do currículo |
| O currículo é atualizado automaticamente |

---

## UC13 - Adicionar formação acadêmica

| **Adicionar formação acadêmica (Ator - Usuário autenticado)** |
|---------------------------------------------------------------|

| **Pré-condições** |
|-------------------|
| Estar logado no sistema |
| Estar editando um currículo |
| Ter acesso à seção de educação |

| **Fluxo básico** |
|------------------|
| O usuário acessa a seção "Formação Acadêmica" |
| O usuário clica em "Adicionar Formação" |
| O usuário preenche: instituição, curso, área de estudo, período |
| O usuário pode marcar se ainda está cursando |
| O usuário pode adicionar descrição adicional |
| O usuário salva a formação |
| O sistema valida e adiciona ao currículo |

| **Fluxo alternativo** |
|----------------------|
| O usuário pode cancelar a adição |
| O usuário pode adicionar múltiplas formações |
| Caso as datas sejam inválidas, o sistema exibe erro |

| **Pós-condições** |
|-------------------|
| A formação é adicionada ao currículo |
| A formação aparece ordenada por data |
| O currículo é atualizado automaticamente |

---

## UC14 - Adicionar habilidade técnica

| **Adicionar habilidade técnica (Ator - Usuário autenticado)** |
|---------------------------------------------------------------|

| **Pré-condições** |
|-------------------|
| Estar logado no sistema |
| Estar editando um currículo |
| Ter acesso à seção de habilidades |

| **Fluxo básico** |
|------------------|
| O usuário acessa a seção "Habilidades Técnicas" |
| O usuário clica em "Adicionar Habilidade" |
| O usuário informa o nome da tecnologia/habilidade |
| O usuário seleciona o nível de proficiência |
| O usuário pode adicionar observações |
| O usuário salva a habilidade |
| O sistema adiciona ao currículo |

| **Fluxo alternativo** |
|----------------------|
| O usuário pode cancelar a adição |
| O usuário pode adicionar múltiplas habilidades rapidamente |
| O sistema pode sugerir habilidades baseadas no que foi digitado |

| **Pós-condições** |
|-------------------|
| A habilidade é adicionada ao currículo |
| A habilidade aparece na seção correspondente |
| O currículo é atualizado automaticamente |

---

## UC15 - Visualizar estatísticas de acesso

| **Visualizar estatísticas de acesso (Ator - Usuário autenticado)** |
|--------------------------------------------------------------------|

| **Pré-condições** |
|-------------------|
| Estar logado no sistema |
| Ter pelo menos um currículo com link compartilhado |
| Ter registros de acesso no sistema |

| **Fluxo básico** |
|------------------|
| O usuário acessa "Estatísticas" ou "Analytics" |
| O usuário seleciona o currículo desejado |
| O sistema exibe dados de acessos: total, por período, origem |
| O usuário pode filtrar por data |
| O usuário pode ver gráficos de visualizações |

| **Fluxo alternativo** |
|----------------------|
| Caso não tenha acessos, o sistema informa que ainda não há dados |
| O usuário pode exportar relatórios |
| O usuário pode comparar estatísticas entre currículos |

| **Pós-condições** |
|-------------------|
| O usuário visualiza métricas de performance do currículo |
| O usuário pode tomar decisões baseadas nos dados |
| O usuário entende o alcance do seu currículo |

---

## UC16 - Consultar currículo via API pública

| **Consultar currículo via API pública (Ator - Sistema Externo/Aplicação Terceira)** |
|--------------------------------------------------------------------------------------|
| **Pré-condições** |
| Ter o hash do link curto válido |
| Link curto deve estar ativo (não revogado) |
| Currículo deve estar público |
| Ter acesso à internet |
| **Fluxo básico** |
| O sistema externo faz requisição GET para `/api/public/curriculum/{hash}` |
| O sistema externo envia o hash do link curto como parâmetro |
| A API CVFast valida se o hash existe no banco de dados |
| A API verifica se o link não está revogado |
| A API busca o currículo completo associado ao link |
| A API retorna os dados do currículo em formato JSON estruturado |
| A API registra o acesso nos logs de auditoria |
| **Fluxo alternativo** |
| Caso o hash seja inválido ou não exista, a API retorna HTTP 404 com mensagem de erro |
| Caso o link esteja revogado, a API retorna HTTP 403 com mensagem "Link revogado" |
| Caso o currículo esteja inativo, a API retorna HTTP 410 com mensagem "Currículo indisponível" |
| Caso ocorra erro interno do servidor, a API retorna HTTP 500 |
| O sistema externo pode implementar retry em caso de timeout |
| **Pós-condições** |
| O sistema externo recebe dados completos do currículo em JSON |
| O acesso é registrado com timestamp, IP e user-agent |
| O proprietário do currículo pode visualizar estatísticas deste acesso |
| Os dados podem ser processados pelo sistema consumidor |

---

## Observações Gerais

### Atores do Sistema
- **Visitante**: Pessoa não autenticada que acessa links públicos
- **Usuário**: Pessoa cadastrada e autenticada no sistema
- **Recrutador**: Pessoa que acessa currículos via links compartilhados
- **Sistema Externo**: APIs e integrações de terceiros (sem autenticação)
- **Sistema**: Processos automáticos internos

### Regras de Negócio
1. Cada usuário pode ter múltiplos currículos
2. Links curtos são gerados automaticamente na criação do currículo
3. Links curtos usam hash base64 de GUID para garantir unicidade
4. Todos os acessos são registrados para auditoria e estatísticas
5. Usuários só podem gerenciar seus próprios currículos
6. Links revogados não podem ser reativados
7. Dados de currículos são validados antes de serem salvos
8. API pública permite integração com sistemas de RH sem autenticação

### Tecnologias Utilizadas
- **Backend**: ASP.NET Core com Entity Framework
- **Banco de Dados**: PostgreSQL
- **Autenticação**: JWT (JSON Web Tokens) para área administrativa
- **Frontend**: React/Vue.js (conforme implementação)
- **API**: RESTful com documentação Swagger
- **API Pública**: Endpoint sem autenticação para consulta de currículos

### Considerações de Segurança
- Senhas são armazenadas com hash seguro
- Tokens JWT têm tempo de expiração
- Validação de entrada em todos os endpoints
- Logs de auditoria para rastreabilidade
- Controle de acesso baseado em propriedade de recursos
- API pública limitada apenas à consulta por hash válido
