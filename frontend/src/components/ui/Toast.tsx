import { useEffect, useState } from 'react';
import styles from './Toast.module.css';

interface Props {
  message: string;
  onDone: () => void;
}

export default function Toast({ message, onDone }: Props) {
  const [visible, setVisible] = useState(false);

  useEffect(() => {
    requestAnimationFrame(() => setVisible(true));
    const hide = setTimeout(() => setVisible(false), 2600);
    const done = setTimeout(onDone, 3000);
    return () => { clearTimeout(hide); clearTimeout(done); };
  }, []);

  return (
    <div className={`${styles.toast} ${visible ? styles.show : ''}`}>
      <span className={styles.icon}>📎</span>
      <span className={styles.text}>{message}</span>
    </div>
  );
}
