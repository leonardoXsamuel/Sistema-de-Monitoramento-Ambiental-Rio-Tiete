import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { Droplets, Lock, User, Leaf, BadgeCheck } from 'lucide-react'
import toast from 'react-hot-toast'
import api from '../services/api'
import { useAuthStore } from '../store/authStore'
import type { AuthResponse } from '../types/types'
import styles from './AuthPage.module.css'

export default function RegisterPage() {
  const navigate  = useNavigate()
  const { login } = useAuthStore()
  const [form, setForm]       = useState({ username: '', password: '', displayName: '', role: 'Inspetor' })
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setLoading(true)
    try {
      const { data } = await api.post<AuthResponse>('/api/auth/register', form)
      login(data)
      navigate('/')
    } catch (err: any) {
      toast.error(err?.response?.data?.message ?? 'Erro ao registrar usuário.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className={styles.wrapper}>
      <div className={styles.bg}>
        <div className={styles.wave1} />
        <div className={styles.wave2} />
        <div className={styles.wave3} />
      </div>

      <div className={styles.card}>
        <div className={styles.logo}>
          <div className={styles.logoIcon}>
            <Droplets size={28} />
          </div>
          <div>
            <h1 className={styles.logoTitle}>MartChat</h1>
            <p className={styles.logoSub}>Novo cadastro de inspetor</p>
          </div>
        </div>

        <div className={styles.badge}>
          <Leaf size={12} />
          <span>Equipe de Inspeção Ambiental</span>
        </div>

        <form onSubmit={handleSubmit} className={styles.form}>
          <div className={styles.field}>
            <User size={15} className={styles.fieldIcon} />
            <input
              type="text"
              placeholder="Username (sem espaços)"
              value={form.username}
              onChange={(e) => setForm({ ...form, username: e.target.value })}
              className={styles.input}
              required autoFocus
            />
          </div>

          <div className={styles.field}>
            <BadgeCheck size={15} className={styles.fieldIcon} />
            <input
              type="text"
              placeholder="Nome completo"
              value={form.displayName}
              onChange={(e) => setForm({ ...form, displayName: e.target.value })}
              className={styles.input}
              required
            />
          </div>

          <div className={styles.field}>
            <Lock size={15} className={styles.fieldIcon} />
            <input
              type="password"
              placeholder="Senha (mín. 6 caracteres)"
              value={form.password}
              onChange={(e) => setForm({ ...form, password: e.target.value })}
              className={styles.input}
              required
            />
          </div>

          <div className={styles.field}>
            <Leaf size={15} className={styles.fieldIcon} />
            <select
              value={form.role}
              onChange={(e) => setForm({ ...form, role: e.target.value })}
              className={styles.input}
            >
              <option value="Inspetor">Inspetor Ambiental</option>
              <option value="Coordenador">Coordenador</option>
            </select>
          </div>

          <button type="submit" className={styles.btn} disabled={loading}>
            {loading ? <span className={styles.spinner} /> : 'Criar Cadastro'}
          </button>
        </form>

        <p className={styles.link}>
          Já tem acesso? <Link to="/login">Entrar</Link>
        </p>
      </div>
    </div>
  )
}