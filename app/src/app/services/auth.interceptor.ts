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

    } else if (!isPublicUrl) {

    }
  } catch (e) {

  }

  return next(requestToSend).pipe(
    tap({
      error: (err) => {
        if (err instanceof HttpErrorResponse && err.status === 401) {

          if (!isPublicUrl) {

            authService.logout();

            if (!router.url.includes('/login')) {
              router.navigate(['/login']);
            }
          }
        }
      }
    })
  );
};


