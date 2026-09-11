import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { ProgressBarModule } from 'primeng/progressbar';
import { ObraService } from '../../../core/services/obra.service';
import { MaterialService } from '../../../core/services/material.service';
import { EmpleadoService } from '../../../core/services/empleado.service';
import { NotificacionService } from '../../../core/services/notificacion.service';
import { Obra } from '../../../core/models/obra.model';
import { Material } from '../../../core/models/material.model';
import { Notificacion } from '../../../core/models/notificacion.model';

@Component({
  selector: 'app-resumen-general',
  standalone: true,
  imports: [CommonModule, RouterLink, CardModule, TagModule, ProgressBarModule],
  templateUrl: './resumen-general.html',
  styleUrl: './resumen-general.scss'
})
export class ResumenGeneral implements OnInit {
  obras = signal<Obra[]>([]);
  materiales = signal<Material[]>([]);
  notificaciones = signal<Notificacion[]>([]);
  cargando = signal(false);

  totalObrasActivas = computed(() => this.obras().filter(o => o.estado === 'Activa').length);
  presupuestoTotal = computed(() => this.obras().reduce((sum, o) => sum + o.presupuesto, 0));
  materialesCriticos = computed(() => this.materiales().filter(m => m.stockBajo).length);
  notificacionesSinLeer = computed(() => this.notificaciones().filter(n => !n.leida).length);

  constructor(
    private obraService: ObraService,
    private materialService: MaterialService,
    private empleadoService: EmpleadoService,
    private notificacionService: NotificacionService
  ) {}

  ngOnInit(): void {
    this.cargando.set(true);
    this.obraService.obtenerTodas().subscribe(data => {
      this.obras.set(data);
      this.cargando.set(false);
    });
    this.materialService.obtenerTodos().subscribe(data => this.materiales.set(data));
    this.notificacionService.obtenerTodas().subscribe(data => this.notificaciones.set(data.slice(0, 5)));
  }

  severityEstado(estado: string): 'success' | 'warn' | 'info' | 'danger' {
    switch (estado) {
      case 'Activa': return 'success';
      case 'Pausada': return 'warn';
      case 'Planificacion': return 'info';
      default: return 'danger';
    }
  }

  iconoNotificacion(tipo: string): string {
    switch (tipo) {
      case 'StockBajo': return 'pi pi-box';
      case 'PresupuestoExcedido': return 'pi pi-wallet';
      case 'CompraSugerida': return 'pi pi-shopping-cart';
      default: return 'pi pi-bell';
    }
  }
}
