import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductoService } from '../../core/services/producto.service';
import { VentaService } from '../../core/services/venta.service';
import { Producto } from '../../core/models/models';

interface LineaVenta {
  idProducto: number;
  nombre: string;
  cantidad: number;
  precioVenta: number;
  stock: number;
}

@Component({
  selector: 'app-ventas',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ventas.component.html',
})
export class VentasComponent implements OnInit {
  private productoSrv = inject(ProductoService);
  private ventaSrv = inject(VentaService);

  productos = signal<Producto[]>([]);
  lineas = signal<LineaVenta[]>([]);
  mensaje = signal<{ tipo: 'success' | 'danger'; texto: string } | null>(null);

  // selProductoId es una SEÑAL para que el computed productoSel reaccione al cambio del dropdown.
  selProductoId = signal(0);
  selCantidad = 1;

  // Producto seleccionado (para mostrar precio y stock en pantalla)
  productoSel = computed(() =>
    this.productos().find(p => p.idProducto === this.selProductoId()) ?? null);

  subTotal = computed(() => this.lineas().reduce((s, l) => s + l.cantidad * l.precioVenta, 0));
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
    const prod = this.productoSel();
    if (!prod) {
      this.mensaje.set({ tipo: 'danger', texto: 'Seleccione un producto.' });
      return;
    }
    // Validación de stock (también se valida en el backend)
    const yaEnLista = this.lineas()
      .filter(l => l.idProducto === prod.idProducto)
      .reduce((s, l) => s + l.cantidad, 0);
    if (this.selCantidad + yaEnLista > prod.stockActual) {
      this.mensaje.set({ tipo: 'danger', texto: `La cantidad no debe ser mayor al stock disponible (${prod.stockActual}).` });
      return;
    }

    this.lineas.update(ls => [...ls, {
      idProducto: prod.idProducto,
      nombre: prod.nombreProducto,
      cantidad: +this.selCantidad,
      precioVenta: prod.precioVenta,
      stock: prod.stockActual,
    }]);
    this.selProductoId.set(0);
    this.selCantidad = 1;
    this.mensaje.set(null);
  }

  quitarLinea(i: number): void {
    this.lineas.update(ls => ls.filter((_, idx) => idx !== i));
  }

  guardarVenta(): void {
    if (this.lineas().length === 0) {
      this.mensaje.set({ tipo: 'danger', texto: 'Agregue al menos un producto.' });
      return;
    }
    const dto = {
      detalles: this.lineas().map(l => ({ idProducto: l.idProducto, cantidad: l.cantidad })),
    };
    this.ventaSrv.registrar(dto).subscribe({
      next: (res) => {
        if (res.success) {
          this.mensaje.set({ tipo: 'success', texto: `Venta #${res.data} registrada. Movimiento de salida generado.` });
          this.lineas.set([]);
          this.cargarProductos();
        } else {
          this.mensaje.set({ tipo: 'danger', texto: res.message ?? 'Error al registrar.' });
        }
      },
      error: (err) => this.mensaje.set({ tipo: 'danger', texto: err?.error?.message ?? 'Error de servidor' }),
    });
  }
}
