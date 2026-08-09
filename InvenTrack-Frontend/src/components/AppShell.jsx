import { useState } from "react";
import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { BarChart3, Boxes, ClipboardList, Menu, Package, Truck, Users, X, LogOut } from "lucide-react";
import { useAuth } from "../context/AuthContext";

const nav = [
  { to: "/dashboard", label: "Overview", icon: BarChart3 },
  { to: "/products", label: "Products", icon: Package },
  { to: "/suppliers", label: "Suppliers", icon: Truck, admin: true },
  { to: "/orders/new", label: "Create order", icon: ClipboardList, staff: true },
  { to: "/users", label: "Users", icon: Users, admin: true }
];

export default function AppShell() {
  const [open, setOpen] = useState(false);
  const { user, isAdmin, logout } = useAuth();
  const navigate = useNavigate();

  const doLogout = async () => { await logout(); navigate("/login", { replace: true }); };

  return (
    <div className="min-h-screen bg-surface">
      {open && <div className="fixed inset-0 z-30 bg-ink/35 lg:hidden" onClick={() => setOpen(false)} />}
      <aside className={`fixed inset-y-0 left-0 z-40 flex w-64 flex-col border-r border-line bg-navy text-white transition-transform lg:translate-x-0 ${open ? "translate-x-0" : "-translate-x-full"}`}>
        <div className="flex h-16 items-center justify-between border-b border-white/10 px-5">
          <div className="flex items-center gap-3">
            <div className="grid h-9 w-9 place-items-center rounded-lg bg-teal text-white"><Boxes size={20} /></div>
            <div><div className="font-bold tracking-tight">InvenTrack</div><div className="text-[10px] uppercase tracking-[.18em] text-white/45">Inventory desk</div></div>
          </div>
          <button className="rounded-lg p-1 text-white/60 hover:bg-white/10 lg:hidden" onClick={() => setOpen(false)}><X size={20} /></button>
        </div>

        <nav className="flex-1 space-y-1 px-3 py-5">
          <p className="px-3 pb-2 text-[11px] font-semibold uppercase tracking-wider text-white/35">Workspace</p>
          {nav.filter(item => !item.admin || isAdmin).filter(item => !item.staff || !isAdmin || user).map(({ to, label, icon: Icon }) => (
            <NavLink key={to} to={to} onClick={() => setOpen(false)}
              className={({ isActive }) => `flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition ${isActive ? "bg-white/10 text-white" : "text-white/65 hover:bg-white/5 hover:text-white"}`}>
              <Icon size={18} strokeWidth={1.8} />{label}
            </NavLink>
          ))}
        </nav>

        <div className="border-t border-white/10 p-3">
          <div className="mb-2 flex items-center gap-3 rounded-lg bg-white/5 p-3">
            <div className="grid h-9 w-9 shrink-0 place-items-center rounded-full bg-teal text-sm font-bold">{(user?.username || "U").slice(0,1).toUpperCase()}</div>
            <div className="min-w-0"><div className="truncate text-sm font-semibold">{user?.username}</div><div className="text-xs text-white/45">{isAdmin ? "Administrator" : "Staff"}</div></div>
          </div>
          <button onClick={doLogout} className="flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-sm text-white/60 hover:bg-white/5 hover:text-white"><LogOut size={18} /> Sign out</button>
        </div>
      </aside>

      <main className="min-h-screen lg:pl-64">
        <header className="sticky top-0 z-20 flex h-16 items-center justify-between border-b border-line bg-white/95 px-4 backdrop-blur sm:px-6">
          <button className="rounded-lg p-2 text-slate hover:bg-surface lg:hidden" onClick={() => setOpen(true)}><Menu size={21} /></button>
          <div className="hidden lg:block text-sm text-slate">Warehouse operations / <span className="font-medium text-ink">{isAdmin ? "Admin workspace" : "Staff workspace"}</span></div>
          <div className="ml-auto flex items-center gap-3">
            <span className="hidden rounded-full bg-teal/10 px-2.5 py-1 text-xs font-semibold text-teal sm:inline-flex">API connected</span>
            <span className="text-sm font-semibold text-ink">{user?.username}</span>
          </div>
        </header>
        <div className="mx-auto max-w-[1500px] p-4 sm:p-6 lg:p-8"><Outlet /></div>
      </main>
    </div>
  );
}