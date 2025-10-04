import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeroComponent } from '../hero/hero.component';
import { PublicacaoService } from '../../services/publicacao.service';
import { Publicacao } from '../../models/publicacao.model';

interface CardInovacao {
  id: number;
  data: string;
  titulo: string;
  resumo: string;
  conteudoCompleto: string;
  icone: string;
  categoria: string;
  featured?: boolean;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [HeroComponent, CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  publicacoesTimeline: Publicacao[] = [];
  mostrarTodasPublicacoes = false;


  mostrarPopup = false;
  publicacaoSelecionada: Publicacao | null = null;


  mostrarPopupInovacao = false;
  cardSelecionado: CardInovacao | null = null;


  cardsInovacao: CardInovacao[] = [
    {
      id: 1,
      data: '06/02/2025',
      titulo: 'Oportunidades de Trabalho',
      resumo: 'UFG abre vagas para profissionais que atuarão em seus ambientes de inovação',
      conteudoCompleto: 'A Universidade Federal de Goiás (UFG) está com várias oportunidades abertas para profissionais qualificados que desejam atuar em seus ambientes de inovação tecnológica. As vagas contemplam diferentes áreas como desenvolvimento de software, pesquisa científica, gestão de projetos e empreendedorismo. Os profissionais selecionados terão a oportunidade de trabalhar com tecnologias de ponta, contribuir para projetos de impacto social e colaborar com uma equipe multidisciplinar de especialistas. As posições oferecem excelentes benefícios, ambiente de trabalho colaborativo e oportunidades de crescimento profissional.',
      icone: '💼',
      categoria: 'Carreira'
    },
    {
      id: 2,
      data: '29/04/2024',
      titulo: 'IPELab no Espaço das Profissões UFG 2024',
      resumo: 'A Universidade Federal de Goiás (UFG) abriu suas portas para receber o evento anual "Espaço das Profissões"',
      conteudoCompleto: 'O IPELab marcou presença no tradicional evento "Espaço das Profissões UFG 2024", uma das maiores feiras de profissões do Centro-Oeste brasileiro. Durante o evento, nossa equipe apresentou as mais diversas oportunidades de carreira na área de inovação e tecnologia, demonstrando projetos práticos desenvolvidos em nossos laboratórios. Estudantes de ensino médio e superior puderam conhecer de perto as atividades do laboratório, participar de workshops práticos e conversar com profissionais experientes sobre as tendências do mercado de trabalho em tecnologia. O evento foi um grande sucesso, atraindo mais de 5.000 visitantes e consolidando o IPELab como referência em inovação educacional.',
      icone: '🔬',
      categoria: 'Evento',
      featured: true
    },
    {
      id: 3,
      data: '24/03/2023',
      titulo: 'Festival SESI de Robótica',
      resumo: 'Equipes apoiadas pelo IPElab são premiadas nacionalmente no Festival SESI de Robótica',
      conteudoCompleto: 'O Festival SESI de Robótica 2023 foi marcado pelo excelente desempenho das equipes apoiadas pelo IPELab. Após meses de preparação intensiva, mentoria técnica e desenvolvimento de soluções inovadoras, nossas equipes conquistaram posições de destaque na competição nacional. O evento reuniu mais de 1.000 estudantes de todo o Brasil, que apresentaram projetos robotizados para solucionar desafios reais da sociedade. As equipes do IPELab se destacaram não apenas pela excelência técnica, mas também pela criatividade e impacto social de suas propostas. Esta conquista reforça nosso compromisso com a educação em STEM e com a formação de jovens talentos em robótica e tecnologia.',
      icone: '🤖',
      categoria: 'Competição'
    },
    {
      id: 4,
      data: '03/03/2023',
      titulo: 'IPEVolante',
      resumo: 'IPEVolante visita a cidade de Cocalzinho de Goiás e o Distrito de Girassol',
      conteudoCompleto: 'O projeto IPEVolante levou inovação e tecnologia diretamente às comunidades de Cocalzinho de Goiás e Distrito de Girassol, promovendo inclusão digital e acesso ao conhecimento tecnológico. Durante a visita, nossa equipe móvel ofereceu oficinas práticas de programação, robótica educacional e empreendedorismo digital para estudantes e professores da região. Mais de 300 pessoas participaram das atividades, que incluíram demonstrações de impressão 3D, desenvolvimento de aplicativos móveis e conceitos básicos de inteligência artificial. O projeto visa democratizar o acesso à educação tecnológica, especialmente em regiões com menor infraestrutura educacional, contribuindo para a formação de uma nova geração de inovadores no interior de Goiás.',
      icone: '🚌',
      categoria: 'Projeto Social'
    },
    {
      id: 5,
      data: '02/02/2023',
      titulo: 'IPE Volante na Cidade de Goiás',
      resumo: 'IPE Volante retorna à Cidade de Goiás para mais uma edição',
      conteudoCompleto: 'O IPE Volante retornou à histórica Cidade de Goiás para mais uma edição especial do projeto de extensão universitária. Esta visita fez parte de uma série de ações continuadas para levar conhecimento tecnológico às cidades do interior goiano. Durante três dias intensivos, estudantes e educadores locais participaram de workshops sobre tecnologias emergentes, desenvolvimento sustentável e inovação social. A programação incluiu oficinas de maker space, palestras sobre empreendedorismo digital e atividades práticas com drones e sensores IoT. A iniciativa fortaleceu a parceria entre a UFG e a comunidade local, criando uma rede de colaboração que continua ativa através de projetos desenvolvidos pelos próprios participantes.',
      icone: '📍',
      categoria: 'Extensão'
    },
    {
      id: 6,
      data: '02/02/2023',
      titulo: 'II Oficina Maker',
      resumo: 'Princípios básicos de impressão de objetos com caneta 3D',
      conteudoCompleto: 'A II Oficina Maker foi um grande sucesso, reunindo entusiastas de tecnologia e educadores para explorar os princípios básicos de impressão 3D com canetas especializadas. Durante o evento, os participantes aprenderam desde conceitos fundamentais de modelagem tridimensional até técnicas avançadas de criação de objetos funcionais. A oficina abordou temas como design thinking, prototipagem rápida e fabricação digital, proporcionando uma experiência hands-on com as mais modernas ferramentas de criação. Mais de 80 participantes tiveram a oportunidade de criar seus próprios projetos, desenvolvendo desde joias personalizadas até componentes para projetos de robótica. O evento consolidou o IPELab como um centro de referência em cultura maker e fabricação digital na região Centro-Oeste.',
      icone: '🖨️',
      categoria: 'Workshop'
    }
  ];

  constructor(private publicacaoService: PublicacaoService) {}

  ngOnInit() {
    this.carregarPublicacoesTimeline();
  }

  carregarPublicacoesTimeline() {
    this.publicacaoService.getPublicacoesAtivas().subscribe({
      next: (todasPublicacoes) => {

        this.publicacoesTimeline = this.mostrarTodasPublicacoes
          ? todasPublicacoes
          : todasPublicacoes.slice(0, 3);
      },
      error: (error) => console.error('Erro ao carregar publicações:', error)
    });
  }

  toggleMostrarTodas() {
    this.mostrarTodasPublicacoes = !this.mostrarTodasPublicacoes;
    this.carregarPublicacoesTimeline();
  }

  curtirPublicacao(id: number) {
    this.publicacaoService.curtirPublicacao(id).subscribe({
      next: () => this.carregarPublicacoesTimeline(),
      error: (error) => console.error('Erro ao curtir publicação:', error)
    });
  }

  formatarData(data: string): string {
    const agora = new Date();
    const dataPublicacao = new Date(data);
    const diffTime = agora.getTime() - dataPublicacao.getTime();
    const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24));
    const diffHours = Math.floor(diffTime / (1000 * 60 * 60));
    const diffMinutes = Math.floor(diffTime / (1000 * 60));

    if (diffDays > 7) {
      return dataPublicacao.toLocaleDateString('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
      });
    } else if (diffDays > 0) {
      return `${diffDays} dia${diffDays > 1 ? 's' : ''} atrás`;
    } else if (diffHours > 0) {
      return `${diffHours} hora${diffHours > 1 ? 's' : ''} atrás`;
    } else if (diffMinutes > 0) {
      return `${diffMinutes} minuto${diffMinutes > 1 ? 's' : ''} atrás`;
    } else {
      return 'Agora mesmo';
    }
  }

  truncarTexto(texto: string, limite: number): string {
    if (texto.length <= limite) return texto;
    return texto.substring(0, limite) + '...';
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


  formatarDataCompleta(data: string): string {
    return new Date(data).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  contarPalavras(texto: string): number {
    return texto.trim().split(/\s+/).filter(palavra => palavra.length > 0).length;
  }

  calcularTempoLeitura(texto: string): number {
    const palavras = this.contarPalavras(texto);
    return Math.ceil(palavras / 200); // 200 palavras por minuto
  }

  getStatusText(status: string): string {
    const statusMap: { [key: string]: string } = {
      'ativa': 'Publicada',
      'rascunho': 'Rascunho',
      'arquivada': 'Arquivada'
    };
    return statusMap[status] || status;
  }

  getStatusClass(status: string): string {
    return `status-${status}`;
  }


  abrirPopupInovacao(card: CardInovacao): void {
    this.cardSelecionado = card;
    this.mostrarPopupInovacao = true;

    document.body.style.overflow = 'hidden';
  }

  fecharPopupInovacao(): void {
    this.mostrarPopupInovacao = false;
    this.cardSelecionado = null;

    document.body.style.overflow = 'auto';
  }


  fecharPopupInovacaoOverlay(event: Event): void {
    if (event.target === event.currentTarget) {
      this.fecharPopupInovacao();
    }
  }


  getCardById(id: number): CardInovacao | undefined {
    return this.cardsInovacao.find(card => card.id === id);
  }
}
