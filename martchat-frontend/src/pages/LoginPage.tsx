import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { Droplets, Lock, User, Leaf } from 'lucide-react'
import toast from 'react-hot-toast'
import api from '../services/Api'
import { useAuthStore } from '../store/AuthStore'
import type { AuthResponse } from '../types/Types'
import styles from './AuthPage.module.css'

export default function LoginPage() {
  const navigate  = useNavigate()
  const { login } = useAuthStore()
  const [form, setForm]       = useState({ username: '', password: '' })
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setLoading(true)
    try {
      const { data } = await api.post<AuthResponse>('/api/auth/login', form)
      login(data)
      navigate('/')
    } catch {
      toast.error('Credenciais inválidas. Verifique username e senha.')
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
        <div className={styles.particles}>
          {Array.from({ length: 12 }).map((_, i) => (
            <div key={i} className={styles.particle} style={{ '--i': i } as React.CSSProperties} />
          ))}
        </div>
      </div>

      <div className={styles.card}>
        <div className={styles.logo}>
          <div className={styles.logoIcon}>
            <Droplets size={28} />
          </div>
          <div>
            <h1 className={styles.logoTitle}>MartChat</h1>
            <p className={styles.logoSub}>Sistema de Monitoramento Ambiental</p>
          </div>
        </div>

        <div className={styles.badge}>
          <Leaf size={12} />
          <span>Rio Tietê — Monitoramento em Tempo Real</span>
        </div>

        <form onSubmit={handleSubmit} className={styles.form}>
          <div className={styles.field}>
            <User size={15} className={styles.fieldIcon} />
            <input
              type="text"
              placeholder="Username"
              value={form.username}
              onChange={(e) => setForm({ ...form, username: e.target.value })}
              className={styles.input}
              required
              autoFocus
            />
          </div>

          <div className={styles.field}>
            <Lock size={15} className={styles.fieldIcon} />
            <input
              type="password"
              placeholder="Senha"
              value={form.password}
              onChange={(e) => setForm({ ...form, password: e.target.value })}
              className={styles.input}
              required
            />
          </div>

          <button type="submit" className={styles.btn} disabled={loading}>
            {loading ? <span className={styles.spinner} /> : 'Entrar no Sistema'}
          </button>
        </form>

        <p className={styles.link}>
          Sem acesso?{' '}
          <Link to="/register">Solicitar cadastro</Link>
        </p>
      </div>
    </div>
  )
}