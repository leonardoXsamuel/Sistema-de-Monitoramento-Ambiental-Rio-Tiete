import * as signalR from '@microsoft/signalr'
 
let connection: signalR.HubConnection | null = null
 
export function getConnection(): signalR.HubConnection {
  if (!connection) {
    const token = localStorage.getItem('token') ?? ''
 
    connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:65148/hubs/chat', {
        accessTokenFactory: () => localStorage.getItem('token') ?? '',
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000])
      .configureLogging(signalR.LogLevel.Warning)
      .build()
  }
  return connection
}
 
export async function startConnection(): Promise<void> {
  const conn = getConnection()
  if (conn.state === signalR.HubConnectionState.Disconnected) {
    await conn.start()
  }
}
 
export async function stopConnection(): Promise<void> {
  if (connection) {
    await connection.stop()
    connection = null
  }
}
 
// ── Métodos do Hub ─────────────────────────────────────────────────────────────
export const hubActions = {
  entrarNoChatRoom: (roomId: number) =>
    getConnection().invoke('EntrarNoChatRoom', roomId),
 
  sairDoChatRoom: (roomId: number) =>
    getConnection().invoke('SairDoChatRoom', roomId),
 
  enviarMensagem: (content: string, roomId: number) =>
    getConnection().invoke('EnviarMensagem', content, roomId),
 
  digitando: (roomId: number, isTyping: boolean) =>
    getConnection().invoke('Digitando', roomId, isTyping),
 
  criarChatRoom: (name: string) =>
    getConnection().invoke('CriarChatRoom', { name }),
 
  notificarArquivo: (file: object, roomId: number) =>
    getConnection().invoke('NotificaçãoDeArquivoCarregado', file, roomId),
}
