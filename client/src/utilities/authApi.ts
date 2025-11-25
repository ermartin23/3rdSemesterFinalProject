import {AuthClient} from "../core/generated-client";
import {baseUrl} from "../core/baseUrl";
import {customFetch} from "./customFetch";

export const authApi = new AuthClient(baseUrl, customFetch);