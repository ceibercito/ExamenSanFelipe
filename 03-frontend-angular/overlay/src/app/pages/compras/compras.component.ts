import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductoService } from '../../core/services/producto.service';
import { CompraService } from '../../core/services/compra.service';
import { Producto } from '../../core/models/models';

interface LineaCompra {
  idProducto: number;
  nombre: string;
  cantidad: number;
  precio: number;
}

@Component({
  selector: 'app-compras',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './compras.component.html',
})
export class ComprasComponent implements OnInit {
  private productoSrv = inject(ProductoService);
  private compraSrv = inject(CompraService);

  productos = signal<Producto[]>([]);
  lineas = signal<LineaCompra[]>([]);
  mensaje = signal<{ tipo: 'success' | 'danger'; texto: string } | null>(null);

  // Selección de línea actual
  selProductoId = 0;
  selCantidad = 1;
  selPrecio = 0;

  // Modal nuevo producto
  modalAbierto = signal(false);
  nuevoNombre = '';
  nuevoLote = '';
  nuevoCosto = 0;

  // Totales calculados
  subTotal = computed(() => this.lineas().reduce((s, l) => s + l.cantidad * l.precio, 0));
  igv = computed(() => this.subTotal() * 0.18);
  total = computed(() => this.subTotal() + this.igv());

  ngOnInit(): void {
    this.cargarProductos();
  }

  cargarProductos(): void {
    this.productoSrv.listar().subscribe(res => {
      if (res.success && res.data) this.productos.set(res.data);
    });
  }

  agregarLinea(): void {
    if (!this.selProductoId || this.selCantidad <= 0 || this.selPrecio <= 0) {
      this.mensaje.set({ tipo: 'danger', texto: 'Seleccione producto, cantidad y precio válidos.' });
      return;
    }
    const prod = this.productos().find(p => p.idProducto === +this.selProductoId);
    if (!prod) return;

    this.lineas.update(ls => [...ls, {
      idProducto: prod.idProducto,
      nombre: prod.nombreProducto,
      cantidad: +this.selCantidad,
      precio: +this.selPrecio,
    }]);
    this.selProductoId = 0;
    this.selCantidad = 1;
    this.selPrecio = 0;
    this.mensaje.set(null);
  }

  quitarLinea(i: number): void {
    this.lineas.update(ls => ls.filter((_, idx) => idx !== i));
  }

  guardarCompra(): void {
    if (this.lineas().length === 0) {
      this.mensaje.set({ tipo: 'danger', texto: 'Agregue al menos un producto.' });
      return;
    }
    const dto = {
      detalles: this.lineas().map(l => ({
        idProducto: l.idProducto, cantidad: l.cantidad, precio: l.precio,
      })),
    };
    this.compraSrv.registrar(dto).subscribe({
      next: (res) => {
        if (res.success) {
          this.mensaje.set({ tipo: 'success', texto: `Compra #${res.data} registrada. Stock y precios actualizados.` });
          this.lineas.set([]);
          this.cargarProductos();
        } else {
          this.mensaje.set({ tipo: 'danger', texto: res.message ?? 'Error al registrar.' });
        }
      },
      error: (err) => this.mensaje.set({ tipo: 'danger', texto: err?.error?.message ?? 'Error de servidor' }),
    });
  }

  // ---- Modal nuevo producto ----
  abrirModal(): void {
    this.nuevoNombre = ''; this.nuevoLote = ''; this.nuevoCosto = 0;
    this.modalAbierto.set(true);
  }
  cerrarModal(): void { this.modalAbierto.set(false); }

  guardarProducto(): void {
    if (!this.nuevoNombre.trim()) return;
    // PrecioVenta inicial = costo * 1.35 (se recalcula al comprar igualmente)
    this.productoSrv.registrar({
      nombreProducto: this.nuevoNombre,
      nroLote: this.nuevoLote,
      costo: +this.nuevoCosto,
      precioVenta: +(this.nuevoCosto * 1.35).toFixed(2),
    }).subscribe(res => {
      if (res.success) {
        this.cargarProductos();
        this.cerrarModal();
        this.mensaje.set({ tipo: 'success', texto: 'Producto registrado. Ya puede seleccionarlo.' });
      }
    });
  }
}
