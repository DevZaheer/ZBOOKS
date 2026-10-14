import { useState } from 'react'
import { Navigate, Route, Routes } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext'
import Header from './components/Header'
import Sidebar from './components/Sidebar'
import ProtectedRoute from './components/ProtectedRoute'
import HomePage from './pages/HomePage'
import BooksPage from './pages/BooksPage'
import BookDetailsPage from './pages/BookDetailsPage'
import CartPage from './pages/CartPage'
import PaymentPage from './pages/PaymentPage'
import WishlistPage from './pages/WishlistPage'
import LibraryPage from './pages/LibraryPage'
import OrdersPage from './pages/OrdersPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import NotFoundPage from './pages/NotFoundPage'
import './App.css'

function App() {
    const [menuOpen, setMenuOpen] = useState(false)
    return (
        <AuthProvider>
            <div className="app-shell">
                <Header onMenuClick={() => setMenuOpen((open) => !open)} />
                <div className="app-body">
                    <Sidebar open={menuOpen} onNavigate={() => setMenuOpen(false)} />
                    <main className="page-content">
                        <Routes>
                            <Route path="/" element={<HomePage />} />
                            <Route path="/books" element={<BooksPage />} />
                            <Route path="/books/:bookId" element={<BookDetailsPage />} />
                            <Route path="/login" element={<LoginPage />} />
                            <Route path="/register" element={<RegisterPage />} />
                            <Route element={<ProtectedRoute />}>
                                <Route path="/cart" element={<CartPage />} />
                                <Route path="/payment/batch/:purchaseIds" element={<PaymentPage />} />
                                <Route path="/payment/:purchaseId" element={<PaymentPage />} />
                                <Route path="/wishlist" element={<WishlistPage />} />
                                <Route path="/library" element={<LibraryPage />} />
                                <Route path="/orders" element={<OrdersPage />} />
                            </Route>
                            <Route path="/not-found" element={<NotFoundPage />} />
                            <Route path="*" element={<Navigate to="/not-found" replace />} />
                        </Routes>
                    </main>
                </div>
            </div>
        </AuthProvider>
    )
}

export default App