import { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { Boxes, Eye, EyeOff } from "lucide-react";
import { useAuth } from "../context/AuthContext";
import { getErrorMessage } from "../services/api";

export default function Login() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [form, setForm] = useState({ email: "", password: "" });
  const [show, setShow] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const submit = async (e) => {
    e.preventDefault(); setError("");
    if (!form.email || !form.password) return setError("Enter your email and password.");
    try { setLoading(true); await login(form); navigate(location.state?.from?.pathname || "/dashboard", { replace: true }); }
    catch (err) { setError(getErrorMessage(err)); } finally { setLoading(false); }
  };

  return (
    <div className="grid min-h-screen lg:grid-cols-[1.05fr_.95fr]">
      <div className="hidden bg-navy p-10 text-white lg:flex lg:flex-col lg:justify-between">
        <div className="flex items-center gap-3"><div className="grid h-10 w-10 place-items-center rounded-lg bg-teal"><Boxes size={22}/></div><span className="text-xl font-bold">InvenTrack</span></div>
        <div className="max-w-lg"><div className="mb-4 text-sm font-semibold uppercase tracking-[.18em] text-teal">Inventory operations</div><h1 className="text-5xl font-bold leading-tight">Know what is in stock. Know what moves.</h1><p className="mt-5 max-w-md leading-7 text-white/60">A focused workspace for products, suppliers and customer orders — without the spreadsheet clutter.</p></div>
        <p className="text-xs text-white/35">Built for day-to-day warehouse work.</p>
      </div>
      <div className="flex items-center justify-center bg-surface p-5 sm:p-8">
        <div className="w-full max-w-md">
          <div className="mb-8 lg:hidden flex items-center gap-3"><div className="grid h-10 w-10 place-items-center rounded-lg bg-navy text-white"><Boxes size={21}/></div><span className="text-xl font-bold">InvenTrack</span></div>
          <div className="card p-6 sm:p-8">
            <h2 className="text-2xl font-bold">Welcome back</h2><p className="mt-1 text-sm text-slate">Sign in to your inventory workspace.</p>
            {error && <div className="mt-5 rounded-lg border border-red/20 bg-red/5 px-3.5 py-3 text-sm text-red">{error}</div>}
            <form onSubmit={submit} className="mt-6 space-y-4">
              <div><label className="label">Email</label><input className="input" type="email" value={form.email} onChange={e=>setForm({...form,email:e.target.value})} placeholder="you@company.com" autoComplete="email"/></div>
              <div><div className="mb-1.5 flex justify-between"><label className="label mb-0">Password</label></div><div className="relative"><input className="input pr-11" type={show?"text":"password"} value={form.password} onChange={e=>setForm({...form,password:e.target.value})} placeholder="••••••••" autoComplete="current-password"/><button type="button" onClick={()=>setShow(!show)} className="absolute right-2 top-1/2 -translate-y-1/2 rounded p-2 text-slate">{show?<EyeOff size={17}/>:<Eye size={17}/>}</button></div></div>
              <button disabled={loading} className="btn-primary w-full">{loading ? "Signing in..." : "Sign in"}</button>
            </form>
            <div className="mt-6 border-t border-line pt-5 text-center text-sm text-slate">New to InvenTrack? <Link className="font-semibold text-teal hover:underline" to="/register">Create a staff account</Link></div>
          </div>
        </div>
      </div>
    </div>
  );
}