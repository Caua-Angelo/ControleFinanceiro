# Controle Financeiro

![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![React](https://img.shields.io/badge/React-18.x-61DAFB?logo=react)
![TypeScript](https://img.shields.io/badge/TypeScript-5.x-3178C6?logo=typescript)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)
![TailwindCSS](https://img.shields.io/badge/TailwindCSS-3.x-06B6D4?logo=tailwindcss)
![CI](https://github.com/Caua-Angelo/ControleFinanceiro/actions/workflows/ci.yml/badge.svg)

Sistema de gerenciamento financeiro pessoal com controle de usuários, categorias e transações. Desenvolvido com foco em arquitetura em camadas, boas práticas e qualidade de código.

## 🛠️ Tecnologias Utilizadas

### Backend

* .NET 10
* ASP.NET Core
* Entity Framework Core
* PostgreSQL
* AutoMapper
* JWT Authentication
* BCrypt
* Serilog
* Swagger

### Frontend

* React
* TypeScript
* Vite
* React Router
* TailwindCSS
* Axios

### Testes

* xUnit
* Moq
* FluentAssertions
* WebApplicationFactory (testes de integração)

## 📐 Arquitetura

O backend segue uma arquitetura em camadas:

* **API**: Controllers, Middlewares
* **Application**: Services, DTOs, Interfaces, Mappings
* **Domain**: Entidades, Validações e Contratos
* **Infra.Data**: Repositórios e contexto EF Core
* **Infra.IoC**: Injeção de dependências

O frontend foi estruturado utilizando componentes reutilizáveis, layout com Outlet e separação de páginas por responsabilidade.

## 📂 Estrutura do Projeto

### Backend

```
Back-End/
├── ControleFinanceiro.API/          # Controllers e configuração
├── ControleFinanceiro.Application/  # Services, DTOs, Interfaces
├── ControleFinanceiro.Domain/       # Entidades e contratos
├── ControleFinanceiro.Infra.Data/   # Repositórios e contexto EF Core
├── ControleFinanceiro.Infra.IoC/    # Injeção de dependências
└── ControleFinanceiro.Test/         # Testes unitários e de integração
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

* ✅ CRUD completo de Transações
* ✅ CRUD completo de Usuários
* ✅ CRUD completo de Categorias
* ✅ Autenticação e autorização com JWT
* ✅ Hash de senhas com BCrypt
* ✅ Validações de domínio (Domain Validation)
* ✅ Regras de negócio no domínio
* ✅ Middleware global de tratamento de exceções
* ✅ Logging com Serilog
* ✅ Documentação com Swagger

### Frontend

* ✅ Tela de listagem e criação de transações
* ✅ Validação de formulários
* ✅ Formatação de valores monetários
* ✅ Formatação e validação de datas
* ✅ Paginação de transações
* ✅ Modal de edição
* ✅ Integração com API via Axios
* ✅ Acessibilidade com labels semânticos
* ⚠️ Interface ainda em refinamento visual

### Testes

* ✅ Testes unitários de domínio (Usuário, Categoria, Transação)
* ✅ Testes de integração dos controllers (Auth, Usuário, Categoria, Transação)
* ✅ Testes de casos negativos (404, 400, 401, 409)
* ✅ Pipeline de CI com GitHub Actions

## 📋 Regras de Negócio Implementadas

* **Usuários menores de 18 anos** só podem criar transações do tipo **Despesa**
* **Categorias são compatíveis** com o tipo de transação:

  * Receita: somente categorias com finalidade "Receita" ou "Ambas"
  * Despesa: somente categorias com finalidade "Despesa" ou "Ambas"
* **Email único** por usuário — não é possível cadastrar dois usuários com o mesmo e-mail
* **Valores monetários** são formatados automaticamente no padrão brasileiro (R$)
* **Datas** são obrigatórias para todas as transações

## 🔌 Endpoints da API

### Auth

* `POST /api/auth/login` - Autenticação
* `POST /api/auth/register` - Registro de usuário

### Transações

* `GET /api/transacoes` - Lista todas as transações
* `GET /api/transacoes/{id}` - Consulta transação por ID
* `POST /api/transacoes` - Cria nova transação
* `PUT /api/transacoes/{id}` - Atualiza transação
* `DELETE /api/transacoes/{id}` - Exclui transação

### Usuários

* `GET /api/usuarios` - Lista todos os usuários
* `GET /api/usuarios/{id}` - Consulta usuário por ID
* `POST /api/usuarios` - Cria novo usuário
* `PUT /api/usuarios/{id}` - Atualiza dados do usuário (exceto email)
* `DELETE /api/usuarios/{id}` - Exclui usuário

### Categorias

* `GET /api/categorias` - Lista todas as categorias
* `GET /api/categorias/{id}` - Consulta categoria por ID
* `POST /api/categorias` - Cria nova categoria
* `PUT /api/categorias/{id}` - Atualiza categoria
* `DELETE /api/categorias/{id}` - Exclui categoria

📚 **Documentação completa:** `https://localhost:7244/swagger`

## ▶️ Como Executar o Projeto

### Pré-requisitos

* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* [Node.js](https://nodejs.org/)
* [PostgreSQL](https://www.postgresql.org/download/)

### 🔧 Backend

1. Acesse a pasta do backend:

```bash
cd Controle_Financeiro/Back-End
```

2. Configure o ambiente

* Copie o arquivo:

appsettings.Example.json

* Renomeie para:

appsettings.Development.json

* Preencha com suas credenciais (banco + JWT)

### 🔐 Observação

Dados sensíveis (connection strings e JWT) não são versionados no repositório.
Utilize o arquivo `appsettings.Example.json` como base para configuração local.

3. Configure o banco de dados

Você pode usar PostgreSQL local ou serviços como Supabase.

> **Banco hospedado:** Este projeto utiliza PostgreSQL (ex: Supabase ou local). Caso o banco não esteja disponível, configure um banco local.

4. Aplique as migrations:

```bash
dotnet ef database update --project ControleFinanceiro.Infra.Data --startup-project ControleFinanceiro.API
```

5. Execute a aplicação:

```bash
dotnet run --project ControleFinanceiro.API
```

A API ficará disponível em `https://localhost:7244` e o Swagger em `https://localhost:7244/swagger`.

### 🖥️ Frontend

1. Acesse a pasta do frontend:

```bash
cd Controle_Financeiro/Front-End
```

2. Instale as dependências:

```bash
npm install
```

3. Execute o projeto:

```bash
npm run dev
```

O frontend ficará disponível em `http://localhost:5173`.

## 🧪 Como Rodar os Testes

```bash
cd Controle_Financeiro/Back-End
dotnet test
```

Os testes de integração utilizam banco em memória (InMemory) e configuração isolada via `appsettings.Testing.json`.

## 🔧 Troubleshooting

**Banco de dados não conecta**

* Verifique se o PostgreSQL está rodando
* Confirme se a connection string está correta
* Execute `dotnet ef database update` para aplicar as migrations

**Porta já em uso**

* Backend padrão: `7244` — altere em `launchSettings.json`
* Frontend padrão: `5173` — altere em `vite.config.ts`

**Erros de dependências no Frontend**

```bash
rm -rf node_modules package-lock.json
npm install
```

**Erros de build no Backend**

```bash
dotnet clean
dotnet build
```

## 👤 Autor

Desenvolvido por **Cauã Angelo Santos Lopes**

📧 Email: [cauasantosangelo@gmail.com](mailto:cauasantosangelo@gmail.com)
💼 LinkedIn: [Cauã Angelo](https://www.linkedin.com/in/cauã-angelo-santos)
🐙 GitHub: [Caua-Angelo](https://github.com/Caua-Angelo)
