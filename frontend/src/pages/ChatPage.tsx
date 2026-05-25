import { useEffect, useState } from 'react';
import { getAllRooms } from '../api/rooms';
import { useChatStore } from '../store/chatStore';
import { useSignalR } from '../hooks/useSignalR';
import Sidebar from '../components/layout/Sidebar';
import ChatArea from '../components/chat/ChatArea';
import type { ChatRoom } from '../types/types';
import styles from './ChatPage.module.css';

export default function ChatPage() {
  const { setRooms, setActiveRoom, activeRoom } = useChatStore();
  const { joinRoom, leaveRoom, sendMessage, sendTyping, notifyFileUploaded, connection } = useSignalR();
  const [mobileView, setMobileView] = useState<'sidebar' | 'chat'>('sidebar');

  useEffect(() => {
    getAllRooms().then(setRooms).catch(console.error);
  }, []);

  const handleSelectRoom = (room: ChatRoom) => {
    if (activeRoom) leaveRoom(activeRoom.id);
    setActiveRoom(room);
    joinRoom(room.id);
    setMobileView('chat');
  };

  return (
    <div className={styles.layout}>
      <div className={`${styles.sidebarWrapper} ${mobileView === 'chat' ? styles.hideMobile : ''}`}>
        <Sidebar onSelectRoom={handleSelectRoom} />
      </div>
      <main className={`${styles.main} ${mobileView === 'sidebar' ? styles.hideMobile : ''}`}>
        <ChatArea
          sendMessage={sendMessage}
          sendTyping={sendTyping}
          notifyFileUploaded={notifyFileUploaded}
          connectionRef={connection}
          onBack={() => setMobileView('sidebar')}
        />
      </main>
    </div>
  );
}
