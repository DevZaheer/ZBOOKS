import { useEffect, useEffectEvent, useRef, useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { jsonBody, request } from '../api/client'
import { accountApi, catalogApi } from '../api/resources'
import type { BookDto, PurchaseDto, ReadingProgressDto } from '../types'
import { EmptyState, ErrorMessage, LoadingSpinner } from '../components/Feedback'
import './LibraryPage.css'

const AUTOSAVE_INTERVAL_MS = 5 * 60 * 1000

interface LibraryItem {
  purchase: PurchaseDto
  book: BookDto
  progress?: ReadingProgressDto
}

interface ProgressValues {
  currentChapter: number
  currentPage: number
}

function LibraryPage() {
  const [items, setItems] = useState<LibraryItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [savingBooks, setSavingBooks] = useState<Set<number>>(() => new Set())
  const [reloadKey, setReloadKey] = useState(0)
  const itemsRef = useRef<LibraryItem[]>([])
  const formRefs = useRef(new Map<number, HTMLFormElement>())
  const lastSavedRef = useRef(new Map<number, ProgressValues>())
  const inFlightRef = useRef(new Set<number>())

  async function loadLibrary() {
    setLoading(true)
    setError('')
    try {
      const [allPurchases, progress] = await Promise.all([accountApi.purchases(), accountApi.readingProgress()])
      const purchases = allPurchases.filter((purchase) => purchase.status.toLowerCase() === 'completed')
      const bookIds = [...new Set(purchases.map((purchase) => purchase.bookId))]
      const books = await Promise.all(bookIds.map((id) => catalogApi.book(id)))
      const loadedItems = purchases.map((purchase) => ({ purchase, book: books.find((book) => book.bookId === purchase.bookId)!, progress: progress.find((entry) => entry.bookId === purchase.bookId) }))
      itemsRef.current = loadedItems
      lastSavedRef.current = new Map(loadedItems.flatMap(({ book, progress: saved }) => saved ? [[book.bookId, { currentChapter: saved.currentChapter, currentPage: saved.currentPage }] as const] : []))
      setItems(loadedItems)
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Your library could not be loaded.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    const timer = window.setTimeout(() => { void loadLibrary() }, 0)
    return () => window.clearTimeout(timer)
  }, [reloadKey])

  async function persistProgress(item: LibraryItem, values: ProgressValues, keepalive: boolean) {
    const bookId = item.book.bookId
    if (inFlightRef.current.has(bookId)) return
    inFlightRef.current.add(bookId)
    setSavingBooks((current) => new Set(current).add(bookId))
    setError('')
    try {
      const saved = item.progress
        ? await request<ReadingProgressDto>(`/readingprogresses/${item.progress.readingProgressId}`, { method: 'PUT', body: jsonBody(values), keepalive })
        : await request<ReadingProgressDto>('/readingprogresses', { method: 'POST', body: jsonBody({ bookId, ...values }), keepalive })
      lastSavedRef.current.set(bookId, values)
      const updatedItems = itemsRef.current.map((entry) => entry.book.bookId === bookId ? { ...entry, progress: saved } : entry)
      itemsRef.current = updatedItems
      setItems(updatedItems)
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Reading progress could not be saved.')
    } finally {
      inFlightRef.current.delete(bookId)
      setSavingBooks((current) => {
        const updated = new Set(current)
        updated.delete(bookId)
        return updated
      })
    }
  }

  function readProgressValues(form: HTMLFormElement): ProgressValues | null {
    const formData = new FormData(form)
    const chapterValue = formData.get('chapter')
    const pageValue = formData.get('page')
    if (typeof chapterValue !== 'string' || typeof pageValue !== 'string' || !chapterValue || !pageValue) return null
    const currentChapter = Number(chapterValue)
    const currentPage = Number(pageValue)
    if (![currentChapter, currentPage].every(Number.isInteger) || currentChapter < 0 || currentPage < 0) return null
    return { currentChapter, currentPage }
  }

  const saveChanged = useEffectEvent((keepalive: boolean) => {
    for (const item of itemsRef.current) {
      const form = formRefs.current.get(item.book.bookId)
      const values = form && readProgressValues(form)
      if (!values) continue
      const lastSaved = lastSavedRef.current.get(item.book.bookId)
      if (lastSaved?.currentChapter === values.currentChapter && lastSaved.currentPage === values.currentPage) continue
      void persistProgress(item, values, keepalive)
    }
  })

  useEffect(() => {
    const interval = window.setInterval(() => saveChanged(false), AUTOSAVE_INTERVAL_MS)
    const handleVisibilityChange = () => {
      if (document.visibilityState === 'hidden') saveChanged(true)
    }
    const handlePageHide = () => saveChanged(true)
    document.addEventListener('visibilitychange', handleVisibilityChange)
    window.addEventListener('pagehide', handlePageHide)
    return () => {
      window.clearInterval(interval)
      document.removeEventListener('visibilitychange', handleVisibilityChange)
      window.removeEventListener('pagehide', handlePageHide)
    }
  }, [])

  async function saveProgress(event: FormEvent<HTMLFormElement>, item: LibraryItem) {
    event.preventDefault()
    const values = readProgressValues(event.currentTarget)
    if (!values) {
      setError('Enter valid non-negative chapter and page numbers.')
      return
    }
    const lastSaved = lastSavedRef.current.get(item.book.bookId)
    if (lastSaved?.currentChapter === values.currentChapter && lastSaved.currentPage === values.currentPage) return
    await persistProgress(item, values, false)
  }

  return <section className="library-page"><div className="page-heading"><div><p className="eyebrow">BOOKS YOU OWN</p><h1>My library</h1></div><span className="result-count">{items.length} titles</span></div>{loading ? <LoadingSpinner label="Opening your library" /> : error && !items.length ? <ErrorMessage message={error} onRetry={() => setReloadKey((key) => key + 1)} /> : items.length ? <div className="library-list">{items.map((item) => <article className="library-item" key={item.purchase.purchaseId}><Link className="library-cover" to={`/books/${item.book.bookId}`}>{item.book.coverImage ? <img src={item.book.coverImage} alt="" /> : <span>{item.book.title.slice(0, 1)}</span>}</Link><div className="library-book"><p className="eyebrow">{item.progress?.status ?? 'Not started'}</p><h2><Link to={`/books/${item.book.bookId}`}>{item.book.title}</Link></h2><p>Purchased {new Date(item.purchase.purchasedAt).toLocaleDateString()}</p><div className="progress-track"><span style={{ width: `${item.progress?.completionPercent ?? 0}%` }} /></div><span className="progress-label">{item.progress?.completionPercent ?? 0}% complete</span></div><form className="progress-form" ref={(form) => { if (form) formRefs.current.set(item.book.bookId, form); else formRefs.current.delete(item.book.bookId) }} onSubmit={(event) => void saveProgress(event, item)}><label>Chapter<input name="chapter" type="number" min="0" defaultValue={item.progress?.currentChapter ?? 0} required /></label><label>Page<input name="page" type="number" min="0" defaultValue={item.progress?.currentPage ?? 0} required /></label><button className="button button-small" disabled={savingBooks.has(item.book.bookId)}>{savingBooks.has(item.book.bookId) ? 'Saving…' : 'Save progress'}</button></form></article>)}</div> : <EmptyState title="Your library is waiting" detail="Purchased books will be collected here, ready when you are." action={<Link to="/books" className="button">Find a book</Link>} />}{error && items.length > 0 && <ErrorMessage message={error} />}</section>
}

export default LibraryPage