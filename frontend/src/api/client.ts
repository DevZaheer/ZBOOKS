export const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5251/api').replace(/\/$/, '')

export class ApiError extends Error {
  status: number

  constructor(message: string, status: number) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

export async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = localStorage.getItem('zbooks_token')
  const headers = new Headers(options.headers)
  if (options.body && !headers.has('Content-Type')) headers.set('Content-Type', 'application/json')
  if (token) headers.set('Authorization', `Bearer ${token}`)

  let response: Response
  try {
    response = await fetch(`${API_BASE_URL}${path}`, { ...options, headers })
  } catch {
    throw new ApiError(`Unable to reach ZBOOKS API at ${API_BASE_URL}. Check that the backend is running.`, 0)
  }

  if (!response.ok) {
    const raw = await response.text()
    let message = raw
    try {
      const parsed: unknown = JSON.parse(raw)
      if (typeof parsed === 'string') message = parsed
      else if (parsed && typeof parsed === 'object' && 'title' in parsed && typeof parsed.title === 'string') message = parsed.title
      else if (parsed && typeof parsed === 'object' && 'message' in parsed && typeof parsed.message === 'string') message = parsed.message
    } catch {
      if (!raw) message = response.statusText
    }
    throw new ApiError(message || response.statusText || 'The request could not be completed.', response.status)
  }

  if (response.status === 204) return undefined as T
  return response.json() as Promise<T>
}

export function jsonBody(value: unknown): string {
  return JSON.stringify(value)
}