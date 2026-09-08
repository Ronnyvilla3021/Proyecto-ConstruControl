import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DatePickerModule } from 'primeng/datepicker';
import { ObraService } from '../../../core/services/obra.service';
import { Obra, EstadoObra } from '../../../core/models/obra.model';

@Component({
  selector: 'app-obras-list',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterLink,
    CardModule, TableModule, ButtonModule, DialogModule, TagModule,
    InputTextModule, InputNumberModule, DatePickerModule
  ],
  templateUrl: './obras-list.html',
  styleUrl: './obras-list.scss'
})
export class ObrasList implements OnInit {
  obras = signal<Obra[]>([]);
  cargando = signal(false);
  mostrarDialogo = signal(false);
  errorMensaje = signal<string | null>(null);

  form: FormGroup;

  transicionesPermitidas: Record<EstadoObra, EstadoObra[]> = {
    Planificacion: ['Activa'],
    Activa: ['Pausada', 'Finalizada'],
    Pausada: ['Activa', 'Finalizada'],
    Finalizada: []
  };

  totalObras = computed(() => this.obras().length);
  obrasActivas = computed(() => this.obras().filter(o => o.estado === 'Activa').length);
  presupuestoTotal = computed(() => this.obras().reduce((sum, o) => sum + o.presupuesto, 0));

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

  abrirDialogo(): void {
    this.form.reset({ presupuesto: 0 });
    this.mostrarDialogo.set(true);
  }

  crearObra(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const valor = this.form.value;
    const fecha = valor.fechaInicio instanceof Date
      ? valor.fechaInicio.toISOString().split('T')[0]
      : valor.fechaInicio;

    this.obraService.crear({ ...valor, fechaInicio: fecha }).subscribe({
      next: () => {
        this.mostrarDialogo.set(false);
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

  obtenerTransiciones(obra: Obra): EstadoObra[] {
    return this.transicionesPermitidas[obra.estado] ?? [];
  }

  severityEstado(estado: EstadoObra): 'success' | 'warn' | 'info' | 'danger' {
    switch (estado) {
      case 'Activa': return 'success';
      case 'Pausada': return 'warn';
      case 'Planificacion': return 'info';
      case 'Finalizada': return 'danger';
    }
  }
}
