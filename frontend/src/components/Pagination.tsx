import './Pagination.css'

interface PaginationProps {
  page: number
  totalPages: number
  onPageChange: (page: number) => void
}

function Pagination({ page, totalPages, onPageChange }: PaginationProps) {
  if (totalPages < 2) return null
  return <nav className="pagination" aria-label="Book pages">
    <button className="button button-outline" disabled={page <= 1} onClick={() => onPageChange(page - 1)}>Previous</button>
    <span>Page <strong>{page}</strong> of {totalPages}</span>
    <button className="button button-outline" disabled={page >= totalPages} onClick={() => onPageChange(page + 1)}>Next</button>
  </nav>
}

export default Pagination