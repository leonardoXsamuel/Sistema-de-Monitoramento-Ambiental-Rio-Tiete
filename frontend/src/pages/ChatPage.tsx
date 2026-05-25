import { useEffect } from 'react';
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

  useEffect(() => {
    getAllRooms().then(setRooms).catch(console.error);
  }, []);

  const handleSelectRoom = (room: ChatRoom) => {
    if (activeRoom) leaveRoom(activeRoom.id);
    setActiveRoom(room);
    joinRoom(room.id);
  };

  return (
    <div className={styles.layout}>
      <Sidebar onSelectRoom={handleSelectRoom} />
      <main className={styles.main}>
        <ChatArea
          sendMessage={sendMessage}
          sendTyping={sendTyping}
          notifyFileUploaded={notifyFileUploaded}
          connectionRef={connection}
        />
      </main>
    </div>
  );
}
