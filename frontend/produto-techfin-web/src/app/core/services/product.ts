import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../../models/product.model';
import { CreateProductRequest } from '../../models/requests/create-product.request';
import { UpdateProductRequest } from '../../models/requests/update-product.request';
import { DeleteBatchRequest } from '../../models/requests/delete-batch.request';
import { DeleteBatchResult } from '../../models/results/delete-batch.result';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly apiUrl = 'https://localhost:7205/api/v1/products';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl);
  }

  getById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(this.apiUrl, request);
  }

  update(id: string, request: UpdateProductRequest): Observable<Product> {
    return this.http.put<Product>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  deleteBatch(request: DeleteBatchRequest): Observable<DeleteBatchResult> {
    return this.http.delete<DeleteBatchResult>(`${this.apiUrl}/batch`, {
      body: request
    });
  }
}