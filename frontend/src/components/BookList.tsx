import type { AuthorDto, BookDto, CategoryDto } from '../types'
import BookCard from './BookCard'
import './BookList.css'

interface BookListProps {
  books: BookDto[]
  authors?: AuthorDto[]
  categories?: CategoryDto[]
}

function BookList({ books, authors = [], categories = [] }: BookListProps) {
  const authorName = (ids: number[]) => ids.map((id) => authors.find((author) => author.authorId === id)?.name).filter(Boolean).join(', ') || 'Independent voices'
  const categoryName = (ids: number[]) => ids.map((id) => categories.find((category) => category.categoryId === id)?.name).filter(Boolean).join(' · ')
  return <div className="book-grid">{books.map((book) => <BookCard key={book.bookId} book={book} authors={authorName(book.authorIds)} categories={categoryName(book.categoryIds)} />)}</div>
}

export default BookList