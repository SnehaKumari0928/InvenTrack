import { Navigate, Route, Routes } from "react-router-dom";
import { useAuth } from "./context/AuthContext";
import ProtectedRoute from "./components/ProtectedRoute";
import AppShell from "./components/AppShell";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Dashboard from "./pages/Dashboard";
import Products from "./pages/Products";
import ProductDetail from "./pages/ProductDetail";
import Suppliers from "./pages/Suppliers";
import OrdersNew from "./pages/OrdersNew";
import OrderDetail from "./pages/OrderDetail";
import Users from "./pages/Users";

export default function App(){
 const {isAuthenticated,isAdmin}=useAuth();
 return <Routes>
   <Route path="/login" element={isAuthenticated?<Navigate to="/dashboard" replace/>:<Login/>}/>
   <Route path="/register" element={isAuthenticated?<Navigate to="/dashboard" replace/>:<Register/>}/>
   <Route element={<ProtectedRoute/>}><Route element={<AppShell/>}>
     <Route path="/" element={<Navigate to="/dashboard" replace/>}/>
     <Route path="/dashboard" element={<Dashboard/>}/>
     <Route path="/products" element={<Products/>}/>
     <Route path="/products/:id" element={<ProductDetail/>}/>
     <Route path="/orders/new" element={<OrdersNew/>}/>
     <Route path="/orders/:id" element={<OrderDetail/>}/>
     <Route element={<ProtectedRoute adminOnly/>}>
       <Route path="/suppliers" element={<Suppliers/>}/>
       <Route path="/users" element={<Users/>}/>
     </Route>
   </Route></Route>
   <Route path="*" element={<Navigate to={isAuthenticated?"/dashboard":"/login"} replace/>}/>
 </Routes>;
}