import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { PublicacaoService } from '../../services/publicacao.service';
import { AuthService } from '../../services/auth.service';
import { Publicacao } from '../../models/publicacao.model';

@Component({
  selector: 'app-publicacoes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './publicacoes.component.html',
  styleUrl: './publicacoes.component.scss'
})
export class PublicacoesComponent implements OnInit {
  publicacoes: Publicacao[] = [];
  mostrarFormulario = false;
  publicacaoEditando: Publicacao | null = null;


  mostrarPopup = false;
  publicacaoSelecionada: Publicacao | null = null;


  novaPublicacao = {
    titulo: '',
    resumo: '',
    descricao: ''
  };

  constructor(
    private publicacaoService: PublicacaoService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.carregarPublicacoes();
  }

  carregarPublicacoes() {
    this.publicacaoService.getPublicacoes().subscribe({
      next: (publicacoes) => this.publicacoes = publicacoes,
      error: (error) => console.error('Erro ao carregar publicações:', error)
    });
  }

  abrirFormulario() {
    this.mostrarFormulario = true;
    this.publicacaoEditando = null;
    this.resetarFormulario();
  }

  fecharFormulario() {
    this.mostrarFormulario = false;
    this.publicacaoEditando = null;
    this.resetarFormulario();
  }

  resetarFormulario() {
    this.novaPublicacao = {
      titulo: '',
      resumo: '',
      descricao: ''
    };
  }

  salvarPublicacao() {
    if (!this.novaPublicacao.titulo || !this.novaPublicacao.resumo || !this.novaPublicacao.descricao) {
      alert('Por favor, preencha todos os campos obrigatórios.');
      return;
    }


    if (this.novaPublicacao.resumo.length < 10) {
      alert('O resumo deve ter pelo menos 10 caracteres.');
      return;
    }


    if (this.novaPublicacao.descricao.length < 50) {
      alert('A descrição deve ter pelo menos 50 caracteres.');
      return;
    }


    console.log('Usuário logado:', this.authService.isLoggedIn);
    console.log('Token atual:', this.authService.getToken());
    console.log('Dados da publicação:', this.novaPublicacao);

    try {
      if (this.publicacaoEditando) {

        this.publicacaoService.atualizarPublicacao(
          this.publicacaoEditando.id,
          this.novaPublicacao
        ).subscribe({
          next: () => {
            alert('Publicação atualizada com sucesso!');
            this.carregarPublicacoes();
            this.fecharFormulario();
          },
          error: (error) => {
            alert('Erro ao atualizar publicação.');
            console.error(error);
          }
        });
      } else {

        this.publicacaoService.criarPublicacao(this.novaPublicacao).subscribe({
          next: () => {
            alert('Publicação criada com sucesso!');
            this.carregarPublicacoes();
            this.fecharFormulario();
          },
          error: (error) => {
            alert('Erro ao criar publicação.');
            console.error(error);
          }
        });
      }
    } catch (error: any) {
      if (error.message.includes('logado')) {
        alert('Você precisa estar logado para realizar esta ação.');
        this.router.navigate(['/login']);
      } else {
        alert('Erro ao salvar publicação: ' + error.message);
      }
    }
  }

  editarPublicacao(publicacao: Publicacao) {
    this.publicacaoEditando = publicacao;
    this.novaPublicacao = {
      titulo: publicacao.titulo,
      resumo: publicacao.resumo,
      descricao: publicacao.descricao
    };
    this.mostrarFormulario = true;
  }

  excluirPublicacao(id: number) {
    if (confirm('Tem certeza que deseja excluir esta publicação?')) {
      this.publicacaoService.excluirPublicacao(id).subscribe({
        next: () => {
          alert('Publicação excluída com sucesso!');
          this.carregarPublicacoes();
        },
        error: (error) => {
          alert('Erro ao excluir publicação.');
          console.error(error);
        }
      });
    }
  }

  alterarStatus(id: number, novoStatus: 'ativa' | 'rascunho' | 'arquivada') {
    this.publicacaoService.alterarStatus(id, novoStatus).subscribe({
      next: () => {
        alert(`Status alterado para ${novoStatus}!`);
        this.carregarPublicacoes();
      },
      error: (error) => {
        alert('Erro ao alterar status.');
        console.error(error);
      }
    });
  }

  formatarData(data: string): string {
    return new Date(data).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatarDataCompleta(data: string): string {
    return new Date(data).toLocaleDateString('pt-BR', {
      weekday: 'long',
      day: 'numeric',
      month: 'long',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatarDataSimples(data: string): string {
    return new Date(data).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  formatarDataRelativa(data: string): string {
    const agora = new Date();
    const dataPublicacao = new Date(data);
    const diferenca = agora.getTime() - dataPublicacao.getTime();

    const minutos = Math.floor(diferenca / (1000 * 60));
    const horas = Math.floor(diferenca / (1000 * 60 * 60));
    const dias = Math.floor(diferenca / (1000 * 60 * 60 * 24));

    if (minutos < 60) {
      return minutos === 0 ? 'Agora' : `${minutos}min atrás`;
    } else if (horas < 24) {
      return `${horas}h atrás`;
    } else if (dias < 7) {
      return `${dias}d atrás`;
    } else {
      return this.formatarDataSimples(data);
    }
  }


  abrirPopup(publicacao: Publicacao): void {
    this.publicacaoSelecionada = publicacao;
    this.mostrarPopup = true;

    document.body.style.overflow = 'hidden';
  }

  fecharPopup(): void {
    this.mostrarPopup = false;
    this.publicacaoSelecionada = null;

    document.body.style.overflow = 'auto';
  }


  fecharPopupOverlay(event: Event): void {
    if (event.target === event.currentTarget) {
      this.fecharPopup();
    }
  }

  contarPalavras(texto: string): number {
    return texto.trim().split(/\s+/).filter(palavra => palavra.length > 0).length;
  }

  calcularTempoLeitura(texto: string): number {
    const palavras = this.contarPalavras(texto);
    const palavrasPorMinuto = 200; // Velocidade média de leitura em português
    return Math.max(1, Math.round(palavras / palavrasPorMinuto));
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'ativa': return 'status-ativa';
      case 'rascunho': return 'status-rascunho';
      case 'arquivada': return 'status-arquivada';
      default: return '';
    }
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'ativa': return 'Ativa';
      case 'rascunho': return 'Rascunho';
      case 'arquivada': return 'Arquivada';
      default: return status;
    }
  }

  truncarTexto(texto: string, limite: number): string {
    if (texto.length <= limite) return texto;
    return texto.substring(0, limite) + '...';
  }
}
