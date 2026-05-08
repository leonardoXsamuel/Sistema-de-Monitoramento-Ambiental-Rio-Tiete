import { X, Download, FileText, FileSpreadsheet, File } from 'lucide-react'
import { useChatStore } from '../../store/ChatStore'
import styles from './FilePanel.module.css'

interface Props {
  roomId: number
  onClose: () => void
}

function fileIcon(tipo: string) {
  if (tipo.includes('pdf'))   return <FileText size={18} style={{ color: '#e74c3c' }} />
  if (tipo.includes('sheet')) return <FileSpreadsheet size={18} style={{ color: '#27ae60' }} />
  return <File size={18} style={{ color: '#2980b9' }} />
}

function formatSize(bytes: number) {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / 1024 / 1024).toFixed(1)} MB`
}

function formatDate(iso: string) {
  try { return new Date(iso).toLocaleDateString('pt-BR') } catch { return '' }
}

export default function FilePanel({ roomId, onClose }: Props) {
  const { files } = useChatStore()

  return (
    <div className={styles.panel}>
      <div className={styles.header}>
        <span>Arquivos da Sala</span>
        <button className={styles.close} onClick={onClose}><X size={16} /></button>
      </div>

      <div className={styles.list}>
        {files.length === 0 && (
          <p className={styles.empty}>Nenhum arquivo enviado ainda.</p>
        )}
        {files.map((f, i) => (
          <div key={i} className={styles.item}>
            <div className={styles.itemIcon}>{fileIcon(f.tipoConteudo)}</div>
            <div className={styles.itemInfo}>
              <span className={styles.itemName}>{f.nomeOriginal}</span>
              <span className={styles.itemMeta}>{formatSize(f.tamanhoBytes)} · {formatDate(f.uploadedAt)}</span>
            </div>
            <a
              href={`http://localhost:65148${f.downloadUrl}`}
              download={f.nomeOriginal}
              className={styles.downloadBtn}
              title="Baixar"
            >
              <Download size={14} />
            </a>
          </div>
        ))}
      </div>
    </div>
  )
}