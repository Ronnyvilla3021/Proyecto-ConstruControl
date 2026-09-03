import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ObraService } from '../../../core/services/obra.service';
import { Obra, EstadoObra } from '../../../core/models/obra.model';

@Component({
  selector: 'app-obras-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './obras-list.html',
  styleUrl: './obras-list.scss'
})
export class ObrasList implements OnInit {
  obras = signal<Obra[]>([]);
  cargando = signal(false);
  mostrarFormulario = signal(false);
  errorMensaje = signal<string | null>(null);

  form: FormGroup;

  transicionesPermitidas: Record<EstadoObra, EstadoObra[]> = {
    Planificacion: ['Activa'],
    Activa: ['Pausada', 'Finalizada'],
    Pausada: ['Activa', 'Finalizada'],
    Finalizada: []
  };

  constructor(private obraService: ObraService, private fb: FormBuilder) {
    this.form = this.fb.group({
      nombre: ['', Validators.required],
      ubicacion: ['', Validators.required],
      presupuesto: [0, [Validators.required, Validators.min(1)]],
      fechaInicio: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.cargarObras();
  }

  cargarObras(): void {
    this.cargando.set(true);
    this.obraService.obtenerTodas().subscribe({
      next: (data) => {
        this.obras.set(data);
        this.cargando.set(false);
      },
      error: () => {
        this.errorMensaje.set('No se pudieron cargar las obras.');
        this.cargando.set(false);
      }
    });
  }

  toggleFormulario(): void {
    this.mostrarFormulario.update((v) => !v);
  }

  crearObra(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.obraService.crear(this.form.value).subscribe({
      next: () => {
        this.form.reset({ presupuesto: 0 });
        this.mostrarFormulario.set(false);
        this.cargarObras();
      },
      error: () => this.errorMensaje.set('No se pudo crear la obra.')
    });
  }

  cambiarEstado(obra: Obra, nuevoEstado: EstadoObra): void {
    this.obraService.cambiarEstado(obra.id, { nuevoEstado }).subscribe({
      next: () => this.cargarObras(),
      error: () => this.errorMensaje.set('No se pudo cambiar el estado de la obra.')
    });
  }

  eliminarObra(id: number): void {
    if (!confirm('¿Eliminar esta obra?')) return;

    this.obraService.eliminar(id).subscribe({
      next: () => this.cargarObras(),
      error: () => this.errorMensaje.set('No se pudo eliminar la obra.')
    });
  }
}
