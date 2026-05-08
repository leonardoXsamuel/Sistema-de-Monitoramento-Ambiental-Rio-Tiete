import { useEffect, useRef } from "react";
import { HubConnectionState } from "@microsoft/signalr";
import toast from "react-hot-toast";
import { getConnection, startConnection } from "../services/SignalR";
import { useChatStore } from "../store/ChatStore";
import { useAuthStore } from "../store/AuthStore";
import type {
  MessageResponseDTO,
  FileTransferResponseDTO,
  RoomWithId,
} from "../types/Types";

export function useSignalR() {
  const { username } = useAuthStore();
  const { addMessage, addFile, setTypingUser, addRoom, setConnected } =
    useChatStore();

  useEffect(() => {
    const conn = getConnection();
    async function init() {
      try {
        if (conn.state !== HubConnectionState.Connected) {
          await startConnection();
        }

        setConnected(true);

        // limpa listeners antigos
        conn.off("ReceberMensagem");
        conn.off("UsuarioEntrou");
        conn.off("UsuarioSaiu");
        conn.off("SalaCriada");
        conn.off("ArquivoDisponível");
        conn.off("UsuárioDigitando");

        // Eventos
        conn.on("ReceberMensagem", (msg: MessageResponseDTO) => {
          addMessage(msg);
        });

        conn.on("UsuarioEntrou", (info: string) => {
          toast(info);
        });

        conn.on("UsuarioSaiu", (info: string) => {
          toast(info);
        });

        conn.on("SalaCriada", (room: RoomWithId) => {
          addRoom(room);
        });

        conn.on("ArquivoDisponível", (file: FileTransferResponseDTO) => {
          addFile(file);
        });

        conn.on("UsuárioDigitando", (info: string, isTyping: boolean) => {
          const username = info.replace("Usuario", "");
          setTypingUser(username, isTyping);
        });
      } catch {
        setConnected(false);
      }
    }

    init();

    conn.onreconnected(() => setConnected(true));
    conn.onreconnecting(() => setConnected(false));
    conn.onclose(() => setConnected(false));

    return () => {
      conn.off("ReceberMensagem");
      conn.off("UsuarioEntrou");
      conn.off("UsuarioSaiu");
      conn.off("SalaCriada");
      conn.off("ArquivoDisponível");
      conn.off("UsuárioDigitando");
    };
  }, []);
}
