import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { Publicacao } from '../models/publicacao.model';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PublicacaoService {
  private readonly apiUrl = `${environment.apiUrl}/publicacao`;
  private publicacoesSubject = new BehaviorSubject<Publicacao[]>([]);
  publicacoes$ = this.publicacoesSubject.asObservable();

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {
    this.loadPublicacoes();
  }

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    const isLoggedIn = this.authService.isLoggedIn;

    console.log('PublicacaoService: criando headers para requisição');
    console.log('PublicacaoService: usuário logado:', isLoggedIn);
    console.log('PublicacaoService: token presente:', !!token);

    if (token) {
      return new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      });
    }
    return new HttpHeaders({
      'Content-Type': 'application/json'
    });
  }

  private loadPublicacoes(): void {
    this.getPublicacoes().subscribe({
      next: (publicacoes) => this.publicacoesSubject.next(publicacoes),
      error: (error) => console.error('Erro ao carregar publicações:', error)
    });
  }

  // Listar todas as publicações
  getPublicacoes(): Observable<Publicacao[]> {
    return this.http.get<Publicacao[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  // Obter publicações ativas para timeline
  getPublicacoesAtivas(): Observable<Publicacao[]> {
    return this.http.get<Publicacao[]>(`${this.apiUrl}/ativas`, { headers: this.getHeaders() });
  }

  // Obter timeline das publicações
  getTimeline(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/timeline`, { headers: this.getHeaders() });
  }

  // Obter publicação por ID
  getPublicacaoById(id: number): Observable<Publicacao> {
    return this.http.get<Publicacao>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  // Obter publicações por status
  getPublicacoesByStatus(status: string): Observable<Publicacao[]> {
    return this.http.get<Publicacao[]>(`${this.apiUrl}/status/${status}`, { headers: this.getHeaders() });
  }

  // Obter minhas publicações
  getMinhasPublicacoes(): Observable<Publicacao[]> {
    return this.http.get<Publicacao[]>(`${this.apiUrl}/minhas`, { headers: this.getHeaders() });
  }

  // Criar nova publicação
  criarPublicacao(publicacao: Omit<Publicacao, 'id' | 'criadoEm' | 'status' | 'visualizacoes' | 'curtidas'>): Observable<Publicacao> {
    // Mapear os campos para o formato esperado pelo backend
    const dadosBackend = {
      Titulo: publicacao.titulo,
      Resumo: publicacao.resumo,
      Descricao: publicacao.descricao
    };

    console.log('Enviando dados para o backend:', dadosBackend);

    return this.http.post<Publicacao>(this.apiUrl, dadosBackend, { headers: this.getHeaders() });
  }

  // Atualizar publicação
  atualizarPublicacao(id: number, publicacaoAtualizada: Partial<Publicacao>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, publicacaoAtualizada, { headers: this.getHeaders() });
  }

  // Excluir publicação
  excluirPublicacao(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  // Alterar status da publicação
  alterarStatus(id: number, novoStatus: 'ativa' | 'rascunho' | 'arquivada'): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/status`, { status: novoStatus }, { headers: this.getHeaders() });
  }

  // Curtir publicação
  curtirPublicacao(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/curtir`, {}, { headers: this.getHeaders() });
  }
}
