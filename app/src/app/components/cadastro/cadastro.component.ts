import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService, CadastroRequest, CadastroEmpresaRequest } from '../../services/auth.service';

@Component({
  selector: 'app-cadastro',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cadastro.component.html',
  styleUrl: './cadastro.component.scss'
})
export class CadastroComponent {

  // Tipo de cadastro
  tipoCadastro: 'usuario' | 'empresa' = 'usuario';

  // Campos para usuário
  nome: string = '';
  sobrenome: string = '';
  email: string = '';
  senha: string = '';
  usuario: string = '';
  telefone: string = '';
  cep: string = '';
  rua: string = '';
  bairro: string = '';
  matricula: string = '';
  numero: string = '';
  referencia: string = '';
  complemento: string = '';

  // Campos para empresa
  razaoSocial: string = '';
  nomeFantasia: string = '';
  cnpj: string = '';
  responsavelNome: string = '';
  responsavelTelefone: string = '';

  constructor(private router: Router, private authService: AuthService) {}

  onSubmit() {

    if (this.tipoCadastro === 'usuario') {
      console.log('Chamando cadastrarUsuario()');
      this.cadastrarUsuario();
    } else {
      console.log('Chamando cadastrarEmpresa()');
      this.cadastrarEmpresa();
    }
  }

  cadastrarUsuario() {
    if (!this.nome || !this.sobrenome || !this.email || !this.senha ||
        !this.usuario || !this.telefone || !this.cep || !this.rua || !this.bairro) {
      alert('Por favor, preencha todos os campos obrigatórios.');
      return;
    }

    const dadosCadastro: CadastroRequest = {
      nome: this.nome,
      sobrenome: this.sobrenome,
      email: this.email,
      matricula: this.matricula || undefined,
      senha: this.senha,
      nomeUsuario: this.usuario,
      telefone: this.telefone,
      endereco: {
        cep: this.cep,
        rua: this.rua,
        bairro: this.bairro,
        numero: this.numero || undefined,
        referencia: this.referencia || undefined,
        complemento: this.complemento || undefined
      }
    };

    this.authService.cadastro(dadosCadastro).subscribe({
      next: (response) => {
        alert('Cadastro realizado com sucesso!');
        this.router.navigate(['/login']);
      },
      error: (error) => {

        if (error.status === 400) {
          alert('Email ou nome de usuário já existe. Tente outros dados.');
        } else {
          alert('Erro no servidor. Tente novamente mais tarde.');
        }
      }
    });
  }

  cadastrarEmpresa() {

    if (!this.razaoSocial || !this.cnpj || !this.email || !this.senha ||
        !this.telefone || !this.cep || !this.rua || !this.bairro) {
      alert('Por favor, preencha todos os campos obrigatórios.');
      return;
    }

    const dadosCadastro: CadastroEmpresaRequest = {
      razaoSocial: this.razaoSocial,
      nomeFantasia: this.nomeFantasia || undefined,
      cnpj: this.cnpj,
      email: this.email,
      senha: this.senha,
      telefone: this.telefone,
      responsavelNome: this.responsavelNome || undefined,
      responsavelTelefone: this.responsavelTelefone || undefined,
      endereco: {
        cep: this.cep,
        rua: this.rua,
        bairro: this.bairro,
        numero: this.numero || undefined,
        referencia: this.referencia || undefined,
        complemento: this.complemento || undefined
      }
    };

    this.authService.cadastroEmpresa(dadosCadastro).subscribe({
      next: (response) => {
        alert('Empresa cadastrada com sucesso!');
        this.router.navigate(['/login']);
      },
      error: (error) => {

        if (error.status === 400) {
          alert('Email ou CNPJ já existe. Tente outros dados.');
        } else {
          alert('Erro no servidor. Tente novamente mais tarde.');
        }
      }
    });
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }

  goToHome() {
    this.router.navigate(['/']);
  }
}
