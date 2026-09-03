import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class SignalrService {
  private hubConnection: signalR.HubConnection | null = null;
  ultimoEvento = signal<{ tipo: string; datos: any } | null>(null);

  constructor(private authService: AuthService) {}

  conectar(obraId: number): void {
    const token = this.authService.getToken();
    if (!token) return;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.hubUrl}?access_token=${token}`)
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('CompraRecepcionada', (datos) => {
      this.ultimoEvento.set({ tipo: 'CompraRecepcionada', datos });
    });

    this.hubConnection.on('ConsumoRegistrado', (datos) => {
      this.ultimoEvento.set({ tipo: 'ConsumoRegistrado', datos });
    });

    this.hubConnection
      .start()
      .then(() => this.hubConnection?.invoke('JoinObraGroup', obraId.toString()))
      .catch((err) => console.error('Error conectando a SignalR:', err));
  }

  desconectar(): void {
    this.hubConnection?.stop();
    this.hubConnection = null;
  }
}
