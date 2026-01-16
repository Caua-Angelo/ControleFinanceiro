Controle Financeiro

Projeto desenvolvido como parte de um teste técnico, com o objetivo de demonstrar organização de código, separação de responsabilidades e boas práticas no desenvolvimento full stack.

O sistema tem como finalidade o gerenciamento financeiro, com controle de usuários, categorias e transações.

🛠️ Tecnologias Utilizadas
Backend

.NET 8

Entity Framework Core

PostgreSQL

AutoMapper

Serilog

Swagger

Frontend

React

TypeScript

Vite

React Router

TailwindCSS

Axios

📐 Arquitetura

O backend segue uma arquitetura em camadas:

API: Controllers

Application: Services, DTOs, Interfaces

Domain: Entidades e contratos

Infrastructure: Repositórios e contexto de dados

O frontend foi estruturado utilizando componentes reutilizáveis, layout com Outlet e separação de páginas por responsabilidade.

⚠️ Status do Projeto

🚧 Projeto em desenvolvimento

Devido ao prazo do teste técnico, nem todas as funcionalidades foram concluídas.
Apesar disso, a base do sistema está estruturada seguindo boas práticas, permitindo fácil evolução e manutenção.

Funcionalidades planejadas:

CRUD completo de usuários

CRUD de categorias

CRUD de transações

Autenticação e autorização

Validações adicionais no frontend

Situação do Frontend

⚠️ O front-end encontra-se em estágio inicial e ainda não está totalmente utilizável, tendo como foco principal demonstrar:

Estrutura do projeto

Organização de páginas e layout

Integração inicial com o backend

🔐 Configuração do Banco de Dados

Este projeto utiliza PostgreSQL.

Por boas práticas de segurança, o recomendado é que as credenciais do banco de dados sejam configuradas por meio de variáveis de ambiente, evitando que informações sensíveis fiquem versionadas no código.

No entanto, excepcionalmente para fins de demonstração neste teste técnico, a connection string foi mantida de forma explícita no arquivo de configuração, utilizando um banco de dados de teste gratuito fornecido pelo Render.com.

⚠️ Importante:
Em um ambiente de produção, essa abordagem não é recomendada. O correto é utilizar variáveis de ambiente ou serviços de gerenciamento de segredos.

Configuração recomendada (padrão profissional)

A aplicação já está preparada para ler a connection string a partir da seguinte variável de ambiente:

ConnectionStrings__DefaultConnection

▶️ Como Executar o Projeto
🔧 Backend (.NET)

Acesse a pasta do backend:

cd md/back-end


Restaure as dependências:

dotnet restore


Configure a connection string (ver seção Configuração do Banco de Dados).

Execute a aplicação:

dotnet run


A API ficará disponível em:

https://localhost:7244


A documentação da API pode ser acessada via Swagger:

https://localhost:7244/swagger

🖥️ Frontend (React)

O front-end foi desenvolvido com Vite + React + TypeScript.

Acesse a pasta do frontend:

cd md/front-end


Instale as dependências:

npm install


Execute o projeto:

npm run dev


O front-end ficará disponível em:

http://localhost:5173


⚠️ Observação:
O front-end ainda está em fase inicial e não representa o fluxo completo da aplicação.

📌 Observações Finais

Este projeto foi desenvolvido com foco em qualidade de código, arquitetura limpa e boas práticas, mesmo com escopo reduzido devido ao prazo do teste técnico.

A estrutura adotada permite fácil expansão e refinamento das funcionalidades existentes.
