import type { MessageResponseDTO } from '../../types/types'
import styles from './MessageBubble.module.css'

interface Props {
  message: MessageResponseDTO
  isOwn: boolean
}

function formatTime(iso: string) {
  try {
    return new Date(iso).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })
  } catch { return '' }
}

export default function MessageBubble({ message, isOwn }: Props) {
  return (
    <div className={`${styles.wrapper} ${isOwn ? styles.own : styles.other}`}>
      {!isOwn && (
        <div className={styles.avatar}>
          {message.sender?.displayName?.[0]?.toUpperCase() ?? 'U'}
        </div>
      )}
      <div className={styles.content}>
        {!isOwn && (
          <div className={styles.header}>
            <span className={styles.name}>{message.sender?.displayName ?? message.sender?.username}</span>
            <span className={styles.role}>{message.sender?.role}</span>
          </div>
        )}
        <div className={`${styles.bubble} ${isOwn ? styles.bubbleOwn : styles.bubbleOther}`}>
          <p className={styles.text}>{message.content}</p>
          <span className={styles.time}>{formatTime(message.sentAt)}</span>
        </div>
      </div>
    </div>
  )
}