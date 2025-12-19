import {AuthClient} from "../core/generated-client";
import { baseUrl } from "../core/config";
import {customFetch} from "./customFetch";

export const authApi = new AuthClient(baseUrl, customFetch);