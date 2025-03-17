import { Component, Input, OnInit } from '@angular/core';
import { NewsArticleDto } from '../../../Core/Dtos/NewsArticleDto';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-coin-news',
  imports:[DatePipe],
  templateUrl: './coin-news.component.html',
  styleUrls: ['./coin-news.component.scss']
})
export class CoinNewsComponent{
  
  @Input() news!: NewsArticleDto;

  goToDetails(data: NewsArticleDto) {
    window.open(data.url, '_blank');
  }  
}
