import { createContext } from 'react'
import type { AuthUser, LoginDto, RegisterDto } from '../types'

export interface AuthContextValue {
  token: string | null
  user: AuthUser | null
  login: (data: LoginDto) => Promise<void>
  register: (data: RegisterDto) => Promise<void>
  logout: () => void
}

export const AuthContext = createContext<AuthContextValue | null>(null)