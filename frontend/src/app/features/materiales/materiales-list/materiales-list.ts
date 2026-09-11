import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ProgressBarModule } from 'primeng/progressbar';
import { MaterialService } from '../../../core/services/material.service';
import { Material } from '../../../core/models/material.model';

@Component({
  selector: 'app-materiales-list',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, FormsModule,
    CardModule, TableModule, ButtonModule, DialogModule, TagModule,
    InputTextModule, InputNumberModule, ProgressBarModule
  ],
  templateUrl: './materiales-list.html',
  styleUrl: './materiales-list.scss'
})
export class MaterialesList implements OnInit {
  materiales = signal<Material[]>([]);
  cargando = signal(false);
  mostrarDialogo = signal(false);
  errorMensaje = signal<string | null>(null);
  filtro = signal('');
  form: FormGroup;

  materialesFiltrados = computed(() => {
    const texto = this.filtro().toLowerCase();
    if (!texto) return this.materiales();
    return this.materiales().filter(m => m.nombre.toLowerCase().includes(texto));
  });

  totalMateriales = computed(() => this.materiales().length);
  stockCritico = computed(() => this.materiales().filter(m => m.stockBajo).length);

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

  abrirDialogo(): void {
    this.form.reset({ stockMinimo: 0, precioUnitario: 0 });
    this.mostrarDialogo.set(true);
  }

  crearMaterial(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.materialService.crear(this.form.value).subscribe({
      next: () => {
        this.mostrarDialogo.set(false);
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

  porcentajeStock(material: Material): number {
    if (material.stockMinimo <= 0) return 100;
    const ratio = (material.stock / (material.stockMinimo * 2)) * 100;
    return Math.min(100, Math.max(0, Math.round(ratio)));
  }

  estadoLabel(material: Material): string {
    return material.stockBajo ? 'Crítico' : 'OK';
  }

  estadoSeverity(material: Material): 'success' | 'danger' {
    return material.stockBajo ? 'danger' : 'success';
  }
}
