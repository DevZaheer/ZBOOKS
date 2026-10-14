import { useEffect, useState } from 'react'
import { catalogApi } from '../api/resources'
import type { AuthorDto, BookDto, BookQueryParams, CategoryDto } from '../types'
import BookList from '../components/BookList'
import SearchBar from '../components/SearchBar'
import Pagination from '../components/Pagination'
import { EmptyState, ErrorMessage, LoadingSpinner } from '../components/Feedback'
import './BooksPage.css'

function BooksPage() {
  const [books, setBooks] = useState<BookDto[]>([])
  const [authors, setAuthors] = useState<AuthorDto[]>([])
  const [categories, setCategories] = useState<CategoryDto[]>([])
  const [search, setSearch] = useState('')
  const [categoryId, setCategoryId] = useState('')
  const [authorId, setAuthorId] = useState('')
  const [sortBy, setSortBy] = useState<BookQueryParams['sortBy']>('title')
  const [descending, setDescending] = useState(false)
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(0)
  const [totalCount, setTotalCount] = useState(0)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [reloadKey, setReloadKey] = useState(0)

  useEffect(() => {
    void Promise.all([catalogApi.authors(), catalogApi.categories()]).then(([authorList, categoryList]) => {
      setAuthors(authorList)
      setCategories(categoryList)
    }).catch((cause: unknown) => setError(cause instanceof Error ? cause.message : 'Filters could not be loaded.'))
  }, [])

  useEffect(() => {
    let active = true
    const timer = window.setTimeout(() => {
      setLoading(true)
      setError('')
      void catalogApi.books({ search: search || undefined, categoryId: categoryId ? Number(categoryId) : undefined, authorId: authorId ? Number(authorId) : undefined, sortBy, descending, page, pageSize: 12 })
        .then((result) => {
          if (!active) return
          setBooks(result.items)
          setTotalPages(result.totalPages)
          setTotalCount(result.totalCount)
        }).catch((cause: unknown) => {
          if (active) setError(cause instanceof Error ? cause.message : 'Books could not be loaded.')
        }).finally(() => { if (active) setLoading(false) })
    }, 180)
    return () => { active = false; window.clearTimeout(timer) }
  }, [search, categoryId, authorId, sortBy, descending, page, reloadKey])

  function updateFilter(setter: (value: string) => void, value: string) {
    setter(value)
    setPage(1)
  }

  return <section className="catalog-page">
    <div className="page-heading"><div><p className="eyebrow">THE COLLECTION</p><h1>Browse books</h1></div><span className="result-count">{totalCount} titles</span></div>
    <div className="catalog-toolbar">
      <SearchBar value={search} onChange={(value) => updateFilter(setSearch, value)} />
      <label className="select-field"><span>Category</span><select value={categoryId} onChange={(event) => updateFilter(setCategoryId, event.target.value)}><option value="">All categories</option>{categories.map((category) => <option key={category.categoryId} value={category.categoryId}>{category.name}</option>)}</select></label>
      <label className="select-field"><span>Author</span><select value={authorId} onChange={(event) => updateFilter(setAuthorId, event.target.value)}><option value="">All authors</option>{authors.map((author) => <option key={author.authorId} value={author.authorId}>{author.name}</option>)}</select></label>
      <label className="select-field"><span>Sort by</span><select value={sortBy} onChange={(event) => { setSortBy(event.target.value as BookQueryParams['sortBy']); setPage(1) }}><option value="title">Title</option><option value="price">Price</option><option value="newest">Newest</option><option value="rating">Top rated</option></select></label>
      <button className="sort-direction" onClick={() => { setDescending((value) => !value); setPage(1) }} aria-label={`Sort ${descending ? 'ascending' : 'descending'}`}>{descending ? '↓' : '↑'}</button>
    </div>
    {loading ? <LoadingSpinner label="Looking through the shelves" /> : error ? <ErrorMessage message={error} onRetry={() => setReloadKey((key) => key + 1)} /> : books.length ? <><BookList books={books} authors={authors} categories={categories} /><Pagination page={page} totalPages={totalPages} onPageChange={setPage} /></> : <EmptyState title="No matching books" detail="Try another title, author, or category." />}
  </section>
}

export default BooksPage