# Controle Financeiro

![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![React](https://img.shields.io/badge/React-18.x-61DAFB?logo=react)
![TypeScript](https://img.shields.io/badge/TypeScript-5.x-3178C6?logo=typescript)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)
![TailwindCSS](https://img.shields.io/badge/TailwindCSS-3.x-06B6D4?logo=tailwindcss)

Projeto desenvolvido como parte de um teste técnico, com o objetivo de demonstrar organização de código, separação de responsabilidades e boas práticas no desenvolvimento full stack.

O sistema tem como finalidade o gerenciamento financeiro, com controle de usuários, categorias e transações.

## 🛠️ Tecnologias Utilizadas

### Backend
- .NET 8
- Entity Framework Core
- PostgreSQL
- AutoMapper
- Serilog
- Swagger

### Frontend
- React
- TypeScript
- Vite
- React Router
- TailwindCSS
- Axios

## 📐 Arquitetura

O backend segue uma arquitetura em camadas:

- **API**: Controllers
- **Application**: Services, DTOs, Interfaces
- **Domain**: Entidades e contratos
- **Infrastructure**: Repositórios e contexto de dados

O frontend foi estruturado utilizando componentes reutilizáveis, layout com Outlet e separação de páginas por responsabilidade.

## 📂 Estrutura do Projeto

### Backend
```
Back-End/
├── API/                    # Controllers e configuração
├── Application/            # Services, DTOs, Interfaces
├── Domain/                 # Entidades e contratos
└── Infrastructure/         # Repositórios e contexto EF Core
```

### Frontend
```
Front-End/
├── src/
│   ├── Pages/             # Páginas da aplicação
│   ├── Services/          # Integração com API
│   ├── Types/             # Tipagens TypeScript
│   ├── Components/        # Componentes reutilizáveis
│   └── assets/            # Imagens e recursos
```

## ✅ Funcionalidades Implementadas

### Backend
- ✅ CRUD completo de Transações
- ✅ CRUD completo de Usuários
- ✅ CRUD completo de Categorias
- ✅ Validação de regras de negócio (menor de idade só pode criar despesas)
- ✅ Filtro de categorias por tipo de transação
- ✅ Logging com Serilog
- ✅ Documentação com Swagger

### Frontend
- ✅ Tela de listagem e criação de transações
- ✅ Validação de formulários
- ✅ Formatação de valores monetários
- ✅ Formatação e validação de datas
- ✅ Paginação de transações
- ✅ Modal de edição
- ✅ Integração com API via Axios
- ✅ Acessibilidade com labels semânticos
- ⚠️ Interface ainda em refinamento visual

## ⚠️ Status do Projeto

🚧 **Projeto em desenvolvimento**

### 📋 Funcionalidades Planejadas
- [x] Resumo de gastos por usuário com filtro de mês
- [x] Resumo de gastos gerais (dashboard)
- [ ] Validações adicionais no frontend
- [ ] Filtros avançados de transações
- [ ] Exportação de relatórios (PDF/Excel)
- [ ] Testes unitários e de integração
- [ ] Deploy em ambiente de produção

### 🎯 Situação Atual do Frontend
⚠️ O front-end encontra-se em **estágio funcional mas em refinamento**, tendo como foco principal demonstrar:
- ✅ Estrutura organizada do projeto
- ✅ Separação de responsabilidades (Pages, Services, Types)
- ✅ Integração completa com o backend
- ✅ CRUD funcional de transações
- ⚠️ Interface visual ainda em aprimoramento

## 📋 Regras de Negócio Implementadas

- **Usuários menores de 18 anos** só podem criar transações do tipo **Despesa**
- **Categorias são filtradas** com base no tipo de transação selecionado:
  - Receita: categorias com finalidade "Receita" ou "Ambos"
  - Despesa: categorias com finalidade "Despesa" ou "Ambos"
- **Valores monetários** são formatados automaticamente no padrão brasileairo (R$)
- **Datas** são obrigatórias para todas as transações
- **Validação de campos** antes do envio ao backend

## 🔐 Configuração do Banco de Dados

Este projeto utiliza **PostgreSQL**.

### ⚠️ Importante para este Teste Técnico
Por questões de **praticidade para avaliação**, a connection string está configurada diretamente no código, apontando para um banco de dados de teste gratuito no **Render.com** (válido até **22 de março de 2026**).

## 🔌 Endpoints Principais da API

### Transações
- `GET /api/transacao` - Lista todas as transações
- `POST /api/transacao` - Cria nova transação
- `PUT /api/transacao/{id}` - Atualiza transação
- `DELETE /api/transacao/{id}` - Exclui transação

### Usuários
- `GET /api/usuario` - Lista todos os usuários
- `POST /api/usuario` - Cria novo usuário
- `PUT /api/usuario/{id}` - Atualiza usuário
- `DELETE /api/usuario/{id}` - Exclui usuário

### Categorias
- `GET /api/categoria` - Lista todas as categorias
- `POST /api/categoria` - Cria nova categoria
- `PUT /api/categoria/{id}` - Atualiza categoria
- `DELETE /api/categoria/{id}` - Exclui categoria

📚 **Documentação completa:** https://localhost:7244/swagger

## ▶️ Como Executar o Projeto

### 🔧 Backend (.NET)

1. Acesse a pasta do backend:
```bash
cd md/back-end
```

2. Restaure as dependências:
```bash
dotnet restore
```

3. Configure a connection string (ver seção **Configuração do Banco de Dados**).

4. Execute a aplicação:
```bash
dotnet run
```

A API ficará disponível em:
- https://localhost:7244

A documentação da API pode ser acessada via Swagger:
- https://localhost:7244/swagger

### 🖥️ Frontend (React)

O front-end foi desenvolvido com Vite + React + TypeScript.

1. Acesse a pasta do frontend:
```bash
cd md/front-end
```

2. Instale as dependências:
```bash
npm install
```

3. Execute o projeto:
```bash
npm run dev
```

O front-end ficará disponível em:
- http://localhost:5173

⚠️ **Observação:**
O front-end ainda está em fase de refinamento visual, mas apresenta funcionalidades completas de CRUD.

## 🔧 Troubleshooting

### 🚫 Não consegue conectar ao banco de dados (Render.com)

**Problema:** Algumas redes corporativas, institucionais ou provedores de internet bloqueiam conexões externas a serviços como o Render.com.

**Sintomas:**
- Timeout ao tentar conectar
- Erro "Unable to connect to the remote server"
- Aplicação não carrega dados do banco

**Possíveis soluções:**

1. **Teste em outra rede:**
   - Use sua rede doméstica ou dados móveis
   - Teste em um café ou ambiente com Wi-Fi público

2. **Verifique firewall/proxy corporativo:**
   - Redes corporativas frequentemente bloqueiam conexões externas
   - Entre em contato com o setor de TI para liberar acesso temporário

3. **Use VPN (se permitido):**
   - Algumas VPNs podem contornar bloqueios de rede
   - ⚠️ Verifique se o uso de VPN é permitido em sua rede

4. **Alternativa: Configure um banco local:**
   
   **Passo 1: Instale o PostgreSQL localmente**
   - Baixe em: https://www.postgresql.org/download/
   - Durante a instalação, defina uma senha para o usuário `postgres`
   
   **Passo 2: Crie o banco de dados**
```bash
   # Via terminal/cmd (após instalar PostgreSQL)
   createdb controle_financeiro
   
   # OU via pgAdmin (interface gráfica)
   # Botão direito em "Databases" > Create > Database
   # Nome: controle_financeiro
```
   
   **Passo 3: Configure a connection string**
   
   Edite o arquivo `appsettings.json` ou configure a variável de ambiente:
```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=controle_financeiro;Username=postgres;Password=SUA_SENHA_AQUI"
   }
```
   
   **Passo 4: Crie as tabelas no banco com Entity Framework**
   
   Navegue até a pasta do projeto backend e execute:
```bash
   # Criar uma nova migration (se necessário)
   dotnet ef migrations add InitialCreate
   
   # Aplicar as migrations e criar as tabelas
   dotnet ef database update
```
   
   ✅ Pronto! Agora você tem um banco de dados local totalmente funcional.

### Erro de CORS no Frontend
Certifique-se de que o backend está configurado para aceitar requisições do frontend.

### Banco de dados não conecta (outros motivos)
- Verifique se a connection string está correta
- Confirme se o PostgreSQL está rodando (caso seja local)
- Valide se o banco de dados de teste no Render.com ainda está ativo (válido até 13/02/2025)

### Porta já em uso
- Backend padrão: 7244
- Frontend padrão: 5173

Altere em `launchSettings.json` (backend) ou `vite.config.ts` (frontend)

### Erros de dependências no Frontend
```bash
# Limpar cache e reinstalar
rm -rf node_modules package-lock.json
npm install
```

### Erros de build no Backend
```bash
# Limpar e rebuildar
dotnet clean
dotnet build
```

### Timeout ou lentidão na primeira requisição
O Render.com em plano gratuito coloca instâncias inativas em "sleep mode". A primeira requisição pode demorar 30-60 segundos para "acordar" o servidor.

**Solução:** Aguarde e tente novamente.

## 📌 Observações Finais

Este projeto foi desenvolvido com foco em:
- ✅ Qualidade de código
- ✅ Arquitetura limpa e escalável
- ✅ Boas práticas de desenvolvimento
- ✅ Separação de responsabilidades
- ✅ Código organizado e legível

A estrutura adotada permite fácil expansão e refinamento das funcionalidades existentes.

## 👤 Autor

Desenvolvido por **[Cauã Angelo santos lopes]**

📧 Email: [cauasantosangelo@gmail.com]  
💼 LinkedIn: [Cauã Angelo][www.linkedin.com/in/cauã-angelo-santos]  
🐙 GitHub: [Caua-Angelo](https://github.com/Caua-Angelo)


---

⭐ **Obrigado por avaliar este projeto!**