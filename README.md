# blog-clube-leitura
O Blog de Clube de Leitura é uma plataforma online onde membros de um clube de leitura podem compartilhar suas experiências de leitura e discutir sobre diferentes livros. Este projeto é desenvolvido usando ASP.NET Core MVC e Docker, com um banco de dados PostgreSQL.

## Funcionalidades

- **Registro de Usuários e Autenticação**: Os usuários podem se registrar e autenticar-se para participar do clube de leitura.
- **Gestão de Posts**: Os usuários podem criar posts sobre livros que leram.
- **Administração**: Administradores podem gerenciar usuários e posts, bem como nomear gestores para ajudar na moderação do conteúdo.
- **Discussão**: Espaço para comentários e discussões sobre os livros.

## Tecnologias Utilizadas

- **ASP.NET Core MVC**: Framework para a criação da aplicação web.
- **Entity Framework Core**: ORM para acesso e gestão da base de dados.
- **PostgreSQL**: Sistema de gerenciamento de banco de dados.
- **Docker**: Containerização da aplicação e do banco de dados.

## Estrutura do Projeto
- **Controllers/**: Contém os controladores MVC que gerenciam a lógica de entrada.
- **Models/**: Define os modelos de dados.
- **Views/**: Contém as páginas Razor para a interface de usuário.
- **docker-compose.yml**: Arquivo de configuração do Docker Compose para ambiente de desenvolvimento.
- **Dockerfile**: Dockerfile para construção da imagem de produção.



## Configuração Inicial

### Pré-requisitos

- Docker
- Docker Compose

### Configuração do Ambiente de Desenvolvimento

1. **Clone o repositório**
- git clone [https://link-para-o-repositorio.com](https://github.com/JulioCesar-X/blog-clube-leitura.git)
- cd blog-clube-leitura
  
### Construa e execute o ambiente com Docker Compose
- docker-compose up --build
  
Isso iniciará a aplicação e o serviço de banco de dados. A aplicação estará disponível em http://localhost:8000.

## Contribuição
Contribuições são sempre bem-vindas!
