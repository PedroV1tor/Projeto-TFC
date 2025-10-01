import { HttpErrorResponse, HttpEvent, HttpInterceptorFn } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

export const authInterceptor: HttpInterceptorFn = (req, next): Observable<HttpEvent<unknown>> => {
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
    } else if (!isPublicUrl) {
      // Só mostra warning para URLs que precisam de autenticação
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
        }
      }
    })
  );
};


