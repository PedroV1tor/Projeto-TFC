import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { Orcamento } from '../models/orcamento.model';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrcamentoService {
  private readonly apiUrl = `${environment.apiUrl}/orcamento`;
  private orcamentosSubject = new BehaviorSubject<Orcamento[]>([]);
  orcamentos$ = this.orcamentosSubject.asObservable();

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {
    this.loadOrcamentos();
  }

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  private loadOrcamentos(): void {
    if (this.authService.isLoggedIn) {
      this.getOrcamentos().subscribe({
        next: (orcamentos) => this.orcamentosSubject.next(orcamentos),
        error: (error) => console.error('Erro ao carregar orçamentos:', error)
      });
    }
  }


  getOrcamentos(): Observable<Orcamento[]> {
    return this.http.get<Orcamento[]>(this.apiUrl, { headers: this.getHeaders() });
  }


  getOrcamentosByStatus(status: 'pendente' | 'aprovado' | 'rejeitado' | 'concluido'): Observable<Orcamento[]> {
    return this.http.get<Orcamento[]>(`${this.apiUrl}/status/${status}`, { headers: this.getHeaders() });
  }


  getOrcamentoById(id: number): Observable<Orcamento> {
    return this.http.get<Orcamento>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }


  getMeusOrcamentos(): Observable<Orcamento[]> {
    return this.http.get<Orcamento[]>(`${this.apiUrl}/meus`, { headers: this.getHeaders() });
  }


  getEstatisticas(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/estatisticas`, { headers: this.getHeaders() });
  }


  getOrcamentosVencendo(dias: number = 7): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/vencendo?dias=${dias}`, { headers: this.getHeaders() });
  }


  criarOrcamento(orcamento: Omit<Orcamento, 'id' | 'criadoEm' | 'status'>): Observable<Orcamento> {

    const dadosBackend = {
      Titulo: orcamento.titulo,
      Descricao: orcamento.descricao,
      PrazoEntrega: new Date(orcamento.prazoEntrega),
      PrazoOrcamento: new Date(orcamento.prazoOrcamento),
      Valor: orcamento.valor,
      Cliente: orcamento.cliente,
      Responsavel: orcamento.responsavel
    };

    console.log('Enviando dados de orçamento para o backend:', dadosBackend);

    return this.http.post<Orcamento>(this.apiUrl, dadosBackend, { headers: this.getHeaders() });
  }


  atualizarOrcamento(id: number, orcamentoAtualizado: Partial<Orcamento>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, orcamentoAtualizado, { headers: this.getHeaders() });
  }


  excluirOrcamento(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }


  alterarStatus(id: number, novoStatus: 'pendente' | 'aprovado' | 'rejeitado' | 'concluido'): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/status`, { status: novoStatus }, { headers: this.getHeaders() });
  }


  aprovarOrcamento(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/aprovar`, {}, { headers: this.getHeaders() });
  }


  rejeitarOrcamento(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/rejeitar`, {}, { headers: this.getHeaders() });
  }


  concluirOrcamento(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/concluir`, {}, { headers: this.getHeaders() });
  }


  filtrarOrcamentos(filtro: any): Observable<Orcamento[]> {

    const dadosBackend: any = {};

    if (filtro.status !== undefined) {
      dadosBackend.Status = filtro.status;
    }
    if (filtro.cliente !== undefined) {
      dadosBackend.Cliente = filtro.cliente;
    }
    if (filtro.responsavel !== undefined) {
      dadosBackend.Responsavel = filtro.responsavel;
    }
    if (filtro.dataInicio !== undefined) {
      dadosBackend.DataInicio = new Date(filtro.dataInicio);
    }
    if (filtro.dataFim !== undefined) {
      dadosBackend.DataFim = new Date(filtro.dataFim);
    }
    if (filtro.valorMinimo !== undefined) {
      dadosBackend.ValorMinimo = filtro.valorMinimo;
    }
    if (filtro.valorMaximo !== undefined) {
      dadosBackend.ValorMaximo = filtro.valorMaximo;
    }
    if (filtro.prazosVencidos !== undefined) {
      dadosBackend.PrazosVencidos = filtro.prazosVencidos;
    }
    if (filtro.diasPrazo !== undefined) {
      dadosBackend.DiasPrazo = filtro.diasPrazo;
    }

    console.log('Enviando filtro de orçamentos para o backend:', dadosBackend);

    return this.http.post<Orcamento[]>(`${this.apiUrl}/filtrar`, dadosBackend, { headers: this.getHeaders() });
  }
}
