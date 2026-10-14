import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

// ==========================================
// INTERFACES
// ==========================================

interface Usuario {
  id: number;
  name: string;
  email: string;
  role: string;
}

interface RespuestaLogin {
  message: string;
  token: string;
  user: Usuario;
}

interface Alerta {
  id: number;
  lotCode: string;
  product: string;
  quantity: number;
  expirationDate: string;
  daysRemaining: number;
}

interface Dashboard {
  totalAlimentos: number;
  totalInventario: number;
  valorInventario: number;
  disponibles: number;
  prioritarios: number;
  vencidos: number;
  alertas: Alerta[];
}

interface NuevoAlimento {
  lotCode: string;
  product: string;
  quantity: number;
  unitPrice: number;
  expirationDate: string;
  foodType: string;
  responsible: string;
}

// ==========================================
// COMPONENTE
// ==========================================

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {

  // ========================================
  // API
  // ========================================

  private readonly apiBase =
    'http://localhost:5037/api';

  // ========================================
  // LOGIN
  // ========================================

  email = '';
  password = '';

  mensaje = '';
  cargando = false;

  loginCorrecto = false;

  usuario: Usuario | null = null;

  // ========================================
  // NAVEGACIÓN
  // ========================================

  seccionActiva = 'dashboard';

  // ========================================
  // DASHBOARD
  // ========================================

  dashboard: Dashboard | null = null;

  cargandoDashboard = false;

  errorDashboard = '';

  // ========================================
  // INVENTARIO / REGISTRO
  // ========================================

  mostrarFormularioRegistro = false;

  guardandoAlimento = false;

  mensajeInventario = '';

  nuevoAlimento: NuevoAlimento = {
    lotCode: '',
    product: '',
    quantity: 1,
    unitPrice: 0,
    expirationDate: '',
    foodType: '',
    responsible: ''
  };

  // ========================================
  // CONSTRUCTOR
  // ========================================

  constructor(
    private http: HttpClient
  ) {}

  // ========================================
  // INICIO
  // ========================================

  ngOnInit(): void {
    this.recuperarSesion();
  }

  // ========================================
  // LOGIN
  // ========================================

  iniciarSesion(): void {

    this.mensaje = '';

    if (
      !this.email.trim() ||
      !this.password
    ) {
      this.mensaje =
        'Ingresa tu correo y contraseña.';
      return;
    }

    this.cargando = true;

    const datos = {
      email: this.email.trim(),
      password: this.password
    };

    this.http
      .post<RespuestaLogin>(
        `${this.apiBase}/Auth/login`,
        datos
      )
      .subscribe({

        next: (respuesta) => {

          localStorage.setItem(
            'token',
            respuesta.token
          );

          localStorage.setItem(
            'usuario',
            JSON.stringify(
              respuesta.user
            )
          );

          this.usuario =
            respuesta.user;

          this.loginCorrecto =
            true;

          this.cargando =
            false;

          this.seccionActiva =
            'dashboard';

          this.cargarDashboard();
        },

        error: (error) => {

          console.error(
            'Error de inicio de sesión:',
            error
          );

          if (error.status === 401) {

            this.mensaje =
              'Correo o contraseña incorrectos.';

          } else {

            this.mensaje =
              'No se pudo conectar con el servidor.';

          }

          this.cargando =
            false;
        }

      });
  }

  // ========================================
  // RECUPERAR SESIÓN
  // ========================================

  recuperarSesion(): void {

    const token =
      localStorage.getItem(
        'token'
      );

    const usuarioGuardado =
      localStorage.getItem(
        'usuario'
      );

    if (
      !token ||
      !usuarioGuardado
    ) {
      return;
    }

    try {

      this.usuario =
        JSON.parse(
          usuarioGuardado
        );

      this.loginCorrecto =
        true;

      this.seccionActiva =
        'dashboard';

      this.cargarDashboard();

    } catch (error) {

      console.error(
        'No se pudo recuperar la sesión:',
        error
      );

      this.cerrarSesion();
    }
  }

  // ========================================
  // TOKEN / HEADERS
  // ========================================

  private obtenerHeaders(): HttpHeaders {

    const token =
      localStorage.getItem(
        'token'
      );

    return new HttpHeaders({
      Authorization:
        `Bearer ${token ?? ''}`
    });
  }

  // ========================================
  // CAMBIAR SECCIÓN
  // ========================================

  cambiarSeccion(
    seccion: string
  ): void {

    this.seccionActiva =
      seccion;

    this.mensajeInventario =
      '';

    this.mostrarFormularioRegistro =
      false;

    if (
      seccion === 'dashboard' ||
      seccion === 'alertas'
    ) {
      this.cargarDashboard();
    }
  }

  // ========================================
  // DASHBOARD
  // ========================================

  cargarDashboard(): void {

    const token =
      localStorage.getItem(
        'token'
      );

    if (!token) {

      this.cerrarSesion();

      return;
    }

    this.cargandoDashboard =
      true;

    this.errorDashboard =
      '';

    this.http
      .get<Dashboard>(
        `${this.apiBase}/Dashboard`,
        {
          headers:
            this.obtenerHeaders()
        }
      )
      .subscribe({

        next: (respuesta) => {

          this.dashboard =
            respuesta;

          this.cargandoDashboard =
            false;
        },

        error: (error) => {

          console.error(
            'Error al cargar Dashboard:',
            error
          );

          this.cargandoDashboard =
            false;

          if (
            error.status === 401 ||
            error.status === 403
          ) {

            this.errorDashboard =
              'Tu sesión ha expirado. Inicia sesión nuevamente.';

            setTimeout(
              () => {
                this.cerrarSesion();
              },
              1500
            );

          } else {

            this.errorDashboard =
              'No se pudo cargar la información del Dashboard.';
          }
        }

      });
  }

  // ========================================
  // ABRIR FORMULARIO DE ALIMENTO
  // ========================================

  abrirFormularioRegistro(): void {

    this.mensajeInventario =
      '';

    this.nuevoAlimento = {
      lotCode: '',
      product: '',
      quantity: 1,
      unitPrice: 0,
      expirationDate: '',
      foodType: '',
      responsible:
        this.usuario?.name ?? ''
    };

    this.mostrarFormularioRegistro =
      true;
  }

  // ========================================
  // CERRAR FORMULARIO
  // ========================================

  cerrarFormularioRegistro(): void {

    this.mostrarFormularioRegistro =
      false;

    this.mensajeInventario =
      '';
  }

  // ========================================
  // REGISTRAR ALIMENTO
  // ========================================

  registrarAlimento(): void {

    const token =
      localStorage.getItem(
        'token'
      );

    if (!token) {

      this.cerrarSesion();

      return;
    }

    // Validaciones

    if (
      !this.nuevoAlimento.lotCode.trim()
    ) {

      this.mensajeInventario =
        'El código de lote es obligatorio.';

      return;
    }

    if (
      !this.nuevoAlimento.product.trim()
    ) {

      this.mensajeInventario =
        'El nombre del producto es obligatorio.';

      return;
    }

    if (
      this.nuevoAlimento.quantity <= 0
    ) {

      this.mensajeInventario =
        'La cantidad debe ser mayor a 0.';

      return;
    }

    if (
      this.nuevoAlimento.unitPrice < 0
    ) {

      this.mensajeInventario =
        'El precio no puede ser negativo.';

      return;
    }

    if (
      !this.nuevoAlimento.expirationDate
    ) {

      this.mensajeInventario =
        'La fecha de vencimiento es obligatoria.';

      return;
    }

    if (
      !this.nuevoAlimento.foodType.trim()
    ) {

      this.mensajeInventario =
        'El tipo de alimento es obligatorio.';

      return;
    }

    this.guardandoAlimento =
      true;

    this.mensajeInventario =
      '';

    const fechaVencimiento =
      new Date(
        `${this.nuevoAlimento.expirationDate}T00:00:00Z`
      );

    const alimento = {

      lotCode:
        this.nuevoAlimento
          .lotCode
          .trim(),

      product:
        this.nuevoAlimento
          .product
          .trim(),

      quantity:
        Number(
          this.nuevoAlimento.quantity
        ),

      unitPrice:
        Number(
          this.nuevoAlimento.unitPrice
        ),

      expirationDate:
        fechaVencimiento.toISOString(),

      foodType:
        this.nuevoAlimento
          .foodType
          .trim(),

      responsible:
        this.nuevoAlimento
          .responsible
          .trim()
    };

    this.http
      .post(
        `${this.apiBase}/Foods`,
        alimento,
        {
          headers:
            this.obtenerHeaders()
        }
      )
      .subscribe({

        next: () => {

          this.guardandoAlimento =
            false;

          this.mostrarFormularioRegistro =
            false;

          this.mensajeInventario =
            'Alimento registrado correctamente.';

          // Actualizar estadísticas
          this.cargarDashboard();
        },

        error: (error) => {

          console.error(
            'Error al registrar alimento:',
            error
          );

          this.guardandoAlimento =
            false;

          if (
            error.status === 401
          ) {

            this.cerrarSesion();

            return;
          }

          this.mensajeInventario =
            error.error?.message ??
            'No se pudo registrar el alimento.';
        }

      });
  }

  // ========================================
  // CERRAR SESIÓN
  // ========================================

  cerrarSesion(): void {

    localStorage.removeItem(
      'token'
    );

    localStorage.removeItem(
      'usuario'
    );

    this.loginCorrecto =
      false;

    this.usuario =
      null;

    this.dashboard =
      null;

    this.seccionActiva =
      'dashboard';

    this.email =
      '';

    this.password =
      '';

    this.mensaje =
      '';

    this.errorDashboard =
      '';

    this.mensajeInventario =
      '';

    this.mostrarFormularioRegistro =
      false;

    this.guardandoAlimento =
      false;

    this.cargandoDashboard =
      false;
  }

  // ========================================
  // TEXTO DE ALERTAS
  // ========================================

  obtenerTextoDias(
    dias: number
  ): string {

    if (dias < 0) {
      return 'Producto vencido';
    }

    if (dias === 0) {
      return 'Vence hoy';
    }

    if (dias === 1) {
      return 'Vence mañana';
    }

    return `Vence en ${dias} días`;
  }

  // ========================================
  // CLASE VISUAL DE ALERTAS
  // ========================================

  obtenerClaseAlerta(
    dias: number
  ): string {

    if (dias <= 1) {
      return 'alerta-urgente';
    }

    if (dias === 2) {
      return 'alerta-media';
    }

    return 'alerta-preventiva';
  }

  // ========================================
  // ROLES
  // ========================================

  esAdministrador(): boolean {

    return (
      this.usuario?.role ===
      'Administrador'
    );
  }
}