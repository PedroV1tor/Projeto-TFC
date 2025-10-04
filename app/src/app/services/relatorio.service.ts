import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

export interface RelatorioResponseDTO<T> {
  total: number;
  items: T[];
  dataInicial?: string;
  dataFinal?: string;
  geradoEm: string;
}

export interface RelatorioUsuarioDTO {
  id: number;
  nome: string;
  email: string;
  telefone: string;
  status: string;
  criadoEm: string;
}

export interface RelatorioPublicacaoDTO {
  id: number;
  titulo: string;
  autor: string;
  status: string;
  visualizacoes: number;
  curtidas: number;
  criadoEm: string;
}

export interface RelatorioAgendamentoDTO {
  id: number;
  titulo: string;
  data: string;
  usuario: string;
  status: string;
  criadoEm: string;
}

export interface RelatorioOrcamentoDTO {
  id: number;
  titulo: string;
  cliente: string;
  valor: number;
  status: string;
  criadoEm: string;
}

export interface RelatorioResumoDTO {
  totalUsuarios: number;
  usuariosAtivos: number;
  totalPublicacoes: number;
  publicacoesAtivas: number;
  totalAgendamentos: number;
  agendamentosAtivos: number;
  totalOrcamentos: number;
  orcamentosAprovados: number;
  valorTotalOrcamentos: number;
  dataInicial?: string;
  dataFinal?: string;
  geradoEm: string;
}

export interface FiltroRelatorio {
  dataInicial?: string;
  dataFinal?: string;
}

@Injectable({
  providedIn: 'root'
})
export class RelatorioService {
  private apiUrl = `${environment.apiUrl}/relatorio`;

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    if (!token) {
      throw new Error('Token de autenticação não encontrado');
    }

    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  private buildParams(filtro?: FiltroRelatorio): HttpParams {
    let params = new HttpParams();

    if (filtro?.dataInicial) {
      params = params.set('dataInicial', filtro.dataInicial);
    }

    if (filtro?.dataFinal) {
      params = params.set('dataFinal', filtro.dataFinal);
    }

    return params;
  }


  getRelatorioUsuarios(filtro?: FiltroRelatorio): Observable<RelatorioResponseDTO<RelatorioUsuarioDTO>> {
    const params = this.buildParams(filtro);

    return this.http.get<RelatorioResponseDTO<RelatorioUsuarioDTO>>(
      `${this.apiUrl}/usuarios`,
      {
        headers: this.getHeaders(),
        params: params
      }
    ).pipe(
      catchError(error => {
        console.error('Erro ao buscar relatório de usuários:', error);
        return throwError(error);
      })
    );
  }


  getRelatorioPublicacoes(filtro?: FiltroRelatorio): Observable<RelatorioResponseDTO<RelatorioPublicacaoDTO>> {
    const params = this.buildParams(filtro);

    return this.http.get<RelatorioResponseDTO<RelatorioPublicacaoDTO>>(
      `${this.apiUrl}/publicacoes`,
      {
        headers: this.getHeaders(),
        params: params
      }
    ).pipe(
      catchError(error => {
        console.error('Erro ao buscar relatório de publicações:', error);
        return throwError(error);
      })
    );
  }


  getRelatorioAgendamentos(filtro?: FiltroRelatorio): Observable<RelatorioResponseDTO<RelatorioAgendamentoDTO>> {
    const params = this.buildParams(filtro);

    return this.http.get<RelatorioResponseDTO<RelatorioAgendamentoDTO>>(
      `${this.apiUrl}/agendamentos`,
      {
        headers: this.getHeaders(),
        params: params
      }
    ).pipe(
      catchError(error => {
        console.error('Erro ao buscar relatório de agendamentos:', error);
        return throwError(error);
      })
    );
  }


  getRelatorioOrcamentos(filtro?: FiltroRelatorio): Observable<RelatorioResponseDTO<RelatorioOrcamentoDTO>> {
    const params = this.buildParams(filtro);

    return this.http.get<RelatorioResponseDTO<RelatorioOrcamentoDTO>>(
      `${this.apiUrl}/orcamentos`,
      {
        headers: this.getHeaders(),
        params: params
      }
    ).pipe(
      catchError(error => {
        console.error('Erro ao buscar relatório de orçamentos:', error);
        return throwError(error);
      })
    );
  }


  getRelatorioResumo(filtro?: FiltroRelatorio): Observable<RelatorioResumoDTO> {
    const params = this.buildParams(filtro);

    return this.http.get<RelatorioResumoDTO>(
      `${this.apiUrl}/resumo`,
      {
        headers: this.getHeaders(),
        params: params
      }
    ).pipe(
      catchError(error => {
        console.error('Erro ao buscar relatório resumo:', error);
        return throwError(error);
      })
    );
  }
}
