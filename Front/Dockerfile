# Build stage
FROM node:18-alpine AS build

WORKDIR /app

# Copiar package.json e package-lock.json
COPY package*.json ./

# Instalar dependências
RUN npm ci

# Copiar código fonte
COPY . .

# Build da aplicação
RUN npm run build

# Só exporta os arquivos prontos para volume compartilhado
FROM alpine AS export
WORKDIR /export
COPY --from=build /app/dist .

CMD ["sh"]