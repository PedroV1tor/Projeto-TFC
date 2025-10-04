import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

interface ContactMethod {
  id: number;
  title: string;
  description: string;
  icon: string;
  value: string;
  color: string;
  link?: string;
}

interface FAQ {
  id: number;
  question: string;
  answer: string;
  isOpen: boolean;
}

interface Office {
  id: number;
  name: string;
  address: string;
  phone: string;
  email: string;
  coordinates: { lat: number; lng: number };
  isMainOffice: boolean;
}

@Component({
  selector: 'app-contato',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './contato.component.html',
  styleUrl: './contato.component.scss'
})
export class ContatoComponent implements OnInit {
  currentOfficeIndex = 0;
  mapAnimationInterval: any;

  contactMethods: ContactMethod[] = [
    {
      id: 1,
      title: 'Telefone',
      description: 'Ligue para nós durante o horário comercial',
      icon: '📞',
      value: '+55 (11) 3456-7890',
      color: '#4CAF50',
      link: 'tel:+551134567890'
    },
    {
      id: 2,
      title: 'Email',
      description: 'Envie sua mensagem para nosso email principal',
      icon: '✉️',
      value: 'contato@inovalab.com',
      color: '#2196F3',
      link: 'mailto:contato@inovalab.com'
    },
    {
      id: 3,
      title: 'WhatsApp',
      description: 'Fale conosco via WhatsApp',
      icon: '💬',
      value: '+55 (11) 99999-8888',
      color: '#25D366',
      link: 'https://wa.me/5511999998888'
    },
    {
      id: 4,
      title: 'Endereço',
      description: 'Visite nosso escritório principal',
      icon: '📍',
      value: 'Av. Paulista, 1000 - São Paulo, SP',
      color: '#FF9800'
    }
  ];

  offices: Office[] = [
    {
      id: 1,
      name: 'INOVALAB',
      address: 'Av. Paulista, 1000 - Bela Vista, São Paulo - SP, 01310-100',
      phone: '+55 (11) 3456-7890',
      email: 'contato@inovalab.com',
      coordinates: { lat: -23.5505, lng: -46.6333 },
      isMainOffice: true
    }
  ];

  faqs: FAQ[] = [
    {
      id: 1,
      question: 'Quais tipos de análises vocês realizam?',
      answer: 'Realizamos uma ampla gama de análises, incluindo análises químicas, microbiológicas, toxicológicas, controle de qualidade e muito mais. Nossa equipe especializada está preparada para atender diversas áreas e segmentos.',
      isOpen: false
    },
    {
      id: 2,
      question: 'Qual é o prazo para entrega dos resultados?',
      answer: 'O prazo varia de acordo com o tipo de análise solicitada. Análises simples podem ser entregues em 24-48 horas, enquanto análises mais complexas podem levar de 5 a 15 dias úteis. Consulte nossos especialistas para prazos específicos.',
      isOpen: false
    },
    {
      id: 3,
      question: 'Vocês atendem em todo o Brasil?',
      answer: 'Sim! Nosso escritório em São Paulo atende clientes em todo o território nacional através de nossa rede de parceiros certificados.',
      isOpen: false
    },
    {
      id: 4,
      question: 'Como posso solicitar um orçamento?',
      answer: 'Você pode solicitar um orçamento através deste formulário de contato, ligando para um de nossos telefones, enviando um email ou acessando nossa plataforma online através da área do cliente.',
      isOpen: false
    },
    {
      id: 5,
      question: 'Vocês oferecem consultoria técnica?',
      answer: 'Sim! Oferecemos consultoria técnica especializada para implementação de sistemas de qualidade, otimização de processos, treinamentos e auditorias. Nossa equipe de especialistas está pronta para ajudar.',
      isOpen: false
    }
  ];

  constructor() {}

  ngOnInit() {
    this.startMapAnimation();
  }

  ngOnDestroy() {
    if (this.mapAnimationInterval) {
      clearInterval(this.mapAnimationInterval);
    }
  }

  startMapAnimation() {




  }

  selectOffice(index: number) {
    this.currentOfficeIndex = index;
  }

  toggleFAQ(index: number) {
    this.faqs[index].isOpen = !this.faqs[index].isOpen;
  }

  get currentOffice() {
    return this.offices[this.currentOfficeIndex];
  }
}
