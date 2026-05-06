import { useEffect, useRef, useState } from 'react'
import { Send, Paperclip, Hash, Users, FileText } from 'lucide-react'
import { useChatStore } from '../../store/chatStore'
import { hubActions } from '../../services/signalR'
import { useAuthStore } from '../../store/authStore'
import MessageBubble from './MessageBubble'
import FilePanel from './FilePanel'
import styles from './ChatArea.module.css'

export default function ChatArea() {
  const { username } = useAuthStore()
  const { currentRoomId, rooms, messages, typingUsers } = useChatStore()
  const [text, setText]           = useState('')
  const [sending, setSending]     = useState(false)
  const [showFiles, setShowFiles] = useState(false)
  const [typingTimer, setTypingTimer] = useState<ReturnType<typeof setTimeout> | null>(null)
  const bottomRef = useRef<HTMLDivElement>(null)

  const currentRoom = rooms.find((r) => r.id === currentRoomId)

  // Scroll para última mensagem
  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [messages])

  async function sendMessage(e: React.FormEvent) {
    e.preventDefault()
    if (!text.trim() || !currentRoomId || sending) return
    setSending(true)
    try {
      await hubActions.enviarMensagem(text.trim(), currentRoomId)
      setText('')
    } finally {
      setSending(false)
    }
  }

  function handleTyping(e: React.ChangeEvent<HTMLInputElement>) {
    setText(e.target.value)
    if (!currentRoomId) return

    hubActions.digitando(currentRoomId, true).catch(() => {})

    if (typingTimer) clearTimeout(typingTimer)
    const t = setTimeout(() => {
      hubActions.digitando(currentRoomId, false).catch(() => {})
    }, 1500)
    setTypingTimer(t)
  }

  if (!currentRoomId) {
    return (
      <div className={styles.empty}>
        <div className={styles.emptyContent}>
          <div className={styles.emptyIcon}>
            <Hash size={40} />
          </div>
          <h2>Selecione uma sala</h2>
          <p>Escolha uma sala de inspeção na sidebar para começar a monitorar</p>
          <div className={styles.emptyDecorations}>
            <div className={styles.wave} />
            <div className={styles.wave} />
            <div className={styles.wave} />
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className={styles.area}>
      {/* Header */}
      <div className={styles.header}>
        <div className={styles.headerLeft}>
          <Hash size={18} className={styles.headerHash} />
          <div>
            <h2 className={styles.headerTitle}>{currentRoom?.name ?? 'Sala'}</h2>
            <p className={styles.headerSub}>Inspeção ambiental em tempo real</p>
          </div>
        </div>
        <div className={styles.headerRight}>
          <button
            className={`${styles.iconBtn} ${showFiles ? styles.iconBtnActive : ''}`}
            onClick={() => setShowFiles(!showFiles)}
            title="Arquivos da sala"
          >
            <FileText size={16} />
            <span>Arquivos</span>
          </button>
        </div>
      </div>

      <div className={styles.body}>
        {/* Messages */}
        <div className={styles.messages}>
          {messages.length === 0 && (
            <div className={styles.noMessages}>
              <p>Nenhuma mensagem ainda. Seja o primeiro a reportar!</p>
            </div>
          )}
          {messages.map((msg, i) => (
            <MessageBubble
              key={i}
              message={msg}
              isOwn={msg.sender?.username === username}
            />
          ))}
          {typingUsers.length > 0 && (
            <div className={styles.typing}>
              <span className={styles.typingDots}>
                <span /><span /><span />
              </span>
              <span>{typingUsers.join(', ')} está digitando...</span>
            </div>
          )}
          <div ref={bottomRef} />
        </div>

        {/* File panel */}
        {showFiles && (
          <FilePanel roomId={currentRoomId} onClose={() => setShowFiles(false)} />
        )}
      </div>

      {/* Input */}
      <form onSubmit={sendMessage} className={styles.inputBar}>
        <label className={styles.attachBtn} title="Enviar arquivo">
          <Paperclip size={18} />
          <input
            type="file"
            accept=".pdf,.docx,.xlsx"
            multiple
            style={{ display: 'none' }}
            onChange={async (e) => {
              if (!e.target.files?.length || !currentRoomId) return
              const form = new FormData()
              Array.from(e.target.files).forEach((f) => form.append('files', f))
              form.append('roomId', String(currentRoomId))
              try {
                const { default: api } = await import('../../services/api')
                const { data } = await api.post('/api/files/upload', form, {
                  headers: { 'Content-Type': 'multipart/form-data' },
                })
                const files = Array.isArray(data) ? data : [data]
                const { useChatStore } = await import('../../store/chatStore')
                const { addFile } = useChatStore.getState()
                files.forEach((f: any) => addFile(f))
                // Notifica via SignalR
                for (const f of files) {
                  await hubActions.notificarArquivo(f, currentRoomId)
                }
                const toast = (await import('react-hot-toast')).default
                toast.success(`${files.length} arquivo(s) enviado(s)!`)
              } catch {
                const toast = (await import('react-hot-toast')).default
                toast.error('Erro ao enviar arquivo.')
              }
              e.target.value = ''
            }}
          />
        </label>

        <input
          type="text"
          placeholder={`Reportar em #${currentRoom?.name ?? 'sala'}...`}
          value={text}
          onChange={handleTyping}
          className={styles.textInput}
          disabled={sending}
        />

        <button type="submit" className={styles.sendBtn} disabled={!text.trim() || sending}>
          <Send size={16} />
        </button>
      </form>
    </div>
  )
}