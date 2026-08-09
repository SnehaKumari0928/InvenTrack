import { X } from "lucide-react";

export default function Modal({ open, title, onClose, children, width = "max-w-lg" }) {
  if (!open) return null;
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-ink/45 p-4" onMouseDown={onClose}>
      <div className={`w-full ${width} max-h-[90vh] overflow-auto rounded-2xl bg-white shadow-2xl`} onMouseDown={(e) => e.stopPropagation()}>
        <div className="sticky top-0 flex items-center justify-between border-b border-line bg-white px-5 py-4">
          <h2 className="text-lg font-bold text-ink">{title}</h2>
          <button className="rounded-lg p-2 text-slate hover:bg-surface" onClick={onClose} aria-label="Close"><X size={19} /></button>
        </div>
        {children}
      </div>
    </div>
  );
}