import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { CompraService } from '../../../core/services/compra.service';
import { ObraService } from '../../../core/services/obra.service';
import { MaterialService } from '../../../core/services/material.service';
import { ProveedorService } from '../../../core/services/proveedor.service';
import { Compra } from '../../../core/models/compra.model';
import { Obra } from '../../../core/models/obra.model';
import { Material } from '../../../core/models/material.model';
import { Proveedor } from '../../../core/models/proveedor.model';

@Component({
  selector: 'app-compras-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './compras-list.html',
  styleUrl: './compras-list.scss'
})
export class ComprasList implements OnInit {
  compras = signal<Compra[]>([]);
  obras = signal<Obra[]>([]);
  materiales = signal<Material[]>([]);
  proveedores = signal<Proveedor[]>([]);
  cargando = signal(false);
  mostrarFormulario = signal(false);
  errorMensaje = signal<string | null>(null);
  form: FormGroup;

  constructor(
    private compraService: CompraService,
    private obraService: ObraService,
    private materialService: MaterialService,
    private proveedorService: ProveedorService,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      proveedorId: ['', Validators.required],
      obraId: ['', Validators.required],
      detalles: this.fb.array([this.crearLineaDetalle()])
    });
  }

  get detalles(): FormArray {
    return this.form.get('detalles') as FormArray;
  }

  crearLineaDetalle(): FormGroup {
    return this.fb.group({
      materialId: ['', Validators.required],
      cantidad: [0, [Validators.required, Validators.min(0.01)]],
      precioUnitario: [0, [Validators.required, Validators.min(0.01)]]
    });
  }

  agregarLinea(): void {
    this.detalles.push(this.crearLineaDetalle());
  }

  quitarLinea(index: number): void {
    if (this.detalles.length > 1) {
      this.detalles.removeAt(index);
    }
  }

  ngOnInit(): void {
    this.cargarCompras();
    this.obraService.obtenerTodas().subscribe((data) => this.obras.set(data));
    this.materialService.obtenerTodos().subscribe((data) => this.materiales.set(data));
    this.proveedorService.obtenerTodos().subscribe((data) => this.proveedores.set(data));
  }

  cargarCompras(): void {
    this.cargando.set(true);
    this.compraService.obtenerTodas().subscribe({
      next: (data) => {
        this.compras.set(data);
        this.cargando.set(false);
      },
      error: () => {
        this.errorMensaje.set('No se pudieron cargar las compras.');
        this.cargando.set(false);
      }
    });
  }

  toggleFormulario(): void {
    this.mostrarFormulario.update((v) => !v);
  }

  crearCompra(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.compraService.crear(this.form.value).subscribe({
      next: () => {
        this.form.reset({ detalles: [] });
        this.detalles.clear();
        this.detalles.push(this.crearLineaDetalle());
        this.mostrarFormulario.set(false);
        this.cargarCompras();
      },
      error: () => this.errorMensaje.set('No se pudo crear la compra.')
    });
  }

  recepcionar(id: number): void {
    if (!confirm('¿Recepcionar esta compra? Esto incrementará el stock de los materiales.')) return;
    this.compraService.recepcionar(id).subscribe({
      next: () => this.cargarCompras(),
      error: () => this.errorMensaje.set('No se pudo recepcionar la compra.')
    });
  }
}
