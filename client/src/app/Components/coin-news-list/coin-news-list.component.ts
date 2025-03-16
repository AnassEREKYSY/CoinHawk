import { AfterViewChecked, AfterViewInit, ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { NewsArticleDto } from '../../Core/Dtos/NewsArticleDto';
import { SnackBarService } from '../../Core/Services/snack-bar.service';
import { CoinNewsService } from '../../Core/Services/coin-news.service';
import { Router } from '@angular/router';
import { SlickCarouselComponent, SlickCarouselModule } from 'ngx-slick-carousel';
import { CommonModule } from '@angular/common';
import { CoinNewsComponent } from './coin-news/coin-news.component';

@Component({
  selector: 'app-coin-news-list',
  imports: [
    CoinNewsComponent,
    SlickCarouselModule,
    CommonModule
  ],
  templateUrl: './coin-news-list.component.html',
  styleUrls: ['./coin-news-list.component.scss']
})
export class CoinNewsListComponent implements OnInit, AfterViewInit, AfterViewChecked {
  @ViewChild('slickModal') slickModal!: SlickCarouselComponent;
  news: NewsArticleDto[] = [];
  isLoading: boolean = true;
  isSlickInitialized = false;
  slideConfig = {
    slidesToShow: 3,
    slidesToScroll: 1,
    dots: true,
    infinite: true,
    autoplay: false
  };
  constructor(
    private snackBarService: SnackBarService,
    private coinNewsService: CoinNewsService,
    private router: Router,
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
        console.log(this.news)
        this.isLoading = false;
        setTimeout(() => {
          if (this.slickModal && this.slickModal.$instance) {
            this.slickModal.slickGoTo(0);
          }
        }, 500);
      },
      error: (error: string) => {
        this.snackBarService.error('Error loading news: ' + error);
        this.isLoading = false;
      }
    });
  }
  goToDetails(data: NewsArticleDto): void {
    this.router.navigate(['/news-details'], { state: data });
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
