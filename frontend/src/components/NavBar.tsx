import { NavLink } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export function NavBar() {
  const { name, logout } = useAuth();

  return (
    <header className="navbar">
      <nav>
        <NavLink to="/" end>
          Dashboard
        </NavLink>
        <NavLink to="/gastos">Gastos</NavLink>
        <NavLink to="/categorias">Categorías</NavLink>
      </nav>
      <div className="navbar-user">
        <span>{name}</span>
        <button type="button" onClick={logout}>
          Salir
        </button>
      </div>
    </header>
  );
}
