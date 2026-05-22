import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, VentaCreate } from '../models/models';

@Injectable({ providedIn: 'root' })
export class VentaService {
  private http = inject(HttpClient);
  private base = `${environment.ventasApi}/ventas`;

  registrar(dto: VentaCreate): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(this.base, dto);
  }

  listar(): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(this.base);
  }
}
