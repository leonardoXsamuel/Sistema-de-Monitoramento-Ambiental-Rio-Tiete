import { useState } from 'react';
import { useChatStore } from '../../store/chatStore';
import { useAuthStore } from '../../store/authStore';
import { createRoom } from '../../api/rooms';
import type { ChatRoom } from '../../types/types';
import styles from './Sidebar.module.css';

interface Props {
  onSelectRoom: (room: ChatRoom) => void;
}

export default function Sidebar({ onSelectRoom }: Props) {
  const { rooms, activeRoom, addRoom } = useChatStore();
  const { user, logout } = useAuthStore();
  const [newRoomName, setNewRoomName] = useState('');
  const [creating, setCreating] = useState(false);
  const [showInput, setShowInput] = useState(false);
  const [error, setError] = useState('');

  const handleCreate = async () => {
    const name = newRoomName.trim();
    if (!name) return;
    setCreating(true);
    setError('');
    try {
      const room = await createRoom(name);
      addRoom(room);
      setNewRoomName('');
      setShowInput(false);
      onSelectRoom(room);
    } catch {
      setError('Erro ao criar sala.');
    } finally {
      setCreating(false);
    }
  };

  return (
    <aside className={styles.sidebar}>
      <div className={styles.header}>
        <div className={styles.appName}>
          <span>MartChat</span>
        </div>
        <p className={styles.tagline}>Monitoramento Ambiental</p>
      </div>

      <div className={styles.userCard}>
        <div className={styles.avatar}>{user?.displayName?.[0]?.toUpperCase() ?? '?'}</div>
        <div className={styles.userInfo}>
          <span className={styles.userName}>{user?.displayName}</span>
          <span className={styles.userRole}>{user?.role}</span>
        </div>
        <button className={styles.logoutBtn} onClick={logout} title="Sair">
          ⏻
        </button>
      </div>

      <div className={styles.roomsHeader}>
        <span>Salas</span>
        <button className={styles.newRoomBtn} onClick={() => setShowInput((v) => !v)} title="Nova sala">
          +
        </button>
      </div>

      {showInput && (
        <div className={styles.newRoomInput}>
          <input
            type="text"
            placeholder="Nome da sala..."
            value={newRoomName}
            onChange={(e) => setNewRoomName(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleCreate()}
            autoFocus
          />
          <button onClick={handleCreate} disabled={creating}>
            {creating ? '...' : '✓'}
          </button>
          {error && <span className={styles.inputError}>{error}</span>}
        </div>
      )}

      <nav className={styles.roomList}>
        {rooms.length === 0 && (
          <p className={styles.emptyRooms}>Nenhuma sala disponível.</p>
        )}
        {rooms.map((room) => (
          <button
            key={room.id}
            className={`${styles.roomItem} ${activeRoom?.id === room.id ? styles.active : ''}`}
            onClick={() => onSelectRoom(room)}
          >
            <span className={styles.roomIcon}>#</span>
            <span className={styles.roomName}>{room.name}</span>
          </button>
        ))}
      </nav>
    </aside>
  );
}
