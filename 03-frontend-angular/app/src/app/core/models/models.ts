// Respuesta estándar del backend
export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
}

export interface LoginResponse {
  token: string;
  username: string;
  nombre: string;
  expiraEn: string;
}

export interface Producto {
  idProducto: number;
  nombreProducto: string;
  nroLote?: string;
  costo: number;
  precioVenta: number;
  stockActual: number;
}

export interface ProductoCreate {
  nombreProducto: string;
  nroLote?: string;
  costo: number;
  precioVenta: number;
}

export interface CompraDetItem {
  idProducto: number;
  cantidad: number;
  precio: number;
}

export interface CompraCreate {
  detalles: CompraDetItem[];
}

export interface VentaDetItem {
  idProducto: number;
  cantidad: number;
}

export interface VentaCreate {
  detalles: VentaDetItem[];
}

export interface Kardex {
  idProducto: number;
  nombreProducto: string;
  stockActual: number;
  costo: number;
  precioVenta: number;
}

export interface Movimiento {
  fechaRegistro: string;
  tipoMovimiento: string;
  cantidad: number;
  documentoOrigen: number;
}
