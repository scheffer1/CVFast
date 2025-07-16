# CVFast Integration API

API pública para consulta de currículos por sistemas integradores.

## Visão Geral

Esta API foi criada especificamente para permitir que sistemas externos consultem currículos públicos do CVFast de forma simples e segura, sem necessidade de autenticação.

## Características

- **Sem Autenticação**: Acesso público aos endpoints
- **Apenas Consulta**: Somente operações de leitura
- **Dados Limpos**: Retorna apenas informações visíveis ao usuário (sem IDs internos)
- **Endereço Único**: Retorna um objeto de endereço ao invés de lista
- **Sem ShortLinks**: Não expõe links curtos na resposta
- **Swagger Dedicado**: Documentação específica para integradores
- **Porta Separada**: Roda independentemente da API principal
- **CORS Liberado**: Permite acesso de qualquer origem

## Configuração

### Desenvolvimento
- **Porta HTTP**: 5208
- **Porta HTTPS**: 7037
- **Swagger**: Disponível na raiz (`/`)

### Produção
- **Porta**: 8081 (mapeada no Docker)
- **URL**: api.cvfast.com.br
- **Swagger**: Sempre habilitado

## Endpoints Disponíveis

### GET /api/curriculum/{hash}
Consulta um currículo através do hash do link curto.

**Parâmetros:**
- `hash` (string): Hash do link curto do currículo

**Respostas:**
- `200`: Retorna os dados completos do currículo
- `403`: Currículo privado - acesso negado
- `404`: Currículo não encontrado ou hash inválido

### HEAD /api/curriculum/{hash}
Verifica se um currículo está disponível sem retornar os dados.

**Parâmetros:**
- `hash` (string): Hash do link curto do currículo

**Respostas:**
- `200`: Currículo público e disponível
- `403`: Currículo privado
- `404`: Currículo não encontrado

## Exemplo de Uso

```bash
# Consultar um currículo
curl -X GET "http://localhost:5208/api/curriculum/ABC123XYZ"

# Verificar disponibilidade
curl -I "http://localhost:5208/api/curriculum/ABC123XYZ"
```

## Estrutura de Resposta

A resposta contém **apenas os dados visíveis ao usuário**, sem IDs internos ou informações técnicas:

```json
{
  "success": true,
  "message": "Operação realizada com sucesso",
  "data": {
    "title": "Título do Currículo",
    "summary": "Resumo profissional",
    "experiences": [
      {
        "companyName": "Empresa XYZ",
        "role": "Desenvolvedor",
        "description": "Descrição das atividades",
        "startDate": "2023-01-01",
        "endDate": "2024-01-01",
        "location": "São Paulo, SP"
      }
    ],
    "educations": [
      {
        "institution": "Universidade ABC",
        "degree": "Bacharelado",
        "fieldOfStudy": "Ciência da Computação",
        "startDate": "2019-01-01",
        "endDate": "2022-12-01",
        "description": "Descrição do curso"
      }
    ],
    "skills": [
      {
        "techName": "JavaScript",
        "proficiency": "Advanced"
      }
    ],
    "languages": [
      {
        "languageName": "Inglês",
        "proficiency": "Advanced"
      }
    ],
    "contacts": [
      {
        "type": "Email",
        "value": "usuario@email.com",
        "isPrimary": true
      }
    ],
    "address": {
      "street": "Rua das Flores",
      "number": "123",
      "complement": "Apto 45",
      "neighborhood": "Centro",
      "city": "São Paulo",
      "state": "SP",
      "zipCode": "01234-567",
      "country": "Brasil"
    }
  }
}
```

## Deploy

### Docker
A API está configurada no `docker-compose.yml` como serviço `integration-api`:

```yaml
integration-api:
  build:
    context: ./Api
    dockerfile: CVFastApi.Integration/Dockerfile
  container_name: cvfast-integration-api
  restart: always
  environment:
    - ASPNETCORE_ENVIRONMENT=Production
    - ASPNETCORE_URLS=http://+:80
  ports:
    - "8081:80"
  depends_on:
    postgres:
      condition: service_healthy
  networks:
    - cvfast-network
```

### Comandos

```bash
# Executar em desenvolvimento
dotnet run --project CVFastApi.Integration

# Build
dotnet build CVFastApi.Integration

# Executar com Docker
docker-compose up integration-api
```

## Logs e Monitoramento

A API registra automaticamente:
- Acessos aos currículos via hash
- Tentativas de acesso a currículos privados
- Erros de consulta
- IPs e User-Agents dos acessos

## Segurança

- Apenas currículos com status "Active" são retornados
- Currículos privados retornam 403 (Forbidden)
- Logs de auditoria para todos os acessos
- Rate limiting pode ser implementado se necessário

## Integração com DNS

Para produção, configure o DNS para apontar `api.cvfast.com.br` para a porta 8081 do servidor.
