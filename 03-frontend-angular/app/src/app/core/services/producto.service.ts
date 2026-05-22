import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Producto, ProductoCreate } from '../models/models';

@Injectable({ providedIn: 'root' })
export class ProductoService {
  private http = inject(HttpClient);
  private base = `${environment.comprasApi}/productos`;

  listar(): Observable<ApiResponse<Producto[]>> {
    return this.http.get<ApiResponse<Producto[]>>(this.base);
  }

  registrar(dto: ProductoCreate): Observable<ApiResponse<Producto>> {
    return this.http.post<ApiResponse<Producto>>(this.base, dto);
  }

  actualizar(id: number, dto: ProductoCreate & { idProducto: number }): Observable<ApiResponse<Producto>> {
    return this.http.put<ApiResponse<Producto>>(`${this.base}/${id}`, dto);
  }
}
