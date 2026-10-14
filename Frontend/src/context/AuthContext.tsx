import { useState, type ReactNode } from 'react'
import { authApi } from '../api/resources'
import type { AuthUser, LoginDto, RegisterDto } from '../types'
import { clearCart } from '../utils/cart'
import { AuthContext } from './auth-context'

function userFromToken(token: string): AuthUser {
  try {
    const payload = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')
    const claims = JSON.parse(atob(payload)) as Record<string, unknown>
    const id = String(claims.nameid ?? claims.sub ?? claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ?? '')
    const email = String(claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? claims.email ?? '')
    const role = String(claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? claims.role ?? 'Reader')
    return { id, email, role }
  } catch {
    return { id: '', email: '', role: 'Reader' }
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem('zbooks_token'))
  const [user, setUser] = useState<AuthUser | null>(() => {
    const saved = localStorage.getItem('zbooks_token')
    return saved ? userFromToken(saved) : null
  })

  async function login(data: LoginDto) {
    const result = await authApi.login(data)
    localStorage.setItem('zbooks_token', result.token)
    setToken(result.token)
    setUser(userFromToken(result.token))
  }

  async function register(data: RegisterDto) {
    await authApi.register(data)
    await login({ email: data.email, password: data.password })
  }

  function logout() {
    clearCart(user?.id || undefined)
    localStorage.removeItem('zbooks_token')
    setToken(null)
    setUser(null)
  }

  return <AuthContext.Provider value={{ token, user, login, register, logout }}>{children}</AuthContext.Provider>
}