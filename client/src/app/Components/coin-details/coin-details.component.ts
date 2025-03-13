import {Component,ElementRef,OnInit,AfterViewInit,ViewChild} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CoinService } from '../../Core/Services/coin.service';
import { CoinDto } from '../../Core/Dtos/CoinDto';
import { Chart } from 'chart.js/auto';
import {CandlestickController,CandlestickElement,OhlcController,OhlcElement} from 'chartjs-chart-financial';
Chart.register(CandlestickController, CandlestickElement, OhlcController, OhlcElement);
import 'chartjs-adapter-date-fns';
import { CommonModule } from '@angular/common';
import { CandlestickPoint } from '../../Core/Dtos/CandlestickPoint';

@Component({
  selector: 'app-coin-details',
  imports: [
    CommonModule
  ],
  templateUrl: './coin-details.component.html',
  styleUrls: ['./coin-details.component.scss']
})
export class CoinDetailsComponent implements OnInit, AfterViewInit {

  @ViewChild('marketChartCanvas', { static: false })
  chartCanvas!: ElementRef<HTMLCanvasElement>;

  coinName = '';
  coinData?: CoinDto;
  marketChart: CandlestickPoint[] = [];
  selectedDuration = 7;
  chart?: Chart;

  constructor(
    private route: ActivatedRoute,
    private coinService: CoinService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.coinName = params.get('coinName') || '';
      if (this.coinName) {
        this.fetchCoinData();
        this.fetchMarketChart();
      }
    });
  }

  ngAfterViewInit(): void {
    if (this.marketChart?.length) {
      this.updateChartAsCandlestick();
    }
  }

  fetchCoinData(): void {
    this.coinService.getCoinData(this.coinName).subscribe({
      next: (data) => {
        this.coinData = data;
      },
      error: (err) => {
        console.error('Failed to load coin data', err);
      }
    });
  }

  changeDuration(days: number): void {
    this.selectedDuration = days;
    this.fetchMarketChart();
  }

  fetchMarketChart(): void {
    this.coinService.getCoinOhlc(this.coinName, this.selectedDuration).subscribe({
      next: (data) => {
        console.log('OHLC data', data);
        this.marketChart = data.map(([time, open, high, low, close]) => ({
          x: time,
          o: open,
          h: high,
          l: low,
          c: close
        }));

        if (this.chartCanvas?.nativeElement) {
          this.updateChartAsCandlestick();
        }
      },
      error: (err) => {
        console.error('Failed to load candlestick data', err);
      }
    });
  }

  updateChartAsCandlestick(): void {
    if (!this.chartCanvas?.nativeElement) {
      console.error('Canvas element not found');
      return;
    }

    if (this.chart) {
      this.chart.destroy();
    }

    this.chart = new Chart(this.chartCanvas.nativeElement, {
      type: 'candlestick',
      data: {
        datasets: [
          {
            label: `${this.coinData?.name} Price`,
            data: this.marketChart,

            borderColor: (ctx) => {
              const candle = ctx.raw as CandlestickPoint;
              if (!candle) return '#999999';
              return candle.c > candle.o ? '#1db954' : '#ff0000';
            },
            backgroundColor: (ctx) => {
              const candle = ctx.raw as CandlestickPoint;
              if (!candle) return 'rgba(153,153,153,0.2)';
              return candle.c > candle.o
                ? 'rgba(29,185,84,0.2)'
                : 'rgba(255,0,0,0.2)';
            }
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          x: {
            type: 'time'
          },
          y: {
            beginAtZero: false
          }
        }
      }
    });
  }
}
