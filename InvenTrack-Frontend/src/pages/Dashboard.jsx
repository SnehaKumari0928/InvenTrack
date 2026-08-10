import { useEffect, useMemo, useState } from "react";
import { ArrowRight, AlertTriangle, ClipboardList, Package, Plus, TrendingDown, Truck } from "lucide-react";
import { Link } from "react-router-dom";
import { productApi, supplierApi, getErrorMessage } from "../services/api";
import Loading from "../components/Loading";

const money = new Intl.NumberFormat("en-IN",{style:"currency",currency:"INR",maximumFractionDigits:2});

export default function Dashboard(){
  const [products,setProducts]=useState([]); 
  const [suppliers,setSuppliers]=useState([]); 
  const [loading,setLoading]=useState(true); 
  const [error,setError]=useState("");
  useEffect(()=>{Promise.all([productApi.list(),supplierApi.list()]).then(([p,s])=>{setProducts(p.data);setSuppliers(s.data)}).catch(e=>setError(getErrorMessage(e))).finally(()=>setLoading(false))},[]);
  const low=useMemo(()=>products.filter(p=>p.stockQuantity<=10),[products]);
  const stock=useMemo(()=>products.reduce((a,p)=>a+p.stockQuantity,0),[products]);
  const inventoryValue=useMemo(()=>products.reduce((a,p)=>a+p.stockQuantity*Number(p.price),0),[products]);
  if(loading)return <Loading label="Loading workspace..." />;
  return <div>
    <div className="flex flex-col justify-between gap-4 sm:flex-row sm:items-end">
      <div>
        <p className="text-sm font-medium text-teal">Good to see you
          </p>
          <h1 className="page-title mt-1">Operations overview
            </h1>
            <p className="mt-1 text-sm text-slate">A quick read on today's inventory position.
              </p>
              </div><Link to="/orders/new" className="btn-primary">
              <Plus size={17}/> 
              New order</Link>
              </div>
    {error&&<div className="mt-5 rounded-lg bg-red/5 p-3 text-sm text-red">{error}</div>}
    <div className="mt-7 grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
      <Stat icon={Package} label="Products" value={products.length} note="Active catalog"/>
      <Stat icon={Truck} label="Suppliers" value={suppliers.length} note="Linked vendors"/>
      <Stat icon={AlertTriangle} label="Low stock" value={low.length} note="10 units or less" danger={low.length>0}/>
      <Stat icon={TrendingDown} label="Units on hand" value={stock.toLocaleString("en-IN")} note={money.format(inventoryValue)+" inventory value"}/>
    </div>
    <div className="mt-6 grid gap-6 xl:grid-cols-[1.4fr_.8fr]">
      <section className="card">
        <div className="flex items-center justify-between border-b border-line px-5 py-4">
          <div>
            <h2 className="font-bold">Stock watch</h2>
            <p className="mt-0.5 text-xs text-slate">Products needing attention</p>
            </div>
            <Link to="/products?lowStock=10" className="text-sm font-semibold text-teal">View all</Link>
            </div>
        {low.length===0?<div className="px-5 py-12 text-center text-sm text-slate">No low-stock items right now.</div>
        :
        <div className="divide-y divide-line">{low.slice(0,5).map(p=><div key={p.id} className="flex items-center justify-between gap-4 px-5 py-4">
          <div className="min-w-0"><div className="truncate font-semibold">{p.name}</div>
          <div className="mt-0.5 text-xs text-slate">{p.sku} · {p.supplierName}</div>
          </div>
          <div className="shrink-0 rounded-full bg-amber/10 px-2.5 py-1 text-xs font-bold text-amber">{p.stockQuantity} left</div>
          </div>)}
          </div>}
      </section>
      <section className="card p-5"><h2 className="font-bold">Quick actions</h2>
      <div className="mt-4 space-y-2">
        <Quick to="/products" icon={Package} title="Manage products" text="Search, edit or inspect stock"/>
        <Quick to="/orders/new" icon={ClipboardList} title="Create an order" text="Reserve stock and record sale"/>
        <Quick to="/suppliers" icon={Truck} title="Manage suppliers" text="Maintain vendor contacts"/>
        </div>
        </section>
    </div>
  </div>;
}
function Stat({icon:Icon,label,value,note,danger})
{return <div className="card p-5">
  <div className="flex items-start justify-between">
    <div>
      <p className="text-sm text-slate">{label}</p>
      <p className="mt-2 text-2xl font-bold tracking-tight">{value}</p>
      </div>
      <div className={`rounded-lg p-2.5 ${danger?"bg-amber/10 text-amber":"bg-teal/10 text-teal"}`}>
      <Icon size={19}/>
      </div>
      </div>
      <p className="mt-3 text-xs text-slate">{note}</p>
      </div>}
function Quick({to,icon:Icon,title,text})
{return <Link to={to} className="group flex items-center gap-3 rounded-lg border border-line p-3 hover:border-teal/30 hover:bg-teal/[.03]">
  <div className="rounded-lg bg-surface p-2 text-teal">
    <Icon size={17}/>
    </div>
    <div className="min-w-0 flex-1">
      <div className="text-sm font-semibold">
        {title}
        </div>
        <div className="truncate text-xs text-slate">{text}</div>
        </div><ArrowRight size={16} className="text-slate/50 transition group-hover:translate-x-0.5 group-hover:text-teal"/>
        </Link>}