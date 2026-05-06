# MartChat — Frontend React

Sistema de comunicação em rede para monitoramento ambiental do Rio Tietê.
APS 2026/1 — Ciência da Computação — UNIP

---

## Pré-requisitos

- Node.js 18+
- Backend ASP.NET Core rodando em `http://localhost:65148`

---

## Como rodar

```bash
# Instalar dependências
npm install

# Iniciar em modo dev
npm run dev
```

Acessa: http://localhost:5173

---

## Estrutura de pastas

```
src/
├── pages/
│   ├── LoginPage.tsx         ← Tela de login
│   ├── RegisterPage.tsx      ← Tela de cadastro
│   ├── ChatPage.tsx          ← Página principal do chat
│   └── AuthPage.module.css   ← CSS das páginas de auth
├── components/
│   ├── layout/
│   │   ├── Sidebar.tsx       ← Sidebar com salas e perfil
│   │   └── Sidebar.module.css
│   └── chat/
│       ├── ChatArea.tsx      ← Área principal do chat
│       ├── MessageBubble.tsx ← Bolha de mensagem
│       ├── FilePanel.tsx     ← Painel de arquivos
│       └── *.module.css
├── services/
│   ├── api.ts                ← Axios com interceptor JWT
│   └── signalr.ts            ← Conexão e métodos do Hub
├── store/
│   ├── authStore.ts          ← Estado de autenticação (Zustand)
│   └── chatStore.ts          ← Estado do chat (Zustand)
├── hooks/
│   └── useSignalR.ts         ← Hook que registra eventos do SignalR
└── types/
    └── index.ts              ← Tipos TypeScript
```

---

## Integração com o Backend

### Endpoints utilizados

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/auth/login` | Login |
| POST | `/api/auth/register` | Registro |
| GET | `/api/ChatRoom/GetAllRooms` | Listar salas |
| POST | `/api/ChatRoom/createChatRoom` | Criar sala |
| GET | `/api/rooms/{id}/messages` | Histórico |
| POST | `/api/files/upload` | Upload de arquivos |
| GET | `/api/files/room/{id}` | Arquivos da sala |
| GET | `/api/files/{id}/download` | Download |

### SignalR Hub: `/hubs/chat`

| Método (cliente→servidor) | Descrição |
|---|---|
| `EntrarNoChatRoom(roomId)` | Entra na sala |
| `SairDoChatRoom(roomId)` | Sai da sala |
| `EnviarMensagem(content, roomId)` | Envia mensagem |
| `Digitando(roomId, isTyping)` | Indicador de digitação |
| `CriarChatRoom({name})` | Cria nova sala |

| Evento (servidor→cliente) | Descrição |
|---|---|
| `ReceberMensagem` | Nova mensagem |
| `UsuarioEntrou` | Usuário entrou |
| `UsuarioSaiu` | Usuário saiu |
| `SalaCriada` | Nova sala criada |
| `ArquivoDisponível` | Novo arquivo |
| `UsuárioDigitando` | Digitando... |

---

## Para a apresentação no laboratório

Troca a URL do backend no `vite.config.ts` e em `src/services/signalr.ts`:

```ts
// vite.config.ts — proxy
target: 'http://192.168.x.x:65148'  // IP do servidor na rede local

// signalr.ts
.withUrl('http://192.168.x.x:65148/hubs/chat', ...)
```