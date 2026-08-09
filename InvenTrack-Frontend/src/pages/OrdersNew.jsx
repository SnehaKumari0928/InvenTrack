
import { useEffect, useMemo, useState } from "react";
import {
  ArrowLeft,
  Minus,
  Plus,
  ShoppingCart,
  Trash2,
} from "lucide-react";
import { Link, useNavigate } from "react-router-dom";

import { productApi, orderApi, getErrorMessage } from "../services/api";
import Loading from "../components/Loading";

const money = new Intl.NumberFormat("en-IN", {
  style: "currency",
  currency: "INR",
  maximumFractionDigits: 2,
});

export default function OrdersNew() {
  const navigate = useNavigate();

  const [products, setProducts] = useState([]);
  const [items, setItems] = useState([]);

  const [customer, setCustomer] = useState({
    name: "",
    email: "",
  });

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState("");
  const [query, setQuery] = useState("");

  // Load products
  useEffect(() => {
    const loadProducts = async () => {
      try {
        setLoading(true);

        const response = await productApi.list();

        setProducts(response.data);
      } catch (err) {
        setError(getErrorMessage(err));
      } finally {
        setLoading(false);
      }
    };

    loadProducts();
  }, []);

  // Search products
  const filteredProducts = products.filter((product) =>
    `${product.name} ${product.sku}`
      .toLowerCase()
      .includes(query.toLowerCase())
  );

  // Calculate total
  const total = useMemo(() => {
    return items.reduce(
      (sum, item) => sum + item.price * item.quantity,
      0
    );
  }, [items]);

  // Add product
  const addProduct = (product) => {
    if (product.stockQuantity < 1) {
      return;
    }

    setItems((currentItems) => {
      const existingItem = currentItems.find(
        (item) => item.productId === product.id
      );

      // Product already exists in order
      if (existingItem) {
        return currentItems.map((item) =>
          item.productId === product.id
            ? {
                ...item,
                quantity: Math.min(
                  item.quantity + 1,
                  product.stockQuantity
                ),
              }
            : item
        );
      }

      // New product
      return [
        ...currentItems,
        {
          productId: product.id,
          name: product.name,
          sku: product.sku,
          price: Number(product.price),
          quantity: 1,
          max: product.stockQuantity,
        },
      ];
    });
  };

  // Change quantity
  const changeQuantity = (productId, change) => {
    setItems((currentItems) =>
      currentItems.map((item) => {
        if (item.productId !== productId) {
          return item;
        }

        return {
          ...item,
          quantity: Math.max(
            1,
            Math.min(item.max, item.quantity + change)
          ),
        };
      })
    );
  };

  // Remove product
  const removeProduct = (productId) => {
    setItems((currentItems) =>
      currentItems.filter((item) => item.productId !== productId)
    );
  };

  // Submit order
  const submitOrder = async () => {
    setError("");

    // Validation
    if (!customer.name.trim()) {
      setError("Customer name is required.");
      return;
    }

    if (
      customer.email &&
      !/^\S+@\S+\.\S+$/.test(customer.email)
    ) {
      setError("Enter a valid customer email.");
      return;
    }

    if (items.length === 0) {
      setError("Add at least one product.");
      return;
    }

    try {
      setSubmitting(true);

      const payload = {
        customerName: customer.name.trim(),
        customerEmail: customer.email.trim(),

        items: items.map((item) => ({
          productId: item.productId,
          quantity: item.quantity,
        })),
      };

      console.log("Submitting order:", payload);

      const response = await orderApi.create(payload);

      console.log("Order response:", response.data);

      // Backend should return the created order
      // with an ID.
      navigate(`/orders/${response.data.id}`);
    } catch (err) {
      console.error("Order creation failed:", err);

      setError(getErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return <Loading label="Loading products..." />;
  }

  return (
    <div>
      {/* Header */}
      <Link
        to="/dashboard"
        className="inline-flex items-center gap-2 text-sm font-semibold text-slate hover:text-teal"
      >
        <ArrowLeft size={16} />
        Back
      </Link>

      <div className="mt-5">
        <h1 className="page-title">Create order</h1>

        <p className="mt-1 text-sm text-slate">
          Choose products, confirm quantities and submit the order.
        </p>
      </div>

      {/* Error */}
      {error && (
        <div className="mt-5 rounded-lg bg-red/5 p-3 text-sm text-red">
          {error}
        </div>
      )}

      <div className="mt-6 grid gap-6 xl:grid-cols-[1fr_420px]">
        {/* PRODUCTS */}
        <section className="card overflow-hidden">
          <div className="border-b border-line p-5">
            <div className="flex items-center justify-between gap-4">
              <h2 className="font-bold">Available products</h2>

              <span className="text-xs text-slate">
                {products.length} items
              </span>
            </div>

            <input
              className="input mt-4"
              placeholder="Search products..."
              value={query}
              onChange={(e) => setQuery(e.target.value)}
            />
          </div>

          <div className="max-h-[580px] divide-y divide-line overflow-y-auto">
            {filteredProducts.map((product) => (
              <div
                key={product.id}
                className="flex items-center gap-4 p-4 hover:bg-surface/60"
              >
                {/* Icon */}
                <div className="grid h-10 w-10 shrink-0 place-items-center rounded-lg bg-teal/10 text-teal">
                  <ShoppingCart size={18} />
                </div>

                {/* Product information */}
                <div className="min-w-0 flex-1">
                  <div className="truncate text-sm font-semibold">
                    {product.name}
                  </div>

                  <div className="mt-0.5 text-xs text-slate">
                    {product.sku} · {money.format(product.price)}
                  </div>
                </div>

                {/* Stock */}
                <div
                  className={`text-right text-xs font-semibold ${
                    product.stockQuantity <= 10
                      ? "text-amber"
                      : "text-slate"
                  }`}
                >
                  {product.stockQuantity} in stock
                </div>

                {/* Add */}
                <button
                  type="button"
                  disabled={product.stockQuantity < 1}
                  className="rounded-lg border border-line p-2 text-teal hover:bg-teal/5"
                  onClick={() => addProduct(product)}
                >
                  <Plus size={17} />
                </button>
              </div>
            ))}

            {filteredProducts.length === 0 && (
              <div className="p-10 text-center text-sm text-slate">
                No matching products.
              </div>
            )}
          </div>
        </section>

        {/* ORDER SUMMARY */}
        <section className="card h-fit">
          <div className="border-b border-line p-5">
            <h2 className="font-bold">Order summary</h2>

            <p className="mt-0.5 text-xs text-slate">
              {items.length} line item
              {items.length !== 1 ? "s" : ""}
            </p>
          </div>

          <div className="p-5">
            {/* Selected products */}
            <div className="space-y-3">
              {items.length === 0 ? (
                <div className="rounded-lg bg-surface p-6 text-center text-sm text-slate">
                  Your order is empty.
                  <br />
                  Add products from the left.
                </div>
              ) : (
                items.map((item) => (
                  <div
                    key={item.productId}
                    className="rounded-lg border border-line p-3"
                  >
                    <div className="flex items-start gap-2">
                      <div className="min-w-0 flex-1">
                        <div className="truncate text-sm font-semibold">
                          {item.name}
                        </div>

                        <div className="text-xs text-slate">
                          {money.format(item.price)} each
                        </div>
                      </div>

                      <button
                        type="button"
                        onClick={() => removeProduct(item.productId)}
                        className="text-slate hover:text-red"
                      >
                        <Trash2 size={16} />
                      </button>
                    </div>

                    <div className="mt-3 flex items-center justify-between">
                      {/* Quantity */}
                      <div className="flex items-center rounded-lg border border-line">
                        <button
                          type="button"
                          onClick={() =>
                            changeQuantity(item.productId, -1)
                          }
                          className="p-1.5 text-slate hover:bg-surface"
                        >
                          <Minus size={15} />
                        </button>

                        <span className="w-8 text-center text-sm font-semibold">
                          {item.quantity}
                        </span>

                        <button
                          type="button"
                          onClick={() =>
                            changeQuantity(item.productId, 1)
                          }
                          className="p-1.5 text-slate hover:bg-surface"
                        >
                          <Plus size={15} />
                        </button>
                      </div>

                      <span className="text-sm font-bold">
                        {money.format(
                          item.price * item.quantity
                        )}
                      </span>
                    </div>
                  </div>
                ))
              )}
            </div>

            {/* Customer */}
            <div className="mt-5 border-t border-line pt-4">
              <div>
                <label className="label">Customer name</label>

                <input
                  className="input"
                  value={customer.name}
                  onChange={(e) =>
                    setCustomer({
                      ...customer,
                      name: e.target.value,
                    })
                  }
                  placeholder="Customer / company"
                />
              </div>

              <div className="mt-4">
                <label className="label">
                  Customer email{" "}
                  <span className="font-normal text-slate">
                    (optional)
                  </span>
                </label>

                <input
                  className="input"
                  type="email"
                  value={customer.email}
                  onChange={(e) =>
                    setCustomer({
                      ...customer,
                      email: e.target.value,
                    })
                  }
                  placeholder="customer@example.com"
                />
              </div>

              {/* Total */}
              <div className="mt-5 flex items-center justify-between">
                <span className="text-sm text-slate">
                  Order total
                </span>

                <span className="text-xl font-bold">
                  {money.format(total)}
                </span>
              </div>

              {/* Submit */}
              <button
                type="button"
                disabled={submitting || items.length === 0}
                onClick={submitOrder}
                className="btn-primary mt-4 w-full"
              >
                {submitting ? "Submitting..." : "Submit order"}
              </button>
            </div>
          </div>
        </section>
      </div>
    </div>
  );
}

