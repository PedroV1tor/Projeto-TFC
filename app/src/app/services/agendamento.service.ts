import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { Agendamento } from '../models/agendamento.model';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AgendamentoService {
  private readonly apiUrl = `${environment.apiUrl}/agendamento`;
  private agendamentosSubject = new BehaviorSubject<Agendamento[]>([]);
  agendamentos$ = this.agendamentosSubject.asObservable();

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {
    this.loadAgendamentos();
  }

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  private loadAgendamentos(): void {
    if (this.authService.isLoggedIn) {
      this.getAgendamentos().subscribe({
        next: (agendamentos) => this.agendamentosSubject.next(agendamentos),
        error: (error) => console.error('Erro ao carregar agendamentos:', error)
      });
    }
  }

  // Listar todos os agendamentos
  getAgendamentos(): Observable<Agendamento[]> {
    return this.http.get<Agendamento[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  // Obter agendamento por ID
  getAgendamentoById(id: number): Observable<Agendamento> {
    return this.http.get<Agendamento>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  // Obter agendamentos por status
  getAgendamentosByStatus(status: string): Observable<Agendamento[]> {
    return this.http.get<Agendamento[]>(`${this.apiUrl}/status/${status}`, { headers: this.getHeaders() });
  }

  // Obter meus agendamentos
  getMeusAgendamentos(): Observable<Agendamento[]> {
    return this.http.get<Agendamento[]>(`${this.apiUrl}/meus`, { headers: this.getHeaders() });
  }

  // Obter próximos eventos
  getProximosEventos(dias: number = 7): Observable<Agendamento[]> {
    return this.http.get<Agendamento[]>(`${this.apiUrl}/proximos?dias=${dias}`, { headers: this.getHeaders() });
  }

  // Criar novo agendamento
  criarAgendamento(agendamento: Omit<Agendamento, 'id' | 'criadoEm' | 'status'>): Observable<Agendamento> {
    // Mapear os campos para o formato esperado pelo backend
    const dadosBackend = {
      Titulo: agendamento.titulo,
      Data: new Date(agendamento.data),
      Descricao: agendamento.descricao,
      Usuario: agendamento.usuario
    };

    console.log('Enviando dados de agendamento para o backend:', dadosBackend);

    return this.http.post<Agendamento>(this.apiUrl, dadosBackend, { headers: this.getHeaders() });
  }

  // Atualizar agendamento
  atualizarAgendamento(id: number, agendamentoAtualizado: Partial<Agendamento>): Observable<void> {
    const dadosBackend: any = {};
    if (agendamentoAtualizado.titulo !== undefined) {
      dadosBackend.Titulo = agendamentoAtualizado.titulo;
    }
    if (agendamentoAtualizado.data !== undefined) {
      dadosBackend.Data = new Date(agendamentoAtualizado.data as unknown as string);
    }
    if (agendamentoAtualizado.descricao !== undefined) {
      dadosBackend.Descricao = agendamentoAtualizado.descricao;
    }
    if (agendamentoAtualizado.usuario !== undefined) {
      dadosBackend.Usuario = agendamentoAtualizado.usuario;
    }

    return this.http.put<void>(`${this.apiUrl}/${id}`, dadosBackend, { headers: this.getHeaders() });
  }

  // Excluir agendamento
  excluirAgendamento(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  // Alterar status do agendamento
  alterarStatus(id: number, novoStatus: 'pendente' | 'aprovado' | 'reprovado'): Observable<void> {
    const payload = { Status: (novoStatus || '').toString().trim().toLowerCase() };
    return this.http.patch<void>(`${this.apiUrl}/${id}/status`, payload, { headers: this.getHeaders() });
  }

  // Filtrar agendamentos
  filtrarAgendamentos(filtro: any): Observable<Agendamento[]> {
    // Mapear os campos para o formato esperado pelo backend
    const dadosBackend: any = {};

    if (filtro.status !== undefined) {
      dadosBackend.Status = filtro.status;
    }
    if (filtro.usuario !== undefined) {
      dadosBackend.Usuario = filtro.usuario;
    }
    if (filtro.dataInicio !== undefined) {
      dadosBackend.DataInicio = new Date(filtro.dataInicio);
    }
    if (filtro.dataFim !== undefined) {
      dadosBackend.DataFim = new Date(filtro.dataFim);
    }
    if (filtro.proximosEventos !== undefined) {
      dadosBackend.ProximosEventos = filtro.proximosEventos;
    }
    if (filtro.proximosDias !== undefined) {
      dadosBackend.ProximosDias = filtro.proximosDias;
    }

    console.log('Enviando filtro de agendamentos para o backend:', dadosBackend);

    return this.http.post<Agendamento[]>(`${this.apiUrl}/filtrar`, dadosBackend, { headers: this.getHeaders() });
  }
}
