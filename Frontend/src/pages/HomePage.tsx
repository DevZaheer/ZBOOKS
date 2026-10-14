import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { catalogApi } from '../api/resources'
import type { AuthorDto, BookDto, CategoryDto } from '../types'
import BookList from '../components/BookList'
import { EmptyState, ErrorMessage, LoadingSpinner } from '../components/Feedback'
import './HomePage.css'

function HomePage() {
  const [books, setBooks] = useState<BookDto[]>([])
  const [authors, setAuthors] = useState<AuthorDto[]>([])
  const [categories, setCategories] = useState<CategoryDto[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [reloadKey, setReloadKey] = useState(0)

  async function loadHome() {
    setLoading(true)
    setError('')
    try {
      const [result, authorList, categoryList] = await Promise.all([
        catalogApi.books({ sortBy: 'newest', descending: true, pageSize: 8 }), catalogApi.authors(), catalogApi.categories(),
      ])
      setBooks(result.items)
      setAuthors(authorList)
      setCategories(categoryList)
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'The catalog could not be loaded.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    const timer = window.setTimeout(() => { void loadHome() }, 0)
    return () => window.clearTimeout(timer)
  }, [reloadKey])

  return <div className="home-page">
    <section className="home-intro">
      <div><p className="eyebrow">A BOOKSHOP FOR EVERY KIND OF CURIOUS</p><h1>Find your next <em>somewhere.</em></h1><p className="intro-copy">Thoughtful reads, fresh perspectives, and old favorites worth keeping close.</p><Link className="button" to="/books">Explore the shelves <span>↗</span></Link></div>
      <div className="intro-art" aria-hidden="true"><div className="art-sun"/><div className="art-book art-book-one"/><div className="art-book art-book-two"/><div className="art-line">READ<br/>WIDELY</div><span className="art-caption">VOL. 01 / THE OPEN SHELF</span></div>
    </section>
    <section className="home-featured">
      <div className="section-heading"><div><p className="eyebrow">JUST IN</p><h2>New to the shelves</h2></div><Link className="quiet-link" to="/books">View all books <span>↗</span></Link></div>
      {loading ? <LoadingSpinner label="Gathering the latest titles" /> : error ? <ErrorMessage message={error} onRetry={() => setReloadKey((key) => key + 1)} /> : books.length ? <BookList books={books} authors={authors} categories={categories} /> : <EmptyState title="The shelves are quiet" detail="New titles will appear here when they arrive." />}
    </section>
  </div>
}

export default HomePage