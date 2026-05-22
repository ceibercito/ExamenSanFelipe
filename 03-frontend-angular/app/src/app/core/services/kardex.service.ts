import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Kardex, Movimiento } from '../models/models';

@Injectable({ providedIn: 'root' })
export class KardexService {
  private http = inject(HttpClient);
  private base = `${environment.movimientosApi}/kardex`;

  listar(): Observable<ApiResponse<Kardex[]>> {
    return this.http.get<ApiResponse<Kardex[]>>(this.base);
  }

  movimientos(idProducto: number): Observable<ApiResponse<Movimiento[]>> {
    return this.http.get<ApiResponse<Movimiento[]>>(`${this.base}/${idProducto}/movimientos`);
  }
}
