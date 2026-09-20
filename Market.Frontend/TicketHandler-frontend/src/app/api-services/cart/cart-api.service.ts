import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AddCartItemCommand, GetCartQueryDto, UpdateCartItemCommand } from './cart-api.model';

@Injectable({
  providedIn: 'root',
})
export class CartApiService {
  private readonly baseUrl = `${environment.apiUrl}/Cart`;
  private http = inject(HttpClient);

  //GET /Cart
    //The cart of the logged-in person (resolved from the token, no id needed)
  get(): Observable<GetCartQueryDto> {
    return this.http.get<GetCartQueryDto>(this.baseUrl);
  }
  //POST /Cart/items
  addItem(payload: AddCartItemCommand): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/items`, payload);
  }
  //PUT /Cart/items/{ticketId}
  updateItem(ticketId: number, payload: UpdateCartItemCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/items/${ticketId}`, payload);
  }
  //DELETE /Cart/items/{ticketId}
  removeItem(ticketId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/items/${ticketId}`);
  }
  //POST /Cart/items/{ticketId}/save-for-later
    //Parks the line: it keeps its quantity but leaves the cart totals and checkout
  saveForLater(ticketId: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/items/${ticketId}/save-for-later`, {});
  }
  //POST /Cart/items/{ticketId}/move-to-cart
  moveToCart(ticketId: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/items/${ticketId}/move-to-cart`, {});
  }
  //DELETE /Cart
  clear(): Observable<void> {
    return this.http.delete<void>(this.baseUrl);
  }
}
