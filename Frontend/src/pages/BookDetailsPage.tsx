import { useEffect, useMemo, useState, type FormEvent } from 'react'
import { Link, useParams } from 'react-router-dom'
import { catalogApi } from '../api/resources'
import { ApiError } from '../api/client'
import { useAuth } from '../context/useAuth'
import type { AuthorDto, BookDto, CategoryDto, ChapterDto, ReviewDto } from '../types'
import StarRating from '../components/StarRating'
import ReviewList from '../components/ReviewList'
import SaveBookButton from '../components/SaveBookButton'
import AddToCartButton from '../components/AddToCartButton'
import { EmptyState, ErrorMessage, LoadingSpinner } from '../components/Feedback'
import './BookDetailsPage.css'

function BookDetailsPage() {
  const { bookId } = useParams()
  const id = Number(bookId)
  const { token } = useAuth()
  const [book, setBook] = useState<BookDto | null>(null)
  const [authors, setAuthors] = useState<AuthorDto[]>([])
  const [categories, setCategories] = useState<CategoryDto[]>([])
  const [reviews, setReviews] = useState<ReviewDto[]>([])
  const [chapters, setChapters] = useState<ChapterDto[]>([])
  const [chapterError, setChapterError] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [rating, setRating] = useState(0)
  const [comment, setComment] = useState('')
  const [reviewError, setReviewError] = useState('')
  const [reviewBusy, setReviewBusy] = useState(false)
  const [reloadKey, setReloadKey] = useState(0)

  useEffect(() => {
    let active = true
    const timer = window.setTimeout(() => {
      if (!Number.isFinite(id) || id <= 0) { setError('That book could not be found.'); setLoading(false); return }
      setLoading(true)
      setError('')
      void Promise.all([catalogApi.book(id), catalogApi.authors(), catalogApi.categories(), catalogApi.reviews()]).then(([item, authorList, categoryList, allReviews]) => {
        if (!active) return
        setBook(item)
        setAuthors(authorList)
        setCategories(categoryList)
        setReviews(allReviews.filter((review) => review.bookId === id).sort((a, b) => b.createdAt.localeCompare(a.createdAt)))
      }).catch((cause: unknown) => {
        if (active) setError(cause instanceof Error ? cause.message : 'Book details could not be loaded.')
      }).finally(() => { if (active) setLoading(false) })
    }, 0)
    return () => { active = false; window.clearTimeout(timer) }
  }, [id, reloadKey])

  useEffect(() => {
    if (!token || !Number.isFinite(id)) return
    void catalogApi.chapters(id).then(setChapters).catch((cause: unknown) => {
      setChapterError(cause instanceof ApiError && cause.status === 403 ? 'Purchase this title to open its chapters.' : cause instanceof Error ? cause.message : 'Chapters are unavailable.')
    })
  }, [id, token])

  const authorNames = useMemo(() => book?.authorIds.map((authorId) => authors.find((author) => author.authorId === authorId)?.name).filter(Boolean).join(', ') || 'Independent voices', [book, authors])
  const categoryNames = useMemo(() => book?.categoryIds.map((categoryId) => categories.find((category) => category.categoryId === categoryId)?.name).filter(Boolean).join(' · '), [book, categories])

  async function submitReview(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!rating) { setReviewError('Choose a star rating before submitting.'); return }
    setReviewBusy(true)
    setReviewError('')
    try {
      await catalogApi.createReview({ bookId: id, rating, comment: comment.trim() || null })
      setComment('')
      setRating(0)
      setReloadKey((key) => key + 1)
    } catch (cause) {
      setReviewError(cause instanceof Error ? cause.message : 'Your review could not be submitted.')
    } finally {
      setReviewBusy(false)
    }
  }

  if (loading) return <LoadingSpinner label="Opening book details" />
  if (error || !book) return <ErrorMessage message={error || 'Book not found.'} onRetry={() => setReloadKey((key) => key + 1)} />

  return <article className="details-page">
    <Link className="back-link" to="/books">← Back to the collection</Link>
    <section className="details-hero">
      <div className="details-cover">{book.coverImage ? <img src={book.coverImage} alt={`Cover of ${book.title}`} /> : <div className="book-cover cover-placeholder"><span>ZBOOKS EDITION</span><strong>{book.title}</strong><i>{authorNames}</i></div>}</div>
      <div className="details-copy"><p className="eyebrow">{categoryNames || 'THE COLLECTION'}</p><h1>{book.title}</h1><p className="details-author">by {authorNames}</p><div className="details-rating"><StarRating value={book.averageRating} /><span>{book.reviewCount ? `${book.averageRating.toFixed(1)} from ${book.reviewCount} reviews` : 'Be the first to review'}</span></div><p className="details-description">{book.description || 'A new story waiting to be discovered.'}</p><div className="details-buy"><strong>${book.price.toFixed(2)}</strong><AddToCartButton bookId={book.bookId} available={book.isAvailable} className="button" /><SaveBookButton bookId={book.bookId} /></div><p className="details-date">Published {new Date(book.publicationDate).toLocaleDateString(undefined, { year: 'numeric', month: 'long' })}</p></div>
    </section>
    <section className="details-section"><div className="section-heading"><div><p className="eyebrow">INSIDE THE BOOK</p><h2>Chapters</h2></div></div>
      {!token ? <p className="inline-note"><Link to="/login">Sign in</Link> and own this book to read its chapters.</p> : chapterError ? <p className="inline-note">{chapterError}</p> : chapters.length ? <div className="chapter-list">{[...chapters].sort((a, b) => a.chapterNumber - b.chapterNumber).map((chapter) => <details key={chapter.chapterId}><summary><span>Chapter {chapter.chapterNumber}</span><strong>{chapter.title}</strong></summary><p>{chapter.content || 'This chapter has no preview.'}</p></details>)}</div> : <EmptyState title="No chapters available yet" detail="The reading text for this title has not been published." />}
    </section>
    <section className="details-section review-section"><div className="section-heading"><div><p className="eyebrow">FROM THE READING ROOM</p><h2>Reader reviews</h2></div><span>{reviews.length} reviews</span></div>
      {reviews.length ? <ReviewList reviews={reviews} /> : <EmptyState title="A clean page" detail="No reviews yet. Your perspective can start the conversation." />}
      {token ? <form className="review-form" onSubmit={submitReview}><h3>Leave a review</h3><label>Your rating<StarRating value={rating} interactive onChange={setRating} label="Choose a rating" /></label><label className="field-label">Your thoughts<textarea value={comment} onChange={(event) => setComment(event.target.value)} rows={4} maxLength={1000} placeholder="What stayed with you?" /></label>{reviewError && <p className="form-error">{reviewError}</p>}<button className="button" disabled={reviewBusy}>{reviewBusy ? 'Submitting…' : 'Submit review'}</button></form> : <p className="inline-note"><Link to="/login">Sign in</Link> to share a review. Reviews are available to readers who have purchased this book.</p>}
    </section>
  </article>
}

export default BookDetailsPage