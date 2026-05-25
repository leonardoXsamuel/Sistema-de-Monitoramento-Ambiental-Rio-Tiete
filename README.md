# MartChat

Sistema de chat corporativo em tempo real, desenvolvido como projeto acadêmico (APS) para a disciplina de Redes de Computadores.

A aplicação demonstra comunicação cliente-servidor via **TCP/IP** usando WebSockets (SignalR), com autenticação JWT, salas de chat, histórico de mensagens e upload de arquivos.

---

## Tecnologias

| Camada     | Stack                                                      |
|------------|------------------------------------------------------------|
| Backend    | ASP.NET Core 8, SignalR, Entity Framework Core 8, SQL Server |
| Frontend   | React 19, TypeScript, Vite, Zustand, @microsoft/signalr   |
| Auth       | JWT (HS256, 120 min)                                       |
| Infra      | Docker, Docker Compose, nginx                              |

---

## Funcionalidades

- Cadastro e login com autenticação JWT
- Salas de chat em tempo real via SignalR (WebSocket)
- Histórico de mensagens persistido no banco de dados
- Upload de arquivos (PDF, DOCX, XLSX — até 200 MB)
- Indicador de digitação em tempo real
- Notificação visual ao enviar/receber arquivos
- Interface responsiva (desktop e mobile)

---

## Como rodar

### Opção 1 — Docker (recomendado)

> Pré-requisito: [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado.

```bash
# Na raiz do projeto
docker compose up --build
```

Aguarde todos os serviços subirem (cerca de 1-2 minutos na primeira vez) e acesse:

**http://localhost**

> O banco de dados, o backend e o frontend sobem automaticamente em ordem correta.

Para parar:

```bash
docker compose down
```

Para parar e apagar os dados do banco:

```bash
docker compose down -v
```

---

### Opção 2 — Rodar localmente (sem Docker)

#### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- SQL Server (LocalDB incluso no Visual Studio, ou SQL Server Express)

#### Backend

```bash
cd ApsMartChat

# Configurar a chave JWT (necessário uma vez)
dotnet user-secrets set "Jwt:Key" "sua-chave-secreta-minimo-32-caracteres"

# Rodar (migrations aplicadas automaticamente na inicialização)
dotnet run
```

Backend disponível em `http://localhost:65148`.

#### Frontend

```bash
cd frontend
npm install
npm run dev
```

Frontend disponível em `http://localhost:5173`.

---

## Estrutura do projeto

```
ApsMartChat/
├── ApsMartChat/              # Backend ASP.NET Core
│   ├── Controllers/          # Endpoints REST (auth, rooms, messages, files)
│   ├── Hubs/                 # ChatHub — lógica SignalR em tempo real
│   ├── Services/             # Regras de negócio
│   ├── Models/               # Entidades do banco (EF Core)
│   ├── DTOs/                 # Objetos de transferência de dados
│   ├── Profiles/             # Mapeamentos AutoMapper
│   ├── Data/                 # AppDbContext
│   └── Dockerfile
├── frontend/                 # Frontend React
│   ├── src/
│   │   ├── api/              # Chamadas REST (axios)
│   │   ├── components/       # Componentes reutilizáveis
│   │   ├── hooks/            # useSignalR (conexão em tempo real)
│   │   ├── pages/            # LoginPage, RegisterPage, ChatPage
│   │   ├── store/            # Estado global (Zustand)
│   │   └── types/            # Interfaces TypeScript
│   ├── nginx.conf            # Proxy reverso para API e SignalR
│   └── Dockerfile
├── docker-compose.yml        # Orquestração dos 3 serviços
└── README.md
```

---

## Fluxo de uma mensagem em tempo real

```
Usuário digita e pressiona Enter
        │
        ▼
React: handleSend()
  ├─ addMessage() → atualiza UI imediatamente (optimistic update)
  └─ SignalR.invoke("EnviarMensagem", conteudo, roomId)
              │
              ▼ WebSocket (TCP/IP)
        ASP.NET Core — ChatHub.EnviarMensagem()
          ├─ Salva mensagem no SQL Server
          └─ Clients.Group("room_{id}").SendAsync("ReceberMensagem", msg)
                      │
              ┌───────┴────────────┐
              ▼                    ▼
        Remetente              Outros usuários
      (dedup ignora)         (exibe nova mensagem)
```

---

## Rotas da API

### Autenticação
| Método | Rota                  | Descrição              |
|--------|-----------------------|------------------------|
| POST   | `/api/auth/register`  | Registrar novo usuário |
| POST   | `/api/auth/login`     | Login e obter JWT      |

### Salas
| Método | Rota         | Descrição                    |
|--------|--------------|------------------------------|
| GET    | `/api/rooms` | Listar salas (paginado)      |

### Mensagens
| Método | Rota                          | Descrição                 |
|--------|-------------------------------|---------------------------|
| GET    | `/api/rooms/{roomId}/messages`| Histórico de mensagens    |

### Arquivos
| Método | Rota                          | Descrição                      |
|--------|-------------------------------|--------------------------------|
| POST   | `/api/files/upload/{roomId}`  | Upload de arquivo              |
| GET    | `/api/files/download/{id}`    | Download de arquivo            |
| GET    | `/api/files/room/{roomId}`    | Listar arquivos de uma sala    |

### SignalR Hub — `/hubs/chat`

**Métodos que o cliente invoca:**

| Método                        | Parâmetros          | Descrição               |
|-------------------------------|---------------------|-------------------------|
| `EntrarNoChatRoom`            | `roomId`            | Entrar na sala          |
| `SairDoChatRoom`              | `roomId`            | Sair da sala            |
| `EnviarMensagem`              | `content, roomId`   | Enviar mensagem         |
| `Digitando`                   | `roomId, isTyping`  | Indicador de digitação  |
| `CriarChatRoom`               | `dto`               | Criar nova sala         |

**Eventos que o cliente escuta:**

| Evento              | Dados          | Descrição                      |
|---------------------|----------------|--------------------------------|
| `ReceberMensagem`   | `Message`      | Nova mensagem na sala          |
| `UsuarioDigitando`  | `label, bool`  | Alguém está digitando          |
| `ArquivoDisponivel` | `FileTransfer` | Novo arquivo disponível        |
| `SalaCriada`        | `ChatRoom`     | Nova sala criada               |

---

## Variáveis de ambiente

### Backend

| Variável                               | Descrição                              |
|----------------------------------------|----------------------------------------|
| `ConnectionStrings__DefaultConnection` | Connection string do SQL Server        |
| `Jwt__Key`                             | Chave secreta JWT (mínimo 32 chars)    |
| `ASPNETCORE_URLS`                      | URL de escuta do Kestrel               |

### Frontend

| Variável        | Padrão                      | Descrição                             |
|-----------------|-----------------------------|---------------------------------------|
| `VITE_API_URL`  | `http://localhost:65148`    | URL base da API (vazio = mesma origem via nginx) |

---

## Autores

Desenvolvido por alunos do curso de Ciência da Computação — APS 2025.
