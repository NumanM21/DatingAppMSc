// Export function so we can use it in other components
import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs";
import { ResultPaginated } from "../_models/Pagination";


// Need full HTTP reponse, not just body, so we can access headers so we can get pagination info
// <T> for reusability

export function getResultPagination<T>
(URL: string, params: HttpParams, httpClient: HttpClient) {

  const resultPaginated: ResultPaginated<T> = new ResultPaginated<T>;

  return httpClient.get<T>(URL, { observe: 'response', params }).pipe(
    map(response => {
      if (response.body) {
        resultPaginated.result = response.body;
      }

      // this is the header we want from our postman response
      const pagin = response.headers.get('Pagination');
      if (pagin) {
        resultPaginated.pagination = JSON.parse(pagin); // to convert serialized json into object (pagination is a string, so we need to convert it to an object)!
      }

      return resultPaginated;
    })
  );
}

// Helper method to get pagination info from parameterUser class
export function getHeadPagination
(pageNumber: number, pageSize: number) {

  let params = new HttpParams();

  params = params.append('pageNumber', pageNumber);
  params = params.append('pageSize', pageSize);

  return params;
}