import { Component, Input, OnInit } from '@angular/core';
import { NewsArticleDto } from '../../../Core/Dtos/NewsArticleDto';
import { Router } from '@angular/router';

@Component({
  selector: 'app-coin-news',
  templateUrl: './coin-news.component.html',
  styleUrls: ['./coin-news.component.scss']
})
export class CoinNewsComponent implements OnInit{
  @Input() news!: NewsArticleDto;

  constructor(private router: Router) {}

  ngOnInit(): void {
    console.log("AERRRRR"+this.news)
  }
  goToDetails(data: NewsArticleDto) {
    this.router.navigateByUrl('/news-details', { state: data });
  }
}
