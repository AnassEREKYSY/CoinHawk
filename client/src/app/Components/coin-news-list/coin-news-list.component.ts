import { AfterViewChecked, AfterViewInit, ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { NewsArticleDto } from '../../Core/Dtos/NewsArticleDto';
import { SnackBarService } from '../../Core/Services/snack-bar.service';
import { CoinNewsService } from '../../Core/Services/coin-news.service';
import { SlickCarouselComponent, SlickCarouselModule } from 'ngx-slick-carousel';
import { CommonModule } from '@angular/common';
import { CoinNewsComponent } from './coin-news/coin-news.component';
import { MatIcon, MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-coin-news-list',
  imports: [
    CoinNewsComponent,
    SlickCarouselModule,
    CommonModule,
    MatIcon
  ],
  templateUrl: './coin-news-list.component.html',
  styleUrls: ['./coin-news-list.component.scss']
})
export class CoinNewsListComponent implements OnInit, AfterViewInit, AfterViewChecked {
  @ViewChild('slickModal') slickModal!: SlickCarouselComponent;
  news: NewsArticleDto[] = [];
  isSlickInitialized = false;
  slideConfig = {
    slidesToShow: 1,
    slidesToScroll: 1,
    infinite: true,
    arrows: false,
    autoplay: true,
    autoplaySpeed: 3000,
  };  
  constructor(
    private snackBarService: SnackBarService,
    private coinNewsService: CoinNewsService,
    private cdr: ChangeDetectorRef
  ) {}
  ngOnInit(): void {
    this.getFollowedCoinNews();
  }
  ngAfterViewInit(): void {}
  ngAfterViewChecked(): void {
    if (!this.isSlickInitialized && this.slickModal && this.slickModal.$instance) {
      this.isSlickInitialized = true;
    }
  }
  async getFollowedCoinNews(): Promise<void> {
    (await this.coinNewsService.getFollowedCoinsNews()).subscribe({
      next: (response: NewsArticleDto[]) => {
        this.news = response;
        setTimeout(() => {
          if (this.slickModal && this.slickModal.$instance) {
            this.slickModal.slickGoTo(0);
          }
        }, 500);
      },
      error: (error: string) => {
        this.snackBarService.error('Error loading news: ' + error);
      }
    });
  }
  prev(): void {
    if (this.isSlickInitialized) {
      this.slickModal.slickPrev();
    }
  }
  next(): void {
    if (this.isSlickInitialized) {
      this.slickModal.slickNext();
    }
  }
}
