import { NavLink } from 'react-router-dom'
import { useAuth } from '../context/useAuth'
import './Sidebar.css'

interface SidebarProps {
    open: boolean
    onNavigate: () => void
}

function Sidebar({ open, onNavigate }: SidebarProps) {
    const { token } = useAuth()
    return (
        <>
        {open && <button className="nav-scrim" aria-label="Close navigation" onClick={onNavigate} />}
        <nav className={`sidebar${open ? ' sidebar-open' : ''}`} aria-label="Main navigation">
            <p className="nav-label">Discover</p>
            <NavLink to="/" onClick={onNavigate} end>Home</NavLink>
            <NavLink to="/books" onClick={onNavigate}>Browse books</NavLink>
            {token && <>
                <p className="nav-label nav-label-spaced">Your shelves</p>
                <NavLink to="/library" onClick={onNavigate}>My library</NavLink>
                <NavLink to="/wishlist" onClick={onNavigate}>Wishlist</NavLink>
                <NavLink to="/cart" onClick={onNavigate}>Cart</NavLink>
                <p className="nav-label nav-label-spaced">Account</p>
                <NavLink to="/orders" onClick={onNavigate}>Orders &amp; payments</NavLink>
            </>}
            <div className="sidebar-note"><span>THE READING ROOM</span><p>A good book is a place you can return to.</p></div>
        </nav>
        </>
    );
}

export default Sidebar;