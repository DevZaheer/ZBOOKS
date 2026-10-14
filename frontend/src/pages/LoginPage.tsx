import { useState, type FormEvent } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { ApiError } from '../api/client'
import PasswordInput from '../components/PasswordInput'
import { useAuth } from '../context/useAuth'
import './AuthPage.css'

function LoginPage() {
  const { login, token } = useAuth()
  const location = useLocation()
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)
  const destination = (location.state as { from?: string } | null)?.from ?? '/library'

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim())) { setError('Enter a valid email address.'); return }
    if (!password) { setError('Enter your password.'); return }
    setBusy(true)
    setError('')
    try {
      await login({ email: email.trim(), password })
      navigate(destination, { replace: true })
    } catch (cause) {
      setError(cause instanceof ApiError && cause.status === 401 ? 'That email and password do not match.' : cause instanceof Error ? cause.message : 'Sign in could not be completed.')
    } finally {
      setBusy(false)
    }
  }

  if (token) return <section className="auth-page"><div className="auth-panel"><p className="eyebrow">WELCOME BACK</p><h1>You’re already signed in.</h1><Link className="button" to="/library">Open your library</Link></div></section>
  return <section className="auth-page"><div className="auth-aside"><span className="auth-monogram">Z</span><p>Every shelf holds a new direction.</p></div><div className="auth-panel"><p className="eyebrow">YOUR READING ROOM</p><h1>Welcome back</h1><p className="auth-subtitle">Sign in to return to your books.</p><form onSubmit={(event) => void submit(event)} noValidate><label className="field-label">Email<input type="email" required autoComplete="email" value={email} onChange={(event) => setEmail(event.target.value)} /></label><PasswordInput id="login-password" label="Password" autoComplete="current-password" value={password} onChange={setPassword} />{error && <p className="form-error" role="alert">{error}</p>}<button className="button button-wide" disabled={busy}>{busy ? 'Signing in…' : 'Sign in'}</button></form><p className="auth-switch">New to ZBOOKS? <Link to="/register">Create an account</Link></p></div></section>
}

export default LoginPage