import { Component, OnInit, OnDestroy, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { DashboardService } from '../../../core/services/dashboard.service';
import { SignalrService } from '../../../core/services/signalr.service';
import { DashboardData } from '../../../core/models/dashboard.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit, OnDestroy {
  data = signal<DashboardData | null>(null);
  cargando = signal(false);
  errorMensaje = signal<string | null>(null);
  eventosEnVivo = signal<string[]>([]);
  obraId!: number;

  constructor(
    private route: ActivatedRoute,
    private dashboardService: DashboardService,
    private signalrService: SignalrService
  ) {
    effect(() => {
      const evento = this.signalrService.ultimoEvento();
      if (evento) {
        const texto = `${evento.tipo}: ${JSON.stringify(evento.datos)}`;
        this.eventosEnVivo.update((lista) => [texto, ...lista].slice(0, 10));
        this.cargarDashboard();
      }
    });
  }

  ngOnInit(): void {
    this.obraId = Number(this.route.snapshot.paramMap.get('obraId'));
    this.cargarDashboard();
    this.signalrService.conectar(this.obraId);
  }

  ngOnDestroy(): void {
    this.signalrService.desconectar();
  }

  cargarDashboard(): void {
    this.cargando.set(true);
    this.dashboardService.obtenerDashboard(this.obraId).subscribe({
      next: (data) => {
        this.data.set(data);
        this.cargando.set(false);
      },
      error: () => {
        this.errorMensaje.set('No se pudo cargar el dashboard.');
        this.cargando.set(false);
      }
    });
  }
}
