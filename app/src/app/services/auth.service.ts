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

    console.log('AuthService: tentando fazer login para', email);

    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, loginRequest)
      .pipe(
        tap({
          next: (response) => {
            console.log('AuthService: login bem-sucedido para', email);


            if (typeof window !== 'undefined' && window.localStorage) {
              localStorage.setItem('authToken', response.token);
              localStorage.setItem('currentUser', JSON.stringify({
                email: response.email,
                nome: response.nome,
                nomeUsuario: response.nomeUsuario
              }));
              console.log('AuthService: token salvo no localStorage:', response.token.substring(0, 20) + '...');
            }


            this.currentUserSubject.next({
              email: response.email,
              nome: response.nome,
              nomeUsuario: response.nomeUsuario
            });
            this.isLoggedInSubject.next(true);

            console.log('AuthService: estado de login atualizado para true');
          },
          error: (error) => {
            console.error('AuthService: erro no login para', email, error);
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

  getToken(): string | null {
    if (typeof window !== 'undefined' && window.localStorage) {
      const token = localStorage.getItem('authToken');
      if (token) {
        console.log('AuthService: retornando token existente:', token.substring(0, 20) + '...');
      } else {
        console.log('AuthService: nenhum token encontrado no localStorage');
      }
      return token;
    }
    console.log('AuthService: ambiente não é navegador, retornando null');
    return null;
  }


  cadastro(dados: CadastroRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/cadastro`, dados);
  }


  getPerfil(): Observable<Usuario> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.getToken()}`
    });

    return this.http.get<Usuario>(`${environment.apiUrl}/user/perfil`, { headers });
  }


  updatePerfil(dados: Partial<Usuario>): Observable<any> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.getToken()}`
    });

    return this.http.put(`${environment.apiUrl}/user/atualizar`, dados, { headers });
  }


  diagnosticarAutenticacao(): void {
    console.log('=== DIAGNÓSTICO DE AUTENTICAÇÃO ===');
    console.log('URL da API:', this.apiUrl);
    console.log('Ambiente de produção:', environment.production);

    if (typeof window !== 'undefined' && window.localStorage) {
      const token = localStorage.getItem('authToken');
      const user = localStorage.getItem('currentUser');

      console.log('Token no localStorage:', token ? 'Presente' : 'Ausente');
      if (token) {
        console.log('Token (primeiros 20 chars):', token.substring(0, 20) + '...');
      }

      console.log('Dados do usuário no localStorage:', user ? 'Presente' : 'Ausente');
      if (user) {
        try {
          const userData = JSON.parse(user);
          console.log('Dados do usuário:', userData);
        } catch (e) {
          console.log('Erro ao fazer parse dos dados do usuário:', e);
        }
      }
    } else {
      console.log('Ambiente não é navegador');
    }

    console.log('Estado atual isLoggedIn:', this.isLoggedIn);
    console.log('Usuário atual:', this.currentUser);
    console.log('=====================================');
  }
}
