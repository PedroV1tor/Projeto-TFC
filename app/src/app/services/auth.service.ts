import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface Usuario {
  id?: number;
  email: string;
  nome?: string;
  sobrenome?: string;
  nomeUsuario?: string;
  telefone?: string;
  matricula?: string;
  cep?: string;
  rua?: string;
  bairro?: string;
  numero?: string;
  complemento?: string;
  referencia?: string;
  dataCriacao?: string;
  ultimoLogin?: string;
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
  cep: string;
  rua: string;
  bairro: string;
  numero?: string;
  referencia?: string;
  complemento?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  private isLoggedInSubject = new BehaviorSubject<boolean>(false);
  private currentUserSubject = new BehaviorSubject<Usuario | null>(null);

  // Observables públicos
  isLoggedIn$ = this.isLoggedInSubject.asObservable();
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    // Verifica se há um usuário logado no localStorage ao inicializar
    this.checkStoredUser();
  }

  private checkStoredUser() {
    // Verifica se está no ambiente do navegador
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
        tap(response => {
          // Salva o token e dados do usuário
          if (typeof window !== 'undefined' && window.localStorage) {
            localStorage.setItem('authToken', response.token);
            localStorage.setItem('currentUser', JSON.stringify({
              email: response.email,
              nome: response.nome,
              nomeUsuario: response.nomeUsuario
            }));
          }

          // Atualiza os subjects
          this.currentUserSubject.next({
            email: response.email,
            nome: response.nome,
            nomeUsuario: response.nomeUsuario
          });
          this.isLoggedInSubject.next(true);
        })
      );
  }

  // Método para solicitar recuperação de senha
  solicitarRecuperacaoSenha(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/recuperar-senha`, { email });
  }

  // Método para verificar código de recuperação
  verificarCodigo(email: string, codigo: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/verificar-codigo`, { email, codigo });
  }

  // Método para redefinir senha
  redefinirSenha(email: string, codigo: string, novaSenha: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/redefinir-senha`, { email, codigo, novaSenha });
  }

  // Método compatível com o código existente
  loginSync(email: string, senha: string): boolean {
    this.login(email, senha).subscribe({
      next: () => {},
      error: () => {}
    });
    return false; // Retorna false por enquanto, pois é assíncrono
  }

  logout() {
    // Remove do localStorage (apenas no navegador)
    if (typeof window !== 'undefined' && window.localStorage) {
      localStorage.removeItem('currentUser');
      localStorage.removeItem('authToken');
    }

    // Atualiza os subjects
    this.currentUserSubject.next(null);
    this.isLoggedInSubject.next(false);
  }

  private extractNameFromEmail(email: string): string {
    // Extrai o nome do email (parte antes do @)
    const name = email.split('@')[0];
    // Capitaliza a primeira letra
    return name.charAt(0).toUpperCase() + name.slice(1);
  }

  // Atualizar perfil do usuário
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

    // Salva no localStorage (apenas no navegador)
    if (typeof window !== 'undefined' && window.localStorage) {
      localStorage.setItem('currentUser', JSON.stringify(updatedUser));
    }

    // Atualiza o subject
    this.currentUserSubject.next(updatedUser);
    return true;
  }

  // Getters para acessar os valores atuais
  get isLoggedIn(): boolean {
    return this.isLoggedInSubject.value;
  }

  get currentUser(): Usuario | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    if (typeof window !== 'undefined' && window.localStorage) {
      return localStorage.getItem('authToken');
    }
    return null;
  }

  // Método para cadastro
  cadastro(dados: CadastroRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/cadastro`, dados);
  }

  // Método para obter perfil do usuário
  getPerfil(): Observable<Usuario> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.getToken()}`
    });

    return this.http.get<Usuario>(`${environment.apiUrl}/user/perfil`, { headers });
  }

  // Método para atualizar perfil
  updatePerfil(dados: Partial<Usuario>): Observable<any> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.getToken()}`
    });

    return this.http.put(`${environment.apiUrl}/user/atualizar`, dados, { headers });
  }
}
