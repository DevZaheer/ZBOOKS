import type { ReviewDto } from '../types'
import StarRating from './StarRating'
import './ReviewList.css'

function ReviewList({ reviews }: { reviews: ReviewDto[] }) {
  return <div className="review-list">{reviews.map((review) => <article className="review-item" key={review.reviewId}>
    <div className="review-meta"><strong>Reader #{review.userId}</strong><time dateTime={review.createdAt}>{new Date(review.createdAt).toLocaleDateString()}</time></div>
    <StarRating value={review.rating} />
    {review.comment && <p>{review.comment}</p>}
  </article>)}</div>
}

export default ReviewList