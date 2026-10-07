import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  email = '';
  password = '';
  mensaje = '';
  cargando = false;
  loginCorrecto = false;

  usuario: any = null;

  private apiUrl = 'http://localhost:5037/api/Auth/login';

  constructor(private http: HttpClient) {}

  iniciarSesion() {
    this.mensaje = '';
    this.cargando = true;

    const datos = {
      email: this.email,
      password: this.password
    };

    this.http.post<any>(this.apiUrl, datos).subscribe({
      next: (respuesta) => {
        localStorage.setItem('token', respuesta.token);
        localStorage.setItem('usuario', JSON.stringify(respuesta.user));

        this.usuario = respuesta.user;
        this.loginCorrecto = true;
        this.cargando = false;
      },
      error: () => {
        this.mensaje = 'Correo o contraseña incorrectos';
        this.cargando = false;
      }
    });
  }

  cerrarSesion() {
    localStorage.removeItem('token');
    localStorage.removeItem('usuario');

    this.loginCorrecto = false;
    this.usuario = null;
    this.email = '';
    this.password = '';
  }
}