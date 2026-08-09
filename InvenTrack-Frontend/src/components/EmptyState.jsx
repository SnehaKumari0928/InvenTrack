import { PackageOpen } from "lucide-react";

export default function EmptyState({ title = "Nothing here yet", description = "There are no records to display." }) {
  return (
    <div className="flex flex-col items-center justify-center px-6 py-16 text-center">
      <div className="mb-4 rounded-full bg-surface p-4 text-slate"><PackageOpen size={25} /></div>
      <h3 className="font-semibold text-ink">{title}</h3>
      <p className="mt-1 max-w-sm text-sm text-slate">{description}</p>
    </div>
  );
}