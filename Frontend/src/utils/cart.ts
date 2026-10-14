const CART_KEY = 'zbooks_cart'

function cartKey(customerId?: string): string {
  return customerId ? `${CART_KEY}:${encodeURIComponent(customerId)}` : CART_KEY
}

export function getCart(customerId?: string): number[] {
  try {
    const value: unknown = JSON.parse(localStorage.getItem(cartKey(customerId)) ?? '[]')
    return Array.isArray(value) ? value.filter((id): id is number => typeof id === 'number') : []
  } catch {
    return []
  }
}

export function saveCart(bookIds: number[], customerId?: string) {
  localStorage.setItem(cartKey(customerId), JSON.stringify(bookIds))
  window.dispatchEvent(new Event('zbooks-cart-change'))
}

export function clearCart(customerId?: string) {
  localStorage.removeItem(cartKey(customerId))
  if (customerId) localStorage.removeItem(CART_KEY)
  window.dispatchEvent(new Event('zbooks-cart-change'))
}

export function addToCart(bookId: number, customerId?: string): boolean {
  const cart = getCart(customerId)
  if (cart.includes(bookId)) return false
  saveCart([...cart, bookId], customerId)
  return true
}