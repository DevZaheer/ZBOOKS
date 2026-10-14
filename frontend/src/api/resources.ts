import { jsonBody, request } from './client'
import type {
  AuthorDto, BookDto, BookQueryParams, CategoryDto, ChapterDto, CreateBatchPaymentDto, CreatePaymentDto,
  CreatePurchaseDto, CreatePurchasesDto, CreateReadingProgressDto, CreateReviewDto, CreateWishlistDto,
  LoginDto, PagedResult, PaymentDto, PurchaseDto, ReadingProgressDto, RegisterDto, UserDto,
  ReviewDto, UpdateReadingProgressDto, WishlistDto,
} from '../types'

function queryString(params: object): string {
  const query = new URLSearchParams()
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') query.set(key, String(value))
  })
  const value = query.toString()
  return value ? `?${value}` : ''
}

export const authApi = {
  register: (data: RegisterDto) => request<UserDto>('/auth/register', { method: 'POST', body: jsonBody(data) }),
  login: (data: LoginDto) => request<{ token: string }>('/auth/login', { method: 'POST', body: jsonBody(data) }),
}

export const catalogApi = {
  books: (params: BookQueryParams = {}) => request<PagedResult<BookDto>>(`/books${queryString(params)}`),
  book: (id: number) => request<BookDto>(`/books/${id}`),
  authors: () => request<AuthorDto[]>('/authors'),
  categories: () => request<CategoryDto[]>('/categories'),
  reviews: () => request<ReviewDto[]>('/reviews'),
  createReview: (data: CreateReviewDto) => request<ReviewDto>('/reviews', { method: 'POST', body: jsonBody(data) }),
  chapters: (bookId: number) => request<ChapterDto[]>(`/chapters${queryString({ bookId })}`),
}

export const accountApi = {
  purchases: () => request<PurchaseDto[]>('/purchases'),
  purchase: (id: number) => request<PurchaseDto>(`/purchases/${id}`),
  createPurchase: (data: CreatePurchaseDto) => request<PurchaseDto>('/purchases', { method: 'POST', body: jsonBody(data) }),
  createPurchases: (data: CreatePurchasesDto) => request<PurchaseDto[]>('/purchases/batch', { method: 'POST', body: jsonBody(data) }),
  payments: () => request<PaymentDto[]>('/payments'),
  createPayment: (data: CreatePaymentDto) => request<PaymentDto>('/payments', { method: 'POST', body: jsonBody(data) }),
  createPayments: (data: CreateBatchPaymentDto) => request<PaymentDto[]>('/payments/batch', { method: 'POST', body: jsonBody(data) }),
  wishlists: () => request<WishlistDto[]>('/wishlists'),
  createWishlist: (data: CreateWishlistDto) => request<WishlistDto>('/wishlists', { method: 'POST', body: jsonBody(data) }),
  deleteWishlist: (id: number) => request<void>(`/wishlists/${id}`, { method: 'DELETE' }),
  readingProgress: () => request<ReadingProgressDto[]>('/readingprogresses'),
  createReadingProgress: (data: CreateReadingProgressDto) => request<ReadingProgressDto>('/readingprogresses', { method: 'POST', body: jsonBody(data) }),
  updateReadingProgress: (id: number, data: UpdateReadingProgressDto) => request<ReadingProgressDto>(`/readingprogresses/${id}`, { method: 'PUT', body: jsonBody(data) }),
}