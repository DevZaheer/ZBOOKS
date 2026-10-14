import { Link } from 'react-router-dom'
import './NotFoundPage.css'

function NotFoundPage() {
  return <section className="not-found-page"><p className="eyebrow">PAGE 404</p><span className="not-found-number">404</span><h1>This page wandered off.</h1><p>We couldn’t find that shelf. Try another aisle.</p><Link className="button" to="/">Return home</Link></section>
}

export default NotFoundPage