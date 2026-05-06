import { useEffect, useRef } from 'react'
import { HubConnectionState } from '@microsoft/signalr'
import toast from 'react-hot-toast'
import { getConnection, startConnection } from '../services/signalR'
import { useChatStore } from '../store/chatStore'
import { useAuthStore } from '../store/authStore'
import type { MessageResponseDTO, FileTransferResponseDTO, RoomWithId } from '../types/types'
 
export function useSignalR() {
  const { username } = useAuthStore()
  const { addMessage, addFile, setTypingUser, addRoom, setConnected } = useChatStore()
  const registered = useRef(false)
 
  useEffect(() => {
    if (registered.current) return
    registered.current = true
 
    const conn = getConnection()
 
    startConnection()
      .then(() => {
        setConnected(true)
 
        // ── Eventos do servidor ──────────────────────────────────────────────
        conn.on('ReceberMensagem', (msg: MessageResponseDTO) => {
          addMessage(msg)
          if (msg.sender?.username !== username) {
            toast(`💬 ${msg.sender?.displayName}: ${msg.content.slice(0, 40)}`, {
              style: { background: '#1a3a2a', color: '#a8d5b5', border: '1px solid #2d5a3d' },
            })
          }
        })
 
        conn.on('UsuarioEntrou', (info: string) => {
          toast(`🌿 ${info}`, {
            style: { background: '#1a3a2a', color: '#a8d5b5' },
          })
        })
 
        conn.on('UsuarioSaiu', (info: string) => {
          toast(`🍂 ${info}`, {
            style: { background: '#1a3a2a', color: '#a8d5b5' },
          })
        })
 
        conn.on('SalaCriada', (room: RoomWithId) => {
          addRoom(room)
          toast(`🏕️ Sala criada: ${room.name}`, {
            style: { background: '#1a3a2a', color: '#a8d5b5' },
          })
        })
 
        conn.on('ArquivoDisponível', (file: FileTransferResponseDTO) => {
          addFile(file)
          toast(`📎 Novo arquivo: ${file.nomeOriginal}`, {
            style: { background: '#1a3a2a', color: '#a8d5b5' },
          })
        })
 
        conn.on('UsuárioDigitando', (info: string, isTyping: boolean) => {
          const name = info.replace('Usuario', '')
          setTypingUser(name, isTyping)
        })
      })
      .catch(() => setConnected(false))
 
    conn.onreconnected(() => setConnected(true))
    conn.onreconnecting(() => setConnected(false))
    conn.onclose(() => setConnected(false))
 
    return () => {
      conn.off('ReceberMensagem')
      conn.off('UsuarioEntrou')
      conn.off('UsuarioSaiu')
      conn.off('SalaCriada')
      conn.off('ArquivoDisponível')
      conn.off('UsuárioDigitando')
    }
  }, [])
}