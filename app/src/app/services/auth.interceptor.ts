import { HttpErrorResponse, HttpEvent, HttpInterceptorFn } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next): Observable<HttpEvent<unknown>> => {
  const router = inject(Router);
  const authService = inject(AuthService);

  let requestToSend = req;

  // URLs que não precisam de autenticação
  const publicUrls = [
    '/api/auth/login',
    '/api/auth/cadastro',
    '/api/auth/recuperar-senha',
    '/api/auth/verificar-codigo',
    '/api/auth/redefinir-senha',
    '/assets/'
  ];

  const isPublicUrl = publicUrls.some(url => req.url.includes(url));

  try {
    const token = typeof window !== 'undefined' && window.localStorage
      ? localStorage.getItem('authToken')
      : null;

    if (token) {
      requestToSend = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
      console.log('AuthInterceptor: adicionando token para', req.url);
    } else if (!isPublicUrl) {
      console.warn('AuthInterceptor: token ausente para URL protegida ->', req.url);
    }
  } catch (e) {
    console.warn('AuthInterceptor: erro ao ler token do localStorage', e);
  }

  return next(requestToSend).pipe(
    tap({
      error: (err) => {
        if (err instanceof HttpErrorResponse && err.status === 401) {
          console.warn('AuthInterceptor: resposta 401 para', req.url);

          // Se não for uma URL pública e recebeu 401, provavelmente o token expirou
          if (!isPublicUrl) {
            console.log('AuthInterceptor: token expirado ou inválido, fazendo logout e redirecionando para login');

            // Fazer logout para limpar dados inválidos
            authService.logout();

            // Redirecionar para página de login apenas se não estivermos já nela
            if (!router.url.includes('/login')) {
              router.navigate(['/login']);
            }
          }
        }
      }
    })
  );
};


