import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { OrcamentoService } from '../../services/orcamento.service';
import { AuthService } from '../../services/auth.service';
import { Orcamento } from '../../models/orcamento.model';

@Component({
  selector: 'app-orcamento',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './orcamento.component.html',
  styleUrl: './orcamento.component.scss'
})
export class OrcamentoComponent implements OnInit {
  orcamentos: Orcamento[] = [];
  mostrarFormulario = false;
  orcamentoEditando: Orcamento | null = null;
  filtroStatus = 'todos';
  mostrarEstatisticas = false;
  estatisticas: any = null;


  novoOrcamento = {
    titulo: '',
    descricao: '',
    prazoEntrega: '',
    prazoOrcamento: '',
    valor: undefined as number | undefined,
    cliente: ''
  };

  constructor(
    private orcamentoService: OrcamentoService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.carregarOrcamentos();
    this.carregarEstatisticas();
  }

  carregarOrcamentos() {
    if (this.filtroStatus === 'todos') {
      this.orcamentoService.getOrcamentos().subscribe({
        next: (orcamentos) => this.orcamentos = orcamentos,
        error: (error) => console.error('Erro ao carregar orçamentos:', error)
      });
    } else {
      this.orcamentoService.getOrcamentosByStatus(
        this.filtroStatus as 'pendente' | 'aprovado' | 'rejeitado' | 'concluido'
      ).subscribe({
        next: (orcamentos) => this.orcamentos = orcamentos,
        error: (error) => console.error('Erro ao carregar orçamentos:', error)
      });
    }
  }

  carregarEstatisticas() {
    this.orcamentoService.getEstatisticas().subscribe({
      next: (estatisticas) => this.estatisticas = estatisticas,
      error: (error) => console.error('Erro ao carregar estatísticas:', error)
    });
  }

  filtrarPorStatus(status: string) {
    this.filtroStatus = status;
    this.carregarOrcamentos();
  }

  abrirFormulario() {
    this.mostrarFormulario = true;
    this.orcamentoEditando = null;
    this.resetarFormulario();
  }

  fecharFormulario() {
    this.mostrarFormulario = false;
    this.orcamentoEditando = null;
    this.resetarFormulario();
  }

  resetarFormulario() {
    this.novoOrcamento = {
      titulo: '',
      descricao: '',
      prazoEntrega: '',
      prazoOrcamento: '',
      valor: undefined,
      cliente: ''
    };
  }

  salvarOrcamento() {
    if (!this.novoOrcamento.titulo || !this.novoOrcamento.descricao ||
        !this.novoOrcamento.prazoEntrega || !this.novoOrcamento.prazoOrcamento) {
      alert('Por favor, preencha todos os campos obrigatórios.');
      return;
    }


    const prazoOrcamento = new Date(this.novoOrcamento.prazoOrcamento);
    const prazoEntrega = new Date(this.novoOrcamento.prazoEntrega);
    const hoje = new Date();

    if (prazoOrcamento < hoje) {
      alert('O prazo do orçamento não pode ser anterior à data atual.');
      return;
    }

    if (prazoEntrega <= prazoOrcamento) {
      alert('O prazo de entrega deve ser posterior ao prazo do orçamento.');
      return;
    }


    if (this.novoOrcamento.descricao.length < 20) {
      alert('A descrição deve ter pelo menos 20 caracteres.');
      return;
    }


    if (this.novoOrcamento.valor !== undefined && this.novoOrcamento.valor <= 0) {
      alert('O valor deve ser maior que zero.');
      return;
    }

    try {
      if (this.orcamentoEditando) {

        this.orcamentoService.atualizarOrcamento(
          this.orcamentoEditando.id,
          this.novoOrcamento
        ).subscribe({
          next: () => {
            alert('Orçamento atualizado com sucesso!');
            this.carregarOrcamentos();
            this.carregarEstatisticas();
            this.fecharFormulario();
          },
          error: (error) => {
            alert('Erro ao atualizar orçamento.');
            console.error(error);
          }
        });
      } else {

        this.orcamentoService.criarOrcamento(this.novoOrcamento).subscribe({
          next: () => {
            alert('Orçamento criado com sucesso!');
            this.carregarOrcamentos();
            this.carregarEstatisticas();
            this.fecharFormulario();
          },
          error: (error) => {
            alert('Erro ao criar orçamento.');
            console.error(error);
          }
        });
      }
    } catch (error: any) {
      if (error.message.includes('logado')) {
        alert('Você precisa estar logado para realizar esta ação.');
        this.router.navigate(['/login']);
      } else {
        alert('Erro ao salvar orçamento: ' + error.message);
      }
    }
  }

  editarOrcamento(orcamento: Orcamento) {
    this.orcamentoEditando = orcamento;
    this.novoOrcamento = {
      titulo: orcamento.titulo,
      descricao: orcamento.descricao,
      prazoEntrega: orcamento.prazoEntrega,
      prazoOrcamento: orcamento.prazoOrcamento,
      valor: orcamento.valor,
      cliente: orcamento.cliente || ''
    };
    this.mostrarFormulario = true;
  }

  excluirOrcamento(id: number) {
    if (confirm('Tem certeza que deseja excluir este orçamento?')) {
      this.orcamentoService.excluirOrcamento(id).subscribe({
        next: () => {
          alert('Orçamento excluído com sucesso!');
          this.carregarOrcamentos();
          this.carregarEstatisticas();
        },
        error: (error) => {
          alert('Erro ao excluir orçamento.');
          console.error(error);
        }
      });
    }
  }

  alterarStatus(id: number, novoStatus: 'pendente' | 'aprovado' | 'rejeitado' | 'concluido') {
    this.orcamentoService.alterarStatus(id, novoStatus).subscribe({
      next: () => {
        alert(`Status alterado para ${this.getStatusText(novoStatus)}!`);
        this.carregarOrcamentos();
        this.carregarEstatisticas();
      },
      error: (error) => {
        alert('Erro ao alterar status.');
        console.error(error);
      }
    });
  }

  aprovarOrcamento(id: number) {
    if (confirm('Tem certeza que deseja aprovar este orçamento?')) {
      this.alterarStatus(id, 'aprovado');
    }
  }

  rejeitarOrcamento(id: number) {
    if (confirm('Tem certeza que deseja rejeitar este orçamento?')) {
      this.alterarStatus(id, 'rejeitado');
    }
  }

  concluirOrcamento(id: number) {
    if (confirm('Tem certeza que deseja marcar este orçamento como concluído?')) {
      this.alterarStatus(id, 'concluido');
    }
  }

  toggleEstatisticas() {
    this.mostrarEstatisticas = !this.mostrarEstatisticas;
  }

  getEstatisticas() {
    return this.estatisticas;
  }

  formatarData(data: string): string {
    return new Date(data).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  formatarDataHora(data: string): string {
    return new Date(data).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatarMoeda(valor: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(valor);
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'pendente': return 'status-pendente';
      case 'aprovado': return 'status-aprovado';
      case 'rejeitado': return 'status-rejeitado';
      case 'concluido': return 'status-concluido';
      default: return '';
    }
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'pendente': return 'Pendente';
      case 'aprovado': return 'Aprovado';
      case 'rejeitado': return 'Rejeitado';
      case 'concluido': return 'Concluído';
      default: return status;
    }
  }

  truncarTexto(texto: string, limite: number): string {
    if (texto.length <= limite) return texto;
    return texto.substring(0, limite) + '...';
  }

  isPrazoVencido(data: string): boolean {
    return new Date(data) < new Date();
  }

  diasRestantes(data: string): number {
    const hoje = new Date();
    const prazo = new Date(data);
    const diffTime = prazo.getTime() - hoje.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }
}
