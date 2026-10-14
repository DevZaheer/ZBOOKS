import './SearchBar.css'

interface SearchBarProps {
  value: string
  onChange: (value: string) => void
  placeholder?: string
}

function SearchBar({ value, onChange, placeholder = 'Search titles, authors, subjects' }: SearchBarProps) {
  return <label className="search-field"><span aria-hidden="true">⌕</span><input type="search" value={value} onChange={(event) => onChange(event.target.value)} placeholder={placeholder} aria-label="Search books" />{value && <button type="button" aria-label="Clear search" onClick={() => onChange('')}>×</button>}</label>
}

export default SearchBar