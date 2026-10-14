import './StarRating.css'

interface StarRatingProps {
  value: number
  interactive?: boolean
  onChange?: (value: number) => void
  label?: string
}

function StarRating({ value, interactive = false, onChange, label }: StarRatingProps) {
  return <div className={`star-rating${interactive ? ' star-rating-editable' : ''}`} aria-label={label ?? `${value.toFixed(1)} out of 5 stars`}>
    {Array.from({ length: 5 }, (_, index) => {
      const rating = index + 1
      return interactive ? <button type="button" key={rating} className={rating <= value ? 'star is-filled' : 'star'} aria-label={`${rating} star${rating === 1 ? '' : 's'}`} onClick={() => onChange?.(rating)}>★</button> : <span key={rating} className={rating <= Math.round(value) ? 'star is-filled' : 'star'}>★</span>
    })}
  </div>
}

export default StarRating