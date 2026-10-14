import { useState } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { accountApi } from '../api/resources'
import { useAuth } from '../context/useAuth'
import './SaveBookButton.css'

function SaveBookButton({ bookId }: { bookId: number }) {
  const { token } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const [saved, setSaved] = useState(false)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')

  async function save() {
    if (!token) {
      navigate('/login', { state: { from: location.pathname } })
      return
    }
    setBusy(true)
    setError('')
    try {
      await accountApi.createWishlist({ bookId })
      setSaved(true)
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Could not save this book.')
    } finally {
      setBusy(false)
    }
  }

  return <span className="save-book-wrap"><button className="save-book" type="button" disabled={saved || busy} onClick={() => void save()} aria-label={saved ? 'Saved to wishlist' : 'Save to wishlist'} title={saved ? 'Saved to wishlist' : 'Save to wishlist'}>{saved ? '♥' : '♡'}</button>{error && <span className="save-book-error" role="alert">{error}</span>}</span>
}

export default SaveBookButton