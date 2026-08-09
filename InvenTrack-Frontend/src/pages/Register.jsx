import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Boxes } from "lucide-react";
import { useAuth } from "../context/AuthContext";
import { getErrorMessage } from "../services/api";

export default function Register() {
  const { register } = useAuth(); const navigate = useNavigate();
  const [form, setForm] = useState({ username:"", email:"", password:"", confirm:"" });
  const [error,setError]=useState(""); const [loading,setLoading]=useState(false);
  const submit=async(e)=>{
    e.preventDefault(); setError("");
    if(form.username.trim().length<2) return setError("Username must be at least 2 characters.");
    if(form.password.length<6) return setError("Password must be at least 6 characters.");
    if(form.password!==form.confirm) return setError("Passwords do not match.");
    try{setLoading(true);await register({username:form.username.trim(),email:form.email.trim(),password:form.password});navigate("/dashboard",{replace:true});}
    catch(err){setError(getErrorMessage(err));}finally{setLoading(false);}
  };
  return <div className="min-h-screen bg-surface flex items-center justify-center p-5 sm:p-8">
    <div className="w-full max-w-md">
      <div className="mb-6 flex items-center justify-center gap-3"><div className="grid h-10 w-10 place-items-center rounded-lg bg-navy text-white"><Boxes size={21}/></div><span className="text-xl font-bold">InvenTrack</span></div>
      <div className="card p-6 sm:p-8">
        <h1 className="text-2xl font-bold">Create your account</h1><p className="mt-1 text-sm text-slate">New registrations are created as Staff.</p>
        {error&&<div className="mt-5 rounded-lg border border-red/20 bg-red/5 px-3.5 py-3 text-sm text-red">{error}</div>}
        <form onSubmit={submit} className="mt-6 space-y-4">
          <div><label className="label">Username</label><input className="input" value={form.username} onChange={e=>setForm({...form,username:e.target.value})} placeholder="Alex Morgan"/></div>
          <div><label className="label">Email</label><input className="input" type="email" value={form.email} onChange={e=>setForm({...form,email:e.target.value})} placeholder="alex@company.com"/></div>
          <div><label className="label">Password</label><input className="input" type="password" value={form.password} onChange={e=>setForm({...form,password:e.target.value})} placeholder="At least 6 characters"/></div>
          <div><label className="label">Confirm password</label><input className="input" type="password" value={form.confirm} onChange={e=>setForm({...form,confirm:e.target.value})} placeholder="Repeat password"/></div>
          <button disabled={loading} className="btn-primary w-full">{loading?"Creating account...":"Create account"}</button>
        </form>
        <div className="mt-6 border-t border-line pt-5 text-center text-sm text-slate">Already have an account? <Link className="font-semibold text-teal hover:underline" to="/login">Sign in</Link></div>
      </div>
    </div>
  </div>;
}