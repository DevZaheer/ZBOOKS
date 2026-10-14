import { useEffect, useRef, useState } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/useAuth'
import { addToCart } from '../utils/cart'
import './AddToCartButton.css'

interface AddToCartButtonProps {
  bookId: number
  available: boolean
  className: string
  compact?: boolean
}

function AddToCartButton({ bookId, available, className, compact = false }: AddToCartButtonProps) {
  const { user } = useAuth()
  const location = useLocation()
  const navigate = useNavigate()
  const [message, setMessage] = useState('')
  const timeout = useRef<number | undefined>(undefined)

  useEffect(() => () => window.clearTimeout(timeout.current), [])

  function handleAdd() {
    if (!user?.id) {
      navigate('/login', { state: { from: `${location.pathname}${location.search}${location.hash}` } })
      return
    }

    const added = addToCart(bookId, user.id)
    setMessage(added ? 'Item added to cart' : 'Item is already in your cart')
    window.clearTimeout(timeout.current)
    timeout.current = window.setTimeout(() => setMessage(''), 2500)
  }

  return <span className="add-to-cart-wrap">
    <button className={className} disabled={!available} onClick={handleAdd}>
      {available ? compact ? 'Add to cart +' : 'Add to cart' : 'Unavailable'}
    </button>
    {message && <span className="add-to-cart-message" role="status">{message}</span>}
  </span>
}

export default AddToCartButton
