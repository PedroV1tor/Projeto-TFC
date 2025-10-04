import { Component, OnInit, OnDestroy, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';

interface TeamMember {
  id: number;
  name: string;
  position: string;
  description: string;
  image: string;
  specialties: string[];
  social: {
    linkedin?: string;
    email?: string;
  };
}

interface TimelineEvent {
  year: string;
  title: string;
  description: string;
  icon: string;
  color: string;
}

interface Value {
  id: number;
  title: string;
  description: string;
  icon: string;
  color: string;
}

@Component({
  selector: 'app-sobre-nos',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sobre-nos.component.html',
  styleUrl: './sobre-nos.component.scss'
})
export class SobreNosComponent implements OnInit, OnDestroy {
  @ViewChild('timelineContainer', { static: false }) timelineContainer!: ElementRef;

  activeTeamMember = 0;
  isTimelineVisible = false;
  currentValueIndex = 0;
  valueRotationInterval: any;
  particleInterval: any;
  particles: any[] = [];

  teamMembers: TeamMember[] = [
    {
      id: 1,
      name: 'Dr. Ana Silva',
      position: 'Diretora Científica',
      description: 'Especialista em biotecnologia com mais de 15 anos de experiência em pesquisa e desenvolvimento.',
      image: '/pessoa1.png',
      specialties: ['Biotecnologia', 'Pesquisa & Desenvolvimento', 'Gestão de Projetos'],
      social: {
        linkedin: 'https://linkedin.com/in/ana-silva',
        email: 'ana.silva@inovalab.com'
      }
    },
    {
      id: 2,
      name: 'Dr. Carlos Santos',
      position: 'Gerente de Qualidade',
      description: 'Doutor em Química Analítica, responsável por garantir os mais altos padrões de qualidade.',
      image: '/pessoa1.png',
      specialties: ['Química Analítica', 'Controle de Qualidade', 'Certificações'],
      social: {
        linkedin: 'https://linkedin.com/in/carlos-santos',
        email: 'carlos.santos@inovalab.com'
      }
    },
    {
      id: 3,
      name: 'Dra. Maria Oliveira',
      position: 'Especialista em Inovação',
      description: 'Mestra em Engenharia Biomédica, lidera nossa equipe de inovação tecnológica.',
      image: '/pessoa1.png',
      specialties: ['Engenharia Biomédica', 'Inovação', 'Automação'],
      social: {
        linkedin: 'https://linkedin.com/in/maria-oliveira',
        email: 'maria.oliveira@inovalab.com'
      }
    },
    {
      id: 4,
      name: 'Dr. João Costa',
      position: 'Diretor de Operações',
      description: 'Especialista em gestão operacional e otimização de processos laboratoriais.',
      image: '/pessoa1.png',
      specialties: ['Gestão Operacional', 'Otimização', 'Logística'],
      social: {
        linkedin: 'https://linkedin.com/in/joao-costa',
        email: 'joao.costa@inovalab.com'
      }
    }
  ];

  timeline: TimelineEvent[] = [
    {
      year: '2015',
      title: 'Fundação',
      description: 'O INOVALAB foi fundado com a missão de revolucionar os serviços laboratoriais no Brasil.',
      icon: '🚀',
      color: '#FF6B6B'
    },
    {
      year: '2017',
      title: 'Primeira Certificação',
      description: 'Obtivemos nossa primeira certificação ISO 9001, consolidando nosso compromisso com a qualidade.',
      icon: '🏆',
      color: '#4ECDC4'
    },
    {
      year: '2019',
      title: 'Expansão Nacional',
      description: 'Expandimos nossas operações para 5 estados, atendendo mais de 100 clientes.',
      icon: '📈',
      color: '#45B7D1'
    },
    {
      year: '2021',
      title: 'Inovação Tecnológica',
      description: 'Lançamos nossa plataforma digital integrada para gestão laboratorial.',
      icon: '💻',
      color: '#96CEB4'
    },
    {
      year: '2023',
      title: 'Reconhecimento',
      description: 'Recebemos o prêmio de "Melhor Laboratório de Análises" da região.',
      icon: '🥇',
      color: '#FECA57'
    },
    {
      year: '2025',
      title: 'Futuro',
      description: 'Continuamos inovando e expandindo nossos serviços para toda América Latina.',
      icon: '🌟',
      color: '#FF9FF3'
    }
  ];



  ngOnInit() {
    this.createParticles();
    this.observeTimeline();
  }

  ngOnDestroy() {
    if (this.valueRotationInterval) {
      clearInterval(this.valueRotationInterval);
    }
    if (this.particleInterval) {
      clearInterval(this.particleInterval);
    }
  }
  createParticles() {
    for (let i = 0; i < 50; i++) {
      this.particles.push({
        id: i,
        x: Math.random() * 100,
        y: Math.random() * 100,
        size: Math.random() * 4 + 1,
        speed: Math.random() * 2 + 0.5,
        opacity: Math.random() * 0.5 + 0.1
      });
    }

    this.animateParticles();
  }

  animateParticles() {
    this.particleInterval = setInterval(() => {
      this.particles.forEach(particle => {
        particle.y -= particle.speed;
        if (particle.y < -5) {
          particle.y = 105;
          particle.x = Math.random() * 100;
        }
      });
    }, 100);
  }

  observeTimeline() {

    setTimeout(() => {
      this.isTimelineVisible = true;
    }, 500);
  }

  trackByFn(index: number, item: any) {
    return item.id || index;
  }


  prevTeamMember() {
    if (this.activeTeamMember > 0) {
      this.activeTeamMember--;
    }
  }

  nextTeamMember() {
    if (this.activeTeamMember < this.teamMembers.length - 1) {
      this.activeTeamMember++;
    }
  }

  selectTeamMember(index: number) {
    this.activeTeamMember = index;
  }
}
