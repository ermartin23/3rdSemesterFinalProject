const isProduction = import.meta.env.PROD;

const prod = "https://3rdsemesterfinalproject.fly.dev";
const dev = "https://localhost:5284";
export const baseUrl = isProduction ? prod : dev;