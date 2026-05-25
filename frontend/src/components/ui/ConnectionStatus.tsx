import * as signalR from '@microsoft/signalr';
import { useEffect, useState, type RefObject } from 'react';

interface Props {
  connectionRef: RefObject<signalR.HubConnection | null>;
}

export default function ConnectionStatus({ connectionRef }: Props) {
  const [state, setState] = useState<signalR.HubConnectionState>(
    signalR.HubConnectionState.Disconnected
  );

  useEffect(() => {
    const interval = setInterval(() => {
      const s = connectionRef.current?.state ?? signalR.HubConnectionState.Disconnected;
      setState(s);
    }, 1000);
    return () => clearInterval(interval);
  }, [connectionRef]);

  const isConnected = state === signalR.HubConnectionState.Connected;
  const isConnecting =
    state === signalR.HubConnectionState.Connecting ||
    state === signalR.HubConnectionState.Reconnecting;

  const color = isConnected ? 'var(--green-pale)' : isConnecting ? 'var(--warning)' : 'var(--error)';
  const label = isConnected ? 'Online' : isConnecting ? 'Conectando...' : 'Offline';

  return (
    <span
      style={{
        display: 'flex',
        alignItems: 'center',
        gap: '0.375rem',
        fontSize: '0.75rem',
        color,
      }}
    >
      <span
        style={{
          width: 8,
          height: 8,
          borderRadius: '50%',
          background: color,
          display: 'inline-block',
        }}
      />
      {label}
    </span>
  );
}
