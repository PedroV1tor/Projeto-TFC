import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-recuperar-senha',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './recuperar-senha.component.html',
  styleUrl: './recuperar-senha.component.scss'
})
export class RecuperarSenhaComponent {
  email: string = '';
  isLoading: boolean = false;
  message: string = '';
  isError: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}


  onSubmit() {
    if (!this.email) {
      this.showMessage('Por favor, digite seu e-mail.', true);
      return;
    }


    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.email)) {
      this.showMessage('Por favor, digite um e-mail válido.', true);
      return;
    }

    this.isLoading = true;
    this.message = '';

    this.authService.solicitarRecuperacaoSenha(this.email).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.showMessage('Código de verificação enviado para seu e-mail!', false);


        setTimeout(() => {
          this.router.navigate(['/verificar-codigo'], {
            queryParams: { email: this.email }
          });
        }, 2000);
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Erro ao solicitar recuperação:', error);

        if (error.status === 404) {
          this.showMessage('E-mail não encontrado em nossos registros.', true);
        } else {
          this.showMessage('Erro no servidor. Tente novamente mais tarde.', true);
        }
      }
    });
  }

  private showMessage(msg: string, isError: boolean) {
    this.message = msg;
    this.isError = isError;
  }

  goBack() {
    this.router.navigate(['/login']);
  }
}
