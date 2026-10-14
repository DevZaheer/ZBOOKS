import { Link } from 'react-router-dom'
import type { BookDto } from '../types'
import StarRating from './StarRating'
import SaveBookButton from './SaveBookButton'
import AddToCartButton from './AddToCartButton'
import './BookCard.css'

interface BookCardProps {
  book: BookDto
  authors?: string
  categories?: string
}

function BookCard({ book, authors = 'Independent voices', categories }: BookCardProps) {
  return <article className="book-card">
    <Link className="book-cover-link" to={`/books/${book.bookId}`} aria-label={`View ${book.title}`}>
      {book.coverImage ? <img className="book-cover" src={book.coverImage} alt={`Cover of ${book.title}`} loading="lazy" /> : <div className="book-cover cover-placeholder"><span>ZBOOKS EDITION</span><strong>{book.title}</strong><i>{authors}</i></div>}
    </Link>
    <div className="book-card-info">
      {categories && <span className="eyebrow">{categories}</span>}
      <h3><Link to={`/books/${book.bookId}`}>{book.title}</Link></h3>
      <p className="book-author">{authors}</p>
      <div className="book-rating"><StarRating value={book.averageRating} /><span>{book.reviewCount ? `${book.averageRating.toFixed(1)} · ${book.reviewCount}` : 'New arrival'}</span></div>
      <div className="book-card-bottom"><strong>${book.price.toFixed(2)}</strong><div className="book-card-actions"><SaveBookButton bookId={book.bookId} /><AddToCartButton bookId={book.bookId} available={book.isAvailable} className="button button-small" compact /></div></div>
    </div>
  </article>
}

export default BookCard