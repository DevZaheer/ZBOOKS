import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import PasswordInput from '../components/PasswordInput'
import { useAuth } from '../context/useAuth'
import './AuthPage.css'

function RegisterPage() {
  const { register, token } = useAuth()
  const navigate = useNavigate()
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError('')
    if (name.trim().length < 2) { setError('Enter a name with at least 2 characters.'); return }
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim())) { setError('Enter a valid email address.'); return }
    if (password.length < 8) { setError('Use a password with at least 8 characters.'); return }
    if (password !== confirmPassword) { setError('The passwords do not match.'); return }
    setBusy(true)
    try {
      await register({ username: name.trim(), email: email.trim(), password })
      navigate('/library', { replace: true })
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Your account could not be created.')
    } finally {
      setBusy(false)
    }
  }

  if (token) return <section className="auth-page"><div className="auth-panel"><p className="eyebrow">YOU’RE IN</p><h1>Your next read is close.</h1><Link className="button" to="/books">Browse books</Link></div></section>
  return <section className="auth-page"><div className="auth-aside auth-aside-register"><span className="auth-monogram">Z</span><p>Make room for a little wonder.</p></div><div className="auth-panel"><p className="eyebrow">JOIN THE READING ROOM</p><h1>Create an account</h1><p className="auth-subtitle">Build a shelf that feels like yours.</p><form onSubmit={(event) => void submit(event)} noValidate><label className="field-label">Full name<input required autoComplete="name" value={name} onChange={(event) => setName(event.target.value)} /></label><label className="field-label">Email<input type="email" required autoComplete="email" value={email} onChange={(event) => setEmail(event.target.value)} /></label><PasswordInput id="register-password" label="Password" autoComplete="new-password" minLength={8} value={password} onChange={setPassword} hint="At least 8 characters" /><PasswordInput id="register-confirm-password" label="Confirm password" autoComplete="new-password" value={confirmPassword} onChange={setConfirmPassword} />{error && <p className="form-error" role="alert">{error}</p>}<button className="button button-wide" disabled={busy}>{busy ? 'Creating account…' : 'Create account'}</button></form><p className="auth-switch">Already have an account? <Link to="/login">Sign in</Link></p></div></section>
}

export default RegisterPage