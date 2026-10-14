export interface BookDto {
  bookId: number
  title: string
  description: string | null
  coverImage: string | null
  price: number
  publicationDate: string
  isAvailable: boolean
  authorIds: number[]
  categoryIds: number[]
  averageRating: number
  reviewCount: number
}

export interface BookQueryParams {
  search?: string
  categoryId?: number
  authorId?: number
  minPrice?: number
  maxPrice?: number
  isAvailable?: boolean
  sortBy?: 'title' | 'price' | 'newest' | 'rating'
  descending?: boolean
  page?: number
  pageSize?: number
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export interface AuthorDto {
  authorId: number
  name: string
  biography: string | null
  photo: string | null
}

export interface CategoryDto {
  categoryId: number
  name: string
  description: string | null
}

export interface ChapterDto {
  chapterId: number
  bookId: number
  title: string
  chapterNumber: number
  content: string | null
  createdAt: string
  updatedAt: string
}

export interface UserDto {
  userId: number
  fullName: string
  email: string
  role: string
  createdAt: string
}

export interface ReviewDto {
  reviewId: number
  userId: number
  bookId: number
  rating: number
  comment: string | null
  createdAt: string
  updatedAt: string
}

export interface PurchaseDto {
  purchaseId: number
  userId: number
  bookId: number
  amount: number
  purchasedAt: string
  status: string
}

export interface PaymentDto {
  paymentId: number
  purchaseId: number
  amount: number
  paymentReference: string
  paymentMethod: string | null
  status: string
  paidAt: string | null
}

export interface WishlistDto {
  wishlistId: number
  userId: number
  bookId: number
  addedAt: string
}

export interface ReadingProgressDto {
  readingProgressId: number
  userId: number
  bookId: number
  currentChapter: number
  currentPage: number
  completionPercent: number
  status: string
  lastReadAt: string
}

export interface RegisterDto {
  username: string
  email: string
  password: string
}

export interface LoginDto {
  email: string
  password: string
}

export interface CreateAuthorDto {
  name: string
  biography: string | null
  photo: string | null
}

export type UpdateAuthorDto = CreateAuthorDto

export interface CreateBookDto {
  title: string
  description: string | null
  coverImage: string | null
  price: number
  publicationDate: string
  isAvailable: boolean
}

export type UpdateBookDto = CreateBookDto

export interface CreateCategoryDto {
  name: string
  description: string | null
}

export type UpdateCategoryDto = CreateCategoryDto

export interface CreateChapterDto {
  bookId: number
  title: string
  chapterNumber: number
  content: string | null
}

export interface UpdateChapterDto {
  title: string
  chapterNumber: number
  content: string | null
}

export interface CreateReviewDto {
  bookId: number
  rating: number
  comment: string | null
}

export interface UpdateReviewDto {
  rating: number
  comment: string | null
}

export interface CreatePurchaseDto {
  bookId: number
}

export interface CreatePurchasesDto {
  bookIds: number[]
}

export interface UpdatePurchaseDto {
  status: string
}

export interface CreatePaymentDto {
  purchaseId: number
  paymentReference: string
  paymentMethod: string | null
}

export interface CreateBatchPaymentDto {
  purchaseIds: number[]
  paymentReference: string
  paymentMethod: string | null
}

export interface UpdatePaymentDto {
  paymentMethod: string | null
  status: string
  paidAt: string | null
}

export interface CreateReadingProgressDto {
  bookId: number
  currentChapter: number
  currentPage: number
}

export interface UpdateReadingProgressDto {
  currentChapter: number
  currentPage: number
}

export interface CreateWishlistDto {
  bookId: number
}

export interface UpdateUserDto {
  fullName: string
  email: string
  role: string
}

export interface AuthUser {
  id: string
  email: string
  role: string
}