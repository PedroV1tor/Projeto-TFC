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
      name: 'Análises Laboratoriais',
      description: 'Análises completas com resultados precisos e confiáveis',
      features: ['Análises químicas', 'Microbiologia', 'Toxicologia', 'Controle de qualidade'],
      price: 'A partir de R$ 150',
      icon: '🧪'
    },
    {
      id: 2,
      name: 'Consultoria Técnica',
      description: 'Consultoria especializada para otimização de processos',
      features: ['Implementação de sistemas', 'Treinamentos', 'Auditorias', 'Validação'],
      price: 'Sob consulta',
      icon: '📋'
    },
    {
      id: 3,
      name: 'Desenvolvimento P&D',
      description: 'Pesquisa e desenvolvimento de soluções customizadas',
      features: ['Projetos customizados', 'Prototipagem', 'Testes piloto', 'Validação'],
      price: 'Projeto personalizado',
      icon: '⚗️'
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
