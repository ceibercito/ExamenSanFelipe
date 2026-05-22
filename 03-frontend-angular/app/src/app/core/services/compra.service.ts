import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, CompraCreate } from '../models/models';

@Injectable({ providedIn: 'root' })
export class CompraService {
  private http = inject(HttpClient);
  private base = `${environment.comprasApi}/compras`;

  registrar(dto: CompraCreate): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(this.base, dto);
  }

  listar(): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(this.base);
  }
}
