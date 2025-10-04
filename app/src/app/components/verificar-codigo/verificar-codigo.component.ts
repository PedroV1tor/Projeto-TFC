import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-verificar-codigo',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './verificar-codigo.component.html',
  styleUrl: './verificar-codigo.component.scss'
})
export class VerificarCodigoComponent implements OnInit {
  codigo: string = '';
  email: string = '';
  isLoading: boolean = false;
  message: string = '';
  isError: boolean = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService
  ) {}

  ngOnInit() {

    this.route.queryParams.subscribe(params => {
      this.email = params['email'] || '';
      if (!this.email) {

        this.router.navigate(['/recuperar-senha']);
      }
    });
  }

  onVerificar() {
    if (!this.codigo) {
      this.showMessage('Por favor, digite o código de verificação.', true);
      return;
    }

    if (this.codigo.length !== 5) {
      this.showMessage('O código deve ter 5 dígitos.', true);
      return;
    }

    this.isLoading = true;
    this.message = '';

    this.authService.verificarCodigo(this.email, this.codigo).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.showMessage('Código verificado com sucesso!', false);


        setTimeout(() => {
          this.router.navigate(['/redefinir-senha'], {
            queryParams: { email: this.email, codigo: this.codigo }
          });
        }, 1000);
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Erro ao verificar código:', error);

        if (error.status === 400) {
          this.showMessage('Código inválido ou expirado. Tente novamente.', true);
        } else {
          this.showMessage('Erro no servidor. Tente novamente mais tarde.', true);
        }
        this.codigo = '';
      }
    });
  }

  reenviarCodigo() {
    this.isLoading = true;
    this.message = '';

    this.authService.solicitarRecuperacaoSenha(this.email).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.showMessage('Novo código enviado para seu e-mail!', false);
        this.codigo = '';
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Erro ao reenviar código:', error);
        this.showMessage('Erro ao reenviar código. Tente novamente.', true);
      }
    });
  }

  private showMessage(msg: string, isError: boolean) {
    this.message = msg;
    this.isError = isError;
  }

  voltarParaLogin() {
    this.router.navigate(['/login']);
  }
}
