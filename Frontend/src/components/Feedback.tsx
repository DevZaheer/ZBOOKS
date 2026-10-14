import type { ReactNode } from 'react'
import './Feedback.css'

export function LoadingSpinner({ label = 'Loading' }: { label?: string }) {
  return <div className="loading-state" role="status"><span className="spinner" /><span>{label}</span></div>
}

export function ErrorMessage({ message, onRetry }: { message: string; onRetry?: () => void }) {
  return <div className="error-state" role="alert"><p>{message}</p>{onRetry && <button className="button button-outline" onClick={onRetry}>Try again</button>}</div>
}

export function EmptyState({ title, detail, action }: { title: string; detail: string; action?: ReactNode }) {
  return <div className="empty-state"><span className="empty-rule" /><h3>{title}</h3><p>{detail}</p>{action}</div>
}