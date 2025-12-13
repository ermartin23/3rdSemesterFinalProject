// @ts-ignore
const isProduction = import.meta.env.PROD;

const prod = "https://deadpigeons.fly.dev";
const dev = "http://localhost:5239";

export const baseUrl = isProduction ? prod : dev;
