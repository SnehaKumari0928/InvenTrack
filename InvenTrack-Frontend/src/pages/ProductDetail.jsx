import { useEffect, useState } from "react";
import { ArrowLeft, Package, Tag, Truck } from "lucide-react";
import { Link, useParams } from "react-router-dom";
import { productApi, getErrorMessage } from "../services/api";
import Loading from "../components/Loading";

const money = new Intl.NumberFormat("en-IN",{style:"currency",currency:"INR",maximumFractionDigits:2});

export default function ProductDetail(){
 const {id}=useParams();const [p,setP]=useState(null),[loading,setLoading]=useState(true),[error,setError]=useState("");
 useEffect(()=>{productApi.get(id).then(r=>setP(r.data)).catch(e=>setError(getErrorMessage(e))).finally(()=>setLoading(false))},[id]);
 if(loading)return <Loading/>;
 if(error)return <div className="rounded-lg bg-red/5 p-4 text-sm text-red">{error}</div>;
 if(!p)return null;
 const low=p.stockQuantity<=10;
 return <div><Link to="/products" className="inline-flex items-center gap-2 text-sm font-semibold text-slate hover:text-teal"><ArrowLeft size={16}/> Back to products</Link>
  <div className="mt-5 grid gap-5 lg:grid-cols-[1fr_320px]"><section className="card p-6 sm:p-8"><div className="flex flex-col gap-5 sm:flex-row sm:items-start sm:justify-between"><div><div className="mb-3 inline-flex rounded-lg bg-teal/10 p-3 text-teal"><Package size={22}/></div><h1 className="text-3xl font-bold tracking-tight">{p.name}</h1><p className="mt-2 font-mono text-sm text-slate">{p.sku}</p></div><span className={`rounded-full px-3 py-1.5 text-sm font-bold ${low?"bg-amber/10 text-amber":"bg-teal/10 text-teal"}`}>{p.stockQuantity} units in stock</span></div>
  <div className="mt-8 grid gap-4 sm:grid-cols-3"><Info icon={Tag} label="Unit price" value={money.format(p.price)}/><Info icon={Package} label="Stock" value={p.stockQuantity.toLocaleString("en-IN")}/><Info icon={Truck} label="Supplier" value={p.supplierName}/></div></section>
  <section className="card p-6"><h2 className="font-bold">Stock status</h2><div className="mt-4 h-2 rounded-full bg-surface"><div className={`h-2 rounded-full ${low?"bg-amber":"bg-teal"}`} style={{width:`${Math.min(100,Math.max(8,p.stockQuantity))}%`}}/></div><p className="mt-3 text-sm text-slate">{low?"This item is at or below the low-stock threshold.":"Stock level is currently healthy."}</p></section></div></div>;
}
function Info({icon:Icon,label,value}){return <div className="rounded-xl bg-surface p-4"><div className="flex items-center gap-2 text-xs font-semibold uppercase tracking-wide text-slate"><Icon size={14}/>{label}</div><div className="mt-2 truncate text-lg font-bold">{value}</div></div>}