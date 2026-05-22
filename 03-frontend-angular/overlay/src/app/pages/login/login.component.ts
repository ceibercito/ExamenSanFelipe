import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  username = 'admin';
  password = 'admin123';
  cargando = signal(false);
  error = signal<string | null>(null);

  ingresar(): void {
    this.cargando.set(true);
    this.error.set(null);
    this.auth.login(this.username, this.password).subscribe({
      next: (res) => {
        this.cargando.set(false);
        if (res.success) this.router.navigate(['/kardex']);
        else this.error.set(res.message ?? 'Error de autenticación');
      },
      error: (err) => {
        this.cargando.set(false);
        this.error.set(err?.error?.message ?? 'No se pudo conectar con el servidor');
      },
    });
  }
}
