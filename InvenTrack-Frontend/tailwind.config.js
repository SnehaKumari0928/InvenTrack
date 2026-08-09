/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,jsx}"],
  theme: {
    extend: {
      colors: {
        ink: "#172033",
        slate: "#526070",
        line: "#E5E7EB",
        surface: "#F7F8FA",
        navy: "#12304A",
        teal: "#087E8B",
        tealDark: "#07636D",
        amber: "#D97706",
        red: "#C2413B"
      },
      boxShadow: {
        soft: "0 8px 30px rgba(23, 32, 51, 0.07)"
      }
    }
  },
  plugins: []
};