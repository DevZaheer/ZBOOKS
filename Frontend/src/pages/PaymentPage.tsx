import { useEffect, useMemo, useState, type FormEvent } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { accountApi, catalogApi } from '../api/resources'
import type { BookDto, PurchaseDto } from '../types'
import { EmptyState, ErrorMessage, LoadingSpinner } from '../components/Feedback'
import './PaymentPage.css'

function PaymentPage() {
  const { purchaseId, purchaseIds } = useParams()
  const isBatch = Boolean(purchaseIds)
  const ids = useMemo(() => isBatch ? (purchaseIds ?? '').split(',').map(Number) : [Number(purchaseId)], [isBatch, purchaseId, purchaseIds])
  const navigate = useNavigate()
  const [purchases, setPurchases] = useState<PurchaseDto[]>([])
  const [books, setBooks] = useState<BookDto[]>([])
  const [method, setMethod] = useState('Card')
  const [reference, setReference] = useState('')
  const [loading, setLoading] = useState(true)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  const [reloadKey, setReloadKey] = useState(0)

  useEffect(() => {
    let active = true
    const timer = window.setTimeout(() => { void loadPayment() }, 0)

    async function loadPayment() {
      setLoading(true)
      setError('')
      if (!ids.length || ids.some((id) => !Number.isInteger(id) || id <= 0) || new Set(ids).size !== ids.length) {
        setError('Purchase not found.')
        setLoading(false)
        return
      }
      try {
        const items = await Promise.all(ids.map((id) => accountApi.purchase(id)))
        const purchasedBooks = await Promise.all(items.map((item) => catalogApi.book(item.bookId)))
        if (active) {
          setPurchases(items)
          setBooks(purchasedBooks)
        }
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : 'Purchase details could not be loaded.')
      } finally {
        if (active) setLoading(false)
      }
    }

    return () => { active = false; window.clearTimeout(timer) }
  }, [ids, reloadKey])

  async function submitPayment(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setBusy(true)
    setError('')
    try {
      const payment = {
        paymentReference: reference.trim(),
        paymentMethod: method,
      }
      if (isBatch) await accountApi.createPayments({ ...payment, purchaseIds: ids })
      else await accountApi.createPayment({ ...payment, purchaseId: ids[0] })
      navigate('/library', { replace: true })
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Payment could not be completed.')
    } finally {
      setBusy(false)
    }
  }

  if (loading) return <LoadingSpinner label="Preparing payment" />
  if (error && !purchases.length) return <ErrorMessage message={error} onRetry={() => setReloadKey((key) => key + 1)} />
  if (!purchases.length || !books.length) return <EmptyState title="Purchase not found" detail="This checkout may have expired." action={<Link to="/books" className="button">Return to books</Link>} />

  const total = purchases.reduce((sum, purchase) => sum + purchase.amount, 0)

  return <section className="payment-page">
    <Link className="back-link" to="/cart">← Back to bag</Link>
    <div className="page-heading"><div><p className="eyebrow">SECURE CHECKOUT</p><h1>Complete your order</h1></div></div>
    <div className="payment-layout">
      <form className="payment-form" onSubmit={submitPayment}>
        <h2>Payment details</h2>
        <label className="field-label">Payment method<select required value={method} onChange={(event) => setMethod(event.target.value)}><option>Card</option><option>Bank transfer</option><option>Mobile payment</option></select></label>
        <label className="field-label">Payment reference<input required value={reference} onChange={(event) => setReference(event.target.value)} minLength={4} maxLength={80} placeholder="Enter your payment reference" /></label>
        <p className="inline-note">Use the reference supplied by your payment provider. Card information is not collected here.</p>
        {error && <p className="form-error" role="alert">{error}</p>}
        <button className="button button-wide" disabled={busy}>{busy ? 'Confirming…' : `Confirm $${total.toFixed(2)}`}</button>
      </form>
      <aside className="payment-order">
        <p className="eyebrow">YOUR ORDER · {books.length} {books.length === 1 ? 'BOOK' : 'BOOKS'}</p>
        <div className="payment-books">{books.map((book) => <div className="payment-book" key={book.bookId}>
          <div className="cart-thumb">{book.coverImage ? <img src={book.coverImage} alt="" /> : <span>{book.title.slice(0, 1)}</span>}</div>
          <div><strong>{book.title}</strong><span>${book.price.toFixed(2)}</span></div>
        </div>)}</div>
        <div className="summary-total"><strong>Total due</strong><strong>${total.toFixed(2)}</strong></div>
      </aside>
    </div>
  </section>
}

export default PaymentPage
