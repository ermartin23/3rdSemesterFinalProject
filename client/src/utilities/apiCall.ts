import {useCallback, useState} from "react";

export function apiCall<TArgs extends any[], TResponse>(
    apiFn: (...args: TArgs) => Promise<TResponse>
){
   const [isLoading, setIsLoading] = useState(false);
   const [error, setError] = useState<string | null>(null);
   
   const callApi = useCallback(
       async (...args: TArgs): Promise<TResponse | null> => {
           setIsLoading(true);
           setError(null);
           
           try {
               const result = await apiFn(...args);
               return result;
           } catch (e: any) {
               const message =
                   e?.message ?? "Unknown error occurred";
               setError(message);
               return null;
           } finally {
               setIsLoading(false);
           }
       },
       [apiFn]
   );
   return {callApi, isLoading, error};
}