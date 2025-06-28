# Build stage
FROM node:18-alpine AS build

WORKDIR /app

# Copiar package.json e package-lock.json
COPY package*.json ./

# Instalar dependências
RUN npm ci

# Copiar código fonte
COPY . .

# Definir variável de ambiente para build
ARG VITE_API_BASE_URL=http://localhost:5207/api
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

# Build da aplicação
RUN npm run build

# Production stage
FROM nginx:alpine AS production

# Copiar arquivos buildados
COPY --from=build /app/dist /usr/share/nginx/html

# Copiar configuração customizada do nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expor porta 80
EXPOSE 80

# Comando para iniciar o nginx
CMD ["nginx", "-g", "daemon off;"]
