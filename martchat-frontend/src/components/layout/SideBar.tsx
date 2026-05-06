import { useState } from 'react'
import { Droplets, Plus, LogOut, Wifi, WifiOff, ChevronRight, Leaf, Hash } from 'lucide-react'
import toast from 'react-hot-toast'
import { useAuthStore } from '../../store/authStore'
import { useChatStore } from '../../store/chatStore'
import { hubActions, stopConnection } from '../../services/signalR'
import api from '../../services/api'
import { useNavigate } from 'react-router-dom'
import styles from './Sidebar.module.css'

export default function Sidebar() {
  const navigate = useNavigate()
  const { displayName, role, logout } = useAuthStore()
  const { rooms, currentRoomId, setCurrentRoom, setMessages, setFiles, connected, addRoom } = useChatStore()
  const [creating, setCreating] = useState(false)
  const [newRoomName, setNewRoomName] = useState('')

  async function enterRoom(roomId: number) {
    if (currentRoomId === roomId) return

    // Sai da sala atual
    if (currentRoomId !== null) {
      try { await hubActions.sairDoChatRoom(currentRoomId) } catch {}
    }

    setCurrentRoom(roomId)

    // Entra na nova sala via SignalR
    try { await hubActions.entrarNoChatRoom(roomId) } catch {}

    // Carrega histórico de mensagens
    try {
      const { data } = await api.get(`/api/rooms/${roomId}/messages`)
      setMessages(Array.isArray(data) ? data : [])
    } catch { setMessages([]) }

    // Carrega arquivos da sala
    try {
      const { data } = await api.get(`/api/files/room/${roomId}`)
      setFiles(Array.isArray(data) ? data : [])
    } catch { setFiles([]) }
  }

  async function createRoom() {
    if (!newRoomName.trim()) return
    try {
      const { data } = await api.post('/api/ChatRoom/createChatRoom', { name: newRoomName.trim() })
      addRoom({ ...data, id: data.id ?? Date.now() })
      toast.success(`Sala "${newRoomName}" criada!`)
      setNewRoomName('')
      setCreating(false)
    } catch {
      toast.error('Erro ao criar sala.')
    }
  }

  async function handleLogout() {
    await stopConnection()
    logout()
    navigate('/login')
  }

  return (
    <aside className={styles.sidebar}>
      {/* Header */}
      <div className={styles.header}>
        <div className={styles.brand}>
          <div className={styles.brandIcon}>
            <Droplets size={20} />
          </div>
          <div>
            <span className={styles.brandName}>MartChat</span>
            <div className={styles.statusRow}>
              {connected
                ? <><Wifi size={10} className={styles.online} /><span className={styles.statusText}>Online</span></>
                : <><WifiOff size={10} className={styles.offline} /><span className={styles.statusText}>Reconectando...</span></>
              }
            </div>
          </div>
        </div>
      </div>

      {/* Contexto ambiental */}
      <div className={styles.envBadge}>
        <Leaf size={11} />
        <span>Monitoramento Rio Tietê</span>
      </div>

      {/* Salas */}
      <div className={styles.section}>
        <div className={styles.sectionHeader}>
          <span>SALAS DE INSPEÇÃO</span>
          {role === 'Coordenador' && (
            <button className={styles.addBtn} onClick={() => setCreating(!creating)} title="Nova sala">
              <Plus size={14} />
            </button>
          )}
        </div>

        {creating && (
          <div className={styles.createRoom}>
            <input
              type="text"
              placeholder="Nome da sala..."
              value={newRoomName}
              onChange={(e) => setNewRoomName(e.target.value)}
              className={styles.createInput}
              onKeyDown={(e) => e.key === 'Enter' && createRoom()}
              autoFocus
            />
            <button className={styles.createBtn} onClick={createRoom}>Criar</button>
          </div>
        )}

        <div className={styles.rooms}>
          {rooms.length === 0 && (
            <p className={styles.emptyRooms}>Nenhuma sala disponível</p>
          )}
          {rooms.map((room) => (
            <button
              key={room.id}
              className={`${styles.room} ${currentRoomId === room.id ? styles.roomActive : ''}`}
              onClick={() => enterRoom(room.id)}
            >
              <Hash size={14} className={styles.roomHash} />
              <span className={styles.roomName}>{room.name}</span>
              {currentRoomId === room.id && <ChevronRight size={14} className={styles.roomArrow} />}
            </button>
          ))}
        </div>
      </div>

      {/* Perfil */}
      <div className={styles.profile}>
        <div className={styles.avatar}>
          {displayName?.[0]?.toUpperCase() ?? 'U'}
        </div>
        <div className={styles.profileInfo}>
          <span className={styles.profileName}>{displayName}</span>
          <span className={styles.profileRole}>{role === 'Coordenador' ? 'Coordenador' : 'Inspetor'}</span>
        </div>
        <button className={styles.logoutBtn} onClick={handleLogout} title="Sair">
          <LogOut size={16} />
        </button>
      </div>
    </aside>
  )
}