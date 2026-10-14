import { useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { accountApi, catalogApi } from '../api/resources'
import { useAuth } from '../context/useAuth'
import type { BookDto } from '../types'
import { getCart, saveCart } from '../utils/cart'
import { EmptyState, ErrorMessage, LoadingSpinner } from '../components/Feedback'
import './CartPage.css'

function CartPage() {
  const navigate = useNavigate()
  const { user } = useAuth()
  const [books, setBooks] = useState<BookDto[]>([])
  const [selectedIds, setSelectedIds] = useState<number[]>([])
  const [loading, setLoading] = useState(true)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  const [reloadKey, setReloadKey] = useState(0)

  useEffect(() => {
    let active = true
    async function loadCart() {
      setLoading(true)
      setError('')
      setBooks([])
      try {
        const ids = getCart(user?.id || undefined)
        const items = await Promise.all(ids.map((id) => catalogApi.book(id)))
        if (active) {
          setBooks(items)
          setSelectedIds(items.filter((book) => book.isAvailable).map((book) => book.bookId))
        }
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : 'Your cart could not be loaded.')
      } finally {
        if (active) setLoading(false)
      }
    }

    const timer = window.setTimeout(() => { void loadCart() }, 0)
    return () => {
      active = false
      window.clearTimeout(timer)
    }
  }, [reloadKey, user?.id])

  function removeBook(bookId: number) {
    saveCart(getCart(user?.id || undefined).filter((id) => id !== bookId), user?.id || undefined)
    setBooks((items) => items.filter((item) => item.bookId !== bookId))
    setSelectedIds((ids) => ids.filter((id) => id !== bookId))
  }

  function toggleBook(bookId: number) {
    setSelectedIds((ids) => ids.includes(bookId) ? ids.filter((id) => id !== bookId) : [...ids, bookId])
  }

  const availableBooks = books.filter((book) => book.isAvailable)
  const selectedBooks = availableBooks.filter((book) => selectedIds.includes(book.bookId))
  const allAvailableSelected = availableBooks.length > 0 && selectedBooks.length === availableBooks.length

  function toggleAll() {
    setSelectedIds(allAvailableSelected ? [] : availableBooks.map((book) => book.bookId))
  }

  async function checkout() {
    setBusy(true)
    setError('')
    try {
      const purchases = await accountApi.createPurchases({ bookIds: selectedBooks.map((book) => book.bookId) })
      const checkedOutIds = purchases.map((purchase) => purchase.bookId)
      saveCart(getCart(user?.id || undefined).filter((id) => !checkedOutIds.includes(id)), user?.id || undefined)
      navigate(`/payment/batch/${purchases.map((purchase) => purchase.purchaseId).join(',')}`)
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Checkout could not be started.')
    } finally {
      setBusy(false)
    }
  }

  const total = selectedBooks.reduce((sum, book) => sum + book.price, 0)
  return <section className="cart-page">
    <div className="page-heading"><div><p className="eyebrow">YOUR NEXT READ</p><h1>Shopping cart</h1></div><Link className="quiet-link" to="/books">Continue browsing ↗</Link></div>
    {loading ? <LoadingSpinner label="Opening your cart" /> : error && !books.length ? <ErrorMessage message={error} onRetry={() => setReloadKey((key) => key + 1)} /> : !books.length ? <EmptyState title="Your cart is empty" detail="A good book is just a few shelves away." action={<Link className="button" to="/books">Browse books</Link>} /> : <div className="cart-layout">
      <div className="cart-items">
        <label className="cart-select-all"><input type="checkbox" checked={allAvailableSelected} onChange={toggleAll} /> Select all available books</label>
        {books.map((book) => <article className="cart-item" key={book.bookId}>
          <input aria-label={`Select ${book.title} for checkout`} type="checkbox" checked={selectedIds.includes(book.bookId)} disabled={!book.isAvailable} onChange={() => toggleBook(book.bookId)} />
          <Link to={`/books/${book.bookId}`} className="cart-thumb">{book.coverImage ? <img src={book.coverImage} alt="" /> : <span>{book.title.slice(0, 1)}</span>}</Link>
          <div className="cart-item-copy"><Link to={`/books/${book.bookId}`}><h2>{book.title}</h2></Link><p>{book.isAvailable ? 'Available now' : 'Unavailable'}</p><button className="quiet-link" onClick={() => removeBook(book.bookId)}>Remove</button></div>
          <strong>${book.price.toFixed(2)}</strong>
        </article>)}
      </div>
      <aside className="order-summary"><p className="eyebrow">ORDER SUMMARY</p><div><span>Selected books</span><span>{selectedBooks.length}</span></div><div className="summary-total"><strong>Total</strong><strong>${total.toFixed(2)}</strong></div><button className="button button-wide" disabled={busy || selectedBooks.length === 0} onClick={() => void checkout()}>{busy ? 'Preparing checkout…' : `Checkout ${allAvailableSelected ? 'all available books' : 'selected books'}`}</button><p>One payment covers every selected title.</p>{error && <p className="form-error" role="alert">{error}</p>}</aside>
    </div>}
  </section>
}

export default CartPage