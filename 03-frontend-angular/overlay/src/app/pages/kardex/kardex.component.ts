import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { KardexService } from '../../core/services/kardex.service';
import { Kardex, Movimiento } from '../../core/models/models';

@Component({
  selector: 'app-kardex',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './kardex.component.html',
})
export class KardexComponent implements OnInit {
  private kardexSrv = inject(KardexService);

  items = signal<Kardex[]>([]);
  cargando = signal(false);

  // Modal de movimientos
  modalAbierto = signal(false);
  productoSel = signal<Kardex | null>(null);
  movimientos = signal<Movimiento[]>([]);

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.cargando.set(true);
    this.kardexSrv.listar().subscribe({
      next: (res) => {
        if (res.success && res.data) this.items.set(res.data);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false),
    });
  }

  verMovimientos(item: Kardex): void {
    this.productoSel.set(item);
    this.modalAbierto.set(true);
    this.movimientos.set([]);
    this.kardexSrv.movimientos(item.idProducto).subscribe(res => {
      if (res.success && res.data) this.movimientos.set(res.data);
    });
  }

  cerrarModal(): void {
    this.modalAbierto.set(false);
    this.productoSel.set(null);
  }
}
