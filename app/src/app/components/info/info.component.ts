import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

interface InfoCard {
  id: number;
  title: string;
  description: string;
  icon: string;
  color: string;
  details: string[];
}

interface ServiceInfo {
  id: number;
  name: string;
  description: string;
  features: string[];
  price: string;
  icon: string;
}

@Component({
  selector: 'app-info',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './info.component.html',
  styleUrl: './info.component.scss'
})
export class InfoComponent implements OnInit, OnDestroy {
  currentCardIndex = 0;
  autoSlideInterval: any;
  isAnimating = false;
  animatedStats = {
    clients: 0,
    projects: 0,
    success: 0
  };

  infoCards: InfoCard[] = [
    {
      id: 1,
      title: 'Inovação Tecnológica',
      description: 'Desenvolvemos soluções inovadoras para laboratórios e pesquisa científica.',
      icon: '🔬',
      color: '#4A90E2',
      details: [
        'Equipamentos de última geração',
        'Tecnologia de ponta em análises',
        'Automação de processos laboratoriais',
        'Integração com sistemas existentes'
      ]
    },
    {
      id: 2,
      title: 'Qualidade Certificada',
      description: 'Todos nossos serviços seguem rigorosos padrões de qualidade internacional.',
      icon: '✅',
      color: '#50C878',
      details: [
        'Certificação ISO 9001:2015',
        'Acreditação pela ABNT',
        'Controle de qualidade rigoroso',
        'Rastreabilidade completa'
      ]
    },
    {
      id: 3,
      title: 'Equipe Especializada',
      description: 'Nossa equipe é formada por profissionais altamente qualificados.',
      icon: '👥',
      color: '#FF6B6B',
      details: [
        'Doutores e mestres especializados',
        'Experiência em diversas áreas',
        'Atualização constante',
        'Atendimento personalizado'
      ]
    },
    {
      id: 4,
      title: 'Sustentabilidade',
      description: 'Comprometidos com práticas sustentáveis e responsabilidade ambiental.',
      icon: '🌱',
      color: '#4ECDC4',
      details: [
        'Processos eco-friendly',
        'Redução de resíduos',
        'Energia renovável',
        'Descarte responsável'
      ]
    }
  ];

  services: ServiceInfo[] = [
    {
      id: 1,
      name: 'TM-340',
      description: 'Impressora profissional de alta performance para produção de materiais de marketing e impressões CAD com qualidade superior. Ideal para ambientes corporativos que demandam produtividade e confiabilidade.',
      features: [
        'Qualidade profissional de impressão',
        'Produtividade confiável e constante',
        'Interface intuitiva e fácil de usar',
        'Processos sustentáveis',
        'Linhas nítidas e cores vibrantes',
        'Suporte a impressões CAD precisas'
      ],
      price: 'Sob consulta',
      icon: '🖨️'
    },
    {
      id: 2,
      name: 'Máquina a Laser MF139',
      description: 'Equipamento laser de precisão para corte e gravação em diversos materiais. Área de trabalho de 1300x900mm permite projetos de todos os tamanhos com acabamento profissional.',
      features: [
        'Área de trabalho: 1300x900mm',
        'Cortes limpos e precisos',
        'Gravações detalhadas',
        'Múltiplos materiais: MDF, acrílico, couro, tecido, madeira',
        'Aplicações versáteis',
        'Alta durabilidade e confiabilidade'
      ],
      price: 'Sob consulta',
      icon: '⚡'
    },
    {
      id: 3,
      name: 'Creality K1C',
      description: 'Impressora 3D FDM/FFF de alta velocidade para protótipos e peças técnicas. Suporte a filamentos avançados e tecnologia de ponta para resultados profissionais.',
      features: [
        'Tecnologia FDM/FFF avançada',
        'Alta velocidade de impressão',
        'Suporte a filamentos especiais',
        'Ideal para protótipos',
        'Peças técnicas de qualidade',
        'Interface amigável',
        'Precisão dimensional'
      ],
      price: 'Sob consulta',
      icon: '🔧'
    },
    {
      id: 4,
      name: 'Máquina de Corte Nagano',
      description: 'Cortadora elétrica de papel e cartões para acabamento gráfico de alta precisão. Equipamento profissional para corte automatizado com tecnologia avançada de controle.',
      features: [
        'Corte elétrico automatizado',
        'Alta precisão no acabamento',
        'Ideal para papel e cartões',
        'Tecnologia de controle avançada',
        'Aplicações em acabamento gráfico',
        'Eficiência e rapidez no processo',
        'Qualidade profissional de corte'
      ],
      price: 'Sob consulta',
      icon: '✂️'
    }
  ];

  stats = {
    clients: { target: 500, current: 0, suffix: '+' },
    projects: { target: 1200, current: 0, suffix: '+' },
    success: { target: 98, current: 0, suffix: '%' }
  };

  ngOnInit() {
    this.startAutoSlide();
    this.animateStats();
  }

  ngOnDestroy() {
    if (this.autoSlideInterval) {
      clearInterval(this.autoSlideInterval);
    }
  }

  startAutoSlide() {
    this.autoSlideInterval = setInterval(() => {
      this.nextCard();
    }, 5000);
  }

  nextCard() {
    if (this.isAnimating) return;
    this.isAnimating = true;
    this.currentCardIndex = (this.currentCardIndex + 1) % this.infoCards.length;
    setTimeout(() => this.isAnimating = false, 300);
  }

  prevCard() {
    if (this.isAnimating) return;
    this.isAnimating = true;
    this.currentCardIndex = this.currentCardIndex === 0 ? this.infoCards.length - 1 : this.currentCardIndex - 1;
    setTimeout(() => this.isAnimating = false, 300);
  }

  goToCard(index: number) {
    if (this.isAnimating || index === this.currentCardIndex) return;
    this.isAnimating = true;
    this.currentCardIndex = index;
    setTimeout(() => this.isAnimating = false, 300);
  }

  animateStats() {
    const duration = 2000;
    const intervals = 50;
    const steps = duration / intervals;

    Object.keys(this.stats).forEach(key => {
      const stat = this.stats[key as keyof typeof this.stats];
      const increment = stat.target / steps;
      let current = 0;

      const timer = setInterval(() => {
        current += increment;
        if (current >= stat.target) {
          current = stat.target;
          clearInterval(timer);
        }
        stat.current = Math.floor(current);
      }, intervals);
    });
  }

  onCardHover(index: number) {
    clearInterval(this.autoSlideInterval);
  }

  onCardLeave() {
    this.startAutoSlide();
  }
}
