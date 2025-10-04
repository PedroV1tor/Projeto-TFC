import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import {
  RelatorioService,
  RelatorioResponseDTO,
  RelatorioUsuarioDTO,
  RelatorioPublicacaoDTO,
  RelatorioAgendamentoDTO,
  RelatorioOrcamentoDTO,
  FiltroRelatorio
} from '../../services/relatorio.service';
import { PublicacaoService } from '../../services/publicacao.service';
import { Publicacao } from '../../models/publicacao.model';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

interface TipoRelatorio {
  id: string;
  nome: string;
  descricao: string;
  icone: string;
  cor: string;
}

@Component({
  selector: 'app-relatorios',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './relatorios.component.html',
  styleUrl: './relatorios.component.scss'
})
export class RelatoriosComponent implements OnInit {

  filtro: FiltroRelatorio = {};


  carregando = false;
  relatorioSelecionado = '';
  mostrarFiltros = false;


  dadosRelatorio: any[] = [];
  publicacoesAtivas: Publicacao[] = [];


  tiposRelatorio: TipoRelatorio[] = [
    {
      id: 'usuarios',
      nome: 'Relatório de Usuários',
      descricao: 'Lista detalhada de usuários cadastrados',
      icone: '👥',
      cor: '#28a745'
    },
    {
      id: 'publicacoes',
      nome: 'Relatório de Publicações',
      descricao: 'Estatísticas das publicações do sistema',
      icone: '📝',
      cor: '#17a2b8'
    },
    {
      id: 'agendamentos',
      nome: 'Relatório de Agendamentos',
      descricao: 'Controle de agendamentos realizados',
      icone: '📅',
      cor: '#fd7e14'
    },
    {
      id: 'orcamentos',
      nome: 'Relatório de Orçamentos',
      descricao: 'Análise financeira dos orçamentos',
      icone: '💰',
      cor: '#6f42c1'
    }
  ];

  constructor(
    private relatorioService: RelatorioService,
    private publicacaoService: PublicacaoService,
    private router: Router
  ) {}

  ngOnInit() {

  }

  selecionarRelatorio(tipoId: string) {
    this.relatorioSelecionado = tipoId;
    this.mostrarFiltros = true;
    this.dadosRelatorio = [];
  }

  aplicarFiltros() {
    if (!this.relatorioSelecionado) return;

    this.carregando = true;

    switch (this.relatorioSelecionado) {
      case 'usuarios':
        this.gerarRelatorioUsuarios();
        break;
      case 'publicacoes':
        this.gerarRelatorioPublicacoes();
        break;
      case 'agendamentos':
        this.gerarRelatorioAgendamentos();
        break;
      case 'orcamentos':
        this.gerarRelatorioOrcamentos();
        break;
    }
  }


  gerarRelatorioUsuarios() {
    this.relatorioService.getRelatorioUsuarios(this.filtro).subscribe({
      next: (relatorio) => {
        this.dadosRelatorio = relatorio.items;
        this.carregando = false;
      },
      error: (error) => {
        console.error('Erro ao gerar relatório de usuários:', error);
        this.carregando = false;
      }
    });
  }

  gerarRelatorioPublicacoes() {
    this.relatorioService.getRelatorioPublicacoes(this.filtro).subscribe({
      next: (relatorio) => {
        this.dadosRelatorio = relatorio.items;
        this.carregando = false;
      },
      error: (error) => {
        console.error('Erro ao gerar relatório de publicações:', error);
        this.carregando = false;
      }
    });
  }

  gerarRelatorioAgendamentos() {
    this.relatorioService.getRelatorioAgendamentos(this.filtro).subscribe({
      next: (relatorio) => {
        this.dadosRelatorio = relatorio.items;
        this.carregando = false;
      },
      error: (error) => {
        console.error('Erro ao gerar relatório de agendamentos:', error);
        this.carregando = false;
      }
    });
  }

  gerarRelatorioOrcamentos() {
    this.relatorioService.getRelatorioOrcamentos(this.filtro).subscribe({
      next: (relatorio) => {
        this.dadosRelatorio = relatorio.items;
        this.carregando = false;
      },
      error: (error) => {
        console.error('Erro ao gerar relatório de orçamentos:', error);
        this.carregando = false;
      }
    });
  }

  exportarPDF() {
    if (!this.relatorioSelecionado) return;

    const doc = new jsPDF();
    const dataAtual = new Date().toLocaleDateString('pt-BR');


    doc.setFontSize(16);
    doc.setFont('helvetica', 'bold');


    doc.text('INOVALAB - Relatórios', 20, 20);
    doc.setFontSize(12);
    doc.setFont('helvetica', 'normal');
    doc.text(`Gerado em: ${dataAtual}`, 20, 30);

    if (this.filtro.dataInicial && this.filtro.dataFinal) {
      doc.text(`Período: ${this.formatarData(this.filtro.dataInicial)} a ${this.formatarData(this.filtro.dataFinal)}`, 20, 40);
    }


    switch (this.relatorioSelecionado) {
      case 'usuarios':
        this.gerarPDFUsuarios(doc);
        break;
      case 'publicacoes':
        this.gerarPDFPublicacoes(doc);
        break;
      case 'agendamentos':
        this.gerarPDFAgendamentos(doc);
        break;
      case 'orcamentos':
        this.gerarPDFOrcamentos(doc);
        break;
    }


    const nomeArquivo = `relatorio-${this.relatorioSelecionado}-${Date.now()}.pdf`;
    doc.save(nomeArquivo);
  }


  private gerarPDFUsuarios(doc: jsPDF) {
    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.text('Relatório de Usuários', 20, 60);

    const dados = this.dadosRelatorio.map(usuario => [
      usuario.nome,
      usuario.email,
      usuario.telefone || 'N/A',
      usuario.status,
      this.formatarData(usuario.criadoEm)
    ]);

    autoTable(doc, {
      startY: 70,
      head: [['Nome', 'Email', 'Telefone', 'Status', 'Criado em']],
      body: dados,
      theme: 'striped',
      headStyles: { fillColor: [40, 167, 69] },
      styles: { fontSize: 8 }
    });
  }

  private gerarPDFPublicacoes(doc: jsPDF) {
    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.text('Relatório de Publicações', 20, 60);

    const dados = this.dadosRelatorio.map(pub => [
      pub.titulo,
      pub.autor,
      pub.status,
      this.formatarData(pub.criadoEm)
    ]);

    autoTable(doc, {
      startY: 70,
      head: [['Título', 'Autor', 'Status', 'Criado em']],
      body: dados,
      theme: 'striped',
      headStyles: { fillColor: [23, 162, 184] },
      styles: { fontSize: 8 }
    });
  }

  private gerarPDFAgendamentos(doc: jsPDF) {
    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.text('Relatório de Agendamentos', 20, 60);

    const dados = this.dadosRelatorio.map(agend => [
      agend.titulo,
      agend.usuario,
      this.formatarData(agend.data),
      agend.status,
      this.formatarData(agend.criadoEm)
    ]);

    autoTable(doc, {
      startY: 70,
      head: [['Título', 'Usuário', 'Data', 'Status', 'Criado em']],
      body: dados,
      theme: 'striped',
      headStyles: { fillColor: [253, 126, 20] },
      styles: { fontSize: 8 }
    });
  }

  private gerarPDFOrcamentos(doc: jsPDF) {
    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.text('Relatório de Orçamentos', 20, 60);

    const dados = this.dadosRelatorio.map(orc => [
      orc.titulo,
      orc.cliente,
      `R$ ${orc.valor.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`,
      orc.status,
      this.formatarData(orc.criadoEm)
    ]);

    autoTable(doc, {
      startY: 70,
      head: [['Título', 'Cliente', 'Valor', 'Status', 'Criado em']],
      body: dados,
      theme: 'striped',
      headStyles: { fillColor: [111, 66, 193] },
      styles: { fontSize: 8 }
    });
  }

  formatarData(data: string): string {
    return new Date(data).toLocaleDateString('pt-BR');
  }

  formatarDataHora(data: string): string {
    return new Date(data).toLocaleString('pt-BR');
  }

  limparFiltros() {
    this.filtro = {};
    this.relatorioSelecionado = '';
    this.mostrarFiltros = false;
    this.dadosRelatorio = [];
    this.publicacoesAtivas = [];
  }

  getTipoRelatorio(id: string): TipoRelatorio | undefined {
    return this.tiposRelatorio.find(tipo => tipo.id === id);
  }


  buscarPublicacoesAtivas(): void {
    this.carregando = true;
    this.publicacaoService.getPublicacoesAtivas().subscribe({
      next: (publicacoes) => {
        this.publicacoesAtivas = publicacoes;
        this.carregando = false;
        console.log('Publicações ativas carregadas:', publicacoes);
      },
      error: (error) => {
        console.error('Erro ao buscar publicações ativas:', error);
        this.carregando = false;
      }
    });
  }


  exibirPublicacoesAtivas(): void {
    this.buscarPublicacoesAtivas();
  }
}
