import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EmpleadoService } from '../../../core/services/empleado.service';
import { AsistenciaService } from '../../../core/services/asistencia.service';
import { ObraService } from '../../../core/services/obra.service';
import { Empleado } from '../../../core/models/empleado.model';
import { Asistencia } from '../../../core/models/asistencia.model';
import { Obra } from '../../../core/models/obra.model';

@Component({
  selector: 'app-personal-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './personal-list.html',
  styleUrl: './personal-list.scss'
})
export class PersonalList implements OnInit {
  empleados = signal<Empleado[]>([]);
  asistencias = signal<Asistencia[]>([]);
  obras = signal<Obra[]>([]);
  cargando = signal(false);
  mostrarFormEmpleado = signal(false);
  errorMensaje = signal<string | null>(null);

  formEmpleado: FormGroup;
  formEntrada: FormGroup;

  constructor(
    private empleadoService: EmpleadoService,
    private asistenciaService: AsistenciaService,
    private obraService: ObraService,
    private fb: FormBuilder
  ) {
    this.formEmpleado = this.fb.group({
      nombre: ['', Validators.required],
      cargo: ['', Validators.required],
      fechaIngreso: ['', Validators.required]
    });

    this.formEntrada = this.fb.group({
      empleadoId: ['', Validators.required],
      obraId: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.cargarEmpleados();
    this.cargarAsistencias();
    this.obraService.obtenerTodas().subscribe((data) => this.obras.set(data));
  }

  cargarEmpleados(): void {
    this.cargando.set(true);
    this.empleadoService.obtenerTodos().subscribe({
      next: (data) => {
        this.empleados.set(data);
        this.cargando.set(false);
      },
      error: () => {
        this.errorMensaje.set('No se pudieron cargar los empleados.');
        this.cargando.set(false);
      }
    });
  }

  cargarAsistencias(): void {
    this.asistenciaService.obtenerTodas().subscribe((data) => this.asistencias.set(data));
  }

  toggleFormEmpleado(): void {
    this.mostrarFormEmpleado.update((v) => !v);
  }

  crearEmpleado(): void {
    if (this.formEmpleado.invalid) {
      this.formEmpleado.markAllAsTouched();
      return;
    }
    this.empleadoService.crear(this.formEmpleado.value).subscribe({
      next: () => {
        this.formEmpleado.reset();
        this.mostrarFormEmpleado.set(false);
        this.cargarEmpleados();
      },
      error: () => this.errorMensaje.set('No se pudo crear el empleado.')
    });
  }

  eliminarEmpleado(id: number): void {
    if (!confirm('¿Eliminar este empleado?')) return;
    this.empleadoService.eliminar(id).subscribe({
      next: () => this.cargarEmpleados(),
      error: () => this.errorMensaje.set('No se pudo eliminar el empleado.')
    });
  }

  registrarEntrada(): void {
    if (this.formEntrada.invalid) {
      this.formEntrada.markAllAsTouched();
      return;
    }
    this.asistenciaService.registrarEntrada(this.formEntrada.value).subscribe({
      next: () => {
        this.formEntrada.reset();
        this.cargarAsistencias();
      },
      error: () => this.errorMensaje.set('Ya existe una entrada registrada hoy para este empleado en esta obra.')
    });
  }

  registrarSalida(id: number): void {
    this.asistenciaService.registrarSalida(id).subscribe({
      next: () => this.cargarAsistencias(),
      error: () => this.errorMensaje.set('No se pudo registrar la salida.')
    });
  }
}
