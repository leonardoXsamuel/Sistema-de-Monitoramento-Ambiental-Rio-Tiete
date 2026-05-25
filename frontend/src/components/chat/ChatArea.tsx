import { useEffect, useRef, useState, type KeyboardEvent } from 'react';
import EmojiPicker, { type EmojiClickData } from 'emoji-picker-react';
import { useChatStore } from '../../store/chatStore';
import { useAuthStore } from '../../store/authStore';
import { getMessages } from '../../api/messages';
import { getFilesByRoom } from '../../api/files';
import MessageBubble from './MessageBubble';
import FileUploadButton from './FileUploadButton';
import ConnectionStatus from '../ui/ConnectionStatus';
import Toast from '../ui/Toast';
import type { FileTransfer } from '../../types/types';
import type * as signalR from '@microsoft/signalr';
import styles from './ChatArea.module.css';

interface Props {
  sendMessage: (content: string, roomId: number) => void;
  sendTyping: (roomId: number, isTyping: boolean) => void;
  notifyFileUploaded: (file: FileTransfer, roomId: number) => void;
  connectionRef: React.RefObject<signalR.HubConnection | null>;
  onBack?: () => void;
}

export default function ChatArea({ sendMessage, sendTyping, notifyFileUploaded, connectionRef, onBack }: Props) {
  const { activeRoom, messages, files, typingUsers, setMessages, setFiles, addFile, addMessage } =
    useChatStore();
  const { user } = useAuthStore();
  const [text, setText] = useState('');
  const [showEmoji, setShowEmoji] = useState(false);
  const [showFiles, setShowFiles] = useState(false);
  const [toasts, setToasts] = useState<{ id: number; name: string }[]>([]);
  const bottomRef = useRef<HTMLDivElement>(null);
  const typingTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const toastIdRef = useRef(0);

  useEffect(() => {
    if (!activeRoom) return;
    setShowFiles(false);

    getMessages(activeRoom.id).then(setMessages).catch(console.error);
    getFilesByRoom(activeRoom.id).then(setFiles).catch(console.error);
  }, [activeRoom?.id]);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  const handleSend = () => {
    const content = text.trim();
    if (!content || !activeRoom || !user) return;

    addMessage({
      content,
      sentAt: new Date().toISOString(),
      sender: {
        username: user.username,
        displayName: user.displayName,
        role: user.role,
        createdAt: '',
      },
    });

    sendMessage(content, activeRoom.id);
    setText('');
    stopTyping();
  };

  const stopTyping = () => {
    if (typingTimeoutRef.current) clearTimeout(typingTimeoutRef.current);
    if (activeRoom) sendTyping(activeRoom.id, false);
  };

  const handleKeyDown = (e: KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  const handleTextChange = (v: string) => {
    setText(v);
    if (!activeRoom) return;
    sendTyping(activeRoom.id, true);
    if (typingTimeoutRef.current) clearTimeout(typingTimeoutRef.current);
    typingTimeoutRef.current = setTimeout(() => sendTyping(activeRoom.id, false), 2000);
  };

  const handleEmojiClick = (data: EmojiClickData) => {
    setText((t) => t + data.emoji);
    setShowEmoji(false);
  };

  const handleFilesUploaded = (uploaded: FileTransfer[]) => {
    uploaded.forEach((f) => {
      addFile(f);
      if (activeRoom) notifyFileUploaded(f, activeRoom.id);
      const id = ++toastIdRef.current;
      setToasts((prev) => [...prev, { id, name: f.nomeOriginal }]);
    });
  };

  const removeToast = (id: number) => setToasts((prev) => prev.filter((t) => t.id !== id));

  const formatBytes = (b: number) => {
    if (b < 1024) return `${b} B`;
    if (b < 1024 * 1024) return `${(b / 1024).toFixed(1)} KB`;
    return `${(b / 1024 / 1024).toFixed(1)} MB`;
  };

  if (!activeRoom) {
    return (
      <div className={styles.empty}>
        <p>Selecione uma sala para começar</p>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      {toasts.map((t) => (
        <Toast key={t.id} message={`${t.name} enviado`} onDone={() => removeToast(t.id)} />
      ))}

      <header className={styles.header}>
        <div className={styles.headerLeft}>
          {onBack && (
            <button className={styles.backBtn} onClick={onBack} title="Voltar">
              ‹
            </button>
          )}
          <span className={styles.roomHash}>#</span>
          <span className={styles.roomName}>{activeRoom.name}</span>
        </div>
        <div className={styles.headerRight}>
          <button
            className={`${styles.filesToggle} ${showFiles ? styles.active : ''}`}
            onClick={() => setShowFiles((v) => !v)}
            title="Arquivos da sala"
          >
            📁 {files.length}
          </button>
          <ConnectionStatus connectionRef={connectionRef} />
        </div>
      </header>

      <div className={styles.body}>
        <div className={`${styles.messages} ${showFiles ? styles.withPanel : ''}`}>
          {messages.length === 0 && (
            <p className={styles.noMessages}>Nenhuma mensagem ainda. Diga olá! 👋</p>
          )}
          {messages.map((msg, i) => (
            <MessageBubble
              key={i}
              message={msg}
              isMine={msg.sender?.username === user?.username}
            />
          ))}
          {typingUsers.length > 0 && (
            <p className={styles.typing}>
              {typingUsers[0]}{' '}
              {typingUsers.length > 1 ? `e mais ${typingUsers.length - 1}` : ''} digitando...
            </p>
          )}
          <div ref={bottomRef} />
        </div>

        {showFiles && (
          <aside className={styles.filePanel}>
            <h3 className={styles.filePanelTitle}>Arquivos</h3>
            {files.length === 0 && <p className={styles.noFiles}>Nenhum arquivo.</p>}
            <ul className={styles.fileList}>
              {files.map((f, i) => (
                <li key={i} className={styles.fileItem}>
                  <span className={styles.fileIcon}>
                    {f.tipoConteudo.includes('pdf')
                      ? '📄'
                      : f.tipoConteudo.includes('sheet')
                        ? '📊'
                        : '📝'}
                  </span>
                  <div className={styles.fileMeta}>
                    <a
                      href={f.downloadUrl}
                      target="_blank"
                      rel="noreferrer"
                      className={styles.fileName}
                    >
                      {f.nomeOriginal}
                    </a>
                    <span className={styles.fileSize}>{formatBytes(f.tamanhoBytes)}</span>
                  </div>
                </li>
              ))}
            </ul>
          </aside>
        )}
      </div>

      <footer className={styles.inputArea}>
        {showEmoji && (
          <div className={styles.emojiPickerWrapper}>
            <EmojiPicker onEmojiClick={handleEmojiClick} theme={'dark' as any} height={380} />
          </div>
        )}
        <div className={styles.inputRow}>
          <button
            className={styles.iconBtn}
            onClick={() => setShowEmoji((v) => !v)}
            title="Emojis"
          >
            😊
          </button>
          <FileUploadButton roomId={activeRoom.id} onUploaded={handleFilesUploaded} />
          <textarea
            className={styles.input}
            placeholder={`Mensagem em #${activeRoom.name}`}
            value={text}
            onChange={(e) => handleTextChange(e.target.value)}
            onKeyDown={handleKeyDown}
            rows={1}
          />
          <button
            className={styles.sendBtn}
            onClick={handleSend}
            disabled={!text.trim()}
            title="Enviar"
          >
            ➤
          </button>
        </div>
      </footer>
    </div>
  );
}
