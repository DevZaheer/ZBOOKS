import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { accountApi, catalogApi } from '../api/resources'
import type { BookDto, PaymentDto, PurchaseDto } from '../types'
import { EmptyState, ErrorMessage, LoadingSpinner } from '../components/Feedback'
import './OrdersPage.css'

interface OrderItem {
  purchase: PurchaseDto
  payment?: PaymentDto
  book: BookDto
}

function OrdersPage() {
  const [items, setItems] = useState<OrderItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [reloadKey, setReloadKey] = useState(0)

  useEffect(() => {
    let active = true

    async function loadOrders() {
      setLoading(true)
      setError('')
      try {
        const [purchases, payments] = await Promise.all([accountApi.purchases(), accountApi.payments()])
        const bookIds = [...new Set(purchases.map((purchase) => purchase.bookId))]
        const books = await Promise.all(bookIds.map((id) => catalogApi.book(id)))
        const orderItems = purchases
          .map((purchase) => ({
            purchase,
            payment: payments.find((payment) => payment.purchaseId === purchase.purchaseId),
            book: books.find((book) => book.bookId === purchase.bookId)!,
          }))
          .sort((left, right) => new Date(right.purchase.purchasedAt).getTime() - new Date(left.purchase.purchasedAt).getTime())

        if (active) setItems(orderItems)
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : 'Your orders could not be loaded.')
      } finally {
        if (active) setLoading(false)
      }
    }

    void loadOrders()
    return () => { active = false }
  }, [reloadKey])

  return <section className="orders-page">
    <div className="page-heading">
      <div><p className="eyebrow">ACCOUNT</p><h1>Orders &amp; payments</h1></div>
      {!loading && !error && <span className="result-count">{items.length} {items.length === 1 ? 'order' : 'orders'}</span>}
    </div>
    {loading ? <LoadingSpinner label="Loading your orders" /> : error && !items.length ? <ErrorMessage message={error} onRetry={() => setReloadKey((key) => key + 1)} /> : items.length ? <>
      <div className="orders-list" aria-label="Order history">
        {items.map(({ purchase, payment, book }) => {
          const status = (payment?.status ?? purchase.status).toLowerCase()
          const isPending = purchase.status.toLowerCase() === 'pending'
          return <article className="order-item" key={purchase.purchaseId}>
            <Link className="order-cover" to={`/books/${book.bookId}`} aria-label={`View ${book.title}`}>
              {book.coverImage ? <img src={book.coverImage} alt="" /> : <span>{book.title.slice(0, 1)}</span>}
            </Link>
            <div className="order-book">
              <p className="eyebrow">ORDER #{purchase.purchaseId}</p>
              <h2><Link to={`/books/${book.bookId}`}>{book.title}</Link></h2>
              <p>Placed {new Date(purchase.purchasedAt).toLocaleDateString()}</p>
            </div>
            <div className="order-payment">
              <span className={`order-status order-status-${status.replace(/[^a-z0-9]+/g, '-')}`}>{payment?.status ?? purchase.status}</span>
              {payment ? <>
                <span>{payment.paymentMethod || 'Payment'}{payment.paidAt ? ` · ${new Date(payment.paidAt).toLocaleDateString()}` : ''}</span>
                <span className="payment-reference">Ref. {payment.paymentReference}</span>
              </> : <span>{isPending ? 'Awaiting payment' : 'No payment record'}</span>}
            </div>
            <div className="order-total">
              <strong>${purchase.amount.toFixed(2)}</strong>
              {isPending && <Link className="button button-small" to={`/payment/${purchase.purchaseId}`}>Complete payment</Link>}
            </div>
          </article>
        })}
      </div>
      {error && <ErrorMessage message={error} />}
    </> : <EmptyState title="No orders yet" detail="Your purchases and payment records will appear here." action={<Link to="/books" className="button">Browse books</Link>} />}
  </section>
}

export default OrdersPage