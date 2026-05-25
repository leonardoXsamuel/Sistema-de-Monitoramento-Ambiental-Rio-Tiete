import { useRef } from 'react';
import { uploadFiles } from '../../api/files';
import type { FileTransfer } from '../../types/types';

interface Props {
  roomId: number;
  onUploaded: (files: FileTransfer[]) => void;
}

const ACCEPTED = '.pdf,.docx,.xlsx';

export default function FileUploadButton({ roomId, onUploaded }: Props) {
  const inputRef = useRef<HTMLInputElement>(null);

  const handleChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const selected = Array.from(e.target.files ?? []);
    if (selected.length === 0) return;
    try {
      const result = await uploadFiles(selected, roomId);
      onUploaded(result);
    } catch (err: any) {
      alert(err.response?.data?.message ?? 'Erro ao enviar arquivo.');
    }
    if (inputRef.current) inputRef.current.value = '';
  };

  return (
    <>
      <input
        ref={inputRef}
        type="file"
        accept={ACCEPTED}
        multiple
        style={{ display: 'none' }}
        onChange={handleChange}
      />
      <button
        type="button"
        title="Enviar arquivo (PDF, DOCX, XLSX)"
        onClick={() => inputRef.current?.click()}
        style={{
          color: 'var(--text-muted)',
          fontSize: '1.1rem',
          padding: '0.375rem',
          borderRadius: 'var(--radius)',
          transition: 'color 0.15s',
        }}
        onMouseEnter={(e) => (e.currentTarget.style.color = 'var(--green-pale)')}
        onMouseLeave={(e) => (e.currentTarget.style.color = 'var(--text-muted)')}
      >
        📎
      </button>
    </>
  );
}
