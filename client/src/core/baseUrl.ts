import {AuthClient} from "./generated-client.ts";

// @ts-ignore
const isProduction = import.meta.env.PROD;

const prod = "https://deadpigeons.fly.dev";
const dev = "http://localhost:5173";

export const finalUrl = isProduction ? prod : dev;

export const todoClient = new AuthClient(finalUrl)