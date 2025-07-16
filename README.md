# CVFast - Centralizador de Currículos

## 📋 Sobre o Projeto

O CVFast é uma plataforma completa para criação, gerenciamento e compartilhamento de currículos profissionais. O sistema permite que usuários criem múltiplos currículos personalizados e os compartilhem através de links curtos públicos, facilitando a integração com sistemas de RH e plataformas de recrutamento.

## 🌐 Onde Está Publicado

- **Frontend**: [cvfast.com.br](https://cvfast.com.br)
- **API Principal**: [web.cvfast.com.br](https://web.cvfast.com.br)
- **API de Integração**: [api.cvfast.com.br](https://api.cvfast.com.br)
- **Hospedagem**: VPS Hostinger com deploy automatizado via Portainer

## 🚀 Tecnologias Utilizadas

### Backend
- **Framework**: ASP.NET Core 9.0
- **Linguagem**: C#
- **Banco de Dados**: PostgreSQL 16
- **ORM**: Entity Framework Core 9.0
- **Autenticação**: JWT (JSON Web Tokens)
- **Documentação**: Swagger/OpenAPI
- **Containerização**: Docker

### Frontend
- **Framework**: React 18.3.1
- **Build Tool**: Vite 5.4.1
- **Linguagem**: TypeScript 5.5.3
- **UI Library**: Radix UI + shadcn/ui
- **Estilização**: Tailwind CSS 3.4.11
- **Gerenciamento de Estado**: TanStack Query 5.56.2
- **Roteamento**: React Router DOM 6.26.2
- **Formulários**: React Hook Form 7.53.0 + Zod 3.23.8

### Infraestrutura
- **Proxy Reverso**: Nginx
- **Containerização**: Docker + Docker Compose
- **Deploy**: Portainer com Git auto-deploy
- **SSL**: Let's Encrypt
- **Monitoramento**: Logs estruturados

## 🏗️ Arquitetura do Sistema

### Estrutura do Projeto
```
CVFast/
├── Api/                          # Backend APIs
│   ├── CVFastApi/               # API Principal (autenticada)
│   ├── CVFastApi.Integration/   # API de Integração (pública)
│   └── CVFastServices/          # Camada de serviços e dados
├── Front/                       # Frontend React
├── Nginx/                       # Configuração do proxy reverso
├── Docs/                        # Documentação do projeto
└── docker-compose.yml          # Orquestração dos containers
```

### APIs Disponíveis

#### API Principal (web.cvfast.com.br)
- **Autenticação**: JWT obrigatório
- **Funcionalidades**: CRUD completo de currículos, usuários e dados
- **Swagger**: Disponível na raiz da API
- **Porta**: 80 (container) / 443 (HTTPS público)

#### API de Integração (api.cvfast.com.br)
- **Autenticação**: Não requerida (pública)
- **Funcionalidades**: Consulta de currículos via hash
- **Swagger**: Disponível na raiz da API
- **Porta**: 8081 (público)

## 📊 Funcionalidades Principais

### Para Usuários
- ✅ Cadastro e autenticação de usuários
- ✅ Criação de múltiplos currículos
- ✅ Gerenciamento de experiências profissionais
- ✅ Cadastro de formação acadêmica
- ✅ Registro de habilidades técnicas e idiomas
- ✅ Informações de contato e endereço
- ✅ Geração automática de links curtos
- ✅ Controle de visibilidade (público/privado)
- ✅ Estatísticas de acesso aos currículos

### Para Integradores
- ✅ API pública para consulta de currículos
- ✅ Acesso via hash do link curto
- ✅ Dados estruturados em JSON
- ✅ Documentação Swagger dedicada
- ✅ CORS liberado para integração web

## 🗄️ Modelo de Dados

### Entidades Principais
- **User**: Usuários do sistema
- **Curriculum**: Currículos dos usuários
- **Experience**: Experiências profissionais
- **Education**: Formação acadêmica
- **Skill**: Habilidades técnicas
- **Language**: Idiomas
- **Contact**: Informações de contato
- **Address**: Endereços
- **ShortLink**: Links curtos para compartilhamento
- **AccessLog**: Logs de acesso para auditoria

## 🔧 Como Executar

### Pré-requisitos
- Docker e Docker Compose
- .NET 9.0 SDK (para desenvolvimento)
- Node.js 18+ (para desenvolvimento do frontend)

### Desenvolvimento
```bash
# Clonar o repositório
git clone <repository-url>
cd CVFast

# Executar com Docker Compose
docker-compose -f docker-compose.dev.yml up

# Ou executar individualmente:

# Backend
cd Api/CVFastApi
dotnet run

# Frontend
cd Front
npm install
npm run dev
```

### Produção
```bash
# Deploy completo
docker-compose up -d

# Verificar status
docker-compose ps
```

## 🌍 Variáveis de Ambiente

### Backend
- `ASPNETCORE_ENVIRONMENT`: Ambiente de execução
- `ConnectionStrings__DefaultConnection`: String de conexão PostgreSQL
- `JwtSettings__SecretKey`: Chave secreta para JWT
- `JwtSettings__Issuer`: Emissor do token
- `JwtSettings__Audience`: Audiência do token

### Frontend
- `VITE_API_URL`: URL da API principal
- `VITE_INTEGRATION_API_URL`: URL da API de integração

## 📚 Documentação

- **Casos de Uso**: [Docs/Diagrams/Especificacoes_Casos_de_Uso_CVFast.md](./Docs/Diagrams/Especificacoes_Casos_de_Uso_CVFast.md)
- **API Principal**: Swagger disponível em `/` da API
- **API de Integração**: [README específico](./Api/CVFastApi.Integration/README.md)

## 🔒 Segurança

- Autenticação JWT com expiração configurável
- Senhas hasheadas com algoritmos seguros
- Validação de entrada em todos os endpoints
- Logs de auditoria para rastreabilidade
- Controle de acesso baseado em propriedade de recursos
- CORS configurado adequadamente

## 📈 Monitoramento

- Logs estruturados em todos os serviços
- Registro de acessos aos currículos
- Health checks nos containers
- Métricas de uso da API

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

