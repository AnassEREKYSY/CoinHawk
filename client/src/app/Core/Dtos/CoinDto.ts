export interface CoinDto {
    id: string;
    name: string;
    symbol: string;
    currentPrice?: number;
    marketCap?: number;
    totalVolume?: number;
    thumb?: string;
    large?: string;
    marketChart?: number[][];
}