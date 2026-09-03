import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MaterialService } from '../../../core/services/material.service';
import { Material } from '../../../core/models/material.model';

@Component({
  selector: 'app-materiales-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './materiales-list.html',
  styleUrl: './materiales-list.scss'
})
export class MaterialesList implements OnInit {
  materiales = signal<Material[]>([]);
  cargando = signal(false);
  mostrarFormulario = signal(false);
  errorMensaje = signal<string | null>(null);
  form: FormGroup;

  constructor(private materialService: MaterialService, private fb: FormBuilder) {
    this.form = this.fb.group({
      nombre: ['', Validators.required],
      stockMinimo: [0, [Validators.required, Validators.min(0)]],
      unidad: ['', Validators.required],
      precioUnitario: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.cargarMateriales();
  }

  cargarMateriales(): void {
    this.cargando.set(true);
    this.materialService.obtenerTodos().subscribe({
      next: (data) => {
        this.materiales.set(data);
        this.cargando.set(false);
      },
      error: () => {
        this.errorMensaje.set('No se pudieron cargar los materiales.');
        this.cargando.set(false);
      }
    });
  }

  toggleFormulario(): void {
    this.mostrarFormulario.update((v) => !v);
  }

  crearMaterial(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.materialService.crear(this.form.value).subscribe({
      next: () => {
        this.form.reset({ stockMinimo: 0, precioUnitario: 0 });
        this.mostrarFormulario.set(false);
        this.cargarMateriales();
      },
      error: () => this.errorMensaje.set('No se pudo crear el material.')
    });
  }

  eliminarMaterial(id: number): void {
    if (!confirm('¿Eliminar este material?')) return;
    this.materialService.eliminar(id).subscribe({
      next: () => this.cargarMateriales(),
      error: () => this.errorMensaje.set('No se pudo eliminar el material.')
    });
  }
}
