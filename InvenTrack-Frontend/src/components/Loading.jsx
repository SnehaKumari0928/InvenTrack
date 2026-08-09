export default function Loading({ label = "Loading..." }) {
  return (
    <div className="flex items-center justify-center gap-3 py-16 text-sm text-slate">
      <span className="h-5 w-5 animate-spin rounded-full border-2 border-slate/20 border-t-teal" />
      {label}
    </div>
  );
}