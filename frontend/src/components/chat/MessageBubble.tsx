import type { Message } from '../../types/types';
import styles from './MessageBubble.module.css';

interface Props {
  message: Message;
  isMine: boolean;
}

function formatTime(iso: string) {
  return new Date(iso).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
}

export default function MessageBubble({ message, isMine }: Props) {
  const senderName = message.sender?.displayName ?? 'Usuário';

  return (
    <div className={`${styles.wrapper} ${isMine ? styles.mine : styles.theirs}`}>
      {!isMine && (
        <div className={styles.avatar}>
          {senderName[0]?.toUpperCase() ?? '?'}
        </div>
      )}
      <div className={styles.bubble}>
        {!isMine && (
          <span className={styles.senderName}>{senderName}</span>
        )}
        <p className={styles.content}>{message.content}</p>
        <span className={styles.time}>{formatTime(message.sentAt)}</span>
      </div>
    </div>
  );
}
