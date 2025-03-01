import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CoinService } from '../../Core/Services/coin.service';
import { CoinDto } from '../../Core/Dtos/CoinDto';
import { Chart } from 'chart.js/auto';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-coin-details',
  imports: [
    CommonModule
  ],
  templateUrl: './coin-details.component.html',
  styleUrls: ['./coin-details.component.scss']
})
export class CoinDetailsComponent implements OnInit {

  coinName: string = '';
  coinData: CoinDto | undefined;
  marketChart: number[][] = [];
  selectedDuration: number = 7;
  chart: any;

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

  fetchCoinData(): void {
    this.coinService.getCoinData(this.coinName).subscribe(
      data => this.coinData = data,
      error => console.error('Failed to load coin data', error)
    );
  }

  changeDuration(days: number): void {
    this.selectedDuration = days;
    this.fetchMarketChart();
  }

  fetchMarketChart(): void {
    this.coinService.getCoinMarketChart(this.coinName, this.selectedDuration).subscribe(
      data => {
        this.marketChart = data;
        this.updateChart();
      },
      error => console.error('Failed to load market chart', error)
    );
  }

  updateChart(): void {
    const canvas = document.getElementById('marketChart') as HTMLCanvasElement;
    if (this.chart) {
      this.chart.destroy(); 
    }

    this.chart = new Chart(canvas, {
      type: 'line',
      data: {
        labels: this.marketChart.map(entry => new Date(entry[0]).toLocaleDateString()),
        datasets: [{
          label: `${this.coinData?.name} Price`,
          data: this.marketChart.map(entry => entry[1]),
          borderColor: '#1db954',
          backgroundColor: 'rgba(29, 185, 84, 0.2)',
          fill: true
        }]
      },
      options: {
        responsive: true,
        scales: {
          x: { display: true },
          y: { display: true }
        }
      }
    });
  }
}
