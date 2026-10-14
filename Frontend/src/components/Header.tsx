import { useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/useAuth'
import { getCart } from '../utils/cart'
import './Header.css'

interface HeaderProps {
  onMenuClick: () => void
}

function Header({ onMenuClick }: HeaderProps) {
    const { user, logout } = useAuth()
    const navigate = useNavigate()
    const [cartCount, setCartCount] = useState(() => getCart(user?.id || undefined).length)
    useEffect(() => {
        const updateCart = () => setCartCount(getCart(user?.id || undefined).length)
        updateCart()
        window.addEventListener('zbooks-cart-change', updateCart)
        window.addEventListener('storage', updateCart)
        return () => {
            window.removeEventListener('zbooks-cart-change', updateCart)
            window.removeEventListener('storage', updateCart)
        }
    }, [user?.id])
    return (
        <header className="topbar">
            <button className="icon-button menu-toggle" aria-label="Open navigation" onClick={onMenuClick}>☰</button>
            <Link className="brand" to="/" aria-label="ZBOOKS home"><span className="brand-mark">Z</span><span>ZBOOKS</span></Link>
            <span className="topbar-note">Stories for the curious</span>
            <div className="topbar-actions">
                {user ? <><span className="welcome-user">{user.email}</span><button className="text-button" onClick={() => { logout(); navigate('/') }}>Sign out</button></> : <Link className="text-button" to="/login">Sign in</Link>}
                <Link className="bag-link" to="/cart" aria-label={`Cart, ${cartCount} books`}>Cart <span className="bag-count">{cartCount}</span></Link>
            </div>
        </header>
    );
}

export default Header;