import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { accountApi, catalogApi } from '../api/resources'
import type { BookDto, WishlistDto } from '../types'
import BookList from '../components/BookList'
import { EmptyState, ErrorMessage, LoadingSpinner } from '../components/Feedback'
import './WishlistPage.css'

function WishlistPage() {
  const [wishlist, setWishlist] = useState<WishlistDto[]>([])
  const [books, setBooks] = useState<BookDto[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [reloadKey, setReloadKey] = useState(0)

  async function loadWishlist() {
    setLoading(true)
    setError('')
    try {
      const items = await accountApi.wishlists()
      setWishlist(items)
      setBooks(await Promise.all(items.map((item) => catalogApi.book(item.bookId))))
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Wishlist could not be loaded.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    const timer = window.setTimeout(() => { void loadWishlist() }, 0)
    return () => window.clearTimeout(timer)
  }, [reloadKey])

  async function remove(bookId: number) {
    const entry = wishlist.find((item) => item.bookId === bookId)
    if (!entry) return
    try {
      await accountApi.deleteWishlist(entry.wishlistId)
      setWishlist((items) => items.filter((item) => item.wishlistId !== entry.wishlistId))
      setBooks((items) => items.filter((item) => item.bookId !== bookId))
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Book could not be removed.')
    }
  }

  return <section className="wishlist-page"><div className="page-heading"><div><p className="eyebrow">SAVED FOR LATER</p><h1>Your wishlist</h1></div><span className="result-count">{books.length} saved</span></div>{loading ? <LoadingSpinner label="Gathering your saved books" /> : error && !books.length ? <ErrorMessage message={error} onRetry={() => setReloadKey((key) => key + 1)} /> : books.length ? <><div className="wishlist-tools"><p>Keep the titles you want to come back to.</p><button className="quiet-link" onClick={() => void Promise.all(wishlist.map((item) => accountApi.deleteWishlist(item.wishlistId))).then(() => { setWishlist([]); setBooks([]) }).catch((cause: unknown) => setError(cause instanceof Error ? cause.message : 'Wishlist could not be cleared.'))}>Clear wishlist</button></div><div className="book-grid">{books.map((book) => <div className="wishlist-card" key={book.bookId}><BookList books={[book]} /><button className="quiet-link remove-saved" onClick={() => void remove(book.bookId)}>Remove from wishlist</button></div>)}</div>{error && <p className="form-error">{error}</p>}</> : <EmptyState title="Nothing saved yet" detail="Save the books you would like to read next." action={<Link to="/books" className="button">Explore books</Link>} />}</section>
}

export default WishlistPage