import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap, switchMap } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface EnderecoUsuario {
  cep: string;
  rua: string;
  bairro: string;
  numero?: string;
  complemento?: string;
  referencia?: string;
}

export interface Usuario {
  tipo?: 'usuario' | 'empresa';
  id?: number;
  email: string;
  // Campos de Pessoa Física
  nome?: string;
  sobrenome?: string;
  nomeUsuario?: string;
  telefone?: string;
  matricula?: string;
  // Campos de Empresa
  razaoSocial?: string;
  nomeFantasia?: string;
  cnpj?: string;
  responsavelNome?: string;
  responsavelTelefone?: string;
  // Comum
  endereco?: EnderecoUsuario;
  dataCriacao?: string;
  ultimoLogin?: string;
  isAdmin?: boolean;
}

export interface LoginRequest {
  email: string;
  senha: string;
}

export interface LoginResponse {
  token: string;
  email: string;
  nome: string;
  nomeUsuario: string;
  isAdmin: boolean;
  expiresAt: string;
}

export interface CadastroRequest {
  nome: string;
  sobrenome: string;
  email: string;
  matricula?: string;
  senha: string;
  nomeUsuario: string;
  telefone: string;
  endereco: EnderecoUsuario;
}

export interface EnderecoEmpresa {
  cep: string;
  rua: string;
  bairro: string;
  numero?: string;
  complemento?: string;
  referencia?: string;
}

export interface CadastroEmpresaRequest {
  razaoSocial: string;
  nomeFantasia?: string;
  cnpj: string;
  email: string;
  senha: string;
  telefone: string;
  responsavelNome?: string;
  responsavelTelefone?: string;
  endereco: EnderecoEmpresa;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  private isLoggedInSubject = new BehaviorSubject<boolean>(false);
  private currentUserSubject = new BehaviorSubject<Usuario | null>(null);

  isLoggedIn$ = this.isLoggedInSubject.asObservable();
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    this.checkStoredUser();
  }

  private checkStoredUser() {
    if (typeof window !== 'undefined' && window.localStorage) {
      const storedUser = localStorage.getItem('currentUser');
      const storedToken = localStorage.getItem('authToken');
      if (storedUser && storedToken) {
        const user = JSON.parse(storedUser);
        this.currentUserSubject.next(user);
        this.isLoggedInSubject.next(true);
      }
    }
  }

  login(email: string, senha: string): Observable<LoginResponse> {
    const loginRequest: LoginRequest = { email, senha };

    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, loginRequest)
      .pipe(
        tap({
          next: (response) => {

            if (typeof window !== 'undefined' && window.localStorage) {
              localStorage.setItem('authToken', response.token);
              localStorage.setItem('currentUser', JSON.stringify({
                email: response.email,
                nome: response.nome,
                nomeUsuario: response.nomeUsuario,
                isAdmin: response.isAdmin
              }));
            }
            this.currentUserSubject.next({
              email: response.email,
              nome: response.nome,
              nomeUsuario: response.nomeUsuario,
              isAdmin: response.isAdmin
            });
            this.isLoggedInSubject.next(true);

          },
          error: (error) => {

          }
        })
      );
  }

  solicitarRecuperacaoSenha(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/recuperar-senha`, { email });
  }

  verificarCodigo(email: string, codigo: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/verificar-codigo`, { email, codigo });
  }

  redefinirSenha(email: string, codigo: string, novaSenha: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/redefinir-senha`, { email, codigo, novaSenha });
  }

  loginSync(email: string, senha: string): boolean {
    this.login(email, senha).subscribe({
      next: () => {},
      error: () => {}
    });
    return false; // Retorna false por enquanto, pois é assíncrono
  }

  logout() {
    if (typeof window !== 'undefined' && window.localStorage) {
      localStorage.removeItem('currentUser');
      localStorage.removeItem('authToken');
    }
    this.currentUserSubject.next(null);
    this.isLoggedInSubject.next(false);
  }

  private extractNameFromEmail(email: string): string {
    const name = email.split('@')[0];
    return name.charAt(0).toUpperCase() + name.slice(1);
  }

  updateProfile(updatedData: Partial<Usuario>): boolean {
    const currentUser = this.currentUserSubject.value;

    if (!currentUser) {
      return false;
    }

    const updatedUser: Usuario = {
      ...currentUser,
      ...updatedData,
      email: currentUser.email // Email não pode ser alterado
    };

    if (typeof window !== 'undefined' && window.localStorage) {
      localStorage.setItem('currentUser', JSON.stringify(updatedUser));
    }
    this.currentUserSubject.next(updatedUser);
    return true;
  }

  get isLoggedIn(): boolean {
    return this.isLoggedInSubject.value;
  }

  get currentUser(): Usuario | null {
    return this.currentUserSubject.value;
  }

  get isAdmin(): boolean {
    const user = this.currentUserSubject.value;
    return user?.isAdmin === true;
  }

  getToken(): string | null {
    if (typeof window !== 'undefined' && window.localStorage) {
      const token = localStorage.getItem('authToken');
      if (token) {
      }
      return token;
    }

    return null;
  }

  cadastro(dados: CadastroRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/cadastro`, dados);
  }

  cadastroEmpresa(dados: CadastroEmpresaRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/cadastro-empresa`, dados);
  }

  getPerfil(): Observable<Usuario> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.getToken()}`,
      'Cache-Control': 'no-cache, no-store, must-revalidate',
      'Pragma': 'no-cache',
      'Expires': '0'
    });

    // Adiciona timestamp para evitar cache do navegador
    const timestamp = new Date().getTime();
    return this.http.get<Usuario>(`${environment.apiUrl}/user/perfil?t=${timestamp}`, { headers });
  }

  updatePerfil(dados: Partial<Usuario>): Observable<any> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.getToken()}`
    });

    return this.http.put(`${environment.apiUrl}/user/atualizar`, dados, { headers });
  }

  deletePerfil(): Observable<any> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.getToken()}`
    });

    // Primeiro busca o perfil para obter o ID
    return this.getPerfil().pipe(
      switchMap((usuario) => {
        if (!usuario.id) {
          throw new Error('ID do usuário não encontrado');
        }
        return this.http.delete(`${environment.apiUrl}/user/${usuario.id}`, { headers });
      })
    );
  }

  diagnosticarAutenticacao(): void {
    // Método de diagnóstico removido
  }
}
