import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConsumoService } from '../../../core/services/consumo.service';
import { ObraService } from '../../../core/services/obra.service';
import { MaterialService } from '../../../core/services/material.service';
import { Consumo } from '../../../core/models/consumo.model';
import { Obra } from '../../../core/models/obra.model';
import { Material } from '../../../core/models/material.model';

@Component({
  selector: 'app-consumo-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './consumo-list.html',
  styleUrl: './consumo-list.scss'
})
export class ConsumoList implements OnInit {
  consumos = signal<Consumo[]>([]);
  obras = signal<Obra[]>([]);
  materiales = signal<Material[]>([]);
  cargando = signal(false);
  errorMensaje = signal<string | null>(null);
  form: FormGroup;

  constructor(
    private consumoService: ConsumoService,
    private obraService: ObraService,
    private materialService: MaterialService,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      materialId: ['', Validators.required],
      obraId: ['', Validators.required],
      cantidad: [0, [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    this.cargarConsumos();
    this.obraService.obtenerTodas().subscribe((data) => this.obras.set(data));
    this.materialService.obtenerTodos().subscribe((data) => this.materiales.set(data));
  }

  cargarConsumos(): void {
    this.cargando.set(true);
    this.consumoService.obtenerTodos().subscribe({
      next: (data) => {
        this.consumos.set(data);
        this.cargando.set(false);
      },
      error: () => {
        this.errorMensaje.set('No se pudieron cargar los consumos.');
        this.cargando.set(false);
      }
    });
  }

  registrarConsumo(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.errorMensaje.set(null);
    this.consumoService.registrar(this.form.value).subscribe({
      next: () => {
        this.form.reset({ cantidad: 0 });
        this.cargarConsumos();
      },
      error: () => this.errorMensaje.set('Stock insuficiente o datos inválidos.')
    });
  }
}
