import { useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { useAuthStore } from '../store/authStore';
import { useChatStore } from '../store/chatStore';
import { BASE_URL } from '../api/http';
import type { Message, FileTransfer, ChatRoom } from '../types/types';

export const useSignalR = () => {
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const readyRef = useRef<Promise<void>>(new Promise(() => {}));
  const token = useAuthStore((s) => s.token);

  useEffect(() => {
    if (!token) return;

    let resolveReady: () => void;
    readyRef.current = new Promise<void>((r) => {
      resolveReady = r;
    });

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${BASE_URL}/hubs/chat`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    connection.onreconnected(() => {
      const room = useChatStore.getState().activeRoom;
      if (room) {
        connection.invoke('EntrarNoChatRoom', room.id).catch(console.error);
      }
    });

    connection.on('ReceberMensagem', (msg: Message) => {
      useChatStore.getState().addMessage(msg);
    });

    connection.on('SalaCriada', (room: ChatRoom) => {
      useChatStore.getState().addRoom(room);
    });

    connection.on('ArquivoDisponivel', (file: FileTransfer) => {
      useChatStore.getState().addFile(file);
    });

    connection.on('UsuarioDigitando', (label: string, isTyping: boolean) => {
      useChatStore.getState().setTyping(label, isTyping);
    });

    connectionRef.current = connection;

    connection
      .start()
      .then(() => resolveReady!())
      .catch(console.error);

    return () => {
      connection.stop();
      connectionRef.current = null;
    };
  }, [token]);

  const ensureReady = () => readyRef.current;

  const joinRoom = async (roomId: number) => {
    try {
      await ensureReady();
      await connectionRef.current!.invoke('EntrarNoChatRoom', roomId);
    } catch (e) {
      console.error('joinRoom:', e);
    }
  };

  const leaveRoom = async (roomId: number) => {
    try {
      await connectionRef.current?.invoke('SairDoChatRoom', roomId);
    } catch {}
  };

  const sendMessage = async (content: string, roomId: number) => {
    try {
      await ensureReady();
      await connectionRef.current!.invoke('EnviarMensagem', content, roomId);
    } catch (e) {
      console.error('sendMessage:', e);
    }
  };

  const notifyFileUploaded = async (file: FileTransfer, roomId: number) => {
    try {
      await ensureReady();
      await connectionRef.current!.invoke('NotificaçãoDeArquivoCarregado', file, roomId);
    } catch (e) {
      console.error('notifyFile:', e);
    }
  };

  const sendTyping = (roomId: number, isTyping: boolean) => {
    connectionRef.current?.invoke('Digitando', roomId, isTyping).catch(() => {});
  };

  return {
    connection: connectionRef,
    joinRoom,
    leaveRoom,
    sendMessage,
    notifyFileUploaded,
    sendTyping,
  };
};
