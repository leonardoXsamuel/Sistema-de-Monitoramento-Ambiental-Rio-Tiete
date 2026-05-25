import { useState, type FormEvent } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { register } from '../api/auth';
import { useAuthStore } from '../store/authStore';
import type { UserRole } from '../types/types';
import styles from './LoginPage.module.css';

export default function RegisterPage() {
  const navigate = useNavigate();
  const setAuth = useAuthStore((s) => s.setAuth);

  const [username, setUsername] = useState('');
  const [displayName, setDisplayName] = useState('');
  const [password, setPassword] = useState('');
  const [role, setRole] = useState<UserRole>('Inspetor');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const data = await register({ username, password, displayName, role });
      setAuth({ username: data.username, displayName: data.displayName, role: data.role }, data.token);
      navigate('/chat');
    } catch (err: any) {
      setError(err.response?.data?.message ?? 'Erro ao cadastrar. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.card}>
        <div className={styles.logo}>
          <h1 className={styles.title}>MartChat</h1>
          <p className={styles.subtitle}>Criar nova conta</p>
        </div>

        <form onSubmit={handleSubmit} className={styles.form}>
          <div className={styles.field}>
            <label htmlFor="username">Usuário</label>
            <input
              id="username"
              type="text"
              placeholder="Nome de usuário (login)"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
            />
          </div>

          <div className={styles.field}>
            <label htmlFor="displayName">Nome de exibição</label>
            <input
              id="displayName"
              type="text"
              placeholder="Como quer ser chamado(a)"
              value={displayName}
              onChange={(e) => setDisplayName(e.target.value)}
              required
            />
          </div>

          <div className={styles.field}>
            <label htmlFor="password">Senha</label>
            <input
              id="password"
              type="password"
              placeholder="Crie uma senha"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>

          <div className={styles.field}>
            <label htmlFor="role">Função</label>
            <select
              id="role"
              value={role}
              onChange={(e) => setRole(e.target.value as UserRole)}
              style={{
                background: 'var(--bg-elevated)',
                border: '1px solid var(--border)',
                borderRadius: 'var(--radius)',
                padding: '0.625rem 0.875rem',
                color: 'var(--text-primary)',
              }}
            >
              <option value="Inspetor">Inspetor</option>
              <option value="Coordenador">Coordenador</option>
            </select>
          </div>

          {error && <p className={styles.error}>{error}</p>}

          <button type="submit" className={styles.btn} disabled={loading}>
            {loading ? 'Cadastrando...' : 'Criar conta'}
          </button>
        </form>

        <p className={styles.footer}>
          Já tem conta? <Link to="/login">Entrar</Link>
        </p>
      </div>
    </div>
  );
}
