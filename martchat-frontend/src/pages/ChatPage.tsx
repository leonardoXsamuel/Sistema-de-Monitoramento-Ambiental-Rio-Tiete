import { useEffect } from 'react'
import { useSignalR } from '../hooks/useSignalR'
import { useChatStore } from '../store/ChatStore'
import api from '../services/api'
import Sidebar from '../components/layout/sideBar'
import ChatArea from '../components/chat/chatArea'
import styles from './ChatPage.module.css'

export default function ChatPage() {
  useSignalR()

  const { setRooms } = useChatStore()

  // Carrega salas ao entrar
  useEffect(() => {
    api.get('/api/ChatRoom/GetAllRooms?page=1&pageSize=50')
      .then(({ data }) => {
        // Backend retorna lista — mapeamos adicionando id helper
        const rooms = Array.isArray(data)
          ? data.map((r: any, i: number) => ({ ...r, id: r.id ?? i + 1 }))
          : []
        setRooms(rooms)
      })
      .catch(console.error)
  }, [])

  return (
    <div className={styles.layout}>
      <Sidebar />
      <ChatArea />
    </div>
  )
}